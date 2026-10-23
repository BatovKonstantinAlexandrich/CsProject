
public interface IWebComponentsFactory
{
    void BindContextMenu(AppViewBase view, IControlBase host);
    IControlBase CreateAudioPanel(AudioView view);
    IControlBase CreateAudioRecorderPanel(AudioRecorderView view);
    IControlBase CreateButton(ButtonView view);
    IControlBase CreateCameraPanel(CameraView view);
    IControlBase CreateCard(CardView view);
    IControlBase CreateColumnsView(ColumnsView view);
    IControlBase CreateControl(string name, ViewItem view);
    IControlBase CreateControl(ViewItem view);
    IControlBase CreateControlRoot(AppViewRoot view);
    IControlBase CreateDataList(ListObjectsView view);
    IControlBase CreateExpansion(ExpansionView view);
    IControlBase CreateGrid(GridView view);
    IControlBase CreateHighchartControl(Highchart view);
    IControlBase CreateHighchartControl(HighchartView view);
    IControlBase CreateImagePanel(ImageView view);
    IControlBase CreateInputSearch(SearchView view);
    IControlBase CreateList(ListView view);
    IControlBase CreateListPanel(ListSelectionView view);
    IControlBase CreateMainMenu(MenuViewModel view);
    IControlBase CreateMaterialList(ListView view);
    IControlBase CreatePagination(PaginationView view);
    IControlBase CreatePanel(ViewItem view);
    IControlBase CreateScrollPane(ScrollView view);
    IControlBase CreateSignalRVideoPanel(SignalRVideoView view);
    IControlBase CreateTable(DataTableView view);
    IControlBase CreateTable(TableView view);
    IControlBase CreateTable(Type generic, dynamic view);
    IControlBase CreateTreePanel(TreeViewModel view);
    IControlBase CreateVideoPanel(VideoView view);
    IControlBase CreateViewPanel(AppViewBase view);
    IControlBase CreateViewPanel(PanelsView view);
}