using System.Collections.ObjectModel;

namespace BlazorErp.Shared.CommonModel.ViewCore;

/// <summary>Базовая модель представления (упрощённый аналог ViewItem из Console_CommonModel).</summary>
public class ViewItem
{
    public string ViewTypeName => GetType().Name;

    public ViewItem? Parent { get; set; }

    public ObservableCollection<ViewItem> Controls { get; } = [];

    public string Title { get; set; } = string.Empty;

    public bool Visible { get; set; } = true;

    public bool Changed { get; set; }

    public Dictionary<string, object?> DataState { get; } = new(StringComparer.OrdinalIgnoreCase);

    public virtual void Append(ViewItem item)
    {
        item.Parent = this;
        Controls.Add(item);
    }

    public virtual void RemoveFromParent()
    {
        if (Parent != null)
        {
            Parent.Controls.Remove(this);
            Parent = null;
        }
    }

    public T? Get<T>(string name) where T : class =>
        DataState.TryGetValue(name, out var value) ? value as T : null;

    public void Set(string name, object? value) => DataState[name] = value;
}
