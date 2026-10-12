# Avance

## Etapa actual

Preparación del entorno (11 de octubre de 2026). Se leyeron AGENTS.md y el chat de planificación completo. Acordado trabajar por etapas y completar el alcance principal en un máximo de dos días.

Preparación completada. Se creó PLAN.md como referencia compartida con etapas y casillas de seguimiento; README incluye su enlace. Esta intervención solo agrega planificación: no crea proyectos ni implementa comportamientos.

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

Crear la solución con los dos proyectos. Resolver primero la integración de Reqnroll mediante un escenario HTTP ejecutable, antes de implementar categorías. Si una terminal abierta previamente no reconoce `dotnet`, abrir una nueva para cargar el PATH actualizado o usar la ruta completa.

## Pendientes

Todas las funcionalidades, escenarios BDD, migraciones, comprobación de persistencia, documentación ejecutable y guion del video.

## Decisiones y dudas

Seguir las tecnologías y el contrato HTTP de AGENTS.md. Supuestos y dudas para el contacto registrados en README.md.

## GitHub

Con autorización del usuario se creó el repositorio privado https://github.com/Lujam2299/Prueba-Tecnica y se configuró como `origin`. `git push -u origin main` terminó con código 0; se verificaron en GitHub la etiqueta Private y los cuatro archivos de preparación. Nombre elegido para la URL: Prueba-Tecnica; descripción: Prueba Técnica.
