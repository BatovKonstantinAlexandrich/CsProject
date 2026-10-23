using Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeModule;

using Console_CommonModel.CommonLib.LibViews.AppViews.TreeViewServices;

namespace Console_CommonModel.CommonLib.LibViews
{
    [Label("Иерархическое справочник")]
    public class TreeAdoView : ControlView
    {


        [Label("title")]
        public string title { get => Get<string>(nameof(title)); set => Set(nameof(title), value); }

        [Label("queries")]
        public List<SequentialQuery> queries { get => Get<List<SequentialQuery>>(nameof(queries)); set => Set(nameof(queries), value); }

        //private readonly TreeView view;

        public TreeAdoView()
        {
            title = "Каталог";
            queries = new List<SequentialQuery>();
            //this.view = new TreeView();
        }


        public override void OnInit()
        {
            base.OnInit();

            BindSequentialSelects(null, queries);
        }

        private void BindSequentialSelects(object value, List<SequentialQuery> queries)
        {
            throw new NotImplementedException();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
