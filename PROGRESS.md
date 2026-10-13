# Avance

## Etapa actual

PLAN.md actualizado con el estado de entrega confirmado: guion local, pruebas manuales, repaso parcial, video grabado y README preparado. Pendientes: enlace de video, acceso del evaluador, revisión personal del README, repaso restante y ejercicio en vivo. Deploy descartado. Se respetaron las modificaciones en curso del usuario a README.md, sin incluirlas en este cambio. Actualización documental; no se repitieron pruebas.

Etapa 9 en preparación de entrega, 13 de octubre de 2026. El usuario confirmó que grabó el video y realizó pruebas manuales en Swagger. Se repasaron endpoints de productos, DbContext y relación Gherkin/Reqnroll/NUnit. Deploy descartado por decisión del usuario. El guion personal permanece local y excluido de Git.

README reorganizado para el evaluador: ejecución numerada, pruebas, decisiones con motivos, supuestos, uso y revisión de IA, contrato HTTP, ejemplos y limitaciones. El usuario lo personalizará antes de entregar. Enlace del video y acceso del evaluador al repositorio privado pendientes. Verificación documental: git diff --check sin errores; guion confirmado ignorado. No cambió código ni se ejecutaron nuevamente pruebas: último resultado comprobado sigue siendo 92 aprobadas.

### Historial: preparación de demostración

Etapa 8 completa. Fase roja de documentación en fc723ea: 1 escenario fallido por GET /openapi/v1.json 404. Se agregaron Microsoft.AspNetCore.OpenApi 10.0.12 y SwaggerUI 10.2.3, AddOpenApi/MapOpenApi y Swagger en /swagger/index.html, además de metadatos de contratos y estados en los diez endpoints. Documentación accesible sin depender del perfil de entorno. No hay frontend propio.

Una primera compilación falló por sintaxis en la edición automática de metadatos; se corrigió antes de verificar. Suite en workspace: código 0, 92 aprobadas (85 BDD y 7 técnicas), 0 omitidas. El escenario nuevo comprueba operaciones, cuerpos JSON, estados, categoryName y recursos de Swagger.

Copia limpia temporal sin .git/bin/obj/base previa: restore, tool restore, build --no-restore (0 advertencias/errores), test --no-build --no-restore: todos código 0, 92 aprobadas. Servidor real en localhost:5080 con --no-launch-profile: migraciones aplicadas, OpenAPI 3.1.1 y Swagger HTTP 200. curl.exe comprobó POST categoría 201, duplicado 409, POST producto 201, PUT 200, validación 400, restricción exacta 409, DELETE producto/categoría 204. Se detuvo el servidor al terminar. Base del workspace ausente; solo la copia temporal recibió datos. No se verificó otro sistema operativo ni una interacción visual completa del navegador.

README contiene arranque, URLs, importación del contrato, configuración y ejemplos con archivos JSON para evitar diferencias de shell; el id 1 es solo ilustrativo y debe sustituirse. No cambiaron migraciones ni reglas de negocio. Siguiente paso: etapa 9, guion del video, repaso del código y práctica de un cambio en vivo. Comprensión del candidato, grabación/publicación y acceso del evaluador siguen pendientes.

### Historial: auditoría de cobertura

Etapa 7 completa, revisión del 12 de octubre de 2026. COVERAGE.md relaciona requisitos de AGENTS.md con características y comprobaciones técnicas. Los comportamientos de negocio obligatorios tienen cobertura HTTP; índice único, clave foránea, RESTRICT y migraciones también tienen pruebas técnicas. Se agregó ScenarioIsolationTests para dos aplicaciones simultáneas con el mismo nombre de categoría y eliminación independiente, comprobada tras reiniciar.

Verificación: dotnet test Catalogo.slnx --no-restore --logger "console;verbosity=minimal", código 0, 91 aprobadas (84 BDD y 7 técnicas), 0 omitidas. La nueva prueba complementa la auditoría del soporte existente: pasó sin cambios de API y sin fase roja artificial. Test-Path src/Catalogo.Api/catalogo.db: False; git diff --check sin errores.

No se modificaron endpoints, esquema ni migraciones. Revisión de errores de negocio: 400/404/409 Problem Details y mensaje obligatorio exacto. No se probó toda permutación de orden; aislamiento respaldado por rutas temporales únicas y comprobación explícita. JSON malformado y carreras HTTP controladas no forman parte de esta verificación.

Siguiente paso concreto: etapa 8, documentación OpenAPI y una forma sencilla de ejecutar peticiones; completar ejemplos y comprobar instrucciones desde directorio limpio. Video y repaso del candidato siguen después.

### Historial: cierre de eliminaciones

