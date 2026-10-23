using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System.Reflection;

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.DbSetViewModule
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class DbSetViewDesigner<TDbContext, TEntity> : DesignerBase, IDisposable
        where TDbContext : DbContextBase
        where TEntity : BaseEntity
    {

        public ChangingDictionary<string, Action> SharedOperations = new();

        public DbSetViewDesigner() : base() { }

        public DbSetViewDesigner([NotNullNotEmpty] DbSetViewModel<TDbContext, TEntity> view) : base(view)
        {
            view.EnsureIsValide();
        }
        public DbSetViewDesigner([NotNullNotEmpty] DbSetView<TDbContext, TEntity> view) : base(view)
        {
            view.EnsureIsValide();
        }

        /// <summary>
        /// Получение аргументов выполнения метода из
        /// формы ввода пользоватлелем
        /// </summary>    
        private object[] GetUserInputParameters(MethodInfo m)
        {
            var inputFormModel = m.DeclaringType.GetInputFormModel(m.Name);
            var controlFormModel = inputFormModel.ToControl();
            List<object> args = new();
            foreach (var name in m.GetParameters().Select(p => p.Name))
            {
                args.Add(inputFormModel.FormFields.FirstOrDefault(f => f.Name == name).Value);
            }
            return args.ToArray();
        }


        /// <summary>
        /// Получение аргументов выполнения метода из
        /// области видимости представления
        /// </summary>
        private object[] GetScopeDicoveryParameters(MethodInfo m)
        {
            var inputFormModel = m.DeclaringType.GetInputFormModel(m.Name);
            List<object> args = new();
            var parameters = new ChangingDictionary<string, object>();
            foreach (var name in m.GetParameters().Select(p => p.Name))
            {
                var key = name;
                view.@scope(name, val => parameters[key] = val);
            }
            return args.ToArray();
        }


        /// <summary>
        /// Подключение функций проектирования и разработки
        /// </summary>
        protected override void Activating(ViewNode viewbase)
        {
            DbSetView<TDbContext, TEntity> view = (DbSetView<TDbContext, TEntity>)viewbase;
            view.FixState();
            rollback.Add(view.AddContextRadioGroup(nameof(view.Dock), view));

            view.AddContextServiceMethods("Настройки", this, m => GetUserInputParameters(m));
            view.AddContextServiceOptions<ViewOptions>(view);
            view.AddContextServiceMethods("Сервисы", SharedOperations);
            view.AddContextMethods(nameof(Dock), new() {
                { "свойства", ()=>{

                    view.Edit(view, (success, data)=>{
                        if(success)
                        {
                            view.Commit();
                        }
                        else
                        {
                            view.Rollback();
                        }
                    }).ShowDialog();
                } }
            });
        }

        public override void Deactivate(ViewNode view)
        {

        }

        public void Dispose()
        {

        }
    }
}
