using System.Net;
using System.Net.Http.Json;
using Catalogo.Api.Contracts;
using Catalogo.AcceptanceTests.Support;
using NUnit.Framework;
using Reqnroll;

namespace Catalogo.AcceptanceTests.Steps;

[Binding]
public sealed class CrearCategoriasSteps(ApiScenario api)
{
    private string name = "";
    private string? description;
    private CategoryResponse created = null!;
    private Uri location = null!;

    [When("creo la categoría {string} con descripción {string}")]
    public async Task Create(string categoryName, string categoryDescription)
    {
        name = categoryName;
        description = categoryDescription.Length == 0 ? null : categoryDescription;
        api.Response = await api.Client.PostAsJsonAsync("/api/categories", new { Name = name, Description = description });
    }

    [Then("la categoría se crea correctamente")]
    public async Task CheckCreated()
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        created = (await api.Response.Content.ReadFromJsonAsync<CategoryResponse>())!;
        Assert.That(created, Is.Not.Null);
        Assert.That(created.Id, Is.GreaterThan(0));
        Assert.That(created.Name, Is.EqualTo(name));
        Assert.That(created.Description, Is.EqualTo(description));
        location = api.Response.Headers.Location!;
        Assert.That(location, Is.Not.Null);
        Assert.That(location.ToString(), Is.EqualTo($"/api/categories/{created.Id}"));
    }

    [Then("puedo consultar la categoría creada")]
    public async Task CheckDetail()
    {
        using var response = await api.Client.GetAsync(location);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await response.Content.ReadFromJsonAsync<CategoryResponse>(), Is.EqualTo(created));
    }

    [Then("la lista contiene la categoría creada")]
    public async Task CheckList()
    {
        using var response = await api.Client.GetAsync("/api/categories");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await response.Content.ReadFromJsonAsync<CategoryResponse[]>(), Is.EqualTo(new[] { created }));
    }

    [When("reinicio la aplicación")]
    public void Restart() => api.Restart();
}
