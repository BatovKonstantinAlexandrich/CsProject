using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.XmlModule;
using Console_CommonModel.CommonLib.LibViews.AppViews;
using Console_CommonModel.CommonLib.LibViews.AppViews.MediaModule;
using Console_CommonModel.CommonLib.LibPages.AuthViews;
using Console_CommonModel.CommonLib.LibViews.AppViews.TypedModule;
using Console_CommonModel.CommonLib.LibViews;
using Console_CommonModel.CommonLib.LibViews.Others;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TabsModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.StatusModule;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbModelService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectorService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO;
using Console_CommonModel.CommonLib.LibViews.AppViews.SplitModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule;
using Console_CommonModel.CommonLib.LibViews.DataGrid;
using Console_CommonModel.CommonLib.LibModels.Console_XmlModule;
using Console_CommonModel.CommonLib.LibPages;
using Console_CommonModel.CommonLib.LibViews.AppViews.PrimitiveCollectionModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.NavModule;
using Console_CommonModel.CommonLib.LibViews.NavModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.MultiLineModule;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageService;
using Console_CommonModel.CommonLib.LibViews.AppViews.MessageModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.CollectionModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;
using Console_CommonModel.CommonLib.LibViews.ViewCustomizers;
using Console_CommonModel.CommonLib.LibViews.AppViews.LineModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.LayoutModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.InputModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.FormModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.DbSetViewModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.ViewControl;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.ImageModule;
using Console_CommonModel.CommonLib.LibViews.ViewModels.HighCharts;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule;
using WinFormsApp;
using Console_CommonModel.CommonLib.LibViews.AppViews.FlexModule;
using Console_CommonModel.CommonLib.LibModels.Console_InputModule.ViewModelsNew;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.DllModule;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.DataTable;
using Console_CommonModel.CommonLib.LibViews.LayoutVIews;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.ComboboxModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.CheckedListModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.BreadcrumbModule;
using Console_CommonModel.CommonLib.LibViews.ViewModels;

public sealed class WebComponentsFactory : IComponentsFactory<IControlBase> 
{
    public void Bind(ViewItem view, IControlBase control)
    {
        throw new NotImplementedException();
    }

    public void BindContextMenu(AppViewBase view, IControlBase host)
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

    public IControlBase CreateBarChartControl(BarChartView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateBreadcrumb(BreadcrumbView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateButton(ButtonView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateButtonToggle(ToggleView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateCameraPanel(CameraView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateCameraStream(CameraStreamView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateCard(CardView view)
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

    public IControlBase CreateCheckedList(CheckedListView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateColumnChartControl(ColumnChartView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateColumnsView(ColumnsView view)
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

    public IControlBase CreateControl(string name, ViewItem view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateControl(ViewItem view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateControlRoot(AppViewRoot view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateCoreChartControl(CoreChartView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateDataList(ListObjectsView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateDatasource(DatasourceView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateDataStream(DataStreamView view)
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

    public IControlBase CreateExpansion(ExpansionView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateField(InputFormField view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateFilters(InputSearchFilters view)
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

    public IControlBase CreateInputDate(InputDateTimeView view)
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

    public IControlBase CreateInputSearch(SearchView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateInputTcpIpAddress(InputTcpIpAddressView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateLayout(AppLayoutView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateLine(LineView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateLineChartControl(LineChartView view)
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

    public IControlBase CreateListView(ListView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateMainMenu(MenuViewModel view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateMaster(DialogView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateMaterialList(ListView view)
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

    public IControlBase CreatePagination(PaginationView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreatePanel(ViewItem view)
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

    public IControlBase CreateRepository(RepositoryGridView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateScroll(ScrollView view)
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

    public IControlBase CreateStatusView(StatusView view)
    {
        throw new NotImplementedException();
    }

    public IControlBase CreateSubmit(SubmitView view)
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

    public IControlBase CreateTreePanel(TreeViewModel view)
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
}
