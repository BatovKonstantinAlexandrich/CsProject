using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace Console_CommonModel.CommonLib.LibViews.AppViews.ViewControl
{
    internal sealed class ViewControlConsoleAdapter : IViewControl
    {
        private readonly AppViewBase _host;

        public ViewControlConsoleAdapter(AppViewBase host) =>
            _host = host ?? throw new ArgumentNullException(nameof(host));


        public bool UserInteractive { get; set; } = true;

        /// <summary>Режим отладки: при нехватке аргументов в неинтерактивном режиме не бросать исключение там, где это предусмотрено.</summary>
        public bool DebugMode { get; set; }




        public void Info(params object[] args) => _host.Info(args);
        public void Warn(params object[] args) => _host.Warn(args);



        private List<string> ValidateWith(Func<object, List<string>> validator, object input)
            => validator != null ? validator(input) ?? new List<string>() : new List<string>();

        private string InputWithPredicate(string title, Func<string, bool> isValid, string validationError, Func<object, List<string>> validator, ref string[] args)
            => InputText(title, input =>
            {
                var errors = ValidateWith(validator, input);
                if (isValid != null && isValid(input?.ToString() ?? string.Empty) == false && string.IsNullOrWhiteSpace(validationError) == false)
                    errors.Add(validationError);
                return errors;
            }, ref args);

        public int InputTcpPort(string title, ref string[] args)
            => InputPositiveNumber(title, value =>
            {
                var port = SafeToInt(value);
                return GetOpenPort().Any(info => info.PortNumber == port)
                    ? new List<string> { "Похоже что этот порт сейчас занят" }
                    : new List<string>();
            }, ref args);

        public T ReadKeyRepeat<T>(Action beforeRead, Action<object> onKeyPressed, Func<T> onCompleted, Action onCanceled)
        {
            try
            {
                while (true)
                {
                    beforeRead?.Invoke();
                    //onKeyPressed(ReadKey(_host).Key);
                }
            }
            catch (CompleteException)
            {
                return onCompleted();
            }
            catch (CancelException ex)
            {
                Error(ex.Message);
                onCanceled?.Invoke();
                throw;
            }
        }

        private object ReadKey(AppViewBase host)
        {
            throw new NotImplementedException();
        }

        public List<PortInfo> GetOpenPort()
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnections = properties.GetActiveTcpConnections();
            return tcpConnections.Select(p => new PortInfo(
                PortNumber: p.LocalEndPoint.Port,
                Local: string.Format("{0}:{1}", p.LocalEndPoint.Address, p.LocalEndPoint.Port),
                Remote: string.Format("{0}:{1}", p.RemoteEndPoint.Address, p.RemoteEndPoint.Port),
                State: p.State.ToString())).ToList();
        }

        public bool ConfirmContinue(string title = "")
        {
            Info(title);
            Info("\n Для продолжения нажмите OK..");
            MessageConfirm(_host, string.IsNullOrEmpty(title) ? "Продолжить?" : title + "\n\nНажмите OK для продолжения.");
            return true;
        }

        private bool MessageConfirm(AppViewBase host, string v)
        {
            throw new NotImplementedException();
        }

        public string Shift(ref string[] args)
        {
            if (args == null || args.Length == 0)
                throw new InvalidOperationException("Не достаточно аргументов");
            string value = args[0];
            var argsOut = new string[args.Length - 1];
            for (int i = 1; i < args.Length; i++)
                argsOut[i - 1] = args[i];
            args = argsOut;
            return value;
        }

        public string ReadText(string name, Func<string, List<string>> validate, ref string[] args)
        {
            string result = "";
            List<string> errors;
            do
            {
                result = ReadLine(_host, name);
                errors = validate(result) ?? new List<string>();
                foreach (var error in errors)
                    Error($"error: {error}");
                if (errors.Count != 0)
                    ConfirmContinue();
            } while (errors.Count != 0);
            return result;
        }

        public string Input(string name, ref string[] args)
            => InputString(name, _ => new List<string>(), ref args);

        public string InputString(string name, Func<string, List<string>> validate, ref string[] args)
        {
            validate ??= _ => new List<string>();

            if (UserInteractive)
            {
                string result = "";
                List<string> errors;
                do
                {
                    result = ReadLine(_host, name);
                    errors = validate(result) ?? new List<string>();
                    foreach (var error in errors)
                        Error($"error: {error}");
                    if (errors.Count != 0)
                        ConfirmContinue();
                } while (errors.Count != 0);
                return result;
            }

            string value = Shift(ref args);
            _host.Info(value);
            return value;
        }

        public bool ConfirmActionDialog(string title, Action todo)
        {
            var shellArgs = Array.Empty<string>();
            switch (SingleSelect(title, new[] { "Подтвердить", "Отменить" }, ref shellArgs))
            {
                case "Подтвердить":
                    todo?.Invoke();
                    return true;
                case "Отменить":
                    return false;
                default:
                    throw new NotSupportedException();
            }
        }
        private IClientAPI api;
        public void Error(params object[] args)
        {
            Info(args);
            var msg = string.Join(" ", args ?? Array.Empty<object>());
            RunOnUiThread(_host, () =>
                //System.Windows.Forms.MessageBox.Show(
                //    ViewControlUi.Owner(_host), msg, "Ошибка",
                //    System.Windows.Forms.MessageBoxButtons.OK,
                //    System.Windows.Forms.MessageBoxIcon.Error)
                api.InfoDialog("Ошибка", msg, "OK")
            );
        }

        private void RunOnUiThread(AppViewBase host, Func<bool> value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
        }

        public string GetWrk() => Directory.GetCurrentDirectory();

        public IEnumerable<string> GetFiles()
            => Directory.Exists(GetWrk()) ? Directory.GetFiles(GetWrk()) : Enumerable.Empty<string>();

        public IEnumerable<string> GetDlls()
            => Directory.Exists(GetWrk()) ? Directory.GetFiles(GetWrk(), "*.dll", SearchOption.AllDirectories) : Enumerable.Empty<string>();

        public IEnumerable<string> GetJsonFiles()
            => GetFiles().Where(f => f.ToLowerInvariant().EndsWith(".json"));

        public IEnumerable<string> GetIniFiles()
            => GetFiles().Where(f => f.ToLowerInvariant().EndsWith(".ini"));

        public void Write(object arg) => _host.Info(arg);

        public void Write(object[] args)
        {
            foreach (var arg in args)
                WriteLine(arg);
        }

        public string ReadLine() => ReadLine(_host, "");

        public bool Confirm(string name)
        {
            Clear();
            var shellArgs = Array.Empty<string>();
            switch (SingleSelect("\nРазрешите выполнить операцию: " + name, new[] { "Разрешаю", "Не разрешаю" }, ref shellArgs))
            {
                case "Разрешаю": return true;
                case "Не разрешаю": return false;
                default: throw new Exception("Нет обработчика для выбранного значения");
            }
        }

        public void ConfirmExecute(string name, Action todo)
        {
            if (Confirm(name))
                todo?.Invoke();
        }




        public T Wait<T>(string title, Func<T> todo)
        {
            Info(title);
            return todo();
        }

        public void Interactive() => UserInteractive = true;

        public IEnumerable<string> CheckList(string title, IEnumerable<string> options, ref string[] args)
            => MultiSelect(title, options, ref args);

        public IEnumerable<T> CheckListTitle<T>(string title, IEnumerable<T> applications, Func<T, string> convert)
        {
            var list = applications.ToList();
            var labels = list.Select(convert).ToList();
            var a = Array.Empty<string>();
            var picked = MultiSelect(title, labels, ref a).ToHashSet();
            return list.Where(x => picked.Contains(convert(x)));
        }

        public IEnumerable<T> CheckListTitle<T>(string title, IEnumerable<T> applications, IEnumerable<T> selected, Func<T, string> convert)
        {
            var list = applications.ToList();
            var sel = selected?.ToHashSet() ?? new HashSet<T>();
            var labels = list.Select(convert).ToList();
            var a = Array.Empty<string>();
            var pickedLabels = MultiSelect(title, labels, ref a).ToHashSet();
            return list.Where(x => pickedLabels.Contains(convert(x)) || sel.Contains(x));
        }

        public string GetDirectoryName(string path) => Path.GetDirectoryName(path);

        public string GetProcessName() => Process.GetCurrentProcess().ProcessName;

        public string Combine(params string[] path) => Path.Combine(path);

        public void WriteYellow(params object[] args)
        {
            WriteLine(args);
        }

        public void WriteLine(params object[] args)
        {
            foreach (var arg in args)
            {
                if (arg is IEnumerable enumerable && arg is not string)
                {
                    foreach (var arg2 in enumerable)
                        WriteLine(arg2);
                }
                else
                    _host.Info(arg);
            }
        }

        public string InputFile(string message, Func<object, List<string>> validate, ref string[] args)
        {
            string dir = Directory.GetCurrentDirectory();
            dir = dir.EndsWith(Path.DirectorySeparatorChar) ? dir.Substring(0, dir.Length - 1) : dir;
            while (true)
            {
                var options = new List<string> { ".." };
                var dirs = Directory.GetDirectories(dir);
                var files = Directory.GetFiles(dir);
                options.AddRange(dirs.Select(d => d.Substring(d.LastIndexOf(Path.DirectorySeparatorChar) + 1)));
                options.AddRange(files.Select(d => d.Substring(d.LastIndexOf(Path.DirectorySeparatorChar) + 1)));
                string selected = SingleSelect(message, options, ref args);
                if (selected == "..")
                {
                    var idx = dir.LastIndexOf(Path.DirectorySeparatorChar);
                    if (idx > 0)
                        dir = dir.Substring(0, idx);
                }
                else
                {
                    string path = Path.Combine(dir, selected);
                    if (File.Exists(path))
                        return path;
                    dir = path;
                }
            }
        }

        public bool ConfirmContinue(string message, Func<object, List<string>> validate, ref string[] args)
        {
            if (InputBool(message, null, ref args))
                validate?.Invoke(args);
            return true;
        }

        public bool ConfirmContinue(ref string[] args, string message = "")
        {
            Info(message);
            Info("\nДля продолжения нажмите OK в диалоге....");
            if (UserInteractive)
            {
                bool result = MessageConfirm(_host, string.IsNullOrEmpty(message) ? "Продолжить?" : message + "\n\nПродолжить?");
                Clear();
                return result;
            }

            if (args == null || args.Length == 0)
            {
                if (DebugMode)
                {
                    Clear();
                    return true;
                }
                throw new Exception("Не достаточно аргументов");
            }

            string value = args[0];
            var argsOut = new string[args.Length - 1];
            for (int i = 1; i < args.Length; i++)
                argsOut[i - 1] = args[i];
            args = argsOut;
            return value.Trim().ToLowerInvariant() is "1" or "true";
        }

        public int ConfirmContinue(string title, Func<object, bool> value, ref string[] args)
            => InputBool(title, _ => new List<string>(), ref args) ? 1 : 0;

        public IEnumerable<string> MultiSelect(string title, IEnumerable<string> options, ref string[] args)
        {
            var selected = new List<string>();
            var values = options?.ToList() ?? new List<string>();
            if (values.Count == 0)
                return selected;

            while (true)
            {
                Clear();
                Info(title);
                Info("Выберите пункт меню — пометка [x]/[ ]; завершение — «Сохранить».");

                var stateOptions = values
                    .Select(v => $"{(selected.Contains(v) ? "[x]" : "[ ]")} {v}")
                    .Concat(new[] { "Сохранить" });

                var picked = SingleSelect("", stateOptions, ref args);
                if (picked == "Сохранить")
                    return selected;

                if (picked.Length < 4)
                    continue;
                var item = picked.Substring(4);
                if (selected.Contains(item))
                    selected.Remove(item);
                else
                    selected.Add(item);
            }
        }

        public string InputCreditCard(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, input =>
            {
                var digits = new string((input ?? string.Empty).Where(char.IsDigit).ToArray());
                return digits.Length >= 13 && digits.Length <= 19;
            }, "Неверный номер карты", value, ref args);

        public string InputColor(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, input =>
            {
                if (string.IsNullOrWhiteSpace(input))
                    return false;
                var hex = input.Trim();
                if (hex.StartsWith("#"))
                    hex = hex.Substring(1);
                return (hex.Length == 3 || hex.Length == 6) && hex.All(Uri.IsHexDigit);
            }, "Используйте HEX-цвет, например #1A2B3C", value, ref args);

        public string InputCurrency(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, input => decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out _), "Введите денежное значение", value, ref args);

        public string InputFilePath(string title, Func<object, List<string>> value, ref string[] args)
            => SelectFile(title, text =>
            {
                var errors = new List<string>();
                if (string.IsNullOrWhiteSpace(text) || !File.Exists(text))
                    errors.Add("Файл не найден");
                errors.AddRange(ValidateWith(value, text));
                return errors;
            }, ref args);

        public string InputEngWord(string title, Func<object, List<string>> value, ref string[] args)
            => InputString(title, inval =>
            {
                var res = ValidateWith(value, inval);
                if (inval == null || !inval.ToString().IsEng())
                    res.Add("Исп. латиницу");
                return res;
            }, ref args);

        public string InputRusWord(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, IsRus, "Используйте кириллицу", value, ref args);

        public string InputPassword(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, input => !string.IsNullOrWhiteSpace(input) && input.Length >= 6, "Минимум 6 символов", value, ref args);

        public string InputPhone(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, input =>
            {
                var digits = new string((input ?? string.Empty).Where(char.IsDigit).ToArray());
                return digits.Length >= 10 && digits.Length <= 15;
            }, "Введите корректный номер телефона", value, ref args);

        public string InputXml(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, input =>
            {
                try
                {
                    XDocument.Parse(input ?? string.Empty);
                    return true;
                }
                catch
                {
                    return false;
                }
            }, "Некорректный XML", value, ref args);

        public string InputYear(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, input => int.TryParse(input, out var y) && y >= 1900 && y <= 3000, "Введите год в диапазоне 1900-3000", value, ref args);

        public string InputMonth(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, input => int.TryParse(input, out var m) && m is >= 1 and <= 12, "Введите месяц от 1 до 12", value, ref args);

        public string InputUrl(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, IsUrl, "Введите корректный URL", value, ref args);

        /// <summary>Порядок аргументов как в ServiceBuilderProgram.</summary>
        public string InputUrl(ref string[] args, string title)
            => InputUrl(title, _ => new List<string>(), ref args);

        public string InputEmail(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, IsEmail, "Введите корректный email", value, ref args);

        public string InputName(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, input => !string.IsNullOrWhiteSpace(input), "Значение не может быть пустым", value, ref args);

        public string InputFloat(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, input => float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out _), "Введите число с плавающей точкой", value, ref args);

        public string InputImage(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, input =>
            {
                if (string.IsNullOrWhiteSpace(input))
                    return false;
                var ext = Path.GetExtension(input).ToLowerInvariant();
                return new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp", ".svg" }.Contains(ext);
            }, "Укажите путь к изображению", value, ref args);

        public string InputDecimal(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, input => decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out _), "Введите десятичное число", value, ref args);

        public string InputTime(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, input => TimeSpan.TryParse(input, out _), "Введите время в формате HH:mm[:ss]", value, ref args);

        public string InputText(string title, Func<object, List<string>> validate, ref string[] args)
        {
            if (!UserInteractive)
            {
                var value = Shift(ref args);
                var errors = validate != null ? validate(value) : null;
                if (errors != null && errors.Count != 0)
                    throw new InvalidOperationException(string.Join("; ", errors));
                return value;
            }

            string valueI = null;
            List<string> errorsI;
            do
            {

                valueI = ReadLine(_host, title);
                errorsI = validate != null ? validate(valueI) : null;
                if (errorsI != null)
                {
                    foreach (var error in errorsI)
                        Error(error);
                }
            } while (errorsI != null && errorsI.Count != 0);
            return valueI;
        }

        public string InputWeek(string title, Func<object, List<string>> value, ref string[] args)
            => InputWithPredicate(title, input => int.TryParse(input, out var w) && w is >= 1 and <= 53, "Введите номер недели от 1 до 53", value, ref args);

        public string InputDate(string title, Func<object, List<string>> value, ref string[] args)
            => InputText(title, inputed =>
            {
                var extra = inputed != null && IsDate(inputed.ToString())
                    ? new List<string>()
                    : new List<string> { "Не является датой (исп. формат dd.mm.yyyy)" };
                return ValidateWith(value, inputed).Concat(extra).ToList();
            }, ref args);

        public string InputIcon(string title, Func<object, List<string>> value, ref string[] args)
            => InputText(title, inputed =>
            {
                var result = inputed != null && IsDate(inputed.ToString())
                    ? new List<string>()
                    : new List<string> { "Не является датой (исп. формат dd.mm.yyyy)" };
                return result.Concat(ValidateWith(value, inputed)).ToList();
            }, ref args);

        public string InputDirectory(string title, Func<object, List<string>> value, ref string[] args)
            => SelectDirectory(ref args);

        public object Input(object pObject)
        {
            Clear();
            return pObject;
        }

        public List<Dictionary<string, object>> InputPrimitiveCollection(string title, Func<object, List<string>> value, ref string[] args)
            => throw new NotSupportedException("InputPrimitiveCollection требует CollectionBuilder из исходного приложения.");

        public List<Dictionary<string, object>> InputStructureCollection(string title, Func<object, List<string>> value, ref string[] args)
            => throw new NotSupportedException("InputStructureCollection требует CollectionBuilder из исходного приложения.");

        public bool InputBool(string title, Func<object, List<string>> value, ref string[] args)
            => SingleSelect(title, new[] { "да", "нет" }, ref args) == "да";

        public string InputString(string title, Func<object, List<string>> validate, ref string[] args)
            => InputText(title, validate, ref args);

        public int InputPercent(string title, Func<object, List<string>> validate, ref string[] args)
            => InputNumber(title, val =>
            {
                var errors = ValidateWith(validate, val);
                if (val == null || !int.TryParse(val.ToString(), out var percent) || percent < 0 || percent > 100)
                    errors.Add("Введите процент от 0 до 100");
                return errors;
            }, ref args);

        public int InputInt(string title, Func<object, List<string>> validate, ref string[] args)
            => InputNumber(title, val => ValidateWith(validate, val), ref args);

        public int InputPositiveNumber(string title, Func<object, List<string>> validate, ref string[] args)
            => InputNumber(title, val =>
            {
                if (val == null || !int.TryParse(val.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) || n <= 0)
                    return new List<string> { "Введите положительное целое число" };
                return ValidateWith(validate, val);
            }, ref args);

        public int InputNumber(string title, Func<object, List<string>> validate, ref string[] args)
        {
            validate ??= _ => new List<string>();
            while (true)
            {
                var text = InputText(title, o =>
                {
                    var err = validate(o);
                    if (o == null || !int.TryParse(o.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                        err.Add("Введите целое число");
                    return err;
                }, ref args);
                if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n))
                    return n;
            }
        }

        public string SelectDirectory(ref string[] args)
        {
            var location = Path.GetPathRoot(Directory.GetCurrentDirectory()) ?? "C:\\";
            while (true)
            {
                location = location.Length == 2 ? location + Path.DirectorySeparatorChar : location;
                IEnumerable<string> dirs = Directory.GetDirectories(location);
                string selected = location.Length > 3
                    ? SingleSelect("", new[] { ".." }.Concat(dirs), ref args)
                    : SingleSelect("", dirs, ref args);
                if (selected == "..")
                {
                    var idx = location.TrimEnd(Path.DirectorySeparatorChar).LastIndexOf(Path.DirectorySeparatorChar);
                    if (idx > 0)
                        location = location.Substring(0, idx + 1);
                }
                else
                    location = Path.Combine(location, selected);
                if (Confirm("Выбрать " + location + "?"))
                    break;
            }
            return location;
        }

        public string InputString(string name, Func<string, IEnumerable<string>> validate, ref string[] args)
        {
            validate ??= _ => Array.Empty<string>();
            string value;
            do
            {
                value = ReadLine(_host, name);
                var errors = validate(value)?.ToList() ?? new List<string>();
                foreach (var error in errors)
                    Error(error);
                if (errors.Count != 0)
                    ConfirmContinue("Повторить попытку");
                if (errors.Count == 0)
                    return value;
            } while (true);
        }

        private string ReadLine(AppViewBase host, string name)
        {
            throw new NotImplementedException();
        }

        public object InputChar(string v, object value, ref string[] args)
        {
            var text = InputText(v, value as Func<object, List<string>>, ref args);
            return string.IsNullOrEmpty(text) ? '\0' : text[0];
        }

        public object InputDateTime(string v, object value, ref string[] args)
            => InputWithPredicate(v, input => DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out _), "Введите дату и время", value as Func<object, List<string>>, ref args);

        public object InputSmallMoney(string v, object value, ref string[] args)
            => InputWithPredicate(v, input => decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out _), "Введите денежное значение", value as Func<object, List<string>>, ref args);

        public object InputStructured(string v, object value, ref string[] args)
            => InputText(v, value as Func<object, List<string>>, ref args);

        public object InputUdt(string v, object value, ref string[] args)
            => InputText(v, value as Func<object, List<string>>, ref args);

        public object InputVariant(string v, object value, ref string[] args)
            => InputText(v, value as Func<object, List<string>>, ref args);

        public object InputVarBinary(string v, object value, ref string[] args)
            => Convert.FromBase64String(InputWithPredicate(v, input =>
            {
                try
                {
                    Convert.FromBase64String(input ?? string.Empty);
                    return true;
                }
                catch
                {
                    return false;
                }
            }, "Введите Base64-строку", value as Func<object, List<string>>, ref args));

        public object InputTimestamp(string v, object value, ref string[] args)
            => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public object InputTinyInt(string v, object value, ref string[] args)
            => byte.Parse(InputWithPredicate(v, input => byte.TryParse(input, out _), "Введите число 0..255", value as Func<object, List<string>>, ref args), CultureInfo.InvariantCulture);

        public object InputDateTimeOffset(string v, object value, ref string[] args)
        {
            var text = InputWithPredicate(v, input => DateTimeOffset.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out _), "Введите дату с часовым поясом", value as Func<object, List<string>>, ref args);
            return DateTimeOffset.Parse(text, CultureInfo.InvariantCulture);
        }

        public object InputUniqueIdentifier(string v, object value, ref string[] args)
            => Guid.Parse(InputWithPredicate(v, input => Guid.TryParse(input, out _), "Введите корректный GUID", value as Func<object, List<string>>, ref args));

        public object InputSmallDateTime(string v, object value, ref string[] args)
            => DateTime.Parse(InputWithPredicate(v, input => DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out _), "Введите дату и время", value as Func<object, List<string>>, ref args), CultureInfo.InvariantCulture);

        public object InputSmallInt(string v, object value, ref string[] args)
            => short.Parse(InputWithPredicate(v, input => short.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out _), "Введите целое число (short)", value as Func<object, List<string>>, ref args), CultureInfo.InvariantCulture);

        public object InputDateTime2(string v, object value, ref string[] args)
            => DateTime.Parse(InputWithPredicate(v, input => DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out _), "Введите дату и время", value as Func<object, List<string>>, ref args), CultureInfo.InvariantCulture);

        public object InputReal(string v, object value, ref string[] args)
            => float.Parse(InputWithPredicate(v, input => float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out _), "Введите число с плавающей точкой", value as Func<object, List<string>>, ref args), CultureInfo.InvariantCulture);

        public object InputNchar(string v, object value, ref string[] args)
            => InputText(v, value as Func<object, List<string>>, ref args);

        public object InputMoney(string v, object value, ref string[] args)
            => decimal.Parse(InputWithPredicate(v, input => decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out _), "Введите денежное значение", value as Func<object, List<string>>, ref args), CultureInfo.InvariantCulture);

        public void CreateTreeModel(string title, ref string[] args)
            => throw new NotSupportedException("CreateTreeModel требует TypeNode из исходного приложения.");

        public string SelectFile(string title, ref string[] args)
            => SelectFile(title, _ => new List<string>(), ref args);

        public string SelectFile(string title, Func<string, List<string>> validate, ref string[] args)
        {
            if (!UserInteractive)
                return Shift(ref args);

            return InputFile(title, o =>
            {
                var path = o?.ToString();
                return validate?.Invoke(path) ?? new List<string>();
            }, ref args);
        }

        public void SelectAction(string title, Dictionary<Action, string> options, ref string[] args)
        {
            var selected = SingleSelect(title, options.Values, ref args);
            options.First(o => o.Value == selected).Key();
        }
        public void SelectAction(string title, Dictionary<string, Action> options, ref string[] args)
        {
            options[SingleSelect(title, options.Keys, ref args)]();
        }
        public string SingleSelect(string title, IEnumerable<string> options, ref string[] args)
        {
            var list = options?.ToList() ?? new List<string>();
            if (list.Count == 0)
                throw new InvalidOperationException("Пустой список опций");

            if (!UserInteractive)
            {
                if (args == null || args.Length == 0)
                {
                    if (DebugMode)
                        return list[0];
                    throw new Exception("Не достаточно аргументов");
                }
                var v = Shift(ref args);
                if (int.TryParse(v, out var idx) && idx >= 1 && idx <= list.Count)
                    return list[idx - 1];
                var byValue = list.FirstOrDefault(x => string.Equals(x, v, StringComparison.Ordinal));
                if (byValue != null)
                    return byValue;
                throw new Exception("Неверный выбор: " + v);
            }

            while (true)
            {
                Clear();
                var labels = list.Select((item, idx) => $"{idx + 1}) {item}").ToList();
                var picked = PickFromList(_host, title, labels, ref args);
                if (picked.Length > 0 && char.IsDigit(picked[0]))
                {
                    var sp = picked.IndexOf(')');
                    var numPart = sp > 0 ? picked.Substring(0, sp).TrimEnd(')') : picked;
                    if (int.TryParse(numPart, out var n) && n >= 1 && n <= list.Count)
                        return list[n - 1];
                }
                var byText = list.FirstOrDefault(x => string.Equals(x, picked, StringComparison.Ordinal));
                if (byText != null)
                    return byText;
                Error("Неверный номер, повторите.");
                ConfirmContinue();
            }
        }

        private string PickFromList(AppViewBase host, string title, List<string> labels, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string SingleSelect(string title, IEnumerable<string> options)
        {
            var a = Array.Empty<string>();
            return SingleSelect(title, options, ref a);
        }

        public string SingleSelect(string title, List<string> options, ref string[] args)
            => SingleSelect(title, (IEnumerable<string>)options, ref args);

        public string SingleSelect(string title, List<string> options)
            => SingleSelect(title, (IEnumerable<string>)options);

        /// <summary>Сохранённые точки входа для консольных меню (как в Console_CommonModel.CommonLib.Others).</summary>
        public void SelectList(string title, List<string> options, Action<string> onselect)
        {
            var a = Array.Empty<string>();
            var picked = SingleSelect(title, options, ref a);
            onselect?.Invoke(picked);
        }

        public void SelectAction(string title, Dictionary<string, Action> actions)
        {
            var keys = actions.Keys.ToList();
            var a = Array.Empty<string>();
            var picked = SingleSelect(title, keys, ref a);
            actions[picked]?.Invoke();
        }

        public void SelectAction(string title, Dictionary<Action, string> actions)
        {
            var keys = actions.Values.ToList();
            var a = Array.Empty<string>();
            var picked = SingleSelect(title, keys, ref a);
            actions.First(a => a.Value == picked).Key?.Invoke();
        }

        public TModel Input<TModel>(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotSupportedException("Input<TModel> требует ReflectionService, Typing и атрибуты из исходного приложения.");
        }

        private int SafeToInt(object value)
        {
            if (value == null) return 0;
            return int.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : 0;
        }

        private bool IsDate(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;
            return DateTime.TryParseExact(s.Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                || DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        private bool IsRus(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            return input.All(c => c == ' ' || c >= '\u0400' && c <= '\u04FF');
        }

        private bool IsUrl(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            return Uri.TryCreate(input.Trim(), UriKind.Absolute, out var u) && (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps);
        }

        private bool IsEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            return Regex.IsMatch(input.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }
    }

    public sealed class CompleteException : Exception
    {
    }

    public sealed class CancelException : Exception
    {
        public CancelException(string message) : base(message)
        {
        }
    }
}
