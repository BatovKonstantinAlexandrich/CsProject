using Console_CommonModel.CommonLib.LibViews.AppViewModules.ContextModule;

using Console_CommonModel.CommonLib.LibViews.AppViews;
using Console_CommonModel.CommonLib.LibViews.AppViews.CardModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.CollectionModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.MessageModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.ScrollModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.StatusModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TypedModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews
{
    [Label("Модуль тестирования компонентов представлений")]
    [Description("Каждая модель представления должна быть сериализуемой как в JSON, так и в XML с" +
        "использование XMLCompiler. При этом должна быть обеспечена сохранемость состояния представления и" +
        " связанных представлений.")]
    public class AppViewsUnit : TestingUnit
    {
        public AppViewsUnit()
        {
            Init(provider);
        }

        public AppViewsUnit(IServiceProvider provider) : base(provider)
        {
            Init(provider);
        }

        private void Init(IServiceProvider provider)
        {


            Append(new CardTests(provider));
            //Append(new ChartTests(provider));
            Append(new CollectionTests(provider));
            Append(new ColumnsTests(provider));


            Append(new ContextTests(provider));
            Append(new ListTests(provider));
            //Append(new MenuTests(provider));
            Append(new MessageTests(provider));
            Append(new NavListTests(provider));
            Append(new PagesTests(provider));
            Append(new ScrollTests(provider));
            //Append(new SearchTests(provider));
            Append(new StatusTests(provider));
            Append(new TableTests(provider));
            //Append(new ToolsTests(provider));
            Append(new TreeTests(provider));
            Append(new TypedTests(provider));
        }
    }
}
