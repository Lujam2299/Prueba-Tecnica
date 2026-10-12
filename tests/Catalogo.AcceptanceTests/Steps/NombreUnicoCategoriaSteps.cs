using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Catalogo.AcceptanceTests.Support;
using Catalogo.Api.Contracts;
using NUnit.Framework;
using Reqnroll;

namespace Catalogo.AcceptanceTests.Steps;

[Binding]
public sealed class NombreUnicoCategoriaSteps(ApiScenario api)
{
    private CategoryResponse original = null!;
    private string newName = "";

    [Given("existe una categoría original llamada {string}")]
    public async Task CreateOriginal(string name)
    {
        using var response = await api.Client.PostAsJsonAsync("/api/categories", new { Name = name, Description = "Original" });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        original = (await response.Content.ReadFromJsonAsync<CategoryResponse>())!;
    }

    [When("intento crear otra categoría llamada {string}")]
    public async Task CreateAnother(string name)
    {
        newName = name;
        api.Response = await api.Client.PostAsJsonAsync("/api/categories", new { Name = name });
    }

    [Then("recibo un conflicto por nombre de categoría duplicado")]
    public async Task CheckConflict()
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        Assert.That(api.Response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/problem+json"));
        using var json = JsonDocument.Parse(await api.Response.Content.ReadAsStringAsync());
        Assert.That(json.RootElement.GetProperty("status").GetInt32(), Is.EqualTo(409));
        Assert.That(json.RootElement.GetProperty("detail").GetString(),
            Is.EqualTo("Ya existe una categoría con ese nombre."));
    }

    [Then("la única categoría sigue siendo la original")]
    public async Task CheckOriginalUnchanged()
    {
        // Comprobar también que el rechazo no alteró los datos persistidos.
        api.Restart();
        using var list = await api.Client.GetAsync("/api/categories");
        Assert.That(list.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await list.Content.ReadFromJsonAsync<CategoryResponse[]>(), Is.EqualTo(new[] { original }));
        using var detail = await api.Client.GetAsync($"/api/categories/{original.Id}");
        Assert.That(detail.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await detail.Content.ReadFromJsonAsync<CategoryResponse>(), Is.EqualTo(original));
    }

    [Then("ambas categorías quedan disponibles")]
    public async Task CheckBoth()
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var created = (await api.Response.Content.ReadFromJsonAsync<CategoryResponse>())!;
        Assert.That(created.Name, Is.EqualTo(newName));
        Assert.That(created.Id, Is.Not.EqualTo(original.Id));
        using var list = await api.Client.GetAsync("/api/categories");
        Assert.That(list.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await list.Content.ReadFromJsonAsync<CategoryResponse[]>(), Is.EqualTo(new[] { original, created }));
    }
}
