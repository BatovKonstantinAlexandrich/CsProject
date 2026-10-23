namespace BlazorErp.Services;

public class VendorsSetupApiOptions
{
    public const string SectionName = "VendorsSetupApi";

    public bool Enabled { get; set; } = true;
    public List<VendorSupplierMapping> Suppliers { get; set; } = [];
}

public class VendorSupplierMapping
{
    public string SupplierName { get; set; } = string.Empty;
    public int VendorId { get; set; }
    public int ShopId { get; set; }
    public Dictionary<string, string> SearchTerms { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
