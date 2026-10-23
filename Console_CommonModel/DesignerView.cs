

using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageModel;
using Console_CommonModel.CommonLib.LibModels;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TabsModule;

using WinFormsApp.API;
using WinFormsApp.AppViews.AppRootModule;
using WinFormsApp.DesignerServices;

using static WinFormsApp.API.DesignerResizeTool;

namespace WinFormsApp
{


    /// <summary>
    /// Конструктор разметки: редактируемый XML в <see cref="AppProgramView.xmlView"/>, а не в <see cref="AppViewRoot.layoutView"/>.
    /// </summary>
    public class DesignerView : AppProgramView
    {
        public readonly AppViewBase layoutPane;


        [Label("Панель инструментов для управления представлениями на форме приложения")]
        public ToolsView toolsView { get; }


        [Label("Форма редактирования/отображения предсталений")]
        public TreeControlView layoutTree { get; set; }


        [Label("Иерархическое представление содержит узлы со ссылками на редактируемые представления")]
        public TreeControlView layoutNodes { get; set; }


        [Label("Панель вкладок слевого края")]
        public TabsView leftTabs { get; set; }
        /// <summary>Палитра типов (вкладка «Элементы»), чтобы отфильтровать из списка фокуса.</summary>
        [Label("elements Tree")]
        public TreeControlView elementsTree { get; set; }
        /// <summary>Как в <see cref="ConstructorView"/> — список элементов в фокусе.</summary>
        [Label("focused List View")]
        public ListView focusedListView { get; } = new();
        /// <summary>Источники данных для перетаскивания (как вкладка «Данные» в конструкторе).</summary>
        [Label("datasources Tree")]
        public TreeControlView datasourcesTree { get; set; }

        /// <summary>Узел «WCf/SOAP сервисы» — дети подгружаются из <see cref="SoapServiceBuilder"/>.</summary>
        public TreeViewNodeModel _wsdlDatasourcesCategoryNode;
        
        public List<Action> rollbackDesignTools = new();
        public readonly List<Action> rollbackDesigners = new();
        /// <summary>На сколько пикселей расширено окно при показе левой панели конструктора (чтобы канва XML не сжималась по ширине).</summary>
        public int _designerHostWidthDelta;
        /// <summary>Уже настроен <see cref="ViewItem.SetDroppable"/> для этой панели (каждый уровень иерархии).</summary>
        public readonly HashSet<AppViewBase> _designerDropSurfaceConfigured = new();
        /// <summary>На чьи <see cref="AppViewBase.Controls"/> уже подписан <c>OnAdded</c> для DnD (без дублирования).</summary>
        public readonly HashSet<AppViewBase> _designerChildAddedSubscribed = new();
        /// <summary>На чьи <see cref="AppViewBase.Controls"/> уже подписан синк удаления в layoutTree.</summary>
        public readonly HashSet<AppViewBase> _layoutTreeRemovalSubscribed = new();
        /// <summary>Для каких представлений уже вызван <see cref="AppViewBase.CreateDesignTools"/> (без повторного дублирования пунктов меню).</summary>
        public readonly HashSet<AppViewBase> _designToolsApplied = new();
        /// <summary>Узлы <see cref="layoutTree"/>, для которых уже подключена синхронизация контекстного меню с <see cref="TreeViewNodeModel.Item"/>.</summary>
        public readonly HashSet<TreeViewNodeModel> _layoutTreeContextMenuSyncAttached = new();
        public bool _elementsPaletteSelectionWired;


        public TreeViewNodeModel apiServices, dbContexts, adods, odbcds;


        /// <summary>Панель инструментов канвы (перемещение / размер по элементам из <see cref="AppViewRoot.FocusedViews"/>).</summary>
        [Label("designer Tools View")]
        public ToolsView designerToolsView { get; } = new ToolsView();


        [Label("User Context")]
        public UserContext UserContext { get; set; }
        public Point FormStartPosition { get; set; }

        public enum DesignerCanvasTool
        {
            None,
            Move,
            Resize
        }
        public override void OnSaveFile()
        {
            PutAction($"Сохранение {filename}", () =>
            {
                GetLayoutRootPane().Trace();
                this.xmlText = //GetLayoutRootPane().ToXmlText();
                    this.xmlView.ToXmlText();
                this.Info(xmlText);
                System.IO.File.WriteAllText(this.filename, this.xmlText);
            });
        }

        /// <summary>Обновляет текст на <see cref="AppViewRoot.StatusView"/> после действия пользователя.</summary>
        public void SetDesignerStatus(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            //RunOnUiThread(() => StatusView.Text = text);
            StatusView.Text = text;
        }

        public void RunDesignerAction(string status, Action action)
        {
            SetDesignerStatus(status);
            action?.Invoke();
        }
        public DesignerCanvasTool _activeCanvasTool = DesignerCanvasTool.None;
        public bool _designerToolsInitialized;
        public readonly Dictionary<AppViewBase, Action> _moveToolUnbind = new();
        public readonly Dictionary<AppViewBase, Action> _resizeToolUnbind = new();
        public const int DesignerResizeHandleSize = 12;
        public const int DesignerLeftPanelWidth = 300;
        public const int DesignerToolbarHeight = 32;

        public DesignerView():base()
        {
            this.CurrentTool = new DesignerViewMoveTool(this);
            this.layoutPane = this.xmlView;
            this.toolsView = new ToolsView();
            
            MenuView.MenuItems.Add(new()
            {
                Text = "Тест далога",
                OnClick = (s,e) => {
                    var p = ((string, string, byte[]) filedata) => {
                        this.Info(filedata.Item1);
                        this.Info(filedata.Item2);
                        this.Info(filedata.Item3);
                    };
                    ((dynamic)(this.GetAppRootView().control)).OpenFileDialog( p );
                }
            });
         
            this.MenuView.AddMenuActions("Представление", new() {
                { "Настроить DragAndDrop", () => RunDesignerAction("Настройка DragAndDrop", ActivateDragAndDrop) },
                { "Добавить контекстное меню", () => RunDesignerAction("Добавление контекстного меню", ActivateContextMenu) },
                { "Настройки клавиатуры", () => RunDesignerAction("Настройки клавиатуры", OpenKeyboardCustomizer) },
            });
            this.MenuView.CreateMenuViewDbContextInitializerActions();
            this.MenuView.AddMenuActions("Операции с данными", new() {
                { "Отчистить базу данных пользователей",()=>{
                    this.PutAction("Отчистить базу данных пользователей",()=>{
                        var initiallizer = new DbContextUserInitializer();
                        initiallizer.Init(new DeliveryDbContext());
                    });
                } },
                { "Регистрация бизнес модели",()=>{
                    this.PutAction("Регистрация бизнес модели",()=>{
                        var initiallizer = new BusinessDbContextInitiallizer();
                        initiallizer.InitData(new DeliveryDbContext());
                    });
                } },
                { "Пересобрать данные для базы службы доставки",()=>{
                    this.PutAction("Пересобрать данные для базы службы доставки",()=>{
                        using (var db = new DeliveryDbContext())
                        {
                            db.Database.EnsureDeleted();
                            db.Database.EnsureCreated();

                            var dbContextUserInitiallizer = new DbContextUserInitializer();
                            dbContextUserInitiallizer.Init(db);

                            var messagesInitiallizer = new MessagesDbContextInitializer();
                            //var messageContext = new MessageDbContext<DeliveryDbContext>();                
                            //messagesInitiallizer.Init(messageContext);

                            var initiallizer = new DeliveryDbContextInitiallizer();
                            initiallizer.InitFromFiles(db, "D:\\System-Config\\ProjectsConsole\\Console_BusinessModel\\wwwroot\\Resources");
                        }
                    });
                } },

            });

            AddTestingHierarchyToMainMenu();
            
            this.toolsView.tools.Add(this.toolsView.CreateButtonTool("размер", () =>
            {
                RunDesignerAction("Инструмент панели: размер", () =>
                    SwitchCurrentTool(new DesignerResizeTool(this)));
            }));
            this.toolsView.tools.Add(this.toolsView.CreateButtonTool("move", () =>
            {
                RunDesignerAction("Инструмент панели: перемещение", () =>
                    SwitchCurrentTool(new DesignerViewMoveTool(this)));
            }));
            this.toolsView.tools.Add(this.toolsView.CreateButtonTool("resize", () =>
            {
                RunDesignerAction("Инструмент панели: размер", () =>
                    SwitchCurrentTool(new DesignerResizeTool(this)));
            }));
            this.ResolveDesignMode = false;
            this.layoutTree = new TreeControlView();
            this.AddPropertyChangeListener(nameof(DesignMode), evt => SetDesignerModeView((bool)evt.After));
            this.AddPropertyChangeListener(nameof(DesignMode), evt =>
            {
                ApplyDesignModeToLayoutRoot((bool)evt.After);
                if ((bool)evt.After)
                {
                    ActivateDesignMode();
                }
                else
                {
                    rollbackDesigners.ToList().ForEach(this.RunOnUiThread);
                    rollbackDesignTools.ToList().ForEach(this.RunOnUiThread);
                    this.rollbackDesigners.Clear();
                    this.rollbackDesignTools.Clear();
                }
            });
            FocusedViews.OnChanged += OnDesignerFocusedViewsChanged;
            this.AddPropertyChangeListener(nameof(DesignMode), evt =>
            {
                SetDesignerStatus((bool)evt.After
                    ? "Режим конструктора: включён"
                    : "Режим конструктора: выключен");
            });
            this.keyboardConfig["q"] = () =>
            {
                DesignMode = !DesignMode;
            };
            SetDesignerStatus("Конструктор готов");
        }

