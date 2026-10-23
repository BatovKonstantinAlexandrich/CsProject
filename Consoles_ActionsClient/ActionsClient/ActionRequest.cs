using System.Collections.Generic;

/// <summary>
/// Модель запроса "Удалённый вызов процедуры"
/// </summary>
public class ActionRequest
{


    /// <summary>
    /// Ключ доступа клиента
    /// </summary>
    //[JsonProperty("token")]
    public string AccessToken { get; set; }


    /// <summary>
    /// Идентификатор сообщения
    /// </summary>
    //[JsonProperty("mid")]
    public string SerialKey { get; set; }


    /// <summary>
    /// Имя процедуры
    /// </summary>
    //[JsonProperty("request.path")]
    public string ActionName { get; set; }


    /// <summary>
    /// Параметры выполнения
    /// </summary>
    //[JsonProperty("request.pars")]
    public Dictionary<string, string> MessageObject { get; set; }


}