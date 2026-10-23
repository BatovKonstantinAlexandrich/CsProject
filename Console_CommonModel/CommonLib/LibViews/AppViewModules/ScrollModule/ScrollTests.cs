using Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.ScrollModule
{
    public class ScrollTests : TestingElement
    {
        public ScrollTests(IServiceProvider serviceProvider)
        {

        }

        public override void OnTest()
        {
            var root = new AppViewRoot();

            var scroll = new SearchView() { Dock = "Fill" };
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });
            scroll.Add(new ButtonView() { Dock = "Top", Height = 200 });

            root.Add(scroll);
            scroll.Dock = "Fill";
            root.ShowDialog();
        }
    }
}
