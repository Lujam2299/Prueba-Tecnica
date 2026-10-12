using Catalogo.Api.Contracts;
using Catalogo.Api.Data;
using Catalogo.Api.Models;
using Microsoft.EntityFrameworkCore;

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
            var category = new Category { Name = request.Name, Description = request.Description };
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var response = new CategoryResponse(category.Id, category.Name, category.Description);
            return Results.Created($"/api/categories/{category.Id}", response);
        });
    }
}
