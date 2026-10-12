# language: es
Característica: Nombre requerido de categoría
  Para identificar las categorías
  Como usuario de la API
  Quiero que los nombres tengan contenido y se guarden sin espacios externos

  Esquema del escenario: Rechazar una categoría sin nombre válido
    Dado que no existen categorías
    Cuando intento crear una categoría con el cuerpo '<cuerpo>'
    Entonces recibo un error de validación del nombre
    Y no se guarda ninguna categoría

    Ejemplos:
      | cuerpo                    |
      | {}                        |
      | {"name":null}             |
      | {"name":""}               |
      | {"name":"   "}            |
      | {"name":"\u0009\u000d\u000a"} |

  Esquema del escenario: Quitar espacios externos sin cambiar el interior del nombre
    Dado que no existen categorías
    Cuando intento crear una categoría con el cuerpo '<cuerpo>'
    Entonces la categoría queda guardada con el nombre "Audio  y video"

    Ejemplos:
      | cuerpo                          |
      | {"name":"  Audio  y video  "}   |
      | {"name":"\u0009Audio  y video\u000a"} |
