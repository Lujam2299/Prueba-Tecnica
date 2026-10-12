namespace Catalogo.Api.Contracts;

public sealed record ProductRequest(string? Name, string? Description, int? CategoryId);
