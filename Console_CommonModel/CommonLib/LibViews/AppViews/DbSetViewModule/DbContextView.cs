using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.DbSetViewModule
{

    [Label("Представление для использование источников данных EF")]
    public class DbContextView : AppViewBase
    {

        [InputSelectExpression(nameof(DbContextTypeNames))]
        [Label("Db Context Type")]
        public string DbContextType { get; set; }


        [NotInput]
        [Label("Db Context Type Names")]
        public List<string> DbContextTypeNames { get; set; }


        public DbContextView()
        {
            DbContextTypeNames = ServiceFactory.Get().GetTypesExtended<DbContextBase>().Where(t => !t.IsAbstract && t.GetGenericArguments().Count() == 0).Select(t => t.GetTypeName()).ToList();
        }

        [OnChange(nameof(DbContextType))]
        public void OnDbContextTypeChanged(PropertyChangedMessage evt)
        {

        }




    }
}
