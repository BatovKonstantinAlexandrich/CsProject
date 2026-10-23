namespace Blazor_CommonModel.Services
{

    public class KeyboardTests : TestingElement
    {
        public KeyboardTests()
        {
        }

        public KeyboardTests(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            var view = new AppViewBase();
            var form = view.ToControl();
            form.KeyPreview = true;
            var service = new KeyboardService();
            service.LoadKeyboardConfig();
            Dictionary<string, Action> commands = new Dictionary<string, Action>() {
                { "SetDesignMode(true)",()=>{ } },
                { "q",()=>{
                    Console.WriteLine("1");
                } }
            };

            this.Info(service.KeyboardConfig.Values.ToJsonOnScreen());
            service.Setup(view, commands);
            form.RunAtForm();
        }
    }
}
