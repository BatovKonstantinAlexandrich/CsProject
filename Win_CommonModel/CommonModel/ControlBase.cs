using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.Others;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win_BusinessApp.CommonModel
{
    public class ControlBase : UserControl, IComponent
    {
        public AppViewBase viewModel { get; set; }
        public ComponentBase component { get; set; }
        public bool Capture { get; internal set; }
        public bool IsHandleCreated { get; internal set; }
        public bool UseModernStyle { get; internal set; }
        public bool InvokeRequired { get; internal set; }
          
 
        internal Point PointToClient(Point point)
        {
            throw new NotImplementedException();
        }

        internal void Invoke(Action openSignup)
        {
            throw new NotImplementedException();
        }

        public ISite? Site { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<string, string> OnInput { get; internal set; }

        public event EventHandler? Disposed;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
