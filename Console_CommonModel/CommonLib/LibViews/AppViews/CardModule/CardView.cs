using Newtonsoft.Json;

using Console_CommonModel.CommonLib.LibViews.AppViews.CollectionModule;

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
using System.Threading.Channels;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.FlexModule;


[ViewUsageExample("var view = new CardView { Text = \"Карточка\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<card-view Text=\"Карточка\" Dock=\"Top\" />")]
[Label("Карточка")]
[Description("Панель для размещения элементов управления")]
public class CardView : FlexView, ILayoutViewNode
{

    [InputBool]
    [Label("Развернуто")]
    public override bool Expanded { get => Get<bool>("Expanded"); set => Set<bool>("Expanded", value); }

    public CardView()
    {
        var card = this;
        card.Expanded = true;
        card.MinHeight = card.MinWidth = 225;
    }
}
