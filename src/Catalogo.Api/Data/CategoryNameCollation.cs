using System.Text;

namespace Catalogo.Api.Data;

// Comparación explícita: blancos externos, equivalencia canónica y mayúsculas invariantes.
public static class CategoryNameCollation
{
    public const string Name = "CATEGORY_NAME";

    public static int Compare(string? left, string? right) =>
        string.CompareOrdinal(Normalize(left ?? ""), Normalize(right ?? ""));

    private static string Normalize(string value) =>
        value.Trim().Normalize(NormalizationForm.FormC)
            .ToUpperInvariant().Normalize(NormalizationForm.FormC);
}
