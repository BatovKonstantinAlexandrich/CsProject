using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System.Collections.Concurrent;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SearchDataList<TEntity> : ChangingDataList<TEntity>, ISearch<TEntity>, IChangeSupport
        where TEntity : BaseEntity, IChangeSupport

    {
        private IChangeSupport changeSupport = new ChangeSupport();
        public string SearchQuery { get => Get<string>(nameof(SearchQuery)); set => Set(nameof(SearchQuery), value); }
        public ChangingList<string> SearchOptions { get => Get<ChangingList<string>>(nameof(SearchOptions)); set => Set(nameof(SearchOptions), value); }
        public ChangingList<string> SearchWords { get => Get<ChangingList<string>>(nameof(SearchWords)); set => Set(nameof(SearchWords), value); }
        public override ChangingList<TEntity> visibleEntities { get => Get<ChangingList<TEntity>>(nameof(visibleEntities)); set => Set(nameof(visibleEntities), value); }




        public SearchDataList(IEntityFasade<TEntity> entityFasade) : base(entityFasade)
        {
            visibleEntities.OnChanged += OnVisibleDataChanged;
            AddPropertyChangeListener(nameof(SearchQuery), evt =>
            {
                SearchWords.Clear();
                SearchWords.AddRange(evt.After.ToString().ToUpper().Trim().Replace("  ", " ").SplitWords());
                UpdateVisibleData();
            });

            SearchOptions = new();
            SearchWords = new();
            SearchQuery = "";
            visibleEntities = new();
            UpdateVisibleData();
        }



        public void OnInput(string query)
        {
            SearchWords.Clear();
            var words = query.ToUpper().Trim().Replace("  ", " ").SplitWords();
        }

        public void OnSearch(string query)
        {
            SearchQuery = query;
        }

        private void UpdateVisibleData()
        {
            visibleEntities.Clear();
            var dataset = this.Where(item => filter(item)).ToList();
            visibleEntities.AddRange(dataset);
        }

        public bool Changed { get => changeSupport.Changed; set => changeSupport.Changed = value; }
        public ConcurrentDictionary<string, object> DataState { get => changeSupport.DataState; set => changeSupport.DataState = value; }
        public bool ValidateOnInput { get => changeSupport.ValidateOnInput; set => changeSupport.ValidateOnInput = value; }

        public void ApplyPropertyChangedEvent(PropertyChangedMessage evt)
        {
            changeSupport.ApplyPropertyChangedEvent(evt);
        }

        public void RemovePropertyChangeListener(Action<PropertyChangedMessage> listener)
        {
            changeSupport.RemovePropertyChangeListener(listener);
        }

        public void AddPropertyChangeListener(Action<PropertyChangedMessage> listener)
        {
            changeSupport.AddPropertyChangeListener(listener);
        }

        public void AddPropertyChangeListener(string property, Action<PropertyChangedMessage> listener)
        {
            changeSupport.AddPropertyChangeListener(property, listener);
        }

        public T Get<T>(string key)
        {
            return changeSupport.Get<T>(key);
        }

        public void OnGet<T>(string key, T value)
        {
            changeSupport.OnGet(key, value);
        }

        public void Set<T>(string key, T value)
        {
            changeSupport.Set(key, value);
        }

        public bool WasChanged()
        {
            return changeSupport.WasChanged();
        }

        public Action<PropertyChangedMessage> OnChange { get => changeSupport.OnChange; set => changeSupport.OnChange = value; }

        public void AddPropertyChangeListener<T>(string property, Action<T> changed)
        {
            changeSupport.AddPropertyChangeListener(property, changed);
        }

        public void Bind(string propertyName, IChangeSupport target, string relativePropertyName)
        {
            changeSupport.Bind(propertyName, target, relativePropertyName);
        }
    }
}
