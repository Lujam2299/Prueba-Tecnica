using System.Net;
using System.Net.Http.Json;
using Catalogo.Api.Contracts;
using Catalogo.Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Catalogo.AcceptanceTests.Support;

public sealed class SqliteProductIntegrityTests
{
    [Test]
    public async Task DatabaseRejectsProductWithMissingCategory()
    {
        var api = new ApiScenario();
        try
        {
            api.Start();
            using (var scope = api.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<CatalogoDbContext>();
                var error = Assert.ThrowsAsync<SqliteException>(async () =>
                {
                    await db.Database.ExecuteSqlRawAsync(
                        "INSERT INTO Products (Name, CategoryId) VALUES ('Sin categoría', 2147483647)");
                });
                Assert.That(error!.SqliteExtendedErrorCode, Is.EqualTo(787));
            }
            using var products = await api.Client.GetAsync("/api/products");
            Assert.That(products.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await products.Content.ReadFromJsonAsync<ProductResponse[]>(), Is.Empty);
        }
        finally
        {
            api.Dispose();
        }
    }
}
