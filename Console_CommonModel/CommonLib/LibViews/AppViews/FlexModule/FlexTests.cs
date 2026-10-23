using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.FormModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.FlexModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.FlexModule
{

    [Label("Проверка размещения")]
    public class FlexTests : TestingElement
    {
        public override void OnTest()
        {
            var side = new AppViewBase();
            var flex = new FlexView();
            side.Top = new AppViewBase();
            side.Top.CreateButton("добавить", () => { flex.Add(new ButtonView() { }); });
            side.Right = EditView.CreateEditViewBase(flex);
            side.RightWidth = 300;
            side.LayoutFill(flex);
            side.ToControl().RunAtForm();
        }

        public static void Run()
        {
            var tests = new FlexTests();
            tests.DoTest().ToDocument().WriteToConsole();
        }
    }
}
