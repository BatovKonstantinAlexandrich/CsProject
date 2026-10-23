using Console_CommonModel.ActionModels;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Org.BouncyCastle.Asn1.X509;

using System.ComponentModel;

namespace Console_CommonModel.CommonLib.LibViews.ViewCustomizers
{

    /// <summary>    
    /// Управление клавиатурой выполняется через файл конфигурации.   
    /// </summary>
    public class KeyboardService : Component
    {
        /// <summary>
        /// Конфигурация клавиатуры 'код клавиши' - 'команда'
        /// </summary>
        public Dictionary<string, string> KeyboardConfig { get; set; }
        public Dictionary<string, Action> Target { get; set; }

        public KeyboardService()
        {

        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            this.Info($"OnKeyDown: {sender} => {e.KeyCode} => {e.KeyValue}");
        }

        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            this.Info($"OnKeyUp: {sender} => {e.KeyCode} => {e.KeyValue}");

        }

        public void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            TryExecuteBinding(e.KeyChar, e);
        }

        public void OnKeyDownExecute(object sender, KeyEventArgs e)
        {
            if (!e.Control && !e.Alt && !e.Shift)
                return;

            var chord = KeyboardChordCodec.FormatFromKeyEvent(e);
            if (string.IsNullOrEmpty(chord))
                return;

            if (TryExecuteBinding(chord, e))
                return;

            TryExecuteBinding(KeyboardChordCodec.NormalizeChord(chord), e);
        }

        public bool TryExecuteBinding(string key, EventArgs e)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            LoadKeyboardConfig();
            if (!KeyboardConfig.TryGetValue(key, out var command))
                return false;

            try
            {
                if (Target is not null)
                    Expressions.Compile(command, Target);
            }
            catch (Exception ex)
            {
                this.Error($"Исключение при выполнении команды для «{key}»: {ex.Message}", ex);
            }

            if (e is KeyEventArgs ke)
            {
                ke.Handled = true;
                ke.SuppressKeyPress = true;
            }

            return true;
        }






        public void Setup(AppViewBase form, Dictionary<string, Action> target)
        {
            LoadKeyboardConfig();
            Target = target;
            WireAppViewKeyboard(form, target);
        }

        public void Setup(AppViewBase form, string filename)
        {
            var config = LoadKeyboardConfig(filename);
            Target = new Dictionary<string, Action>(config.Select(kv => new KeyValuePair<string, Action>(kv.Key, () =>
            {
                try
                {
                    form.GetAppRootView().ForEach(p =>
                    {
                        try
                        {
                            Expressions.Compile(kv.Value, form);
                        }
                        catch (Exception ex)
                        {
                            this.Error(ex);
                        }
                    });
                }
                catch (Exception ex)
                {
                    this.Error($"Исключение при нажатии клавиши {kv.Key}: {ex.Message}");
                }
            })));

            WireAppViewKeyboard(form, Target);
        }

        static void WireAppViewKeyboard(AppViewBase form, Dictionary<string, Action> target)
        {
            form.OnKeyPressed += (s, e) =>
            {
                var key = e.KeyChar;
                if (target.ContainsKey(key))
                    target[key]();
            };

            form.OnKeyDown += (s, e) =>
            {
                if (!e.Control && !e.Alt && !e.Shift)
                    return;

                var chord = KeyboardChordCodec.FormatFromKeyEvent(e);
                if (string.IsNullOrEmpty(chord))
                    return;

                if (target.ContainsKey(chord))
                {
                    target[chord]();
                    return;
                }

                var normalized = KeyboardChordCodec.NormalizeChord(chord);
                if (target.ContainsKey(normalized))
                    target[normalized]();
            };
        }

        /// <summary>
        /// Считывание конфигурации управления клавиатурой
        /// </summary>
        public Dictionary<string, string> LoadKeyboardConfig(string filename = KeyboardConfigDefaults.DefaultFileName)
        {
            var path = KeyboardConfigDefaults.ResolveConfigPath(filename);
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                KeyboardConfig = json.FromJson<Dictionary<string, string>>();
            }

            return KeyboardConfig = new Dictionary<string, string>();
        }

        public void SaveKeyboardConfig(Dictionary<string, string> data, string filename = KeyboardConfigDefaults.DefaultFileName)
        {
            var path = KeyboardConfigDefaults.ResolveConfigPath(filename);
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(path, data.ToJson());
            KeyboardConfig = new Dictionary<string, string>(data);
        }

        /// <summary>Проверка конфигурации перед сохранением.</summary>
        public static IReadOnlyList<string> Validate(Dictionary<string, string> data)
        {
            var errors = new List<string>();
            if (data is null || data.Count == 0)
            {
                errors.Add("Конфигурация пуста — добавьте хотя бы одну привязку.");
                return errors;
            }

            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var kv in data)
            {
                if (string.IsNullOrWhiteSpace(kv.Key))
                {
                    errors.Add("Есть строка с пустой клавишей.");
                    continue;
                }
                if (!seen.Add(kv.Key))
                    errors.Add($"Дублирующаяся клавиша: «{DescribeKey(kv.Key)}».");
                if (string.IsNullOrWhiteSpace(kv.Value))
                    errors.Add($"Для клавиши «{DescribeKey(kv.Key)}» не задана команда.");
            }
            return errors;
        }

        /// <summary>Отображаемое имя клавиши для спецсимволов.</summary>
        public static string DescribeKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "(пусто)";
            if (KeyboardChordCodec.LooksLikeChord(key))
                return key.Replace("Control", "Ctrl", StringComparison.OrdinalIgnoreCase);
            if (key.Length == 1 && char.IsControl(key[0]))
                return $"спец.{(int)key[0]}";
            return key;
        }

        /// <summary>Сериализация нажатой клавиши для колонки «Клавиша» (KeyChar, как в runtime).</summary>
        public static string FromKeyChar(char keyChar)
        {
            if (keyChar == '\b')
                return "\b";
            if (keyChar == '\r' || keyChar == '\n')
                return "\r";
            if (keyChar == '\t')
                return "\t";
            if (keyChar == '\u001b')
                return "\u001b";
            return keyChar.ToString();
        }
    }
}
