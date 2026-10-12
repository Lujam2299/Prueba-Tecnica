using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Catalogo.AcceptanceTests.Support;
using Catalogo.Api.Contracts;
using NUnit.Framework;
using Reqnroll;

namespace Catalogo.AcceptanceTests.Steps;

[Binding]
public sealed class ValidarProductosSteps(ApiScenario api)
{
    private CategoryResponse category = null!;

    [Given("existe una categoría válida para validar productos")]
    public async Task CreateCategory()
    {
        using var response = await api.Client.PostAsJsonAsync("/api/categories",
            new { Name = "Electrónica", Description = "Sin modificar" });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        category = (await response.Content.ReadFromJsonAsync<CategoryResponse>())!;
    }

    [When("intento crear un producto con el cuerpo '(.*)'")]
    public async Task CreateProduct(string body)
    {
        body = body.Replace("$categoryId", category.Id.ToString(CultureInfo.InvariantCulture));
        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        api.Response = await api.Client.PostAsync("/api/products", content);
    }

    [Then("recibo un error de producto en {string} con mensaje {string}")]
    public async Task CheckError(string field, string message)
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(api.Response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/problem+json"));
        using var json = JsonDocument.Parse(await api.Response.Content.ReadAsStringAsync());
        Assert.That(json.RootElement.GetProperty("status").GetInt32(), Is.EqualTo(400));
        Assert.That(json.RootElement.GetProperty("errors").GetProperty(field)[0].GetString(), Is.EqualTo(message));
    }

    [Then("no se guarda ningún producto ni se alteran las categorías")]
    public async Task CheckUnchanged()
    {
        api.Restart();
        using var products = await api.Client.GetAsync("/api/products");
        Assert.That(products.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await products.Content.ReadFromJsonAsync<ProductResponse[]>(), Is.Empty);
        await CheckCategoryUnchanged();
    }

    private async Task CheckCategoryUnchanged()
    {
        using var categories = await api.Client.GetAsync("/api/categories");
        Assert.That(categories.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await categories.Content.ReadFromJsonAsync<CategoryResponse[]>(), Is.EqualTo(new[] { category }));
        using var detail = await api.Client.GetAsync($"/api/categories/{category.Id}");
        Assert.That(detail.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await detail.Content.ReadFromJsonAsync<CategoryResponse>(), Is.EqualTo(category));
    }

    [Then("el producto queda guardado con el nombre {string}")]
    public async Task CheckTrimmedName(string name)
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var created = (await api.Response.Content.ReadFromJsonAsync<ProductResponse>())!;
        Assert.That(created.Id, Is.GreaterThan(0));
        Assert.That(created, Is.EqualTo(new ProductResponse(created.Id, name, "Original", category.Id, category.Name)));
        var location = api.Response.Headers.Location;
        Assert.That(location, Is.Not.Null);
        api.Restart();
        using var detail = await api.Client.GetAsync(location);
        Assert.That(detail.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await detail.Content.ReadFromJsonAsync<ProductResponse>(), Is.EqualTo(created));
        using var list = await api.Client.GetAsync("/api/products");
        Assert.That(list.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await list.Content.ReadFromJsonAsync<ProductResponse[]>(), Is.EqualTo(new[] { created }));
        await CheckCategoryUnchanged();
    }
}
