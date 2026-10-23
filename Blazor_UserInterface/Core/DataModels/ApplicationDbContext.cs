
using ApplicationDb.Entities;
using Blazor.Core.SharedData.DataModels.Authorization;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public static class ApplicationDbContextExtensions
{

    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration Configuration)
    { 
        
        string connection = Configuration.GetConnectionString(nameof(ApplicationDbContext));
        if(connection == null)
        {
            throw new Exception("Добавьте строку соединения для "+nameof(ApplicationDbContext));
        }

        Console.WriteLine($"AddApplicationDbContext({connection})");
        services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
            optionsBuilder.UseSqlServer(connection)
        );
        services.AddScoped(typeof(IBusinessModel), (provider) => {
            var ctx = (ApplicationDbContext)provider.GetService(typeof(ApplicationDbContext));
            return ctx;
        });
        services.AddScoped(typeof(IAuthorizationModel), (provider) => {
            var ctx = (ApplicationDbContext)provider.GetService(typeof(ApplicationDbContext));
            return ctx;
        });
        services.AddScoped(typeof(IMedicalCardModel), (provider) => {
            var ctx = (ApplicationDbContext)provider.GetService(typeof(ApplicationDbContext));
            return ctx;
        });
        services.AddScoped(typeof(IResourseModel), (provider) => {
            var ctx = (ApplicationDbContext)provider.GetService(typeof(ApplicationDbContext));
            return ctx;
        });
        
        services.AddScoped(typeof(IManagmentModel), (provider) => {
            var ctx = (ApplicationDbContext)provider.GetService(typeof(ApplicationDbContext));
            return ctx;
        });
        services.AddScoped(typeof(IAuthorizationModel), (provider) => {
            var ctx = (ApplicationDbContext)provider.GetService(typeof(ApplicationDbContext));
            return ctx;
        });                
        return services;
    }
}

public class ApplicationDbContext : DbContext, IAuthorizationModel, IBusinessModel, IManagmentModel, IMedicalModel, IResourseModel

{
    public static int Counter = StartupApplication.AddWithOptions((services, conf) => {
        services.AddApplicationDbContext(conf);
    });
    public virtual DbSet<Migration> Migrations { get; set; }
    public virtual DbSet<UserAccount> Accounts { get; set; }
    public virtual DbSet<Group> Groups { get; set; }
    public virtual DbSet<GroupMessage> GroupMessages { get; set; }
    public virtual DbSet<UserAuthEvent> LoginFacts { get; set; }
    public virtual DbSet<UserMessage> Messages { get; set; }
    public virtual DbSet<NewsMessage> News { get; set; }
    public virtual DbSet<UserPerson> Persons { get; set; }
    public virtual DbSet<UserSettings> Settings { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserGroups> UserGroups { get; set; }
    public virtual DbSet<Service> Services { get; set; }
    public virtual DbSet<ServiceGroups> ServiceGroups { get; set; }

    internal string[] GetTableMetaData()
    {
        return GetEntityTypeNames().ToArray();
    }

    public virtual DbSet<GroupsBusinessFunctions> GroupsBusinessFunctions { get; set; }
    public virtual DbSet<BusinessDatasource> BusinessDatasources { get; set; }

    internal MyActionModel[] GetEntityFasade()
    {
        return GetEntityTypeNames()
            .Select(name => new MyActionModel
            {
                Name = name,
                Path = "/" + name
            })
            .ToArray();
    }

    public virtual DbSet<BusinessFunction> BusinessFunctions { get; set; }
    public virtual DbSet<BusinessLogic> BusinessLogics { get; set; }
    public virtual DbSet<BusinessProcess> BusinessProcesss { get; set; }
    public virtual DbSet<BusinessReport> BusinessReports { get; set; }
    public virtual DbSet<BusinessResource> BusinessResources { get; set; }
    public virtual DbSet<BusinessProcess> BusinessProcesses { get; set; }
    public virtual DbSet<MessageAttribute> MessageAttributes { get; set; }
    public virtual DbSet<MessageProperty> MessageProperties { get; set; }
    public virtual DbSet<MessageProtocol> MessageProtocols { get; set; }
    public virtual DbSet<ValidationModel> ValidationModels { get; set; }
    public virtual DbSet<BusinessData> BusinessData { get; set; }
    public virtual DbSet<BusinessIndicator> BusinessIndicators { get; set; }
    public virtual DbSet<BusinessDataset> BusinessDatasets { get; set; }
    public virtual DbSet<BusinessGranularities> Granularities { get; set; }
    public virtual DbSet<ManagmentOrganization> Organizations { get; set; }
    public virtual DbSet<ManagmentDepartment> Departments { get; set; }
    public virtual DbSet<ManagmentEmployee> Employees { get; set; }
    public virtual DbSet<EmployeeExpirience> EmployeeExpirience { get; set; }
    public virtual DbSet<ManagmentPosition> Positions { get; set; }
    public virtual DbSet<PositionFunction> PositionFunctions { get; set; }
    public virtual DbSet<FunctionSkills> FunctionSkills { get; set; }
    public virtual DbSet<SalaryReport> SalaryReports { get; set; }
    public virtual DbSet<ProfessionalSkill> Skills { get; set; }
    public virtual DbSet<StaffsTable> Staffs { get; set; }
    public virtual DbSet<PaymentPersonal> TariffRates { get; set; }
    public virtual DbSet<TimeSheet> TimeSheets { get; set; }
    public virtual DbSet<Calendar> Calendars { get; set; }
    public virtual DbSet<MedicalCard> MedicalCards { get; set; }
    public virtual DbSet<MedicalStep> MedicalSteps { get; set; }
    public virtual DbSet<MedicalCase> MedicalCases { get; set; }
    public virtual DbSet<ManagmentPosition> ManagmentPositions { get; set; }
    public virtual DbSet<MedicalBed> MedicalBeds { get; set; }
    public virtual DbSet<MedicalDevice> MedicalDevices { get; set; }
    public virtual DbSet<MedicalFunction> MedicalFunctions { get; set; }
    public virtual DbSet<MedicalLab> MedicalLabs { get; set; }
    public virtual DbSet<MedicalRoom> MedicalRoom { get; set; }
    public virtual DbSet<FileCatalog> FileCatalogs { get; set; }
    public virtual DbSet<FileResource> FilResources { get; set; }
    public virtual DbSet<ImageResource> Photos { get; set; }
    public virtual DbSet<Resource> Resources { get; set; }

    public ApplicationDbContext() : base() {
    }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public List<string> GetEntityTypeNames()
    {        
        return this.GetEntitiesTypes().Select(e => Typing.ParseCollectionType(e)).ToList();
    }

    public int Save()
    {
        return SaveChanges();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if(optionsBuilder.IsConfigured == false)
        {
            optionsBuilder.UseSqlServer(
                @"Server=DESKTOP-IHJM9RD;DataBase=RootForms;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"
            );
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserPerson>(entity =>
        {
            entity.Property(e => e.Birthday).IsRequired(false);
        });
    }

}
