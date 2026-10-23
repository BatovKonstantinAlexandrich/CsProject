using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Label("База данных содержит сведения по интеграциям")]
public class IntegrationDbContext : DbContextBase
{
    public virtual DbSet<IntegrationLog> IntegrationLogs { get; set; }

    public IntegrationDbContext()
    {
    }

    public IntegrationDbContext(DbContextOptions options) : base(options)
    {
    }


    [Label("Запись об результатах выполнения задачи интеграции")]
    public class IntegrationLog: BaseEntity
    {
        [NotNullNotEmpty]
        public string OperationName { get; set; }

        public DateTime StartedAt { get; set; }

        public bool OperationCompleted { get; set; }
        
        public string ExceptionMessage { get; set; }
    }
}