using Catalogo.Api.Contracts;
using Catalogo.Api.Data;
using Catalogo.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

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
                ? ProductNotFound()
                : Results.Ok(product);
        });

        app.MapPost("/api/products", async (ProductRequest request, CatalogoDbContext db) =>
        {
            var errors = await Validate(request, db);
            if (errors.Count > 0)
                return InvalidProduct(errors);

            var product = new Product
            {
                Name = request.Name!.Trim(),
                Description = request.Description,
                CategoryId = request.CategoryId.GetValueOrDefault()
            };
            db.Products.Add(product);
            if (!await SaveWithoutCategoryConflict(db))
                return InvalidCategory();

            var response = await Responses(db.Products.Where(saved => saved.Id == product.Id)).SingleAsync();
            return Results.Created($"/api/products/{product.Id}", response);
        });

        app.MapPut("/api/products/{id:int}", async (int id, ProductRequest request, CatalogoDbContext db) =>
        {
            var product = await db.Products.FindAsync(id);
            if (product is null)
                return ProductNotFound();

            var errors = await Validate(request, db);
            if (errors.Count > 0)
                return InvalidProduct(errors);

            product.Name = request.Name!.Trim();
            product.Description = request.Description;
            product.CategoryId = request.CategoryId.GetValueOrDefault();
            if (!await SaveWithoutCategoryConflict(db))
                return InvalidCategory();

            var response = await Responses(db.Products.Where(saved => saved.Id == id)).SingleAsync();
            return Results.Ok(response);
        });
    }

    private static IResult ProductNotFound() =>
        Results.Problem(statusCode: StatusCodes.Status404NotFound,
            title: "Producto no encontrado.",
            detail: "No existe un producto con el identificador indicado.");

    private static IResult InvalidCategory() => InvalidProduct(new Dictionary<string, string[]>
    {
        ["CategoryId"] = ["La categoría indicada no existe."]
    });

    private static async Task<bool> SaveWithoutCategoryConflict(CatalogoDbContext db)
    {
        try
        {
            await db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException exception) when
            (exception.InnerException is SqliteException { SqliteExtendedErrorCode: 787 })
        {
            // La base protege la relación si la categoría desaparece antes de guardar.
            return false;
        }
    }

    private static async Task<Dictionary<string, string[]>> Validate(ProductRequest request, CatalogoDbContext db)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(request.Name))
            errors["Name"] = ["El nombre es obligatorio."];

        if (request.CategoryId is null or <= 0)
            errors["CategoryId"] = ["La categoría es obligatoria."];
        else if (!await db.Categories.AnyAsync(category => category.Id == request.CategoryId.Value))
            errors["CategoryId"] = ["La categoría indicada no existe."];

        return errors;
    }

    private static IResult InvalidProduct(Dictionary<string, string[]> errors) =>
        Results.ValidationProblem(errors, title: "Datos de producto inválidos.");

    // EF traduce la navegación a una consulta que obtiene el nombre actual de la categoría.
    private static IQueryable<ProductResponse> Responses(IQueryable<Product> products) =>
        products.AsNoTracking().Select(product => new ProductResponse(
            product.Id, product.Name, product.Description, product.CategoryId, product.Category.Name));
}
