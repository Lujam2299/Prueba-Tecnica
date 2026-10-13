# Guía de desarrollo

Referencia compartida para saber dónde estamos y qué sigue. Actualizaremos las casillas al terminar cada etapa y comprobar su resultado. `PROGRESS.md` conserva los comandos, resultados, decisiones y problemas reales; `AGENTS.md` define las reglas del proyecto.

## Punto actual

**Etapas 1–8 completadas; etapa 9 en curso.** Última suite verificada: 92 pruebas aprobadas (85 BDD y 7 técnicas). Guion personal creado solo en local, video grabado según confirmación del usuario y README reorganizado para la entrega, en revisión personal. Falta publicar y verificar el enlace del video, confirmar acceso al repositorio, finalizar el repaso. Deploy descartado por decisión del usuario.

## Ciclo que repetiremos por comportamiento

1. Explicar la regla de negocio y escribir su escenario Gherkin en español.
2. Conectar los pasos del escenario con peticiones HTTP y verificaciones (bindings de Reqnroll).
3. Ejecutar la prueba y comprobar que falla por la funcionalidad pendiente. Un error de configuración o compilación no cuenta como esa evidencia.
4. Implementar lo mínimo para cumplir el comportamiento.
5. Ejecutar las pruebas relacionadas y revisar el código juntos.
6. Registrar el avance en un commit pequeño y actualizar esta guía y PROGRESS.md.

No escribiremos toda la API de una sola vez. Al cerrar cada bloque revisaremos qué hace el código y cómo podrías modificarlo en una entrevista.

## Día 1: base y funcionalidad principal

### 1. Preparar el entorno — completado

- [x] Leer requisitos y acordar alcance de dos días.
- [x] Revisar Git y VS Code.
- [x] Instalar y verificar SDK de .NET 10.
- [x] Crear Git local, README, PROGRESS y exclusiones de archivos generados.
- [x] Crear repositorio privado en GitHub y subir la preparación.

Resultado comprobado: SDK 10.0.401 instalado y repositorio privado `Lujam2299/Prueba-Tecnica` disponible.

### 2. Crear la solución — completado

- [x] Crear una solución que agrupe los dos proyectos.
- [x] Crear `src/Catalogo.Api` con Minimal APIs para `net10.0`.
- [x] Crear `tests/Catalogo.AcceptanceTests` para Reqnroll y NUnit.
- [x] Fijar versiones compatibles de los paquetes y del SDK.
- [x] Comprobar restauración de dependencias y compilación.
- [x] Documentar comandos comprobados y guardar el avance.

Conceptos a explicar: una solución agrupa proyectos; la API recibe peticiones HTTP; el proyecto de pruebas verifica su comportamiento.

### 3. Ejecutar el primer escenario BDD

- [x] Configurar Reqnroll, NUnit y el arranque de la API desde las pruebas.
- [x] Preparar aislamiento de datos para cada escenario (archivo SQLite temporal único, eliminado al finalizar).
- [x] Escribir primero: «Consultar categorías cuando no hay ninguna devuelve una lista vacía».
- [x] Crear sus bindings HTTP y comprobar el fallo por el endpoint pendiente.
- [x] Implementar ese comportamiento con respuesta 200 y lista vacía.
- [x] Comprobar que el escenario pasa y registrar el ciclo BDD.

Resultado esperado: una prueba de aceptación real ejecutable. Este bloque resuelve temprano la integración de herramientas.

### 4. Desarrollar categorías por comportamientos

- [x] Configurar EF Core 10, SQLite en archivo y migraciones conforme los escenarios requieran persistencia.
- [x] Crear categoría válida: descripción opcional, respuesta 201 y ubicación del detalle.
- [x] Validar nombres ausentes, null, vacíos y blancos; quitar espacios externos conservando los internos.
- [x] Definir, documentar y probar la estrategia Unicode de unicidad también en la base: mayúsculas ignoradas, acentos conservados, ejemplo «Electrónica».
- [x] Consultar lista y detalle de categorías creadas.
- [x] Cubrir por BDD el detalle inexistente con 404.
- [x] Editar categoría y validar conflictos con otras; conservar su propio nombre es válido.

