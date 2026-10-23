namespace WinFormsApp.AppComponents.AppControls
{
    public class AppViewRootControl : IControlService<AppViewRootControl>
    {
        public AppViewRootControl programDbContext { get; set; }

        public void SetControl(AppViewRootControl item)
        {
            programDbContext = item;
        }
    }
}
