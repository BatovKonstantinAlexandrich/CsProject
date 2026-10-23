using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO;
using Console_CommonModel.CommonLib.LibModels.Console_InputModule.ViewModelsNew;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.BreadcrumbModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.ComboboxModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.ImageModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.LayoutModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.MultiLineModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.StatusModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.DbSetViewModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.FlexModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.InputModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.LineModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.MediaModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TabsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TypedModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.XmlModule;
using Console_CommonModel.CommonLib.LibViews.DataGrid;
using Console_CommonModel.CommonLib.LibViews.Others;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.CheckedListModule;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.DataTable;
using Console_CommonModel.CommonLib.LibViews.ViewModels;

using LiveCharts.Definitions.Charts;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Win_BusinessApp.CommonModel;

using WinFormsApp.Icons;

using WinFormsDesignerApp;

namespace Win_CommonModel.CommonModel
{
    public class WinComponentsFactory: IComponentsFactory<IControlBase>
    {
        /// <summary>
        /// Дублирует <see cref="ContextMenuStrip"/> на вложенные элементы, не являющиеся хостами другого представления
        /// (<see cref="ControlBase"/>). Иначе ПКМ приходится на дочерний control (например кнопку внутри панели),
        /// и меню модели не открывается.
        /// </summary>
        private static void PropagateContextMenuStripToHostedVisuals(Control parent, ContextMenuStrip menu)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is ControlBase)
                    continue;
                child.ContextMenuStrip = menu;
                PropagateContextMenuStripToHostedVisuals(child, menu);
            }
        }

        public ContextMenuStrip CreateContextMenuStrip(ContextView view)
        {
            var menu = new ContextMenuStrip();
            foreach (var childItem in view.Items)
            {
                var item = CreateContextMenu(childItem);
                menu.Items.Add(item);
            }
            view.Items.OnAdded((i, p) => { menu.Items.Add(CreateContextMenu((ContextView)p)); });
            view.Items.OnRemoved((i, p) => { menu.Items.RemoveAt(i); });
            return menu;
        }
        public ToolStripMenuItem CreateContextMenu(ContextView view)
        {
            switch (view.GetTypeName())
            {
                case nameof(ContextViewCheckbox):
                    /*var checkbox = new ToolCheckbox(check => {
                        ((ContextViewCheckbox)view).Checked = check;
                    });
                    checkbox.Text = view.Label;
                    checkbox.Checked = ((ContextViewCheckbox)view).Checked;
                     */
                    var checkbox = new ToolStripMenuItem();
                    if (!String.IsNullOrWhiteSpace(view.Icon))
                        checkbox.Image = Image.FromFile(new IconsManager().GetImageByName(view.Icon));
                    view.AddPropertyChangeListener("Icon", evt =>
                    {
                        if (!string.IsNullOrWhiteSpace(view.Icon))
                        {
                            try
                            {
                                checkbox.Image = Image.FromFile(new IconsManager().GetImageByName(view.Icon));
                            }
                            catch
                            {
                                checkbox.Image = null;
                            }
                        }
                        else
                        {
                            checkbox.Image = null;
                        }
                    });
                    checkbox.Text = view.Label;
                    checkbox.Checked = ((ContextViewCheckbox)view).IsChecked;
                    checkbox.CheckOnClick = true;
                    checkbox.CheckedChanged += (s, val) => {
                        ((ContextViewCheckbox)view).IsChecked = checkbox.Checked;
                    };
                    view.AddPropertyChangeListener(nameof(ContextViewCheckbox.IsChecked), evt => {
                        if (checkbox.Checked != (bool)evt.After)
                            checkbox.Checked = (bool)evt.After;
                    });
                    return checkbox;
                case nameof(ContextViewRadioGroup):
                case nameof(ContextViewTextbox):
                default:
                    var menu = new ToolStripMenuItem();

                    if (view.Items.Count() > 0)
                    {
                        menu.Text = view.Label;
                        foreach (var childItem in view.Items)
                        {
                            var item = CreateContextMenu(childItem);
                            menu.DropDownItems.Add(item);
                        };
                    }
                    else
                    {
                        menu.Text = view.Label;

                    }
                    menu.Click += (s, e) => {
                        if (view.OnClick is not null)
                            view.OnClick(s, e);
                    };

                    view.Items.OnAdded((i, p) => {
                        menu.DropDownItems.Add(CreateContextMenu((ContextView)p));
                    });
                    view.Items.OnRemoved((i, p) => { menu.DropDownItems.RemoveAt(i); });
                    return menu;
                    break;
            }
        }
        
        public void BindContextMenu(AppViewBase view, IControlBase control)
        {
            if (control.Cast<ControlBase>().ContextMenuStrip is null)
                control.Cast<ControlBase>().ContextMenuStrip = new();
            control.Cast<ControlBase>().ContextMenuStrip = CreateContextMenuStrip(view.ContextMenu);
            PropagateContextMenuStripToHostedVisuals(control.Cast<ControlBase>(), control.Cast<ControlBase>().ContextMenuStrip);
            /*control.ContextMenuStrip = CreateContextMenu(view.ContextMenu, command =>
            {
                try
                {
                    if (view.ContextMenu is ContextViewButton && ((ContextViewButton)view.ContextMenu).OnClick is not null)
                    {
                        ((ContextViewButton)view.ContextMenu).OnClick(view.ContextMenu);
                    }
                    if (view.ContextMenu is ContextViewCheckbox && ((ContextViewCheckbox)view.ContextMenu).OnCheckedChanged is not null)
                    {
                        ((ContextViewCheckbox)view.ContextMenu).OnCheckedChanged(((ContextViewCheckbox)view.ContextMenu).IsChecked);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"{this.GetTypeName()}.{MethodBase.GetCurrentMethod().Name} => {ex.Message}");
                }
            });*/
        }

        private ControlsFactory controlsFactory = new();
         
        public IControlBase CreateControl(string name, ViewItem view)
        {
            IControlBase control = null;
            switch (name)
            {
                case nameof(DataStreamView): control = CreateDataStream((DataStreamView)view); break;
                case nameof(LineView): control = CreateLine((LineView)view); break;
                //case nameof(InputPrimitiveCollectionView): control = this.CreatePrimitiveCollection((InputPrimitiveCollectionView)view); break;
                case nameof(InputDateTimeView): control = CreateInputDate((InputDateTimeView)view); break;
                case nameof(AppLayoutView): control = CreateLayout((AppLayoutView)view); break;
                case nameof(CameraStreamView): control = CreateCameraStream((CameraStreamView)view); break;
                case nameof(InputSearchFilters): control = CreateFilters((InputSearchFilters)view); break;
                case nameof(SubmitView): control = CreateSubmit((SubmitView)view); break;
                case nameof(CheckedListView): control = CreateCheckedList((CheckedListView)view); break;
                case nameof(RepositoryGridView): control = CreateRepository((RepositoryGridView)view); break;
                case nameof(InputFormField): control = CreateFormField((InputFormField)view); break;
                case nameof(InputFormModel): control = CreateFormModel((InputFormModel)view); break;
                case nameof(ButtonView): control = CreateButton((ButtonView)view); break;
                case nameof(ComboboxView): control = CreateCombobox((ComboboxView)view); break;
                case nameof(TypeView): control = CreateType((TypeView)view); break;
                case nameof(TreeControlView): control = CreateTreeView((TreeControlView)view); break;
                case nameof(TextView): control = CreateText((TextView)view); break;
                case nameof(HighchartView): control = CreateHighchartControl((HighchartView)view); break;
                case nameof(InputPercentView): control = CreateInputPercent((InputPercentView)view); break;
                case nameof(InputNumberView): control = CreateInputNumber((InputNumberView)view); break;
                case nameof(InputDecimalView): control = CreateInputDecimal((InputDecimalView)view); break;
                case nameof(InputNaturalView): control = CreateInputNatural((InputNaturalView)view); break;
                case nameof(InputPasswordView): control = CreatePassword((InputPasswordView)view); break;
                case nameof(TypedView): control = CreateTypedView((TypedView)view); break;
                //case nameof(NavigationView): control = CreateNavigationView((NavigationView)view); break;
                case nameof(StatusView): control = CreateStatusView((StatusView)view); break;
                case nameof(ToolsView): control = CreateToolsView((ToolsView)view); break;
                case nameof(MenuView): control = CreateMenuView((MenuView)view); break;
                case nameof(BreadcrumbView): control = CreateBreadcrumb((BreadcrumbView)view); break;
                case nameof(AppViewRoot): return CreateViewRoot(view, control); break;
                case nameof(AppViewBase):
                case nameof(PanelsView): control = CreatePanelsView(view); return control; break;
                case nameof(ViewItem): control = CreatePanel(view); break;
                case nameof(ColumnsView): control = CreateColumnsView((ColumnsView)view); Bind(view, control); break;
                case nameof(EntitySelectList): control = CreateEntitySelectList(view); break;
                case nameof(ImageView): control = CreateImagePanel((ImageView)view); break;
                case nameof(ImageSliderView): control = CreateImageSliderPanel((ImageSliderView)view); break;
                case nameof(AudioView): control = CreateAudioPanel((AudioView)view); break;
                case nameof(AudioRecorderView): control = CreateAudioRecorderPanel((AudioRecorderView)view); break;
                case nameof(VideoView): control = CreateVideoPanel((VideoView)view); break;
                case nameof(CameraView): control = CreateCameraPanel((CameraView)view); break;
                case nameof(SignalRVideoView): control = CreateSignalRVideoPanel((SignalRVideoView)view); break;
                case nameof(RecaptchaView): control = CreateRecaptchaPanel((RecaptchaView)view); break;
                case nameof(XmlView):
                case nameof(SearchView): control = CreateInputSearch((SearchView)view); break;
                case nameof(ListSelectionView): control = CreateListPanel((ListSelectionView)view); break;
                case nameof(SearchProductView): control = CreatePanel(view); Bind(view, control); break;
                //case nameof(FieldView): control = CreateField((FieldView)view); break;
                case nameof(PaginationView): control = CreatePagination((PaginationView)view); break;
                case nameof(RepetableView): control = CreatePanel(view); Bind(view, control); break;
                case nameof(PaneItem): control = CreatePanel(view); break;
                case nameof(AppView): control = CreatePanel(view); break;
                case nameof(ReturnOrderView): control = CreatePanel(view); Bind(view, control); break;
                //case nameof(TextEditorView): control = CreateTextEditor((TextEditorView)view); break;
                case nameof(ViewComponentStyle): control = CreatePanel(view); Bind(view, control); break;
                case nameof(StructuredCollectionView): control = CreatePanel(view); Bind(view, control); break;
                //case nameof(SearchDatabase): control = CreateSearchDatabase((SearchDatabase)view); break;
                case nameof(SearchByCtrl): control = CreatePanel(view); Bind(view, control); break;
                case nameof(ScrollView): control = CreateScroll((ScrollView)view); break;
                //case nameof(ScrollableView): control = CreateScrollPane((ScrollView)view); break;
                case nameof(MyList): control = CreatePanel(view); Bind(view, control); break;
                case nameof(MenuViewModel): control = CreateMainMenu((MenuViewModel)view); break;
                case nameof(ListItemView): control = CreatePanel(view); Bind(view, control); break;
                case nameof(LinkViewModel): control = CreatePanel(view); Bind(view, control); break;
                case nameof(DialogView): control = CreateMaster((DialogView)view); break;
                case nameof(TextBoxView): control = CreateTextBox((TextBoxView)view); Bind(view, control); break;
                case nameof(MultiLineTextView): control = CreateMultilineText((MultiLineTextView)view); Bind(view, control); break;
                case nameof(InputColorView): control = CreateInputColor((InputColorView)view); Bind(view, control); break;
                case nameof(InputFilePathView): control = CreateInputFilePath((InputFilePathView)view); Bind(view, control); break;
                case nameof(InputTcpIpAddressView): control = CreateInputTcpIpAddress((InputTcpIpAddressView)view); Bind(view, control); break;
                case nameof(InputCreditCardView): control = CreateInputCreditCard((InputCreditCardView)view); Bind(view, control); break;
                case nameof(MatrixView): control = CreateMatrix((MatrixView)view); Bind(view, control); break;
                //case nameof(MasterPage): control = CreateMaster((MasterPage)view); break;
                case nameof(FocusModel): control = CreateFocus((FocusModel)view); Bind(view, control); break;
                //case nameof(FormField): control = CreateFormField((FormField)view); Bind(view, control); break;
                //case nameof(FormView): control = CreateFormView((FormView)view); Bind(view, control); break;
                case nameof(FormViewModel): control = CreateFormView((FormViewModel)view); Bind(view, control); break;
                case nameof(GridView): control = CreateGrid((GridView)view); break;
                //case nameof(ViewItemSet): control = CreatePanel(view); Bind(view, control); break;
                //case nameof(ViewItem): control = new Panel(); ((Panel)control).SetVerticalScroll(); break;
                case nameof(FlexContainer): control = CreatePanel(view); Bind(view, control); break;
                case nameof(BookView): control = CreatePagedView((BookView)view); break;
                case nameof(CardView): control = CreateCard((CardView)view); break;
                case nameof(ExpansionView): control = CreateExpansion((ExpansionView)view); break;
                case nameof(FlexView): control = CreateFlex((FlexView)view); break;
                case nameof(ChartView): control = CreateChartControl((ChartView)view); break;
                case nameof(LineChartView): control = CreateLineChartControl((LineChartView)view); break;
                case nameof(BarChartView): control = CreateBarChartControl((BarChartView)view); break;
                case nameof(ColumnChartView): control = CreateColumnChartControl((ColumnChartView)view); break;
                case nameof(CoreChartView): control = CreateCoreChartControl((CoreChartView)view); break;
                case nameof(InputView2): control = CreateInputForm((InputView2)view); break;
                case nameof(ContainerView): control = CreateContainer((ContainerView)view); break;
                case nameof(DatasourceView): control = CreateDatasource((DatasourceView)view); break;
                case nameof(DropDown): control = CreateDropDown((DropDown)view); break;
                case nameof(DropDownMenu): control = CreateDropDown((DropDown)view); break;
                case nameof(TreeViewModel): control = CreateTreePanel((TreeViewModel)view); break;
                case nameof(DataTableView): control = CreateDataTable((DataTableView)view); break;
                case nameof(TableView): control = CreateTable((TableView)view); break;
                case nameof(EntityTable): control = CreateEntityTable((EntityTable)view); break;
                case nameof(CheckboxView): control = CreateCheckboxView((CheckboxView)view); break;
                case nameof(ToggleView): control = CreateButtonToggle((ToggleView)view); break;
                case nameof(TabsView): control = CreateTabs((TabsView)view); break;
                //..case nameof(PageItem): control = CreatePanel(view); break;
                case nameof(ListObjectsView): control = CreateDataList((ListObjectsView)view); break;
                case nameof(ListView): control = CreateListView((ListView)view); break;
                default: return CreateViewPanel((PanelsView)view);
            }
            return control;
        }

        private IControlBase CreateViewRoot(ViewItem view, IControlBase? control)
        {
            throw new NotImplementedException();
        }

        private IControlBase? CreateRepository(RepositoryGridView view)
        {
            throw new NotImplementedException();
        }

        private IControlBase? CreateCheckedList(CheckedListView view)
        {
            throw new NotImplementedException();
        }

        private IControlBase? CreateSubmit(SubmitView view)
        {
            throw new NotImplementedException();
        }

        private IControlBase? CreateFilters(InputSearchFilters view)
        {
            throw new NotImplementedException();
        }

        private IControlBase? CreateCameraStream(CameraStreamView view)
        {
            throw new NotImplementedException();
        }

        private IControlBase? CreateInputDate(InputDateTimeView view)
        {
            throw new NotImplementedException();
        }

        private IControlBase? CreateLayout(AppLayoutView view)
        {
            throw new NotImplementedException();
        }

        private IControlBase? CreateLine(LineView view)
        {
            throw new NotImplementedException();
        }

        private IControlBase? CreateDataStream(DataStreamView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateControl(ViewItem view)
        {
            return this.CreateControl(view.GetTypeName(), view);
        }

        public IControlBase CreateControlRoot(AppViewRoot view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateAudioPanel(AudioView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateAudioRecorderPanel(AudioRecorderView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateButton(ButtonView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateCameraPanel(CameraView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateCard(CardView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateColumnsView(ColumnsView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateDataList(ListObjectsView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateExpansion(ExpansionView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateGrid(GridView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateHighchartControl(Highchart view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateHighchartControl(HighchartView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateImagePanel(ImageView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateInputSearch(SearchView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateList(ListView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateListPanel(ListSelectionView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateMainMenu(MenuViewModel view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateMaterialList(ListView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreatePagination(PaginationView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreatePanel(ViewItem view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateScrollPane(ScrollView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateSignalRVideoPanel(SignalRVideoView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateTable(DataTableView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateTable(TableView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateTable(Type generic, dynamic view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateTreePanel(TreeViewModel view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateVideoPanel(VideoView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateViewPanel(AppViewBase view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateViewPanel(PanelsView view)
        {
            throw new NotImplementedException();
        }

        public void Bind(ViewItem view, IControlBase control)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateBarChartControl(BarChartView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateBreadcrumb(BreadcrumbView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateButtonToggle(ToggleView view)
        {
            throw new NotImplementedException();
        }

        IControlBase IComponentsFactory<IControlBase>.CreateCameraStream(CameraStreamView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateChartControl(ChartView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateCheckboxView(CheckboxView view)
        {
            throw new NotImplementedException();
        }

        IControlBase IComponentsFactory<IControlBase>.CreateCheckedList(CheckedListView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateColumnChartControl(ColumnChartView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateCombobox(ComboboxView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateContainer(ContainerView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateCoreChartControl(CoreChartView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateDatasource(DatasourceView view)
        {
            throw new NotImplementedException();
        }

        IControlBase IComponentsFactory<IControlBase>.CreateDataStream(DataStreamView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateDataTable(DataTableView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateDropDown(DropDown view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateEntitySelectList(ViewItem view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateEntityTable(EntityTable view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateField(InputFormField view)
        {
            throw new NotImplementedException();
        }

        IControlBase IComponentsFactory<IControlBase>.CreateFilters(InputSearchFilters view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateFlex(FlexView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateFocus(FocusModel view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateFormField(InputFormField view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateFormModel(InputFormModel view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateFormView(FormViewModel view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateImageSliderPanel(ImageSliderView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateInputColor(InputColorView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateInputCreditCard(InputCreditCardView view)
        {
            throw new NotImplementedException();
        }

        IControlBase IComponentsFactory<IControlBase>.CreateInputDate(InputDateTimeView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateInputDecimal(InputDecimalView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateInputFilePath(InputFilePathView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateInputForm(InputView2 view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateInputNatural(InputNaturalView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateInputNumber(InputNumberView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateInputPercent(InputPercentView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateInputTcpIpAddress(InputTcpIpAddressView view)
        {
            throw new NotImplementedException();
        }

        IControlBase IComponentsFactory<IControlBase>.CreateLayout(AppLayoutView view)
        {
            throw new NotImplementedException();
        }

        IControlBase IComponentsFactory<IControlBase>.CreateLine(LineView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateLineChartControl(LineChartView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateListView(ListView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateMaster(DialogView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateMatrix(MatrixView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateMenuView(MenuView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateMultilineText(MultiLineTextView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreatePagedView(BookView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreatePanelsView(ViewItem view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreatePassword(InputPasswordView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateRecaptchaPanel(RecaptchaView view)
        {
            throw new NotImplementedException();
        }

        IControlBase IComponentsFactory<IControlBase>.CreateRepository(RepositoryGridView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateScroll(ScrollView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateStatusView(StatusView view)
        {
            throw new NotImplementedException();
        }

        IControlBase IComponentsFactory<IControlBase>.CreateSubmit(SubmitView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateTabs(TabsView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateText(TextView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateTextBox(TextBoxView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateToolsView(ToolsView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateTreeView(TreeControlView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateType(TypeView view)
        {
            throw new NotImplementedException();
        }

        public IControlBase CreateTypedView(TypedView view)
        {
            var ctrl = new TypedControlDefault();
            var comp = new TypedComponent(view, ctrl);

            return ctrl;
        }
    }
}
