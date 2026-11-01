using System;
using System.Collections.Generic;


using Console_CommonModel.API;

using static IConsoleСontrol;


namespace Console_CommonModel.CommonLib.Others
{


    public static class InputConsole
    {

        private static string[] args = new string[0];
        //реализация
        public static IConsoleСontrol control = new InputConsoleСontrol(ref args);



        public static bool DebugMode { get => control.DebugMode; set => control.DebugMode = value; }
        public static bool UserInteractive { get => control.UserInteractive; set => control.UserInteractive = value; }

        public static IEnumerable<string> CheckList(string title, IEnumerable<string> options, ref string[] args)
        {
            return control.CheckList(title, options, ref args);
        }

        public static IEnumerable<T> CheckListTitle<T>(string title, IEnumerable<T> applications, Func<T, string> convert)
        {
            return control.CheckListTitle(title, applications, convert);
        }

        public static IEnumerable<T> CheckListTitle<T>(string title, IEnumerable<T> applications, IEnumerable<T> selected, Func<T, string> convert)
        {
            return control.CheckListTitle(title, applications, selected, convert);
        }

        public static void Clear()
        {
            control.Clear();
        }

        public static string Combine(params string[] path)
        {
            return control.Combine(path);
        }

        public static bool Confirm(string name)
        {
            return control.Confirm(name);
        }

        public static bool ConfirmActionDialog(string title, Action todo)
        {
            return control.ConfirmActionDialog(title, todo);
        }

        public static bool ConfirmContinue(string title = "")
        {
            return control.ConfirmContinue(title);
        }

        public static bool ConfirmContinue(ref string[] args, string message = "")
        {
            return control.ConfirmContinue(ref args, message);
        }

        public static int ConfirmContinue(string title, Func<object, bool> value, ref string[] args)
        {
            return control.ConfirmContinue(title, value, ref args);
        }

        public static bool ConfirmContinue(string message, Func<object, List<string>> validate, ref string[] args)
        {
            return control.ConfirmContinue(message, validate, ref args);
        }

        public static void ConfirmExecute(string name, Action todo)
        {
            control.ConfirmExecute(name, todo);
        }

        public static void CreateTreeModel(string title, ref string[] args)
        {
            control.CreateTreeModel(title, ref args);
        }

        public static void Error(params object[] args)
        {
            control.Error(args);
        }

        public static void Exit()
        {
            control.Exit();
        }

        public static string GetDirectoryName(string path)
        {
            return control.GetDirectoryName(path);
        }

        public static IEnumerable<string> GetDlls()
        {
            return control.GetDlls();
        }

        public static IEnumerable<string> GetFiles()
        {
            return control.GetFiles();
        }

        public static IEnumerable<string> GetIniFiles()
        {
            return control.GetIniFiles();
        }

        public static IEnumerable<string> GetJsonFiles()
        {
            return control.GetJsonFiles();
        }

        public static List<PortInfo> GetOpenPort()
        {
            return control.GetOpenPort();
        }

        public static string GetProcessName()
        {
            return control.GetProcessName();
        }

        public static string GetWrk()
        {
            return control.GetWrk();
        }

        public static void Info(params object[] args)
        {
            control.Info(args);
        }

        

        public static string Input(string name, ref string[] args)
        {
            return control.Input(name, ref args);
        }

        public static TModel Input<TModel>(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.Input<TModel>(title, value, ref args);
        }

        public static bool InputBool(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputBool(title, value, ref args);
        }

        public static object InputChar(string v, object value, ref string[] args)
        {
            return control.InputChar(v, value, ref args);
        }

        public static string InputColor(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputColor(title, value, ref args);
        }

        public static string InputCreditCard(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputCreditCard(title, value, ref args);
        }

        public static string InputCurrency(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputCurrency(title, value, ref args);
        }

        public static string InputDate(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputDate(title, value, ref args);
        }

        public static object InputDateTime(string v, object value, ref string[] args)
        {
            return control.InputDateTime(v, value, ref args);
        }

        public static object InputDateTime2(string v, object value, ref string[] args)
        {
            return control.InputDateTime2(v, value, ref args);
        }

