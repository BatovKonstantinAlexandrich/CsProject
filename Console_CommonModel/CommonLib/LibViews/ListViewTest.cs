
using Console_CommonModel.CommonLib.LibViews.LayoutVIews;

namespace Console_CommonModel.CommonLib.LibViews
{
    internal class ListViewTest : TestingElement
    {
        public ListViewTest(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            var context = new DbContextLayoutView<DeliveryDbContext>();
            var view = context.CreateSelectList("Таблицы", 
                context.GetDbContext().GetTableNames().ToList(), 
                selectedTable => {
                
            });
            context.Append(view);

            view.DesignMode = true;
            view.DesignMode = false;

            this.Info(view.ContextMenu);
        }
    }
}