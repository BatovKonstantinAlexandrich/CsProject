using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System.Net.Http;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.ComboboxModule;


[Label("Выбор из интернет справочника")]
public class SingleSelectApiAttribute : ControlAttribute
{
    private readonly string _resource;

    public SingleSelectApiAttribute() : base(ControlTypes.Select)
    {
    }

    public SingleSelectApiAttribute(string resource) : base(ControlTypes.Select)
    {
        _resource = resource;
    }

    //public override Control CreateWinFormsControl(InputFormField field)
    //{
    //    var options = LoadOptions();
    //    var factory = new ControlsFactory();
    //    var combobox = factory.CreateSelectObjectbyIdBox(options);
    //    combobox.SelectedIndexChanged += (_, _) =>
    //    {
    //        try
    //        {
    //            if (combobox.SelectedIndex == -1)
    //            {
    //                field.OnInput(null);
    //                return;
    //            }

    //            var item = combobox.Items[combobox.SelectedIndex];
    //            var id = options.FirstOrDefault(kv => kv.Value == $"{item}").Key;
    //            field.OnInput(id.ToString());
    //        }
    //        catch (Exception ex)
    //        {
    //            this.Error(ex);
    //        }
    //    };
    //    return combobox;
    //}

    public override AppViewBase CreateView(InputFormField field)
    {
        var combobox = new ComboboxView { Text = field.Label };
        Dictionary<int, string> options;

        try
        {
            options = LoadOptions();
        }
        catch (Exception ex)
        {
            this.Error($"Ошибка при работе с удалённым источником: {ex.Message}");
            return combobox;
        }

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

    private Dictionary<int, string> LoadOptions()
    {
        if (string.IsNullOrWhiteSpace(_resource))
            throw new ArgumentException("Не указан URL ресурса для SingleSelectApiAttribute.");

        using var http = new HttpClient();
        var response = http.GetAsync(_resource).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        var dict = json.FromJson<Dictionary<int, string>>();
        if (dict is not null && dict.Count > 0)
            return dict;

        var list = json.FromJson<List<Dictionary<string, object>>>();
        if (list is null)
            return new Dictionary<int, string>();

        var result = new Dictionary<int, string>();
        var index = 0;
        foreach (var row in list)
        {
            var id = row.ContainsKey("Id") ? row["Id"]?.ToString()?.ToInt() ?? index
                : row.ContainsKey("id") ? row["id"]?.ToString()?.ToInt() ?? index
                : index;
            var name = row.ContainsKey(nameof(NamedObject.Name))
                ? row[nameof(NamedObject.Name)]?.ToString()
                : row.Values.FirstOrDefault()?.ToString();
            if (!string.IsNullOrWhiteSpace(name))
                result[id] = name;
            index++;
        }

        return result;
    }
}
