using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.ComboboxModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibAttributes
{
    /// <summary>
    /// Список выбора с возможностью создания нового объекта
    /// </summary>
    [Label("Список выбора с возможностью создания нового объекта")]
    public class NewComboboxAttribute : SelectDictionaryAttribute
    {
        public NewComboboxAttribute() : base() { }

        public NewComboboxAttribute(string expression) : base(expression) { }

        public override AppViewBase CreateView(InputFormField field)
        {
            var root = new AppViewBase { AutoSize = false, MinHeight = 56 };
            var row = new AppViewBase { AutoSize = false };

            ComboboxView combobox;
            try
            {
                combobox = BuildCombobox(field, out _);
            }
            catch (Exception ex)
            {
                this.Error($"Ошибка при загрузке справочника: {ex.Message}");
                combobox = new ComboboxView { Text = field.Label };
            }

            row.LayoutFill(combobox);
            row.LayoutRight(row.CreateButton("Создать", () => OnCreateEntity(field, combobox)));
            root.LayoutFill(row);
            return root;
        }

        //public override Control CreateWinFormsControl(FieldView field)
        //{
        //    var inputField = new InputFormField
        //    {
        //        Name = field.Name,
        //        Label = field.Label,
        //        Value = field.Value,
        //        Attributes = field.Attributes,
        //        OnInput = field.OnInput
        //    };

        //    var panel = new FlowLayoutPanel
        //    {
        //        AutoSize = true,
        //        FlowDirection = FlowDirection.LeftToRight,
        //        WrapContents = false
        //    };

        //    var combo = CreateWinFormsControl(inputField);
        //    panel.Controls.Add(combo);

        //    var button = new Button { Text = "Создать", AutoSize = true };
        //    button.Click += (_, _) =>
        //    {
        //        var entityType = GetEntityType();
        //        var entityRef = entityType.New();
        //        entityRef.ToEditDialog((success, model) =>
        //        {
        //            if (!success)
        //                return;

        //            if (model is BaseEntity entity)
        //                entity.Create();

        //            RefreshWinFormsCombobox((ComboBox)combo, inputField);
        //        });
        //    };
        //    panel.Controls.Add(button);
        //    return panel;
        //}

        private void OnCreateEntity(InputFormField field, ComboboxView combobox)
        {
            var entityType = GetEntityType();
            var entityRef = entityType.New();
            ToEditDialog(entityRef, (success, model) =>
            {
                if (!success)
                    return;

                if (model is BaseEntity entity)
                    entity.Create();

                RefreshCombobox(field, combobox);
            });
        }

        private void ToEditDialog(object entityRef, Action<bool, object> value)
        {
            throw new NotImplementedException();
        }

        private void RefreshCombobox(InputFormField field, ComboboxView combobox)
        {
            var refreshed = BuildCombobox(field, out var options);
            combobox.Options.Clear();
            foreach (var item in refreshed.Options)
                combobox.Options.Add(item);

            combobox.Value = options.Values.LastOrDefault();
        }

        //private static void RefreshWinFormsCombobox(ComboBox combo, InputFormField field)
        //{
        //    var attribute = field.Attributes.Keys
        //        .Select(key => key.ToType())
        //        .First(t => t.IsExtends(typeof(NewComboboxAttribute)))
        //        .New<NewComboboxAttribute>(field.Attributes.Values.First());

        //    var dataset = attribute.GetDataDictionary();
        //    var propertyBinding = attribute.GetPropertyName();
        //    var options = dataset.ToDictionary(
        //        kv => kv.Key,
        //        kv => $"{kv.Value?.GetProperty(propertyBinding)}");

        //    combo.Items.Clear();
        //    foreach (var text in options.Values)
        //        combo.Items.Add(text);

        //    if (combo.Items.Count > 0)
        //        combo.SelectedIndex = combo.Items.Count - 1;
        //}

        private ComboboxView BuildCombobox(InputFormField field, out Dictionary<int, string> options)
        {
            var combobox = new ComboboxView { Text = field.Label };
            var dataset = GetDataDictionary();
            var propertyBinding = GetPropertyName();
            options = dataset.ToDictionary(
                kv => kv.Key,
                kv => $"{kv.Value?.GetProperty(propertyBinding)}");

            foreach (var text in options.Values)
                combobox.Options.Add(text);

            if (field.Value is not null)
            {
                var current = $"{field.Value}";
                if (options.Values.Contains(current))
                    combobox.Value = current;
                else if (int.TryParse(current, out var id) && options.TryGetValue(id, out var text))
                    combobox.Value = text;
            }
            var p = options;
            combobox.OnSelected += text =>
            {
                if (string.IsNullOrWhiteSpace(text))
                    return;

                var id = p.FirstOrDefault(kv => kv.Value == text).Key;
                field.Value = id;
                field.Target?.SetValue(field.Name, id);
                field.OnInput?.Invoke(id.ToString());
            };

            return combobox;
        }

        internal string GetExpression()
        {
            return expression;
        }
    }
}
