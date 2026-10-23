using pickpoint_delivery_service;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

using Console_CommonModel.CommonLib.LibModels.Console_LocationModule;


using static Console_CommonModel.CommonLib.LibModels.Console_LocationModule.LocationDbContext;

namespace Console_CommonModel.CommonLib.LibModels.Console_LocationModule
{

    [Label("Инициализация моделей страниц")]
    public class LocationDbContextInitiallizer : DbContextInitiliazer<LocationDbContext>
    {
        public override Dictionary<string, int> Init(LocationDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            Dictionary<string, int> result = new();

            result[nameof(AppPage)] = InitAppPages(context);
            result[nameof(PageComponent)] = InitPageComponents(context);
            result[nameof(AppRoute)] = InitAppRoutes(context);


            Test(context);
            return result;
        }

        private int InitPageComponents(LocationDbContext context)
        {

            if (context.PageComponents.Count() == 0)
            {
                var pageId = context.AppPages.First().Id;
                context.PageComponents.Add(new PageComponent()
                {
                    AppPageId = pageId
                });
                return context.SaveChanges();
            }

            return 0;
        }

        private void Test(LocationDbContext context)
        {
            foreach (var page in context.AppPages)
            {
                context.Info(page.ToJson());
            }
            foreach (var page in context.AppRoutes)
            {
                context.Info(page.ToJson());
            }
            foreach (var page in context.PageComponents)
            {
                context.Info(page.ToJson());
            }
            foreach (var page in context.AppPages)
            {
                context.Info(page.ToJson());
            }
        }

        private int InitAppPages(LocationDbContext context)
        {
            if (context.AppPages.Count() == 0)
            {
                try
                {
                    context.AppPages.Add(new AppPage()
                    {
                        Name = nameof(DeliveryDbContext),
                        ModelType = nameof(DeliveryDbContext),
                        ModelInstance = typeof(DeliveryDbContext).New<DeliveryDbContext>(),
                    });
                    var result = context.SaveChanges();
                    return result;
                }
                catch (DbException ex)
                {
                    this.Error(ex);
                    this.Error(ex.InnerException);
                }
            }
            return 0;
        }

        private int InitAppRoutes(LocationDbContext context)
        {

            foreach (var page in context.AppPages.ToList())
            {
                if (!context.AppRoutes.Any(r => r.AppPageId == page.Id))
                {
                    context.AppRoutes.Add(new()
                    {
                        Uri = "/" + page.Name.ToLower(),
                        AppPageId = page.Id

                    });

                }

            }
            return context.SaveChanges();

        }
    }
}
