# language: es
Característica: Creación y consulta de productos
  Como usuario de la API
  Quiero guardar productos y conocer a qué categoría pertenecen

  Escenario: Consultar productos cuando no existe ninguno
    Dado que no existen categorías
    Cuando consulto la lista de productos
    Entonces la consulta es exitosa
    Y recibo una lista vacía

  Esquema del escenario: Crear un producto con descripción opcional
    Dado que no existen categorías
    Y existe una categoría para productos llamada "Electrónica"
    Cuando creo el producto "Teclado" con descripción "<descripcion>"
    Entonces el producto se crea con su categoría
    Y puedo consultar el producto creado
    Y la lista contiene el producto con su categoría

    Ejemplos:
      | descripcion       |
      | Teclado mecánico  |
      |                   |

  Escenario: Conservar el producto y su categoría después de reiniciar
    Dado que no existen categorías
    Y existe una categoría para productos llamada "Electrónica"
    Cuando creo el producto "Teclado" con descripción "Mecánico"
    Entonces el producto se crea con su categoría
    Cuando reinicio la aplicación
    Entonces puedo consultar el producto creado
    Y la lista contiene el producto con su categoría

  Escenario: Permitir dos productos con el mismo nombre
    Dado que no existen categorías
    Y existe una categoría para productos llamada "Electrónica"
    Cuando creo el producto "Teclado" con descripción "Primero"
    Entonces el producto se crea con su categoría
    Cuando creo otro producto con el mismo nombre
    Entonces ambos productos quedan disponibles

  Escenario: Mostrar el nombre actual de la categoría del producto
    Dado que no existen categorías
    Y existe una categoría para productos llamada "Electrónica"
    Cuando creo el producto "Teclado" con descripción "Mecánico"
    Entonces el producto se crea con su categoría
    Cuando cambio el nombre de la categoría del producto a "Tecnología"
    Entonces puedo consultar el producto creado
    Y la lista contiene el producto con su categoría

  Escenario: Consultar un producto inexistente
    Dado que no existen categorías
    Cuando consulto un producto inexistente
    Entonces recibo un error de producto inexistente
