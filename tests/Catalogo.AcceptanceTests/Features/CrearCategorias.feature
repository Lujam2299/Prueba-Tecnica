# language: es
Característica: Creación de categorías
  Como usuario de la API
  Quiero guardar categorías para organizar productos

  Esquema del escenario: Crear una categoría válida con descripción opcional
    Dado que no existen categorías
    Cuando creo la categoría "Electrónica" con descripción "<descripcion>"
    Entonces la categoría se crea correctamente
    Y puedo consultar la categoría creada
    Y la lista contiene la categoría creada

    Ejemplos:
      | descripcion          |
      | Dispositivos y audio |
      |                      |

  Escenario: Conservar una categoría después de reiniciar la aplicación
    Dado que no existen categorías
    Cuando creo la categoría "Libros" con descripción "Lectura"
    Entonces la categoría se crea correctamente
    Cuando reinicio la aplicación
    Entonces puedo consultar la categoría creada
    Y la lista contiene la categoría creada
