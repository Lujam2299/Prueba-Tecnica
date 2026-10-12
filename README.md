# API de categorías y productos

Prueba técnica de entrevista. Se desarrollará por etapas pequeñas con ASP.NET Core Minimal APIs (.NET 10), Entity Framework Core 10, SQLite y pruebas BDD con Reqnroll y NUnit.

## Estado

Primer comportamiento implementado y verificado con BDD: `GET /api/categories` devuelve 200 y `[]`. Todavía no hay creación ni persistencia de datos. Consulta `PROGRESS.md` para retomar el trabajo.

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
dotnet build Catalogo.slnx --no-restore
dotnet test Catalogo.slnx --no-restore
```

`Catalogo.slnx` agrupa la API y las pruebas. `global.json` selecciona el SDK 10.0.401 y permite parches posteriores de esa misma banda. Los paquetes del proyecto de pruebas tienen versiones explícitas: Reqnroll.NUnit 3.3.4, NUnit 4.6.1, NUnit3TestAdapter 6.3.1 y Microsoft.NET.Test.Sdk 18.10.1. Los archivos generados de Reqnroll se ubicarán en `obj/` para excluirlos de Git.

Si una terminal anterior a la instalación no reconoce `dotnet`, abre otra o usa `& 'C:\Program Files\dotnet\dotnet.exe'` en lugar de `dotnet`.

Las pruebas usan Microsoft.AspNetCore.Mvc.Testing 10.0.12 y WebApplicationFactory: levantan la API en un servidor de pruebas en memoria y realizan peticiones HTTP con HttpClient. Cada escenario tiene su propia aplicación, que se cierra al finalizar; no hace falta arrancar la API manualmente. La prueba actual verifica código 200, tipo JSON y arreglo vacío. EF Core, migraciones y bases aisladas se agregarán con la creación de categorías.

Los comandos de migraciones y ejecución manual se incorporarán al comprobarlos.

## Uso de IA

Codex asistió en la planificación, revisión del entorno, estructura y primer escenario BDD con sus bindings y endpoint. Se verificaron restauración y compilación; la prueba HTTP falló primero por 404 y pasó después con 200 y arreglo JSON vacío. La implementación se explica por etapas para que el candidato pueda comprenderla y modificarla; su comprensión no se da por comprobada solo por haber entregado una explicación.
