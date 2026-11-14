using System;
using System.Threading.Tasks;
[Label("Активный объект сеанса")]
public interface IActiveObject
{
    [Label("Признак активности")]
    [InputBool]
    bool IsActive { get; set; }

    [Label("Время последнего обращения")]
    [InputNumber]
    long LastActive { get; set; }

    [Label("Время последнего обращения")]
    [InputDateTime]
    DateTime LastActiveTime { get; set; }

    [Label("Ключ доступа")]    
    string SecretKey { get; set; }

    [Label("Адрес текущей странице")]
    [InputUrl]
    string URL { get; set; }

    [Label("ИП адрес клиента")]
    [InputTcpIpAddress]
    string Ip4 { get; set; }

    [Label("Браузер пользователя")]
    [InputEngWord]
    string UserAgent { get; set; }

 
}
