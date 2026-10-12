# language: es
Característica: Nombre único de categoría
  Como usuario de la API
  Quiero evitar categorías repetidas aunque cambie el uso de mayúsculas

  Esquema del escenario: Rechazar un nombre equivalente al de una categoría existente
    Dado que no existen categorías
    Y existe una categoría original llamada "<original>"
    Cuando intento crear otra categoría llamada "<nuevo>"
    Entonces recibo un conflicto por nombre de categoría duplicado
    Y la única categoría sigue siendo la original

    Ejemplos:
      | original    | nuevo          |
      | Libros      | Libros         |
      | Libros      | LIBROS         |
      | Electrónica | ELECTRÓNICA    |
      | Electrónica |   electrónica  |
      | Electrónica | ELECTRÓNICA    |
      | Niñez       | NIÑEZ          |

  Escenario: Detectar un duplicado después de quitar espacios externos
    Dado que no existen categorías
    Y existe una categoría original llamada "Electrónica"
    Cuando intento crear otra categoría llamada "  ELECTRÓNICA  "
    Entonces recibo un conflicto por nombre de categoría duplicado
    Y la única categoría sigue siendo la original

  Esquema del escenario: Permitir nombres que tienen diferencias significativas
    Dado que no existen categorías
    Y existe una categoría original llamada "<original>"
    Cuando intento crear otra categoría llamada "<nuevo>"
    Entonces ambas categorías quedan disponibles

    Ejemplos:
      | original       | nuevo         |
      | Electrónica    | Electronica   |
      | Audio  y video | Audio y video |
