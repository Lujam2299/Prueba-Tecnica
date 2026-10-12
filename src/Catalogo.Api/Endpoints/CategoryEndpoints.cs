using Catalogo.Api.Contracts;
using Catalogo.Api.Data;
using Catalogo.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace Catalogo.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        app.MapGet("/api/categories", async (CatalogoDbContext db) =>
            Results.Ok(await db.Categories.AsNoTracking().OrderBy(category => category.Id)
                .Select(category => new CategoryResponse(category.Id, category.Name, category.Description))
                .ToListAsync()));

        app.MapGet("/api/categories/{id:int}", async (int id, CatalogoDbContext db) =>
        {
            var category = await db.Categories.AsNoTracking().SingleOrDefaultAsync(category => category.Id == id);
            return category is null
                ? CategoryNotFound()
                : Results.Ok(new CategoryResponse(category.Id, category.Name, category.Description));
        });

        app.MapPost("/api/categories", async (CategoryRequest request, CatalogoDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return NameRequired();
            }

            var name = request.Name.Trim();
            if (await db.Categories.AnyAsync(category => category.Name == name))
                return DuplicateName();

            var category = new Category { Name = name, Description = request.Description };
            db.Categories.Add(category);
            if (!await SaveWithoutNameConflict(db))
                return DuplicateName();

            var response = new CategoryResponse(category.Id, category.Name, category.Description);
            return Results.Created($"/api/categories/{category.Id}", response);
        });

        app.MapPut("/api/categories/{id:int}", async (int id, CategoryRequest request, CatalogoDbContext db) =>
        {
            var category = await db.Categories.FindAsync(id);
            if (category is null)
                return CategoryNotFound();

            if (string.IsNullOrWhiteSpace(request.Name))
                return NameRequired();

            var name = request.Name.Trim();
            if (await db.Categories.AnyAsync(other => other.Id != id && other.Name == name))
                return DuplicateName();

            category.Name = name;
            category.Description = request.Description;
            if (!await SaveWithoutNameConflict(db))
                return DuplicateName();

            return Results.Ok(new CategoryResponse(category.Id, category.Name, category.Description));
        });
    }

    private static IResult NameRequired() => Results.ValidationProblem(
        new Dictionary<string, string[]> { ["Name"] = ["El nombre es obligatorio."] },
        title: "Datos de categoría inválidos.");

    private static IResult CategoryNotFound() => Results.Problem(
        statusCode: StatusCodes.Status404NotFound,
        title: "Categoría no encontrada.",
        detail: "No existe una categoría con el identificador indicado.");

    private static async Task<bool> SaveWithoutNameConflict(CatalogoDbContext db)
    {
        try
        {
            await db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException exception) when
            (exception.InnerException is SqliteException { SqliteExtendedErrorCode: 2067 })
        {
            // El índice protege también ante dos peticiones que comprobaron el nombre a la vez.
            return false;
        }
    }

    private static IResult DuplicateName() => Results.Problem(
        statusCode: StatusCodes.Status409Conflict,
        title: "Nombre de categoría duplicado.",
        detail: "Ya existe una categoría con ese nombre.");
}
