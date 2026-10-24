using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.Authorization
{
    public interface IBusinessProcessBuilder
    {
        public void StartWith<InputModel, OutputModel, ErrorModel>(string name);
        public BusinessProcess BuildBusiness();
        IBusinessProcessBuilder CreateUserBusinessFunction<TBusinessResource, TInputMessageProtocol>(Dictionary<string, BusinessFunction> logic);
    }

}
