using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.Connection
{
    //"Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;"
    [Description("")]
    public class SqlServerTrustedConnection : SqlServerStandardConnection
    {
        public SqlServerTrustedConnection() : base() { }
    }
}
