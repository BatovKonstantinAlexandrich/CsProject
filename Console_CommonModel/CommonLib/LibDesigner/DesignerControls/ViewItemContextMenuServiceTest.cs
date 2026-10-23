
public class ViewItemContextMenuServiceTest: TestingElement
{
    public ViewItemContextMenuServiceTest(IServiceProvider provider) : base(provider)
    {
    }

    public override void OnTest()
    {
        this.Assert(p => {
            
            var p1 = new AppViewBase();
            var p2 = new AppViewBase();
            p1.Name = "p1";
            p2.Name = "p2";

            p1.ContextMenu.Items.OnAdded((i, p) => { this.Info(p1.Name + $" ContextMenu.ItemAdded"); });
            p1.ContextMenu.Items.OnRemoved((i, p) => { this.Info(p1.Name + $" ContextMenu.ItemRemoved"); });
            p1.ContextMenuPublic.Items.OnAdded((i, p) => { this.Info(p1.Name + $" ContextMenuPublic.ItemAdded"); });
            p1.ContextMenuPublic.Items.OnRemoved((i, p) => { this.Info(p1.Name + $" ContextMenuPublic.ItemRemoved"); });
            p2.ContextMenu.Items.OnAdded((i, p) => { this.Info(p2.Name + $" ContextMenu.ItemAdded"); });
            p2.ContextMenu.Items.OnRemoved((i, p) => { this.Info(p2.Name + $" ContextMenu.ItemRemoved"); });
            p2.ContextMenuPublic.Items.OnAdded((i, p) => { this.Info(p2.Name + $" ContextMenuPublic.ItemAdded"); });
            p2.ContextMenuPublic.Items.OnRemoved((i, p) => { this.Info(p2.Name + $" ContextMenuPublic.ItemRemoved"); });


            p1.ContextMenuPublic.AddContextView("test", "test", () => { });
            this.Info(p1.ContextMenu.Items.Select(p => p.Label).ToList().ToJson());
            p1.Append(p2);
            this.Info(p1.ContextMenu.Items.Select(p => p.Label).ToList().ToJson());
            var p3 = new AppViewBase();
            p3.Name = "p3";
            p3.ContextMenu.Items.OnAdded((i, p) => { this.Info(p3.Name + $" ContextMenu.ItemAdded"); });
            p3.ContextMenu.Items.OnRemoved((i, p) => { this.Info(p3.Name + $" ContextMenu.ItemRemoved"); });
            p3.ContextMenuPublic.Items.OnAdded((i, p) => { this.Info(p3.Name + $" ContextMenuPublic.ItemAdded"); });
            p3.ContextMenuPublic.Items.OnRemoved((i, p) => { this.Info(p3.Name + $" ContextMenuPublic.ItemRemoved"); });

            p2.Append(p3);
            this.Info(p1.ContextMenu.Items.Select(p => p.Label).ToList().ToJson());
            this.Info(p2.ContextMenu.Items.Select(p => p.Label).ToList().ToJson());
            this.Info(p3.ContextMenu.Items.Select(p => p.Label).ToList().ToJson());
            this.Info(p1.ContextMenuPublic.Items.Select(p => p.Label).ToList().ToJson());
            this.Info(p2.ContextMenuPublic.Items.Select(p => p.Label).ToList().ToJson());
            this.Info(p3.ContextMenuPublic.Items.Select(p => p.Label).ToList().ToJson());
            p1.Remove(p2);
            this.Info(p1.Name + $" {p1.ContextMenu.Items.Select(p => p.Label).ToList().ToJson()}");
            this.Info(p2.Name + $" {p2.ContextMenu.Items.Select(p => p.Label).ToList().ToJson()}");
            this.Info(p3.Name + $" {p3.ContextMenu.Items.Select(p => p.Label).ToList().ToJson()}");
            this.Info(p2.ContextMenu.Items.Select(p => p.Label).ToList().ToJson());
            this.Info(p3.ContextMenu.Items.Select(p => p.Label).ToList().ToJson());
            this.Info(p1.ContextMenuPublic.Items.Select(p => p.Label).ToList().ToJson());
            this.Info(p2.ContextMenuPublic.Items.Select(p => p.Label).ToList().ToJson());
            this.Info(p3.ContextMenuPublic.Items.Select(p => p.Label).ToList().ToJson());
            this.Info(p2.ContextMenu.Items.Select(p => p.Label).ToList().ToJson());
            //new AppViewBase().Append(p2);
            this.Info(p2.ContextMenu.Items.Select(p => p.Label).ToList().ToJson());
            var result = p2.ContextMenuPublic.Items.Count() != 0;
            return result;
        }, 
        "Контекстное меню контейнера доступно в элементе",
        "Контекстное меню контейнера не доступно в элементе");
        this.Assert(p => {
            try{
                
                var nav = new NavView();
                var button = new ButtonView();
                nav.AddNavOption("1", button);
                nav.AddNavOption("2", new ButtonView());                
                nav.OnNavSelected("1");
                if(button.Parent?.Parent != nav)
                {
                    Messages.Add("Не установилась ссылка на parent при переходе навигации");
                }
                nav.ResolveDesignMode = true;
                nav.DesignMode = true;
                this.Info(nav.ViewServices.Select(p => p.GetTypeName()).ToList().ToJson());
                if (!nav.ViewDesigner.active)
                {
                    Messages.Add("ViewDesigner не активировался");
                }
                nav.ContextMenu.Trace();
                button.ContextMenu.Trace();
                return true;
            }
            catch
            {
                return false;
            }
        },
        "Сервис связывает модели контекстного меню с контейнером элемента управления",
        "Сервис не связывает контекстное меню");

    }
}