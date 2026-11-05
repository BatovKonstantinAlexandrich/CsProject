using System;
using System.Collections.Generic;
using Console_CommonModel.CommonLib.Others;


using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.ViewControl
{
    /// <summary>
    /// Реализация <see cref="IViewControl"/> для WinForms: делегирует в <see cref="ViewControlConsoleAdapter"/>,
    /// используя фабрику и UI-поток через <see cref="AppViewBase"/>.
    /// </summary>
    [Label("Консольное представление (WinForms)")]
    public class InputConsoleView : AppViewBase, IViewControl
    {
        private readonly ViewControlConsoleAdapter _impl;

        public InputConsoleView()
        {
            _impl = new ViewControlConsoleAdapter(this);
            UserInteractive = true;
        }

        [InputBool]
        public bool DebugMode
        {
            get => _impl.DebugMode;
            set => _impl.DebugMode = value;
        }

        [InputBool]
        public bool UserInteractive
        {
            get => _impl.UserInteractive;
            set => _impl.UserInteractive = value;
        }

        public IEnumerable<string> CheckList(string title, IEnumerable<string> options, ref string[] args) => _impl.CheckList(title, options, ref args);
        public IEnumerable<T> CheckListTitle<T>(string title, IEnumerable<T> applications, Func<T, string> convert) => _impl.CheckListTitle(title, applications, convert);
        public IEnumerable<T> CheckListTitle<T>(string title, IEnumerable<T> applications, IEnumerable<T> selected, Func<T, string> convert) => _impl.CheckListTitle(title, applications, selected, convert);
        public void Clear() => _impl.Clear();
        public string Combine(params string[] path) => _impl.Combine(path);
        public bool Confirm(string name) => _impl.Confirm(name);
        public bool ConfirmActionDialog(string title, Action todo) => _impl.ConfirmActionDialog(title, todo);
        public bool ConfirmContinue(string title = "") => _impl.ConfirmContinue(title);
        public bool ConfirmContinue(ref string[] args, string message = "") => _impl.ConfirmContinue(ref args, message);
        public int ConfirmContinue(string title, Func<object, bool> value, ref string[] args) => _impl.ConfirmContinue(title, value, ref args);
        public bool ConfirmContinue(string message, Func<object, List<string>> validate, ref string[] args) => _impl.ConfirmContinue(message, validate, ref args);
        public void ConfirmExecute(string name, Action todo) => _impl.ConfirmExecute(name, todo);
        public void CreateTreeModel(string title, ref string[] args) => _impl.CreateTreeModel(title, ref args);
        public void Error(params object[] args) => _impl.Error(args);
        public void Exit() => _impl.Exit();
        public string GetDirectoryName(string path) => _impl.GetDirectoryName(path);
        public IEnumerable<string> GetDlls() => _impl.GetDlls();
        public IEnumerable<string> GetFiles() => _impl.GetFiles();
        public IEnumerable<string> GetIniFiles() => _impl.GetIniFiles();
        public IEnumerable<string> GetJsonFiles() => _impl.GetJsonFiles();
        public List<PortInfo> GetOpenPort() => _impl.GetOpenPort();
        public string GetProcessName() => _impl.GetProcessName();
        public string GetWrk() => _impl.GetWrk();
        public void Info(params object[] args) => _impl.Info(args);
        public object Input(object pObject) => _impl.Input(pObject);
        public string Input(string name, ref string[] args) => _impl.Input(name, ref args);
        public TModel Input<TModel>(string title, Func<object, List<string>> value, ref string[] args) => _impl.Input<TModel>(title, value, ref args);
        public bool InputBool(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputBool(title, value, ref args);
        public object InputChar(string v, object value, ref string[] args) => _impl.InputChar(v, value, ref args);
        public string InputColor(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputColor(title, value, ref args);
        public string InputCreditCard(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputCreditCard(title, value, ref args);
        public string InputCurrency(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputCurrency(title, value, ref args);
        public string InputDate(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputDate(title, value, ref args);
        public object InputDateTime(string v, object value, ref string[] args) => _impl.InputDateTime(v, value, ref args);
        public object InputDateTime2(string v, object value, ref string[] args) => _impl.InputDateTime2(v, value, ref args);
        public object InputDateTimeOffset(string v, object value, ref string[] args) => _impl.InputDateTimeOffset(v, value, ref args);
        public string InputDecimal(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputDecimal(title, value, ref args);
        public string InputDirectory(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputDirectory(title, value, ref args);
        public string InputEmail(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputEmail(title, value, ref args);
        public string InputEngWord(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputEngWord(title, value, ref args);
        public string InputFile(string message, Func<object, List<string>> validate, ref string[] args) => _impl.InputFile(message, validate, ref args);
        public string InputFilePath(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputFilePath(title, value, ref args);
        public string InputFloat(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputFloat(title, value, ref args);
        public string InputIcon(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputIcon(title, value, ref args);
        public string InputImage(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputImage(title, value, ref args);
        public int InputInt(string title, Func<object, List<string>> validate, ref string[] args) => _impl.InputInt(title, validate, ref args);
        public object InputMoney(string v, object value, ref string[] args) => _impl.InputMoney(v, value, ref args);
        public string InputMonth(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputMonth(title, value, ref args);
        public string InputName(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputName(title, value, ref args);
        public object InputNchar(string v, object value, ref string[] args) => _impl.InputNchar(v, value, ref args);
        public int InputNumber(string title, Func<object, List<string>> validate, ref string[] args) => _impl.InputNumber(title, validate, ref args);
        public string InputPassword(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputPassword(title, value, ref args);
        public int InputPercent(string title, Func<object, List<string>> validate, ref string[] args) => _impl.InputPercent(title, validate, ref args);
        public string InputPhone(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputPhone(title, value, ref args);
        public int InputPositiveNumber(string title, Func<object, List<string>> validate, ref string[] args) => _impl.InputPositiveNumber(title, validate, ref args);
        public List<Dictionary<string, object>> InputPrimitiveCollection(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputPrimitiveCollection(title, value, ref args);
        public object InputReal(string v, object value, ref string[] args) => _impl.InputReal(v, value, ref args);
        public string InputRusWord(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputRusWord(title, value, ref args);
        public object InputSmallDateTime(string v, object value, ref string[] args) => _impl.InputSmallDateTime(v, value, ref args);
        public object InputSmallInt(string v, object value, ref string[] args) => _impl.InputSmallInt(v, value, ref args);
        public object InputSmallMoney(string v, object value, ref string[] args) => _impl.InputSmallMoney(v, value, ref args);
        public string InputString(string title, Func<object, List<string>> validate, ref string[] args) => _impl.InputString(title, validate, ref args);
        public string InputString(string name, Func<string, IEnumerable<string>> validate, ref string[] args) => _impl.InputString(name, validate, ref args);
        public string InputString(string name, Func<string, List<string>> validate, ref string[] args) => _impl.InputString(name, validate, ref args);
        public List<Dictionary<string, object>> InputStructureCollection(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputStructureCollection(title, value, ref args);
        public object InputStructured(string v, object value, ref string[] args) => _impl.InputStructured(v, value, ref args);
        public int InputTcpPort(string title, ref string[] args) => _impl.InputTcpPort(title, ref args);
        public string InputText(string title, Func<object, List<string>> validate, ref string[] args) => _impl.InputText(title, validate, ref args);
        public string InputTime(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputTime(title, value, ref args);
        public object InputTimestamp(string v, object value, ref string[] args) => _impl.InputTimestamp(v, value, ref args);
        public object InputTinyInt(string v, object value, ref string[] args) => _impl.InputTinyInt(v, value, ref args);
        public object InputUdt(string v, object value, ref string[] args) => _impl.InputUdt(v, value, ref args);
        public object InputUniqueIdentifier(string v, object value, ref string[] args) => _impl.InputUniqueIdentifier(v, value, ref args);
        public string InputUrl(ref string[] args, string title) => _impl.InputUrl(ref args, title);
        public string InputUrl(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputUrl(title, value, ref args);
        public object InputVarBinary(string v, object value, ref string[] args) => _impl.InputVarBinary(v, value, ref args);
        public object InputVariant(string v, object value, ref string[] args) => _impl.InputVariant(v, value, ref args);
        public string InputWeek(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputWeek(title, value, ref args);
        public string InputXml(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputXml(title, value, ref args);
        public string InputYear(string title, Func<object, List<string>> value, ref string[] args) => _impl.InputYear(title, value, ref args);
        public void Interactive() => _impl.Interactive();
        public IEnumerable<string> MultiSelect(string title, IEnumerable<string> options, ref string[] args) => _impl.MultiSelect(title, options, ref args);
        public T ReadKeyRepeat<T>(Action beforeRead, Action<object> onKeyPressed, Func<T> onCompleted, Action onCanceled) => _impl.ReadKeyRepeat(beforeRead, onKeyPressed, onCompleted, onCanceled);
        public string ReadLine() => _impl.ReadLine();
        public string ReadText(string name, Func<string, List<string>> validate, ref string[] args) => _impl.ReadText(name, validate, ref args);
        public void SelectAction(string title, Dictionary<Action, string> actions) => _impl.SelectAction(title, actions);
        public void SelectAction(string title, Dictionary<Action, string> options, ref string[] args) => _impl.SelectAction(title, options, ref args);
        public void SelectAction(string title, Dictionary<string, Action> actions) => _impl.SelectAction(title, actions);
        public void SelectAction(string title, Dictionary<string, Action> options, ref string[] args) => _impl.SelectAction(title, options, ref args);
        public string SelectDirectory(ref string[] args) => _impl.SelectDirectory(ref args);
        public string SelectFile(string title, Func<string, List<string>> validate, ref string[] args) => _impl.SelectFile(title, validate, ref args);
        public string SelectFile(string title, ref string[] args) => _impl.SelectFile(title, ref args);
        public void SelectList(string title, List<string> options, Action<string> onselect) => _impl.SelectList(title, options, onselect);
        public string Shift(ref string[] args) => _impl.Shift(ref args);
        public string SingleSelect(string title, IEnumerable<string> options) => _impl.SingleSelect(title, options);
        public string SingleSelect(string title, IEnumerable<string> options, ref string[] args) => _impl.SingleSelect(title, options, ref args);
        public string SingleSelect(string title, List<string> options) => _impl.SingleSelect(title, options);
        public string SingleSelect(string title, List<string> options, ref string[] args) => _impl.SingleSelect(title, options, ref args);
        public T Wait<T>(string title, Func<T> todo) => _impl.Wait(title, todo);
        public void Warn(params object[] args) => _impl.Warn(args);
        public void Write(object arg) => _impl.Write(arg);
        public void Write(object[] args) => _impl.Write(args);
        public void WriteLine(params object[] args) => _impl.WriteLine(args);
        public void WriteYellow(params object[] args) => _impl.WriteYellow(args);

        public ConsoleKeyInfo ReadKey()
        {
            throw new NotImplementedException();
        }

        public void SelectAction(string title, object target, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public void SelectList(string title, List<string> options, Action<string> onselect, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public void ConfirmExecute(string name, Action todo, ref string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
