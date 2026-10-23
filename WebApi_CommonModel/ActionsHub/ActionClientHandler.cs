using Microsoft.Extensions.Logging;

using System;

/// <summary>
/// Обработчик ответных сообщений, регистрируется в ClientContext 
/// после передачи запроса
/// </summary>
public class ActionClientHandler
{
    public Action<object> OnSuccess { get; set; } = response => {
        LoggerFactory.Create(options => options.AddConsole()).CreateLogger<ActionClientHandler>().LogInformation(response is null? "null": response.ToJsonOnScreen());
    };
    public Action<string> OnError { get; set; } = error => {
        LoggerFactory.Create(options => options.AddConsole()).CreateLogger<ActionClientHandler>().LogError(error is null ? "null" : error);
    };


    public ActionClientHandler()
    {
    }

    public ActionClientHandler(Action<object> onSuccess, Action<string> onError)
    {
        this.OnSuccess += p=> { };
        OnError = onError;
    }    
}

