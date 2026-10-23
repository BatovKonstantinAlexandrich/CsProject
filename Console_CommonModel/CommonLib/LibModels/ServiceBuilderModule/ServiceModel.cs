using ActionModels;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule
{
    [Label("Модель службы")]
    public class ServiceModel : IntegratedObject
    {
        [UniqValue]
        public string Schema { get; set; } // wsdl-document or openapi-schema        

        [InputSelect("WSDL,OpenAPI")]
        [NotNullNotEmpty]
        public string Type { get; set; }

        //[NotNullNotEmpty]
        [AllowNull]
        public byte[]? Dll { get; set; }






    }
}
