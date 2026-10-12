using Microsoft.AspNetCore.Mvc.Testing;
using Reqnroll;

namespace Catalogo.AcceptanceTests.Support;

// Reqnroll crea esta clase para cada escenario mediante inyección por constructor.
public sealed class ApiScenario
{
    private WebApplicationFactory<Program>? factory;

    public HttpClient Client { get; private set; } = null!;
    public HttpResponseMessage? Response { get; set; }

    public void Start()
    {
        factory = new WebApplicationFactory<Program>();
        Client = factory.CreateClient();
    }

    public void Dispose()
    {
        Response?.Dispose();
        Client?.Dispose();
        factory?.Dispose();
    }
}

[Binding]
public sealed class ApiHooks(ApiScenario api)
{
    [AfterScenario]
    public void CloseApi() => api.Dispose();
}
