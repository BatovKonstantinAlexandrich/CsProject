namespace BlazorErp.Services;

public interface IMessageComposeDraftService
{
    void SetDraft(string subject, string body);
    bool TryTakeDraft(out string subject, out string body);
}
