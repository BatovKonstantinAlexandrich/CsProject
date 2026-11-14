using System;

/// <summary>
/// Обработчик ответных сообщений, регистрируется в ClientContext 
/// после передачи запроса
/// </summary>
public class ActionHandler
{
    public Action<object> OnSuccess { get; set; }
    public Action<string> OnError { get; set; }
}

