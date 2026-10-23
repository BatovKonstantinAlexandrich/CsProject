
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.Others;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Google.Protobuf.WellKnownTypes;

using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;

using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.FormView;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.CheckedListModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.InputModule
{

    [Label("Редактор коллекции примитивных типов")]
    public class InputPrimitiveCollectionView : AppViewBase
    {
        private readonly ToolsItemButtonView deleteButton;
        private readonly ToolsView toolsView = new();
        private readonly ScrollView scrollView = new();

        //private readonly CheckedListView checkedList = new() { MinHeight = 200 };

        [Label("Items")]
        public ChangingList<object> Items { get; } = new();


        public InputPrimitiveCollectionView()
        {
            LayoutFill(scrollView = new());
            Items.OnAdded((i, p) =>
            {
                scrollView.LayoutTop(new TextBoxView() { Text = $"{p}" });
            });
            Top = toolsView;
            toolsView.tools.Add(CreateButtonTool("добавить", OnAddButtonClicked));
            deleteButton = CreateButtonTool("удалить", OnRemoveButtonClicked);
            AutoSize = true;
            MinHeight = 320;
        }

        private ToolsItemButtonView CreateButtonTool(string text, Action todo)
        {
            var button = new ToolsItemButtonView() { Text = text };
            button.OnClick += (p, e) => todo();
            button.OnItemClicked += todo;
            return button;
        }



        private void OnRemoveButtonClicked()
        {

        }

        private void OnAddButtonClicked()
        {
            var submitView = new SubmitView() { Dock = "Fill" };
            var textbox = new TextBoxView();
            submitView.SetView(textbox);
            submitView.oncompleted += (s, p) =>
            {
                if (s)
                {
                    Items.Add(textbox.Text);
                }
                Controls.Remove(submitView);
                Controls.Add(scrollView);
            };
            scrollView.RemoveFromParent();
            Controls.Add(submitView);

        }
    }
}
