namespace BlazorErp.Services;

internal static class AddressProximity
{
    public static int Score(string deliveryAddress, string? locationAddress)
    {
        if (string.IsNullOrWhiteSpace(locationAddress))
            return 0;

        var deliveryTokens = Tokenize(deliveryAddress);
        var locationTokens = Tokenize(locationAddress);
        return deliveryTokens.Intersect(locationTokens).Count();
    }

    private static HashSet<string> Tokenize(string address) =>
        address.Split([' ', ',', '.', ';', '-'], StringSplitOptions.RemoveEmptyEntries)
            .Where(t => t.Length > 2)
            .Select(t => t.ToLowerInvariant())
            .ToHashSet();
}
