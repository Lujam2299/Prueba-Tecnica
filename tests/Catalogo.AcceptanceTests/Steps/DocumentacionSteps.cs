using System.Net;
using System.Text.Json;
using Catalogo.AcceptanceTests.Support;
using NUnit.Framework;
using Reqnroll;

namespace Catalogo.AcceptanceTests.Steps;

[Binding]
public sealed class DocumentacionSteps(ApiScenario api)
{
    [When("consulto la documentación de la API")]
    public async Task GetDocument() => api.Response = await api.Client.GetAsync("/openapi/v1.json");

    [Then("el contrato describe las diez operaciones y sus respuestas")]
    public async Task CheckContract()
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        using var document = JsonDocument.Parse(await api.Response.Content.ReadAsStringAsync());
        var paths = document.RootElement.GetProperty("paths");
        foreach (var entity in new[] { "categories", "products" })
        {
            var list = paths.GetProperty($"/api/{entity}");
            var detail = paths.GetProperty($"/api/{entity}/{{id}}");
            Assert.That(list.GetProperty("get").GetProperty("responses").TryGetProperty("200", out _), Is.True);
            var created = list.GetProperty("post");
            Assert.That(created.GetProperty("requestBody").GetProperty("content").TryGetProperty("application/json", out _), Is.True);
            Assert.That(created.GetProperty("responses").TryGetProperty("201", out _), Is.True);
            Assert.That(created.GetProperty("responses").TryGetProperty("400", out _), Is.True);
            foreach (var method in new[] { "get", "put", "delete" })
            {
                var responses = detail.GetProperty(method).GetProperty("responses");
                Assert.That(responses.TryGetProperty(method == "delete" ? "204" : "200", out _), Is.True);
                Assert.That(responses.TryGetProperty("404", out _), Is.True);
            }
        }
        Assert.That(paths.GetProperty("/api/categories/{id}").GetProperty("delete")
            .GetProperty("responses").TryGetProperty("409", out _), Is.True);
        var schemas = document.RootElement.GetProperty("components").GetProperty("schemas");
        Assert.That(schemas.GetProperty("ProductResponse").GetProperty("properties").TryGetProperty("categoryName", out _), Is.True);
    }

    [Then("la interfaz de pruebas está disponible en el navegador")]
    public async Task CheckUi()
    {
        using var response = await api.Client.GetAsync("/swagger/index.html");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Content.Headers.ContentType!.MediaType, Is.EqualTo("text/html"));
        Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain("swagger-ui"));
        using var script = await api.Client.GetAsync("/swagger/swagger-ui-bundle.js");
        Assert.That(script.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
