using System;

internal class AbstractHub
{
    internal void ConfirmDialog(string title, string text)
    {
    }

    internal void InfoDialog(string title, string text, string button)
    {
    }

    internal void HandleEvalResult(Func<object, object> handle, string js)
    {
    }

    internal void Callback(string action, string[] args)
    {
    }

    internal void ShowHelp(string text)
    {
    }
}
