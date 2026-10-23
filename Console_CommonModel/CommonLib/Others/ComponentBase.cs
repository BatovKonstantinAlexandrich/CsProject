using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi;

using Org.BouncyCastle.Asn1.Crmf;

using System.Collections.Concurrent;
using System.ComponentModel;
using Console_CommonModel.CommonLib.LibDesigner.DesignerServices;

using static TreeViewModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.Others
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IComponentBaseEvents<T>
    {
        public void Append(T item);
        public void Replace(T item);
    }




    /// <summary>
    /// 
    /// </summary>
    public class ComponentBase : Component, IChangeSupport, IComponentBaseEvents<ComponentBase>, IValidateOptional
    {
        private ChangeSupport changeSupport = new();
        public ServiceFactory serviceFactory = ServiceFactory.Get();

        [NotNullNotEmpty]
        public virtual ComponentBase Parent { get; set; }

        [NotNullNotEmpty]
        public virtual ChangingList<ComponentBase> Children { get; set; } = new();

        [NotNullNotEmpty]
        public virtual AppViewBase ViewModel { get; set; }

        [NotNullNotEmpty]
        public virtual IControlBase Control { get; set; }

        public ComponentBase()
        {
        }

        public void Append([NotNullNotEmpty] ComponentBase item)
        {
            //MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(item);
            Children.Add(item);
            item.Parent = this;
        }

        public void Replace(ComponentBase item)
        {
            Children.Remove(item);
            item.Parent = null;
        }

        public ComponentBase GetRootComponent()
        {
            ComponentBase p = this;
            while (p.Parent is not null)
                p = p.Parent;
            return p;
        }

        /*public RootComponent GetRoot()
        {
            ComponentBase p = this;
            while (p.Parent is not null)
                p = p.Parent;
            if (p is not RootComponent)
                throw new Exception($"Корневой компонент не является типом {nameof(RootComponent)}");
            return (RootComponent)p;
        }*/


        public TComponent GetFirstParent<TComponent>() where TComponent : ComponentBase
        {
            ComponentBase p = this;
            while (p is not null && !p.GetType().IsExtends(typeof(TComponent)))
                p = p.Parent;
            return (TComponent)p;
        }

        public List<TComponent> GetParentComponents<TComponent>() where TComponent : ComponentBase
        {
            var results = new List<TComponent>();
            ComponentBase p = this;
            while (p is not null)
            {
                if (p.GetType().IsExtends(typeof(TComponent)))
                {
                    results.Add((TComponent)p);
                }
                p = p.Parent;
            }
            return results;
        }


        public List<ComponentBase> GetAllChildren(ComponentBase p)
        {
            var results = new List<ComponentBase>();
            results.Add(p);
            foreach (var pchild in Children)
            {
                results.AddRange(GetAllChildren(p));
            }
            return results;
        }

        public List<ComponentBase> GetBrothers()
        {
            if (Parent is not null)
            {
                var all = new List<ComponentBase>(Parent.Children);
                all.Remove(this);
                return all;
            }
            return new List<ComponentBase>();
        }

        public List<ComponentBase> GetAllComponents()
        {
            ComponentBase p = GetRootComponent();
            return GetAllChildren(p);
        }

        public TreeViewModel ToTreeViewModel()
        {
            TreeViewModel model = new()
            {
                Item = new NamedNode()
                {
                    Name = this.GetTypeName()
                }
            };
            foreach (var pchild in Children)
            {
                model.Controls.Add(pchild.ToTreeViewModel());
            }
            return model;
        }

        //public Control ToTreeView()
        //{
        //    var factory = new ComponentsFactory();
        //    var model = ToTreeViewModel();
        //    return factory.CreateTreePanel(model);
        //}

        public void Trace()
        {
            Trace(1, this);
        }

        private void Trace(int level, ComponentBase p)
        {
            for (int i = 0; i < level; i++)
                Console.Write("  ");
            Console.WriteLine(p.GetTypeName());
            foreach (var child in p.Children)
            {
                Trace(level + 1, child);
            }
        }

        [NotInput]
        public bool Changed { get => ((IChangeSupport)changeSupport).Changed; set => ((IChangeSupport)changeSupport).Changed = value; }
        [NotInput]
        public ConcurrentDictionary<string, object> DataState { get => ((IChangeSupport)changeSupport).DataState; set => ((IChangeSupport)changeSupport).DataState = value; }

        [NotInput]
        public bool ValidateOnInput { get => ((IChangeSupport)changeSupport).ValidateOnInput; set => ((IChangeSupport)changeSupport).ValidateOnInput = value; }

        public void ApplyPropertyChangedEvent(PropertyChangedMessage evt)
        {
            ((IChangeSupport)changeSupport).ApplyPropertyChangedEvent(evt);
        }

        public void AddPropertyChangeListener(Action<PropertyChangedMessage> listener)
        {
            ((IChangeSupport)changeSupport).AddPropertyChangeListener(listener);
        }

        public void AddPropertyChangeListener(string property, Action<PropertyChangedMessage> listener)
        {
            ((IChangeSupport)changeSupport).AddPropertyChangeListener(property, listener);
        }

        public T Get<T>(string key)
        {
            return ((IChangeSupport)changeSupport).Get<T>(key);
        }

        public void OnGet<T>(string key, T value)
        {
            ((IChangeSupport)changeSupport).OnGet(key, value);
        }

        public void Set<T>(string key, T value)
        {
            ((IChangeSupport)changeSupport).Set(key, value);
        }

        public bool WasChanged()
        {
            return ((IChangeSupport)changeSupport).WasChanged();
        }

        public void RemovePropertyChangeListener(Action<PropertyChangedMessage> listener)
        {
            ((IChangeSupport)changeSupport).RemovePropertyChangeListener(listener);
        }

        public Action<PropertyChangedMessage> OnChange { get => ((IChangeSupport)changeSupport).OnChange; set => ((IChangeSupport)changeSupport).OnChange = value; }

        public List<string> GetPath()
        {
            List<string> path = new();
            var p = this;
            while (p is not null)
            {
                if (p.Parent is not null)
                {
                    path.Add(p.Parent.Children.IndexOf(p).ToString());
                }
                p = p.Parent;
            }
            return path;
        }

        public string GetPathUrl()
        {
            string path = "/";
            var p = this;
            while (p is not null)
            {
                if (p.Parent is not null)
                {
                    path += p.Parent.Children.IndexOf(p).ToString() + "/";
                }
                p = p.Parent;
            }
            return path;
        }



        public Dictionary<string, List<string>> ValidateOptional()
        {
            Dictionary<string, List<string>> result = new();
            foreach (ComponentBase comp in GetAllComponents())
            {
                result[comp.GetPathUrl()] = new();
                if (ViewModel.control != comp.Control)
                {
                    result[comp.GetPathUrl()].Add("Ссылка на элемент управления компонента и представления отличаются друг от друга");
                }
            }
            return result;
        }

        private static ConcurrentDictionary<Type, List<Type>> ViewServices = new();


        /// <summary>
        /// Получение сервисов представления
        /// </summary>        
        public virtual List<DesignerBase> GetViewServices()
        {
            return new List<DesignerBase>();
        }


        public List<object> GetControlServices()
        {
            Type controlType = Control.GetType();
            Type controlServiceType = typeof(IControlService<>).MakeGenericType(controlType);

            List<Type> serviceTypes = new();
            serviceTypes.AddRange(serviceFactory.GetTypesImplemented(controlServiceType));
            var exts = controlType.GetExtendedTypes();
            this.Info(controlType.GetTypeName());
            this.Info(exts.Select(e => e.GetTypeName()).ToList());
            exts.Add(controlType);
            foreach (var extsType in exts)
            {
                var ptypes = serviceFactory.GetTypesImplemented(typeof(IControlService<>).MakeGenericType(extsType));
                serviceTypes.AddRange(ptypes);
            }
            List<object> controls = new();
            foreach (var extsType in serviceTypes)
            {
                try
                {
                    //this.Info(extsType.GetTypeName());
                    var serviceControl = extsType.New();
                    if (serviceControl.GetType().GetMethods().Any(m => m.Name == "SetControl"))
                    {
                        serviceControl.Invoke("SetControl", new object[] { Control });
                        controls.Add(serviceControl);
                    }

                }
                catch (Exception ex)
                {
                    this.Error($"Не удалось подключить сервис {extsType.GetTypeName()} => {ex.Message}");
                }
            }
            return controls;
        }


        public void AddPropertyChangeListener<T>(string property, Action<T> changed)
        {
            ((IChangeSupport)changeSupport).AddPropertyChangeListener(property, changed);
        }

        public void Bind(string propertyName, IChangeSupport target, string relativePropertyName)
        {
            ((IChangeSupport)changeSupport).Bind(propertyName, target, relativePropertyName);
        }
    }







    [Label("Базовый класс компонентов")]
    [Description("Определяет базовую функцию связывания для ContextMenu")]
    public abstract class ComponentBase<TViewModel, TViewControl> : ComponentBase,
        IComponentBase<TViewModel, TViewControl> 
        where TViewModel : AppViewBase
        where TViewControl : IControlBase
    {

        public TViewModel GetViewModel() => (TViewModel)ViewModel;
        public TViewControl GetControl() => (TViewControl)Control;

        public IComponentsFactory<IControlBase> componentsFactory;

        public ChangingList<DesignerBase> viewServices { get; set; } = new();
        public bool Binded { get; }

        protected ComponentBase([NotNullNotEmpty] TViewModel ViewModel, [NotNullNotEmpty] TViewControl Control)
        {
            if (ViewModel is null)
                throw new ArgumentNullException(nameof(ViewModel));
            if (Control is null)
                throw new ArgumentNullException(nameof(Control));
            this.ViewModel = ViewModel;
            this.Control = Control;
            this.Control.ViewModel = ViewModel;
            this.ViewModel.control = this.Control;
            this.ViewModel.component = this;
            this.Control.component = this;
            this.Binded = false;
            //BindContextMenu();
            //BindProperties();
            //BindChildren();
            ////BindEvents();
            //this.LogPerf(
            //    $"Component.Bind → {typeof(TViewModel).Name} / {typeof(TViewControl).Name}",
            //    Bind);
            this.LogPerf(
                $"Component.Bind → {typeof(TViewModel).Name} / {typeof(TViewControl).Name}",
                Bind);
            this.Binded = false;
            BindContextMenu();
            BindProperties();
            BindChildren();
            BindEvents();
            BindControlEvents();
            InitViewServices();
            BindServices();

        }

        public void BindControlEvents()
        {
            // События мыши уже подписаны в BindEvents(view, control); повторная подписка здесь
            // вызывала двойной вызов OnMouseMove/OnMouseDown/OnMouseUp с теми же аргументами.
            Control.OnClick += (s, e) =>
            {
                ViewModel.OnClick(s, e);
            };
            Control.OnMouseDoubleClick += (s, e) =>
            {
                ViewModel.OnMouseDoubleClick(s, e);
            };
            Control.OnKeyPressed += (s, e) =>
            {
                ViewModel.OnKeyPressed(s, e);
            };
            Control.OnKeyDown += (s, e) =>
            {
                ViewModel.OnKeyDown(s, e);
            };
            //BindViewDragDropToWinFormsControl(item, control);
        }


        /// <summary>
        /// Получение сервисов представления
        /// </summary>        
        public void InitViewServices()
        {
            if (typeof(TViewModel).IsExtendedFrom(typeof(MyValidatableObject).GetTypeName()))
            {
                viewServices.Add(new MyValidatableObjectDesignerService(GetViewModel()));
            }
            if (typeof(TViewModel).IsExtendedFrom(typeof(PanelsView).GetTypeName()))
            {
                viewServices.Add(new PanelsViewDesignerService(GetViewModel()));
            }
            if (typeof(TViewModel).IsExtendedFrom(typeof(StyledItem).GetTypeName()))
            {
                viewServices.Add(new StyledItemDesignerService(GetViewModel()));
            }
            if (typeof(TViewModel).IsExtendedFrom(typeof(ViewDataBuilder).GetTypeName()))
            {
                viewServices.Add(new ViewDataBuilderDesignerService(GetViewModel()));
            }
            if (typeof(TViewModel).IsExtendedFrom(typeof(ViewFactory).GetTypeName()))
            {
                viewServices.Add(new ViewFactoryDesignerService(GetViewModel()));
            }
            if (typeof(TViewModel).IsExtendedFrom(typeof(ViewFactory).GetTypeName()))
            {
                viewServices.Add(new ViewFactoryDesignerService(GetViewModel()));
            }
            if (typeof(TViewModel).IsExtendedFrom(typeof(ViewItem).GetTypeName()))
            {
                viewServices.Add(new ViewItemDesignerService(GetViewModel()));
            }
            if (typeof(TViewModel).IsExtendedFrom(typeof(ViewOptions).GetTypeName()))
            {
                viewServices.Add(new ViewOptionsDesignerService(GetViewModel()));
            }
            viewServices.Reverse();
        }

        private void BindServices()
        {
            List<Action> todo = new();
            //var controlService = GetControlServices();            
            foreach (DesignerBase service in viewServices)
            {
                var p = service;
                ViewModel.ViewServices.Add(service);
                todo.Add(() =>
                {
                    ViewModel.ViewServices.Remove(p);
                });
            }
        }


        public void BindSides(PanelsView view, IControlBase side)
        {
            IComponentsFactory<IControlBase> componentFactory = AppProviderService.GetInstance().Get<IComponentsFactory<IControlBase>>();
            side.ViewModel = (AppViewBase)view;
            if (view.Left is not null) side.Left = componentFactory.CreateControl(view.Left);
            if (view.Right is not null) side.Right = componentFactory.CreateControl(view.Right);
            if (view.Top is not null) side.Top = componentFactory.CreateControl(view.Top);
            if (view.Bottom is not null) side.Bottom = componentFactory.CreateControl(view.Bottom);
            /* if (view.Left is not null) side.Left.Width = view.LeftWidth;
            if (view.Right is not null) side.Right.Width = view.RightWidth;
            if (view.Bottom is not null) side.Bottom.Height = view.BottomHeight;
            if (view.Top is not null) side.Top.Height = view.TopHeight; */
            /*view.AddPropertyChangeListener<int>(nameof(view.TopHeight), val =>
            {
                if (view.Top is not null) side.Top.Height = view.TopHeight;
            });
            view.AddPropertyChangeListener<int>(nameof(view.BottomHeight), val =>
            {
                if (view.Bottom is not null) side.Bottom.Height = view.BottomHeight;
            });
            view.AddPropertyChangeListener<int>(nameof(view.RightWidth), val =>
            {
                if (view.Right is not null) side.Right.Width = view.RightWidth;
            });
            view.AddPropertyChangeListener<int>(nameof(view.LeftWidth), val =>
            {
                if (view.Left is not null) side.Left.Width = view.LeftWidth;
            })
            /*view.Top.AddPropertyChangeListener(nameof(AppViewBase.Height), evt => side.Top.Height = evt.After.ToString().ToInt());
            view.Bottom.AddPropertyChangeListener(nameof(AppViewBase.Height), evt => side.Bottom.Height = evt.After.ToString().ToInt());
            view.Left.AddPropertyChangeListener(nameof(AppViewBase.Width), evt => side.Left.Width = evt.After.ToString().ToInt());
            view.Right.AddPropertyChangeListener(nameof(AppViewBase.Width), evt => side.Right.Width = evt.After.ToString().ToInt());*/

            //service.SetView(new Panel() { Dock = DockStyle.Fill });
            view.AddPropertyChangeListener("Left", evt =>
            {
                if (evt.After is null)
                {
                    side.SetLeft(null);
                }
                else
                {
                    var control = componentFactory.CreateControl((ViewItem)evt.After);

                    side.SetLeft(control);
                }
            });
            view.AddPropertyChangeListener("Right", evt =>
            {
                if (evt.After is null)
                {
                    side.SetRight(null);
                }
                else
                {
                    ((ViewItem)evt.After).Width = 100;
                    var control = componentFactory.CreateControl((ViewItem)evt.After);
                    Console.WriteLine(control.Width);
                    side.SetRight(control);
                }
            });
            view.AddPropertyChangeListener("Top", evt =>
            {
                if (evt.After is null)
                {
                    side.SetTop(null);
                }
                else
                {
                    var control = componentFactory.CreateControl((ViewItem)evt.After);
                    side.SetTop(control);
                }
            });
            view.AddPropertyChangeListener("Bottom", evt =>
            {
                if (evt.After is null)
                {
                    side.SetBottom(null);
                }
                else
                {
                    var control = componentFactory.CreateControl((ViewItem)evt.After);
                    this.Info(control.GetTypeName());
                    side.SetBottom(control);
                }
            });

        }
        private void BindEvents()
        {
            var view = ViewModel;
            var control = Control;
            //control.MouseEnter += (s, e) => { view.OnMouseEnter(s, e); };
            //control.MouseLeave += (s, e) => { view.OnMouseLeave(s, e); };
            control.OnClick += (s, e) => { view.OnClick(s, e); };
            control.OnKeyPressed += (s, e) => { view.OnKeyPressed(s, e); };
            control.OnResized += (s, e) => { view.OnResized(s, e); };
            view.OnEvent += (evt) =>
            {
            };
            control.OnClick += (s, e) =>
            {
                view.OnEvent(new
                {
                    sender = s,
                    evt = e
                });
            };
        }




        /// <summary>
        /// 
        /// </summary>
        public class EditorService(object viewModel, Action<object, dynamic> onsubmit)
        {
            public void Edit()
            {
                ToEditDialog((s, e) =>
                {
                    if (onsubmit is not null)
                    {
                        onsubmit(s, e);
                    }
                });
            }

            private void ToEditDialog(Action<object, object> value)
            {
                throw new NotImplementedException();
            }
        }

        private void BindChildren()
        {
            ViewModel.Controls.OnChanged += (s, evt) =>
            {
                switch (evt.Target.Action)
                {
                    case "added":
                        {
                            try
                            {
                                if (evt.Target.Item is ViewItem)
                                {
                                    var view = (ViewItem)evt.Target.Item;
                                    var ctrl = componentsFactory.CreateControl(view);
                                    ctrl.Name = view.GetHashCode().ToString();

                                    Control.Controls.Add(ctrl);
                                    if (Control.component is not null && ctrl is IControlBase && ctrl.component is not null)
                                    {
                                        Control.component.Append(ctrl.component);
                                    }

                                }
                                else if (evt.Target.Item is IControlBase)
                                {
                                    Control.Controls.Add((IControlBase)evt.Target.Item);
                                }

                                break;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"Исключение при обработки события добавления модели представления: {ex.Message}");
                            }
                        }
                    case "removed":
                        {
                            var view = (ViewItem)evt.Target.Item;
                            AppViewBase ctrl = null;
                            foreach (AppViewBase p in Control.Controls)
                            {
                                if (p.Name == view.GetHashCode().ToString())
                                {
                                    ctrl = p;
                                    break;
                                }

                            }
                            if (ctrl is not null)
                            {
                                Control.Controls.Remove(ctrl.ConvertTo<IControlBase>());
                            }
                            break;
                        }
                }
            };
        }



        /// <summary>
        /// Выполняет связывание модели представления и элемента управления
        /// </summary>
        public abstract void Bind();

        private void BindProperties()
        {
            ViewModel.OnChange += (evt) =>
            {
                switch (evt.Property)
                {

                    case "Dock":
                        {
                            if (evt.After is not null)
                            {
                                var dockValue = evt.After.ToString();
                                Control.Dock = dockValue;

                            }
                            break;
                        }
                }
            };

            var temp = ViewModel.Dock;
            ViewModel.Dock = "None";
            ViewModel.Dock = temp;
            Control.Dock = ViewModel.Dock.ToString();
        }


        /// <summary>
        /// Связать сервисы 
        /// </summary>
        public virtual void BindContextMenu()
        {
            //ViewModel.contextMenuServices.OnChanged += (s, e) =>
            //{
            //    switch (e.Target.Action)
            //    {
            //        case "added":
            //            Control.contextMenuServices.Add(e.Target.Item);
            //            break;
            //        case "removed":
            //            Control.contextMenuServices.Remove(e.Target.Item);
            //            break;
            //    }
            //};
        }
    }
}