        public override void HandleError(object sender, string message)
        {
            SetDesignerStatus($"{sender.GetTypeName()}: {message}");
            base.HandleError(sender, message);
        }

        public void SetupAfterFileLoad()
        {
            if (this.DesignMode)
            {

                layoutTree.root.DoBroadcastToHierElements(pnode =>
                {
                    ((TreeViewNodeModel)pnode).Expanded = true;
                    AppViewBase view = (AppViewBase)((TreeViewNodeModel)pnode).Item;
                    if (view is not null)
                    {
                        SetDesigneToolsActive(view);
                    }

                });

                AttachLayoutTreeRemovalToHierarchy();
                ActivateDragAndDrop();
            }
        }

        protected override void OnFileOpened(string filename)
        {
            base.OnFileOpened(filename);
            this.xmlView.Trace();
            SetupAfterFileLoad();
            RefreshDesignerAfterXmlLoad();
            SetDesignerStatus($"Открыт файл: {filename}");
        }

        /// <summary>Корень разметки из XML: <c>xmlView.Controls[0]</c>.</summary>
        public AppViewBase GetLayoutRootPane()
        {
            if (layoutPane?.Controls is null || layoutPane.Controls.Count == 0)
                return null;
            return layoutPane.Controls[0] as AppViewBase;
        }

        public void ApplyDesignModeToLayoutRoot(bool state)
        {
            var droppable = GetLayoutRootPane();
            if (droppable is null)
                return;
            droppable.ResolveDesignMode = true;
            droppable.DesignMode = state;
        }

