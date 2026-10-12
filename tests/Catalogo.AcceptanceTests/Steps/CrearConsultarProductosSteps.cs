using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Catalogo.AcceptanceTests.Support;
using Catalogo.Api.Contracts;
using NUnit.Framework;
using Reqnroll;

namespace Catalogo.AcceptanceTests.Steps;

[Binding]
public sealed class CrearConsultarProductosSteps(ApiScenario api)
{
    private CategoryResponse category = null!;
    private ProductResponse created = null!;
    private string name = "";
    private string? description;
    private Uri location = null!;

    [Given("existe una categoría para productos llamada {string}")]
    public async Task CreateCategory(string categoryName)
    {
        // Otra categoría primero evita suponer que el identificador de la elegida es 1.
        using var other = await api.Client.PostAsJsonAsync("/api/categories", new { Name = "Otra categoría" });
        Assert.That(other.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        using var response = await api.Client.PostAsJsonAsync("/api/categories", new { Name = categoryName });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        category = (await response.Content.ReadFromJsonAsync<CategoryResponse>())!;
    }

    [When("creo el producto {string} con descripción {string}")]
    public async Task CreateProduct(string productName, string productDescription)
    {
        name = productName;
        description = productDescription.Length == 0 ? null : productDescription;
        api.Response = await api.Client.PostAsJsonAsync("/api/products",
            new { Name = name, Description = description, CategoryId = category.Id });
    }

    [Then("el producto se crea con su categoría")]
    public async Task CheckCreated()
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        created = (await api.Response.Content.ReadFromJsonAsync<ProductResponse>())!;
        Assert.That(created.Id, Is.GreaterThan(0));
        Assert.That(created.Name, Is.EqualTo(name));
        Assert.That(created.Description, Is.EqualTo(description));
        Assert.That(created.CategoryId, Is.EqualTo(category.Id));
        Assert.That(created.CategoryName, Is.EqualTo(category.Name));
        location = api.Response.Headers.Location!;
        Assert.That(location, Is.Not.Null);
        Assert.That(location.ToString(), Is.EqualTo($"/api/products/{created.Id}"));
    }

    [Then("puedo consultar el producto creado")]
    public async Task CheckDetail()
    {
        using var response = await api.Client.GetAsync(location);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await response.Content.ReadFromJsonAsync<ProductResponse>(), Is.EqualTo(created));
    }

    [When("consulto la lista de productos")]
    public async Task GetList() => api.Response = await api.Client.GetAsync("/api/products");

    [Then("la lista contiene el producto con su categoría")]
    public async Task CheckList()
    {
        using var response = await api.Client.GetAsync("/api/products");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await response.Content.ReadFromJsonAsync<ProductResponse[]>(), Is.EqualTo(new[] { created }));
    }

    [When("creo otro producto con el mismo nombre")]
    public async Task CreateDuplicateName() =>
        api.Response = await api.Client.PostAsJsonAsync("/api/products",
            new { Name = created.Name, CategoryId = category.Id });

    [Then("ambos productos quedan disponibles")]
    public async Task CheckBoth()
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var second = (await api.Response.Content.ReadFromJsonAsync<ProductResponse>())!;
        Assert.That(second.Id, Is.Not.EqualTo(created.Id));
        Assert.That(second, Is.EqualTo(created with { Id = second.Id, Description = null }));
        using var response = await api.Client.GetAsync("/api/products");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await response.Content.ReadFromJsonAsync<ProductResponse[]>(), Is.EqualTo(new[] { created, second }));
    }

    [When("cambio el nombre de la categoría del producto a {string}")]
    public async Task RenameCategory(string categoryName)
    {
        using var response = await api.Client.PutAsJsonAsync($"/api/categories/{category.Id}", new { Name = categoryName });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        created = created with { CategoryName = categoryName };
    }

    [When("consulto un producto inexistente")]
    public async Task GetMissing() => api.Response = await api.Client.GetAsync($"/api/products/{int.MaxValue}");

    [Then("recibo un error de producto inexistente")]
    public async Task CheckMissing()
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(api.Response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/problem+json"));
        using var json = JsonDocument.Parse(await api.Response.Content.ReadAsStringAsync());
        Assert.That(json.RootElement.GetProperty("status").GetInt32(), Is.EqualTo(404));
        Assert.That(json.RootElement.GetProperty("detail").GetString(),
            Is.EqualTo("No existe un producto con el identificador indicado."));
    }
}
