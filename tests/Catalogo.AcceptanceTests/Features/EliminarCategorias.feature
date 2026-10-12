# language: es
Característica: Eliminación de categorías
  Como usuario de la API
  Quiero eliminar categorías sin perder productos asignados

  Antecedentes:
    Dado que no existen categorías
    Y existen dos categorías para comprobar su eliminación

  Escenario: Eliminar una categoría sin productos
    Cuando elimino la categoría de prueba
    Entonces la eliminación de categoría responde sin contenido
    Y la categoría eliminada permanece ausente al reiniciar

  Escenario: Eliminar una categoría inexistente
    Cuando elimino una categoría que no existe
    Entonces la eliminación devuelve categoría inexistente
    Y las categorías y productos de prueba conservan sus datos

  Escenario: Eliminar nuevamente una categoría
    Cuando elimino la categoría de prueba
    Entonces la eliminación de categoría responde sin contenido
    Cuando elimino la categoría de prueba
    Entonces la eliminación devuelve categoría inexistente
    Y la categoría eliminada permanece ausente al reiniciar

  Escenario: Rechazar la eliminación de una categoría con productos
    Dado que la categoría de prueba tiene dos productos
    Cuando elimino la categoría de prueba
    Entonces recibo el conflicto exacto de categoría con productos
    Y las categorías y productos de prueba conservan sus datos

  Escenario: Eliminar la categoría después de retirar todos sus productos
    Dado que la categoría de prueba tiene dos productos
    Cuando retiro un producto de la categoría de prueba
    Y elimino la categoría de prueba
    Entonces recibo el conflicto exacto de categoría con productos
    Y las categorías y productos de prueba conservan sus datos
    Cuando retiro los productos restantes de la categoría de prueba
    Y elimino la categoría de prueba
    Entonces la eliminación de categoría responde sin contenido
    Y la categoría eliminada permanece ausente al reiniciar
