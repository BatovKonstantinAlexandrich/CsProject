using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.EditModule
{

    /// <summary>
    /// Представление для редактирования объекта
    /// </summary>
    [Label("Представление формы редактирования объекта")]
    public class EditView : ControlView, ILayoutViewNode
    {

        public static ConcurrentDictionary<Type, ConcurrentDictionary<string, InputFormField>> Fields = new();

        [InputBool]
        [Label("Show Buttons")]
        //public bool ShowButtons { get => submit.ShowButtons; set => submit.ShowButtons = true; }
        public readonly ScrollView scrollView;
        //public readonly SubmitView submit;

        [NotInput]

        public event Action<bool, AppViewBase> OnEditCompleted = delegate { };

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotInput]
        private Action ClearBindings = () => { };


        [InputStructureCollection<InputFormField>()]
        [Label("Поля формы")]
        public ChangingList<InputFormField> FormFields { get; set; } = new();

        [NotInput]
        [Label("Target")]
        public object Target { get; set; }

        [NotInput]
        public TextView TitleView { get; set; }

        public EditView()
        {
            MaxWidth = 520;
            Padding = 20;
            MinHeight = MinWidth = 100;
            TitleView = CreateTitle(Text);
            AddPropertyChangeListener(nameof(Text), evt => TitleView.Text = $"{evt.After}");            
          
            LayoutFill(scrollView = new ScrollView());
        }


        public SubmitView CreateSubmitView()
        {
            var submit = new SubmitView() { Padding = 20 };
            submit.Parent = this.Parent;
            //submit.Top = (TitleView);
            //submit.MinHeight = 120;
            submit.oncompleted += (s, e) =>
            {
                OnEditCompleted(s, e);
            };
            //OnEditCompleted += (s, e) =>
            //{
            //};
            //submit.LayoutFill(this);
            //return submit;
            this.OnValidationStateChanged += state => {
                submit.ValidationSuccess = state;
            };
            submit.Set(this);
            return submit;
        }

        public void SetAutoSize()
        {
            MinHeight = FormFields.Sum(f => f.ToControl().Height);
        }


        public static EditView CreateEditViewBase(object target, Action<bool, object> OnEditCompleted = null)
        {
            var properties = target.GetType().GetOwnInputPropertyNames();
            return CreateEditView(target, properties, OnEditCompleted);
        }

        /// <summary>
        /// Прогревает кэш метаданных полей формы для типа элемента коллекции.
        /// </summary>
        public void WarmFieldCache(Type itemType)
        {
            if (itemType is null)
                return;
            try
            {
                var sample = itemType.New();
                GetFields(sample, itemType.GetOwnInputPropertyNames());
            }
            catch
            {
                /* ignored */
            }
        }



        public void SetTargetValue(object target, Action<bool, object> oncompleted)
        {
            var targetType = target.GetType();
            target.Info(targetType.GetTypeName());
            var properties = targetType.GetOwnInputPropertyNames();
            if (properties is null || properties.Count() == 0)
            {
                throw new ArgumentException(nameof(properties),
                    $"Не удалось определить набор свойств для ввода для типа {target.GetTypeName()}");
            }
            BindEditControls(target, properties, (s, p) =>
            {
                if (oncompleted is not null)
                    oncompleted(s, p);
            });

        }


        public static EditView CreateEditViewBase(object target, List<string> properties, Action<bool, object> OnEditCompleted = null)
        {
            return CreateEditView(target, properties, OnEditCompleted);
        }


        public static EditView CreateEditView<T>(T target, Action<bool, T> oncompleted = null)
        {
            List<string> names = typeof(T).GetOwnInputPropertyNames();
            return CreateEditView(target, names, (s, p) =>
            {
                if (oncompleted is not null)
                {
                    oncompleted(s, p);
                }
            });

        }


        public static EditView CreateEditView<T>(T target, List<string> properties, Action<bool, T> oncompleted = null)
        {
            EditView view = new EditView();
            if (target is null)
                throw new NullReferenceException(nameof(target));
            if (properties is null)
                throw new NullReferenceException(nameof(properties));
            view.InfoTime($"Создание редактора формы для {target.GetType().GetTypeName()} кол-во свойств {properties.ToJson()}", () =>
            {                
                if (properties is null || properties.Count() == 0)
                {
                    throw new ArgumentException(nameof(properties),
                        $"Не удалось определить набор свойств для ввода для типа {typeof(T).GetTypeName()}");
                }

                view.BindEditControls(target, properties, (s, p) =>
                {
                    if (oncompleted is not null)
                        oncompleted(s, (T)p);
                });
                if (target is AppViewBase vb)
                {
                    var menu = new MenuView();
                    if (vb.ViewServices is null || vb.ViewServices.Count() == 0)
                    {
                        vb.ViewServices.AddRange(vb.GetViewServices());
                    }
                    foreach (var p in vb.ViewServices)
                    {
                        var submenu = p.CreateDesignerMenuView();
                        if (submenu.MenuItems.Count() > 0)
                        {
                            menu.MenuItems.Add(submenu);
                        }
                    }

                    var topMenu = new MenuView()
                    {
                        Text = "Специальные возможности",
                        MenuItems = new()
                    {
                        menu
                    }
                    };
                    menu.Text = "Специальные возможности";
                    view.Top = topMenu;
                }
            });
            return view;
        }


        public void RunAtForm()
        {
            var form = ToControl().RunAtForm(Text);
            form.ShowDialog();
        }

        public void ShowDialog()
        {
            var form = ToControl().RunAtForm(Text);
            form.ShowDialog();
        }



        public void BindEditControls(object target, List<string> properties = null, Action<bool, object> oncompleted = null)
        {
            Target = target;
            Text = target.GetLabel();
            if (properties is null || properties.Count() == 0)
            {
                properties = target.GetType().GetOwnInputPropertyNames();
            }

            FormFields = GetFields(target, properties);
            UpdateEditView(target, oncompleted);
            Text = target.GetLabel();
            if(target is MyValidatableObject mo)
            {
                mo.OnValidationStateChanged += (successValidated) => {
                    this.ValidationSuccess = successValidated;
                };
            }
        }


        private void UpdateEditView(object target, Action<bool, object> oncompleted)
        {
            ClearBindings();
            ClearBindings = () => { };

            scrollView.Controls.Clear();
            FormFields.Sort((f1, f2) =>
            {
                return
                    f1.IsCollection && !f2.IsCollection ? 10 :
                    !f1.IsCollection && f2.IsCollection ? -10 :
                    f1.Order - f2.Order;
            });
            //target.SetTextValue(prop, val);
            //System.Console.Clear();
            //this.Info(fields.Select(f => (f.Name, f.Type)));
            foreach (var p in FormFields)
            {
                p.Target = target;
                if (p.Name == "Icon")
                {
                    p.Height += 400;
                    p.AutoSize = false;
                }
                scrollView.LayoutTop(p);
            }
            Action<bool, AppViewBase> toremove = (s, v) =>
            {
                if (oncompleted is not null)
                    oncompleted(s, target);
                //if (this.ToControl().Parent is Form f)
                //{
                //    f.Close();
                //}
                //else if (this.Parent is AppViewBase avb)
                //{
                //    avb.GoBack();
                //}
                /*
                else
                {
                    throw new Exception();
                }*/
            };
            OnEditCompleted += toremove;
            var list = scrollView.Controls.ToList();
            list.Reverse();
            scrollView.Controls.Clear();
            scrollView.Controls.AddRange(list);
            Top = CreateTitle(Text);
            ClearBindings = () => { OnEditCompleted -= toremove; };
        }

        public void UpdateEditView()
        {

            ClearBindings();
            ClearBindings = () => { };

            scrollView.Controls.Clear();
            FormFields.Sort((f1, f2) =>
            {
                return f2.Order - f1.Order;
            });
            //target.SetTextValue(prop, val);
            //System.Console.Clear();
            //this.Info(fields.Select(f => (f.Name, f.Type)));
            foreach (var p in FormFields)
            {
                if (p.Name == "Icon")
                {
                    p.Height += 400;
                    p.AutoSize = false;
                }
                else
                {
                    p.Height = 220;
                }
                scrollView.LayoutTop(p);
            }

            var list = scrollView.Controls.ToList();
            list.Reverse();
            scrollView.Controls.Clear();
            scrollView.Controls.AddRange(list);

        }

        public ChangingList<InputFormField> CreateFieldsForType(Type ptype)
        {
            var p = ptype.New();
            return GetFields(p, p.GetOwnInputPropertyNames());
        }


            
        public ChangingList<InputFormField> GetFields(object target, List<string> properties)
        {
            if (target is null) throw new ArgumentNullException(nameof(target));
            if (!Fields.ContainsKey(target.GetType()))
            {

                var allProperties = target.GetType().GetOwnInputPropertyNames();
                var fields = target.GetOwnInputFormFields(allProperties, (target, prop, val) =>
                {
                    
                });
                
                Fields[target.GetType()] = new();
                lock (Fields[target.GetType()])
                {
                    foreach (var field in fields)
                    {
                        if (field is null || field.Name is null)
                            throw new Exception();
                        field.Value = target.GetValue(field.Name);
                        field.OnInput = (text) =>
                        {
                            target.SetTextValue(field.Name, text);
                            this.ValidationResults = target.Validate();
                            this.ValidationSuccess = this.ValidationResults.All(kv => kv.Value.Count() == 0);
                            foreach(var kv in Fields[target.GetType()])
                            {
                                kv.Value.Errors.Clear();
                                if(this.ValidationResults.ContainsKey(kv.Key))
                                {
                                    kv.Value.Errors.AddRange(this.ValidationResults[kv.Key]);
                                }                                
                            }
                        };
                        field.oninput = (text) =>
                        {
                            target.SetTextValue(field.Name, text);
                        };
                        field.Parent = this;
                        if(field.Control is not null)
                        {
                            field.Control.Parent = field;
                        }
                        Fields[target.GetType()][field.Name] = field;
                    }
                }
            }

            var propertySet = new HashSet<string>(properties);
            var res = Fields[target.GetType()]
                .Where(kv => propertySet.Contains(kv.Key))
                .Select(kv => PrepareFieldForTarget(kv.Value, target))
                .ToList();
            foreach(var p in res)
            {
                p.Parent = this;
            }
            return new(res);
        }

        private static InputFormField PrepareFieldForTarget(InputFormField template, object target)
        {
            if (template is null || template.Name is null)
                throw new Exception();

            template.Control = null;
            template.control = null;
            template.Target = target;
            template.Value = target.GetValue(template.Name);
            var propertyName = template.Name;
            template.OnInput = text => target.SetTextValue(propertyName, text);
            template.oninput = text => target.SetTextValue(propertyName, text);
            return template;
        }

        public void BindEditControlsTo(AppViewBase p)
        {
            throw new NotImplementedException();
        }

    }

}
