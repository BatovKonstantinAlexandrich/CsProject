using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi
{
    public interface ISqlExecute
    {
        public DataTable ExecuteProcedure(string name, IDictionary<string, string> input, IDictionary<string, string> output);
        public DataTable ExecuteQuery(string SQL);
        public int PrepareQuery(string SQL);
        public DataTable ExecuteProcedure(string name, IDictionary<string, string> input);

    }
}
