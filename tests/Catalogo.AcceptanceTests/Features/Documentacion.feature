# language: es
Característica: Documentación para probar la API
  Como evaluador
  Quiero conocer las operaciones sin depender de un IDE

  Escenario: Consultar el contrato y la interfaz de pruebas
    Dado que no existen categorías
    Cuando consulto la documentación de la API
    Entonces el contrato describe las diez operaciones y sus respuestas
    Y la interfaz de pruebas está disponible en el navegador
