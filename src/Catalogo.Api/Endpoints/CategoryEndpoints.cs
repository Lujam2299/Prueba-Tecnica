using Catalogo.Api.Contracts;

namespace Catalogo.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        // Primer comportamiento: aún no hay creación ni almacenamiento de categorías.
        app.MapGet("/api/categories", () => Results.Ok(Array.Empty<CategoryResponse>()));
    }
}
