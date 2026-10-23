using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API;
using Console_CommonModel.CommonLib.LibViews.AppViewModules;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeServices;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.DataComponents
{


    [Label("Редактор структуры данных")]
    public class DataEditorControl<TDbContext, TEntity> : AppViewBase
        where TEntity : NamedObject, IChangeSupport
        where TDbContext : DbContextBase
    {
        private readonly ChangingDataView<TEntity> collectionView;

        private readonly SearchView searchView;

        private SearchDataList<TEntity> dataList;
        private TreeControlView treeView;

        private TreeViewEntityBuilder<TDbContext> treeBuilder = new();

        public DataEditorControl()
        {
            IApi api = null;
            using (var db = typeof(TDbContext).New<DbContextBase>())
            {
                api = db.GetSqlServerWebApi();
            }

            var fasade = api.GetFasade<TEntity>();
            treeBuilder.Join<TEntity>();
            treeView = treeBuilder.Build();
            dataList = new(fasade);
            LayoutFill(collectionView = new ChangingDataView<TEntity>(api));

            Top = searchView = new Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule.SearchView();
            TopHeight = 25;
            Left = treeView;
            LeftWidth = 200;
        }


        //public override Control ToControl() => ToControl<AppViewBase>();
    }

}