Etapa 6 completa: eliminaciones de productos y categorías con restricción de productos asignados. Fase roja en 1378774: dotnet test Catalogo.slnx --no-restore --filter "FullyQualifiedName~EliminacionDeCategorias|FullyQualifiedName~DatabaseRejectsDeletingCategoryWithProducts" --logger "console;verbosity=minimal", código 1, 5 casos HTTP fallidos por 405 y 1 prueba SQL aprobada, 0 omitidos.

Fase verde: dotnet test Catalogo.slnx --no-restore --logger "console;verbosity=minimal", código 0, 90 aprobadas (84 BDD y 6 técnicas), 0 omitidas. DELETE de categorías devuelve 204/404 y consulta productos antes de eliminar; conflicto 409 con detail exacto requerido. SQLite RESTRICT ya configurado se comprobó por SQL directo (1811), conservando ambos recursos tras reiniciar. Endpoint captura SQLite 787/1811 al guardar. No cambió el esquema ni se generaron migraciones.

Escenarios: eliminar categoría vacía, inexistente, segunda eliminación, rechazo con dos productos, rechazo tras retirar uno y eliminación después de retirar todos. Comprueban listas y detalles persistentes, conservación de otra categoría y productos restantes. git diff --check sin errores; Test-Path src/Catalogo.Api/catalogo.db: False. Limitación: no se ha provocado una carrera HTTP controlada entre comprobar productos y guardar.

Siguiente paso: etapa 7, auditoría de escenarios contra AGENTS.md, integridad, errores y aislamiento; después OpenAPI, ejecución de peticiones y comprobación desde directorio limpio (etapa 8).

### Historial: eliminación de productos

Etapa 6 en curso: eliminación de productos completada. Fase roja en f2cb059: dotnet test Catalogo.slnx --no-restore --filter "FullyQualifiedName~EliminacionDeProductos" --logger "console;verbosity=minimal", código 1, 3 fallos por HTTP 405, 0 omitidas. Fase verde: dotnet test Catalogo.slnx --no-restore --logger "console;verbosity=minimal", código 0, 84 aprobadas (79 BDD y 5 técnicas), 0 omitidas.

DELETE busca mediante FindAsync, devuelve Problem Details 404 si no existe, marca la entidad con Remove y persiste con SaveChangesAsync; responde 204 sin cuerpo. Escenarios verifican desaparición persistente tras reinicio, conservación de categoría y otro producto, inexistentes y segunda eliminación 404. No cambió el esquema. git diff --check sin errores; Test-Path src/Catalogo.Api/catalogo.db: False.

Siguiente paso: eliminación de categorías sin productos, rechazo 409 con mensaje exacto cuando tienen productos, conservación tras rechazo, eliminación tras retirar todos sus productos y prueba directa de restricción SQLite.

### Historial: cierre del paso 5

Paso 5 completo: creación, consulta, validación y edición de productos con cambio de categoría. Fase roja en 3869433: 19 casos nuevos fallaron por HTTP 405, sin omisiones. Fase verde: dotnet test Catalogo.slnx --no-restore --logger "console;verbosity=minimal", código 0, 81 aprobadas (76 BDD y 5 técnicas), 0 omitidas.

PUT busca el producto antes de validar, reutiliza validaciones de POST y captura específica de SQLite 787; guarda nombre recortado, descripción y CategoryId. Respuesta 200 conserva id e incluye nombre actual de categoría. Escenarios verifican cambios persistentes, descripción omitida null, duplicados permitidos, validaciones 400 e inexistentes 404, y conservación de datos tras rechazos. No cambió el esquema.

Siguiente paso concreto: etapa 6, escenarios y endpoints de eliminación de productos y categorías, restricción 409 con mensaje exacto y comprobación directa de la restricción en SQLite. La carrera de desaparición de categoría entre validación y guardado sigue sin una prueba controlada.

### Historial: validación de productos

Validación de productos completada. Fase roja guardada en `8db4757`: 13 casos BDD fallidos y 1 comprobación directa FK aprobada, 0 omitidos. Nombres sin contenido se guardaban o provocaban 500, categoría ausente/inexistente provocaba 500 y blancos externos se conservaban. SQL directo ya rechazaba categoría inexistente con SQLite 787.

Fase verde: `dotnet test Catalogo.slnx --no-restore --logger "console;verbosity=minimal"`, código 0, 62 pruebas aprobadas (57 BDD y 5 técnicas), 0 omitidas. Validación por campo en ProductEndpoints: IsNullOrWhiteSpace para nombre; CategoryId debe ser positivo y existir; se informan juntos ambos errores requeridos. ValidationProblem 400 consistente, Trim() al guardar y captura específica de SQLite 787 (la carrera entre comprobar y guardar no fue probada). Los escenarios verifican que rechazos no guarden productos ni alteren categorías, también tras reiniciar, y recorte persistente sin modificar blancos internos. No cambió el esquema. Base local sigue ausente; git diff --check sin errores.

