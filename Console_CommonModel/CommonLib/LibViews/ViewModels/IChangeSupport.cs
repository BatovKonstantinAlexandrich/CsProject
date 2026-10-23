using System.Collections.Concurrent;

public interface IChangeSupport
{
    bool Changed { get; set; }
    ConcurrentDictionary<string, object> DataState { get; set; }
    bool ValidateOnInput { get; set; }
    void ApplyPropertyChangedEvent(PropertyChangedMessage evt);
    /// <summary>
    /// Событие изменения значения свойства
    /// </summary>
    void Bind(string propertyName, IChangeSupport target, string relativePropertyName);


    Action<PropertyChangedMessage> OnChange { get; set; }
    void RemovePropertyChangeListener(Action<PropertyChangedMessage> listener);
    void AddPropertyChangeListener(Action<PropertyChangedMessage> listener);
    void AddPropertyChangeListener<T>(string property, Action<T> changed);
    void AddPropertyChangeListener(string property, Action<PropertyChangedMessage> listener);
    T Get<T>(string key);
    void OnGet<T>(string key, T value);
    void Set<T>(string key, T value);
    bool WasChanged();
}