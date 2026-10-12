# Revisión de cobertura

Revisión del paso 7, 12 de octubre de 2026. Esta tabla relaciona los requisitos de AGENTS.md con pruebas ejecutables; no es una medición de porcentaje de líneas de código.

Los archivos `.feature` están en `tests/Catalogo.AcceptanceTests/Features/`; sus bindings en `Steps/` realizan peticiones HTTP y comprueban respuestas y datos observables. Las comprobaciones técnicas complementarias están en `Support/`.

| Requisito | Evidencia |
| --- | --- |
| Lista de categorías vacía: 200 | ConsultarCategorias.feature |
| Crear categorías, descripción opcional, 201 y Location, lista y detalle | CrearCategorias.feature y CrearCategoriasSteps |
| Nombres requeridos y blancos externos | ValidarNombreCategoria.feature, ValidarProductos.feature y esquemas de edición |
| Categorías únicas sin distinguir mayúsculas, acentos y Unicode compuesto/descompuesto | NombreUnicoCategoria.feature y SqliteCategoryIntegrityTests |
| Acento diferente de ausencia de acento; espacios internos significativos | NombreUnicoCategoria.feature |
| Edición de categoría conserva id y acepta su propio nombre; rechaza conflicto con otra | EditarCategorias.feature |
| Crear productos, descripción opcional, nombres repetidos, 201 y Location | CrearConsultarProductos.feature y CrearConsultarProductosSteps |
| Categoría obligatoria y existente; errores de ambos campos | ValidarProductos.feature y EditarProductos.feature |
| Lista y detalle de producto incluyen id y nombre actual de categoría | CrearConsultarProductos.feature y EditarProductos.feature |
| Cambiar de categoría sin alterar otros recursos | EditarProductos.feature |
| GET/PUT/DELETE inexistentes: 404 Problem Details | CrearConsultarProductos.feature, EditarCategorias.feature, EditarProductos.feature y ambas características de eliminación |
| Eliminar productos con 204 sin cuerpo; segunda eliminación: 404 | EliminarProductos.feature |
| Eliminar categorías sin productos con 204; segunda eliminación: 404 | EliminarCategorias.feature |
| Categoría con productos: 409, mensaje exacto y conservación de datos | EliminarCategorias.feature |
| Retirar uno no basta; retirar todos permite eliminar la categoría | EliminarCategorias.feature |
| Persistencia después de reiniciar: creación, edición, eliminación y rechazos | Escenarios y bindings de creación, edición, validaciones y eliminación |
| Unicidad protegida por índice SQLite, incluso al omitir la API | SqliteCategoryIntegrityTests: tres casos de nombres equivalentes |
| Migraciones conservan datos del esquema anterior | SqliteCategoryIntegrityTests.MigrationPreservesExistingCategoryAndProtectsItsName |
| Clave foránea rechaza producto sin categoría y RESTRICT impide borrar categoría con productos | SqliteProductIntegrityTests: SQLite 787 y 1811, respectivamente |
| Bases separadas y cambios sin efecto en otra aplicación de prueba | ScenarioIsolationTests.IndependentApplicationsDoNotShareData |

## Verificación

Comando: `dotnet test Catalogo.slnx --no-restore --logger "console;verbosity=minimal"`.

Resultado real: código 0, **91 aprobadas: 84 casos BDD y 7 comprobaciones técnicas**, 0 fallidas y 0 omitidas. La prueba de aislamiento es una comprobación de infraestructura añadida durante la auditoría; pasó sobre el soporte existente, sin cambios de implementación ni una fase roja artificial.

ApiScenario crea una ruta temporal única con Guid por instancia y sustituye la conexión antes del arranque; no hay estado de datos estático compartido entre escenarios. Reiniciar conserva únicamente su archivo y Dispose elimina ese archivo y sus auxiliares. La prueba de aislamiento mantiene dos aplicaciones abiertas, crea el mismo nombre en ambas y comprueba tras reiniciar que eliminar en una no altera la otra. La prueba de actualización de esquema también usa una ruta temporal propia.

`Test-Path src/Catalogo.Api/catalogo.db` devolvió False después de ejecutar la suite. No se utilizó ni se creó la base local de la aplicación. La independencia se respalda en el diseño de los fixtures y en la prueba de dos aplicaciones; no se ejecutaron todas las permutaciones posibles del orden de las 91 pruebas.

## Límites y trabajo pendiente

Los errores de negocio comprobados usan Problem Details: validaciones 400, inexistentes 404 y conflictos 409. Esta auditoría no amplía el contrato a cuerpos JSON malformados, tipos JSON incorrectos o errores inesperados del servidor.

No se provocaron carreras HTTP controladas entre consultar y guardar; sí se comprobaron las restricciones SQLite mediante SQL directo. La comparación Unicode es la estrategia explícita documentada en README, no una equivalencia lingüística universal.

OpenAPI, peticiones de demostración y reproducibilidad desde un directorio limpio corresponden al paso 8 y siguen pendientes. La comprensión del candidato y la grabación del video se revisarán en el paso 9.
