using Console_CommonModel.CommonLib.Others;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static IConsoleСontrol;


//public class In : IIn
//{
//    public IConsoleСontrol control = new ConsoleСontrol();
//    public IIn inputConsole { get; set; }
//    public bool DebugMode { get => inputConsole.DebugMode; set => inputConsole.DebugMode = value; }
//    public bool UserInteractive { get => inputConsole.UserInteractive; set => inputConsole.UserInteractive = value; }

//    public IEnumerable<string> CheckList(string title, IEnumerable<string> options, ref string[] args)
//    {
//        return inputConsole.CheckList(title, options, ref args);
//    }

//    public IEnumerable<T> CheckListTitle<T>(string title, IEnumerable<T> applications, Func<T, string> convert)
//    {
//        return inputConsole.CheckListTitle(title, applications, convert);
//    }

//    public IEnumerable<T> CheckListTitle<T>(string title, IEnumerable<T> applications, IEnumerable<T> selected, Func<T, string> convert)
//    {
//        return inputConsole.CheckListTitle(title, applications, selected, convert);
//    }

//    public void Clear()
//    {
//        inputConsole.Clear();
//    }

//    public string Combine(params string[] path)
//    {
//        return inputConsole.Combine(path);
//    }

//    public bool Confirm(string name)
//    {
//        return inputConsole.Confirm(name);
//    }

//    public bool ConfirmActionDialog(string title, Action todo)
//    {
//        return inputConsole.ConfirmActionDialog(title, todo);
//    }

//    public bool ConfirmContinue(string title = "")
//    {
//        return inputConsole.ConfirmContinue(title);
//    }

//    public bool ConfirmContinue(ref string[] args, string message = "")
//    {
//        return inputConsole.ConfirmContinue(ref args, message);
//    }

//    public int ConfirmContinue(string title, Func<object, bool> value, ref string[] args)
//    {
//        return inputConsole.ConfirmContinue(title, value, ref args);
//    }

//    public bool ConfirmContinue(string message, Func<object, List<string>> validate, ref string[] args)
//    {
//        return inputConsole.ConfirmContinue(message, validate, ref args);
//    }

//    public void ConfirmExecute(string name, Action todo)
//    {
//        inputConsole.ConfirmExecute(name, todo);
//    }

//    public void CreateTreeModel(string title, ref string[] args)
//    {
//        inputConsole.CreateTreeModel(title, ref args);
//    }

//    public void Error(params object[] args)
//    {
//        inputConsole.Error(args);
//    }

//    public void Exit()
//    {
//        inputConsole.Exit();
//    }

//    public string GetDirectoryName(string path)
//    {
//        return inputConsole.GetDirectoryName(path);
//    }

//    public IEnumerable<string> GetDlls()
//    {
//        return inputConsole.GetDlls();
//    }

//    public IEnumerable<string> GetFiles()
//    {
//        return inputConsole.GetFiles();
//    }

//    public IEnumerable<string> GetIniFiles()
//    {
//        return inputConsole.GetIniFiles();
//    }

//    public IEnumerable<string> GetJsonFiles()
//    {
//        return inputConsole.GetJsonFiles();
//    }

//    public List<PortInfo> GetOpenPort()
//    {
//        return inputConsole.GetOpenPort();
//    }

//    public string GetProcessName()
//    {
//        return inputConsole.GetProcessName();
//    }

//    public string GetWrk()
//    {
//        return inputConsole.GetWrk();
//    }

//    public void Info(params object[] args)
//    {
//        inputConsole.Info(args);
//    }

//    public object Input(object pObject)
//    {
//        return inputConsole.Input(pObject);
//    }

//    public string Input(string name, ref string[] args)
//    {
//        return inputConsole.Input(name, ref args);
//    }

//    public TModel Input<TModel>(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.Input<TModel>(title, value, ref args);
//    }

//    public bool InputBool(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputBool(title, value, ref args);
//    }

//    public object InputChar(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputChar(v, value, ref args);
//    }

//    public string InputColor(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputColor(title, value, ref args);
//    }

//    public string InputCreditCard(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputCreditCard(title, value, ref args);
//    }

