using System.Reflection;

using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.ContextModule
{
    /// <summary>
    /// Порождает объекты типа ContextView
    /// </summary>
    public class ContextFactory : IContextFactory
    {
        /// <summary>
        /// Создание меню управления свойствами примитивного типа
        /// </summary>
        public ContextView CreateContextOptionsView(object target, Type delaringType)
        {
            ContextView r = new();
            var properties = target.GetType().GetProperties().Where(p => p.DeclaringType == delaringType).ToList();
            var factory = new ContextViewElementsFactory();
            foreach (var property in properties)
            {
                if (!Typing.IsPrimitive(property.PropertyType))
                    continue;

                if (Typing.IsLogicalType(property.PropertyType))
                {
                    var isChecked = target.GetValue<bool>(property.Name);
                    r.Items.Add(factory.CreateCheckbox("home",
                        property.DeclaringType.GetPropertyLabel(property.Name),
                        isChecked,
                        val =>
                        {
                            target.SetValue(property.Name, val);
                        }
                    ));
                }
                else
                {
                    var val = target.GetValue(property.Name);
                    r.Items.Add(factory.CreateTextbox("home",
                        property.DeclaringType.GetPropertyLabel(property.Name),
                        val is not null ? val.ToString() : "",
                        nval =>
                        {
                            target.SetValue(property.Name, nval);
                        }
                    ));
                }

            }
            return r;
        }

        /// <summary>
        /// По справочнику операций
        /// </summary>
        public ContextView CreateContextView(string text, Dictionary<string, Action> operations)
        {
            var children = operations.Select(kv => new ContextViewButton()
            {
                Label = kv.Key,
                OnClick = (p, s) => { }
            }).ToList();
            ContextView r = new ContextView()
            {
                Label = text,
                Items = new(children)
            };
            return r;
        }


        /// <summary>
        /// По ссылки на сервис
        /// </summary>
        public ContextView CreateContextView(string text, object service)
        {
            IDictionary<string, Action> operations = GetActionMap(service, service.GetType());
            var vcmb = new ViewContextMenuBuilder();
            vcmb.AddContexxtMethods(text, operations);
            return vcmb.ContextMenu.Items[0];
        }



        /// <summary>
        /// Получение справочника операций
        /// </summary>
        public Dictionary<string, Action> GetActionMap(object service, Type ptype, Action<string, object> onresult = null)
            => GetActionMap(service, ptype, GetUserInputParameters, onresult);
        public Dictionary<string, Action> GetActionMap(object service, List<string> methods, Action<string, object> onresult = null)
            => GetActionMap(service, methods, GetUserInputParameters, onresult);
        public Dictionary<string, Action> GetActionMap(
            object service,
            Type ptype,
            Func<MethodInfo, object[]> parametersProvider,
            Action<string, object> onresult = null)
        {
            var methods = ptype.GetMethods().Where(m => m.DeclaringType == ptype && !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_")).Select(m => m.Name).ToList();
            return GetActionMap(service, methods, parametersProvider, onresult);
        }

        public Dictionary<string, Action> GetActionMap(object service, List<string> methods,
            Func<MethodInfo, object[]> parametersProvider, Action<string, object> onresult)
        {

            var kvs = methods.Distinct().Select(name => new KeyValuePair<string, Action>($"{name}", () =>
            {
                var method = service.GetType().GetMethods().FirstOrDefault(m => m.Name == name);

                try
                {
                    var args = parametersProvider(method);
                    var result = method.Invoke(service, args);
                    var label = service.GetType().GetMethodLabel(method.Name);
                    if (onresult is not null)
                        onresult(label, result);
                }
                catch (Exception ex)
                {
                    this.Error($"Исключение при выполнении {method.Name} => {ex.Message}");
                    throw;
                }
            }));
            kvs = kvs.Where(kv => !service.GetType().GetMethodAttributes(kv.Key).ContainsKey(nameof(NotInputAttribute)));
            return new Dictionary<string, Action>(kvs);
        }



        /// <summary>
        /// Получение аргументов выполнения метода из
        /// формы ввода пользоватлелем
        /// </summary>    
        private object[] GetUserInputParameters(MethodInfo m)
        {
            if (m.GetParameters().Count() == 0)
                return new object[0];
            var inputFormModel = m.DeclaringType.GetInputFormModel(m.Name);
            var editView = new EditView();
            editView.FormFields.AddRange(inputFormModel.FormFields);
            editView.FormFields.Sort((f1, f2) =>
            {
                return f2.Order - f1.Order;
            });
            editView.UpdateEditView();
            editView.ShowDialog(m.GetLabel());
            return new object[0];
        }

        /// <summary>
        /// Получение справочника операций
        /// </summary>
        public Dictionary<string, Action> GetActionMap(List<object> services, Action<string, object> onresult = null)
        {
            Dictionary<string, Action> actions = new();
            foreach (var service in services)
            {

                var map = GetActionMap(service, service.GetType(), onresult);
                foreach (var kv in map)
                {
                    actions[$"/{service.GetTypeName()}/{kv.Key}"] = kv.Value;
                }
            }
            return actions;
        }

        /// <summary>
        /// Получение справочника операций
        /// </summary>
        public Dictionary<string, Action> GetActionNamesMap(List<object> services, Action<string, object> onresult)
        {
            Dictionary<string, Action> actions = new();
            foreach (var service in services)
            {

                var map = GetActionMap(service, service.GetType(), onresult);
                foreach (var kv in map)
                {
                    actions[$"{service.GetType().GetMethodLabel(kv.Key)}"] = kv.Value;
                }
            }
            return actions;
        }

        public ContextView AddContextView(string menu, string text, Action onclick)
        {
            
            throw new NotImplementedException();
        }
    }
}
