using Console_CommonModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.Others;
using System.ComponentModel;
using Win_CommonModel.CommonModel;
using WinFormsApp.Console.Win_MaterialModule;


namespace Win_BusinessApp
{

    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            AppProviderService.GetInstance().AddServiceImplementation<IComponentsFactory<IControlBase>, WinComponentsFactory>();
            InputConsole.clientAPI = new DialogService();

            CommonProgram.RunTests<Program>(args);

            var root = new AppViewRoot();
            root.RunAtForm();

        }
    }

}