using Catalogo.Api.Endpoints;
using Catalogo.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Catalogo")
    ?? $"Data Source={Path.Combine(builder.Environment.ContentRootPath, "catalogo.db")}";
builder.Services.AddDbContext<CatalogoDbContext>(options => options.UseSqlite(connectionString));

var app = builder.Build();

// Para esta prueba local, aplicar las migraciones al iniciar simplifica la ejecución.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogoDbContext>();
    await db.Database.MigrateAsync();
}

app.MapCategoryEndpoints();
app.MapProductEndpoints();
app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Catálogo API v1");
    options.DocumentTitle = "Catálogo: categorías y productos";
});

app.Run();

// Permite que el proyecto de pruebas identifique el punto de entrada de la API.
public partial class Program { }
