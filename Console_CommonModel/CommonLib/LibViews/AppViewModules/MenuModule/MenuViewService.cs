using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;

using Console_CommonModel.CommonLib.LibViews.AppViews;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule
{


    public interface IMenuViewService
    {
        public Action AddMenuActions(Dictionary<string, Action> actions);
        public Action AddMenuActions(string name, Dictionary<string, Action> actions);
        public MenuView CreateMenuActions(string name, Dictionary<string, Action> actions);
        public List<MenuView> CreateMenuActions(Dictionary<string, Action> actions);
        public MenuView CreateMenuViewServiceActions<TService>();
        public MenuView CreateMenuViewDbContextInitializerActions();




    }

    public class MenuViewService : IMenuViewService
    {
        public readonly MenuView menuView;

        public MenuViewService(MenuView menuView)
        {
            this.menuView = menuView;
        }

        public Action AddMenuActions(Dictionary<string, Action> actions)
        {
            var items = CreateMenuActions(actions);
            menuView.MenuItems.AddRange(items);
            return () =>
            {
                menuView.MenuItems.RemoveRange(items);
            };
        }
        public Action AddMenuActions(string name, Dictionary<string, Action> actions)
        {
            MenuView menu = CreateMenuActions(name, actions);
            return () =>
            {
                menuView.MenuItems.Remove(menu);
            };
        }

        public MenuView CreateMenuActions(string name, Dictionary<string, Action> actions)
        {
            var menu = new MenuView()
            {
                Text = name
            };
            menu.MenuItems.AddRange(CreateMenuActions(actions));
            menuView.MenuItems.Add(menu);
            return menu;
        }

        public List<MenuView> CreateMenuActions(Dictionary<string, Action> actions)
        {
            var menu = new List<MenuView>();
            foreach (var kv in actions)
            {
                menu.Add(new()
                {
                    Text = kv.Key,
                    OnClick = (s, e) =>
                    {
                        try
                        {
                            kv.Value();
                        }
                        catch (Exception ex)
                        {
                            this.Error($"Исключение при нажатии пункта меню {kv.Key} => {ex}");
                        }
                    }
                });
            }
            return menu;
        }

        public MenuView CreateMenuViewServiceActions<TService>()
        {
            var menu = new MenuView();
            return menu.CreateMenuActions(
                typeof(TService).GetLabel(),
                new(menu.GetActionMap(
                    new List<object>() { menu.GetService<TService>() }, typeof(TService)))
            );
        }

        /// <summary>
        /// Меню операций для всех наследников <see cref="DbContextInitiliazer{TDBContext}"/>.
        /// Параметры типа <see cref="DbContext"/> / <see cref="DbContextBase"/> создаются автоматически.
        /// </summary>
        public MenuView CreateMenuViewDbContextInitializerActions()
        {
            var rootMenu = new MenuView() { Text = "Инициализация БД" };
            var contextFactory = new ContextFactory();
            var initillizers = ServiceFactory.Get().GetDbContextsInitializers()
                .Where(t => !t.IsAbstract)
                .OrderBy(t => t.GetTypeName());
            //this.Info(ServiceFactory.Get().GetDbContextsInitializers());
            foreach (var initializerType in initillizers)
            {
                try
                {
                    var dbContextType = GetDbContextType(initializerType);
                    object initializer = initializerType.New();
                    Func<MethodInfo, object[]> parametersProvider =
                        m => ResolveDbContextInitializerParameters(m, dbContextType);

                    var actionMap = new Dictionary<string, Action>();
                    foreach (var method in GetDbContextInitializerMethods(initializerType))
                    {
                        if (initializerType.GetMethodAttributes(method.Name).ContainsKey(nameof(NotInputAttribute)))
                            continue;

                        var methodInfo = method;
                        var label = initializerType.GetMethodLabel(method.Name);
                        actionMap[label] = () =>
                        {
                            IDisposable[] disposableContexts = Array.Empty<IDisposable>();
                            try
                            {
                                var args = parametersProvider(methodInfo);
                                disposableContexts = args.OfType<IDisposable>().ToArray();
                                var result = methodInfo.Invoke(initializer, args);
                                //contextFactory.Info($"{initializerType.GetTypeName()}.{methodInfo.Name}: {result?.ToJsonOnScreen()}");
                            }
                            catch (Exception ex)
                            {
                                menuView.Error($"Исключение при выполнении {initializerType.GetTypeName()}.{methodInfo.Name} => {ex.Message}");
                            }
                            finally
                            {
                                foreach (var disposable in disposableContexts)
                                    disposable.Dispose();
                            }
                        };
                    }

                    if (actionMap.Count == 0)
                        continue;

                    var subMenu = new MenuView() { Text = initializerType.GetLabel() };
                    subMenu.MenuItems.AddRange(CreateMenuActions(actionMap));
                    rootMenu.MenuItems.Add(subMenu);
                }
                catch (Exception ex)
                {
                    menuView.Error($"Ошибка при создании меню для {initializerType.GetTypeName()}: {ex.Message}");
                }
            }

            menuView.MenuItems.Add(rootMenu);
            return rootMenu;
        }

        private static Type GetDbContextType(Type initializerType)
        {
            for (var type = initializerType; type is not null; type = type.BaseType)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DbContextInitiliazer<>))
                    return type.GetGenericArguments()[0];
            }

            throw new InvalidOperationException(
                $"Тип {initializerType.GetTypeName()} не наследует {typeof(DbContextInitiliazer<>).GetTypeName()}");
        }

        private static IEnumerable<MethodInfo> GetDbContextInitializerMethods(Type initializerType)
        {
            var methods = new Dictionary<string, MethodInfo>();
            for (var type = initializerType; type is not null; type = type.BaseType)
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (method.IsSpecialName || method.IsStatic)
                        continue;
                    if (!methods.ContainsKey(method.Name))
                        methods[method.Name] = method;
                }

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DbContextInitiliazer<>))
                    break;
            }

            return methods.Values.OrderBy(m => m.Name);
        }

        private static bool IsDbContextParameter(Type parameterType) =>
            typeof(DbContext).IsAssignableFrom(parameterType);

        private static object CreateDbContextArgument(Type dbContextType, Type parameterType)
        {
            var contextType = parameterType.IsAssignableFrom(dbContextType) ? dbContextType : parameterType;
            return contextType.New();
        }

        private static object[] ResolveDbContextInitializerParameters(MethodInfo method, Type dbContextType)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 0)
                return Array.Empty<object>();

            var userParameters = parameters.Where(p => !IsDbContextParameter(p.ParameterType)).ToList();
            var userValues = PromptUserParameterValues(method, userParameters);

            return parameters.Select(parameter =>
            {
                if (IsDbContextParameter(parameter.ParameterType))
                    return CreateDbContextArgument(dbContextType, parameter.ParameterType);

                if (userValues.TryGetValue(parameter.Name, out var value))
                    return value;

                if (parameter.HasDefaultValue)
                    return parameter.DefaultValue;

                throw new InvalidOperationException(
                    $"Не задано значение параметра {parameter.Name} для метода {method.Name}");
            }).ToArray();
        }

        private static Dictionary<string, object> PromptUserParameterValues(
            MethodInfo method,
            List<ParameterInfo> userParameters)
        {
            var values = new Dictionary<string, object>();
            if (userParameters.Count == 0)
                return values;

            var inputFormModel = method.DeclaringType.GetInputFormModel(method.Name);
            var editView = new EditView();
            foreach (var parameter in userParameters)
            {
                var field = inputFormModel.FormFields.FirstOrDefault(f => f.Name == parameter.Name);
                if (field is not null)
                    editView.FormFields.Add(field);
            }

            if (editView.FormFields.Count == 0)
                return values;

            var completed = false;
            editView.OnEditCompleted += (success, _) =>
            {
                if (success)
                {
                    foreach (var parameter in userParameters)
                    {
                        var field = inputFormModel.FormFields.FirstOrDefault(f => f.Name == parameter.Name);
                        if (field is not null)
                            values[parameter.Name] = field.Value;
                    }
                    completed = true;
                }
            };
            editView.ShowDialog(method.GetLabel());
            if (!completed)
                throw new OperationCanceledException($"Ввод параметров для {method.Name} отменён");

            return values;
        }
    }
}
