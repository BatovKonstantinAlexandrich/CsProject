using BlazorErp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorErp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext( ): base( )
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured)
        {
            var connectionString = "Data Source=blazorerp.db";
            optionsBuilder.UseSqlite(connectionString);
        }
    }

    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<StoreProduct> StoreProducts => Set<StoreProduct>();
    public DbSet<StoreSeller> StoreSellers => Set<StoreSeller>();
    public DbSet<StoreSellerRegistrationRequest> StoreSellerRegistrationRequests => Set<StoreSellerRegistrationRequest>();
    public DbSet<StoreCashier> StoreCashiers => Set<StoreCashier>();
    public DbSet<StoreCashierRegistrationRequest> StoreCashierRegistrationRequests => Set<StoreCashierRegistrationRequest>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SupplierBrand> SupplierBrands => Set<SupplierBrand>();
    public DbSet<SupplierProduct> SupplierProducts => Set<SupplierProduct>();
    public DbSet<SupplierOrder> SupplierOrders => Set<SupplierOrder>();
    public DbSet<SupplierOrderItem> SupplierOrderItems => Set<SupplierOrderItem>();
    public DbSet<WarehouseOrder> WarehouseOrders => Set<WarehouseOrder>();
    public DbSet<WarehouseOrderItem> WarehouseOrderItems => Set<WarehouseOrderItem>();
    public DbSet<NewsItem> NewsItems => Set<NewsItem>();
    public DbSet<NewsAttachment> NewsAttachments => Set<NewsAttachment>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<AutoService> AutoServices => Set<AutoService>();
    public DbSet<AutoServiceOffering> AutoServiceOfferings => Set<AutoServiceOffering>();
    public DbSet<AutoServiceStaffMember> AutoServiceStaffMembers => Set<AutoServiceStaffMember>();
    public DbSet<AutoServiceAppointment> AutoServiceAppointments => Set<AutoServiceAppointment>();
    public DbSet<AutoServiceEmployee> AutoServiceEmployees => Set<AutoServiceEmployee>();
    public DbSet<AutoServiceEmployeeRegistrationRequest> AutoServiceEmployeeRegistrationRequests =>
        Set<AutoServiceEmployeeRegistrationRequest>();
    public DbSet<SaleBasket> SaleBaskets => Set<SaleBasket>();
    public DbSet<SaleBasketItem> SaleBasketItems => Set<SaleBasketItem>();
    public DbSet<CashPayment> CashPayments => Set<CashPayment>();
    public DbSet<CashRegisterEntry> CashRegisterEntries => Set<CashRegisterEntry>();
    public DbSet<PaymentLinkLog> PaymentLinkLogs => Set<PaymentLinkLog>();
    public DbSet<MailingConfiguration> MailingConfigurations => Set<MailingConfiguration>();
    public DbSet<MailingTestAudience> MailingTestAudiences => Set<MailingTestAudience>();
    public DbSet<ReportGroup> ReportGroups => Set<ReportGroup>();
    public DbSet<ReportProcedure> ReportProcedures => Set<ReportProcedure>();
    public DbSet<ReportParameter> ReportParameters => Set<ReportParameter>();
    public DbSet<ReportColumn> ReportColumns => Set<ReportColumn>();
    public DbSet<ReportUserFavorite> ReportUserFavorites => Set<ReportUserFavorite>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CartItem>()
            .HasIndex(c => new { c.UserId, c.ProductId })
            .IsUnique();

        builder.Entity<Order>()
            .HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.Entity<Order>()
            .HasOne(o => o.Payment)
            .WithOne(p => p.Order)
            .HasForeignKey<Payment>(p => p.OrderId);

        builder.Entity<Order>()
            .HasOne(o => o.Delivery)
            .WithOne(d => d.Order)
            .HasForeignKey<Delivery>(d => d.OrderId);

        builder.Entity<Order>()
            .HasOne(o => o.Review)
            .WithOne(r => r.Order)
            .HasForeignKey<Review>(r => r.OrderId);

        builder.Entity<Message>()
            .HasOne(m => m.ParentMessage)
            .WithMany(m => m.Replies)
            .HasForeignKey(m => m.ParentMessageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Category>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StoreProduct>()
            .HasIndex(sp => new { sp.StoreId, sp.ProductId })
            .IsUnique();

        builder.Entity<StoreSeller>()
            .HasIndex(s => s.UserId)
            .IsUnique();

        builder.Entity<StoreCashier>()
            .HasIndex(c => c.UserId)
            .IsUnique();

        builder.Entity<AutoServiceEmployee>()
            .HasIndex(e => e.UserId)
            .IsUnique();

        builder.Entity<SupplierBrand>()
            .HasIndex(sb => new { sb.SupplierId, sb.BrandId })
            .IsUnique();

        builder.Entity<SupplierProduct>()
            .HasIndex(sp => new { sp.SupplierId, sp.ProductId })
            .IsUnique();

        builder.Entity<Product>()
            .HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<SaleBasket>()
            .HasIndex(b => b.BasketNumber)
            .IsUnique();

        builder.Entity<SaleBasket>()
            .HasIndex(b => b.OrderUid)
            .IsUnique();

        builder.Entity<CashPayment>()
            .HasIndex(p => p.ExternalPaymentId)
            .IsUnique();

        builder.Entity<ReportUserFavorite>()
            .HasIndex(f => new { f.UserId, f.ReportId })
            .IsUnique();
    }
}
