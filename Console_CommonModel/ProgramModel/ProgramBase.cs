using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

public abstract class ProgramBase : IConsoleСontrol
{
    private IConsoleСontrol Control;
    private string[] args = new string[0];

    protected ProgramBase()
    {
        Control = new InputConsoleСontrol(ref args);
    }

    public virtual void Run(ref string[] args)
    {
        //ProgramHistory.TraceHistory();
        var typeName = Assembly.GetCallingAssembly().GetClassTypes().Select(t => t.GetTypeName()).SingleSelect("Выберите тип контроллера", ref args);        
        ConsoleProgram.RunInteractive(typeName.ToType());
    }

    public IEnumerable<string> CheckList(string title, IEnumerable<string> options, ref string[] args)
    {
        return Control.CheckList(title, options, ref args);
    }

    public bool ConfirmContinue(string title)
    {
        return Control.ConfirmContinue(title);
    }

    public TModel Input<TModel>(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.Input<TModel>(title, validate, ref args);
    }

    public bool InputBool(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputBool(title, validate, ref args);
    }

    public string InputColor(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputColor(title, validate, ref args);
    }

    public string InputCreditCard(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputCreditCard(title, validate, ref args);
    }

    public string InputCurrency(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputCurrency(title, validate, ref args);
    }

    public string InputDate(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputDate(title, validate, ref args);
    }

    public string InputDecimal(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputDecimal(title, validate, ref args);
    }

    public string InputDirectory(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputDirectory(title, validate, ref args);
    }

    public string InputEmail(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputEmail(title, validate, ref args);
    }

    public string InputFile(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputFile(title, validate, ref args);
    }

    public string InputFilePath(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputFilePath(title, validate, ref args);
    }

    public string InputIcon(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputIcon(title, validate, ref args);
    }

    public string InputImage(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputImage(title, validate, ref args);
    }

    public int InputInt(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputInt(title, validate, ref args);
    }

    public string InputMonth(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputMonth(title, validate, ref args);
    }

    public string InputName(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputName(title, validate, ref args);
    }

    public int InputNumber(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputNumber(title, validate, ref args);
    }

    public string InputPassword(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputPassword(title, validate, ref args);
    }

    public int InputPercent(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputPercent(title, validate, ref args);
    }

    public string InputPhone(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputPhone(title, validate, ref args);
    }

    public int InputPositiveNumber(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputPositiveNumber(title, validate, ref args);
    }

    public List<Dictionary<string, object>> InputPrimitiveCollection(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputPrimitiveCollection(title, validate, ref args);
    }

    public string InputRusWord(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputRusWord(title, validate, ref args);
    }

    public string InputString(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputString(title, validate, ref args);
    }

    public List<Dictionary<string, object>> InputStructureCollection(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputStructureCollection(title, validate, ref args);
    }

    public string InputText(string title, Func<object, List<string>> validate, ref string[] args)
    {
        Control.UserInteractive = this.UserInteractive;
        return Control.InputText(title, validate, ref args);
    }

    public string InputTime(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputTime(title, validate, ref args);
    }

    public string InputUrl(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputUrl(title, validate, ref args);
    }

    public string InputWeek(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputWeek(title, validate, ref args);
    }

    public string InputXml(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputXml(title, validate, ref args);
    }

    public string InputYear(string title, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.InputYear(title, validate, ref args);
    }

    public IEnumerable<string> MultiSelect(string title, IEnumerable<string> options, ref string[] args)
    {
        return Control.MultiSelect(title, options, ref args);
    }

    public string SingleSelect(string title, IEnumerable<string> options, ref string[] args)
    {
        return Control.SingleSelect(title, options, ref args);
    }

    public bool DebugMode { get => Control.DebugMode; set => Control.DebugMode = value; }
    public bool UserInteractive { get => Control.UserInteractive; set => Control.UserInteractive = value; }

    public IEnumerable<T> CheckListTitle<T>(string title, IEnumerable<T> applications, Func<T, string> convert)
    {
        return Control.CheckListTitle(title, applications, convert);
    }

