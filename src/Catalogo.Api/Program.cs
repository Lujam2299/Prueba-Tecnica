var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Run();

// Permite que el proyecto de pruebas identifique el punto de entrada de la API.
public partial class Program { }
