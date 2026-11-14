using WinFormsApp.Console.Console_ClassModule.ClassDataModel;



namespace WinFormsApp.Console.Console_ClassModule.ClassServiceModel
{
    public static class ClientClassDbContextServiceExtensions
    {
        public static ClassModel ToClassModel(this Type ptype)
        {
            return AppProviderService.GetInstance().Get<ClientClassDbContextService>().CreateAndSaveClassModel(ptype);
        }
    }
}
