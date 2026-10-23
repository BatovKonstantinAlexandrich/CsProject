using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.MessageModule
{


    [Label("Информационное сообщение")]
    public class MessageView : AppViewBase
    {

        [Label("Title")]
        public string Title { get => Get<string>(nameof(Title)); set => Set(nameof(Title), value); }

        [Label("Chapter")]
        public string Chapter { get => Get<string>(nameof(Chapter)); set => Set(nameof(Chapter), value); }

        [Label("Message")]
        public string Message { get => Get<string>(nameof(Message)); set => Set(nameof(Message), value); }

        public MessageView()
        {
            Title = "";
            Chapter = "";
            Message = "";
        }
    }
}
