# API de categorías y productos

Prueba técnica de entrevista. Se desarrollará por etapas pequeñas con ASP.NET Core Minimal APIs (.NET 10), Entity Framework Core 10, SQLite y pruebas BDD con Reqnroll y NUnit.

## Estado

Preparación del entorno. La API y las pruebas todavía no están creadas. Consulta `PROGRESS.md` para retomar el trabajo.

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

Los comandos de creación, configuración, migraciones, ejecución y pruebas se incorporarán cuando se comprueben. Aún no hay funcionalidades ni pruebas ejecutables.

## Uso de IA

Codex asistió en la planificación, lectura de requisitos y revisión del entorno. La implementación se revisará y explicará por etapas para que el candidato pueda comprenderla y modificarla. No se han ejecutado pruebas de la API todavía.
