namespace Blazor_SiteModel.Models;

public sealed record MenuLink(string Title, string Href, bool Marked = false, IReadOnlyList<MenuLink>? Children = null);

public sealed record ServiceItem(string Title, string Slug, string Icon, decimal PriceFrom, int Minutes, string Summary);

public sealed record ServiceCategory(string Title, string Slug, string Icon, IReadOnlyList<ServiceItem> Items);

public sealed record PartCategory(string Title, string Slug, string Icon, string Text);

public sealed record Station(string City, string Address, string Hours, string Phone, string Note);

public sealed record Promotion(string Title, string Slug, string City, DateOnly From, DateOnly To, string Lead, string Details);

public sealed record Advantage(string Title, string Text, string Icon);

public sealed record Article(string Title, string Slug, string Category, DateOnly Date, string Excerpt, string Body);

public sealed record FaqItem(string Question, string Answer);

public sealed record FeatureCard(string Title, string Text, string Href, string Button, string Tone);

public sealed record HeroContent(string Title, string Subtitle, string ButtonText, string ButtonHref);

public sealed record InfoPage(string Slug, string Title, string Body);

public sealed record GarageEntry(string Car, string Work, string Place, DateOnly Date, string Result);

public sealed record Vacancy(string Title, string City, string Schedule, string Summary);

public sealed record ChannelCard(string Title, string Text, string Href, string Button);

public sealed record PartnerBadge(string Title, string Caption);

public sealed class AppointmentRequest
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public string Kind { get; init; } = "Запись";
    public string Name { get; init; } = "";
    public string Phone { get; init; } = "";
    public string? ServiceTitle { get; init; }
    public string? Station { get; init; }
    public DateOnly? Date { get; init; }
    public string? Comment { get; init; }
    public decimal? Estimate { get; init; }
}
