using Microsoft.Extensions.Logging;

using System;

/// <summary>
/// Обработчик ответных сообщений, регистрируется в ClientContext 
/// после передачи запроса
/// </summary>
public class ActionHandler
{
    public Action<object> OnSuccess { get; set; } = response => {
        LoggerFactory.Create(options => options.AddConsole()).CreateLogger<ActionHandler>().LogInformation(response is null? "null": response.ToJsonOnScreen());
    };
    public Action<string> OnError { get; set; } = error => {
        LoggerFactory.Create(options => options.AddConsole()).CreateLogger<ActionHandler>().LogError(error is null ? "null" : error);
    };


    public ActionHandler()
    {
    }

    public ActionHandler(Action<object> onSuccess, Action<string> onError)
    {
        OnSuccess = onSuccess;
        OnError = onError;
    }    
}

