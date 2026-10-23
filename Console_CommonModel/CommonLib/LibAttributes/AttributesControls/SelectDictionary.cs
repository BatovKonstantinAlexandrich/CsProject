using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class NewSelectDictionary2 : ControlAttribute
{
    private readonly string expression;
    public override bool IsValidValue(object? value) => true;

    public NewSelectDictionary2() : base(ControlTypes.Select) { }

    public NewSelectDictionary2([NotNullNotEmpty] string expression) : base(expression)
    {
        this.expression = expression;
    }


    public override AppViewBase CreateControl(InputFormField field)
    {
        return new Select()
        {

            Options = new()
        };
    }

    public override AppViewBase CreateView(InputFormField field)
    {
        var combobox = new ComboboxView { Text = field.Label };
        Dictionary<int, object> dataset;
        string propertyBinding;

        try
        {
            dataset = GetDataDictionary();
            propertyBinding = GetPropertyName();
        }
        catch (Exception ex)
        {
            this.Error($"Ошибка при загрузке справочника: {ex.Message}");
            return combobox;
        }

        var options = dataset.ToDictionary(
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

        combobox.OnSelected += text =>
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            var id = options.FirstOrDefault(kv => kv.Value == text).Key;
            field.Value = id;
            field.OnInput?.Invoke(id.ToString());
        };

        return combobox;
    }

    public Dictionary<int, object> GetDataDictionary()
    {
        Type entityType = GetEntityType();
        Dictionary<int, object> dataset = this.SelectDictionaryData(entityType.GetTypeName());
        return dataset;
    }

    public string GetPropertyName()
    {
        return expression.Split(",")[1].Replace("\"", "");
    }

    public Type GetEntityType()
    {
        var entityTypeName = expression.Split(",")[0];
        var entityType = entityTypeName.Replace("\"", "").ToType();
        return entityType;
    }

    public Dictionary<int, object> SelectDictionaryData(string entityTypeName)
    {
        Dictionary<int, object> result = new();
        var entityType = entityTypeName.ToType();
        var dbContextType = entityType.GetDbContextWithEntityType();
        using (var db = dbContextType.New<DbContext>())
        {
            foreach (dynamic p in db.GetDbSet(entityTypeName))
            {
                object next = p;
                object id = next.GetValue("Id");
                if (id is null)
                {
                    id = next.GetValue("id");
                }
                result[id.ToString().ToInt()] = next;
            }
        }
        return result;
    }

    public override string Validate(object model, string property, object value)
    {
        return null;
    }

    public override string Validate(object value)
    {
        return null;
    }
}
