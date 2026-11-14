using ClassModelLib.ActionsClient;

using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using Org.BouncyCastle.Asn1.X509;

using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Активные объекты.
/// Проходят процедуру авторизации в приложении.
/// Система будет отслеживать их состояние.
/// </summary>
public class ActiveObject : AsyncContext, IActiveObject
{
    internal ConcurrentQueue<ActionResponse> output;
    internal ConcurrentQueue<ActionRequest> input;

    public void DoAction(string name, Dictionary<string,object> args, Action<object> onsuccess, Action<string> onerror)
    {
        if(input is null)
            throw new ArgumentNullException(nameof(input));
        
        this.input.Enqueue(new()
        {
            SerialKey = this.Put(new()
            {
                OnError = onerror,
                OnSuccess = onsuccess
            }),
            AccessToken = this.SecretKey,
            ActionName = name,
            MessageObject = args
        });
    }

    public object Invoke(string name, Dictionary<string, object> args)
    {
        var method = this.GetType().GetMethods().First(m => m.Name == name);
        object[] pars = method.GetParameters().Select(par => args[name]).ToArray();
        return method.Invoke(this, pars);
    }

    [DisplayName("Последнее посещение")]
    [NotInput]
    public long LastActive { get; set; }

    [DisplayName("Последняя активность")]
    [NotInput]
    public DateTime LastActiveTime { get; set; }

    [Label("Программа связи")]
    [NotInput]
    public string UserAgent { get; set; }

    [Label("Онлайн")]
    [NotInput]
    public bool IsActive { get; set; } = false;

    [Label("Секретный ключ")]
    [NotInput]
    public string SecretKey { get; set; } = "";

    [InputUrl("Значение не похоже на URL")]
    [NotInput]
    public string URL { get; set; } = "";


    [Label("последний ИП")]
    [InputTcpIpAddress]
    [NotInput]
    public string Ip4 { get; set; }

    [Label("Геопозиция")]
    [NotInput]
    public DateTime GeoUpdated { get; set; } = DateTime.Now;

    [Label("Широта")]
    [NotInput]
    public string Latitude { get; set; } = "60.000000";

    [Label("Долгота")]
    [NotInput]
    public string Longitude { get; set; } = "30.000000";

    public ActiveObject()
    {
    }
} 