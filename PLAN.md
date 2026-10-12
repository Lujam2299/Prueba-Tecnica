# Guía de desarrollo

Referencia compartida para saber dónde estamos y qué sigue. Actualizaremos las casillas al terminar cada etapa y comprobar su resultado. `PROGRESS.md` conserva los comandos, resultados, decisiones y problemas reales; `AGENTS.md` define las reglas del proyecto.

## Punto actual

**Etapa 5 completada: creación, consulta, validación y edición de productos, incluido cambio de categoría.** Suite de 81 pruebas aprobada (76 BDD y 5 técnicas). Lo siguiente es la etapa 6: eliminaciones y restricciones.

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

- [ ] Eliminar productos con 204; recurso inexistente con 404.
- [ ] Eliminar categorías sin productos con 204.
- [ ] Rechazar categorías con productos con 409 y el mensaje exacto: `Esta categoria no se puede eliminar porque tiene productos asignados`.
- [ ] Comprobar que el rechazo conserva categoría y productos.
- [ ] Restringir la eliminación también en la base de datos.
- [ ] Comprobar que retirar todos los productos permite eliminar la categoría.

### 7. Completar cobertura e integridad

- [ ] Revisar todos los escenarios obligatorios contra AGENTS.md.
- [ ] Mantener errores estructurados y consistentes, preferentemente Problem Details.
- [x] Comprobar persistencia después de reiniciar la aplicación.
- [ ] Comprobar unicidad y relación protegidas por la base.
- [ ] Comprobar que las pruebas no usan la base local de la API ni dependen del orden.
- [ ] Ejecutar la suite completa y registrar resultados reales.

### 8. Preparar la demostración y comprobar reproducibilidad

- [ ] Añadir documentación OpenAPI y una forma sencilla de ejecutar peticiones.
- [ ] Completar README: instalación, configuración, migraciones, ejecución, pruebas y ejemplos.
- [ ] Registrar decisiones, supuestos, limitaciones y uso de IA revisado por el usuario.
- [ ] Comprobar las instrucciones desde un directorio limpio.
- [ ] Revisar Git y subir los avances autorizados al repositorio.

### 9. Preparar la entrega

- [ ] Crear un guion corto que demuestre todos los requisitos.
- [ ] Repasar el código y practicar un cambio pequeño en vivo.
- [ ] Usuario: grabar y publicar el video, y comprobar su enlace.
- [ ] Revisar acceso del evaluador al repositorio privado o decidir hacerlo público con autorización.
- [ ] Confirmar enlaces de entrega y requisitos completos.

### 10. Extra opcional

- [ ] Considerar deploy únicamente si las etapas principales están completas y queda tiempo.

## Tiempo y actualización

Estimación inicial: 14–19 horas de trabajo concentrado, repartidas en dos días. Revisaremos el tiempo real al cerrar cada etapa. Si el plazo peligra, recortaremos extras y comunicaremos pendientes sin omitir requisitos.

Al retomar una sesión: leer AGENTS.md, README.md, PROGRESS.md y esta guía; revisar Git; continuar con la primera etapa pendiente. Marcar una casilla requiere evidencia, no solo haber intentado la acción.
