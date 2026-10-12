using Catalogo.Api.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Api.Data;

public sealed class CatalogoDbContext : DbContext
{
    public CatalogoDbContext(DbContextOptions<CatalogoDbContext> options) : base(options)
    {
        // Registrar también para migraciones y conexiones utilizadas fuera de los endpoints.
        var connection = (SqliteConnection)Database.GetDbConnection();
        connection.CreateCollation(CategoryNameCollation.Name, CategoryNameCollation.Compare);
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().Property(category => category.Name)
            .IsRequired().UseCollation(CategoryNameCollation.Name);
        modelBuilder.Entity<Category>().HasIndex(category => category.Name).IsUnique();

        modelBuilder.Entity<Product>().Property(product => product.Name).IsRequired();
        modelBuilder.Entity<Product>().HasOne(product => product.Category).WithMany()
            .HasForeignKey(product => product.CategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}
