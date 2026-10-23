using ApplicationDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class BusinessPageServiceExtensions
{
    public static IServiceCollection AddBusinessPages( this IServiceCollection services )
    {
        services.AddScoped<IBusinessPageService, BusinessPageService>();
        return services;
    }
}

public interface IBusinessPageService 
{
    public void CreateBusinessPage(string location);
    public bool HasPageForLocation(string location);
    public InputFormModel GetFormForLocation(string location);
}

public class BusinessPageService : IBusinessPageService
{
    private readonly InputFormDbContext _forms;

    public BusinessPageService(InputFormDbContext forms) {
        _forms = forms;
    }

    public void CreateBusinessPage(string location)
    {
        var form = new InputFormModel { Icon = "home" };
        _forms.InputFormModels.Add(form);
        _forms.SaveChanges();

        var path = NormalizePath(location);
        _forms.BusinessPageModel.Add(new BusinessPageModel()
        {
            Form = form,
            Location = path,
            Name = $"страница {DateTime.UtcNow.Ticks}"
        });
        _forms.SaveChanges();
    }

    public InputFormModel GetFormForLocation(string location)
    {
        return FindPage(location)?.Form;
    }

    public bool HasPageForLocation(string location)
    {
        return FindPage(location) != null;
    }

    private BusinessPageModel FindPage(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            return null;

        var path = NormalizePath(location);
        var segment = LastPathSegment(path);
        var raw = location.Trim();

        return _forms.BusinessPageModel
            .Include(p => p.Form)
            .FirstOrDefault(p =>
                (!string.IsNullOrEmpty(p.Location) &&
                 (string.Equals(p.Location, raw, StringComparison.OrdinalIgnoreCase)
                  || string.Equals(p.Location, path, StringComparison.OrdinalIgnoreCase)))
                || (!string.IsNullOrEmpty(p.Name) &&
                    (string.Equals(p.Name, raw, StringComparison.OrdinalIgnoreCase)
                     || (!string.IsNullOrEmpty(segment) &&
                         string.Equals(p.Name, segment, StringComparison.OrdinalIgnoreCase)))));
    }

    private static string NormalizePath(string location)
    {
        var t = location.Trim();
        if (t.Contains("://", StringComparison.Ordinal))
        {
            try
            {
                var uri = new Uri(t);
                return string.IsNullOrEmpty(uri.AbsolutePath) ? "/" : uri.AbsolutePath;
            }
            catch (UriFormatException)
            {
                // fall through
            }
        }

        return t.StartsWith("/", StringComparison.Ordinal) ? t : "/" + t;
    }

    private static string LastPathSegment(string path)
    {
        if (string.IsNullOrEmpty(path) || path == "/")
            return null;
        var parts = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 0 ? null : parts[^1];
    }
}
