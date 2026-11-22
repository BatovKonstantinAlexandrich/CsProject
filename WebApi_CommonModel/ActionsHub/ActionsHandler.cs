using Microsoft.Extensions.Logging;

using System;

/// <summary>
/// Обработчик ответных сообщений, регистрируется в ClientContext 
/// после передачи запроса
/// </summary>
public class ActionsHandler
{
    public Action<object> OnSuccess { get; set; } = response => {
        LoggerFactory.Create(options => options.AddConsole()).CreateLogger<ActionsHandler>().LogInformation(response is null? "null": response.ToJsonOnScreen());
    };
    public Action<string> OnError { get; set; } = error => {
        LoggerFactory.Create(options => options.AddConsole()).CreateLogger<ActionsHandler>().LogError(error is null ? "null" : error);
    };


    public ActionsHandler()
    {
    }

    public ActionsHandler(Action<object> onSuccess, Action<string> onError)
    {
        OnSuccess = onSuccess;
        OnError = onError;
    }    
}

