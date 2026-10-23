using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.LayoutModule
{

    [Label("Представление позиционирования")]
    [Description("Представление позиционирования реализует свойства: \n" +
        "IsVertical - true размещение вертикально вниз, каждый следующий элемент будет расположен сразу после последнего, false - справа\n" +
        "WrapContent - переносить контент в случае переполнения, убирает возможность прокрутки контента")]
    public class AppLayoutView
        : AppViewBase, ILayoutViewNode, ILayoutView
    {

        [InputBool]
        [Label("Is Vertical")]
        public bool IsVertical { get => Get<bool>(nameof(IsVertical)); set => Set(nameof(IsVertical), value); }

        [InputBool]
        [Label("Wrap Content")]
        public bool WrapContent { get => Get<bool>(nameof(WrapContent)); set => Set(nameof(WrapContent), value); }

        [InputBool]
        [Label("Fill Size")]
        public bool FillSize { get => Get<bool>(nameof(FillSize)); set => Set(nameof(FillSize), value); }


        public AppLayoutView()
        {
            WrapContent = false;
            IsVertical = true;
            FillSize = true;
            AutoSize = true;
        }

        public void Add(AppViewBase item)
        {
            Controls.Add(item);
        }

        public void Remove(AppViewBase item)
        {
            Controls.Remove(item);
        }


    }
}
