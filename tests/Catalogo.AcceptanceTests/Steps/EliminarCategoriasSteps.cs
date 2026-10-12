using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Catalogo.AcceptanceTests.Support;
using Catalogo.Api.Contracts;
using NUnit.Framework;
using Reqnroll;

namespace Catalogo.AcceptanceTests.Steps;

[Binding]
public sealed class EliminarCategoriasSteps(ApiScenario api)
{
    private CategoryResponse category = null!;
    private CategoryResponse other = null!;
    private readonly List<ProductResponse> products = [];

    [Given("existen dos categorías para comprobar su eliminación")]
    public async Task Prepare()
    {
        category = await CreateCategory("Electrónica");
        other = await CreateCategory("Libros");
    }

    private async Task<CategoryResponse> CreateCategory(string name)
    {
        using var response = await api.Client.PostAsJsonAsync("/api/categories", new { Name = name });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        return (await response.Content.ReadFromJsonAsync<CategoryResponse>())!;
    }

    [Given("que la categoría de prueba tiene dos productos")]
    public async Task AddProducts()
    {
        foreach (var name in new[] { "Teclado", "Ratón" })
        {
            using var response = await api.Client.PostAsJsonAsync("/api/products",
                new { Name = name, CategoryId = category.Id });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            products.Add((await response.Content.ReadFromJsonAsync<ProductResponse>())!);
        }
    }

    [When("elimino la categoría de prueba")]
    public Task DeleteCategory() => Delete(category.Id);

    [When("elimino una categoría que no existe")]
    public Task DeleteMissing() => Delete(int.MaxValue);

    private async Task Delete(int id)
    {
        api.Response?.Dispose();
        api.Response = await api.Client.DeleteAsync($"/api/categories/{id}");
    }

    [When("retiro un producto de la categoría de prueba")]
    public async Task RemoveOne()
    {
        using var response = await api.Client.DeleteAsync($"/api/products/{products[0].Id}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        products.RemoveAt(0);
    }

    [When("retiro los productos restantes de la categoría de prueba")]
    public async Task RemoveRemaining()
    {
        while (products.Count > 0)
            await RemoveOne();
    }

    [Then("la eliminación de categoría responde sin contenido")]
    public async Task CheckDeleted()
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(await api.Response.Content.ReadAsStringAsync(), Is.Empty);
    }

    [Then("la eliminación devuelve categoría inexistente")]
    public Task CheckMissing() => CheckProblem(HttpStatusCode.NotFound,
        "No existe una categoría con el identificador indicado.");

    [Then("recibo el conflicto exacto de categoría con productos")]
    public Task CheckConflict() => CheckProblem(HttpStatusCode.Conflict,
        "Esta categoria no se puede eliminar porque tiene productos asignados");

    private async Task CheckProblem(HttpStatusCode status, string detail)
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(status));
        Assert.That(api.Response.Content.Headers.ContentType!.MediaType, Is.EqualTo("application/problem+json"));
        using var json = JsonDocument.Parse(await api.Response.Content.ReadAsStringAsync());
        Assert.That(json.RootElement.GetProperty("status").GetInt32(), Is.EqualTo((int)status));
        Assert.That(json.RootElement.GetProperty("detail").GetString(), Is.EqualTo(detail));
    }

    [Then("las categorías y productos de prueba conservan sus datos")]
    public Task CheckUnchanged() => CheckStored([category, other]);

    [Then("la categoría eliminada permanece ausente al reiniciar")]
    public async Task CheckAbsent()
    {
        await CheckStored([other]);
        using var detail = await api.Client.GetAsync($"/api/categories/{category.Id}");
        Assert.That(detail.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    private async Task CheckStored(CategoryResponse[] expected)
    {
        api.Restart();
        using var categories = await api.Client.GetAsync("/api/categories");
        Assert.That(categories.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await categories.Content.ReadFromJsonAsync<CategoryResponse[]>(), Is.EqualTo(expected));
        foreach (var item in expected)
        {
            using var detail = await api.Client.GetAsync($"/api/categories/{item.Id}");
            Assert.That(detail.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await detail.Content.ReadFromJsonAsync<CategoryResponse>(), Is.EqualTo(item));
        }
        using var list = await api.Client.GetAsync("/api/products");
        Assert.That(list.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await list.Content.ReadFromJsonAsync<ProductResponse[]>(), Is.EqualTo(products));
        foreach (var item in products)
        {
            using var detail = await api.Client.GetAsync($"/api/products/{item.Id}");
            Assert.That(detail.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await detail.Content.ReadFromJsonAsync<ProductResponse>(), Is.EqualTo(item));
        }
    }
}
