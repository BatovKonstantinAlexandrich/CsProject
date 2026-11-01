using Blazor_SiteModel.Models;

namespace Blazor_SiteModel.Services;

public interface IMenuService
{
    IReadOnlyList<MenuLink> GetMainMenu();

    IReadOnlyList<MenuLink> GetFooterClients();

    IReadOnlyList<MenuLink> GetFooterCompany();

    IReadOnlyList<MenuLink> GetLegalLinks();
}

public sealed class MenuService(ICatalogService catalog, IStationService stations) : IMenuService
{
    public IReadOnlyList<MenuLink> GetMainMenu()
    {
        var services = new List<MenuLink> { new("Все", "/services") };
        foreach (var category in catalog.GetCategories())
        {
            services.Add(new MenuLink(
                category.Title,
                $"/services?category={category.Slug}",
                Children: category.Items
                    .Select(item => new MenuLink(item.Title, $"/services/{item.Slug}"))
                    .ToList()));
        }

        var cities = stations.GetCities()
            .Select(city => new MenuLink(city, $"/stations?city={Uri.EscapeDataString(city)}"))
            .ToList();

        return
        [
            new MenuLink("Услуги", "/services", Children: services),
            new MenuLink("Рассчитать стоимость", "/calculate"),
            new MenuLink("Адреса СТО", "/stations", Children: cities),
            new MenuLink("Акции", "/promotions", Marked: true),
            new MenuLink("Запчасти", "/parts", Marked: true),
            new MenuLink("Ещё", "/about", Children:
            [
                new MenuLink("О нас", "/about"),
                new MenuLink("Корпоративным клиентам", "/corporate"),
                new MenuLink("Блог", "/blog"),
                new MenuLink("Вакансии", "/vacancies")
            ])
        ];
    }

    public IReadOnlyList<MenuLink> GetFooterClients() =>
    [
        new("Оплата", "/info/oplata"),
        new("Доставка", "/info/dostavka"),
        new("Гарантия и возврат", "/info/garantiya"),
        new("Программа лояльности", "/info/loyalnost"),
        new("Акции", "/promotions")
    ];

    public IReadOnlyList<MenuLink> GetFooterCompany() =>
    [
        new("О нас", "/about"),
        new("Сертификаты", "/info/sertifikaty"),
        new("Блог", "/blog"),
        new("Академия", "/info/akademiya"),
        new("Вакансии", "/vacancies")
    ];

    public IReadOnlyList<MenuLink> GetLegalLinks() =>
    [
        new("Политика конфиденциальности", "/info/privacy"),
        new("Юридическая информация", "/info/legal"),
        new("Контакты", "/info/kontakty")
    ];
}
