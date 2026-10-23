using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;

public class HttpMessage
{

    public string ClientIp { get; set; }
    public Dictionary<string, string> Cookies { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public byte[] Content { get; set; }
    public string Location { get; internal set; }
    public Dictionary<string, object> Query { get; internal set; }
}
public class RequestMessage
{
    public string AccessToken { get; set; }
    public string SerialKey { get; set; }
    public object MessageObject { get; set; }
    public string ActionName { get; set; }

    public Dictionary<string, object> GetDictionary()
    {
        return JsonConvert.DeserializeObject<Dictionary<string, object>>(JObject.FromObject(MessageObject).ToString());
    }
}