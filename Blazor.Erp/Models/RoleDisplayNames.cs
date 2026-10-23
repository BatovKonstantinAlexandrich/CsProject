namespace BlazorErp.Models;

public static class RoleDisplayNames
{
    public static string GetDisplayName(string role) => role switch
    {
        RoleNames.Client => "Клиент",
        RoleNames.TransportCompany => "Транспортная компания",
        RoleNames.StoreOwner => "Владелец магазина",
        RoleNames.Seller => "Продавец",
        RoleNames.Cashier => "Кассир",
        RoleNames.Supplier => "Поставщик",
        RoleNames.AutoServiceOwner => "Владелец автосервиса",
        RoleNames.AutoMechanic => "Авто-механик",
        RoleNames.AutoElectrician => "Авто-электрик",
        RoleNames.TireServiceWorker => "Шиномонтаж",
        RoleNames.CarWashWorker => "Мойка",
        RoleNames.Admin => "Администратор",
        RoleNames.Developer => "Разработчик",
        RoleNames.TechnicalSupport => "Тех. поддержка",
        RoleNames.CallCenter => "Кол-центр",
        RoleNames.Marketing => "Отдел маркетинга",
        RoleNames.Analyst => "Аналитик",
        _ => role
    };
}
