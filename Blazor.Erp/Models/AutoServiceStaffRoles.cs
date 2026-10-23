namespace BlazorErp.Models;

public static class AutoServiceStaffRoles
{
    public static readonly string[] All =
    [
        RoleNames.AutoMechanic,
        RoleNames.AutoElectrician,
        RoleNames.TireServiceWorker,
        RoleNames.CarWashWorker
    ];

    public static bool IsStaffRole(string? role) =>
        role != null && All.Contains(role);

    public static string GetDisplayName(string role) => role switch
    {
        RoleNames.AutoMechanic => "Авто-механик",
        RoleNames.AutoElectrician => "Авто-электрик",
        RoleNames.TireServiceWorker => "Шиномонтаж",
        RoleNames.CarWashWorker => "Мойка",
        _ => role
    };

    public static string GetDisplayNameAccusative(string role) => role switch
    {
        RoleNames.AutoMechanic => "авто-механика",
        RoleNames.AutoElectrician => "авто-электрика",
        RoleNames.TireServiceWorker => "шиномонтажа",
        RoleNames.CarWashWorker => "мойки",
        _ => role
    };
}
