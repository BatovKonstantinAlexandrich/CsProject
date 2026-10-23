namespace Console_CommonModel.CommonLib.LibModels.Console_WebModule
{
    internal interface IAuthenticationApi
    {
        public bool can(string url, Dictionary<string, string> queryParams, Dictionary<string, string> headers);

    }
}
