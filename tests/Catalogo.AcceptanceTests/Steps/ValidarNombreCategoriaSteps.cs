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
public sealed class ValidarNombreCategoriaSteps(ApiScenario api)
{
    [When("intento crear una categoría con el cuerpo '(.*)'")]
    public async Task CreateWithBody(string body)
    {
        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        api.Response = await api.Client.PostAsync("/api/categories", content);
    }

    [Then("recibo un error de validación del nombre")]
    public async Task CheckValidationError()
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(api.Response.Content.Headers.ContentType?.MediaType,
            Is.EqualTo("application/problem+json"));
        using var json = JsonDocument.Parse(await api.Response.Content.ReadAsStringAsync());
        Assert.That(json.RootElement.GetProperty("status").GetInt32(), Is.EqualTo(400));
        Assert.That(json.RootElement.GetProperty("errors").GetProperty("Name")[0].GetString(),
            Is.EqualTo("El nombre es obligatorio."));
    }

    [Then("no se guarda ninguna categoría")]
    public async Task CheckNoCategorySaved()
    {
        using var response = await api.Client.GetAsync("/api/categories");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await response.Content.ReadFromJsonAsync<CategoryResponse[]>(), Is.Empty);
    }

    [Then("la categoría queda guardada con el nombre {string}")]
    public async Task CheckStoredName(string expectedName)
    {
        Assert.That(api.Response!.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var created = await api.Response.Content.ReadFromJsonAsync<CategoryResponse>();
        Assert.That(created!.Name, Is.EqualTo(expectedName));
        Assert.That(created.Id, Is.GreaterThan(0));

        // Verificar lo almacenado después de cerrar y reabrir la aplicación.
        var location = api.Response.Headers.Location;
        Assert.That(location, Is.Not.Null);
        api.Restart();
        using var detail = await api.Client.GetAsync(location);
        Assert.That(detail.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await detail.Content.ReadFromJsonAsync<CategoryResponse>(), Is.EqualTo(created));
    }
}
