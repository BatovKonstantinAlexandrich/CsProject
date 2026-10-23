using Console_CommonModel.CommonLib.LibViews.AppViewModules.HighchartsModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.MediaModule;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.DataTable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win_BusinessApp.CommonModel
{
    internal class ComponentsFactory: IComponentsFactory<ControlBase>
    {
        public void Bind(ControlBase TControl, ViewItem view)
        {
            throw new NotImplementedException();
        }

        public void Bind(ViewItem view, ControlBase control)
        {
            throw new NotImplementedException();
        }

        public void BindContextMenu(ViewItem view, ControlBase control)
        {
            throw new NotImplementedException();
        }

        public void BindControlEvents(ViewItem item, ControlBase control)
        {
            throw new NotImplementedException();
        }

        public void BindEvents(ControlBase control, ViewItem view)
        {
            throw new NotImplementedException();
        }

        public void BindEvents(ViewItem view, ControlBase control)
        {
            throw new NotImplementedException();
        }

        public void BindSideControls(AppViewBase view, ControlBase control)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateButton(ButtonControlView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateButton(ButtonView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateCard(CardView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateExpansion(ExpansionView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateHighchartControl(HighchartView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateHighchartControl(Highchart model)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateColumnsView(ColumnsView columnsView)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateControl(string name, ViewItem view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateControl(ViewItem view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateControl<TTControl>(ViewItem view) where TTControl : ControlBase
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateControl3(ViewItem view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateControlRoot(AppViewRoot view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateDataList(ListObjectsView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateGrid(GridView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateImagePanel(ImageView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateAudioPanel(AudioView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateAudioRecorderPanel(AudioRecorderView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateVideoPanel(VideoView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateCameraPanel(CameraView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateSignalRVideoPanel(SignalRVideoView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateInputSearch(SearchView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateList(ListView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateListPanel(ListSelectionView list)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateMainMenu(MenuViewModel view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateMaterialList(ListView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateMenuItem(ILink plink, Action<string> onclick)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreatePagination(PaginationView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreatePanel(ViewItem view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateScrollPane(ScrollView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateTable(DataTableView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateTable(TableView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateTable(Type generic, dynamic view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateTreePanel(TreeViewModel view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateViewPanel(PanelsView view)
        {
            throw new NotImplementedException();
        }

        public ControlBase CreateViewPanel(AppViewBase view)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetComponentNames()
        {
            throw new NotImplementedException();
        }
    }
}
