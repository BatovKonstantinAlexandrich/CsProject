namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.StatusModule
{
    public interface IStatusView
    {
        public void PutAction(string name, Action todo, int timeout = 100000  );
        
    }
}