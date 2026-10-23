namespace BlazorErp.Models;

public static class RoleNames
{
    public const string Client = "Client";
    public const string TransportCompany = "TransportCompany";
    public const string StoreOwner = "StoreOwner";
    public const string Seller = "Seller";
    public const string Cashier = "Cashier";
    public const string Supplier = "Supplier";
    public const string AutoServiceOwner = "AutoServiceOwner";
    public const string AutoMechanic = "AutoMechanic";
    public const string AutoElectrician = "AutoElectrician";
    public const string TireServiceWorker = "TireServiceWorker";
    public const string CarWashWorker = "CarWashWorker";
    public const string Admin = "Admin";
    public const string Developer = "Developer";
    public const string TechnicalSupport = "TechnicalSupport";
    public const string CallCenter = "CallCenter";
    public const string Marketing = "Marketing";
    public const string Analyst = "Analyst";

    public static readonly string[] All =
    [
        Client, TransportCompany, StoreOwner, Seller, Cashier, Supplier,
        AutoServiceOwner, AutoMechanic, AutoElectrician, TireServiceWorker, CarWashWorker,
        Admin, Developer, TechnicalSupport, CallCenter, Marketing, Analyst
    ];

    public static readonly string[] PreviewableRoles =
    [
        Client, TransportCompany, StoreOwner, Seller, Cashier, Supplier,
        AutoServiceOwner, AutoMechanic, AutoElectrician, TireServiceWorker, CarWashWorker,
        Admin, TechnicalSupport, CallCenter, Marketing, Analyst
    ];
}