    public IEnumerable<T> CheckListTitle<T>(string title, IEnumerable<T> applications, IEnumerable<T> selected, Func<T, string> convert)
    {
        return Control.CheckListTitle(title, applications, selected, convert);
    }

    public void Clear()
    {
        Control.Clear();
    }

    public string Combine(params string[] path)
    {
        return Control.Combine(path);
    }

    public bool Confirm(string name)
    {
        return Control.Confirm(name);
    }

    public bool ConfirmActionDialog(string title, Action todo)
    {
        return Control.ConfirmActionDialog(title, todo);
    }

    public bool ConfirmContinue(ref string[] args, string message = "")
    {
        return Control.ConfirmContinue(ref args, message);
    }

    public int ConfirmContinue(string title, Func<object, bool> value, ref string[] args)
    {
        return Control.ConfirmContinue(title, value, ref args);
    }

    public bool ConfirmContinue(string message, Func<object, List<string>> validate, ref string[] args)
    {
        return Control.ConfirmContinue(message, validate, ref args);
    }

    public void ConfirmExecute(string name, Action todo)
    {
        Control.ConfirmExecute(name, todo);
    }

    public void CreateTreeModel(string title, ref string[] args)
    {
        Control.CreateTreeModel(title, ref args);
    }

    public void Error(params object[] args)
    {
        Control.Error(args);
    }

    public void Exit()
    {
        Control.Exit();
    }

    public string GetDirectoryName(string path)
    {
        return Control.GetDirectoryName(path);
    }

    public IEnumerable<string> GetDlls()
    {
        return Control.GetDlls();
    }

    public IEnumerable<string> GetFiles()
    {
        return Control.GetFiles();
    }

    public IEnumerable<string> GetIniFiles()
    {
        return Control.GetIniFiles();
    }

    public IEnumerable<string> GetJsonFiles()
    {
        return Control.GetJsonFiles();
    }

    public List<PortInfo> GetOpenPort()
    {
        return Control.GetOpenPort();
    }

    public string GetProcessName()
    {
        return Control.GetProcessName();
    }

    public string GetWrk()
    {
        return Control.GetWrk();
    }

    public void Info(params object[] args)
    {
        Control.Info(args);
    }

    public string Input(string name, ref string[] args)
    {
        return Control.Input(name, ref args);
    }

    public object InputChar(string v, object value, ref string[] args)
    {
        return Control.InputChar(v, value, ref args);
    }

    public object InputDateTime(string v, object value, ref string[] args)
    {
        return Control.InputDateTime(v, value, ref args);
    }

    public object InputDateTime2(string v, object value, ref string[] args)
    {
        return Control.InputDateTime2(v, value, ref args);
    }

    public object InputDateTimeOffset(string v, object value, ref string[] args)
    {
        return Control.InputDateTimeOffset(v, value, ref args);
    }

    public string InputEngWord(string title, Func<object, List<string>> value, ref string[] args)
    {
        return Control.InputEngWord(title, value, ref args);
    }

    public string InputFloat(string title, Func<object, List<string>> value, ref string[] args)
    {
        return Control.InputFloat(title, value, ref args);
    }

    public object InputMoney(string v, object value, ref string[] args)
    {
        return Control.InputMoney(v, value, ref args);
    }

    public object InputNchar(string v, object value, ref string[] args)
    {
        return Control.InputNchar(v, value, ref args);
    }

 
    public object InputReal(string v, object value, ref string[] args)
    {
        return Control.InputReal(v, value, ref args);
    }

    public object InputSmallDateTime(string v, object value, ref string[] args)
    {
        return Control.InputSmallDateTime(v, value, ref args);
    }

    public object InputSmallInt(string v, object value, ref string[] args)
    {
        return Control.InputSmallInt(v, value, ref args);
    }

    public object InputSmallMoney(string v, object value, ref string[] args)
    {
        return Control.InputSmallMoney(v, value, ref args);
    }

