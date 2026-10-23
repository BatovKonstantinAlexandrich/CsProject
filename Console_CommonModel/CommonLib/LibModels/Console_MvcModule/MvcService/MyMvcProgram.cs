using Console_CommonModel.CommonLib.Others;

namespace Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService
{
    public class MyMvcProgram
    {
        public static void Run()
        {
            var mvc = new MyMvcServiceModel();
            var args = Array.Empty<string>();

            while (true)
            {
                var apps = SafeList(() => mvc.GetMvcApplications());
                apps.Add("Зарегистрировать приложение");

                var app = InputConsole.SingleSelect("Mvc-приложения", apps, ref args);
                if (string.IsNullOrEmpty(app) || app == "Назад")
                    return;

                if (app == "Зарегистрировать приложение")
                {
                    var appName = InputConsole.InputText("Введите наименование");
                    if (string.IsNullOrWhiteSpace(appName))
                        continue;
                    mvc.GetOrCreateByName<MyApplicationModel>(appName);
                    continue;
                }

                if (!OnMvcAppSelected(mvc, app, args))
                    return;
            }
        }

        static bool OnMvcAppSelected(MyMvcServiceModel mvc, string app, string[] args)
        {
            while (true)
            {
                var controllers = SafeList(() => mvc.GetMvcControllers(app));
                controllers.Add("Назад");

                var ctrl = InputConsole.SingleSelect($"Mvc-контроллеры ({app})", controllers, ref args);
                if (ctrl == "Назад")
                    return true;

                if (!OnMvcControllerSelected(mvc, app, ctrl, args))
                    return true;
            }
        }

        static bool OnMvcControllerSelected(MyMvcServiceModel mvc, string app, string ctrl, string[] args)
        {
            while (true)
            {
                var actions = SafeList(() => mvc.GetMvcControllerActions(app, ctrl));
                actions.Add("Назад");

                var action = InputConsole.SingleSelect($"Методы ({app}/{ctrl})", actions, ref args);
                if (action == "Назад")
                    return true;

                OnMvcControllerActionSelected(mvc, app, ctrl, action);
                InputConsole.ConfirmContinue();
            }
        }

        public static void OnMvcControllerActionSelected(MyMvcServiceModel mvc, string app, string ctrl, string action)
        {
            try
            {
                var actionModel = mvc.GetMvcControllerActionModel(app, ctrl, action);
                InputConsole.Write(actionModel.ToJsonOnScreen());
            }
            catch (Exception ex)
            {
                InputConsole.Error(ex.InnerException?.Message ?? ex.Message);
            }
        }

        static List<string> SafeList(Func<List<string>> loader)
        {
            try
            {
                return loader() ?? new List<string>();
            }
            catch (Exception ex)
            {
                InputConsole.Warn($"БД MVC недоступна: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
