using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.Connection
{
    [Description("")]
    //"Data Source=myServerAddress;Catalog=myDataBase;ProtectionLevel=PktPrivacy;"
    public class OlapEncrypedConnection : OlapStandartConnection
    {
        public OlapEncrypedConnection() : base()
        {

        }
    }
}
