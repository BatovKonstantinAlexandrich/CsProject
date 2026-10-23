using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.SharedViewCollections.CheckedListModule
{
    [Label("Список выбора")]
    public class CheckedListView : ControlView
    {
        [Label("Надпись")]
        [NotNullNotEmpty()]
        public virtual string Text { get => Get<string>("Text"); set => Set("Text", value); }

        [Label("Включена/Выключена")]
        public virtual bool Enabled { get => Get<bool>("Enabled"); set => Set("Enabled", value); }

        [Label("Элементы выбора")]
        [InputPrimitiveCollection()]
        public ChangingList<object> ListItems { get; set; }

        [Label("Отмеченные элементы")]
        [NotInput]
        public ChangingList<object> CheckedItems { get; set; }

        /// <summary>Смена отметки пункта: индекс, элемент, новое состояние.</summary>
        [NotInput]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Label("On Item Checked Changed")]
        public Action<int, object, bool> OnItemCheckedChanged { get; set; } = (_, _, _) => { };

        /// <summary>Смена отметки (как у <see cref="CheckboxView.OnCheckedChanged"/>), <see cref="PropertyChangedMessage{T}.Source"/> — элемент списка.</summary>
        [NotInput]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Label("On Checked Changed")]
        public Action<object, PropertyChangedMessage<bool>> OnCheckedChanged { get; set; } = (_, _) => { };

        public CheckedListView()
        {
            Enabled = true;
            Text = "Выбор элементов";
            ListItems = new();
            CheckedItems = new();
            MinWidth = MinHeight = 200;
        }

        public void ApplyItemChecked(int index, object item, bool isChecked)
        {
            if (item is null)
                return;

            if (isChecked)
            {
                if (!CheckedItems.Contains(item))
                    CheckedItems.Add(item);
            }
            else
            {
                CheckedItems.Remove(item);
            }

            var wasChecked = !isChecked;
            OnCheckedChanged(item, new PropertyChangedMessage<bool>
            {
                Source = item,
                Property = "Checked",
                Before = wasChecked,
                After = isChecked
            });
            OnItemCheckedChanged(index, item, isChecked);
            Changed = true;
        }
    }
}
