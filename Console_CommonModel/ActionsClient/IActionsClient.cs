using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
