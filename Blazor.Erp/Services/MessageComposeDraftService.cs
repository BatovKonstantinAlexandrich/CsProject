namespace BlazorErp.Services;

public class MessageComposeDraftService : IMessageComposeDraftService
{
    private string? _subject;
    private string? _body;

    public void SetDraft(string subject, string body)
    {
        _subject = subject;
        _body = body;
    }

    public bool TryTakeDraft(out string subject, out string body)
    {
        if (_subject == null)
        {
            subject = "";
            body = "";
            return false;
        }

        subject = _subject;
        body = _body ?? "";
        _subject = null;
        _body = null;
        return true;
    }
}
