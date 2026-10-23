using pickpoint_delivery_service;
using Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule.BusinessAnaliticsServices;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.DataTable;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;


namespace Console_CommonModel.CommonLib.LibDesigner.DesignerServices
{
    /// <summary>
    /// Управление формированием данных для представления
    /// </summary>
    [Label("Управление данными представления")]
    public class ViewDataBuilderDesignerService : DesignerBase,
        IControlService<ViewDataBuilder>
    {
        public ViewDataBuilder Control { get; set; }

        public ViewDataBuilderDesignerService() { }
        public ViewDataBuilderDesignerService(ViewDataBuilder view)
        {
            Control = view;
        }



        public void SetControl(ViewDataBuilder item)
        {
            Control = item;
            if (Control.DesignMode)
            {
                Activating(Control);
            }
        }

        protected override void Activating(ViewNode node)
        {
            if (node is AppViewBase vb)
            {
                var view = (AppViewBase)node;
                //AddSettingsContextMenuItem(view);

                //AddSelectDatasourceContext(vb);
                //AddSelectDatasetContext(vb);
                //AddEditBindingsContext(vb);
                //AddEnableProceduresContext(vb);
            }
        }

        private void AddSettingsContextMenuItem(AppViewBase view)
        {
            var ctx = new ContextView();
            ctx.Label = "Настройки";
            ctx.OnClick = (s, e) =>
            {

                var tempTop = view.Top;
                view.Top = view.CreateTools(new Dictionary<string, Action>() {
                        { "назад", ()=>{ view.GoBack(); } }
                    });
                var tempCtrls = view.Controls.ToList();
                BusinessDatasource ds = null;
                view.CreateSelectEntityList<BusinessDatasource, DeliveryDbContext>(
                    "Выберите источник данных", p =>
                    {
                        ds = p;

                    }).ToSubmit((success, p) =>
                    {
                        if (success)
                        {

                            var datasets = new DeliveryDbContext().BusinessDatasets.Where(d => d.BusinessDatasourceId == ds.Id).ToList();
                            AppViewBase list = null;
                            list = view.CreateSelectEntityList<BusinessDataset, DeliveryDbContext>(
                                "Выберите набор данных",
                                datasets,
                                p =>
                                {
                                    BusinessDataset bd = p;
                                    var service = new BusinessDatasourceService();
                                    service.Setup(ds);

                                    try
                                    {
                                        var dataTable = service.ExecuteQuery(bd.Expression);
                                        var dtv = new DataTableView();
                                        dtv.Data = dataTable;
                                        list.LayoutFill(dtv.ToSubmit((success, p) =>
                                        {
                                            if (success)
                                            {
                                                view.DatasourceId = p.Id;
                                                view.DatasetId = bd.Id;


                                                view.Controls.Clear();
                                                view.Controls.AddRange(tempCtrls);

                                                view.Top = tempTop;
                                            }
                                            else
                                            {
                                                view.GoBack();
                                            }
                                        }));
                                    }
                                    catch (Exception ex)
                                    {
                                        view.PutError("Выполнение SQL", $"Ошибка: {ex.Message}");
                                    }
                                });
                            list.ShowDialog();
                        }

                        //p.Cast<AppViewBase>().LayoutFill(list);
                        //p.Cast<AppViewBase>().ToControl().GetAppForm().Close();
                    }).ShowDialog();
            };
            view.ContextMenu.Items.Add(ctx);
            rollback.Add(() =>
            {
                view.ContextMenu.Items.Remove(ctx);
            });
        }

        private void AddEnableProceduresContext(AppViewBase vb)
        {
        }

        private void AddEditBindingsContext(AppViewBase vb)
        {
        }

        private void AddSelectDatasetContext(AppViewBase vb)
        {
            rollback.Add(vb.AddContextSelectOption(
                typeof(BusinessDataset).GetLabel(),
                vb.Dataset?.Name,
                new(vb.GetDatasetNames()),
                datasetName =>
                {
                    SetDatasetByName(vb, datasetName);
                }));
        }


        private void AddSelectDatasourceContext(AppViewBase vb)
        {
            rollback.Add(vb.AddContextSelectOption(
                typeof(BusinessDatasource).GetLabel(),
                vb.Datasource?.Name,
                new(vb.GetDatasourceNames()),
                datasourceName =>
                {
                    SetDatasourceByName(vb, datasourceName);
                }));
        }


        [Label("Задать источник данных")]
        public void SetDatasourceByName(AppViewBase vb, string datasourceName)
        {
            using (var db = new DeliveryDbContext())
            {
                var datasource = db.BusinessDatasources.FirstOrDefault(ds => ds.Name == datasourceName);
                if (datasource is not null)
                {
                    vb.DatasetId = datasource.Id;
                }
            }
        }

        public void SetDatasetByName(AppViewBase vb, string datasetName)
        {
            using (var db = new DeliveryDbContext())
            {
                var datasource = db.BusinessDatasets.FirstOrDefault(ds => ds.Name == datasetName);
                if (datasource is not null)
                {
                    vb.DatasetId = datasource.Id;
                }
            }
        }

        public override void OnDatasetChanged(PropertyChangedMessage message)
        {

        }

        public override void OnResultSetChanged(PropertyChangedMessage message)
        {
        }

        public override void OnDatasourceChanged(PropertyChangedMessage message)
        {
        }

        public override void OnDatasourceIdChanged(PropertyChangedMessage message)
        {
        }
    }
}