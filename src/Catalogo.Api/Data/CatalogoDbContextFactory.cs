using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Catalogo.Api.Data;

// La herramienta de migraciones construye el contexto sin arrancar la API.
public sealed class CatalogoDbContextFactory : IDesignTimeDbContextFactory<CatalogoDbContext>
{
    public CatalogoDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CatalogoDbContext>()
            .UseSqlite("Data Source=catalogo.db")
            .Options;
        return new CatalogoDbContext(options);
    }
}
