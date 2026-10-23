
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


public class StatisticsService: BaseService
{
    private readonly ILogger<StatisticsService> _logger;
    private readonly ApplicationDbContext _context;

    public StatisticsService(ILogger<StatisticsService> logger, ApplicationDbContext db) {
        _logger = logger;
        _context = db;


    }

 

    public async Task SaveStatisticsOLAP()
    {
        
        
            foreach (Type entityType in GetFactsTables(_context))
            {
                var fact = entityType;
                var dims = ReflectionService.CreateWithDefaultConstructor<BaseEntity>(entityType).GetDimensions();                
            }

            await _context.SaveChangesAsync();
        
    }

    public IEnumerable<Type> GetFactsTables(ApplicationDbContext context)
    {
        return DataQualificationExtensions.GetFactsTables(context);
    }
}
