# Instrucciones del proyecto: API de Categorías y Productos

## Objetivo y forma de colaboración

Construir una prueba técnica para una entrevista: una API de categorías y productos en .NET 10 con Entity Framework Core, persistencia y BDD mediante Reqnroll. El plazo de trabajo acordado es de dos días como máximo. Priorizar una entrega completa, simple y comprensible.

El usuario debe entender el código y poder explicarlo y modificarlo en vivo. Comunicarse en español. Trabajar por etapas pequeñas; no generar toda la implementación de una sola vez. Explicar brevemente el propósito de cada cambio y los conceptos nuevos. Antes de comenzar, revisar el estado del proyecto y las herramientas instaladas. No asumir experiencia previa con C#, .NET o BDD.

Esta carpeta será la raíz del proyecto. Este archivo guía el trabajo; no es necesario copiarlo dentro de cada subcarpeta.

## Alcance y tecnologías

- ASP.NET Core con Minimal APIs, dirigido a `net10.0`.
- Entity Framework Core 10 y proveedor SQLite, con versiones compatibles y fijadas en los archivos del proyecto.
- SQLite en archivo para persistir los datos entre reinicios.
- Migraciones de EF Core para crear y actualizar el esquema.
- Reqnroll y NUnit para pruebas BDD de aceptación que ejerciten la API mediante HTTP.
- Git y GitHub para control de versiones y entrega.
- Documentación OpenAPI y una forma sencilla de ejecutar peticiones para demostrar la API.
- Entregables principales: repositorio, README y video corto de demostración. El usuario grabará y publicará el video con ayuda de un guion.
- Deploy opcional solo cuando todos los requisitos principales estén completos y quede tiempo.

No agregar frontend, autenticación, Docker, paginación, CQRS, MediatR, repositorio genérico ni arquitectura de múltiples capas sin una necesidad concreta o una petición del usuario.

## Estructura inicial

Mantener dos proyectos en una solución:

```text
src/Catalogo.Api/
  Models/
  Contracts/
  Data/
  Endpoints/
tests/Catalogo.AcceptanceTests/
  Features/
  Steps/
  Support/
README.md
PROGRESS.md
AGENTS.md
```

Los nombres pueden ajustarse antes de crear la solución. Usar `Program.cs` para composición y configuración; organizar los endpoints por entidad. Extraer servicios únicamente cuando simplifiquen responsabilidades o eviten duplicación significativa. No devolver entidades de EF directamente: usar contratos de entrada y salida.

## Requisitos obligatorios

### Categorías

- Atributos: identificador, nombre requerido y descripción opcional.
- Crear, editar, listar, consultar detalle y eliminar.
- El nombre no puede repetirse, ignorando mayúsculas y minúsculas, tanto al crear como al editar.
- Editar sin cambiar el nombre debe ser válido: excluir la propia categoría al comprobar duplicados.
- No permitir eliminar una categoría con productos.
- En ese caso devolver exactamente: `Esta categoria no se puede eliminar porque tiene productos asignados`.
- Una eliminación rechazada debe conservar la categoría y sus productos.

### Productos

- Atributos: identificador, nombre requerido, descripción opcional y categoría obligatoria.
- Cada producto pertenece a una categoría existente; una categoría puede tener muchos productos.
- Crear, editar, listar, consultar detalle y eliminar.
- La edición permite cambiar de categoría.
- Las respuestas de lista y detalle deben mostrar el identificador y el nombre de la categoría.
- Después de eliminar todos los productos de una categoría, debe ser posible eliminarla.

### Persistencia e integridad

- Los datos de la aplicación sobreviven al reinicio.
- Proteger la relación mediante clave foránea y restringir la eliminación de categorías con productos en la base.
- Proteger la unicidad del nombre de categoría también en la base, con una estrategia explícita de normalización o comparación compatible con SQLite.
- No asumir que la comparación predeterminada de SQLite cubre todos los caracteres Unicode. Definir y probar la estrategia elegida, incluida una palabra con acentos.
- Las pruebas deben usar bases aisladas, sin modificar el archivo de datos de la aplicación ni depender del orden de ejecución.

## Supuestos y contrato HTTP

Registrar en README los supuestos y las respuestas que dé el contacto de la prueba. Preguntas útiles: si la interfaz de documentación basta para demostrar la API; tratamiento de espacios en nombres; longitudes máximas; confirmación de que los nombres de productos pueden repetirse.

Mientras no haya una respuesta diferente:

