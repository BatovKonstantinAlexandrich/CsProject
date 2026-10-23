using Console_CommonModel.CommonLib.LibViews.AppViews.FormModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.InputModule
{
    /// <summary>
    /// Представление выбора пути к файлу (диалог + текстовое поле).
    /// </summary>
    [Label("Путь к файлу")]
    public class InputFilePathView : InputView
    {
        [Label("Путь")]
        public string Path
        {
            get => Get<string>(nameof(Path));
            set => Set(nameof(Path), value);
        }

        [Label("Фильтр диалога")]
        public string Filter
        {
            get => Get<string>(nameof(Filter));
            set => Set(nameof(Filter), value);
        }

        [Label("Заголовок диалога")]
        public string DialogTitle
        {
            get => Get<string>(nameof(DialogTitle));
            set => Set(nameof(DialogTitle), value);
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotInput]
        public Action<string> OnPathChanged { get; set; } = _ => { };

        public InputFilePathView()
        {
            Path = "";
            Filter = "All files (*.*)|*.*";
            DialogTitle = "Выберите файл";
            AddPropertyChangeListener(nameof(Path), evt =>
            {
                OnPathChanged(evt.After?.ToString() ?? "");
            });
        }

        /// <summary>Строит фильтр OpenFileDialog из списка расширений («mp4,avi» / «*.mp4;*.avi» / готовый filter с |).</summary>
        public static string BuildOpenFileFilter(string exts)
        {
            if (string.IsNullOrWhiteSpace(exts))
                return "All files (*.*)|*.*";
            if (exts.Contains('|'))
                return exts.Trim();

            var parts = exts
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => p.Length > 0)
                .Select(p =>
                {
                    if (p.StartsWith("*.", StringComparison.Ordinal))
                        return p;
                    if (p.StartsWith(".", StringComparison.Ordinal))
                        return "*" + p;
                    return "*." + p;
                })
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (parts.Count == 0)
                return "All files (*.*)|*.*";

            var joined = string.Join(";", parts);
            return $"Files ({joined})|{joined}|All files (*.*)|*.*";
        }
    }
}
