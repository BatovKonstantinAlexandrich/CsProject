using Blazor_SiteModel.Models;

namespace Blazor_SiteModel.Services;

public interface IAppointmentService
{
    void Add(AppointmentRequest request);

    IReadOnlyList<AppointmentRequest> GetAll();
}

public sealed class AppointmentService : IAppointmentService
{
    private readonly List<AppointmentRequest> _items = [];
    private readonly object _gate = new();

    public void Add(AppointmentRequest request)
    {
        lock (_gate)
            _items.Insert(0, request);
    }

    public IReadOnlyList<AppointmentRequest> GetAll()
    {
        lock (_gate)
            return _items.ToList();
    }
}
