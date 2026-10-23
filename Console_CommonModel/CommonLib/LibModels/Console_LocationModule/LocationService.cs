using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;

using Console_CommonModel.CommonLib.LibModels.Console_LocationModule;

using static Console_CommonModel.CommonLib.LibModels.Console_LocationModule.LocationDbContext;

namespace Console_CommonModel.CommonLib.LibModels.Console_LocationModule
{

    [Label("Проверка виртуального пространства")]
    [Description("LocationService обходит все URI из GetUrls и вызывает Navigate без исключений.")]
    public class LocationServiceTest : TestingService<LocationService>
    {
        public LocationServiceTest(IServiceProvider sp) : base(sp)
        {
        }

        public override bool OnServiceTest(LocationService service)
        {
            var navModel = service.GetUrls();
            LogMessage($"Маршрутов в навигационной модели: {navModel.Count}");
            foreach (var url in navModel.Keys)
            {
                LogMessage($"Navigate: {url} → {navModel[url]}");
                service.Navigate(url);
            }
            LogMessage("Обход всех URI завершён");
            return true;
        }
    }


    /// <summary>
    /// Сервис виртуальной навигации
    /// </summary>
    public class LocationService
    {
        private readonly LocationDbContext locationDbContext;

        public LocationService(LocationDbContext locationDbContext)
        {
            this.locationDbContext = locationDbContext;
        }

        /// <summary>
        /// Получение навигационной модели
        /// </summary>
        public Dictionary<string, string> GetUrls()
        {

            var kvs = locationDbContext.AppRoutes.Include(p => p.AppPage).Select(r => new KeyValuePair<string, string>(r.AppPage.Name, r.Uri)).ToList();
            return new(kvs);
        }

        /// <summary>
        /// Переход на другой адрес
        /// </summary>
        /// <param name="uri">относительный путь</param>
        /// <returns>страница компонентов</returns>
        public AppPage Navigate(string uri)
        {
            var route = locationDbContext.AppRoutes.FirstOrDefault(route => route.Uri == uri);
            if (route == null)
            {
                return null;
            }
            else
            {
                return locationDbContext.AppPages.Include(p => p.PageComponents).FirstOrDefault(p => p.Id == route.AppPageId);
            }
        }
    }
}
