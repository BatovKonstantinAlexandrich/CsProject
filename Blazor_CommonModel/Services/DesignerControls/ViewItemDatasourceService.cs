using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.AppViews.ApiServices;
using WinFormsApp.Console.Console_DataModule.DataADO;

namespace WinFormsApp.DesignerControls
{

    [Label("Сервис выполняет настройку источника данных управляемых представлений")]
    public class ViewItemDatasourceService : DesignerBase
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        /// <summary>Контекст источника данных дизайнера (зарезервирован для расширения привязки).</summary>
        private readonly DatasourceView _datasourceView;

        public ViewItemDatasourceService() : this(new DatasourceView()) { }

        public ViewItemDatasourceService(DatasourceView datasourceView)
        {
            _datasourceView = datasourceView;
        }

        protected override void Activating(ViewNode view)
        {
            GC.KeepAlive(_datasourceView);
            DatasourceId = ((ViewItem)view).DatasourceId;
            ((ViewItem)view).AddPropertyChangeListener(nameof(DatasourceId), evt => DatasourceId = evt.After.ToString().ToInt());
            rollback.Add(() =>
            {
                DatasourceId = 0;
            });
        }




    }

}