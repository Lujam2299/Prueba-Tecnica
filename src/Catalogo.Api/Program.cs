using Catalogo.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapCategoryEndpoints();

app.Run();

// Permite que el proyecto de pruebas identifique el punto de entrada de la API.
public partial class Program { }