        public static object InputDateTimeOffset(string v, object value, ref string[] args)
        {
            return control.InputDateTimeOffset(v, value, ref args);
        }

        public static string InputDecimal(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputDecimal(title, value, ref args);
        }

        public static string InputDirectory(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputDirectory(title, value, ref args);
        }

        public static string InputEmail(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputEmail(title, value, ref args);
        }

        public static string InputEngWord(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputEngWord(title, value, ref args);
        }

        public static string InputFile(string message, Func<object, List<string>> validate, ref string[] args)
        {
            return control.InputFile(message, validate, ref args);
        }

        public static string InputFilePath(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputFilePath(title, value, ref args);
        }

        public static string InputFloat(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputFloat(title, value, ref args);
        }

        public static string InputIcon(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputIcon(title, value, ref args);
        }

        public static string InputImage(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputImage(title, value, ref args);
        }

        public static int InputInt(string title, Func<object, List<string>> validate, ref string[] args)
        {
            return control.InputInt(title, validate, ref args);
        }

        public static object InputMoney(string v, object value, ref string[] args)
        {
            return control.InputMoney(v, value, ref args);
        }

        public static string InputMonth(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputMonth(title, value, ref args);
        }

        public static string InputName(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputName(title, value, ref args);
        }

        public static object InputNchar(string v, object value, ref string[] args)
        {
            return control.InputNchar(v, value, ref args);
        }

        public static int InputNumber(string title, Func<object, List<string>> validate, ref string[] args)
        {
            return control.InputNumber(title, validate, ref args);
        }

        public static string InputPassword(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputPassword(title, value, ref args);
        }

        public static int InputPercent(string title, Func<object, List<string>> validate, ref string[] args)
        {
            return control.InputPercent(title, validate, ref args);
        }

        public static string InputPhone(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputPhone(title, value, ref args);
        }

        public static int InputPositiveNumber(string title, Func<object, List<string>> validate, ref string[] args)
        {
            return control.InputPositiveNumber(title, validate, ref args);
        }

        public static List<Dictionary<string, object>> InputPrimitiveCollection(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputPrimitiveCollection(title, value, ref args);
        }

        public static object InputReal(string v, object value, ref string[] args)
        {
            return control.InputReal(v, value, ref args);
        }

        public static string InputRusWord(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputRusWord(title, value, ref args);
        }

        public static object InputSmallDateTime(string v, object value, ref string[] args)
        {
            return control.InputSmallDateTime(v, value, ref args);
        }

        public static object InputSmallInt(string v, object value, ref string[] args)
        {
            return control.InputSmallInt(v, value, ref args);
        }

        public static object InputSmallMoney(string v, object value, ref string[] args)
        {
            return control.InputSmallMoney(v, value, ref args);
        }

        public static string InputString(string title, Func<object, List<string>> validate, ref string[] args)
        {
            return control.InputString(title, validate, ref args);
        }

        public static string InputString(string name, Func<string, IEnumerable<string>> validate, ref string[] args)
        {
            return control.InputString(name, validate, ref args);
        }

        public static string InputString(string name, Func<string, List<string>> validate, ref string[] args)
        {
            return control.InputString(name, validate, ref args);
        }

        public static List<Dictionary<string, object>> InputStructureCollection(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputStructureCollection(title, value, ref args);
        }

        public static object InputStructured(string v, object value, ref string[] args)
        {
            return control.InputStructured(v, value, ref args);
        }

        public static int InputTcpPort(string title, ref string[] args)
        {
            return control.InputTcpPort(title, ref args);
        }

        public static string InputText(string title, Func<object, List<string>> validate, ref string[] args)
        {
            return control.InputText(title, validate, ref args);
        }

        public static string InputText(string title)
        {
            string[] args = new string[0];
            return InputText(title, p => null, ref args);
        }

        public static string InputTime(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputTime(title, value, ref args);
        }

        public static object InputTimestamp(string v, object value, ref string[] args)
        {
            return control.InputTimestamp(v, value, ref args);
        }

        public static object InputTinyInt(string v, object value, ref string[] args)
        {
            return control.InputTinyInt(v, value, ref args);
        }

