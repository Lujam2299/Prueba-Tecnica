# API de categorías y productos

Prueba técnica de entrevista. Se desarrollará por etapas pequeñas con ASP.NET Core Minimal APIs (.NET 10), Entity Framework Core 10, SQLite y pruebas BDD con Reqnroll y NUnit.

## Estado

Creación, consulta y edición de categorías con persistencia SQLite, nombre requerido y unicidad implementadas. Suite actual: 37 casos BDD y 4 comprobaciones técnicas SQLite aprobadas. Eliminaciones y productos siguen pendientes. Consulta `PROGRESS.md` para retomar el trabajo.

Consulta [PLAN.md](PLAN.md) para ver el paso a paso, las etapas completadas y cuál sigue.

## Requisitos

- SDK de .NET 10 (permite compilar y ejecutar pruebas).
- Git. VS Code es el editor disponible en este equipo.

Verificación del entorno:

```powershell
dotnet --info
git --version
```

## Forma de trabajo

Por cada comportamiento: escenario Gherkin en español, prueba HTTP que falla por la funcionalidad pendiente, implementación mínima, prueba que pasa y commit pequeño. La solución tendrá un proyecto para la API y otro para las pruebas. Estas usarán bases aisladas.

## Decisiones y supuestos

- SQLite en archivo para conservar datos entre reinicios; esquema administrado con migraciones de EF Core.
- Nombres obligatorios, con espacios externos eliminados; rechazar nombres vacíos o solo espacios.
- En creación y edición de categorías, `string.IsNullOrWhiteSpace` rechaza nombres ausentes, null o compuestos solo por blancos (también tabulaciones y saltos de línea). `Trim()` elimina blancos externos y conserva los internos. Rechazo: 400 con `application/problem+json`, `errors.Name` y mensaje `El nombre es obligatorio.`; no se crean ni modifican recursos.
- Categorías con nombre único ignorando mayúsculas; conservar la diferencia entre letras con y sin acento. Estrategia implementada y probada: comparación SQLite personalizada `CATEGORY_NAME`, que aplica `Trim()`, normalización Unicode NFC, `ToUpperInvariant()` y NFC nuevamente; compara el resultado de forma ordinal. Se conserva en Name el texto recibido sin blancos externos. No es una comparación lingüística para todas las culturas ni una eliminación de acentos.
- La columna Name usa esa comparación y tiene un índice único. Tanto la consulta previa como el índice aplican la misma regla. `Electrónica`, `ELECTRÓNICA` y la ó formada con letra más acento combinante son equivalentes; `Electronica` es diferente. Los espacios internos son significativos. SQLite `NOCASE` solo cubre ASCII por defecto; véase la [documentación oficial](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/collation).
- Duplicados en creación y edición: 409 Problem Details con detail `Ya existe una categoría con ese nombre.`. En edición se excluye la propia categoría por identificador. Se captura también la infracción del índice único (SQLite 2067); otras fallas de base no se convierten en duplicados.
- PUT usa `CategoryRequest`, el mismo contrato de entrada que POST, y reemplaza nombre y descripción; omitir la descripción la deja en null. Conserva el identificador y responde 200 con el recurso actualizado. Primero comprueba existencia; una categoría inexistente devuelve 404 aunque el nombre enviado sea inválido. GET y PUT inexistentes devuelven Problem Details con detail `No existe una categoría con el identificador indicado.`. Los rechazos conservan las categorías anteriores.
- Los nombres de productos pueden repetirse. La descripción es opcional.
- No se han establecido longitudes máximas adicionales.
- No permitir eliminar categorías con productos; mensaje obligatorio: `Esta categoria no se puede eliminar porque tiene productos asignados`.
- Respuestas: GET 200/404, POST 201, PUT 200/404, DELETE 204/404; validaciones 400 y conflictos 409. Los errores usarán un formato consistente.
- Entrega: repositorio, README y video. Deploy opcional después de completar los requisitos principales.

Pendiente confirmar con el contacto: interfaz de documentación suficiente para demostrar la API, tratamiento de espacios, longitudes máximas y repetición de nombres de productos.

