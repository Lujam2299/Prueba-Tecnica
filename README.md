# API de categorías y productos

API REST desarrollada como prueba técnica con ASP.NET Core Minimal APIs (.NET 10), Entity Framework Core, SQLite y pruebas BDD con Reqnroll y NUnit.

Permite crear, consultar, editar y eliminar categorías y productos. Evita categorías duplicadas y rechaza eliminar categorías con productos asignados. Los datos se conservan en un archivo SQLite entre reinicios.

## Requisitos

- SDK compatible con global.json: .NET SDK 10.0.401 o un parche posterior de la misma banda 10.0.4xx.
- Git y acceso al repositorio mientras sea privado.
- Internet para restaurar los paquetes NuGet.
- Navegador o cliente HTTP; no se necesita un IDE específico ni instalar un servidor de base de datos.

Comprueba las herramientas:

```sh
dotnet --info
git --version
```

## Cómo ejecutar la aplicación

### 1. Clonar y entrar en el repositorio

```sh
git clone https://github.com/Lujam2299/Prueba-Tecnica.git
cd Prueba-Tecnica
```

### 2. Restaurar dependencias y herramientas

```sh
dotnet restore Catalogo.slnx
dotnet tool restore
```

Los paquetes tienen versiones explícitas. EF Core y dotnet-ef utilizan 10.0.12; la herramienta se declara en dotnet-tools.json.

### 3. Compilar

```sh
dotnet build Catalogo.slnx --no-restore
```

### 4. Iniciar la API

```sh
dotnet run --project src/Catalogo.Api --no-build --no-launch-profile --urls http://localhost:5080
```

El arranque aplica las migraciones pendientes y crea src/Catalogo.Api/catalogo.db si no existe. No se requiere ejecutar migraciones manualmente para empezar.

Mantén abierta la terminal. Ctrl+C detiene el servidor; reiniciarlo conserva los datos. Si el puerto está ocupado, cambia 5080 tanto en el comando como en las peticiones.

### 5. Abrir y probar

