
using System.Threading.Tasks;

namespace MedOrg.Client.Services
{
    public interface IClientHttp
    {

        public Task<TResponse> DoGet<TResponse>(string url );
        public Task<TResponse> DoPost<TResponse>(string url, object request=null);
    }
}