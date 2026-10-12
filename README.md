# API de categorías y productos

Prueba técnica de entrevista. Se desarrollará por etapas pequeñas con ASP.NET Core Minimal APIs (.NET 10), Entity Framework Core 10, SQLite y pruebas BDD con Reqnroll y NUnit.

## Estado

Solución creada y compilación verificada. La API aún no tiene endpoints y el proyecto de pruebas aún no contiene escenarios. Consulta `PROGRESS.md` para retomar el trabajo.

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
```

`Catalogo.slnx` agrupa la API y las pruebas. `global.json` selecciona el SDK 10.0.401 y permite parches posteriores de esa misma banda. Los paquetes del proyecto de pruebas tienen versiones explícitas: Reqnroll.NUnit 3.3.4, NUnit 4.6.1, NUnit3TestAdapter 6.3.1 y Microsoft.NET.Test.Sdk 18.10.1. Los archivos generados de Reqnroll se ubicarán en `obj/` para excluirlos de Git.

Si una terminal anterior a la instalación no reconoce `dotnet`, abre otra o usa `& 'C:\Program Files\dotnet\dotnet.exe'` en lugar de `dotnet`.

Los comandos de migraciones, ejecución y pruebas se incorporarán al comprobarlos. Aún no hay funcionalidades ni escenarios ejecutables; EF Core y SQLite se agregarán cuando el comportamiento requiera persistencia.

## Uso de IA

Codex asistió en la planificación, lectura de requisitos, revisión del entorno y creación de la estructura de proyectos. Se verificaron la restauración y compilación. La implementación se revisará y explicará por etapas para que el candidato pueda comprenderla y modificarla. No se han ejecutado pruebas de la API todavía.