- [Swagger UI](http://localhost:5080/swagger/index.html): explorar endpoints y enviar peticiones mediante **Try it out → Execute**.
- [OpenAPI JSON](http://localhost:5080/openapi/v1.json): importar el contrato en clientes compatibles con OpenAPI 3.1.

Crea primero una categoría y utiliza el id devuelto para crear productos. En Swagger, **Server response** contiene el resultado real; los ejemplos de **Responses** describen resultados posibles.

### Configuración de SQLite

La conexión puede reemplazarse mediante la variable ConnectionStrings__Catalogo. Ejemplo en PowerShell, antes de iniciar:

```powershell
$env:ConnectionStrings__Catalogo = 'Data Source=C:/ruta/catalogo.db'
```

En Bash:

```sh
export ConnectionStrings__Catalogo='Data Source=/ruta/catalogo.db'
```

La carpeta elegida debe existir y ser escribible. Los archivos de datos quedan excluidos de Git.

## Cómo ejecutar las pruebas

Desde la raíz, después de restaurar dependencias:

```sh
dotnet test Catalogo.slnx --no-restore
```

El comando compila y ejecuta las pruebas. Si ya compilaste y no cambiaste el código:

```sh
dotnet test Catalogo.slnx --no-build --no-restore
```

No necesitas iniciar la API: WebApplicationFactory la levanta para las pruebas y HttpClient realiza las peticiones HTTP. Cada escenario usa un archivo SQLite temporal propio y lo elimina al finalizar, sin modificar la base local.

Los escenarios Gherkin están en tests/Catalogo.AcceptanceTests/Features/. Reqnroll conecta sus frases con los métodos de Steps/; NUnit ejecuta las pruebas y verifica resultados con aserciones.

La última ejecución verificada tuvo **92 pruebas aprobadas, 0 fallidas y 0 omitidas**: 85 casos BDD y 7 comprobaciones técnicas. Cubren CRUD, validaciones, Unicode, relaciones, errores, persistencia, migraciones, aislamiento y documentación. [COVERAGE.md](COVERAGE.md) relaciona requisitos y pruebas.

Se comprobó también una copia limpia sin bin/, obj/ ni base previa: restauración de dependencias y herramientas, compilación sin advertencias ni errores, y las 92 pruebas aprobadas. Se inició el servidor real y se probaron peticiones con curl. La comprobación se realizó en Windows; otros sistemas operativos no se han verificado.

## Estructura

```text
src/Catalogo.Api/
  Program.cs       Configuración y composición
  Endpoints/       Operaciones HTTP por entidad
  Contracts/       Datos de entrada y salida
  Models/          Entidades
  Data/            Contexto, comparación de nombres y migraciones
tests/Catalogo.AcceptanceTests/
  Features/        Escenarios Gherkin en español
  Steps/           Bindings HTTP y verificaciones
  Support/         Arranque, aislamiento y comprobaciones técnicas
examples/          Cuerpos JSON para demostración
```

## Decisiones importantes y sus motivos

- **Minimal APIs y estructura sencilla:** mantener una implementación pequeña y fácil de recorrer, sin capas innecesarias para el alcance. Los endpoints se organizan por entidad y Program.cs concentra la composición.
- **SQLite en archivo:** persistir datos sin exigir instalar o administrar un servidor de base de datos.
- **EF Core y migraciones:** configurar relaciones y restricciones en el modelo y versionar los cambios de esquema. Las migraciones crean categorías, agregan unicidad y crean productos.
- **Migraciones al iniciar:** simplificar la ejecución local; no es una estrategia de despliegue multiinstancia.
- **Contratos de entrada y salida:** controlar el formato HTTP sin exponer entidades EF directamente. ProductResponse incluye categoryId y categoryName.
- **Validación HTTP y restricciones SQLite:** ofrecer errores comprensibles y proteger la integridad incluso si se omite la API.
- **Comparación Unicode explícita:** SQLite NOCASE no cubre todos los caracteres Unicode. CATEGORY_NAME aplica Trim, normalización NFC, ToUpperInvariant y NFC nuevamente; compara ordinalmente. Electrónica y ELECTRÓNICA son equivalentes, pero Electronica es distinto. Consultas e índice único utilizan esa regla; el texto se guarda conservando su escritura, sin blancos externos.
- **Clave foránea y eliminación restringida:** evitar eliminar categorías con productos y perder datos relacionados.
- **Problem Details:** devolver errores de negocio estructurados y consistentes.
- **BDD mediante HTTP:** comprobar comportamiento observable. Para funcionalidades nuevas se escribieron escenarios y se comprobó el fallo antes de implementar. Las verificaciones complementarias de auditoría no simulan una fase roja.
- **Bases aisladas:** evitar dependencia entre escenarios y proteger la base de la aplicación. La persistencia se comprueba reiniciando con el mismo archivo de prueba.
- **Swagger y curl:** permitir evaluación sin frontend propio ni dependencia de un IDE. La documentación se habilita también sin perfil Development para esta entrega local.

## Supuestos adoptados

En aspectos no especificados con precisión, se adoptaron estas reglas:

- Recortar blancos externos del nombre y conservar los internos. Rechazar nombres ausentes, null, vacíos o compuestos solo por blancos.
- Permitir nombres repetidos de productos; exigir nombres únicos de categorías según la comparación indicada.
- Diferenciar letras acentuadas de letras sin acento.
- Permitir descripción opcional. PUT reemplaza los campos: omitir descripción la deja en null.
- No imponer longitudes máximas adicionales.
- CategoryId debe ser un entero positivo de una categoría existente.
- En PUT se comprueba existencia del recurso antes de validar los valores recibidos.
- Las listas vacías devuelven 200 con un arreglo vacío.
- No implementar autenticación, frontend, paginación ni despliegue público, fuera de la entrega acordada.

## Contrato HTTP

| Operación | Categorías | Productos |
| --- | --- | --- |
| GET lista | 200 | 200 |
| GET detalle | 200 / 404 | 200 / 404 |
| POST | 201 / 400 / 409 | 201 / 400 |
| PUT | 200 / 400 / 404 / 409 | 200 / 400 / 404 |
| DELETE | 204 / 404 / 409 | 204 / 404 |

POST devuelve el recurso y Location. PUT conserva el id. DELETE exitoso devuelve 204 sin cuerpo.

Nombre de categoría duplicado: 409 con detail **Ya existe una categoría con ese nombre.**

Categoría con productos: 409 con detail exacto:

```text
Esta categoria no se puede eliminar porque tiene productos asignados
```

El rechazo conserva categoría y productos. Retirar todos los productos permite eliminar la categoría. Lista y detalle de productos muestran el identificador y nombre actual de categoría.

## Ejemplos con curl

Con el servidor abierto, ejecuta desde la raíz en otra terminal. En Windows PowerShell usa curl.exe para evitar el alias de versiones antiguas. Los cuerpos JSON se leen de archivos para evitar diferencias de comillas entre shells.

```sh
curl -i http://localhost:5080/api/categories
curl -i http://localhost:5080/api/categories -H "Content-Type: application/json" --data-binary "@examples/category.json"
curl -i http://localhost:5080/api/categories -H "Content-Type: application/json" --data-binary "@examples/category-duplicate.json"
```

Resultados esperados: 200, 201 y 409 si el nombre no existía antes de crearlo. Anota el id devuelto. El número 1 de los siguientes ejemplos es ilustrativo: reemplázalo por los ids reales y ajusta categoryId en los archivos JSON de productos.

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

Resultados: consultas 200, creación 201, edición 200, validación 400, categoría con productos 409 y eliminaciones finales 204 si no quedan otros productos. El ejemplo inválido usa 2147483647 como categoría inexistente: cámbialo si ese id existe.

Para editar categorías, modifica examples/category.json y envíalo con PUT a /api/categories/{id}. Para cambiar la categoría de un producto, crea otra categoría y usa su id en examples/product-update.json antes de ejecutar PUT. Confirma con GET; reinicia y consulta otra vez para comprobar persistencia.

## Uso de IA y revisión

Se utilizó Codex(ChatGPT Plus) para apoyar la planificación, estructura, escenarios Gherkin, bindings, endpoints, configuración de EF Core y migraciones, documentación y ejemplos, manteniendo al margen su uso mediante reglas y solicitudes de autorizaciones explícitas.

Las propuestas se contrastaron con requisitos y se verificaron mediante:

- Escenarios HTTP con fallo previo y resultado correcto posterior en las funcionalidades desarrolladas mediante BDD.
- Suite completa: 92 pruebas aprobadas en la última verificación.
- SQL directo para unicidad, clave foránea, eliminación restringida y migraciones con conservación de datos.
- Una prueba de dos aplicaciones independientes para comprobar aislamiento.
- Restauración, compilación y pruebas desde copia limpia, además de peticiones reales con curl.

## Video de demostración

El video ya está grabado. El enlace será enviado por medio del número telefónico.


[COVERAGE.md](COVERAGE.md) relaciona requisitos y pruebas. [PROGRESS.md](PROGRESS.md) conserva el historial de verificaciones y [PLAN.md](PLAN.md) el plan de trabajo.
