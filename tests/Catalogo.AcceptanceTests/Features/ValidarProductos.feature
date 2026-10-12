# language: es
Característica: Validación al crear productos
  Como usuario de la API
  Quiero crear productos con nombre y categoría válida sin alterar datos ante errores

  Antecedentes:
    Dado que no existen categorías
    Y existe una categoría válida para validar productos

  Esquema del escenario: Rechazar datos inválidos sin guardar el producto
    Cuando intento crear un producto con el cuerpo '<cuerpo>'
    Entonces recibo un error de producto en "<campo>" con mensaje "<mensaje>"
    Y no se guarda ningún producto ni se alteran las categorías

    Ejemplos:
      | cuerpo                                                | campo      | mensaje                          |
      | {"categoryId":$categoryId}                             | Name       | El nombre es obligatorio.        |
      | {"name":null,"categoryId":$categoryId}                 | Name       | El nombre es obligatorio.        |
      | {"name":"","categoryId":$categoryId}                   | Name       | El nombre es obligatorio.        |
      | {"name":"   ","categoryId":$categoryId}                | Name       | El nombre es obligatorio.        |
      | {"name":"\u0009\u000a","categoryId":$categoryId}       | Name       | El nombre es obligatorio.        |
      | {"name":"Teclado"}                                    | CategoryId | La categoría es obligatoria.     |
      | {"name":"Teclado","categoryId":null}                  | CategoryId | La categoría es obligatoria.     |
      | {"name":"Teclado","categoryId":0}                     | CategoryId | La categoría es obligatoria.     |
      | {"name":"Teclado","categoryId":-1}                    | CategoryId | La categoría es obligatoria.     |
      | {"name":"Teclado","categoryId":2147483647}            | CategoryId | La categoría indicada no existe. |

  Escenario: Informar ambos campos requeridos ausentes
    Cuando intento crear un producto con el cuerpo '{}'
    Entonces recibo un error de producto en "Name" con mensaje "El nombre es obligatorio."
    Y recibo un error de producto en "CategoryId" con mensaje "La categoría es obligatoria."
    Y no se guarda ningún producto ni se alteran las categorías

  Esquema del escenario: Guardar nombre sin blancos externos conservando los internos
    Cuando intento crear un producto con el cuerpo '<cuerpo>'
    Entonces el producto queda guardado con el nombre "Teclado  mecánico"

    Ejemplos:
      | cuerpo                                                                                           |
      | {"name":"  Teclado  mecánico  ","description":"Original","categoryId":$categoryId}                |
      | {"name":"\u0009Teclado  mecánico\u000a","description":"Original","categoryId":$categoryId}        |
