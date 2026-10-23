namespace WinFormsApp.AppComponents.AppControls
{
    public class AppViewRootControl : IControlService<AppViewRootControl>
    {
        public AppViewRootControl Control { get; set; }

        public void SetControl(AppViewRootControl item)
        {
            Control = item;
        }
    }
}
