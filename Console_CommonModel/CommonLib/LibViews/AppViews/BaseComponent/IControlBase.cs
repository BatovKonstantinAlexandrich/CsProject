using Console_CommonModel.CommonLib.Others;

using System.ComponentModel;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent
{

    public interface IControlRoot
    {
        public void Dispose() { }  
        public void Close();
        public void Invoke(Action showErr);
        public void SetSize(int v1, int v2);
        public IControlBase ShowDialog(string title = "");
        public IControlBase ToBaseForm(string title = "");
        public IControlBase RunAtForm(string title = "");
        public bool Capture { get; set; }
        public bool IsHandleCreated { get; set; }
        public bool InvokeRequired { get; set; }
        public bool IsDisposed { get; set; }
    }
    public interface IControlBase :
        IControlView,
        IComponent,
        IContainer,

        IObjectWithId<string>,
        ITreeContainer<IControlBase>, 
        IEventSupport, 
        IChangeSupport,
        IClientAPI,
        IStyledItem,
        IViewEventSupport,
        INamedObject,
        IControlRoot

    {
        AppViewBase ViewModel { get; set; }
        ComponentBase component { get; set; }


        bool UseModernStyle { get; set; }
       
        bool KeyPreview { get; set; }
        
         

        IControlBase Left { get; set; }
        IControlBase Right { get; set; }
        IControlBase Top { get; set; }
        IControlBase Bottom { get; set; }
        object AutoSizeMode { get; set; }
        bool AllowDrop { get; set; }
        Size MinimumSize { get; set; }
        Size PreferredSize { get; set; }
        Action<object, object> HandleCreated { get; set; }

        void SetLeft(IControlBase value);
        void SetTop(IControlBase value);
        void SetRight(IControlBase value);
        void SetBottom(IControlBase value);

        IControlBase RunAtForm(string title="");
        IControlBase ShowDialog(string title = "");
        IControlBase GetAppForm();
        

        Point PointToClient(Point point);
        void SetSize(int v1, int v2);
        void SetBounds(int x, int y, int w, int h);
        //void BeginInvoke(Action action);
    }
}