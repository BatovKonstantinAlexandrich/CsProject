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
using Blazor_CommonModel.Components.Shared;
using Console_CommonModel.CommonLib.LibViews.ViewModels;


public sealed class WebComponentsFactory : IComponentsFactory<WebComponentBase> 
{




    public WebComponentBase CreateControl(string name, ViewItem view)
    {
        WebComponentBase control = null;
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

    public WebComponentBase CreateFormView(FormViewModel view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateInputForm(InputView2 view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateText(TextView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateInputDate(InputDateTimeView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public void Bind(ViewItem view, IControlBase control)
    {
    }

    public WebComponentBase CreateFocus(FocusModel view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateMatrix(MatrixView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateListView(ListView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateTreeView(TreeControlView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateRecaptchaPanel(RecaptchaView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateScroll(ScrollView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateFlex(FlexView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateChartControl(ChartView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateEntityTable(EntityTable view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateTabs(TabsView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateButtonToggle(ToggleView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateCheckboxView(CheckboxView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateDataTable(DataTableView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateDropDown(DropDown view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateDatasource(DatasourceView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateContainer(ContainerView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateCoreChartControl(CoreChartView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateColumnChartControl(ColumnChartView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateLineChartControl(LineChartView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateBarChartControl(BarChartView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreatePagedView(BookView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateInputCreditCard(InputCreditCardView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateInputTcpIpAddress(InputTcpIpAddressView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateInputFilePath(InputFilePathView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateInputColor(InputColorView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateMultilineText(MultiLineTextView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateTextBox(TextBoxView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateMaster(DialogView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateField(InputFormField view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateImageSliderPanel(ImageSliderView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateEntitySelectList(ViewItem view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = (AppViewBase)view; return result;
    }

    public WebComponentBase CreatePanelsView(ViewItem view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = (AppViewBase)view; return result;
    }

    public WebComponentBase CreateViewRoot(ViewItem view, WebComponentBase control)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = (AppViewBase)view; return result;
    }

    public WebComponentBase CreateBreadcrumb(BreadcrumbView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateMenuView(MenuView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateToolsView(ToolsView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateStatusView(StatusView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateTypedView(TypedView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreatePassword(InputPasswordView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateInputNatural(InputNaturalView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateInputDecimal(InputDecimalView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateInputNumber(InputNumberView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateInputPercent(InputPercentView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateType(TypeView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateCombobox(ComboboxView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateFormModel(InputFormModel view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateFormField(InputFormField view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateRepository(RepositoryGridView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateCheckedList(CheckedListView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateSubmit(SubmitView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateFilters(InputSearchFilters view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateCameraStream(CameraStreamView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateLayout(AppLayoutView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateLine(LineView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateDataStream(DataStreamView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }




    public WebComponentBase CreateControl(ViewItem view)
        => CreateControl(view.GetTypeName(), view);




    public WebComponentBase CreateControlRoot(AppViewRoot view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }





    public WebComponentBase CreateButton(ButtonView view)
    {
        var web = new WebComponentBase();
        web.ViewModel = view;
        return web;
    }



    public WebComponentBase CreateCard(CardView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateExpansion(ExpansionView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateHighchartControl(HighchartView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateHighchartControl(Highchart view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateColumnsView(ColumnsView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateDataList(ListObjectsView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateGrid(GridView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateImagePanel(ImageView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateAudioPanel(AudioView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateAudioRecorderPanel(AudioRecorderView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateVideoPanel(VideoView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateCameraPanel(CameraView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateSignalRVideoPanel(SignalRVideoView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateInputSearch(SearchView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateList(ListView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateListPanel(ListSelectionView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateMainMenu(MenuViewModel view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateMaterialList(ListView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }



    public WebComponentBase CreatePagination(PaginationView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreatePanel(ViewItem view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = (AppViewBase)view; return result;
    }

    public WebComponentBase CreateScrollPane(ScrollView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateTable(DataTableView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateTable(TableView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateTable(Type generic, dynamic view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = (AppViewBase)view; return result;
    }

    public WebComponentBase CreateTreePanel(TreeViewModel view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public WebComponentBase CreateViewPanel(PanelsView view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = (AppViewBase)view; return result;
    }

    public WebComponentBase CreateViewPanel(AppViewBase view)
    {
        var result = "ContainerControl".New<WebComponentBase>(); result.ViewModel = view; return result;
    }

    public void BindContextMenu(AppViewBase view, IControlBase host)
    {
        throw new NotImplementedException();
    }
}
