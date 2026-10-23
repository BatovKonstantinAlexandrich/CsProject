using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System.ComponentModel;

/// <summary>
/// Активные объекты.
/// Проходят процедуру авторизации в приложении.
/// Система будет отслеживать их состояние.
/// </summary>
public class ActiveObject : BaseEntity, IActiveObject
{

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

    public async Task DoCheck(object context, string key)
    {
        this.Info($"DoCheck({context},{key})");
        await Task.CompletedTask;
    }

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
        //this.Info("CREATED");
    }
} 