        public static object InputUdt(string v, object value, ref string[] args)
        {
            return control.InputUdt(v, value, ref args);
        }

        public static object InputUniqueIdentifier(string v, object value, ref string[] args)
        {
            return control.InputUniqueIdentifier(v, value, ref args);
        }

        public static string InputUrl(ref string[] args, string title)
        {
            return control.InputUrl(ref args, title);
        }

        public static string InputUrl(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputUrl(title, value, ref args);
        }

        public static object InputVarBinary(string v, object value, ref string[] args)
        {
            return control.InputVarBinary(v, value, ref args);
        }

        public static object InputVariant(string v, object value, ref string[] args)
        {
            return control.InputVariant(v, value, ref args);
        }

        public static string InputWeek(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputWeek(title, value, ref args);
        }

        public static string InputXml(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputXml(title, value, ref args);
        }

        public static string InputYear(string title, Func<object, List<string>> value, ref string[] args)
        {
            return control.InputYear(title, value, ref args);
        }

        public static void Interactive()
        {
            control.Interactive();
        }

        public static IEnumerable<string> MultiSelect(string title, IEnumerable<string> options, ref string[] args)
        {
            return control.MultiSelect(title, options, ref args);
        }



        public static T ReadKeyRepeat<T>(Action beforeRead, Action<object> onKeyPressed, Func<T> onCompleted, Action onCanceled)
        {
            return control.ReadKeyRepeat(beforeRead, onKeyPressed, onCompleted, onCanceled);
        }

        public static string ReadLine()
        {
            return control.ReadLine();
        }

        public static string ReadText(string name, Func<string, List<string>> validate, ref string[] args)
        {
            return control.ReadText(name, validate, ref args);
        }

    

        public static void SelectActions2(string title, Dictionary<Action, string> options, ref string[] args)
        {
            control.SelectAction(title, options, ref args);
        }

      

        public static void SelectAction(string title, Dictionary<string, Action> options, ref string[] args)
        {
            control.SelectAction(title, options, ref args);
        }

        public static string SelectDirectory(ref string[] args)
        {
            return control.SelectDirectory(ref args);
        }

        public static string SelectFile(string title, Func<string, List<string>> validate, ref string[] args)
        {
            return control.SelectFile(title, validate, ref args);
        }

        public static string SelectFile(string title, ref string[] args)
        {
            return control.SelectFile(title, ref args);
        }

        public static void SelectList(string title, List<string> options, Action<string> onselect, ref string[] args)
        {
            control.SelectList(title, options, onselect, ref args);
        }

        public static void SelectListWithExcape(string title, List<string> options, Action<string> onselect, ref string[] args)
        {
            options.Add("Назад");
            control.SelectList(title, options, onselect, ref args);
        }
        public static void SelectListWithExit(string title, List<string> options, Action<string> onselect, ref string[] args)
        {
            options.Add("Выход");
            control.SelectList(title, options, onselect, ref args);
        }

        public static string Shift(ref string[] args)
        {
            return control.Shift(ref args);
        }
 

        public static string SingleSelect(string title, IEnumerable<string> options, ref string[] args)
        {
            return control.SingleSelect(title, options, ref args);
        }

   

        public static string SingleSelect(string title, List<string> options, ref string[] args)
        {
            return control.SingleSelect(title, options, ref args);
        }

        public static T Wait<T>(string title, Func<T> todo)
        {
            return control.Wait(title, todo);
        }

        public static void Warn(params object[] args)
        {
            control.Warn(args);
        }

        public static void Write(object arg)
        {
            control.Write(arg);
        }

        public static void Write(object[] args)
        {
            control.Write(args);
        }

        public static void WriteLine(params object[] args)
        {
            control.WriteLine(args);
        }

        public static void WriteYellow(params object[] args)
        {
            control.WriteYellow(args);
        }
        public static IClientAPI clientAPI;
        public static void CreateSubmitDialog(string title, object control, Action<object> value)
        {
            clientAPI.SubmitDialog(title,control,value);  
        }

        internal static string SelectOption(string[] strings)
        {
            string[] args = new string[0];
            return SingleSelect("", strings, ref args);
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


