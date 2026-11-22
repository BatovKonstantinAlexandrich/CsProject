using Blazor_SiteModel.Models;

namespace Blazor_SiteModel.Services;

public interface IScheduleService
{
    IReadOnlyList<DateOnly> GetDates(int days = 14);

    IReadOnlyList<TimeSlot> GetSlots(DateOnly date, string stationKey);
}

public sealed class ScheduleService : IScheduleService
{
    public IReadOnlyList<DateOnly> GetDates(int days = 14) =>
        Enumerable.Range(0, days).Select(offset => DateOnly.FromDateTime(DateTime.Today.AddDays(offset))).ToList();

    public IReadOnlyList<TimeSlot> GetSlots(DateOnly date, string stationKey)
    {
        var seed = date.DayNumber + stationKey.Sum(ch => ch);
        return Enumerable.Range(9, 13)
            .Select(hour => new TimeSlot($"{hour:00}:00", FreePlaces(seed, hour)))
            .ToList();
    }

    private static int FreePlaces(int seed, int hour)
    {
        var value = (seed + hour * 3) % 7;
        return value switch
        {
            0 or 1 => 0,
            2 => 1,
            3 or 4 => 2,
            _ => 3
        };
    }
}
