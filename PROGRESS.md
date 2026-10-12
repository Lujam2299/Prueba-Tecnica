# Avance

## Etapa actual

Primer escenario de etapa 3 completado. Fase roja: `dotnet test Catalogo.slnx --logger "console;verbosity=normal"` compiló y descubrió 1 escenario; código 1 porque esperaba 200 y recibió 404 al consultar /api/categories. Guardado antes del endpoint en commit `37ff772`.

Fase verde: `dotnet test Catalogo.slnx --no-restore --logger "console;verbosity=normal"`, código 0, 1 escenario aprobado; verificó 200, application/json y arreglo con cero elementos. Endpoint organizado en CategoryEndpoints y contrato CategoryResponse. Lista vacía fija como implementación mínima, sin creación ni almacenamiento todavía.

WebApplicationFactory 10.0.12 crea una aplicación por escenario y HttpClient realiza las peticiones HTTP; AfterScenario cierra los recursos. Todavía no hay base de datos; el aislamiento de bases permanece pendiente para el bloque de SQLite.

Preparación del entorno (11 de octubre de 2026). Se leyeron AGENTS.md y el chat de planificación completo. Acordado trabajar por etapas y completar el alcance principal en un máximo de dos días.

Etapas 1 y 2 completadas. PLAN.md contiene las casillas de seguimiento. Se crearon Catalogo.slnx, global.json y los dos proyectos para net10.0. Program.cs solo compone y arranca la aplicación; no tiene endpoints. El proyecto de pruebas referencia la API y contiene los paquetes de Reqnroll y NUnit, sin escenarios todavía.

Verificaciones de etapa 2:

- Consulta de versiones estables en NuGet: Reqnroll.NUnit 3.3.4, NUnit 4.6.1 (rama 4), NUnit3TestAdapter 6.3.1 y Microsoft.NET.Test.Sdk 18.10.1, fijadas en el proyecto.
- `dotnet restore Catalogo.slnx`: código 0; ambos proyectos restaurados.
- `dotnet build Catalogo.slnx --no-restore`: fuera del sandbox, código 0, cero advertencias y errores. Dentro del sandbox terminó con código 1 sin diagnóstico útil.
- No se ejecutaron pruebas: todavía no hay escenarios. La compilación no demuestra comportamientos ni integración completa de Reqnroll.

## Verificaciones reales

- La carpeta inicialmente solo contenía AGENTS.md.
- `git status --short` y `git log -5 --oneline`: la carpeta aún no era un repositorio Git.
- `git --version`: 2.53.0.windows.3.
- `dotnet --info`: comando no encontrado; tampoco se encontró el ejecutable en las rutas habituales comprobadas.
- VS Code encontrado en `C:\Users\lujam\AppData\Local\Programs\Microsoft VS Code\Code.exe`.
- Windows Package Manager instaló correctamente Microsoft.DotNet.SDK.10.
- `C:\Program Files\dotnet\dotnet.exe --info`: verificado fuera del sandbox, código 0; SDK 10.0.401, runtime ASP.NET Core 10.0.12. Dentro del sandbox la consulta falló por acceso a servicios de Windows.
- Repositorio local inicializado, rama `main`; sin remoto. Autor solicitado por el usuario: Lujam2299. Primer commit de preparación creado: `a58a197`; `git status --short` quedó sin cambios después del commit.
- Creados README.md, PROGRESS.md y .gitignore. No se ha implementado la API ni ejecutado pruebas.

## Siguiente paso

Primer ciclo BDD explicado al usuario. Después: escenario de creación de categoría, persistencia EF Core/SQLite con migración e aislamiento de bases, antes de implementar el comportamiento. Usuario con experiencia web, nuevo en C#/.NET; seguir explicando por etapas.

## Pendientes

Todas las funcionalidades, escenarios BDD, migraciones, comprobación de persistencia, documentación ejecutable y guion del video.

## Decisiones y dudas

Seguir las tecnologías y el contrato HTTP de AGENTS.md. Supuestos y dudas para el contacto registrados en README.md.

## GitHub

Con autorización del usuario se creó el repositorio privado https://github.com/Lujam2299/Prueba-Tecnica y se configuró como `origin`. `git push -u origin main` terminó con código 0; se verificaron en GitHub la etiqueta Private y los cuatro archivos de preparación. Nombre elegido para la URL: Prueba-Tecnica; descripción: Prueba Técnica.

Con autorización del usuario se subieron los cinco commits pendientes hasta `4e4935c` mediante `git push origin main` (código 0). `git ls-remote origin refs/heads/main` coincidió con `git rev-parse HEAD`; `git status --short --branch` mostró main sincronizada con origin/main y sin cambios. Incluye guía, estructura, punto de control, escenario rojo e implementación verde. Política acordada: commits pequeños y push al cerrar etapas o sesiones.
