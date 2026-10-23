using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.EntityFasade
{
    public class EntitySearch<TModel> : IEntitySearch<TModel>
    {
        public Task<Dictionary<string, string>> Lastest(int size)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetMostPopulare(int size)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> GetKeywords(string query)
        {
            throw new NotImplementedException();
        }

        public List<string> GetIndexes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TModel> Search(string query)
        {
            throw new NotImplementedException();
        }

        public Task<TModel[]> Search(string query, int page, int size)
        {
            throw new NotImplementedException();
        }

        public Task<TModel[]> Search(string query, int page, int size, params string[] sorting)
        {
            throw new NotImplementedException();
        }
    }
}
