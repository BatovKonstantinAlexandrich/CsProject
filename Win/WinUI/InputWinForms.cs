using Console_CommonModel.CommonLib.Others;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win_BusinessApp.WinUI
{


    
    internal class InputWinForms: IIn
    {
        public bool DebugMode { get; set; }
        public bool UserInteractive { get; set; }

        public IEnumerable<string> CheckList(string title, IEnumerable<string> options, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> CheckListTitle<T>(string title, IEnumerable<T> applications, Func<T, string> convert)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> CheckListTitle<T>(string title, IEnumerable<T> applications, IEnumerable<T> selected, Func<T, string> convert)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public string Combine(params string[] path)
        {
            throw new NotImplementedException();
        }

        public bool Confirm(string name)
        {
            throw new NotImplementedException();
        }

        public bool ConfirmActionDialog(string title, Action todo)
        {
            throw new NotImplementedException();
        }

        public bool ConfirmContinue(string title = "")
        {
            throw new NotImplementedException();
        }

        public bool ConfirmContinue(ref string[] args, string message = "")
        {
            throw new NotImplementedException();
        }

        public int ConfirmContinue(string title, Func<object, bool> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public bool ConfirmContinue(string message, Func<object, List<string>> validate, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public void ConfirmExecute(string name, Action todo)
        {
            throw new NotImplementedException();
        }

        public void CreateTreeModel(string title, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public void Error(params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }

        public string GetDirectoryName(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetDlls()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetFiles()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetIniFiles()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetJsonFiles()
        {
            throw new NotImplementedException();
        }

        public List<PortInfo> GetOpenPort()
        {
            throw new NotImplementedException();
        }

        public string GetProcessName()
        {
            throw new NotImplementedException();
        }

        public string GetWrk()
        {
            throw new NotImplementedException();
        }

        public void Info(params object[] args)
        {
            throw new NotImplementedException();
        }

        public object Input(object pObject)
        {
            throw new NotImplementedException();
        }

        public string Input(string name, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public TModel Input<TModel>(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public bool InputBool(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputChar(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputColor(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputCreditCard(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputCurrency(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputDate(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputDateTime(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputDateTime2(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputDateTimeOffset(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputDecimal(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputDirectory(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputEmail(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputEngWord(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputFile(string message, Func<object, List<string>> validate, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputFilePath(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputFloat(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputIcon(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputImage(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public int InputInt(string title, Func<object, List<string>> validate, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputMoney(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputMonth(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputName(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputNchar(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public int InputNumber(string title, Func<object, List<string>> validate, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputPassword(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public int InputPercent(string title, Func<object, List<string>> validate, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputPhone(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public int InputPositiveNumber(string title, Func<object, List<string>> validate, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public List<Dictionary<string, object>> InputPrimitiveCollection(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputReal(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputRusWord(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputSmallDateTime(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputSmallInt(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputSmallMoney(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputString(string title, Func<object, List<string>> validate, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputString(string name, Func<string, IEnumerable<string>> validate, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputString(string name, Func<string, List<string>> validate, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public List<Dictionary<string, object>> InputStructureCollection(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputStructured(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public int InputTcpPort(string title, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputText(string title, Func<object, List<string>> validate, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputTime(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputTimestamp(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputTinyInt(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputUdt(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputUniqueIdentifier(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputUrl(ref string[] args, string title)
        {
            throw new NotImplementedException();
        }

        public string InputUrl(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputVarBinary(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public object InputVariant(string v, object value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputWeek(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputXml(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string InputYear(string title, Func<object, List<string>> value, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public void Interactive()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> MultiSelect(string title, IEnumerable<string> options, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public ConsoleKeyInfo ReadKey()
        {
            throw new NotImplementedException();
        }

        public T ReadKeyRepeat<T>(Action beforeRead, Action<object> onKeyPressed, Func<T> onCompleted, Action onCanceled)
        {
            throw new NotImplementedException();
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }

        public string ReadText(string name, Func<string, List<string>> validate, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public void SelectAction(string title, Dictionary<string, Action> actions)
        {
            throw new NotImplementedException();
        }

        public void SelectAction(string title, Dictionary<string, Action> options, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public void SelectAction1(string title, Dictionary<Action, string> actions)
        {
            throw new NotImplementedException();
        }

        public void SelectActions2(string title, Dictionary<Action, string> options, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string SelectDirectory(ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string SelectFile(string title, Func<string, List<string>> validate, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string SelectFile(string title, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public void SelectList(string title, List<string> options, Action<string> onselect)
        {
            throw new NotImplementedException();
        }

        public string Shift(ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string SingleSelect(string title, IEnumerable<string> options)
        {
            throw new NotImplementedException();
        }

        public string SingleSelect(string title, IEnumerable<string> options, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public string SingleSelect(string title, List<string> options)
        {
            throw new NotImplementedException();
        }

        public string SingleSelect(string title, List<string> options, ref string[] args)
        {
            throw new NotImplementedException();
        }

        public T Wait<T>(string title, Func<T> todo)
        {
            throw new NotImplementedException();
        }

        public void Warn(params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Write(object arg)
        {
            throw new NotImplementedException();
        }

        public void Write(object[] args)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(params object[] args)
        {
            throw new NotImplementedException();
        }

        public void WriteYellow(params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
