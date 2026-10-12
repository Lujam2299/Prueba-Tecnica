using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Catalogo.AcceptanceTests.Support;
using Catalogo.Api.Contracts;
using NUnit.Framework;
using Reqnroll;

namespace Catalogo.AcceptanceTests.Steps;

[Binding]
public sealed class EditarProductosSteps(ApiScenario api)
{
    private CategoryResponse originalCategory = null!;
    private CategoryResponse newCategory = null!;
    private ProductResponse originalProduct = null!;
    private ProductResponse otherProduct = null!;
    private ProductResponse updated = null!;

    [Given("existen productos y categorías para comprobar la edición")]
    public async Task PrepareData()
    {
        originalCategory = await CreateCategory("Electrónica");
        newCategory = await CreateCategory("Libros");
        originalProduct = await CreateProduct("Teclado", "Original");
        otherProduct = await CreateProduct("Otro", "Sin modificar");
    }

    private async Task<CategoryResponse> CreateCategory(string name)
    {
        using var response = await api.Client.PostAsJsonAsync("/api/categories", new { Name = name });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        return (await response.Content.ReadFromJsonAsync<CategoryResponse>())!;
    }

    private async Task<ProductResponse> CreateProduct(string name, string description)
    {
        using var response = await api.Client.PostAsJsonAsync("/api/products",
            new { Name = name, Description = description, CategoryId = originalCategory.Id });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        return (await response.Content.ReadFromJsonAsync<ProductResponse>())!;
    }

    [When("edito el producto con el cuerpo '(.*)'")]
    public Task EditProduct(string body) => Put(originalProduct.Id, body);

    [When("edito un producto inexistente con el cuerpo '(.*)'")]
    public Task EditMissing(string body) => Put(int.MaxValue, body);

    private async Task Put(int id, string body)
    {
        body = body.Replace("$originalCategoryId", originalCategory.Id.ToString(CultureInfo.InvariantCulture))
            .Replace("$newCategoryId", newCategory.Id.ToString(CultureInfo.InvariantCulture));
        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        api.Response = await api.Client.PutAsync($"/api/products/{id}", content);
    }

    [Then("el producto se actualiza a {string} con descripción {string} en la categoría {string}")]
    public async Task CheckUpdated(string name, string description, string categoryName)
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var category = categoryName == originalCategory.Name ? originalCategory : newCategory;
        updated = new ProductResponse(originalProduct.Id, name, description.Length == 0 ? null : description,
            category.Id, category.Name);
        Assert.That(await api.Response.Content.ReadFromJsonAsync<ProductResponse>(), Is.EqualTo(updated));
    }

    [Then("el producto editado y los demás datos quedan disponibles al reiniciar")]
    public async Task CheckPersistentUpdate()
    {
        api.Restart();
        await CheckStored(new[] { updated, otherProduct });
    }

    [Then("los productos y categorías conservan sus datos anteriores")]
    public async Task CheckUnchanged()
    {
        api.Restart();
        await CheckStored(new[] { originalProduct, otherProduct });
    }

    private async Task CheckStored(ProductResponse[] expected)
    {
        using var products = await api.Client.GetAsync("/api/products");
        Assert.That(products.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await products.Content.ReadFromJsonAsync<ProductResponse[]>(), Is.EqualTo(expected));
        foreach (var product in expected)
        {
            using var detail = await api.Client.GetAsync($"/api/products/{product.Id}");
            Assert.That(detail.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await detail.Content.ReadFromJsonAsync<ProductResponse>(), Is.EqualTo(product));
        }
        using var categories = await api.Client.GetAsync("/api/categories");
        Assert.That(categories.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await categories.Content.ReadFromJsonAsync<CategoryResponse[]>(),
            Is.EqualTo(new[] { originalCategory, newCategory }));
    }
}
