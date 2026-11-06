using System.Reflection;
using Console_CommonModel.CommonLib.LibDesigner.DesignerServices;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.Others;
using System.ComponentModel;
using Console_CommonModel.ActionModels;
using System;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent
{
    public interface IAppViewDiagnostics
    {
        public void EnsureHasRootView();
        public void EnsureHasDesigner();
        public void EnsureHasClientApi();
        public void EnsureHasControl();
        public void TraceContextMenu();
        public void TraceContextMenuPublic();
        public void TraceViewServices();
    }
    public class AppViewDiagnosticService: IControlService<AppViewBase, IAppViewDiagnostics>, IAppViewDiagnostics
    {
        public AppViewBase Control { get; set; }


        public void SetControl(AppViewBase item)
        {
            this.Control = item;
        }

        public IAppViewDiagnostics BuildControl() => this;

        public void EnsureHasRootView()
        {
            if(Control.GetAppRootView() != null)
            {
                throw new Exception($"{Control.GetTypeName()} не линкуется к Root");
            }
        }

        public void EnsureHasDesigner()
        {
            if (Control.ViewDesigner != null)
            {
                throw new Exception($"{Control.GetTypeName()} нет ссылки на ViewDesigner");
            }
        }

        public void EnsureHasClientApi()
        {
            if (Control.Client != null)
            {
                throw new Exception($"{Control.GetTypeName()} нет ссылки на Client");
            }
        }

        public void EnsureHasControl()
        {
            if (Control.control != null)
            {
                throw new Exception($"{Control.GetTypeName()} нет ссылки на control");
            }
        }

        public void TraceContextMenu()
        {
            Control.ContextMenu.Trace();
        }

        public void TraceViewServices()
        {
            this.Info(Control.ViewServices.Select(s => s.GetTypeName()).ToJson());
        }

        public void TraceContextMenuPublic()
        {
            Control.ContextMenuPublic.Trace();
        }
    }

    [Label("Представление с интеграцией сервиса управления")]
    public class AppViewBase<TControlService> : AppViewBase
        where TControlService : class, IControlService<AppViewBase<TControlService>>
    {
        private readonly TControlService controlService;

        public AppViewBase()
        {                        
            controlService = AppProviderService.GetInstance().Get<TControlService>();
            controlService.SetControl(this);
        }
    }


    [Label("Модель представления")]
    public class AppViewBase : SearchableView, IErrorHandler, IStatusView, IServiceProvider, IAppViewDiagnostics
    {
        private IAppViewDiagnostics ViewDiagnostics { get; set; }


        public Action<string> OnProgressStarted { get; set; } = text => { };
        public Action<string> OnProgressCompleted { get; set; } = text => { };


        /// <summary>
        /// Получение сервисов представления
        /// </summary>        
        public List<DesignerBase> InitViewServices()
        {
            List<DesignerBase> viewServices = new();
            var TViewModel = this.GetType();
            this.OnLayout += pnode => {
                try
                {
                    this.EnsureHasRootView();
                }
                catch (Exception ex)
                {
                    this.Error($"{typeof(AppViewBase).GetTypeName()} OnLayout() => {ex}");
                }
            };
            if (TViewModel.IsExtendedFrom(typeof(MyValidatableObject).GetTypeName()))
            {
                viewServices.Add(new MyValidatableObjectDesignerService(this));
            }
            if (TViewModel.IsExtendedFrom(typeof(PanelsView).GetTypeName()))
            {
                viewServices.Add(new PanelsViewDesignerService(this));
            }
            if (TViewModel.IsExtendedFrom(typeof(StyledItem).GetTypeName()))
            {
                viewServices.Add(new StyledItemDesignerService(this));
            }
            if (TViewModel.IsExtendedFrom(typeof(ViewDataBuilder).GetTypeName()))
            {
                viewServices.Add(new ViewDataBuilderDesignerService(this));
            }
            if (TViewModel.IsExtendedFrom(typeof(ViewFactory).GetTypeName()))
            {
                viewServices.Add(new ViewFactoryDesignerService(this));
            }
            if (TViewModel.IsExtendedFrom(typeof(ViewFactory).GetTypeName()))
            {
                viewServices.Add(new ViewFactoryDesignerService(this));
            }
            if (TViewModel.IsExtendedFrom(typeof(ViewItem).GetTypeName()))
            {
                viewServices.Add(new ViewItemDesignerService(this));
            }
            if (TViewModel.IsExtendedFrom(typeof(ViewOptions).GetTypeName()))
            {
                viewServices.Add(new ViewOptionsDesignerService(this));
            }
            viewServices.Reverse();
            return viewServices;
        }
        /// <summary>
        /// Отображение представления
        /// </summary>        
        public static void Run<TAppViewBase>()
            where TAppViewBase : AppViewBase
        {
            var view = typeof(TAppViewBase).New<AppViewBase>();
            view.RunAtForm();

        }

        public string GetContainerCssClassName()
        {
            return this.GetTypeName() + "ContainerControl".ToKebabStyle("-").ToLower();
        }
        public string GetContainerDomId()
        {
            return $"p_container_control"+GetHashCode();
        }

        public Action AddContextModalDialog<TService>(Func<IClientAPI> get_api)
            where TService : AppViewBase
        {
            //this.ViewServices.AddRange(this.InitViewServices());
            this.ContextMenu.Items.Add(new ContextView()
            {
                Label = typeof(TService).GetLabel(),
                Icon = typeof(TService).GetIcon(),
                OnClick = (p, e) => {

                    try
                    {
                        var appViewBase = this.Cast<AppViewBase>();
                        var serviceView = typeof(TService).New<TService>();                                                
                        var rootView = appViewBase.GetAppRootView();
                        IClientAPI rootCtrl = rootView.control;
                        appViewBase.Commit();
                        Action<bool, TService> todo = (bool success, TService p) => { 
                            if(!success)
                            {
                                appViewBase.Rollback();
                            }
                        };
                        rootCtrl.ShowView<TService>(serviceView.GetLabel(), serviceView, todo);
                        
                    }
                    catch (Exception ex)
                    {
                        this.Error(ex.Message);

                    }

                }
            });
            return () => { };
        }
        /// <summary>
        /// Используется внутри LayoutView 
        /// если = true и LayoutView.FillContent = true
        /// то элемент заполняет оставшееся свободное пространство 
        /// панели размещения прямо пропорционально суммарной ширине
        /// всех FillContent элементов на панели
        /// </summary>
        [InputBool]
        public bool FillContent { get; set; }

        /// <summary>
        /// Передавать значение параметра ResolveDesignMode вниз по иерархи
        /// </summary>
        [InputBool]
        public bool ResolveDesignMode { get; set; } = true;


        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public List<Action> rollbackDesigners = new();



        /// <summary>
        /// Методы действия этих объектов вызываются в контекстном меню созданном автоматически 
        /// при добавлении в коллекцию.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public readonly ChangingList<object> contextMenuServices = new();

        /// <summary>
        /// Ссылка на компонент, который предоставляет комплексное управление представлением.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ComponentBase component { get; set; }

        /// <summary>
        /// Ссылка на WinForms Control
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual IControlBase control { get => Get<IControlBase>(nameof(control)); set=> Set<IControlBase>(nameof(control),value); }

        [OnChange(nameof(control))]
        public void OnControlChanged()
        {
            if(control is not null)
            {
                control.ViewModel = this;
            }
        }

        /// <summary>
        /// Вспомогательные методы создания компонентов представлений
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public static IComponentsFactory<IControlBase> componentsFactory;



        /// <summary>
        /// Элемент управления поиском создаётся при установки признака Searchable
        /// Предполагается дальнейшее связывание для корректной работы
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        private SearchView searchView;



        /// <summary>
        /// При установки режима DesignMode выполняется поиск сервиса
        /// для управдения в среде разработки. 
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [NotInput]
        [InputConstructor<DesignerBase>]
        public DesignerBase ViewDesigner { get; set; }

        [Label("Левая панель")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputConstructor<AppViewBase>()]
        [XmlChildNode]
        public override ViewItem Left { get => Get<ViewItem>("Left"); set => Set("Left", value); }

        [Label("Правая панель")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputConstructor<AppViewBase>()]
        [XmlChildNode]
        public override ViewItem Right { get => Get<ViewItem>("Right"); set => Set("Right", value); }

        [Label("Верхняя панель")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputConstructor<AppViewBase>()]
        [XmlChildNode]
        public override ViewItem Top { get => Get<ViewItem>("Top"); set => Set("Top", value); }

        [Label("Нижняя панель")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputConstructor<AppViewBase>()]
        [XmlChildNode]
        public override ViewItem Bottom { get => Get<ViewItem>("Bottom"); set => Set("Bottom", value); }


        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public ChangingDictionary<string, Action> keyboardConfig = new();

        public AppViewBase() : base()
        {
            //this.AddPropertyChangeListener(nameof(Top), evt => {
            //    if (evt.Before is not null && evt.Before is AppViewBase vb)
            //    {
            //        vb.Parent = null;
            //    }
            //    if (evt.After is not null && evt.After is AppViewBase vb2)
            //    {
            //        vb2.Parent = this;
            //    }
            //});
            //this.AddPropertyChangeListener(nameof(Bottom), evt => {
            //    if (evt.Before is not null && evt.Before is AppViewBase vb)
            //    {
            //        vb.Parent = null;
            //    }
            //    if (evt.After is not null && evt.After is AppViewBase vb2)
            //    {
            //        vb2.Parent = this;
            //    }
            //});
            //this.AddPropertyChangeListener(nameof(Left), evt => {
            //    if (evt.Before is not null && evt.Before is AppViewBase vb)
            //    {
            //        vb.Parent = null;
            //    }
            //    if (evt.After is not null && evt.After is AppViewBase vb2)
            //    {
            //        vb2.Parent = this;
            //    }
            //});
            //this.AddPropertyChangeListener(nameof(Right), evt => {
            //    if (evt.Before is not null && evt.Before is AppViewBase vb)
            //    {
            //        vb.Parent = null;
            //    }
            //    if (evt.After is not null && evt.After is AppViewBase vb2)
            //    {
            //        vb2.Parent = this;
            //    }
            //});
            this.OnAppendedChild += pnode => {
                pnode.Parent = this;
                ((ViewNode)pnode).OnLayout(this);
            };
            this.OnRemovedChild += pnode => {
                pnode.Parent = null;
                ((ViewNode)pnode).OnLayoutLeft(this);
            };
            this.OnLayoutLeft += pnode =>
            {
                ViewNode ctrl = this;
                foreach (var service in ViewServices)
                {
                    service.Deactivate(ctrl);
                }
                ViewServices.Clear();
            };
            this.OnLayout += pnode =>
            {
                ViewNode ctrl = this;
                List<object> last = new();
                this.DoBroadcastToParent<ViewNode>(pnode =>
                {
                    try
                    {
                        if (ctrl != pnode)
                        {
                            try
                            {
                                foreach (var service in pnode.GetLayoutDesignServices())
                                {
                                    this.Info($"{ctrl.GetTypeName()} {pnode.GetTypeName()} {service.GetTypeName()}");
                                    service.Activates(pnode);
                                    pnode.ViewServices.Add(service);

                                }
                            }
                            catch (Exception ex)
                            {
                                this.Error(ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {                         
                        this.Error(ex);
                        throw;
                    }
                });
            };
            var diagnosticsService = new AppViewDiagnosticService();
            diagnosticsService.SetControl(this);
            this.OnAppendedChild += (pnode) => { 
                if(pnode is AppViewBase view)
                {                    
                    view.DoBroadcastToHierElements(p => {
                        p.Cast<AppViewBase>().ContextMenuPublic.Items.AddRange(this.ContextMenuPublic.Items);
                    });
                }
            };

            this.OnAppendedChild += (pnode) => {
                if (pnode is AppViewBase view)
                {
                    EventHandler<IChangingListEvent> handler = (source, evt) => {
                        switch (evt.Target.Action)
                        {
                            case "added": view.ContextMenuPublic.Items.Add((ContextView)evt.Target.Item); break;
                            case "removed": view.ContextMenuPublic.Items.Remove((ContextView)evt.Target.Item); break;
                        }
                    };
                    this.Cast<AppViewBase>().ContextMenuPublic.Items.OnChanged += handler;

                    Action<ViewNode> toremove = null;
                    toremove = (p) => {
                        if (p == pnode)
                        {
                            this.OnRemovedChild -= toremove;
                            this.Cast<AppViewBase>().ContextMenuPublic.Items.OnChanged -= handler;
                        }
                    };
                    this.OnRemovedChild += toremove;
                }
            };


            this.OnRemovedChild += (pnode) =>
            {
                if (pnode is AppViewBase view)
                {
                    view.DoBroadcastToHierElements(p => {
                        p.Cast<AppViewBase>().ContextMenuPublic.Items.RemoveRange(this.ContextMenuPublic.Items);
                    });
                }
            };                         

            //this.OnLayoutLeft += (pnode) => {
            //    if (pnode is AppViewBase view)
            //    {
            //        view.ContextMenuPublic.Items.RemoveRange(this.ContextMenuPublic.Items);
            //    }
            //};
            this.ViewDiagnostics = diagnosticsService;
            this.OnKeyPressed += (object s, KeyPressEventArgs e) =>
            {
                this.Info($"{GetType().GetTypeName()} {e}");
                if (keyboardConfig.ContainsKey("" + e.KeyChar))
                {
                    keyboardConfig["" + e.KeyChar]();
                }
            };
            this.OnKeyDown += (object s, KeyEventArgs e) =>
            {
                this.Info($"{GetType().GetTypeName()}.keydown {e.KeyCodeStr}");                 

                if (KeyboardShortcutKeyFormatter.TryFormatFromKeyEvent(e, out var comboKey)
                    && keyboardConfig.TryGetValue(comboKey, out var comboAction))
                {
                    comboAction();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };

            if (componentsFactory is null)
            {
                componentsFactory = (IComponentsFactory<IControlBase>)
                AppProviderService.GetInstance().GetService(typeof(IComponentsFactory<IControlBase>));
            }
            
            //this.MinHeight = this.MinWidth = 25;
            OnLoad += (s, e) =>
            {
                foreach (var ctrl in Controls.ToList())
                    ctrl.OnLoad(s, e);
            };

            OnLoad += (s, e) =>
            {
                ViewServices.AddRange(GetViewServices());
            };
            ViewServices.OnAdded((i, p) =>
            {

            });
            ViewServices.OnRemoved((i, p) =>
            {

            });


            var ctrl = new ViewDataBuilderDesignerService();
            ctrl.SetControl(this);
            AddPropertyChangeListener(nameof(DesignMode), evt =>
            {
                if ((bool)evt.After)
                {
                    ctrl.Activates(this);
                }
                else
                {
                    ctrl.Deactivate(this);
                }
            });


            RightWidth = LeftWidth = TopHeight = BottomHeight = 0;
            FillContent = false;
            contextMenuServices.OnChanged += OnContextMenuChanged;
            OnMouseEnter += OnDesignerToolHover;
            OnMouseLeave += OnDesignerToolLeave;
            AddPropertyChangeListener(nameof(DesignMode), evt =>
            {
                if (ResolveDesignMode)
                {
                    foreach (var control in Controls)
                    {
                        control.DesignMode = evt.After.ToString().ToBool();
                    }
                }
            });
            AddPropertyChangeListener(nameof(DesignMode), evt =>
            {
                var state = (bool)evt.After;
            });
            AddPropertyChangeListener(nameof(DesignMode), OnActivateDesigners);
            AddPropertyChangeListener(nameof(SearchQuery), evt => {
                this.Info($"{typeof(AppViewBase).GetTypeName()} {nameof(SearchQuery)} {this.SearchQuery}");
            });
            this.oninput += (text) => {
                this.Info($"{typeof(AppViewBase).GetTypeName()} {nameof(oninput)} {text}");
            };
            AddPropertyChangeListener(nameof(Searchable), evt =>
            {
                if (evt.After.ToString().ToBool())
                {
                    searchView = new SearchView() { Height = 20, Width = 100, WidthUnit="%", HeightUnit = "px" };
                    searchView.AddPropertyChangeListener(nameof(SearchQuery), evt => {
                        this.Info($"{typeof(SearchView).GetTypeName()} {nameof(searchView.SearchQuery)} {searchView.SearchQuery}");
                    });
                    searchView.oninput += (text) => {
                        this.Info($"{typeof(SearchView).GetTypeName()} {nameof(oninput)} {text}");
                    };
                    Top = searchView;
                    BindSearchLine(searchView);
                }
                else
                {
                    Top = null;
                }
            });
            DesignMode = false;
        }


        public void CreateContextMenuForExtentedeTypes()
        {
            var extTypes = GetType().GetExtendedTypes();
            extTypes.Remove(typeof(object));
            foreach (var ptype in extTypes)
            {
                ContextMenu.CreateContextMenuEdit(ptype, this);
            }
        }



        public void SetFullSize()
        {
            this.Height = this.Width = 100;
            this.WidthUnit = this.HeightUnit = "%";
            //var form = this.ToControl().GetAppForm();
            //form.Location = new(0,0);
            //if (form is not null)
            //    form.Size = new(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }
        public void SetDefaultSize()
        {
            //var form = this.ToControl().GetAppForm();
            //form.Location = new(120, 60);
            //if (form is not null)
            //    form.Size = new(1024, 800);
        }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public List<AppViewBase> history = new();


        public object GetService(Type type)
        {
            return AppProviderService.GetInstance().GetService(type);
        }
        public TService GetService<TService>()
        {
            return (TService)AppProviderService.GetInstance().GetService(typeof(TService));
        }

        public void RunOnUiThread(Action action)
        {
            action();
            //var sync = this.ToControl();
            //if (sync is null || action is null)
            //    return;
            //try
            //{
            //    if (sync.IsDisposed)
            //        return;
            //    if (sync.InvokeRequired)
            //        sync.BeginInvoke(action);
            //    else
            //        action();

            //}
            //catch (ObjectDisposedException ex)
            //{
            //    this.Error(ex);
            //    throw new Exception($"Исключение при выполнении RunOnUiThread() => {ex.Message}");
            //}
        }

        private void OnActivateDesigners(PropertyChangedMessage evt)
        {
            this.Info(nameof(OnActivateDesigners)+$"({evt.After?.ToString()})");
            if ((bool)evt.After)
            {
                if (ViewDesigner is not null)
                {
                    ViewDesigner.Activates(this);
                }
            }
            else
            {
                if (ViewDesigner is not null)
                {
                    ViewDesigner.Deactivate(this);
                }
            }
        }





        public List<DesignerBase> GetViewServices()
        {
            if (component is null)
            {
                control = ToControl();
                component = ToControl().component;
            }
            return component is null ? new() : component.GetViewServices();
        }
        public List<object> GetControlServices()
            => component.GetControlServices();



        /// <summary>
        /// Вызывается при изменении contextMenuServices
        /// </summary>
        private void OnContextMenuChanged(object sender, IChangingListEvent e)
        {

            var contextMenu = CreateContextMenuView(
                contextMenuServices.ToList(),
                target => target.GetType().Name,
                target => target.GetType().GetMethods().Select(m => m.Name).ToList(),
                (target, action) => action,
                (target, action) =>
                {
                    var method = target.GetType().GetMethods().FirstOrDefault(m => m.Name == action);
                    var result = method.Invoke(target, new object[] { });
                    return result;
                }
            );
            ContextMenu.Items.AddRange(contextMenu.Items);
        }




        private static readonly BindingFlags DesignerLayoutMemberFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static AppViewBase GetDesignerLayoutPanel(AppViewBase parent)
        {
            if (parent is null)
                return null;
            var type = parent.GetType();
            if (type.GetProperty("Layout", DesignerLayoutMemberFlags)?.GetValue(parent) is AppViewBase layoutFromProperty)
                return layoutFromProperty;
            if (type.GetField("Layout", DesignerLayoutMemberFlags)?.GetValue(parent) is AppViewBase layoutFromField)
                return layoutFromField;
            return null;
        }

        private static bool IsDescendantOf(AppViewBase view, AppViewBase ancestor)
        {
            if (view is null || ancestor is null)
                return false;
            for (var current = view; current is not null; current = current.Parent as AppViewBase)
            {
                if (ReferenceEquals(current, ancestor))
                    return true;
            }
            return false;
        }

        private static bool ShouldBubbleDesignerFocusToParent(AppViewBase view, AppViewBase parent)
        {
            var layout = GetDesignerLayoutPanel(parent);
            if (layout is not null && (ReferenceEquals(layout, view) || IsDescendantOf(view, layout)))
                return true;

            // Дочерние панели с Dock (Fill/Top/...) — часть виджета на канве, а не отдельные перемещаемые элементы.
            if (parent.Dock == "None" && view.Dock != "None")
                return true;

            return false;
        }

        /// <summary>
        /// В конструкторе фокус и перемещение — на внешнем виджете (например CollectionView), а не на внутреннем Layout/FlexView.
        /// </summary>
        internal static AppViewBase ResolveDesignerFocusTarget(AppViewBase view)
        {
            var current = view;
            while (current?.Parent is AppViewBase parent)
            {
                if (!ShouldBubbleDesignerFocusToParent(current, parent))
                    break;
                current = parent;
            }
            return current ?? view;
        }

        internal static bool IsDesignerInternalLayoutView(AppViewBase view)
        {
            if (view?.Parent is not AppViewBase parent)
                return false;
            return ShouldBubbleDesignerFocusToParent(view, parent);
        }

        private void OnDesignerToolLeave(object arg1, EventArgs args)
        {
            if (DesignMode)
            {
                if (IsDesignerInternalLayoutView(this))
                    return;

                // При resize/move курсор уходит за границу (влево/вниз) — Leave снимал фокус
                // и Deactivate обрывал жест. Capture иногда сбрасывается при SetBounds,
                // поэтому дополнительно смотрим зажатую ЛКМ.
                var ctrl = control as IControlBase ?? ToControl();
                if (ctrl is not null && ctrl.Capture)
                    return;
                if (IsLeftMouseButtonsActive())
                    return;

                var target = ResolveDesignerFocusTarget(this);
                var root = target.GetAppRootView();
                if (root is null)
                    return;
                if (root.FocusedViews.Contains(target))
                    root.FocusedViews.Remove(target);
            }
        }

        private bool IsLeftMouseButtonsActive()
        {
            throw new NotImplementedException();
        }

        private void OnDesignerToolHover(object arg1, EventArgs args)
        {
            if (DesignMode)
            {
                var target = ResolveDesignerFocusTarget(this);
                var root = target.GetAppRootView();
                if (root is null)
                    return;
                if (!root.FocusedViews.Contains(target))
                    root.FocusedViews.Add(target);
            }
        }


        public TControl ConvertTo<TControl>()
            where TControl: ITreeContainer<TControl>
        {            
            return this.Get<IComponentsFactory<TControl>>().CreateControl(this);
        }


        public virtual IControlBase ToControl()
        {
            if (control is not null)
            {
                return control;
            }

            var r = this.LogPerf($"ToControl → {this.GetTypeName()}", () =>
            {
                var ctrl = componentsFactory.CreateControl(this);

                if (ctrl is IControlBase cb)
                {
                    control = cb;
                    if (this is ViewItem vi)
                        PropagateMouseEventsToHostedVisuals(cb);
                }

                return ctrl;
            });
            return (IControlBase)r;
        }

        private void PropagateMouseEventsToHostedVisuals(IControlBase cb)
        {
            foreach (var child in cb.Controls)
            {
                 //TODO: test
            }
        }
        ///// <summary>
        ///// Пробрасывает MouseDown/MouseUp/MouseMove с вложенных WinForms-контролов на хост <see cref="ControlBase"/>,
        ///// чтобы срабатывали <see cref="EventSupport.OnMouseDown"/> / <see cref="EventSupport.OnMouseUp"/> представления.
        ///// </summary>
        //public static void PropagateMouseEventsToHostedVisuals(ControlBase host)
        //{
        //    if (host is null)
        //        return;

        //    void Walk(Control parent)
        //    {
        //        foreach (Control child in parent.Controls)
        //        {
        //            if (child is ControlBase nested && nested.viewModel is not null)
        //            {
        //                Walk(nested);
        //                continue;
        //            }

        //            host.BindMouseEvents(child);
        //            Walk(child);
        //        }
        //    }

        //    Walk(host);
        //}

        public AppViewBase RunAtForm(string title = "")
        {
            ToControl().RunAtForm(title);
            return this;
        }

        public AppViewBase RunAtForm<TControl>(string title)
        {
            ToControl<TControl>().RunAtForm(title);
            return this;
        }

        public AppViewBase RunAtForm<TControl>()
        {
            ToControl<TControl>().RunAtForm();
            return this;
        }



        public SubmitView ToSubmit(string title, Action<bool, AppViewBase> oncompleted, bool closeForm = false)
        {
            AppViewBase ctrl = this;
            var panel = new SubmitView(ctrl, oncompleted)
            {
                Text = title,
            };
            return panel;
        }

        public SubmitView ToSubmit(Action<bool, AppViewBase> oncompleted, bool closeForm = false)
        {
            string title = GetLabel();
            AppViewBase ctrl = this;
            var panel = new SubmitView(ctrl, oncompleted)
            {
                Text = title,
            };
            return panel;
        }


        public IControlBase ToControl<TControl>()
        {
            var factory = GetService<IComponentsFactory<IControlBase>>();
            return factory.CreateControl(typeof(TControl).GetTypeName(), this);
        }

        public virtual string Trace()
        {
            return Trace(1, this);
        }

        protected virtual string Trace(int level, ViewNode p)
        {
            string text = "";
            for (int i = 0; i < level; i++)
            {
                Console.Write("  ");
                text += "  ";
            }
            text += p.GetTypeName() + " " + p.GetHashCode() + " " + p.Dropable + " " + $"[{p.Cast<AppViewBase>().Width},{p.Cast<AppViewBase>().Height}] {((AppViewBase)p).Dock}\n";
            Console.WriteLine(text);
            if (p is PanelsView pv)
            {
                if (pv.Left is not null)
                {
                    text += Trace(level + 1, pv.Left);
                }
                if (pv.Top is not null)
                {
                    text += Trace(level + 1, pv.Top);
                }
                if (pv.Bottom is not null)
                {
                    text += Trace(level + 1, pv.Bottom);
                }
                if (pv.Right is not null)
                {
                    text += Trace(level + 1, pv.Right);
                }
            }

            foreach (var child in p.Controls)
            {
                text += Trace(level + 1, child);
            }
            return text;
        }


        public virtual void HandleError(object sender, string message)
        {
            var root = GetAppRootView();
            if (root is not null)
            {
                root.HandleError(sender, message);
                return;
            }

            ShowErrorDialog(sender, message);
        }

        /// <summary>
        /// Показывает модальное окно с текстом ошибки.
        /// </summary>
        public virtual void ShowErrorDialog(object sender, string message, string title = "Ошибка")
        {
            var text = string.IsNullOrWhiteSpace(message)
                ? "Произошла неизвестная ошибка."
                : message.Trim();

            if (sender is not null)
            {
                var who = sender is Type t ? t.Name : sender.GetType().Name;
                text = $"{who}: {text}";
            }

            void Show()
            {
                //var owner = control?.FindForm();
                //MessageBox.Show(
                //    owner,
                //    text,
                //    title,
                //    MessageBoxButtons.OK,
                //    MessageBoxIcon.Error);
            }

            if (control is not null && control.IsHandleCreated)
                RunOnUiThread(Show);
            else
                Show();
        }

        public List<ViewNode> GetParentViews<T>()
        {
            List<ViewNode> results = new();
            var p = Parent;
            while (p is not null)
            {
                if (p.IsExtends(typeof(T)))
                {
                    results.Add(p);
                }
                p = p.Parent;
            }
            return results;
        }
        public T GetParent<T>()
            where T : ViewNode
        {
            var p = Parent;
            while (p is not null)
            {
                if (p.IsExtends(typeof(T)))
                {
                    return (T)p;
                }
                p = p.Parent;
            }
            throw new Exception();
        }

        public AppViewRoot GetAppRootView()
        {
            
            if (this is AppViewRoot)
                return (AppViewRoot)this;
            this.Debug(GetParentViews<AppViewBase>().Select(p => p.GetTypeName()).ToList().ToJson());
            var p = GetParentViews<AppViewRoot>().FirstOrDefault();
            var c = (AppViewRoot)p;


            return c;
        }


        public AppViewRoot GetOrCreateAppRootView()
        {
            if (this is AppViewRoot)
                return (AppViewRoot)this;
            var p = GetParentViews<AppViewRoot>().FirstOrDefault();

            if (p is null)
            {
                ViewNode pnode = this;
                while (pnode.Parent is not null)
                {
                    pnode = pnode.Parent;
                }
                var root = new AppViewRoot();
                root.control = root.ToControl();
                root.Add(pnode);
            }
            return GetAppRootView();
        }


        public virtual void PutAction(string name, Action todo, int timeout = 1000000 )
        {
            var r = GetAppRootView();
            if (r is null)
            {
                todo();
                return;
            }
            r.PutAction(name, () => {
                //var cts = new CancellationTokenSource();
                //var started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                //Task.Run(() => {
                //    todo();
                //}, cts.Token);
                //while((DateTimeOffset.Now.ToUnixTimeMilliseconds()-started)<timeout)
                //    Thread.Sleep(timeout);
                //throw new Exception("Превышен интервеавл ожидания выполнения операции");
            });
        }

        public virtual void PutError(string name, string error)
        {
            var r = GetAppRootView();
            if (r is null)
            {

                return;
            }
            r.PutError(name, error);
        }


        public int LeftWidth { get => Get<int>(nameof(LeftWidth)); set => Set(nameof(LeftWidth), value); }
        public int TopHeight { get => Get<int>(nameof(TopHeight)); set => Set(nameof(TopHeight), value); }
        public int BottomHeight { get => Get<int>(nameof(BottomHeight)); set => Set(nameof(BottomHeight), value); }
        public int RightWidth { get => Get<int>(nameof(RightWidth)); set => Set(nameof(RightWidth), value); }

        public void TogglePanels()
        {
            ToggleTop();
            ToggleLeft();
            ToggleRight();
            ToggleBottom();
        }



        private ViewItem _topTemp;

        public void ToggleTop()
        {
            if (Top is null)
            {
                if (_topTemp is not null)
                {
                    ((AppViewBase)_topTemp).control = null;
                    Top = _topTemp;
                }
            }
            else
            {
                _topTemp = Top;
                Top = null;
            }
        }

        private ViewItem _leftTemp;

        public void ToggleLeft()
        {
            if (Left is null)
            {
                if (_leftTemp is not null)
                {
                    ((AppViewBase)_leftTemp).control = null;
                    Left = _leftTemp;
                }
            }
            else
            {
                _leftTemp = Left;
                Left = null;
            }
        }



        private ViewItem _rightTemp;
        public void ToggleRight()
        {
            if (Right is null)
            {
                //Right = new AppViewBase();
                if (_rightTemp is not null)
                {
                    ((AppViewBase)_rightTemp).control = null;
                    Right = _rightTemp;
                }
            }
            else
            {
                _rightTemp = Right;
                Right = null;
            }
        }

        private ViewItem _bottomTemp;

        public void ToggleBottom()
        {
            if (Bottom is null)
            {
                Bottom = _bottomTemp;
            }
            else
            {
                _bottomTemp = Bottom;
                Bottom = null;
            }
        }


        


        public override void LayoutFill(ViewItem node)
        {
            if (Controls.Count > 0)
            {
                history.Add((AppViewBase)Controls[0]);
            }
            node.Dock = "Fill";
            var controlList = Controls.ToList();
            controlList.Reverse();
            foreach (var ctrl in controlList)
            {
                try
                {
                    Controls.Remove(ctrl);
                }
                catch (Exception ex)
                {
                    this.Error($"Не удалось удалить ");
                    Controls.Remove(ctrl);
                }

            }
            Append(node);
        }

        public void GoBack()
        {
            if (history.Count() > 0)
            {
                var view = history.Last();
                history.RemoveAt(history.Count() - 1);
                view.Dock = "Fill";
                Controls.Clear();
                Controls.Add(view);
            }
        }

        public void LayoutCollection(List<object> dataset, List<string> list = null, string label = "", Action<object, object, object, object, object, object> value = null)
        {
            LayoutFill(CreateTable(label,
                list == null ? dataset.First().GetType().GetOwnInputPropertyNames() : list,
                dataset,
                value));
        }

        public void LayoutCollectionAsTable(List<object> dataset, List<string> list = null, string label = "", Action<object, object, object, object, object, object> value = null)
        {
            LayoutFill(CreateTable(label,
                list == null ? dataset.First().GetType().GetOwnInputPropertyNames() : list,
                dataset,
                value));
        }
        public void LayoutCollectionAsList<T>(string label, List<T> dataset, Action<T> onselected = null)
            where T : BaseEntity
        {
            LayoutFill(CreateSelectList(label,
                dataset,
                select =>
                {
                    if (onselected is not null)
                        onselected(select);
                }));
        }

        public void ShowDialog(string title = "")
        {
            if (this.Client is not null)
                this.Client.SubmitDialog(title, this, clicked =>
                {
                });
            else this.Warn("SubmitDialog");
        }

        internal void UpdateFormFields(Dictionary<string, InputFormField>? formFields)
        {
            throw new NotImplementedException();
        }

        public void EnsureHasRootView()
        {
            ViewDiagnostics.EnsureHasRootView();
        }

        public void EnsureHasDesigner()
        {
            ViewDiagnostics.EnsureHasDesigner();
        }

        public void EnsureHasClientApi()
        {
            ViewDiagnostics.EnsureHasClientApi();
        }

        public void EnsureHasControl()
        {
            ViewDiagnostics.EnsureHasControl();
        }

        public void TraceContextMenu()
        {
            ViewDiagnostics.TraceContextMenu();
        }

        public void TraceViewServices()
        {
            ViewDiagnostics.TraceViewServices();
        }

        public void TraceContextMenuPublic()
        {
            ViewDiagnostics.TraceContextMenuPublic();
        }
    }
}

namespace Console_CommonModel
{
    class ScreenSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
    class PrimaryScreen
    {
        public ScreenSize Bounds { get; internal set; }
    }
    class Screen
    {
        public static PrimaryScreen PrimaryScreen { get; internal set; }
    }
}