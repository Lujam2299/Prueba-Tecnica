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
public sealed class EditarCategoriasSteps(ApiScenario api)
{
    private readonly List<CategoryResponse> previous = [];
    private CategoryResponse edited = null!;

    [Given("existe una categoría para editar llamada {string} con descripción {string}")]
    public async Task CreateTarget(string name, string description) =>
        previous.Add(await Create(name, description));

    [Given("que también existe la categoría {string}")]
    public async Task CreateOther(string name) => previous.Add(await Create(name, "Otra"));

    private async Task<CategoryResponse> Create(string name, string description)
    {
        using var response = await api.Client.PostAsJsonAsync("/api/categories", new { Name = name, Description = description });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        return (await response.Content.ReadFromJsonAsync<CategoryResponse>())!;
    }

    [When("edito la categoría con el cuerpo '(.*)'")]
    public Task Edit(string body) => Put(previous[0].Id, body);

    [When("edito una categoría inexistente con el cuerpo '(.*)'")]
    public Task EditMissing(string body) => Put(int.MaxValue, body);

    private async Task Put(int id, string body)
    {
        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        api.Response = await api.Client.PutAsync($"/api/categories/{id}", content);
    }

    [Then("la categoría se actualiza a {string} con descripción {string}")]
    public async Task CheckUpdated(string name, string description)
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        edited = new CategoryResponse(previous[0].Id, name, description.Length == 0 ? null : description);
        Assert.That(await api.Response.Content.ReadFromJsonAsync<CategoryResponse>(), Is.EqualTo(edited));
    }

    [Then("la edición queda disponible después de reiniciar")]
    public async Task CheckPersistentUpdate()
    {
        api.Restart();
        await CheckStored(new[] { edited });
    }

    [Then("las categorías conservan sus datos anteriores")]
    public async Task CheckUnchanged()
    {
        api.Restart();
        await CheckStored(previous);
    }

    private async Task CheckStored(IEnumerable<CategoryResponse> expected)
    {
        using var list = await api.Client.GetAsync("/api/categories");
        Assert.That(list.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await list.Content.ReadFromJsonAsync<CategoryResponse[]>(), Is.EqualTo(expected));
        foreach (var category in expected)
        {
            using var detail = await api.Client.GetAsync($"/api/categories/{category.Id}");
            Assert.That(detail.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await detail.Content.ReadFromJsonAsync<CategoryResponse>(), Is.EqualTo(category));
        }
    }

    [When("consulto el detalle de una categoría inexistente")]
    public async Task GetMissing() => api.Response = await api.Client.GetAsync($"/api/categories/{int.MaxValue}");

    [Then("recibo un error de categoría inexistente")]
    public async Task CheckNotFound()
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(api.Response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/problem+json"));
        using var json = JsonDocument.Parse(await api.Response.Content.ReadAsStringAsync());
        Assert.That(json.RootElement.GetProperty("status").GetInt32(), Is.EqualTo(404));
        Assert.That(json.RootElement.GetProperty("detail").GetString(),
            Is.EqualTo("No existe una categoría con el identificador indicado."));
    }
}
