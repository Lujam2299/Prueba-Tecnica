# language: es
Característica: Consulta de categorías
  Para conocer las categorías disponibles
  Como usuario de la API
  Quiero consultar su lista

  Escenario: Consultar cuando no hay categorías
    Dado que no existen categorías
    Cuando consulto la lista de categorías
    Entonces la consulta es exitosa
    Y recibo una lista vacía
