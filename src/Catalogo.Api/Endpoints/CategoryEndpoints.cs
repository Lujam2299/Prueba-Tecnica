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
                ? Results.NotFound()
                : Results.Ok(new CategoryResponse(category.Id, category.Name, category.Description));
        });

        app.MapPost("/api/categories", async (CreateCategoryRequest request, CatalogoDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["Name"] = ["El nombre es obligatorio."]
                }, title: "Datos de categoría inválidos.");
            }

            var name = request.Name.Trim();
            if (await db.Categories.AnyAsync(category => category.Name == name))
                return DuplicateName();

            var category = new Category { Name = name, Description = request.Description };
            db.Categories.Add(category);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException exception) when
                (exception.InnerException is SqliteException { SqliteExtendedErrorCode: 2067 })
            {
                // El índice resuelve también dos peticiones que comprobaron el nombre a la vez.
                return DuplicateName();
            }

            var response = new CategoryResponse(category.Id, category.Name, category.Description);
            return Results.Created($"/api/categories/{category.Id}", response);
        });
    }

    private static IResult DuplicateName() => Results.Problem(
        statusCode: StatusCodes.Status409Conflict,
        title: "Nombre de categoría duplicado.",
        detail: "Ya existe una categoría con ese nombre.");
}
