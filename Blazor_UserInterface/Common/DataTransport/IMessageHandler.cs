using System.Threading.Tasks;

namespace HospitalConstructor.Server
{
    public interface IMessageHandler
    {
        public Task<string> OnMessage(string message);

    }
}