namespace Catalogo.Api.Contracts;

public sealed record ProductResponse(int Id, string Name, string? Description, int CategoryId, string CategoryName);