Pendiente: PUT de productos, cambio de categoría, eliminaciones y comprobación directa de restricción de eliminación. Creación y consulta de productos con sus validaciones ya completas.

### Historial: creación y consulta de productos

Etapa 5 en curso: creación válida, lista y detalle de productos completos. Fase roja guardada en `4add51d`: 7 casos fallidos, 0 omitidos, por endpoints pendientes. Casos: lista vacía, descripción presente/ausente, persistencia tras reinicio, nombres repetidos, nombre actual de categoría tras renombrarla y detalle inexistente.

Product, ProductRequest y ProductEndpoints agregados; ProductResponse incluye CategoryId y CategoryName. DbContext agrega Products y relación con categoría con DeleteBehavior.Restrict. Migración AddProducts generada con dotnet-ef, código 0, y aplicada en pruebas. Primera ejecución completa encontró 7 fallos nuevos por traducción de filtros/orden sobre el record proyectado; se corrigió filtrando y ordenando entidades antes de proyectar. Segunda ejecución `dotnet test Catalogo.slnx --no-restore --logger "console;verbosity=minimal"`: código 0, 48 aprobadas (44 BDD y 4 técnicas), 0 omitidas. La base local de aplicación sigue ausente.

Limitación actual: POST de producto implementa solo entradas válidas. Nombre requerido/espacios y categoría ausente/inexistente aún no tienen validación 400; se escribirán sus escenarios en el siguiente bloque. Clave foránea y restricción de eliminación están configuradas en migración, pero sus pruebas directas y rechazos HTTP siguen pendientes. No hay PUT ni DELETE de productos.

### Historial: edición de categorías

Etapa 4 completada: creación, consulta y edición de categorías. Fase roja de edición guardada en `c04d870`: 17 casos nuevos ejecutados y fallidos, 0 omitidos; PUT devolvía 405 y GET inexistente 404 sin Problem Details. Casos: 6 ediciones válidas, 5 nombres inválidos, 3 conflictos Unicode con otra categoría, 2 PUT inexistentes y GET inexistente.

Fase verde: `dotnet test Catalogo.slnx --no-restore --logger "console;verbosity=minimal"`, código 0, 41 pruebas aprobadas (37 BDD y 4 técnicas), 0 omitidas. Verifican id conservado, cambios persistentes al reiniciar y ningún cambio tras rechazos. PUT busca entidad con FindAsync, comprueba existencia, valida nombre y consulta duplicados excluyendo su id antes de modificar y guardar. POST y PUT comparten CategoryRequest, NameRequired y SaveWithoutNameConflict. GET/PUT inexistentes usan Problem Details 404. No cambió el esquema ni se generó otra migración.

Supuestos precisados: PUT reemplaza nombre y descripción, omitir descripción la deja null; en PUT existencia se comprueba antes que validez del nombre. Eliminaciones y productos siguen pendientes.

### Historial: unicidad

Unicidad de creación completada. Fase roja guardada en `bf70e9e`: ejecución filtrada `FullyQualifiedName~NombreUnico|FullyQualifiedName~SqliteCategoryIntegrityTests`, código 1, 11 fallos y 2 aprobadas. Duplicados recibían 201 en vez de 409; SQL directo y actualización desde InitialCategories no restringían duplicados.

Fase verde: `dotnet test Catalogo.slnx --no-restore --logger "console;verbosity=minimal"`, código 0, 24 aprobadas y 0 omitidas: 20 BDD y 4 técnicas. Casos incluyen mayúsculas, Ñ, ó compuesta/descompuesta, espacios externos, conservación de la original al reiniciar, acento distinto de ausencia de acento, espacios internos significativos, restricción mediante SQL directo y migración desde el esquema anterior.

Implementación: CategoryNameCollation compara Trim + NFC + ToUpperInvariant + NFC ordinalmente. CatalogoDbContext registra CATEGORY_NAME en cada conexión; Name utiliza esa collation y un índice único. Endpoint comprueba duplicado antes del guardado y captura SQLite 2067 por conflicto de índice; devuelve Problem Details 409 con "Ya existe una categoría con ese nombre.". Migración UniqueCategoryNames generada mediante dotnet-ef (código 0) y aplicada en pruebas, conservando datos anteriores. Base local sigue ausente después de las pruebas.

Limitaciones documentadas: herramientas externas deben registrar la misma collation; datos previos duplicados impiden crear el índice y no se borran automáticamente. Algoritmo explícito de casing invariable, no comparación lingüística para todas las culturas. Aún no hay edición ni se ha comprobado el camino de captura con una carrera HTTP controlada; sí se comprobó la restricción SQLite.

