using System.Net;
using System.Text.Json;
using Catalogo.AcceptanceTests.Support;
using NUnit.Framework;
using Reqnroll;

namespace Catalogo.AcceptanceTests.Steps;

[Binding]
public sealed class ConsultarCategoriasSteps(ApiScenario api)
{
    [Given("que no existen categorías")]
    public void GivenNoCategories()
    {
        // Cada escenario inicia la aplicación con su propia base nueva.
        api.Start();
    }

    [When("consulto la lista de categorías")]
    public async Task WhenListingCategories()
    {
        api.Response = await api.Client.GetAsync("/api/categories");
    }

    [Then("la consulta es exitosa")]
    public void ThenSuccessful()
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Then("recibo una lista vacía")]
    public async Task ThenEmptyList()
    {
        Assert.That(api.Response!.Content.Headers.ContentType?.MediaType,
            Is.EqualTo("application/json"));
        using var json = JsonDocument.Parse(await api.Response.Content.ReadAsStringAsync());
        Assert.That(json.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Array));
        Assert.That(json.RootElement.GetArrayLength(), Is.Zero);
    }
}
