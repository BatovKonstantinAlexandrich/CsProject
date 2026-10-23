using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.ViewModels.Connection
{
    //s"=myServerAddress;Catalog=myDataBase;ProtectionLevel=Connect;"
    [Description("")]
    public class OlapAuthenticatedConnection
    {

        [Alias("Data Source")]
        [NotNullNotEmpty("Необходимо ввести источник данных")]
        public string DataSource { get; set; }

        [Alias("Catalog")]
        [NotNullNotEmpty("Необходимо ввести каталог")]
        public string Catalog { get; set; }



        public OlapAuthenticatedConnection() : base()
        {

        }
    }

}