Resultado esperado: creación, consulta y edición con escenarios aprobados y contratos de entrada/salida, sin devolver entidades EF directamente.

### 5. Desarrollar productos por comportamientos

- [x] Definir relación obligatoria con una categoría existente y protegerla con clave foránea (rechazo comprobado por HTTP y SQL directo).
- [x] Crear producto válido con descripción opcional; permitir nombres repetidos.
- [x] Validar nombre requerido y quitar blancos externos del nombre de producto.
- [x] Rechazar categoría ausente o inexistente con 400.
- [x] Consultar lista y detalle mostrando identificador y nombre actual de categoría; cubrir detalle inexistente.
- [x] Editar producto, incluido cambio de categoría.
- [x] Verificar recursos inexistentes y validaciones en edición.

Resultado esperado: creación, consulta y edición de productos verificadas mediante HTTP.

## Día 2: restricciones, verificación y entrega

### 6. Completar eliminaciones

- [x] Eliminar productos con 204; recurso inexistente con 404.
- [x] Eliminar categorías sin productos con 204.
- [x] Rechazar categorías con productos con 409 y el mensaje exacto: `Esta categoria no se puede eliminar porque tiene productos asignados`.
- [x] Comprobar que el rechazo conserva categoría y productos.
- [x] Restringir la eliminación también en la base de datos.
- [x] Comprobar que retirar todos los productos permite eliminar la categoría.

### 7. Completar cobertura e integridad

- [x] Revisar todos los escenarios obligatorios contra AGENTS.md.
- [x] Mantener errores estructurados y consistentes, preferentemente Problem Details.
- [x] Comprobar persistencia después de reiniciar la aplicación.
- [x] Comprobar unicidad y relación protegidas por la base.
- [x] Comprobar que las pruebas no usan la base local de la API ni dependen del orden.
- [x] Ejecutar la suite completa y registrar resultados reales.

### 8. Preparar la demostración y comprobar reproducibilidad

- [x] Añadir documentación OpenAPI y una forma sencilla de ejecutar peticiones.
- [x] Completar README: instalación, configuración, migraciones, ejecución, pruebas y ejemplos.
- [x] Registrar decisiones, supuestos, limitaciones y uso de IA; revisión con el candidato pendiente en etapa 9.
- [x] Comprobar las instrucciones desde un directorio limpio.
- [x] Revisar Git y subir los avances autorizados al repositorio.

### 9. Preparar la entrega

- [x] Crear un guion corto que demuestre los requisitos, guardado solo en local y excluido de Git.
- [x] Realizar pruebas manuales con Swagger (confirmado por el usuario).
- [x] Repasar el flujo de ProductEndpoints, CatalogoDbContext e integración Gherkin/Reqnroll/NUnit.
- [ ] Completar el repaso de migraciones y arranque de la aplicación.
- [x] Usuario: grabar el video (confirmado por el usuario; su contenido no se revisó aquí).
- [ ] Publicar el video y comprobar que su enlace permita al evaluador reproducirlo.
- [x] Reorganizar README con ejecución paso a paso, decisiones y motivos, supuestos y revisión del uso de IA.
- [ ] Usuario: personalizar y revisar el README final, añadir el enlace del video y subir la versión de entrega.
- [ ] Revisar acceso del evaluador al repositorio privado o decidir hacerlo público con autorización.
- [ ] Confirmar enlaces de entrega y requisitos completos.

### 10. Extra opcional

Deploy considerado y descartado por decisión del usuario. No forma parte de los pendientes de entrega; no hay alojamiento público verificado.

## Tiempo y actualización

Estimación inicial: 14–19 horas de trabajo concentrado, repartidas en dos días. Revisaremos el tiempo real al cerrar cada etapa. Si el plazo peligra, recortaremos extras y comunicaremos pendientes sin omitir requisitos.

Al retomar una sesión: leer AGENTS.md, README.md, PROGRESS.md y esta guía; revisar Git; continuar con la primera etapa pendiente. Marcar una casilla requiere evidencia, no solo haber intentado la acción.