## Ejecución y pruebas

Desde la raíz del repositorio:

```powershell
dotnet restore Catalogo.slnx
dotnet tool restore
dotnet build Catalogo.slnx --no-restore
dotnet test Catalogo.slnx --no-restore
```

`Catalogo.slnx` agrupa la API y las pruebas. `global.json` selecciona el SDK 10.0.401 y permite parches posteriores de esa misma banda. Los paquetes del proyecto de pruebas tienen versiones explícitas: Reqnroll.NUnit 3.3.4, NUnit 4.6.1, NUnit3TestAdapter 6.3.1 y Microsoft.NET.Test.Sdk 18.10.1. Los archivos generados de Reqnroll se ubicarán en `obj/` para excluirlos de Git.

Si una terminal anterior a la instalación no reconoce `dotnet`, abre otra o usa `& 'C:\Program Files\dotnet\dotnet.exe'` en lugar de `dotnet`.

Las pruebas usan Microsoft.AspNetCore.Mvc.Testing 10.0.12 y WebApplicationFactory: levantan la API en un servidor de pruebas en memoria y realizan peticiones HTTP con HttpClient. Cada escenario utiliza su propio archivo SQLite temporal, sin conexión compartida; lo elimina al finalizar. El reinicio cierra la aplicación y abre otra conservando el mismo archivo dentro del escenario. No hace falta arrancar la API manualmente.

EF Core SQLite y Design están fijados en 10.0.12; `dotnet-tools.json` fija la herramienta local dotnet-ef en esa misma versión. `CatalogoDbContext` define el modelo de almacenamiento. La migración InitialCategories, incluida en Git, crea la tabla; `Program.cs` aplica migraciones pendientes al arrancar. Esta elección simplifica una demostración local; no es una estrategia de despliegue multiinstancia.

La migración UniqueCategoryNames configura la comparación y el índice único. Se comprobó actualizar desde InitialCategories conservando una categoría con acento. Si una base anterior contiene nombres equivalentes duplicados, la creación del índice falla; no se eliminan registros automáticamente. Habría que resolver esos datos antes de aplicar la restricción.

`CatalogoDbContext` registra `CATEGORY_NAME` en cada conexión SQLite que administra, incluidas las de migraciones. Una herramienta externa que compare o escriba Name debe registrar la misma comparación; SQLite no incorpora esta función por sí solo. Cambiar su algoritmo en el futuro requiere revisar/reconstruir el índice existente, no solo cambiar código.

La conexión predeterminada guarda `catalogo.db` en la raíz del proyecto de la API. Puede reemplazarse con `ConnectionStrings__Catalogo`; las pruebas la reemplazan antes del arranque. Los archivos de base quedan excluidos de Git.

Comando comprobado para generar la primera migración (ya existe; no repetirlo):

```powershell
dotnet tool run dotnet-ef migrations add InitialCategories --project src/Catalogo.Api --output-dir Data/Migrations
```

`CatalogoDbContextFactory` permite a la herramienta generar el esquema sin arrancar la API. Su ruta relativa es solo una configuración de diseño; para aplicar manualmente una migración a un archivo específico habría que indicar su conexión. Las pruebas verificaron la aplicación automática de la migración. La ejecución manual de la API y sus ejemplos se comprobarán en una etapa posterior.

## Uso de IA

Codex asistió en la planificación, estructura, escenarios BDD, bindings HTTP, endpoints, EF/SQLite, migraciones y validaciones. Los escenarios se ejecutaron antes de implementar: lista vacía falló por 404; creación/edición por 405; nombres inválidos por 201/500; duplicados por 201 y falta de restricción SQL; detalle inexistente por falta de error estructurado. La suite actual pasó con 41 pruebas (37 BDD y 4 técnicas SQLite), incluidas persistencia, rechazos sin cambios, SQL directo y actualización del esquema. La implementación se explica por etapas; la comprensión del candidato no se da por comprobada solo por haber entregado una explicación.