- Quitar espacios al inicio y final del nombre; rechazar nombres vacíos o compuestos solo por espacios.
- Permitir nombres repetidos de productos.
- No equiparar letras acentuadas con letras sin acento.
- No inventar límites de longitud silenciosamente: cualquier límite adicional debe justificarse y documentarse.
- `GET /api/categories` y `GET /api/products`: 200, con una lista vacía cuando no hay datos.
- `GET /api/categories/{id}` y `GET /api/products/{id}`: 200 o 404.
- `POST`: 201, con respuesta del recurso creado y ubicación del detalle.
- `PUT`: 200, con el recurso actualizado, o 404 si el recurso a editar no existe.
- `DELETE`: 204 o 404 si el recurso no existe.
- Datos inválidos o categoría inexistente en el cuerpo de un producto: 400.
- Nombre de categoría duplicado o eliminación de categoría con productos: 409.
- Mantener errores estructurados y consistentes, preferentemente Problem Details; conservar el mensaje obligatorio exacto en un campo visible.

## BDD: escenarios antes que implementación

Para cada comportamiento:

1. Escribir o ajustar el escenario Gherkin antes de implementar la funcionalidad.
2. Crear los bindings necesarios y ejecutar la prueba.
3. Confirmar que falla por el comportamiento pendiente; un error de compilación o configuración no demuestra ese comportamiento.
4. Implementar lo mínimo necesario para que pase.
5. Ejecutar las pruebas relacionadas y simplificar si hace falta.
6. Registrar el avance con un commit pequeño y claro.

Usar Gherkin en español con `# language: es`. Describir comportamiento de negocio, evitando detalles internos de EF o nombres de métodos. Usar esquemas de escenario y ejemplos cuando reduzcan repetición sin ocultar intención. Los bindings deben realizar peticiones HTTP y verificar respuestas y estado observable.

Cubrir: nombres requeridos; descripción opcional; duplicados de categorías con distintas mayúsculas; edición de la misma categoría y conflicto con otra; listas y detalles; categoría obligatoria y existente; cambio de categoría de un producto; nombre de categoría en respuestas; eliminaciones; mensaje exacto de restricción; eliminación de categoría tras retirar sus productos; recursos inexistentes y persistencia.

No sustituir las pruebas BDD por pruebas manuales. Agregar pruebas unitarias solo cuando una lógica concreta lo justifique.

## Git y documentación

- Revisar el estado de Git antes de editar y respetar cambios existentes del usuario.
- Realizar commits pequeños por avance coherente, con mensajes claros. No acumular toda la solución en un único commit.
- Puede separarse el commit de escenarios del de implementación para dejar evidencia del orden BDD. No reescribir el historial para simularlo.
- No hacer push, publicar ni crear un repositorio remoto sin autorización del usuario.
- Excluir archivos de base local, secretos, resultados de pruebas y carpetas `bin/` y `obj/`. Incluir las migraciones y configuración reproducible.
- Mantener README desde el inicio: requisitos, instalación, configuración, migraciones, ejecución, pruebas, ejemplos de uso, decisiones, supuestos y limitaciones.
- Documentar honestamente el uso de IA: tareas asistidas, revisión del código, pruebas ejecutadas y comprensión del usuario.
- No afirmar que una prueba pasó, que un deploy funciona o que una entrega se publicó sin comprobarlo.

## Plan de dos días

### Día 1

1. Revisar herramientas y crear solución y repositorio.
2. Resolver temprano la integración de Reqnroll: ejecutar un primer escenario.
3. Desarrollar creación, consulta y edición de categorías con sus validaciones.
4. Desarrollar creación, consulta y edición de productos con su categoría.

### Día 2

1. Completar eliminaciones y restricciones entre categorías y productos.
2. Completar escenarios de errores, integridad y persistencia.
3. Ejecutar todas las pruebas y comprobar instrucciones desde un entorno o directorio limpio.
4. Completar README y preparar un guion corto del video que muestre todos los requisitos.
5. Usar el tiempo restante para correcciones. Considerar deploy solo después de completar lo anterior.

Estimación inicial: 14–19 horas de trabajo concentrado. Revisar el plan según experiencia, disponibilidad y bloqueos reales. Si peligra el plazo, recortar extras y comunicar qué falta; no eliminar requisitos silenciosamente.

## Continuidad entre sesiones

Al iniciar cada sesión, leer este archivo, README y `PROGRESS.md` si existen; revisar `git status` y los commits recientes. No repetir trabajo ya completado ni dar por terminadas tareas sin evidencia.

Crear y mantener `PROGRESS.md` con:

- Etapa actual y siguiente paso concreto.
- Funcionalidades y escenarios completados y pendientes.
- Últimos comandos de verificación y sus resultados reales.
- Decisiones, supuestos y dudas por resolver.
- Problemas conocidos y cómo reproducirlos.

Actualizarlo al cerrar una etapa o sesión, sin incluir credenciales ni datos privados. Al finalizar una intervención, resumir brevemente qué cambió, qué se verificó y cuál es el siguiente paso. Si el usuario pide únicamente planificación o explicación, respetar ese alcance y no implementar por adelantado.
