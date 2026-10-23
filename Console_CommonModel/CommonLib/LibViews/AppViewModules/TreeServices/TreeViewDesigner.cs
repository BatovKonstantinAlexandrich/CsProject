using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeServices
{

    /// <summary>
    /// Предоставляет инструменты для разработки представлений типа TreeView.    
    /// </summary>
    [Label("Проектирование иерархического представления")]
    public class TreeViewDesigner : DesignerBase
    {
        public TreeViewDesigner() : base()
        {
        }

        public TreeViewDesigner(TreeControlView view) : base(view)
        {
        }

        protected override void Activating(ViewNode viewbase)
        {
            try
            {
                TreeControlView view = (TreeControlView)viewbase;
                rollback.Add(view.AddContextMethods("Связывание данных", GetDbContextSetups(view)));
                rollback.Add(view.AddContextRadioGroup(nameof(Dock), view));

            }
            catch (Exception ex)
            {
                this.Error(ex);
            }
        }


        private Dictionary<string, Action> GetDbContextSetups(AppViewBase viewbase)
        {
            TreeControlView view = (TreeControlView)viewbase;
            Dictionary<string, Action> todos = new();
            var ctxTypes = ServiceFactory.Get().GetTypesExtended<DbContextBase>().Where(ctx => ctx.GetGenericArguments().Count() == 0);
            foreach (var ctxType in ctxTypes)
            {
                var ptype = ctxType;
                todos[ctxType.GetLabel()] = () =>
                {
                    //Form form = null;
                    //var ctrl = typeof(TreeViewEfService<>).MakeGenericType(ptype).New<Control>(new object[] { (bool success,TreeControlView created)=> {
                    //    if(success) {
                    //        //view.root = created.root;
                    //        view.root.Children.Clear();
                    //        view.root.Children.AddRange(created.root.Children);


                    //    }
                    //    form.Close();
                    //} });
                    //form = ctrl.ToBaseForm();
                };
            }
            return todos;
        }


        public void ActivateEditable(TreeControlView view)
        {
            if (view.Editable)
            {
                view.OnClick += (s, p) => { };

                //поиск, выбор, перетаскивание
                rollback.Add(view.AddContextServiceOptions<ViewOptions>(view));
                rollback.Add(view.AddContextServiceOptions<AppViewBase>(view));
            }
        }





    }
}