### Historial: validación de nombre

Nombre requerido y espacios completados. Fase roja guardada en `c4b15fe`: 7 nuevos casos ejecutados, 7 fallos y 0 omitidos; nombres ausentes/null devolvían 500, blancos se guardaban y espacios externos se conservaban. Ejemplos de blancos usan escapes Unicode JSON para evitar que Gherkin interprete saltos de línea.

Fase verde: `dotnet test Catalogo.slnx --no-restore --logger "console;verbosity=minimal"`, código 0, 11 casos aprobados y 0 omitidos. `string.IsNullOrWhiteSpace` rechaza nombre ausente, null, vacío, espacios, tabulaciones y saltos; `Results.ValidationProblem` devuelve 400, application/problem+json y errors.Name con "El nombre es obligatorio.". `Trim()` elimina blancos externos y conserva los internos; los escenarios comprueban ausencia de filas tras rechazo y nombre persistente al reiniciar. Contrato de entrada nullable para validar explícitamente; entidad sigue con Name requerido. No cambió el esquema ni se generó migración.

Estado actual: creación válida, persistencia, lista, detalle creado y validación de nombre completos. Pendiente unicidad, edición, eliminaciones, productos y errores adicionales.

### Historial de etapas anteriores

Etapa 4 en curso: creación válida, descripción opcional, detalle, lista y conservación al reiniciar completados. Fase roja guardada en `14ff828`: 3 nuevos casos fallaron por POST 405 en lugar de 201; lista vacía pasó (total 4). Fase verde: `dotnet test Catalogo.slnx --no-restore --logger "console;verbosity=minimal"`, código 0, 4 casos aprobados, ninguno omitido.

Se agregaron Category, CreateCategoryRequest, CatalogoDbContext y EF SQLite/Design 10.0.12. InitialCategories fue generada con `dotnet tool run dotnet-ef migrations add InitialCategories --project src/Catalogo.Api --output-dir Data/Migrations`, código 0, compilación correcta. `dotnet tool restore` comprobado. Program aplica migraciones al inicio; la conexión predeterminada apunta a src/Catalogo.Api/catalogo.db. ApiScenario usa ruta temporal única y Pooling=False; reinicia conservando el archivo y elimina solo sus archivos al finalizar. `Test-Path src/Catalogo.Api/catalogo.db` devolvió False después de la suite: las pruebas no crearon la base local de la aplicación.

Limitaciones actuales: solo creación válida; nombres vacíos, espacios y duplicados todavía no están validados. El detalle devuelve 404 para inexistentes, pero ese caso aún no tiene escenario BDD. No hay edición, eliminación ni productos.

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

Explicar validaciones de productos. Después: escenarios de edición de producto, incluido cambio de categoría, datos inválidos, inexistentes y conservación de datos tras rechazo. Comprobar fallos antes de implementar PUT. Usuario con experiencia web, nuevo en C#/.NET; seguir explicando por etapas.

## Pendientes

Edición de productos, eliminaciones de ambas entidades; cobertura restante de integridad/restricción de eliminación, ejecución limpia, documentación de demostración y guion del video. Categorías (creación/consulta/edición y validaciones) y productos (creación/consulta/validaciones, nombres repetidos y relación observable), clave foránea por SQL directo, migraciones y reinicio persistente ya comprobados.

## Decisiones y dudas

Seguir las tecnologías y el contrato HTTP de AGENTS.md. Supuestos y dudas para el contacto registrados en README.md.

## GitHub

Con autorización del usuario se creó el repositorio privado https://github.com/Lujam2299/Prueba-Tecnica y se configuró como `origin`. `git push -u origin main` terminó con código 0; se verificaron en GitHub la etiqueta Private y los cuatro archivos de preparación. Nombre elegido para la URL: Prueba-Tecnica; descripción: Prueba Técnica.

Con autorización del usuario se subieron los cinco commits pendientes hasta `4e4935c` mediante `git push origin main` (código 0). `git ls-remote origin refs/heads/main` coincidió con `git rev-parse HEAD`; `git status --short --branch` mostró main sincronizada con origin/main y sin cambios. Incluye guía, estructura, punto de control, escenario rojo e implementación verde. Política acordada: commits pequeños y push al cerrar etapas o sesiones.

### Edición de productos: fase roja

19 casos nuevos ejecutados con filtro FullyQualifiedName~EdicionDeProductos: código 1, todos fallidos por HTTP 405, 0 omitidos. Cubren edición, cambio de categoría, descripción opcional, nombres repetidos, validaciones, inexistentes y conservación de datos al reiniciar. Implementación pendiente.
