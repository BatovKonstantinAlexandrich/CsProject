
namespace Blazor_RoyaltyModel.Apis
{
    public interface IAuthCodeApi
    {
        public Task SendAuthCode(string phone);
        public Task<string> SubmitAuthCode(string phone, string code);
        public Dictionary<string, object> DecodeToken(string base64);
    }

}