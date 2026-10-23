namespace BlazorErp.Models;

public class SupplierBrand
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public int BrandId { get; set; }
    public Brand Brand { get; set; } = null!;
}
