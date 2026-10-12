using System.Net;
using System.Net.Http.Json;
using Catalogo.AcceptanceTests.Support;
using Catalogo.Api.Contracts;
using NUnit.Framework;
using Reqnroll;

namespace Catalogo.AcceptanceTests.Steps;

[Binding]
public sealed class EliminarProductosSteps(ApiScenario api)
{
    private CategoryResponse category = null!;
    private ProductResponse first = null!;
    private ProductResponse second = null!;

    [Given("hay dos productos para comprobar su eliminación")]
    public async Task Prepare()
    {
        using var response = await api.Client.PostAsJsonAsync("/api/categories", new { Name = "Electrónica" });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        category = (await response.Content.ReadFromJsonAsync<CategoryResponse>())!;
        first = await Create("Teclado");
        second = await Create("Ratón");
    }

    private async Task<ProductResponse> Create(string name)
    {
        using var response = await api.Client.PostAsJsonAsync("/api/products",
            new { Name = name, CategoryId = category.Id });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        return (await response.Content.ReadFromJsonAsync<ProductResponse>())!;
    }

    [When("elimino el primer producto")]
    public Task DeleteFirst() => Delete(first.Id);

    [When("elimino un producto que no existe")]
    public Task DeleteMissing() => Delete(int.MaxValue);

    private async Task Delete(int id)
    {
        api.Response?.Dispose();
        api.Response = await api.Client.DeleteAsync($"/api/products/{id}");
    }

    [Then("la eliminación del producto responde sin contenido")]
    public async Task CheckDeleted()
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(await api.Response.Content.ReadAsStringAsync(), Is.Empty);
    }

    [Then("el producto eliminado ya no está disponible y los demás datos se conservan")]
    public async Task CheckRemaining()
    {
        await CheckStored([second]);
        using var detail = await api.Client.GetAsync($"/api/products/{first.Id}");
        Assert.That(detail.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Then("la eliminación rechazada conserva ambos productos y su categoría")]
    public Task CheckUnchanged() => CheckStored([first, second]);

    private async Task CheckStored(ProductResponse[] expected)
    {
        api.Restart();
        using var list = await api.Client.GetAsync("/api/products");
        Assert.That(list.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await list.Content.ReadFromJsonAsync<ProductResponse[]>(), Is.EqualTo(expected));
        foreach (var product in expected)
        {
            using var detail = await api.Client.GetAsync($"/api/products/{product.Id}");
            Assert.That(detail.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await detail.Content.ReadFromJsonAsync<ProductResponse>(), Is.EqualTo(product));
        }
        using var categories = await api.Client.GetAsync("/api/categories");
        Assert.That(categories.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await categories.Content.ReadFromJsonAsync<CategoryResponse[]>(), Is.EqualTo(new[] { category }));
    }
}
