using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static BusinessDbBehaviour;
using static MailRuService2;

namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.Authorization
{
    public class BusinessProcessValidator<TBusinessDbContext> : IBusinessProcessValidator 
        where TBusinessDbContext : BusinessDbContext
    {
        public Dictionary<string, List<string>> Validate(BusinessProcess process)
        {
            Dictionary<string, List<string>> results = new();




            results.AddRange(process.Validate());
            results.AddRange(process.Input.Validate());
            results.AddRange(process.Output.Validate());
            foreach (var function in process.BusinessFunctions)
            {
                results.AddRange(function.Validate());
            }
            return results;

        }



        public bool HasBusinessIndicator(BusinessFunction function)
        {
            var business = typeof(TBusinessDbContext).New<BusinessDbFactory>();
            return false;
        }

        public string GetBusinessIndicatorName(BusinessFunction function)
        {
            return null;
        }
        public string GetStoredProcedureName(BusinessFunction function)
        {
            return null;
        }
        public string GetTableName(MessageProtocol protocol)
        {
            return null;
        }

        public void HasStoredProcedure(BusinessFunction function)
        {

        }


        public bool HasTableFor(MessageProtocol protocol)
        {
            GetTableName(protocol);
            return false;
        }


    }
}
