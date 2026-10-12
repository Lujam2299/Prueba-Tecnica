using System.Net;
using System.Net.Http.Json;
using Catalogo.Api.Contracts;
using NUnit.Framework;

namespace Catalogo.AcceptanceTests.Support;

public sealed class ScenarioIsolationTests
{
    [Test]
    public async Task IndependentApplicationsDoNotShareData()
    {
        var first = new ApiScenario();
        var second = new ApiScenario();
        try
        {
            first.Start();
            second.Start();
            var firstCategory = await CreateCategory(first);
            using (var emptyList = await second.Client.GetAsync("/api/categories"))
            {
                Assert.That(emptyList.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(await emptyList.Content.ReadFromJsonAsync<CategoryResponse[]>(), Is.Empty);
            }
            // Un mismo nombre es válido porque las bases pertenecen a escenarios distintos.
            var secondCategory = await CreateCategory(second);
            using (var deleted = await first.Client.DeleteAsync($"/api/categories/{firstCategory.Id}"))
                Assert.That(deleted.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            first.Restart();
            second.Restart();
            using var firstList = await first.Client.GetAsync("/api/categories");
            Assert.That(firstList.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await firstList.Content.ReadFromJsonAsync<CategoryResponse[]>(), Is.Empty);
            using var secondList = await second.Client.GetAsync("/api/categories");
            Assert.That(secondList.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await secondList.Content.ReadFromJsonAsync<CategoryResponse[]>(),
                Is.EqualTo(new[] { secondCategory }));
        }
        finally
        {
            first.Dispose();
            second.Dispose();
        }
    }

    private static async Task<CategoryResponse> CreateCategory(ApiScenario api)
    {
        using var response = await api.Client.PostAsJsonAsync("/api/categories", new { Name = "Electrónica" });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        return (await response.Content.ReadFromJsonAsync<CategoryResponse>())!;
    }
}
