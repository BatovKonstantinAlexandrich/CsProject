using System.ComponentModel.DataAnnotations;

namespace Blazor_SiteModel.Models;

public sealed class MustBeTrueAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => value is true;
}

public sealed class VisitFormModel
{
    [Required(ErrorMessage = "Укажите имя")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Укажите телефон")]
    [MinLength(6, ErrorMessage = "Телефон слишком короткий")]
    public string Phone { get; set; } = "";

    public string ServiceSlug { get; set; } = "";

    [Required(ErrorMessage = "Выберите станцию")]
    public string Station { get; set; } = "";

    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "Выберите свободное время")]
    public string Time { get; set; } = "";

    public string Comment { get; set; } = "";

    [MustBeTrue(ErrorMessage = "Нужно согласие на обработку данных")]
    public bool Consent { get; set; }
}

public sealed class EstimateFormModel
{
    [Required(ErrorMessage = "Выберите услугу")]
    public string ServiceSlug { get; set; } = "";

    public string CarClass { get; set; } = "1";

    public decimal Multiplier =>
        decimal.TryParse(CarClass, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var value)
            ? value
            : 1m;

    public string Station { get; set; } = "";

    [Required(ErrorMessage = "Укажите имя")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Укажите телефон")]
    [MinLength(6, ErrorMessage = "Телефон слишком короткий")]
    public string Phone { get; set; } = "";
}

public sealed record TimeSlot(string Time, int FreePlaces);