//    public string InputCurrency(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputCurrency(title, value, ref args);
//    }

//    public string InputDate(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputDate(title, value, ref args);
//    }

//    public object InputDateTime(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputDateTime(v, value, ref args);
//    }

//    public object InputDateTime2(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputDateTime2(v, value, ref args);
//    }

//    public object InputDateTimeOffset(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputDateTimeOffset(v, value, ref args);
//    }

//    public string InputDecimal(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputDecimal(title, value, ref args);
//    }

//    public string InputDirectory(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputDirectory(title, value, ref args);
//    }

//    public string InputEmail(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputEmail(title, value, ref args);
//    }

//    public string InputEngWord(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputEngWord(title, value, ref args);
//    }

//    public string InputFile(string message, Func<object, List<string>> validate, ref string[] args)
//    {
//        return inputConsole.InputFile(message, validate, ref args);
//    }

//    public string InputFilePath(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputFilePath(title, value, ref args);
//    }

//    public string InputFloat(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputFloat(title, value, ref args);
//    }

//    public string InputIcon(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputIcon(title, value, ref args);
//    }

//    public string InputImage(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputImage(title, value, ref args);
//    }

//    public int InputInt(string title, Func<object, List<string>> validate, ref string[] args)
//    {
//        return inputConsole.InputInt(title, validate, ref args);
//    }

//    public object InputMoney(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputMoney(v, value, ref args);
//    }

//    public string InputMonth(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputMonth(title, value, ref args);
//    }

//    public string InputName(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputName(title, value, ref args);
//    }

//    public object InputNchar(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputNchar(v, value, ref args);
//    }

//    public int InputNumber(string title, Func<object, List<string>> validate, ref string[] args)
//    {
//        return inputConsole.InputNumber(title, validate, ref args);
//    }

//    public string InputPassword(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputPassword(title, value, ref args);
//    }

//    public int InputPercent(string title, Func<object, List<string>> validate, ref string[] args)
//    {
//        return inputConsole.InputPercent(title, validate, ref args);
//    }

//    public string InputPhone(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputPhone(title, value, ref args);
//    }

//    public int InputPositiveNumber(string title, Func<object, List<string>> validate, ref string[] args)
//    {
//        return inputConsole.InputPositiveNumber(title, validate, ref args);
//    }

//    public List<Dictionary<string, object>> InputPrimitiveCollection(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputPrimitiveCollection(title, value, ref args);
//    }

//    public object InputReal(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputReal(v, value, ref args);
//    }

//    public string InputRusWord(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputRusWord(title, value, ref args);
//    }

//    public object InputSmallDateTime(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputSmallDateTime(v, value, ref args);
//    }

//    public object InputSmallInt(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputSmallInt(v, value, ref args);
//    }

//    public object InputSmallMoney(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputSmallMoney(v, value, ref args);
//    }

//    public string InputString(string title, Func<object, List<string>> validate, ref string[] args)
//    {
//        return inputConsole.InputString(title, validate, ref args);
//    }

//    public string InputString(string name, Func<string, IEnumerable<string>> validate, ref string[] args)
//    {
//        return inputConsole.InputString(name, validate, ref args);
//    }

//    public string InputString(string name, Func<string, List<string>> validate, ref string[] args)
//    {
//        return inputConsole.InputString(name, validate, ref args);
//    }

//    public List<Dictionary<string, object>> InputStructureCollection(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputStructureCollection(title, value, ref args);
//    }

//    public object InputStructured(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputStructured(v, value, ref args);
//    }

//    public int InputTcpPort(string title, ref string[] args)
//    {
//        return inputConsole.InputTcpPort(title, ref args);
//    }

//    public string InputText(string title, Func<object, List<string>> validate, ref string[] args)
//    {
//        return inputConsole.InputText(title, validate, ref args);
//    }

//    public string InputTime(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputTime(title, value, ref args);
//    }

//    public object InputTimestamp(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputTimestamp(v, value, ref args);
//    }

//    public object InputTinyInt(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputTinyInt(v, value, ref args);
//    }

//    public object InputUdt(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputUdt(v, value, ref args);
//    }

//    public object InputUniqueIdentifier(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputUniqueIdentifier(v, value, ref args);
//    }

