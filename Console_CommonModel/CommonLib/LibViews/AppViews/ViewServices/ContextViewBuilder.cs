using System.Reflection;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.ContextModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.ViewServices
{
    public class ContextViewBuilder(ContextView contextView)
    {
        public Action AddViewFactoryMenu()
        {
            if(contextView.Parent is null)
            {
                throw new ArgumentException($"Не установлено свойство: {contextView.Parent}");
            }
            var viewTypes = ServiceFactory.Get().GetTypesExtended<AppViewBase>();
            Dictionary<string,Action> todos = new Dictionary<string,Action>();
            foreach (var viewType in viewTypes)
            {
                todos[viewType.GetLabel()] = () =>
                {
                    contextView.Parent.Append(viewType.New<AppViewBase>());
                };
            }
            var menu = this.CreateServiceMethods("Добавить представление", todos);
            contextView.Items.Add(menu);
            return () => {
                contextView.Items.Remove(menu);
            };
        }



        /// <summary>
        /// Добавление в контекстное меню пунктов управления свойствами определёнными в заданном тип/
        /// Результат содержит ссылку на действие для отмены.
        /// </summary>
        public Action AddContextServiceOptions<TServiceOptions>([NotNullNotEmpty] object service)
        {
            var factory = new ContextFactory();
            var view = factory.CreateContextOptionsView(service, typeof(TServiceOptions));
            view.Label = typeof(TServiceOptions).GetLabel();
            contextView.Items.Add(view);
            return () =>
            {
                contextView.Items.Remove(view);
            };
        }

        public void AddContextRadioGroup([NotNullNotEmpty] string name, [NotNullNotEmpty] object service, [NotNullNotEmpty] List<string> options, Action<string> onchanged)
        {
            var factory = new ContextViewElementsFactory();
            var value = service.GetValue(name)?.ToString();
            var menu = factory.CreateRedioGroup("home",
                service.GetType().GetPropertyLabel(name),
                value,
                service,
                options,
                val =>
                {
                    service.SetValue(name, val);
                    if (onchanged is not null)
                        onchanged(val);
                });
            contextView.Items.Add(menu);
        }


        /// <summary>
        /// Добавление в контекстное меню пунктов выполнения методов сервиса
        /// </summary>
        /// <typeparam name="TService">тип сервиса</typeparam>
        /// <param name="name">название меню</param>
        /// <param name="service">ссылка на сервис</param>
        /// <param name="parametersProvider">функция получения параметров</param>
        public Action AddServiceMethodsType<TService>(string name, TService service, Func<MethodInfo, object[]> parametersProvider)
        {
            var actions = service.GetType().GetOwnMethodNames();
            ContextView menu = new() { Label = name };
            foreach (var action in actions)
            {
                var method = service.GetType().GetMethods().FirstOrDefault(m => m.DeclaringType == typeof(TService) && m.Name == action);
                var sublabel = service.GetType().GetMethodLabel(action);
                ContextViewButton submenu = new() { Label = sublabel };
                submenu.OnClick += (p, e) =>
                {
                    try
                    {
                        object[] parameters = parametersProvider(method);
                        method.Invoke(service, parameters);
                    }
                    catch (Exception ex)
                    {
                        this.Error($"Исключение при вызове метода {method.Name}", ex);
                    }
                };
                menu.Items.Add(submenu);
            }
            contextView.Items.Add(menu);
            return () =>
            {
                contextView.Items.Remove(menu);
            };
        }


        /// <summary>
        /// Добавление в контекстное меню пунктов выполнения методов сервиса
        /// </summary>
        /// <typeparam name="TService">тип сервиса</typeparam>
        /// <param name="name">название меню</param>
        /// <param name="service">ссылка на сервис</param>
        /// <param name="parametersProvider">функция получения параметров</param>
        public Action AddServiceMethods<TService>(string name, TService service)
        {
            return AddServiceMethods(name, service, service.GetOwnMethodNames(), method => GetUserInputParameters(method) );
        }
        public Action AddServiceMethods<TService>(string name, TService service, List<string> methods, Func<MethodInfo, object[]> parametersProvider)
        {
            var actions = methods;
            ContextView menu = new() { Label = name };
            foreach (var action in actions)
            {
                var method = service.GetType().GetMethods().FirstOrDefault(m => m.DeclaringType == typeof(TService) && m.Name == action);
                var sublabel = service.GetType().GetMethodLabel(action);
                ContextViewButton submenu = new() { Label = sublabel };
                submenu.OnClick += (p, e) =>
                {
                    try
                    {
                        object[] parameters = parametersProvider(method);
                        method.Invoke(service, parameters);
                    }
                    catch (Exception ex)
                    {
                        this.Error($"Исключение при вызове метода {method.Name}", ex);
                    }
                };
                menu.Items.Add(submenu);
            }
            contextView.Items.Add(menu);
            return () =>
            {
                contextView.Items.Remove(menu);
            };
        }
        /// <summary>
        /// Добавление в контекстное меню пунктов выполнения методов сервиса
        /// </summary>
        /// <typeparam name="TService">тип сервиса</typeparam>
        /// <param name="name">название меню</param>
        /// <param name="service">ссылка на сервис</param>
        /// <param name="parametersProvider">функция получения параметров</param>
        public Action AddServiceMethods<TService>(string name, TService service, Func<MethodInfo, object[]> parametersProvider)
        {
            var actions = service.GetType().GetOwnMethodNames();
            ContextView menu = new() { Label = name };
            foreach (var action in actions)
            {
                var method = service.GetType().GetMethods().FirstOrDefault(m => m.DeclaringType == typeof(TService) && m.Name == action);
                var sublabel = service.GetType().GetMethodLabel(action);
                ContextViewButton submenu = new() { Label = sublabel };
                submenu.OnClick += (p, e) =>
                {
                    try
                    {
                        object[] parameters = parametersProvider(method);
                        method.Invoke(service, parameters);
                    }
                    catch (Exception ex)
                    {
                        this.Error($"Исключение при вызове метода {method.Name}", ex);
                    }
                };
                menu.Items.Add(submenu);
            }
            contextView.Items.Add(menu);
            return () =>
            {
                contextView.Items.Remove(menu);
            };
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

        public ContextView CreateServiceMethods(string name, IDictionary<string, Action> todos)
        {
            ContextView menu = new() { Label = name };
            foreach (var action in todos)
            {
                Action onclick = action.Value;

                ContextView submenu = new() { Label = action.Key };
                submenu.OnClick += (s, e) =>
                {
                    onclick();
                };
                menu.Items.Add(submenu);
            }
            return menu;
        }

        public Action AddServiceMethods(string name, object service, Func<MethodInfo, object[]> parametersProvider)
        {
            var actions = service.GetType().GetOwnMethodNames();
            ContextView menu = new() { Label = name };
            foreach (var action in actions)
            {
                var method = service.GetType().GetMethods().FirstOrDefault(m => m.DeclaringType == service.GetType() && m.Name == action);
                var sublabel = service.GetType().GetMethodLabel(action);
                ContextViewButton submenu = new() { Label = sublabel };
                submenu.OnClick += (p, s) =>
                {
                    try
                    {
                        object[] parameters = parametersProvider(method);
                        method.Invoke(service, parameters);
                    }
                    catch (Exception ex)
                    {
                        this.Error($"Исключение при вызове метода {method.Name}", ex);
                    }
                };
                menu.Items.Add(submenu);
            }
            contextView.Items.Add(menu);
            return () =>
            {
                contextView.Items.Remove(menu);
            };
        }



        public Action AddServiceMethods(object service)
        {
            var name = service.GetType().GetLabel();
            var actions = service.GetType().GetOwnMethodNames();
            ContextView menu = new() { Label = name };
            foreach (var action in actions)
            {
                var method = service.GetType().GetMethods().FirstOrDefault(m => m.DeclaringType == service.GetType() && m.Name == action);
                var sublabel = service.GetType().GetMethodLabel(action);
                ContextViewButton submenu = new() { Label = sublabel };
                submenu.OnClick += (p, e) =>
                {
                    try
                    {
                        object[] parameters = GetUserInputParameters(method);
                        method.Invoke(service, parameters);
                    }
                    catch (Exception ex)
                    {
                        this.Error($"Исключение при вызове метода {method.Name}", ex);
                    }
                };
                menu.Items.Add(submenu);
            }
            contextView.Items.Add(menu);
            return () =>
            {
                contextView.Items.Remove(menu);
            };
        }
    }
}
