using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationModel.ServiceModel
{


    [Label("Ресурс доступа к службам")]
    public class ServiceResource : LibModels.ServiceBuilderModule.ServiceModel
    {


        public int ServiceContextId { get; set; }
        public ServiceContext ServiceContext { get; set; }

    }
}
