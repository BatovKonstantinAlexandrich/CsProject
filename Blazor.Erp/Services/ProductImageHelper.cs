using BlazorErp.Models;

namespace BlazorErp.Services;

public static class ProductImageHelper
{
    private static readonly string[] Palette =
    [
        "6750A4", "006493", "386A20", "7D5260", "625B71", "984061", "006874"
    ];

    public static string GetImageUrl(Product product) =>
        !string.IsNullOrWhiteSpace(product.ImageUrl)
            ? product.ImageUrl
            : GetPlaceholderUrl(product.Id, product.Name);

    public static string GetPlaceholderUrl(int productId, string name)
    {
        var color = Palette[Math.Abs(productId) % Palette.Length];
        var text = Uri.EscapeDataString(name.Length > 18 ? name[..18] : name);
        return $"https://placehold.co/480x320/{color}/FFFFFF/png?text={text}";
    }
}
