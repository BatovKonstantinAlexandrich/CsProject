
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.MediaModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.DataTable;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO;
using Console_CommonModel.CommonLib.LibModels.Console_InputModule.ViewModelsNew;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.BreadcrumbModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.ImageModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.LayoutModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.MultiLineModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.DbSetViewModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.FlexModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.InputModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.LineModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TabsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;
using Console_CommonModel.CommonLib.LibViews.DataGrid;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.CheckedListModule;





public interface IComponentsFactory<out TControl> 
{
    void Bind(ViewItem view, IControlBase control);
    void BindContextMenu(AppViewBase view, IControlBase host);
    TControl CreateAudioPanel(AudioView view);
    TControl CreateAudioRecorderPanel(AudioRecorderView view);
    TControl CreateBarChartControl(BarChartView view);
    TControl CreateBreadcrumb(BreadcrumbView view);
    TControl CreateButton(ButtonView view);
    TControl CreateButtonToggle(ToggleView view);
    TControl CreateCameraPanel(CameraView view);
    TControl CreateCameraStream(CameraStreamView view);
    TControl CreateCard(CardView view);
    TControl CreateChartControl(ChartView view);
    TControl CreateCheckboxView(CheckboxView view);
    TControl CreateCheckedList(CheckedListView view);
    TControl CreateColumnChartControl(ColumnChartView view);
    TControl CreateColumnsView(ColumnsView view);
    TControl CreateCombobox(ComboboxView view);
    TControl CreateContainer(ContainerView view);
    TControl CreateControl(string name, ViewItem view);
    TControl CreateControl(ViewItem view);
    TControl CreateControlRoot(AppViewRoot view);
    TControl CreateCoreChartControl(CoreChartView view);
    TControl CreateDataList(ListObjectsView view);
    TControl CreateDatasource(DatasourceView view);
    TControl CreateDataStream(DataStreamView view);
    TControl CreateDataTable(DataTableView view);
    TControl CreateDropDown(DropDown view);
    TControl CreateEntitySelectList(ViewItem view);
    TControl CreateEntityTable(EntityTable view);
    TControl CreateExpansion(ExpansionView view);
    TControl CreateField(InputFormField view);
    TControl CreateFilters(InputSearchFilters view);
    TControl CreateFlex(FlexView view);
    TControl CreateFocus(FocusModel view);
    TControl CreateFormField(InputFormField view);
    TControl CreateFormModel(InputFormModel view);
    TControl CreateFormView(FormViewModel view);
    TControl CreateGrid(GridView view);
    TControl CreateHighchartControl(Highchart view);
    TControl CreateHighchartControl(HighchartView view);
    TControl CreateImagePanel(ImageView view);
    TControl CreateImageSliderPanel(ImageSliderView view);
    TControl CreateInputColor(InputColorView view);
    TControl CreateInputCreditCard(InputCreditCardView view);
    TControl CreateInputDate(InputDateTimeView view);
    TControl CreateInputDecimal(InputDecimalView view);
    TControl CreateInputFilePath(InputFilePathView view);
    TControl CreateInputForm(InputView2 view);
    TControl CreateInputNatural(InputNaturalView view);
    TControl CreateInputNumber(InputNumberView view);
    TControl CreateInputPercent(InputPercentView view);
    TControl CreateInputSearch(SearchView view);
    TControl CreateInputTcpIpAddress(InputTcpIpAddressView view);
    TControl CreateLayout(AppLayoutView view);
    TControl CreateLine(LineView view);
    TControl CreateLineChartControl(LineChartView view);
    TControl CreateList(ListView view);
    TControl CreateListPanel(ListSelectionView view);
    TControl CreateListView(ListView view);
    TControl CreateMainMenu(MenuViewModel view);
    TControl CreateMaster(DialogView view);
    TControl CreateMaterialList(ListView view);
    TControl CreateMatrix(MatrixView view);
    TControl CreateMenuView(MenuView view);
    TControl CreateMultilineText(MultiLineTextView view);
    TControl CreatePagedView(BookView view);
    TControl CreatePagination(PaginationView view);
    TControl CreatePanel(ViewItem view);
    TControl CreatePanelsView(ViewItem view);
    TControl CreatePassword(InputPasswordView view);
    TControl CreateRecaptchaPanel(RecaptchaView view);
    TControl CreateRepository(RepositoryGridView view);
    TControl CreateScroll(ScrollView view);
    TControl CreateScrollPane(ScrollView view);
    TControl CreateSignalRVideoPanel(SignalRVideoView view);
    TControl CreateStatusView(StatusView view);
    TControl CreateSubmit(SubmitView view);
    TControl CreateTable(DataTableView view);
    TControl CreateTable(TableView view);
    TControl CreateTable(Type generic, dynamic view);
    TControl CreateTabs(TabsView view);
    TControl CreateText(TextView view);
    TControl CreateTextBox(TextBoxView view);
    TControl CreateToolsView(ToolsView view);
    TControl CreateTreePanel(TreeViewModel view);
    TControl CreateTreeView(TreeControlView view);
    TControl CreateType(TypeView view);
    TControl CreateTypedView(TypedView view);
    TControl CreateVideoPanel(VideoView view);
    TControl CreateViewPanel(AppViewBase view);
    TControl CreateViewPanel(PanelsView view); 
}

public interface IElementsFactory<out TControl>
{
   
    TControl CreateButton(ButtonControlView view);
    TControl CreateButton(ButtonView view);
    TControl CreateCard(CardView view);
    TControl CreateExpansion(ExpansionView view);
    //TControl CreateChart(ChartView view);
    //TControl CreateChartControl(ChartView view);
    //TControl CreateLineChart(LineChartView view);
    //TControl CreateLineChartControl(LineChartView view);
    //TControl CreateBarChart(BarChartView view);
    //TControl CreateBarChartControl(BarChartView view);
    //TControl CreateColumnChart(ColumnChartView view);
    //TControl CreateColumnChartControl(ColumnChartView view);
    //TControl CreateCoreChart(CoreChartView view);
    //TControl CreateCoreChartControl(CoreChartView view);
    TControl CreateHighchartControl(HighchartView view);
    TControl CreateHighchartControl(Highchart model);
    TControl CreateColumnsView(ColumnsView columnsView);

    TControl CreateDataList(ListObjectsView view);
    TControl CreateGrid(GridView view);
    TControl CreateImagePanel(ImageView view);
    TControl CreateAudioPanel(AudioView view);
    TControl CreateAudioRecorderPanel(AudioRecorderView view);
    TControl CreateVideoPanel(VideoView view);
    TControl CreateCameraPanel(CameraView view);
    TControl CreateSignalRVideoPanel(SignalRVideoView view);
    TControl CreateInputSearch(SearchView view);
    TControl CreateList(ListView view);
    TControl CreateListPanel(ListSelectionView list);
    TControl CreateMainMenu(MenuViewModel view);
    TControl CreateMaterialList(ListView view);
    TControl CreateMenuItem(ILink plink, Action<string> onclick);
    //NodeModel CreateNodeModel(ViewNode node);
    TControl CreatePagination(PaginationView view);
    TControl CreatePanel(ViewItem view);
    TControl CreateScrollPane(ScrollView view);
    //ContentPanel CreateSidePanel(ContentPanel view);
    TControl CreateTable(DataTableView view);
    TControl CreateTable(TableView view);
    TControl CreateTable(Type generic, dynamic view);
    TControl CreateTreePanel(TreeViewModel view);
    TControl CreateViewPanel(PanelsView view);
    TControl CreateViewPanel(AppViewBase view);



}