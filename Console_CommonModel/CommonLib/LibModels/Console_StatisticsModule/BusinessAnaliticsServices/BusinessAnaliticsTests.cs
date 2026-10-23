using Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule.Services;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule.BusinessAnaliticsServices
{
    [Label("Statistics: загрузка данных HighchartBusinessDataService")]
    [Description("Выбор параметров аналитики в EditView, Load и отображение HighchartBarBasic.")]
    public class BusinessAnaliticsTests : TestingService<HighchartBusinessDataService>
    {
        public override bool OnServiceTest(HighchartBusinessDataService service)
        {
            LogMessage("EditView: выбор параметров HighchartAnalyticsSelection");
            HighchartAnalyticsSelection res = null;
            EditView.CreateEditViewBase(new HighchartAnalyticsSelection(), (success, p) =>
            {
                if (success)
                {
                    res = (HighchartAnalyticsSelection)p;
                }
            }).ShowDialog();
            HighchartBusinessDataResult hr = service.Load(res);
            var bar = new HighchartBarBasic();
            return false;
        }
    }
}
