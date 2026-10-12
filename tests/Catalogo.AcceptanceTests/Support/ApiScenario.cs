using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Reqnroll;

namespace Catalogo.AcceptanceTests.Support;

// Reqnroll crea esta clase para cada escenario mediante inyección por constructor.
public sealed class ApiScenario
{
    private WebApplicationFactory<Program>? factory;
    private readonly string databasePath = Path.Combine(Path.GetTempPath(), $"catalogo-test-{Guid.NewGuid():N}.db");

    public HttpClient Client { get; private set; } = null!;
    public HttpResponseMessage? Response { get; set; }

    public void Start()
    {
        factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.UseSetting("ConnectionStrings:Catalogo", $"Data Source={databasePath};Pooling=False"));
        Client = factory.CreateClient();
    }

    public void Restart()
    {
        Close();
        Start();
    }

    public void Dispose()
    {
        Close();
        // Solo archivos con el nombre único creado para este escenario.
        foreach (var suffix in new[] { "", "-wal", "-shm" })
            File.Delete(databasePath + suffix);
    }

    private void Close()
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
