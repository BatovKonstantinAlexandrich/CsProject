using ApplicationDb.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public static class DoctorServiceExtensions
{
    public static IServiceCollection AddDoctorRole(this IServiceCollection services)
    {
        services.AddScoped<IDoctorService, DoctorService>();
        return services;
    }
}




public interface IDoctorService
{
    public IEnumerable<UserPerson> GetDoctorPatients(int doctorId);
}




public class DoctorService : IDoctorService
{
    public IEnumerable<UserPerson> GetDoctorPatients(int doctorId)
    {
     
        return new UserPerson[] { 
            new UserPerson(){ SurName="Батов",  FirstName="Константин",  LastName="Александрович", Birthday=DateTime.Parse("26.08.2021")}
        };
    }
}