using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Newtonsoft.Json;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule
{
    /// <summary>
    /// Действие по нажатию: OnExecute 
    /// </summary>
    [Label("Модель элемента меню")]
    public class MenuView : AppViewBase, IMenuViewFactory, IMenuViewService
    {
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        private MenuViewFactory factory = new();
        private readonly MenuViewService service;

        [Label("Текст пункта меню")]
        [InputRusText()]
        public string Text { get => Get<string>(nameof(Text)); set => Set(nameof(Text), value); }

        [Label("Иконка (имя файла в папке Icons)")]
        [InputIcon()]
        public string Icon { get => Get<string>(nameof(Icon)); set => Set(nameof(Icon), value); }

        [Label("Сочетание клавиш (например Ctrl+O или F1)")]
        [InputKeys()]
        public string Shortcut { get => Get<string>(nameof(Shortcut)); set => Set(nameof(Shortcut), value); }


        public string SetText(string text)
        {
            return Text = text;
        }

        [NotInput]
        [Label("Выделено галками")]
        public override bool Checked { get => Get<bool>("Checked"); set => Set("Checked", value); }

        [Label("Команда при нажатии")]

        public string Command { get; set; }

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [NotInput]
        [Label("Действие при нажатии")]
        public Action<string> OnCommandExecute { get; set; } = (cmd) =>
        {
            Console.WriteLine(cmd);
        };

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        private string _registeredKeyboardShortcutKey;

        [Label("Дочерние пункты меню")]
        [InputOrder(10)]
        [InputStructureCollection(nameof(MenuView))]
        [XmlChildNode()]
        public ChangingList<MenuView> MenuItems { get; set; }

        [Label("Расположение пункта меню")]
        [InputSelect("left,right")]
        public string Alignment { get => Get<string>(nameof(Alignment)); set => Set(nameof(Alignment), value); }
        public string _Href;
        public string Href
        { 
            get => _Href;
            set {
                _Href = value;
                OnClick = (p, e) => {
                    NavigateTo(_Href);
                };
            } 
        }


        public void NavigateTo(string href)
        {
            ((dynamic)this.GetAppRootView().control).NavigateTo(href);
        }

        public MenuView() : this("/")
        {
            InitViewModel();
            Alignment = "left";
            Text = "";
            service = new MenuViewService(this);
            Shortcut = "";
            Icon = "";
            ViewDesigner = new MenuViewDesigner(this);
            MenuItems.OnAdded((i, p) =>
            {
                Controls.Add((MenuView)p);
            });
            MenuItems.OnRemoved((i, p) =>
            {
                Controls.Remove((MenuView)p);
            });
            MinHeight = 30;
        }


        public override void Add(ViewNode item)
        {
            if (item is not MenuView mv)
                throw new InvalidOperationException("В MenuView можно добавлять только MenuView.");

            if (MenuItems.Contains(mv))
                return;

            if (mv.Parent is MenuView menuParent && !ReferenceEquals(menuParent, this))
                menuParent.MenuItems.Remove(mv);
            else if (mv.Parent is not null && mv.Parent is not MenuView)
                mv.RemoveFromParent();

            MenuItems.Add(mv);
        }

        public override void Remove(ViewNode item)
        {
            if (item is MenuView mv)
                MenuItems.Remove(mv);
            else
                throw new InvalidOperationException("Из MenuView можно удалять только MenuView.");
        }

        public override void RemoveFromParent()
        {
            if (Parent is MenuView menuParent && menuParent.MenuItems.Contains(this))
            {
                menuParent.MenuItems.Remove(this);
                return;
            }
            base.RemoveFromParent();
        }


        private void InitViewModel()
        {
            //MenuItems.OnAdded(OnMenuItemAdded);
            //MenuItems.OnRemoved(OnMenuItemRemoved);
            AddPropertyChangeListener(nameof(Shortcut), _ => SyncKeyboardShortcutRegistration());
        }


        public MenuView(string path) : base()
        {
            var names = path.Split('/');
            Text = names.Last();
            Command = path;
            MenuItems = new ChangingList<MenuView>();
            WireMenuItemsCollection();
        }

        /// <summary>
        /// Пункт вложенного меню: <see cref="Parent"/> = владелец из <see cref="MenuItems"/>.
        /// Само меню в разметке получает <see cref="Parent"/> от контейнера (Controls, Top, …).
        /// </summary>
        private void WireMenuItemsCollection()
        {
            MenuItems.OnAdded((_, item) => AttachMenuItem((MenuView)item));
            MenuItems.OnRemoved((_, item) => DetachMenuItem((MenuView)item));
        }

        private void AttachMenuItem(MenuView child)
        {
            if (child is null)
                return;
            child.Parent = this;
            if (!Controls.Contains(child))
                Controls.Add(child);
        }

        private void DetachMenuItem(MenuView child)
        {
            if (child is null)
                return;
            if (Controls.Contains(child))
                Controls.Remove(child);
            else if (ReferenceEquals(child.Parent, this))
                child.Parent = null;
        }



        private void OnMenuItemAdded(int arg1, object arg2)
        {
            if (arg2 is MenuView child)
                child.RegisterShortcutsRecursive();
        }

        private void OnMenuItemRemoved(int arg1, object arg2)
        {
            if (arg2 is MenuView child)
                child.UnregisterShortcutsRecursive();
        }

        /// <summary>
        /// Регистрирует сочетания клавиш этого пункта и всех вложенных в словарь <c>keyboardConfig</c> у <see cref="AppViewRoot"/>.
        /// </summary>
        public void RegisterShortcutsRecursive()
        {
            TryRegisterKeyboardShortcut();
            foreach (var child in MenuItems)
                child.RegisterShortcutsRecursive();
        }

        /// <summary>
        /// Удаляет регистрацию сочетаний для этого пункта и вложенных.
        /// </summary>
        public void UnregisterShortcutsRecursive()
        {
            UnregisterKeyboardShortcut();
            foreach (var child in MenuItems.ToList())
                child.UnregisterShortcutsRecursive();
        }

        public void SyncKeyboardShortcutRegistration()
        {
            TryRegisterKeyboardShortcut();
        }

        private void TryRegisterKeyboardShortcut()
        {
            UnregisterKeyboardShortcut();
            if (string.IsNullOrWhiteSpace(Shortcut) || string.IsNullOrWhiteSpace(Command))
                return;
            if (!KeyboardShortcutKeyFormatter.TryParseKeys(Shortcut, out _))
                return;
            var root = GetAppRootView();
            if (root is null)
                return;
            var key = KeyboardShortcutKeyFormatter.Normalize(Shortcut);
            if (string.IsNullOrEmpty(key))
                return;
            _registeredKeyboardShortcutKey = key;
            root.keyboardConfig[key] = () => ExecuteMenuCommandFromShortcut(root);
        }

        private void UnregisterKeyboardShortcut()
        {
            if (_registeredKeyboardShortcutKey is null)
                return;
            var root = GetAppRootView();
            root?.keyboardConfig.Remove(_registeredKeyboardShortcutKey);
            _registeredKeyboardShortcutKey = null;
        }

        private void ExecuteMenuCommandFromShortcut(AppViewRoot root)
        {
            if (string.IsNullOrEmpty(Command))
                return;
            var stripRoot = root.MenuView;
            OnCommandExecute?.Invoke(Command);
            stripRoot?.OnCommandExecute?.Invoke(Command);
        }



        public MenuView CreateMenuView(string name, Dictionary<string, Action> operations)
        {
            var p = factory.CreateMenuView(name, operations);
            MenuItems.Add(p);
            return p;
        }

        public MenuView CreateMenuView(string name, object service)
        {
            var p = factory.CreateMenuView(name, service);
            MenuItems.Add(p);
            return p;
        }


        public string ToJson() =>
            JsonConvert.SerializeObject(this);
        public static List<MenuView> FromJson(string json) =>
            JsonConvert.DeserializeObject<List<MenuView>>(json);

        public Action AddMenuActions(string name, Dictionary<string, Action> actions)
        {
            return ((IMenuViewService)service).AddMenuActions(name, actions);
        }

        public Action AddMenuActions(Dictionary<string, Action> actions)
        {
            return ((IMenuViewService)service).AddMenuActions(actions);
        }

        public MenuView CreateMenuActions(string name, Dictionary<string, Action> actions)
        {
            return ((IMenuViewService)service).CreateMenuActions(name, actions);
        }

        public List<MenuView> CreateMenuActions(Dictionary<string, Action> actions)
        {
            return ((IMenuViewService)service).CreateMenuActions(actions);
        }

        public MenuView CreateMenuViewServiceActions<TService>()
        {
            return ((IMenuViewService)service).CreateMenuViewServiceActions<TService>();


        }

        public MenuView CreateMenuViewDbContextInitializerActions()
            => ((IMenuViewService)service).CreateMenuViewDbContextInitializerActions();

        public IDictionary<string, Action> GetActionMap(object service, Type ptype)
        {
            return contextFactory.GetActionMap(service, ptype);
        }

    }
}
