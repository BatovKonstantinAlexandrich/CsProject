using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.Console.Console_ClassModule.ClassDataModel;

namespace ClassModelLib.ActionsClient
{
    public interface IActionsClient
    {
        public void OnPublish(MethodModel method);
        public void OnUnPublish(MethodModel method);       
        public void OnConnected();
        public void OnDisconnected(Exception ex);
    }
}
