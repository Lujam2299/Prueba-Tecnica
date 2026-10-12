using System.Net;
using System.Net.Http.Json;
using Catalogo.Api.Contracts;
using Catalogo.Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Catalogo.AcceptanceTests.Support;

// Comprobación técnica complementaria: omitir la API no permite omitir la unicidad.
public sealed class SqliteCategoryIntegrityTests
{
    [Test]
    public async Task MigrationPreservesExistingCategoryAndProtectsItsName()
    {
        var path = Path.Combine(Path.GetTempPath(), $"catalogo-upgrade-{Guid.NewGuid():N}.db");
        try
        {
            var options = new DbContextOptionsBuilder<CatalogoDbContext>()
                .UseSqlite($"Data Source={path};Pooling=False").Options;
            await using var db = new CatalogoDbContext(options);
            var migrator = db.Database.GetService<IMigrator>();
            await migrator.MigrateAsync("InitialCategories");
            await db.Database.ExecuteSqlRawAsync("INSERT INTO Categories (Name, Description) VALUES ('Electrónica', 'Anterior')");
            await migrator.MigrateAsync();
            var category = await db.Categories.AsNoTracking().SingleAsync();
            Assert.That(category.Name, Is.EqualTo("Electrónica"));
            Assert.That(category.Description, Is.EqualTo("Anterior"));
            var error = Assert.ThrowsAsync<SqliteException>(async () =>
            {
                await db.Database.ExecuteSqlRawAsync("INSERT INTO Categories (Name) VALUES ('ELECTRÓNICA')");
            });
            Assert.That(error!.SqliteExtendedErrorCode, Is.EqualTo(2067));
        }
        finally
        {
            foreach (var suffix in new[] { "", "-wal", "-shm" })
                File.Delete(path + suffix);
        }
    }

    [TestCase("Electrónica", "ELECTRÓNICA")]
    [TestCase("Electrónica", "ELECTRO\u0301NICA")]
    [TestCase("Niñez", "  NIÑEZ  ")]
    public async Task DatabaseRejectsEquivalentName(string original, string duplicate)
    {
        var api = new ApiScenario();
        try
        {
            api.Start();
            using var response = await api.Client.PostAsJsonAsync("/api/categories", new { Name = original });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            using (var scope = api.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<CatalogoDbContext>();
                await db.Database.OpenConnectionAsync();
                using var command = db.Database.GetDbConnection().CreateCommand();
                command.CommandText = "INSERT INTO Categories (Name) VALUES ($name)";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "$name";
                parameter.Value = duplicate;
                command.Parameters.Add(parameter);
                var error = Assert.ThrowsAsync<SqliteException>(async () => { await command.ExecuteNonQueryAsync(); });
                Assert.That(error!.SqliteExtendedErrorCode, Is.EqualTo(2067));
            }
            using var list = await api.Client.GetAsync("/api/categories");
            var categories = (await list.Content.ReadFromJsonAsync<CategoryResponse[]>())!;
            Assert.That(categories, Has.Length.EqualTo(1));
            Assert.That(categories[0].Name, Is.EqualTo(original));
        }
        finally
        {
            api.Dispose();
        }
    }
}
