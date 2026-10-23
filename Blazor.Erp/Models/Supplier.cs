namespace BlazorErp.Models;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public ApplicationUser Owner { get; set; } = null!;
    public List<SupplierBrand> Brands { get; set; } = [];
    public List<SupplierProduct> Products { get; set; } = [];
    public List<SupplierOrder> Orders { get; set; } = [];
}
