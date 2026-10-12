# language: es
Característica: Eliminación de productos
  Como usuario de la API
  Quiero retirar productos sin perder su categoría ni otros productos

  Antecedentes:
    Dado que no existen categorías
    Y hay dos productos para comprobar su eliminación

  Escenario: Eliminar un producto conserva los demás datos después de reiniciar
    Cuando elimino el primer producto
    Entonces la eliminación del producto responde sin contenido
    Y el producto eliminado ya no está disponible y los demás datos se conservan

  Escenario: Eliminar un producto inexistente conserva los datos
    Cuando elimino un producto que no existe
    Entonces recibo un error de producto inexistente
    Y la eliminación rechazada conserva ambos productos y su categoría

  Escenario: Eliminar nuevamente un producto devuelve recurso inexistente
    Cuando elimino el primer producto
    Entonces la eliminación del producto responde sin contenido
    Cuando elimino el primer producto
    Entonces recibo un error de producto inexistente
    Y el producto eliminado ya no está disponible y los demás datos se conservan