        /// <summary>
        /// Увеличивает ширину окна до появления левой панели конструктора, чтобы область разметки XML сохранила прежнюю ширину.
        /// </summary>
        public void WidenDesignerHostForLeftChrome(int delta)
        {
            if (delta <= 0 || _designerHostWidthDelta != 0)
                return;
            var sync = controlView.ToControl();
            if (sync is null || sync.IsDisposed)
                return;
            void widen()
            {
                var form = sync.GetAppForm();
                if (form is null || form.IsDisposed || !form.IsHandleCreated)
                    return;
                try
                {
                    var minW = Math.Max(100, form.MinimumSize.Width);
                    form.Width = Math.Max(minW, form.Width + delta);
                    _designerHostWidthDelta = delta;
                }
                catch (InvalidOperationException)
                {
                }
            }
            try
            {
                if (sync.InvokeRequired)
                    sync.Invoke(widen);
                else
                    widen();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        /// <summary>Сужает окно после снятия левой панели на величину, добавленную в <see cref="WidenDesignerHostForLeftChrome"/>.</summary>
        public void RestoreDesignerHostAfterLeftChrome()
        {
            var delta = _designerHostWidthDelta;
            _designerHostWidthDelta = 0;
            if (delta <= 0)
                return;
            var sync = controlView.ToControl();
            if (sync is null || sync.IsDisposed)
                return;
            void shrink()
            {
                var form = sync.GetAppForm();
                if (form is null || form.IsDisposed || !form.IsHandleCreated)
                    return;
                try
                {
                    var minW = Math.Max(100, form.MinimumSize.Width);
                    form.Width = Math.Max(minW, form.Width - delta);
                }
                catch (InvalidOperationException)
                {
                }
            }
            try
            {
                if (sync.InvokeRequired)
                    sync.Invoke(shrink);
                else
                    shrink();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public void ResetLayoutTreeRoot()
        {
            _layoutTreeContextMenuSyncAttached.Clear();
            layoutTree.root.Children.Clear();
            var droppable = GetLayoutRootPane();
            if (droppable is not null)
            {
                layoutTree.root.Title = droppable.GetTypeName();
                layoutTree.root.Item = droppable;
            }
            else
            {
                layoutTree.root.Title = "Каталог";
                layoutTree.root.Item = null;
            }
        }

        /// <summary>
        /// Дерево «Иерархия» по полному поддереву <see cref="GetLayoutRootPane"/>.
        /// </summary>
        public void SyncLayoutTreeFromPane()
        {
            ResetLayoutTreeRoot();
            var droppable = GetLayoutRootPane();
            if (droppable is null)
                return;
            if (DesignMode)
                AttachLayoutTreeNodeContextMenuSync(layoutTree.root, droppable);
            ForEachLayoutView(droppable, UpdateToDesignMode);
        }

        public void EnsureDesignerTabs()
        {
            InitDesignerTools();
            if (leftTabs is not null)
            {
                WireElementsPaletteDrag();
                return;
            }
            this.elementsTree = CreateElementsPalette();
            this.leftTabs = new TabsView();
            this.leftTabs.Parent = this;
            this.leftTabs.AddTab("Элементы", this.elementsTree);
            this.leftTabs.AddTab("Фокус", focusedListView);
            this.datasourcesTree = CreateDatasourcesTreeInstance();
            this.leftTabs.AddTab("Данные", this.datasourcesTree);
            WireElementsPaletteDrag();
            WireElementsPaletteSelectionDescription();
            layoutTree.Expand();
            this.leftTabs.AddTab("Иерархия", this.layoutTree);
            this.leftTabs.Width = DesignerLeftPanelWidth;
            this.leftTabs.MinWidth = DesignerLeftPanelWidth;
        }

        /// <summary>
        /// Палитра всех конкретных <see cref="AppViewBase"/> из <see cref="ServiceFactory.GetTypesExtended{T}"/> (как в <see cref="ConstructorView"/>).
        /// </summary>
        public TreeControlView CreateElementsPalette()
        {
            
            ServiceFactory.Get().AddTypes(typeof(AppViewBase).Assembly);
            var componentTypes = ServiceFactory.Get().GetTypesExtended<AppViewBase>()
                .Where(t => !t.GetAttributes().ContainsKey(nameof(NotInputAttribute)))
                .OrderBy(t => t.GetTypeName(), StringComparer.CurrentCultureIgnoreCase)
                .ToList();
            
            var hts = componentTypes.Where(t => t.IsExtendsFrom(typeof(Highchart)));
            componentTypes = componentTypes.Where(t => !t.IsExtendsFrom(typeof(Highchart))).ToList();
            hts = hts.Where(t => t.GetTypeName().EndsWith("View"));
            componentTypes.AddRange(hts);
            var tree = new TreeControlView
            {
                root = new TreeViewNodeModel
                {
                    Title = "Компоненты представления",
                    OnClick = (p, e) => { 
                        
                    },
                    Children = new(componentTypes.Select(t => {
                        var pnode = new TreeViewNodeModel
                        {
                            Title = $"{t.GetTypeName()} / {t.GetLabel()}",
                            NodeName = t.GetTypeName(),
                            OnClick = (p, e) => {
                                try
                                {
                                    var view = t.New<AppViewBase>();
                                    this.layoutView.Set(view);

                                }
                                catch(Exception ex)
                                {
                                    this.Error(ex);
                                }
                            },
                            Item = t,
                            Icon = t.GetIcon()
                        };
                        pnode.ContextMenu.Items.Add(new() {
                            Label = "Информация"
                        });
                        return pnode;
                    }).ToList())
                }
            };
            tree.Searchable = true;
            return tree;
        }

        /// <summary>Текст drag — <see cref="TypeExtensions2.GetTypeName"/>; только для узлов с конкретным типом.</summary>
        public static string ResolveElementsPaletteDragText(object pnode)
        {
            if (pnode is not TreeViewNodeModel tvm)
                return null;
            if (tvm.Item is Type t)
            {
                if (t.IsAbstract)
                    return null;
                return t.GetTypeName();
            }
            return tvm.NodeName;
        }

        public void WireElementsPaletteDrag()
        {
            if (elementsTree is null)
                return;
            elementsTree.Draggable = true;
            elementsTree.SetDraggable(pnode => ResolveElementsPaletteDragText(pnode));
        }

        /// <summary>AppViewContent`1
        /// При выделении узла на вкладке «Элементы» показывает snackbar с <see cref="DescriptionAttribute"/> типа представления.
        /// </summary>
        public void WireElementsPaletteSelectionDescription()
        {
            if (elementsTree is null)
                return;

            void TryWire()
            {
                if (_elementsPaletteSelectionWired)
                    return;
                if (elementsTree.ToControl() is not ITreeControl treeCtrl)
                    return;

                _elementsPaletteSelectionWired = true;
                treeCtrl.OnNodeSelected += OnElementsPaletteNodeSelected;
            }

            elementsTree.OnLoad += (_, _) => TryWire();
            RunOnUiThread(TryWire);
        }

        public void OnElementsPaletteNodeSelected(List<int> path)
        {
            if (path is null || path.Count == 0)
                return;

            try
            {
                var node = elementsTree.Find(path);
                var type = (Type)node?.Item;
                
                ShowElementsPaletteTypeDescription(type);
            }
            catch
            {
            }
        }

        public void ShowElementsPaletteTypeDescription(Type type)
        {
            if (type is null)
                return;

            var label = type.GetLabel();
            if (string.IsNullOrWhiteSpace(label))
                label = type.GetTypeName();

            var description = type.GetDescription();
            SetDesignerStatus(string.IsNullOrWhiteSpace(description)
                ? $"Палитра: {label}"
                : $"Палитра: {label} — {description}");

            if (string.IsNullOrWhiteSpace(description))
                return;

            RunOnUiThread(() =>
            {
                var form = ToControl()?.GetAppForm();
                form?.ShowSnakBar(description);
            });
        }

        /// <summary>
        /// После загрузки/перезагрузки XML в <see cref="AppProgramView.xmlView"/> — вкладки и дерево.
        /// </summary>
        public void RefreshDesignerAfterXmlLoad()
        {
            if (xmlView?.Controls is null || xmlView.Controls.Count == 0)
                return;
            _layoutTreeRemovalSubscribed.Clear();
            _designerDropSurfaceConfigured.Clear();
            _designerChildAddedSubscribed.Clear();
            _designToolsApplied.Clear();
            EnsureDesignerTabs();
            SyncLayoutTreeFromPane();
            TryRefreshWsdlDatasourcesBranch();
            if (DesignMode)
            {
                AttachLayoutTreeRemovalToHierarchy();
                ActivateDragAndDrop();
                ForEachLayoutView(GetLayoutRootPane(), pn =>
                {
                    if (pn is AppViewBase av)
                        DesignerViewSizeHelper.EnsureVisibleInDesigner(av);
                });
            }
            SetDesignerStatus("Разметка обновлена из XML");
        }

        /// <summary>
        /// Не пишем в <see cref="AppViewRoot.layoutView"/> — только в <see cref="AppProgramView.xmlView"/>, иначе конструктор «не видит» разметку.
        /// </summary>
        public override AppViewBase LoadXmlText(string xml)
        {
            this.xmlText = xml ?? "";
            this.xmlView.FromXml(this.xmlText);
            this.xmlView.Trace();
            RefreshDesignerAfterXmlLoad();
            return GetLayoutRootPane();
        }

      

        public override void OnFileChanged(string changedPath, string text)
        {
            base.OnFileChanged(changedPath, text);
            RefreshDesignerAfterXmlLoad();
        }

        public void SetDesignerModeView(bool active)
        {
            if (active)
            {
                SetDesignerStatus("Панель конструктора: показана");
                SetupFocusControl();
                EnsureDesignerTabs();
                SyncFocusedListFromFocusedViews();

                // Левая панель (вкладки + layoutTree) уменьшает клиентскую область NavView — сдвигаются абсолютные координаты из XML.
                // Расширяем форму на ширину панели до привязки Left, при выходе из режима возвращаем ширину.
                WidenDesignerHostForLeftChrome(DesignerLeftPanelWidth);
                this.controlView.Left = this.leftTabs;
                this.controlView.Left.Width = 480;
                this.rollbackDesignTools.Add(() =>
                {
                    this.controlView.Left = null;
                    RestoreDesignerHostAfterLeftChrome();
                });
                
                TryRefreshWsdlDatasourcesBranch();
                _layoutTreeRemovalSubscribed.Clear();
                SyncLayoutTreeFromPane();
                AttachLayoutTreeRemovalToHierarchy();
                //AttachDesignerDroppableToHierarchy();
                
                this.designerToolsView.MinHeight = DesignerToolbarHeight;
                this.designerToolsView.Height = DesignerToolbarHeight;
                this.controlView.Top = this.designerToolsView;
                this.controlView.TopHeight = DesignerToolbarHeight;
                ApplyDesignerNavShellLayout();
                SyncDesignerCanvasTools();
                rollbackDesignTools.Add(() =>
                {
                    ClearDesignerCanvasToolBindings();
                    if (this.controlView is not null)
                    {
                        this.controlView.Top = null;
                        if (this.controlView.Left is not null)
                            this.controlView.Left.Width = 0;
                        this.controlView.Left = null;
                    }
                });
            }
            else
            {
                SetDesignerStatus("Панель конструктора: скрыта");
                ClearDesignerCanvasToolBindings();
                _activeCanvasTool = DesignerCanvasTool.None;
                foreach (var todo in rollbackDesignTools)
                    todo();
                rollbackDesignTools.Clear();
            }
        }

        /// <summary><see cref="SetDroppable"/> и обновление layoutTree на каждом уровне, в т.ч. после загрузки XML.</summary>
        public void ActivateDragAndDrop()
        {
            var root = GetLayoutRootPane();
            if (root is null)
                return;
            ForEachLayoutView(root, pn =>
            {
                if (pn is AppViewBase av)
                    SetupDesigner(av);
            });
            RefreshLayoutDropAllow();
        }

        public void RefreshLayoutDropAllow()
        {
            var root = GetLayoutRootPane();
            if (root is null)
                return;
            ForEachLayoutView(root, pn =>
            {
                if (pn is AppViewBase av && av.Dropable)
                    EnsureViewAllowDrop(av);
            });
        }

        public void EnsureViewAllowDrop(AppViewBase view)
        {
            if (view is null || !view.Dropable)
                return;
            try
            {
                var ctrl = view.ToControl();
                if (ctrl is null || ctrl.IsDisposed)
                    return;
                ctrl.AllowDrop = true;
            }
            catch
            {
            }
        }

        public void ActivateContextMenu()
        {
            var root = GetLayoutRootPane();
            if (root is null)
                return;
            ForEachLayoutView(root, pn =>
            {
                if (pn is AppViewBase av)
                    SetDesigneToolsActive(av);
            });
        }

        public void SetupDesigner(AppViewBase droppablePane)
        {
            if (droppablePane is null)
                return;

            AppViewBase ptarget = null;
            bool dropped = false;

            void OnDesignerChildAdded(int _, object p)
            {
                if (p is not AppViewBase pnode)
                    return;
                try
                {
                    DesignerViewSizeHelper.EnsureVisibleInDesigner(pnode);
                    EnsureLayoutTreeNodeForView(pnode);
                    SetupDesigner(pnode);
                }
                catch (Exception ex)
                {
                    this.Error(ex);
                }
            }

            if (_designerChildAddedSubscribed.Add(droppablePane))
            {
                droppablePane.Controls.OnAdded(OnDesignerChildAdded);
                if (droppablePane is MenuView menuDropTarget)
                    menuDropTarget.MenuItems.OnAdded(OnDesignerChildAdded);
            }
            _designerDropSurfaceConfigured.Add(droppablePane);

            droppablePane.SetDroppable((droppedView, x, y) =>
                {
                    try
                    {
                        dropped = true;
                        if (droppedView is AppViewBase av)
                        {
                            av.X = x;
                            av.Y = y;
                            av.DesignMode = DesignMode;
                            DesignerViewSizeHelper.EnsureVisibleInDesigner(av);
                            if (DesignMode)
                                SetDesigneToolsActive(av);
                            else
                                ApplyDesignerContextMenuToControl(av);
                            EnsureLayoutTreeNodeForView(av);
                            SetDesignerStatus($"Добавлено: {av.GetTypeName()} ({av.X}, {av.Y})");
                        }
                        ptarget = null;
                    }
                    catch (Exception ex)
                    {
                        this.Error(ex);
                    }
                },
                (text) => CreateDesignerDragPreview(droppablePane, ref ptarget, text),
                (removed) =>
                {
                    if (dropped)
                    {
                        ptarget = null;
                        dropped = false;
                        return;
                    }
                    if (removed is AppViewBase child)
                    {
                        DesignerRemoveChild(droppablePane, child);
                        SetDesignerStatus($"Отмена добавления: {child.GetTypeName()}");
                    }
                    else if (ptarget is not null)
                    {
                        DesignerRemoveChild(droppablePane, ptarget);
                        SetDesignerStatus($"Отмена добавления: {ptarget.GetTypeName()}");
                    }
                    ptarget = null;
                },
                (p, x, y) =>
                {
                    if (dropped)
                        return;
                    if (p is AppViewBase av)
                    {
                        av.X = x;
                        av.Y = y;
                        return;
                    }
                    if (ptarget is not null)
                    {
                        ptarget.X = x;
                        ptarget.Y = y;
                    }
                });
            EnsureViewAllowDrop(droppablePane);
        }

        /// <summary>Создаёт превью при перетаскивании типа с палитры «Элементы» на разметку.</summary>
        public object CreateDesignerDragPreview(AppViewBase droppablePane, ref AppViewBase ptarget, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            Type ptype;
            try
            {
                ptype = text.ToType();
            }
            catch (Exception ex)
            {
                SetDesignerStatus($"Тип не найден: {text}");
                this.Error(ex);
                return null;
            }

            SetDesignerStatus($"Перетаскивание: {text}");

            if (ptarget is not null && ptarget.GetTypeName() == text)
                return ptarget;

            if (ptype.IsExtendedFrom(nameof(AppViewBase)) || ptype.IsExtends(typeof(AppViewBase)))
            {
                AppViewBase created;
                if (ptype.IsGenericTypeDefinition)
                {
                    var typeArg = ptype.GetGenericArguments()[0];
                    var argBase = typeArg.BaseType ?? typeof(object);
                    created = new AppViewBase { MinHeight = 320, MinWidth = 25 };
                    created.CreateSelectList("Выберите тип",
                        ServiceFactory.Get().GetTypesExtended(argBase).Select(t => t.GetTypeName()).ToList(),
                        arg =>
                        {
                            var pview = ptype.MakeGenericType(arg.ToType()).New<AppViewBase>();
                            created.LayoutFill(pview);
                        });
                }
                else
                {
                    created = ptype.New<AppViewBase>();
                }

                created.Dock = "None";
                DesignerViewSizeHelper.EnsureVisibleInDesigner(created);
                if (DesignMode)
                {
                    SetupDesigner(created);
                    SetDesigneToolsActive(created);
                }

                if (!DesignerContainsChild(droppablePane, created))
                    droppablePane.Add(created);
                created.MinWidth = created.MinWidth<50?50: created.MinWidth;
                created.MinHeight = created.MinHeight < 50?50: created.MinHeight;
                
                created.BorderWidth = 1;
                ptarget = created;
                return created;
            }

            var container = new ContainerView { Service = ptype.New() };
            if (!droppablePane.Controls.Contains(container))
                droppablePane.Controls.Add(container);
            ptarget = container;
            return container;
        }

        public void SetupFocusControl()
        {
        }

        public bool IsDesignerChromeView(ViewItem view)
        {
            if (view is null)
                return true;
            if (ReferenceEquals(view, focusedListView))
                return true;
            if (datasourcesTree is not null && ReferenceEquals(view, datasourcesTree))
                return true;
            if (layoutTree is not null && ReferenceEquals(view, layoutTree))
                return true;
            if (elementsTree is not null && ReferenceEquals(view, elementsTree))
                return true;
            if (leftTabs is not null && ReferenceEquals(view, leftTabs))
                return true;
            return false;
        }


        public ViewTool CurrentTool = null;

        /// <summary>Смена инструмента панели: деактивирует старый на фокусе и активирует новый.</summary>
        public void SwitchCurrentTool(ViewTool next)
        {
            var focused = FocusedViews.OfType<ViewItem>()
                .Where(v => v is not null && !IsDesignerChromeView(v))
                .ToList();

            if (CurrentTool is not null)
            {
                foreach (var v in focused)
                    CurrentTool.Deactivate(v);
            }

            CurrentTool = next;

            if (CurrentTool is not null)
            {
                foreach (var v in focused)
                    CurrentTool.Activates(v);
            }
        }

        public void OnDesignerFocusedViewsChanged(object sender, IChangingListEvent evt)
        {
            var view = evt.Target.Item as ViewItem;
            if (view is not null && !IsDesignerChromeView(view))
            {
                switch (evt.Target.Action)
                {
                    case "added":
                        focusedListView.Options.Add(new Option
                        {
                            Item = view,
                            Value = view.GetTypeName()
                        });
                        SetDesignerStatus("Фокус: " + view.GetTypeName());
                        if (CurrentTool is not null)
                        {
                            CurrentTool.Activates(view);
                        }
                        break;
                    case "removed":
                        var option = focusedListView.Options.FirstOrDefault(o => ReferenceEquals(o.Item, view));
                        if (option is not null)
                            focusedListView.Options.Remove(option);
                        SetDesignerStatus("Снят фокус: " + view.GetTypeName());
                        if (CurrentTool is not null)
                        {
                            CurrentTool.Deactivate(view);
                        }
                        break;
                }
            }

            if (DesignMode)
                SyncDesignerCanvasTools();
            
        }

        /// <summary>
        /// Высота Top/ширина Left на модели не всегда доходят до WinForms (<see cref="ComponentsFactory.BindSides"/> без слушателей TopHeight/LeftWidth).
        /// Принудительно выставляем размеры у docked-детей <see cref="PanelsControl"/>.
        /// </summary>
        public void ApplyDesignerNavShellLayout()
        {
            //    this.controlView.RunOnUiThread(() =>
            //    {
            //        if (this.controlView?.ToControl() is not IPanelsControl pc)
            //            return;
            //        foreach (Control ch in pc.Controls)
            //        {
            //            if (ch.Dock == DockStyle.Left && this.controlView.Left is not null)
            //                ch.Width = DesignerLeftPanelWidth;
            //            else if (ch.Dock == DockStyle.Top && this.controlView.Top is not null)
            //                ch.Height = DesignerToolbarHeight;
            //        }
            //        pc.PerformLayout();
            //    });
        }

        public void InitDesignerTools()
        {
            if (_designerToolsInitialized)
                return;
            _designerToolsInitialized = true;
            designerToolsView.CreateToolItemButton("Выбор", null, () => SetActiveCanvasTool(DesignerCanvasTool.None));
            designerToolsView.CreateToolItemButton("Перемещение", null, () => SetActiveCanvasTool(DesignerCanvasTool.Move));
            designerToolsView.CreateToolItemButton("Размер", null, () => SetActiveCanvasTool(DesignerCanvasTool.Resize));
        }

        public void SetActiveCanvasTool(DesignerCanvasTool tool)
        {
            _activeCanvasTool = tool;
            SetDesignerStatus(tool switch
            {
                DesignerCanvasTool.Move => "Инструмент: перемещение (элементы в фокусе)",
                DesignerCanvasTool.Resize => "Инструмент: размер (элементы в фокусе)",
                _ => "Инструмент: выбор"
            });
            SyncDesignerCanvasTools();
        }

        /// <summary>Снимает/вешает обработчики перемещения и ресайза в соответствии с <see cref="_activeCanvasTool"/> и <see cref="FocusedViews"/>.</summary>
        public void SyncDesignerCanvasTools()
        {
            if (!DesignMode)
                return;

            var focused = FocusedViews.OfType<AppViewBase>()
                .Select(AppViewBase.ResolveDesignerFocusTarget)
                .Where(v => !IsDesignerChromeView(v))
                .Distinct()
                .ToList();

            foreach (var v in _moveToolUnbind.Keys.ToList())
            {
                if (_activeCanvasTool != DesignerCanvasTool.Move || !focused.Contains(v))
                    RemoveMoveTool(v);
            }

            foreach (var v in _resizeToolUnbind.Keys.ToList())
            {
                if (_activeCanvasTool != DesignerCanvasTool.Resize || !focused.Contains(v))
                    RemoveResizeTool(v);
            }

            if (_activeCanvasTool == DesignerCanvasTool.Move)
            {
                foreach (var v in focused)
                    TryAttachMoveTool(v);
            }

            if (_activeCanvasTool == DesignerCanvasTool.Resize)
            {
                foreach (var v in focused)
                    TryAttachResizeTool(v);
            }
        }

        public void ClearDesignerCanvasToolBindings()
        {
            foreach (var v in _moveToolUnbind.Keys.ToList())
                RemoveMoveTool(v);
            foreach (var v in _resizeToolUnbind.Keys.ToList())
                RemoveResizeTool(v);
        }

        public void RemoveMoveTool(AppViewBase view)
        {
            if (view is not null && _moveToolUnbind.TryGetValue(view, out var unbind))
            {
                unbind();
                _moveToolUnbind.Remove(view);
            }
        }

        public void RemoveResizeTool(AppViewBase view)
        {
            if (view is not null && _resizeToolUnbind.TryGetValue(view, out var unbind))
            {
                unbind();
                _resizeToolUnbind.Remove(view);
            }
        }

        public void TryAttachMoveTool(AppViewBase view)
        {
            if (view is null || _moveToolUnbind.ContainsKey(view))
                return;

            bool dragging = false;
            Point anchorParent = default;
            int startX = 0;
            int startY = 0;

            Action<object, MouseEventArgs> onMouseDown = (s, e) =>
            {
                //if (e.Button != MouseButtons.Left)
                //    return;
                //var control = view.ToControl();
                //if (control?.Parent is null)
                //    return;
                //dragging = true;
                //control.Capture = true;
                //view.Dock = "None";
                //anchorParent = control.Parent.PointToClient(Control.MousePosition);
                //startX = view.X;
                //startY = view.Y;
            };
            Action<object, MouseEventArgs> onMouseMove = (s, e) =>
            {
                //if (!dragging)
                //    return;
                //var control = view.ToControl();
                //if (control?.Parent is null)
                //    return;
                //var cur = control.Parent.PointToClient(Control.MousePosition);
                //view.X = startX + (cur.X - anchorParent.X);
                //view.Y = startY + (cur.Y - anchorParent.Y);
            };
            Action<object, MouseEventArgs> onMouseUp = (s, e) =>
            {
                if (!dragging)
                    return;
                dragging = false;
                var control = view.ToControl();
                if (control is not null)
                    control.Capture = false;
                SetDesignerStatus($"Перемещено: {view.GetTypeName()} ({view.X}, {view.Y})");
            };
            Action<object, EventArgs> onMouseLeave = (s, e) =>
            {
                if (dragging)
                    return;
                var control = view.ToControl();
                if (control is not null)
                    control.Cursor = Cursors.Default;
            };

            var subscription = new DesignerViewSubtreeEvents.Subscription();
            subscription.Wire(view, onMouseDown, onMouseMove, onMouseUp, onMouseLeave);

            _moveToolUnbind[view] = () =>
            {
                subscription.Unwire(onMouseDown, onMouseMove, onMouseUp, onMouseLeave);
                var control = view.ToControl();
                if (control is not null)
                {
                    control.Capture = false;
                    control.Cursor = WinFormsApp.API.DesignerResizeTool.Cursors.Default;
                }
            };
        }

        public void TryAttachResizeTool(AppViewBase view)
        {
            if (view is null || _resizeToolUnbind.ContainsKey(view))
                return;

            bool resizing = false;
            int startWidth = 0;
            int startHeight = 0;
            int startX = 0;
            int startY = 0;
            Point resizeAnchorParent = default;
            bool savedAutoSize = false;
            // SE-only для canvas-инструмента «Размер» (полный набор краёв — в DesignerResizeTool).
            Action<object, MouseEventArgs> onMouseDown = (s, e) =>
            {
                //var control = view.ToControl();
                //if (control is null || control.Parent is null)
                //    return;
                //var client = control.PointToClient(Control.MousePosition);
                //bool isHandle =
                //    client.X >= control.Width - DesignerResizeHandleSize &&
                //    client.Y >= control.Height - DesignerResizeHandleSize;
                //if (e.Button != MouseButtons.Left || !isHandle)
                //    return;
                //startWidth = Math.Max(1, view.Width);
                //startHeight = Math.Max(1, view.Height);
                //startX = view.X;
                //startY = view.Y;
                //resizeAnchorParent = control.Parent.PointToClient(Control.MousePosition);
                //view.Dock = "None";
                //savedAutoSize = view.AutoSize;
                //view.AutoSize = false;
                //resizing = true;
                //control.Capture = true;
            };
            Action<object, MouseEventArgs> onMouseMove = (s, e) =>
            {
                var control = view.ToControl();
                if (control is null)
                    return;
                //var client = control.PointToClient(Control.MousePosition);
                //bool isHandle =
                //    client.X >= control.Width - DesignerResizeHandleSize &&
                //    client.Y >= control.Height - DesignerResizeHandleSize;
                //control.Cursor = resizing || isHandle ? Cursors.SizeNWSE : Cursors.Default;
                //if (!resizing)
                //    return;
                //if (control.Parent is null)
                //    return;
                //var cur = control.Parent.PointToClient(Control.MousePosition);
                //int width = Math.Max(40, Math.Max(view.MinWidth, startWidth + (cur.X - resizeAnchorParent.X)));
                //int height = Math.Max(25, Math.Max(view.MinHeight, startHeight + (cur.Y - resizeAnchorParent.Y)));
                //view.X = startX;
                //view.Y = startY;
                //view.Width = width;
                //view.Height = height;
                //control.AutoSize = false;
                ////if (control is ControlBase cb)
                ////    cb.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                //control.SetBounds(startX, startY, width, height);
            };
            Action<object, MouseEventArgs> onMouseUp = (s, e) =>
            {
                if (!resizing)
                    return;
                resizing = false;
                view.AutoSize = savedAutoSize;
                var control = view.ToControl();
                if (control is not null)
                {
                    control.Capture = false;
                    control.Cursor = Cursors.Default;
                }
                SetDesignerStatus($"Размер: {view.GetTypeName()} ({view.Width}×{view.Height})");
            };
            Action<object, EventArgs> onMouseLeave = (s, e) =>
            {
                if (!resizing)
                {
                    var control = view.ToControl();
                    if (control is not null)
                        control.Cursor = Cursors.Default;
                }
            };

            var subscription = new DesignerViewSubtreeEvents.Subscription();
            subscription.Wire(view, onMouseDown, onMouseMove, onMouseUp, onMouseLeave);

            _resizeToolUnbind[view] = () =>
            {
                subscription.Unwire(onMouseDown, onMouseMove, onMouseUp, onMouseLeave);
                var control = view.ToControl();
                if (control is not null)
                {
                    control.Capture = false;
                    control.Cursor = Cursors.Default;
                }
            };
        }

        /// <summary>При открытии режима проектирования подтягиваем уже выделенные элементы.</summary>
        public void SyncFocusedListFromFocusedViews()
        {
            focusedListView.Options.Clear();
            foreach (var v in FocusedViews)
            {
                if (v is null || IsDesignerChromeView(v))
                    continue;
                focusedListView.Options.Add(new Option
                {
                    Item = v,
                    Value = v.GetTypeName()
                });
            }
        }

        /// <summary>Дерево источников данных; WCF/SOAP наполняется из БД <see cref="ServiceBuilderDbContext"/>.</summary>
        public TreeControlView CreateDatasourcesTreeInstance()
        {
            apiServices = DesignerDatasourcesTreeNodes.GetWebServicesTree();
            dbContexts = DesignerDatasourcesTreeNodes.GetDbContextTree();
            adods = DesignerDatasourcesTreeNodes.GetAdoDatasourcesTree();
            odbcds = DesignerDatasourcesTreeNodes.GetOdbcDatasourcesTree();
            var tree = new TreeControlView
            {
                root = new TreeViewNodeModel
                {
                    Title = "Источники данных",
                    Children = new()
                    {
                        BuildWsdlServicesCategoryNode(),
                        this.apiServices,
                        dbContexts,
                        adods,
                        odbcds
                    }
                }
            };
            tree.SetDraggable(pnode =>
            {
                if (pnode is TreeViewNodeModel tvm)
                    return tvm.NodeName ?? tvm.Title;
                return null;
            });
            return tree;
        }

        public TreeViewNodeModel BuildWsdlServicesCategoryNode()
        {
            var root = new TreeViewNodeModel
            {
                Title = "WCf/SOAP сервисы",
                NodeName = "wsdl-root"
            };
            root.ContextMenu.Items.Add(new ContextView
            {
                Label = "Добавить WSDL…",
                OnClick = (s, e) => OnAddWsdlRequested(root)
            });
            _wsdlDatasourcesCategoryNode = root;
            RefreshWsdlDatasourcesChildren(root);
            return root;
        }

        public void RefreshWsdlDatasourcesChildren(TreeViewNodeModel wsdlCategory)
        {
            foreach (var ch in wsdlCategory.Children.ToList())
                wsdlCategory.Children.Remove(ch);

            foreach (var svc in SoapServiceBuilder.Instance.ListRegisteredWsdlServices())
            {
                wsdlCategory.Children.Add(new TreeViewNodeModel
                {
                    Title = string.IsNullOrEmpty(svc.DisplayName) ? svc.WsdlUrl : svc.DisplayName,
                    NodeName = svc.WsdlUrl,
                    Item = svc.Id
                });
            }
        }

        public void OnAddWsdlRequested(TreeViewNodeModel wsdlCategory)
        {
            var owner = this.ToControl()?.GetAppForm();
            if (!TryGetWsdlUrl(owner, out var url))
                return;
            try
            {
                SoapServiceBuilder.Instance.RegisterFromWsdlUrl((string)url);
                RefreshWsdlDatasourcesChildren(wsdlCategory);
                SetDesignerStatus("WSDL зарегистрирован: " + url);
            }
            catch (Exception ex)
            {
                this.ShowSubmitMessageDialog(owner, ex.Message, "Ошибка регистрации WSDL", "OK", "Warning");
            }
        }

        public void ShowSubmitMessageDialog(IControlBase owner, string message, string v1, string v2, string v3)
        {
            throw new NotImplementedException();
        }

        public bool TryGetWsdlUrl(IControlBase owner, out object url)
        {
            throw new NotImplementedException();
        }

        public void TryRefreshWsdlDatasourcesBranch()
        {
            if (_wsdlDatasourcesCategoryNode is not null)
                RefreshWsdlDatasourcesChildren(_wsdlDatasourcesCategoryNode);
        }

        public void UpdateToDesignMode(ViewNode pnode)
        {
            var droppable = GetLayoutRootPane();
            if (droppable is not null && ReferenceEquals(pnode, droppable))
            {
                if (DesignMode)
                {
                    AttachLayoutTreeNodeContextMenuSync(layoutTree.root, droppable);
                    CreateDesignControls(pnode);
                }
                return;
            }


            if(pnode is MenuView mv)
            {
                this.Info(mv.Text);
                
            }
            TreeViewNodeModel parent = GetParentLayoutNode(pnode);
            if(parent is null)
                return;
            if (LayoutTreeHasNodeForItem(parent, pnode))
            {
                if (DesignMode)
                {
                    var existingNode = FindLayoutTreeNodeForView((AppViewBase)pnode);
                    if (existingNode is not null)
                        AttachLayoutTreeNodeContextMenuSync(existingNode, (AppViewBase)pnode);
                    CreateDesignControls(pnode);
                }
                EnsureLayoutTreeRemovalHookForSubtree((AppViewBase)pnode);
                return;
            }
            TreeViewNodeModel viewNode = new TreeViewNodeModel()
            {
                Title = pnode.GetTypeName(),
                Item = pnode
            };
            AttachLayoutTreeNodeContextMenuSync(viewNode, (AppViewBase)pnode);
            parent.AppendTreeViewNode(viewNode);
            if (DesignMode)
                CreateDesignControls(pnode);
            EnsureLayoutTreeRemovalHookForSubtree((AppViewBase)pnode);
        }

        /// <summary>
        /// Обход разметки: <see cref="ViewNode.Controls"/> и <see cref="MenuView.MenuItems"/> (дочерние пункты не всегда видны только через Controls при загрузке XML).
        /// </summary>
        public static void ForEachLayoutView(AppViewBase root, Action<ViewNode> action)
        {
            if (root is null || action is null)
                return;

            void Visit(ViewNode node)
            {
                action(node);
                foreach (ViewNode child in node.Controls)
                    Visit(child);
                if (node is MenuView menu)
                {
                    foreach (var menuChild in menu.MenuItems)
                    {
                        if (menuChild is ViewNode vn && !node.Controls.Contains(menuChild))
                            Visit(vn);
                    }
                }
            }

            Visit(root);
        }

        /// <summary>
        /// Узел «Иерархия» для представления и всех предков (нужно для вложенных <see cref="MenuView"/>).
        /// </summary>
        public void EnsureLayoutTreeNodeForView(AppViewBase view)
        {
            if (view is null || layoutTree?.root is null)
                return;

            var layoutRoot = GetLayoutRootPane();
            if (layoutRoot is null)
                return;

            if (ReferenceEquals(view, layoutRoot))
            {
                layoutTree.root.Title = view.GetTypeName();
                layoutTree.root.Item = view;
                return;
            }

            if (FindLayoutTreeNodeForView(view) is not null)
                return;

            if (view.Parent is AppViewBase parentView && !ReferenceEquals(parentView, view))
                EnsureLayoutTreeNodeForView(parentView);

            if (FindLayoutTreeNodeForView(view) is not null)
                return;

            DesignerViewSizeHelper.EnsureVisibleInDesigner(view);
            UpdateToDesignMode(view);
        }

        public void CreateDesignControls(ViewNode pnode)
        {
            var view = ((AppViewBase)pnode);
        
            pnode.DesignMode = true;
           
        }



        /// <summary>
        /// Контекстное меню узла дерева «Иерархия» отражает меню связанного <paramref name="layoutView"/> и обновляется при его изменении.
        /// </summary>
        public void AttachLayoutTreeNodeContextMenuSync(TreeViewNodeModel treeNode, AppViewBase layoutView)
        {
            if (treeNode is null || layoutView is null)
                return;
            if (!_layoutTreeContextMenuSyncAttached.Add(treeNode))
                return;
            Action<ContextView, ChangingList<ContextView>> todo = null;
            todo = (menu, items) => {
                foreach(var item in items)
                {
                    var newItem = new ContextView()
                    {
                        Label = item.Label,
                        OnClick = (s, e) => {
                            item.OnClick(s,e);
                        }
                    };
                    todo(newItem, item.Items);
                    menu.Items.Add(newItem);
                }
                items.OnRemoved((i, p) => {
                    menu.Items.RemoveAt(i);
                });
                items.OnAdded((i, p) => {
                    var pitem = (ContextView)p;
                    var newItem = new ContextView()
                    {
                        Label = pitem.Label,
                        OnClick = (s, e) => {
                            pitem.OnClick(s, e);
                        }
                    };
                    todo(newItem, pitem.Items);
                    menu.Items.Add(newItem);
                });
            };
            todo(treeNode.ContextMenu, layoutView.ContextMenu.Items);
        }

        /// <summary>Узел <see cref="layoutTree"/>, у которого <see cref="TreeViewNodeModel.Item"/> совпадает с представлением разметки.</summary>
        public TreeViewNodeModel FindLayoutTreeNodeForView(AppViewBase view)
        {
            if (view is null || layoutTree?.root is null)
                return null;
            if (ReferenceEquals(layoutTree.root.Item, view))
                return layoutTree.root;
            TreeViewNodeModel found = null;
            layoutTree.root.DoBroadcastToHierElements(p =>
            {
                if (p is TreeViewNodeModel tvm && ReferenceEquals(tvm.Item, view))
                    found = tvm;
            });
            return found;
        }

        public TreeViewNodeModel GetParentLayoutNode(ViewNode pnode)
        {
            var layoutRoot = GetLayoutRootPane();
            if (layoutRoot is null)
                throw new InvalidOperationException("Нет корневого представления в xmlView (загрузите XML).");

            TreeViewNodeModel parentNode = null;

            if (ReferenceEquals(pnode.Parent, layoutRoot))
            {
                parentNode = layoutTree.root;
            }
            else if (pnode.Parent is not null)
            {
                layoutTree.root.DoBroadcastToHierElements(p =>
                {
                    if (p is TreeViewNodeModel tvm && ReferenceEquals(tvm.Item, pnode.Parent))
                        parentNode = tvm;
                });
            }

            if (parentNode is null)
            {
                ((AppViewBase)pnode).Trace();
            }
                //throw new InvalidOperationException("Не найден родитель в layoutTree для " + pnode.GetTypeName());
            return parentNode;
        }

        /// <summary>Дочернее представление уже в коллекции цели drop (для MenuView — <see cref="MenuView.MenuItems"/>, не <see cref="ViewItem.Controls"/>).</summary>
        public static bool DesignerContainsChild(AppViewBase parent, AppViewBase child)
        {
            if (parent is null || child is null)
                return false;
            if (parent is MenuView menu)
                return menu.MenuItems.Contains(child);
            return parent.Controls.Contains(child);
        }

        public static void DesignerRemoveChild(AppViewBase parent, AppViewBase child)
        {
            if (parent is null || child is null)
                return;
            if (parent is MenuView menu && child is MenuView menuChild)
                menu.Remove(menuChild);
            else
                parent.Controls.Remove(child);
        }

        /// <summary>Узел для этого представления уже есть у родителя (защита от двойного Add при DnD + синхронизации).</summary>
        public static bool LayoutTreeHasNodeForItem(TreeViewNodeModel parent, ViewNode item)
        {
            foreach (var c in parent.Children)
            {
                if (ReferenceEquals(c.Item, item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Удаляет узел «Иерархии» по ссылке на представление. Не опирается на <see cref="ViewNode.Parent"/>:
        /// при <c>Controls.Remove</c> сначала срабатывает <c>OnRemovedChild</c> и обнуляет родителя.
        /// </summary>
        public void RemoveLayoutTreeNodeForView(AppViewBase removed)
        {
            if (removed is null)
                return;
            TreeViewNodeModel model = null;
            layoutTree.root.DoBroadcastToHierElements(p =>
            {
                if (p is TreeViewNodeModel tvm && ReferenceEquals(tvm.Item, removed))
                    model = tvm;
            });
            if (model is null || ReferenceEquals(model, layoutTree.root))
                return;
            var parent = model.ParentNode;
            if (parent is not null)
                parent.Children.Remove(model);
        }

        /// <summary>Подписка на удаление дочерних представлений у каждого узла разметки (не только у корня).</summary>
        public void AttachLayoutTreeRemovalToHierarchy()
        {
            var root = GetLayoutRootPane();
            if (root is null)
                return;
            EnsureLayoutTreeRemovalHookForSubtree(root);
        }

        /// <summary>Новые ветви (DnD, XML) должны получать те же <c>Controls.OnRemoved</c>, что и при полном attach.</summary>
        public void EnsureLayoutTreeRemovalHookForSubtree(AppViewBase subtreeRoot)
        {
            if (subtreeRoot is null)
                return;
            ForEachLayoutView(subtreeRoot, pn =>
            {
                if (pn is not AppViewBase av)
                    return;
                if (!_layoutTreeRemovalSubscribed.Add(av))
                    return;
                av.Controls.OnRemoved((i, p) =>
                {
                    if (p is AppViewBase removed)
                        RemoveLayoutTreeNodeForView(removed);
                });
                if (av is MenuView menuView)
                {
                    menuView.MenuItems.OnRemoved((i, p) =>
                    {
                        if (p is AppViewBase removed)
                            RemoveLayoutTreeNodeForView(removed);
                    });
                }
            });
        }


        public void ActivateDesignMode( )  
        {
            this.layoutPane.Top = toolsView;
            this.rollbackDesigners.Add(() => {
                this.layoutPane.Top = null;
            });
            this.Top = MenuView;
            this.rollbackDesigners.Add(() => {
                this.Top = null;
            });
            layoutTree.root.DoBroadcastToHierElements(pnode =>
            {
             
                AppViewBase view = (AppViewBase)((TreeViewNodeModel)pnode).Item;
                if(view is not null)
                {
                    SetDesigneToolsActive(view);

                }

            });
            AttachLayoutTreeRemovalToHierarchy();
            ActivateDragAndDrop();
        }

        public static void ApplyDesignerContextMenuToControl(AppViewBase view)
        {
            if (view is null)
                return;
            try
            {
                if (view.ToControl() is not IControlBase host)
                    return;
                AppViewBase.componentsFactory.BindContextMenu(view, host);
            }
            catch
            {
            }
        }

        public void SetDesigneToolsActive(AppViewBase view)
        {
            if (view is null)
                return;
            if (!_designToolsApplied.Add(view))
            {
                ApplyDesignerContextMenuToControl(view);
                return;
            }

            var context = view.ContextMenu;
            rollbackDesignTools.Add(view.AddContextRadioGroup(nameof(ViewItem.Dock), view));
            rollbackDesignTools.Add(view.ContextMenuBuilder.AddServiceMethods<ViewFactory>(view.GetLabel(), view));
            
            
            var ctx = new ContextView()
            {
                Label = "Удалить",
                OnClick = (s, e) =>
                {
                    SetDesignerStatus($"Удаление: {view.GetTypeName()}");
                    view.RemoveFromParent();
                }
            };
            context.Items.Add(ctx);
            rollbackDesignTools.Add(() =>
            {
                context.Items.Remove(ctx);
            });

            var ctxOptions = new ContextView()
            {
                Label = "Свойства",
                OnClick = (s, e) =>
                {
                    SetDesignerStatus($"Свойства: {view.GetTypeName()}");
                    this.controlView.Right = EditView.CreateEditView(view, view.GetType().GetInputPropertiesSorted());
                    this.controlView.Right.MinWidth = 600;
                    rollbackDesignTools.Add(() =>
                    {
                        this.controlView.Right = null;
                    });
                }
            };
            var configureEventsCtx = new ContextView()
            {
                Label = "События",
                OnClick = (s, e) =>
                {
                    var eventsBindingView = new EventsBindingsEditorView();
                    eventsBindingView.Bind(view); ;
                    this.controlView.Right = eventsBindingView;
                    this.controlView.Right.MinWidth = 600;
                    rollbackDesignTools.Add(() =>
                    {
                        this.controlView.Right = null;
                    });
                }
            };
            context.Items.Add(ctxOptions);
            context.Items.Add(configureEventsCtx);
            rollbackDesignTools.Add(() =>
            {
                context.Items.Remove(configureEventsCtx);
            }); 
            rollbackDesignTools.Add(() =>
            {
                context.Items.Remove(ctxOptions);
            });
            ApplyDesignerContextMenuToControl(view);
        }

        public void UpgradeView()
        {
            EnsureDesignerTabs();
            RefreshDesignerAfterXmlLoad();
        }

        public void OpenFile(string filepath)
        {
            this.OnFileOpened(filepath);
        }

        public void OpenKeyboardCustomizer()
        {
            RunOnUiThread(() =>
            {
                try
                {
                    //var customizer = new KeyboardCustomizer();
                    //customizer.commands.Clear();
                    //foreach (var cmd in CollectKeyboardCommands())
                    //    customizer.commands.Add(cmd);

                    //var form = customizer.ToBaseForm("Настройки клавиатуры");
                    //form.StartPosition = FormStartPosition.CenterScreen;
                    //var owner = ToControl()?.GetAppForm();
                    //if (owner is not null)
                    //{
                    //    form.StartPosition = FormStartPosition.CenterParent;
                    //    form.ShowDialog(owner);
                    //}
                    //else
                    //    form.ShowDialog();

                    //LoadKeyboardConfig();
                    //SetDesignerStatus("Клавиатура: конфигурация обновлена");
                }
                catch (Exception ex)
                {
                    SetDesignerStatus($"Ошибка настройки клавиатуры: {ex.Message}");
                    this.Error(ex);
                }
            });
        }

        public IEnumerable<string> DiscoveryAllCommands()
        {
            List<string> commands = new();
            this.ForEach(( pnode) => {
                
                
                commands.AddRange(pnode.GetCommands().Select(command => $"{pnode.NodeName}.{command}").ToList());
            });
            return commands;
        }

        public IEnumerable<string> CollectKeyboardCommands()
        {
            var set = new HashSet<string>(StringComparer.Ordinal);
            foreach (var cmd in LoadKeyboardConfig().Values)
                AddKeyboardCommand(set, cmd);
            /*foreach (var cmd in KeyboardConfigDefaults.CreateSample().Values)
                AddKeyboardCommand(set, cmd);

           
            
            AddKeyboardCommand(set, "Close()");
            AddKeyboardCommand(set, "Save()");
            AddKeyboardCommand(set, "OnSaveFile()");
            AddKeyboardCommand(set, "SetDesignMode(true)");
            AddKeyboardCommand(set, "SetDesignMode(false)");*/

            CollectMenuCommands(MenuView, set);
            return set.OrderBy(s => s, StringComparer.Ordinal);
        }

        public static void AddKeyboardCommand(HashSet<string> set, string? command)
        {
            if (!string.IsNullOrWhiteSpace(command))
                set.Add(command.Trim());
        }

        public static void CollectMenuCommands(MenuView? menu, HashSet<string> set)
        {
            if (menu is null)
                return;

            AddKeyboardCommand(set, menu.Command);
            foreach (var child in menu.MenuItems)
                CollectMenuCommands(child, set);
        }

        /// <summary>
        /// Добавляет в <see cref="MenuView"/> иерархию модульных тестов
        /// <see cref="TestingUnit"/> / <see cref="TestingElement"/> (корень — <see cref="ProgramUnit"/>).
        /// </summary>
        public void AddTestingHierarchyToMainMenu()
        {
            
            var rootUnit = new CommonModelUnit();
            var testsMenu = new MenuView { Text = "Тесты" };
            testsMenu.MenuItems.Add(CreateRunTestMenuItem("Запустить все", rootUnit));
            AppendTestingElementMenuItems(testsMenu, rootUnit);
            MenuView.MenuItems.Add(testsMenu);
            
        }

        public void AppendTestingElementMenuItems(MenuView parent, TestingElement element)
        {
            foreach (TestingElement child in element.Children)
            {
                if (child.Children.Count > 0)
                {
                    var sub = new MenuView { Text = GetTestingMenuLabel(child) };
                    sub.MenuItems.Add(CreateRunTestMenuItem("Запустить", child));
                    AppendTestingElementMenuItems(sub, child);
                    parent.MenuItems.Add(sub);
                }
                else
                {
                    parent.MenuItems.Add(CreateRunTestMenuItem(GetTestingMenuLabel(child), child));
                }
            }
        }

        public MenuView CreateRunTestMenuItem(string title, TestingElement test)
        {
            return new MenuView
            {
                Text = title,
                OnClick = (_, _) => RunTestingElement(test)
            };
        }

        public static string GetTestingMenuLabel(TestingElement element)
        {
            var label = element.GetType().GetLabel();
            return string.IsNullOrWhiteSpace(label) ? element.GetType().GetTypeName() : label;
        }

        public void RunTestingElement(TestingElement test)
        {
            if (test is null)
                return;

            var title = GetTestingMenuLabel(test);
            PutAction(title, () =>
            {
                this.RunOnUiThread(() => {
                    try
                    {
                        this.Info(test.DoTest(interactive: false, force: true).ToDocument());
                    }
                    catch (Exception ex)
                    {
                        this.Error(ex);
                    }
                });
            });
        }
    }

    /// <summary>Дерево «Данные» — совпадает с <see cref="AppComponents.AppComponentsService.GetDatasourceTree"/>.</summary>
    internal static class DesignerDatasourcesTreeNodes
    {
        internal static TreeViewNodeModel GetOdbcDatasourcesTree()
        {
            var odbc = new OdbcDriverManager();
            return new TreeViewNodeModel
            {
                Title = "ODBC источники",
                Children = new(odbc.GetOdbcDatasourceNames().Select(name => new TreeViewNodeModel
                {
                    Title = name
                }))
            };
        }

        public static TreeViewNodeModel GetAdoDatasourcesTree()
        {
            var root = new TreeViewNodeModel() { Title = "ADO Datasources" };
            using (var db = AppProviderService.GetInstance().Get<DeliveryDbContext>())
            {
                root.Children.AddRange(db.BusinessDatasources.ToList().Select(d => new TreeViewNodeModel()
                {
                    Title = d.Name,
                    Item = d
                }).ToList());
            }
            return root;
        }


        internal static TreeViewNodeModel GetDbContextTree()
        {
            var factory = new TreeFactory();
            var proot = factory.CreateClassTreeView<DbContextBase>(ptype => { }).root;
            proot.Title = "EF DbContext";
            
            return proot;
        }


        internal static TreeViewNodeModel GetWebServicesTree() 
        {
            var root = new TreeViewNodeModel()
            {
                Title = "SOAP сервисы"
            };
            using (var db = new ServiceBuilderDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                root.Children.AddRange(db.RegisteredWsdlServices.Select(d => new TreeViewNodeModel()
                {
                    Title = d.DisplayName,
                    Item = d
                }).ToList());
            }
            return root;
            
        }

    }
}
