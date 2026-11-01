using ApplicationDb.Entities;

using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext: DeliveryDbContext
{
    public DbSet<Calendar> Calendars { get; set; }
}