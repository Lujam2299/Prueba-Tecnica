namespace Catalogo.Api.Contracts;

// POST y PUT reciben los mismos campos; el identificador proviene de la ruta en PUT.
public sealed record CategoryRequest(string? Name, string? Description);
