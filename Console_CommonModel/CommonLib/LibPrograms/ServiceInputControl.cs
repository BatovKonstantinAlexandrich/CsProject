using Console_CommonModel.CommonLib.Others;

namespace Console_CommonModel.CommonLib.LibPrograms
{

    [Label("Интерфейс управление сервисом")]
    public class ServiceInputControl
    {
        private readonly object _target;
        private readonly Type _tservice;
        private readonly IConsoleСontrol _control;

        private ServiceInputControl(object target, Type tservice, IConsoleСontrol control)
        {
            this._target = target;
            this._tservice = tservice;
            this._control = control;
        }

        public static void Run<TService, TConsoleСontrol>(TService service, IConsoleСontrol control)
            where TConsoleСontrol : IConsoleСontrol
        {
            var input = new ServiceInputControl(service, typeof(TService), control);
            input.Run();
        }

        private void Run()
        {
            Dictionary<string, Action> actionMap = new();

            foreach(var kv in this._tservice.GetActions()(_target))
            {
                actionMap[kv.Key] = () => {

                    var model = this._tservice.GetActionModel(kv.Key);
                    
                }; 
            }

        }

        
    }
}
