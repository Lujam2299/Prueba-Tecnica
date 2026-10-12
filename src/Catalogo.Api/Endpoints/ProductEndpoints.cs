using Catalogo.Api.Contracts;
using Catalogo.Api.Data;
using Catalogo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        app.MapGet("/api/products", async (CatalogoDbContext db) =>
            Results.Ok(await Responses(db.Products.OrderBy(product => product.Id)).ToListAsync()));

        app.MapGet("/api/products/{id:int}", async (int id, CatalogoDbContext db) =>
        {
            var product = await Responses(db.Products.Where(product => product.Id == id)).SingleOrDefaultAsync();
            return product is null
                ? Results.Problem(statusCode: StatusCodes.Status404NotFound,
                    title: "Producto no encontrado.",
                    detail: "No existe un producto con el identificador indicado.")
                : Results.Ok(product);
        });

        app.MapPost("/api/products", async (ProductRequest request, CatalogoDbContext db) =>
        {
            // Este bloque implementa creación válida; las validaciones se agregan con sus escenarios.
            var product = new Product
            {
                Name = request.Name!,
                Description = request.Description,
                CategoryId = request.CategoryId.GetValueOrDefault()
            };
            db.Products.Add(product);
            await db.SaveChangesAsync();

            var response = await Responses(db.Products.Where(saved => saved.Id == product.Id)).SingleAsync();
            return Results.Created($"/api/products/{product.Id}", response);
        });
    }

    // EF traduce la navegación a una consulta que obtiene el nombre actual de la categoría.
    private static IQueryable<ProductResponse> Responses(IQueryable<Product> products) =>
        products.AsNoTracking().Select(product => new ProductResponse(
            product.Id, product.Name, product.Description, product.CategoryId, product.Category.Name));
}
