using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViewModules.HighchartsModule;
using WinFormsApp.AppViewModules.MenuModule;
using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.DesignerServices
{

    /// <summary>
    /// Выполняет сборку компонента
    /// </summary>
    public class DesignerFactory
    {

        /// <summary>
        /// Подключение управления к модели представления
        /// </summary>
        public List<object> CreateServices(AppViewBase view)
        {
            ComponentBase componentBase = new ComponentBase();
            var contentBox = new Panel();

            var types = view.GetType().GetExtendedTypes();
            types.Remove(typeof(object));
            types.Add(view.GetType());
            types.Reverse();

            List<object> services = new();
            foreach (var ptype in types)
            {
                this.InfoTime($"Тип {ptype.GetTypeName()}", () => {
                    this.Info(ptype.GetTypeName());
                    switch (ptype.GetTypeName())
                    {
                        case nameof(MyValidatableObject):
                            {
                                var ctrl = new MyValidatableObjectDesignerService();
                                ctrl.SetControl(view);
                                services.Add(ctrl);
                                break;
                            }
                        case nameof(ChangeSupport):
                            {
                                var ctrl = new EventSupportControl();
                                ctrl.SetControl(view);
                                services.Add(ctrl);
                                break;
                            }

                        case nameof(PanelsView):
                            {
                                var ctrl = new PanelsViewDesignerService();
                                ctrl.SetControl(view);
                                services.Add(ctrl);
                                break;
                            }
                        case nameof(ViewOptions):
                            {
                                var ctrl = new ViewOptionsDesignerService();
                                ctrl.SetControl(view);
                                services.Add(ctrl);
                                break;
                            }
                        case nameof(StyledItem):
                            {
                                var ctrl = new StyledItemDesignerService();
                                ctrl.SetControl(view);
                                services.Add(ctrl);
                                break;
                            }
                        case nameof(ViewItem):
                            {
                                var ctrl = new ViewItemDesignerService();
                                ctrl.SetControl(view);
                                services.Add(ctrl);
                                break;
                            }
                        case nameof(ViewDataBuilder):
                            {
                                var ctrl = new ViewDataBuilderDesignerService();
                                ctrl.SetControl(view);
                                services.Add(ctrl);
                                break;
                            }
                        case nameof(ViewFactory):
                            {
                                var ctrl = new ViewFactoryDesignerService();
                                ctrl.SetControl(view);
                                services.Add(ctrl);
                                break;
                            }
                        case nameof(InputFormModel):
                            {
                                var ctrl = new InputFormModelDesignerService();
                                ctrl.SetControl((InputFormModel)view);
                                services.Add(ctrl);
                                break;
                            }
                        case nameof(TreeControlView):
                            {
                                var ctrl = new TreeControlViewDesignerService();
                                ctrl.SetControl((TreeControlView)view);
                                services.Add(ctrl);
                                break;
                            }

                        case nameof(ChartView):
                        case nameof(LineChartView):
                        case nameof(BarChartView):
                        case nameof(ColumnChartView):
                        case nameof(CoreChartView):
                            {
                                var ctrl = new ChartViewDesignerService();
                                ctrl.SetControl((ChartSeriesViewBase)view);
                                services.Add(ctrl);
                                break;
                            }
                        case nameof(MenuView):
                            {
                                var ctrl = new MenuViewDesignerService();
                                ctrl.SetControl((MenuView)view);
                                services.Add(ctrl);
                                break;
                            }
                        case nameof(HighchartView):
                            {
                                var ctrl = new HighchartViewDesignerService();
                                ctrl.SetControl((HighchartView)view);
                                services.Add(ctrl);
                                break;
                            }
                    }
                });
            }
            return services;
        }

    }
}
