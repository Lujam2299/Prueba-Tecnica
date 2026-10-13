# API de categorías y productos

Prueba técnica de entrevista. Se desarrollará por etapas pequeñas con ASP.NET Core Minimal APIs (.NET 10), Entity Framework Core 10, SQLite y pruebas BDD con Reqnroll y NUnit.

## Estado

Creación, consulta y edición de categorías completas; creación, validación, lista, detalle y edición de productos implementados con SQLite. Suite actual: 85 casos BDD y 7 comprobaciones técnicas aprobadas. Eliminaciones de productos y categorías completas, incluida la restricción con productos. Cobertura, documentación OpenAPI, interfaz de pruebas y reproducibilidad local verificadas. Pendiente guion del video y repaso del candidato. Consulta [COVERAGE.md](COVERAGE.md) para ver la relación entre requisitos y pruebas. Consulta `PROGRESS.md` para retomar el trabajo.

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
- POST /api/products crea productos válidos con 201 y ubicación del detalle. GET de lista devuelve 200 (arreglo vacío si no hay productos); detalle 200 o 404 Problem Details. ProductResponse contiene id, name, description, categoryId y categoryName. El nombre de categoría se obtiene de la relación al consultar, por lo que muestra también los cambios posteriores de nombre.
- Creación de productos: nombre ausente/null/vacío o solo blancos devuelve 400 con errors.Name: `El nombre es obligatorio.`. El nombre válido se guarda con Trim(), sin cambiar espacios internos. CategoryId ausente/null/cero/negativo devuelve 400 con errors.CategoryId: `La categoría es obligatoria.`; los identificadores de SQLite son enteros positivos. Un identificador positivo inexistente devuelve `La categoría indicada no existe.`. Se incluyen ambos errores cuando ambos campos faltan. Formato: application/problem+json. Los rechazos no guardan productos ni alteran categorías.
- PUT /api/products/{id} comparte ProductRequest y validaciones con POST. Reemplaza nombre, descripción y categoría; conserva el identificador, recorta blancos externos y permite nombres repetidos. Omitir descripción la deja null. Devuelve 200 con el producto y su categoría actual. Comprueba primero existencia: producto inexistente devuelve 404 Problem Details incluso con cuerpo {}. Entradas inválidas devuelven 400 y conservan datos anteriores. Edición y cambio de categoría se comprobaron también al reiniciar, sin modificar otros productos ni categorías.
- DELETE /api/products/{id} elimina el producto con 204 y cuerpo vacío; inexistente o ya eliminado devuelve 404 Problem Details. La eliminación persiste tras reiniciar y conserva categoría y otros productos. No requiere cambios de esquema.
- No se han establecido longitudes máximas adicionales.
- DELETE /api/categories/{id}: categoría sin productos devuelve 204 sin cuerpo; inexistente o ya eliminada devuelve 404 Problem Details. Con productos devuelve 409 y detail exacto: `Esta categoria no se puede eliminar porque tiene productos asignados`. El rechazo conserva categoría y productos incluso al reiniciar. Retirar todos los productos permite eliminar la categoría; retirar solo uno no basta.
- Respuestas: GET 200/404, POST 201, PUT 200/404, DELETE 204/404; validaciones 400 y conflictos 409. Los errores usarán un formato consistente.
- Entrega: repositorio, README y video. Deploy opcional después de completar los requisitos principales.

Pendiente confirmar con el contacto: tratamiento de espacios, longitudes máximas y repetición de nombres de productos.

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

AddProducts crea Products y la clave foránea CategoryId → Categories.Id con eliminación restringida. El nombre de producto es no nulo en la tabla y no tiene índice único. Se verificaron creación, validaciones, consultas, descripción opcional, repetición de nombres y persistencia. SQL directo confirmó que la clave foránea rechaza un producto con categoría inexistente (SQLite 787); el endpoint también captura esa infracción si la categoría desaparece entre la validación y el guardado, aunque no se ha probado esa carrera controlada. SQL directo también confirmó que eliminar una categoría con productos falla con SQLite 1811 (RESTRICT) y conserva los datos tras reiniciar. DELETE captura específicamente SQLite 787/1811 ante una relación que impida el guardado; la carrera entre la comprobación y el guardado no se ha probado de forma controlada. Cada prueba usa su propio archivo temporal.

`CatalogoDbContext` registra `CATEGORY_NAME` en cada conexión SQLite que administra, incluidas las de migraciones. Una herramienta externa que compare o escriba Name debe registrar la misma comparación; SQLite no incorpora esta función por sí solo. Cambiar su algoritmo en el futuro requiere revisar/reconstruir el índice existente, no solo cambiar código.

La conexión predeterminada guarda `catalogo.db` en la raíz del proyecto de la API. Puede reemplazarse con `ConnectionStrings__Catalogo`; las pruebas la reemplazan antes del arranque. Los archivos de base quedan excluidos de Git.

Comando comprobado para generar la primera migración (ya existe; no repetirlo):

```powershell
dotnet tool run dotnet-ef migrations add InitialCategories --project src/Catalogo.Api --output-dir Data/Migrations
```

`CatalogoDbContextFactory` permite a la herramienta generar el esquema sin arrancar la API. Su ruta relativa es solo una configuración de diseño; para aplicar manualmente una migración a un archivo específico habría que indicar su conexión. Las pruebas verificaron la aplicación automática de la migración. La ejecución manual y los ejemplos se comprobaron sobre una copia limpia; véase la sección de demostración.

## Arrancar y probar sin depender de un IDE

Desde la raíz, después de restaurar y compilar:

```sh
dotnet run --project src/Catalogo.Api --no-build --no-launch-profile --urls http://localhost:5080
```

La aplicación aplica las migraciones al arrancar y crea `src/Catalogo.Api/catalogo.db` si no existe. Detén el servidor con Ctrl+C; reiniciarlo conserva los datos. Si el puerto está ocupado, cambia 5080 y usa ese puerto también en las peticiones. Para usar otra base, configura `ConnectionStrings__Catalogo` con una conexión SQLite antes de arrancar (por ejemplo, `Data Source=/ruta/catalogo.db`); la carpeta debe existir y ser escribible.

- Interfaz de pruebas: [Swagger UI](http://localhost:5080/swagger/index.html).
- Contrato importable: [OpenAPI JSON](http://localhost:5080/openapi/v1.json).

Swagger permite expandir una operación, pulsar **Try it out**, editar el cuerpo JSON y pulsar **Execute**. Crear primero una categoría y usar su id para crear productos. La interfaz y sus recursos se sirven desde la API, sin desarrollar frontend ni exigir un IDE. La documentación se habilita también sin perfil Development para simplificar esta entrega local. Swagger UI usa el documento generado por Microsoft.AspNetCore.OpenApi, con paquetes fijados en 10.2.3 y 10.0.12 respectivamente; [documentación oficial de esta integración](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-10.0).

En Postman u otro cliente que acepte OpenAPI, importa la URL del documento o el JSON descargado y configura la dirección de la API como `http://localhost:5080`. No se requiere autenticación. Si el cliente no admite OpenAPI 3.1, puede realizar las peticiones HTTP directamente.

### Ejemplos con curl

Ejecuta desde la raíz, en otra terminal mientras el servidor sigue abierto. En Windows PowerShell usa `curl.exe` en lugar de `curl` para evitar el alias de versiones antiguas. Los cuerpos están en archivos JSON UTF-8 para evitar diferencias de comillas entre shells.

```sh
curl -i http://localhost:5080/api/categories
curl -i http://localhost:5080/api/categories -H "Content-Type: application/json" --data-binary "@examples/category.json"
curl -i http://localhost:5080/api/categories -H "Content-Type: application/json" --data-binary "@examples/category-duplicate.json"
```

Resultados: lista 200, creación 201 con `Location`, duplicado 409. Anota el id devuelto; en los siguientes ejemplos se usa 1 como ejemplo, no como garantía. Sustituye el id en las URLs y el `categoryId` de `examples/product.json` y `examples/product-update.json` por el obtenido.

```sh
curl -i http://localhost:5080/api/categories/1
curl -i http://localhost:5080/api/products -H "Content-Type: application/json" --data-binary "@examples/product.json"
curl -i http://localhost:5080/api/products
curl -i http://localhost:5080/api/products/1
curl -i -X PUT http://localhost:5080/api/products/1 -H "Content-Type: application/json" --data-binary "@examples/product-update.json"
curl -i http://localhost:5080/api/products -H "Content-Type: application/json" --data-binary "@examples/invalid-product.json"
curl -i -X DELETE http://localhost:5080/api/categories/1
curl -i -X DELETE http://localhost:5080/api/products/1
curl -i -X DELETE http://localhost:5080/api/categories/1
```

Consulta 200, creación 201, edición 200, datos inválidos 400, categoría con productos 409, eliminación de producto 204 y luego eliminación de categoría 204. Si hay más productos en la categoría, debes eliminarlos todos. El ejemplo de categoría inexistente usa 2147483647: cámbialo si ese id existe en tu base.

Para editar una categoría, modifica nombre y descripción en `examples/category.json` y envíalo con `curl -i -X PUT http://localhost:5080/api/categories/1 -H "Content-Type: application/json" --data-binary "@examples/category.json"`. Para cambiar la categoría de un producto, crea otra categoría y utiliza su id en `examples/product-update.json` antes de ejecutar PUT. Los cambios se observan en lista y detalle, también después de reiniciar.

### Comprobación desde un directorio limpio

Se copiaron únicamente los archivos fuente a un directorio temporal, sin `.git`, `bin`, `obj` ni base previa. Allí se ejecutaron `dotnet restore`, `dotnet tool restore`, `dotnet build --no-restore` y `dotnet test --no-build --no-restore` sobre Catalogo.slnx: compilación sin advertencias ni errores y 92 pruebas aprobadas. Se arrancó la copia con el comando anterior, que aplicó las tres migraciones, y se verificaron OpenAPI, Swagger y los ejemplos de creación, edición, validación, conflicto y eliminación mediante curl.exe. Estas peticiones modificaron solo la base de la copia temporal.

Un evaluador con acceso al repositorio privado puede clonar con `git clone https://github.com/Lujam2299/Prueba-Tecnica.git`, entrar en `Prueba-Tecnica` y seguir los comandos de restauración, compilación y ejecución anteriores. Git, SDK y conexión para restaurar NuGet son necesarios; no hace falta Visual Studio, VS Code ni Postman. La comprobación se realizó en Windows; la ejecución en otros sistemas no se ha verificado.

## Uso de IA

Codex asistió en la planificación, estructura, escenarios BDD, bindings HTTP, endpoints, EF/SQLite, migraciones y validaciones. Los escenarios se ejecutaron antes de implementar: validaciones de productos fallaron por 201/500 y blancos conservados; después se corrigieron. La suite actual pasó con 92 pruebas (85 BDD y 7 técnicas), incluidas persistencia, rechazos sin cambios, SQL directo y actualización del esquema. La implementación se explica por etapas; la comprensión del candidato no se da por comprobada solo por haber entregado una explicación.
