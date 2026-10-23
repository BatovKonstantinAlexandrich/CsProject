using System.ComponentModel;

namespace BlazorErp.Shared.CommonModel.ViewCore;

/// <summary>Заголовок и описание представления (AppViewBaseModel из Console_CommonModel).</summary>
public class AppViewBaseModel : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private string _description = string.Empty;

    public string Title
    {
        get => _title;
        set
        {
            if (_title == value)
                return;
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (_description == value)
                return;
            _description = value;
            OnPropertyChanged(nameof(Description));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler? OnCancel;
    public event EventHandler? OnSave;

    public void RaiseCancel() => OnCancel?.Invoke(this, EventArgs.Empty);
    public void RaiseSave() => OnSave?.Invoke(this, EventArgs.Empty);

    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
