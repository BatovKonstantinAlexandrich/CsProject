using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibModels.Console_LocationModule;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;

namespace Console_CommonModel.CommonLib.LibModels.Console_LocationModule
{
    /// <summary>
    /// Хранилище динамических форм ввода с навигацией
    /// </summary>
    [Label("Динамические формы")]
    public partial class LocationDbContext : DbContextBase
    {

        /// <summary>
        /// Маршруты привязывают адреса со страницами
        /// </summary>
        public DbSet<AppRoute> AppRoutes { get; set; }


        /// <summary>
        /// Страница определяют набор визуальных компонентов
        /// </summary>
        public DbSet<AppPage> AppPages { get; set; }


        /// <summary>
        /// Компоненты соединяют данные с параметрами визуализации
        /// </summary>
        public DbSet<PageComponent> PageComponents { get; set; }


        public LocationDbContext() : base() { }
        public LocationDbContext(DbContextOptions<LocationDbContext> options) : base(options) { }


        /// <summary>
        /// Настройка соединения
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (optionsBuilder.IsConfigured == false)
            {
                optionsBuilder.UseSqlServer($@"Data Source=DESKTOP-IHJM9RD;Initial Catalog={typeof(LocationDbContext).GetTypeName()};Integrated Security=True;Connect Timeout=30;Encrypt=False;Application Intent=ReadWrite;");
            }
        }
    }
}
