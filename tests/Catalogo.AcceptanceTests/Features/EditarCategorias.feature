# language: es
Característica: Edición de categorías
  Como usuario de la API
  Quiero actualizar categorías sin perder su identidad ni crear nombres duplicados

  Antecedentes:
    Dado que no existen categorías
    Y existe una categoría para editar llamada "Electrónica" con descripción "Original"

  Esquema del escenario: Guardar una edición válida conservando el identificador
    Cuando edito la categoría con el cuerpo '<cuerpo>'
    Entonces la categoría se actualiza a "<nombre>" con descripción "<descripcion>"
    Y la edición queda disponible después de reiniciar

    Ejemplos:
      | cuerpo                                                | nombre      | descripcion |
      | {"name":"Tecnología","description":"Actualizada"}     | Tecnología  | Actualizada |
      | {"name":"Electrónica","description":"Actualizada"}    | Electrónica | Actualizada |
      | {"name":"ELECTRÓNICA","description":"Actualizada"}    | ELECTRÓNICA | Actualizada |
      | {"name":"  Electrónica  ","description":"Original"}   | Electrónica | Original    |
      | {"name":"Electronica","description":"Sin acento"}    | Electronica | Sin acento  |
      | {"name":"Electrónica"}                                | Electrónica |             |

  Esquema del escenario: Rechazar una edición sin nombre válido
    Cuando edito la categoría con el cuerpo '<cuerpo>'
    Entonces recibo un error de validación del nombre
    Y las categorías conservan sus datos anteriores

    Ejemplos:
      | cuerpo                         |
      | {}                             |
      | {"name":null}                  |
      | {"name":""}                    |
      | {"name":"   "}                 |
      | {"name":"\u0009\u000a"}        |

  Esquema del escenario: Rechazar el nombre de otra categoría sin modificar ninguna
    Dado que también existe la categoría "<otra>"
    Cuando edito la categoría con el cuerpo '<cuerpo>'
    Entonces recibo un conflicto por nombre de categoría duplicado
    Y las categorías conservan sus datos anteriores

    Ejemplos:
      | otra       | cuerpo                                                        |
      | Libros     | {"name":"  LIBROS  ","description":"No guardar"}              |
      | Tecnología | {"name":"TECNOLOGÍA","description":"No guardar"}              |
      | Tecnología | {"name":"TECNOLOGI\u0301A","description":"No guardar"}        |

  Esquema del escenario: Informar que la categoría a editar no existe
    Cuando edito una categoría inexistente con el cuerpo '<cuerpo>'
    Entonces recibo un error de categoría inexistente
    Y las categorías conservan sus datos anteriores

    Ejemplos:
      | cuerpo                |
      | {"name":"Libros"}     |
      | {"name":""}           |

  Escenario: Consultar el detalle de una categoría inexistente
    Cuando consulto el detalle de una categoría inexistente
    Entonces recibo un error de categoría inexistente
