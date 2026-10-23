using System.Text.Json.Serialization;

namespace BlazorErp.Models.BookingApi;

public class BookingTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}

public class BookingServiceStation
{
    [JsonPropertyName("service_station_id")]
    public string ServiceStationId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("opening_hours")]
    public string OpeningHours { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public BookingCity? City { get; set; }
}

public class BookingCity
{
    [JsonPropertyName("city_id")]
    public string CityId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class BookingJob
{
    [JsonPropertyName("job_id")]
    public string JobId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("customer_description_problem")]
    public string? CustomerDescriptionProblem { get; set; }
}

public class BookingFreeSlotDay
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("time")]
    public List<string> Time { get; set; } = [];
}

public class CreateBookingRequest
{
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    [JsonPropertyName("service_station_id")]
    public string ServiceStationId { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;

    [JsonPropertyName("job_ids")]
    public List<string> JobIds { get; set; } = [];

    [JsonPropertyName("specific_job_parameters")]
    public List<object> SpecificJobParameters { get; set; } = [];

    [JsonPropertyName("user_request_summary")]
    public string UserRequestSummary { get; set; } = string.Empty;
}

public class CreateBookingResponse
{
    [JsonPropertyName("booking_id")]
    public string? BookingId { get; set; }
}

public class ClientBooking
{
    [JsonPropertyName("booking_id")]
    public string BookingId { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;

    [JsonPropertyName("service_station_id")]
    public string ServiceStationId { get; set; } = string.Empty;

    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    [JsonPropertyName("jobs")]
    public List<ClientBookingJob> Jobs { get; set; } = [];
}

public class ClientBookingJob
{
    [JsonPropertyName("job_id")]
    public string JobId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class BookingSlotsRequest
{
    [JsonPropertyName("job_ids")]
    public List<string> JobIds { get; set; } = [];

    [JsonPropertyName("specific_job_parameters")]
    public List<object> SpecificJobParameters { get; set; } = [];

    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    [JsonPropertyName("days_ahead")]
    public int? DaysAhead { get; set; }
}

public class BookingApiValidationError
{
    [JsonPropertyName("detail")]
    public List<BookingApiValidationErrorItem> Detail { get; set; } = [];
}

public class BookingApiValidationErrorItem
{
    [JsonPropertyName("msg")]
    public string? Message { get; set; }

    [JsonPropertyName("loc")]
    public List<string>? Location { get; set; }
}

public class TireFittingParametersPayload
{
    [JsonPropertyName("specific_job")]
    public string SpecificJob { get; set; } = "tire_fitting";

    [JsonPropertyName("specific")]
    public TireFittingSpecificPayload Specific { get; set; } = new();
}

public class TireFittingSpecificPayload
{
    [JsonPropertyName("tire_source")]
    public string TireSource { get; set; } = "new_assembled";

    [JsonPropertyName("keep_current_in_storage")]
    public bool KeepCurrentInStorage { get; set; }

    [JsonPropertyName("diameter")]
    public int Diameter { get; set; } = 16;

    [JsonPropertyName("car_body_type")]
    public string CarBodyType { get; set; } = "passenger_or_crossover";
}
