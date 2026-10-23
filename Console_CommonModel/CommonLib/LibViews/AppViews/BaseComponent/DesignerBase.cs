using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent
{
    /// <summary>
    /// Класс сервисов для инициализации управления в среде разработки.
    /// При смены режима DesignMode все сделанные изменения должны быть отменены,
    /// для этого в rollback нужно регистрировать операции для отката изменений.    
    /// </summary>
    public abstract class DesignerBase : ViewItem
    {

        [Label("Состояние")]
        public bool active { get; set; } = false;


        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public readonly AppViewBase view;

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public List<Action> rollback = new();


        public DesignerBase()
        {
        }

        public DesignerBase(AppViewBase view)
        {
            this.view = view;
        }


        protected abstract void Activating(ViewNode view);


        public void Activates(ViewNode view)
        {
            //this.Info(GetType().GetTypeName());
            if (!active)
            {
                Activating(view);
                foreach (var p in view.Controls)
                {
                    Activating(p);
                }
            }
            
            active = true;
        }

        public virtual void Deactivate(ViewNode view)
        {
            if (active)
            {
                foreach (var action in rollback)
                {
                    action();
                }
                rollback.Clear();
                active = false;
            }
        }

        public virtual MenuView CreateDesignerMenuView()
        {
            var menu = new MenuView();
            return menu.CreateMenuView(
                GetLabel(),
                GetActionMap(this)
            );
        }

        public virtual ContextView CreateDesignerContextView()
        {
            var menu = new MenuView().CreateContextView(
                GetLabel(),
                GetActionMap(this)
            );
            return menu;
        }


        public override Dictionary<string, Action> GetActionMap(object service, Action<string, object> onresult = null)
        {
            var actionNames = GetActionNamesMap(new List<object>() { service }, onresult);
            var valideKeys = actionNames.Keys.ToHashSet()
                .Except(typeof(DesignerBase).GetOwnMethodNames())
                .Except(new List<string>() { nameof(IControlService<AppViewBase>.SetControl) })

                .Except(typeof(IControlService<>).GetOwnMethodNames());
            return new Dictionary<string, Action>(actionNames.Where(kv => valideKeys.Contains(kv.Key)));
        }
    }
}