    public string InputString(string name, Func<string, IEnumerable<string>> validate, ref string[] args)
    {
        return Control.InputString(name, validate, ref args);
    }

    public string InputString(string name, Func<string, List<string>> validate, ref string[] args)
    {
        return Control.InputString(name, validate, ref args);
    }

    public object InputStructured(string v, object value, ref string[] args)
    {
        return Control.InputStructured(v, value, ref args);
    }

    public int InputTcpPort(string title, ref string[] args)
    {
        return Control.InputTcpPort(title, ref args);
    }

    public object InputTimestamp(string v, object value, ref string[] args)
    {
        return Control.InputTimestamp(v, value, ref args);
    }

    public object InputTinyInt(string v, object value, ref string[] args)
    {
        return Control.InputTinyInt(v, value, ref args);
    }

    public object InputUdt(string v, object value, ref string[] args)
    {
        return Control.InputUdt(v, value, ref args);
    }

    public object InputUniqueIdentifier(string v, object value, ref string[] args)
    {
        return Control.InputUniqueIdentifier(v, value, ref args);
    }

    public string InputUrl(ref string[] args, string title)
    {
        return Control.InputUrl(ref args, title);
    }

    public object InputVarBinary(string v, object value, ref string[] args)
    {
        return Control.InputVarBinary(v, value, ref args);
    }

    public object InputVariant(string v, object value, ref string[] args)
    {
        return Control.InputVariant(v, value, ref args);
    }

    public void Interactive()
    {
        Control.Interactive();
    }

    public ConsoleKeyInfo ReadKey()
    {
        return Control.ReadKey();
    }

    public T ReadKeyRepeat<T>(Action beforeRead, Action<object> onKeyPressed, Func<T> onCompleted, Action onCanceled)
    {
        return Control.ReadKeyRepeat(beforeRead, onKeyPressed, onCompleted, onCanceled);
    }

    public string ReadLine()
    {
        return Control.ReadLine();
    }

    public string ReadText(string name, Func<string, List<string>> validate, ref string[] args)
    {
        return Control.ReadText(name, validate, ref args);
    }

    public void SelectAction(string title, object target, ref string[] args)
    {
        Control.SelectAction(title, target, ref args);
    }

    public void SelectAction(string title, Dictionary<Action, string> options, ref string[] args)
    {
        Control.SelectAction(title, options, ref args);
    }

    public void SelectAction(string title, Dictionary<string, Action> options, ref string[] args)
    {
        Control.SelectAction(title, options, ref args);
    }

    public string SelectDirectory(ref string[] args)
    {
        return Control.SelectDirectory(ref args);
    }

    public string SelectFile(string title, Func<string, List<string>> validate, ref string[] args)
    {
        return Control.SelectFile(title, validate, ref args);
    }

    public string SelectFile(string title, ref string[] args)
    {
        return Control.SelectFile(title, ref args);
    }

    public void SelectList(string title, List<string> options, Action<string> onselect, ref string[] args)
    {
        Control.SelectList(title, options, onselect, ref args);
    }

    public string Shift(ref string[] args)
    {
        return Control.Shift(ref args);
    }

    public string SingleSelect(string title, List<string> options, ref string[] args)
    {
        return Control.SingleSelect(title, options, ref args);
    }

    public T Wait<T>(string title, Func<T> todo)
    {
        return Control.Wait(title, todo);
    }

    public void Warn(params object[] args)
    {
        Control.Warn(args);
    }

    public void Write(object arg)
    {
        Control.Write(arg);
    }

    public void Write(object[] args)
    {
        Control.Write(args);
    }

    public void WriteLine(params object[] args)
    {
        Control.WriteLine(args);
    }

    public void WriteYellow(params object[] args)
    {
        Control.WriteYellow(args);
    }

    public void ConfirmExecute(string name, Action todo, ref string[] args)
    {
        Control.ConfirmExecute(name, todo, ref args);
    }
}