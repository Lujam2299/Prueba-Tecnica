# language: es
Característica: Edición de productos
  Como usuario de la API
  Quiero actualizar productos y cambiar su categoría sin alterar datos ante errores

  Antecedentes:
    Dado que no existen categorías
    Y existen productos y categorías para comprobar la edición

  Esquema del escenario: Actualizar un producto conservando su identificador
    Cuando edito el producto con el cuerpo '<cuerpo>'
    Entonces el producto se actualiza a "<nombre>" con descripción "<descripcion>" en la categoría "<categoria>"
    Y el producto editado y los demás datos quedan disponibles al reiniciar

    Ejemplos:
      | cuerpo                                                                                       | nombre            | descripcion | categoria   |
      | {"name":"Actualizado","description":"Nueva","categoryId":$originalCategoryId}              | Actualizado       | Nueva       | Electrónica |
      | {"name":"Teclado","description":"Original","categoryId":$newCategoryId}                    | Teclado           | Original    | Libros      |
      | {"name":"Actualizado","description":"Nueva","categoryId":$newCategoryId}                   | Actualizado       | Nueva       | Libros      |
      | {"name":"Teclado","categoryId":$originalCategoryId}                                         | Teclado           |             | Electrónica |
      | {"name":"  Teclado  mecánico  ","description":"Original","categoryId":$newCategoryId}       | Teclado  mecánico | Original    | Libros      |
      | {"name":"Otro","description":"Repetido","categoryId":$newCategoryId}                       | Otro              | Repetido    | Libros      |

  Esquema del escenario: Rechazar una edición inválida conservando productos y categorías
    Cuando edito el producto con el cuerpo '<cuerpo>'
    Entonces recibo un error de producto en "<campo>" con mensaje "<mensaje>"
    Y los productos y categorías conservan sus datos anteriores

    Ejemplos:
      | cuerpo                                                      | campo      | mensaje                          |
      | {"categoryId":$originalCategoryId}                           | Name       | El nombre es obligatorio.        |
      | {"name":null,"categoryId":$originalCategoryId}               | Name       | El nombre es obligatorio.        |
      | {"name":"","categoryId":$originalCategoryId}                 | Name       | El nombre es obligatorio.        |
      | {"name":"   ","categoryId":$originalCategoryId}              | Name       | El nombre es obligatorio.        |
      | {"name":"\u0009\u000a","categoryId":$originalCategoryId}     | Name       | El nombre es obligatorio.        |
      | {"name":"Teclado"}                                          | CategoryId | La categoría es obligatoria.     |
      | {"name":"Teclado","categoryId":null}                        | CategoryId | La categoría es obligatoria.     |
      | {"name":"Teclado","categoryId":0}                           | CategoryId | La categoría es obligatoria.     |
      | {"name":"Teclado","categoryId":-1}                          | CategoryId | La categoría es obligatoria.     |
      | {"name":"Teclado","categoryId":2147483647}                  | CategoryId | La categoría indicada no existe. |

  Escenario: Informar ambos campos requeridos ausentes al editar
    Cuando edito el producto con el cuerpo '{}'
    Entonces recibo un error de producto en "Name" con mensaje "El nombre es obligatorio."
    Y recibo un error de producto en "CategoryId" con mensaje "La categoría es obligatoria."
    Y los productos y categorías conservan sus datos anteriores

  Esquema del escenario: Informar que el producto a editar no existe
    Cuando edito un producto inexistente con el cuerpo '<cuerpo>'
    Entonces recibo un error de producto inexistente
    Y los productos y categorías conservan sus datos anteriores

    Ejemplos:
      | cuerpo                                                |
      | {"name":"Teclado","categoryId":$originalCategoryId}   |
      | {}                                                    |