//    public string InputUrl(ref string[] args, string title)
//    {
//        return inputConsole.InputUrl(ref args, title);
//    }

//    public string InputUrl(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputUrl(title, value, ref args);
//    }

//    public object InputVarBinary(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputVarBinary(v, value, ref args);
//    }

//    public object InputVariant(string v, object value, ref string[] args)
//    {
//        return inputConsole.InputVariant(v, value, ref args);
//    }

//    public string InputWeek(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputWeek(title, value, ref args);
//    }

//    public string InputXml(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputXml(title, value, ref args);
//    }

//    public string InputYear(string title, Func<object, List<string>> value, ref string[] args)
//    {
//        return inputConsole.InputYear(title, value, ref args);
//    }

//    public void Interactive()
//    {
//        inputConsole.Interactive();
//    }

//    public IEnumerable<string> MultiSelect(string title, IEnumerable<string> options, ref string[] args)
//    {
//        return inputConsole.MultiSelect(title, options, ref args);
//    }

//    public ConsoleKeyInfo ReadKey()
//    {
//        return inputConsole.ReadKey();
//    }

//    public T ReadKeyRepeat<T>(Action beforeRead, Action<object> onKeyPressed, Func<T> onCompleted, Action onCanceled)
//    {
//        return inputConsole.ReadKeyRepeat(beforeRead, onKeyPressed, onCompleted, onCanceled);
//    }

//    public string ReadLine()
//    {
//        return inputConsole.ReadLine();
//    }

//    public string ReadText(string name, Func<string, List<string>> validate, ref string[] args)
//    {
//        return inputConsole.ReadText(name, validate, ref args);
//    }

//    public void SelectAction(string title, Dictionary<string, Action> actions)
//    {
//        inputConsole.SelectAction(title, actions);
//    }

//    public void SelectAction(string title, Dictionary<string, Action> options, ref string[] args)
//    {
//        inputConsole.SelectAction(title, options, ref args);
//    }

//    public void SelectAction1(string title, Dictionary<Action, string> actions)
//    {
//        inputConsole.SelectAction1(title, actions);
//    }

//    public void SelectActions2(string title, Dictionary<Action, string> options, ref string[] args)
//    {
//        inputConsole.SelectActions2(title, options, ref args);
//    }

//    public string SelectDirectory(ref string[] args)
//    {
//        return inputConsole.SelectDirectory(ref args);
//    }

//    public string SelectFile(string title, Func<string, List<string>> validate, ref string[] args)
//    {
//        return inputConsole.SelectFile(title, validate, ref args);
//    }

//    public string SelectFile(string title, ref string[] args)
//    {
//        return inputConsole.SelectFile(title, ref args);
//    }

//    public void SelectList(string title, List<string> options, Action<string> onselect)
//    {
//        inputConsole.SelectList(title, options, onselect);
//    }

//    public string Shift(ref string[] args)
//    {
//        return inputConsole.Shift(ref args);
//    }

//    public string SingleSelect(string title, IEnumerable<string> options)
//    {
//        return inputConsole.SingleSelect(title, options);
//    }

//    public string SingleSelect(string title, IEnumerable<string> options, ref string[] args)
//    {
//        return inputConsole.SingleSelect(title, options, ref args);
//    }

//    public string SingleSelect(string title, List<string> options)
//    {
//        return inputConsole.SingleSelect(title, options);
//    }

//    public string SingleSelect(string title, List<string> options, ref string[] args)
//    {
//        return inputConsole.SingleSelect(title, options, ref args);
//    }

//    public T Wait<T>(string title, Func<T> todo)
//    {
//        return inputConsole.Wait(title, todo);
//    }

//    public void Warn(params object[] args)
//    {
//        inputConsole.Warn(args);
//    }

//    public void Write(object arg)
//    {
//        inputConsole.Write(arg);
//    }

//    public void Write(object[] args)
//    {
//        inputConsole.Write(args);
//    }

//    public void WriteLine(params object[] args)
//    {
//        inputConsole.WriteLine(args);
//    }

//    public void WriteYellow(params object[] args)
//    {
//        inputConsole.WriteYellow(args);
//    }

     
//}
