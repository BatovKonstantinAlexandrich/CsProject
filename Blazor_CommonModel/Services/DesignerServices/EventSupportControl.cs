namespace WinFormsApp.DesignerServices
{
    [Label("События")]
    public class EventSupportControl : IControlService<EventSupport>
    {
        public EventSupport Control { get; set; }

        public EventSupportControl()
        {
        }

        public EventSupportControl(EventSupport view)
        {
            SetControl(Control = view);
        }


        public void SetControl(EventSupport view)
        {
            Control = view;
        }



        [Label("Связать состояние")]
        public void AddValidaionStateChangedListener(Action<bool> onchanged)
        {
            
        }
    }

}