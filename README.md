# API de categorías y productos

Prueba técnica de entrevista. Se desarrollará por etapas pequeñas con ASP.NET Core Minimal APIs (.NET 10), Entity Framework Core 10, SQLite y pruebas BDD con Reqnroll y NUnit.

## Estado

Creación de categorías con persistencia SQLite y validación de nombre implementadas. POST devuelve 201 y la ubicación del detalle; lista y detalle leen la base. Once casos BDD aprobados, incluido reinicio de la aplicación. Duplicados, edición y eliminaciones siguen pendientes. Consulta `PROGRESS.md` para retomar el trabajo.

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
- En creación de categorías, `string.IsNullOrWhiteSpace` rechaza nombres ausentes, null o compuestos solo por blancos (también tabulaciones y saltos de línea). `Trim()` elimina blancos externos y conserva los internos. Rechazo: 400 con `application/problem+json`, `errors.Name` y mensaje `El nombre es obligatorio.`; no se guarda ningún recurso.
- Categorías con nombre único ignorando mayúsculas; conservar la diferencia entre letras con y sin acento. La estrategia Unicode se definirá y probará antes de implementar esta regla.
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

La conexión predeterminada guarda `catalogo.db` en la raíz del proyecto de la API. Puede reemplazarse con `ConnectionStrings__Catalogo`; las pruebas la reemplazan antes del arranque. Los archivos de base quedan excluidos de Git.

Comando comprobado para generar la primera migración (ya existe; no repetirlo):

```powershell
dotnet tool run dotnet-ef migrations add InitialCategories --project src/Catalogo.Api --output-dir Data/Migrations
```

`CatalogoDbContextFactory` permite a la herramienta generar el esquema sin arrancar la API. Su ruta relativa es solo una configuración de diseño; para aplicar manualmente una migración a un archivo específico habría que indicar su conexión. Las pruebas verificaron la aplicación automática de la migración. La ejecución manual de la API y sus ejemplos se comprobarán en una etapa posterior.

## Uso de IA

Codex asistió en la planificación, estructura, escenarios BDD, bindings HTTP, endpoints, configuración EF/SQLite, migración y validación de nombres. Los casos se ejecutaron antes de implementar: lista vacía falló por 404; creación por 405; nombres inválidos por 201/500 y recorte por contenido incorrecto. La suite completa actual pasó con 11 casos, incluido reinicio con datos conservados. La implementación se explica por etapas; la comprensión del candidato no se da por comprobada solo por haber entregado una explicación.
