using Blazor_CommonModel.Components.Shared;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.MediaModule;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.DataTable;

using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Org.BouncyCastle.Crmf;

using ListViewModel = global::ListView;
using TableViewModel = global::TableView;

namespace BlazorErp.CommonModel;

public sealed class WebComponentsFactory : IComponentsFactory<WebComponentBase>
{

    public Dictionary<string, string> GetComponentNames()
    {
        throw new NotImplementedException();
    }


    public WebComponentBase CreateControl(string name, ViewItem view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateControl(ViewItem view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateControl3(ViewItem view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateControlRoot(AppViewRoot view)
    {
        throw new NotImplementedException();
    }





    public WebComponentBase CreateButton(ButtonControlView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateButton(ButtonView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateCard(CardView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateExpansion(ExpansionView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateHighchartControl(HighchartView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateHighchartControl(Highchart model)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateColumnsView(ColumnsView columnsView)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateDataList(ListObjectsView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateGrid(GridView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateImagePanel(ImageView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateAudioPanel(AudioView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateAudioRecorderPanel(AudioRecorderView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateVideoPanel(VideoView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateCameraPanel(CameraView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateSignalRVideoPanel(SignalRVideoView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateInputSearch(SearchView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateList(ListViewModel view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateListPanel(ListSelectionView list)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateMainMenu(MenuViewModel view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateMaterialList(ListViewModel view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateMenuItem(ILink plink, Action<string> onclick)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreatePagination(PaginationView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreatePanel(ViewItem view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateScrollPane(ScrollView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateTable(DataTableView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateTable(TableViewModel view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateTable(Type generic, dynamic view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateTreePanel(TreeViewModel view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateViewPanel(PanelsView view)
    {
        throw new NotImplementedException();
    }

    public WebComponentBase CreateViewPanel(AppViewBase view)
    {
        throw new NotImplementedException();
    }

    

}
