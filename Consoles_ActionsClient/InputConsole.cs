using Console_CommonModel.CommonLib.Others;

using System;
using System.Collections.Generic;

 

namespace ClassModelLib.ServicesModule
{ 
//{
//    public interface IConsoleСontrol
//    {
//        bool DebugMode { get; set; }
//        bool UserInteractive { get; set; }

//        IEnumerable<string> CheckList(string title, IEnumerable<string> options, ref string[] args);
//        IEnumerable<T> CheckListTitle<T>(string title, IEnumerable<T> applications, Func<T, string> convert);
//        IEnumerable<T> CheckListTitle<T>(string title, IEnumerable<T> applications, IEnumerable<T> selected, Func<T, string> convert);
//        void Clear();
//        string Combine(params string[] path);
//        bool Confirm(string name);
//        bool ConfirmActionDialog(string title, Action todo);
//        bool ConfirmContinue(string title = "");
//        bool ConfirmContinue(ref string[] args, string message = "");
//        int ConfirmContinue(string title, Func<object, bool> value, ref string[] args);
//        bool ConfirmContinue(string message, Func<object, List<string>> validate, ref string[] args);
//        void ConfirmExecute(string name, Action todo);
//        void CreateTreeModel(string title, ref string[] args);
//        void Error(params object[] args);
//        void Exit();
//        string GetDirectoryName(string path);
//        IEnumerable<string> GetDlls();
//        IEnumerable<string> GetFiles();
//        IEnumerable<string> GetIniFiles();
//        IEnumerable<string> GetJsonFiles();
//        List< PortInfo> GetOpenPort();
//        string GetProcessName();
//        string GetWrk();
//        void Info(params object[] args);
//        object Input(object pObject);
//        string Input(string name, ref string[] args);
//        TModel Input<TModel>(string title, Func<object, List<string>> value, ref string[] args);
//        bool InputBool(string title, Func<object, List<string>> value, ref string[] args);
//        object InputChar(string v, object value, ref string[] args);
//        string InputColor(string title, Func<object, List<string>> value, ref string[] args);
//        string InputCreditCard(string title, Func<object, List<string>> value, ref string[] args);
//        string InputCurrency(string title, Func<object, List<string>> value, ref string[] args);
//        string InputDate(string title, Func<object, List<string>> value, ref string[] args);
//        object InputDateTime(string v, object value, ref string[] args);
//        object InputDateTime2(string v, object value, ref string[] args);
//        object InputDateTimeOffset(string v, object value, ref string[] args);
//        string InputDecimal(string title, Func<object, List<string>> value, ref string[] args);
//        string InputDirectory(string title, Func<object, List<string>> value, ref string[] args);
//        string InputEmail(string title, Func<object, List<string>> value, ref string[] args);
//        string InputEngWord(string title, Func<object, List<string>> value, ref string[] args);
//        string InputFile(string message, Func<object, List<string>> validate, ref string[] args);
//        string InputFilePath(string title, Func<object, List<string>> value, ref string[] args);
//        string InputFloat(string title, Func<object, List<string>> value, ref string[] args);
//        string InputIcon(string title, Func<object, List<string>> value, ref string[] args);
//        string InputImage(string title, Func<object, List<string>> value, ref string[] args);
//        int InputInt(string title, Func<object, List<string>> validate, ref string[] args);
//        object InputMoney(string v, object value, ref string[] args);
//        string InputMonth(string title, Func<object, List<string>> value, ref string[] args);
//        string InputName(string title, Func<object, List<string>> value, ref string[] args);
//        object InputNchar(string v, object value, ref string[] args);
//        int InputNumber(string title, Func<object, List<string>> validate, ref string[] args);
//        string InputPassword(string title, Func<object, List<string>> value, ref string[] args);
//        int InputPercent(string title, Func<object, List<string>> validate, ref string[] args);
//        string InputPhone(string title, Func<object, List<string>> value, ref string[] args);
//        int InputPositiveNumber(string title, Func<object, List<string>> validate, ref string[] args);
//        List<Dictionary<string, object>> InputPrimitiveCollection(string title, Func<object, List<string>> value, ref string[] args);
//        object InputReal(string v, object value, ref string[] args);
//        string InputRusWord(string title, Func<object, List<string>> value, ref string[] args);
//        object InputSmallDateTime(string v, object value, ref string[] args);
//        object InputSmallInt(string v, object value, ref string[] args);
//        object InputSmallMoney(string v, object value, ref string[] args);
//        string InputString(string title, Func<object, List<string>> validate, ref string[] args);
//        string InputString(string name, Func<string, IEnumerable<string>> validate, ref string[] args);
//        string InputString(string name, Func<string, List<string>> validate, ref string[] args);
//        List<Dictionary<string, object>> InputStructureCollection(string title, Func<object, List<string>> value, ref string[] args);
//        object InputStructured(string v, object value, ref string[] args);
//        int InputTcpPort(string title, ref string[] args);
//        string InputText(string title, Func<object, List<string>> validate, ref string[] args);
//        string InputTime(string title, Func<object, List<string>> value, ref string[] args);
//        object InputTimestamp(string v, object value, ref string[] args);
//        object InputTinyInt(string v, object value, ref string[] args);
//        object InputUdt(string v, object value, ref string[] args);
//        object InputUniqueIdentifier(string v, object value, ref string[] args);
//        string InputUrl(ref string[] args, string title);
//        string InputUrl(string title, Func<object, List<string>> value, ref string[] args);
//        object InputVarBinary(string v, object value, ref string[] args);
//        object InputVariant(string v, object value, ref string[] args);
//        string InputWeek(string title, Func<object, List<string>> value, ref string[] args);
//        string InputXml(string title, Func<object, List<string>> value, ref string[] args);
//        string InputYear(string title, Func<object, List<string>> value, ref string[] args);
//        void Interactive();
//        IEnumerable<string> MultiSelect(string title, IEnumerable<string> options, ref string[] args);
//        ConsoleKeyInfo ReadKey();
//        T ReadKeyRepeat<T>(Action beforeRead, Action<object> onKeyPressed, Func<T> onCompleted, Action onCanceled);
//        string ReadLine();
//        string ReadText(string name, Func<string, List<string>> validate, ref string[] args);
//        void SelectAction(string title, Dictionary<Action, string> actions);
//        void SelectAction(string title, Dictionary<Action, string> options, ref string[] args);
//        void SelectAction(string title, Dictionary<string, Action> actions);
//        void SelectAction(string title, Dictionary<string, Action> options, ref string[] args);
//        string SelectDirectory(ref string[] args);
//        string SelectFile(string title, Func<string, List<string>> validate, ref string[] args);
//        string SelectFile(string title, ref string[] args);
//        void SelectList(string title, List<string> options, Action<string> onselect);
//        string Shift(ref string[] args);
//        string SingleSelect(string title, IEnumerable<string> options);
//        string SingleSelect(string title, IEnumerable<string> options, ref string[] args);
//        string SingleSelect(string title, List<string> options);
//        string SingleSelect(string title, List<string> options, ref string[] args);
//        T Wait<T>(string title, Func<T> todo);
//        void Warn(params object[] args);
//        void Write(object arg);
//        void Write(object[] args);
//        void WriteLine(params object[] args);
//        void WriteYellow(params object[] args);
//    }

    public static class InputConsole
    {
        public static IConsoleСontrol control = new ConsoleСontrol();

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

        public static object Input(object pObject)
        {
            return control.Input(pObject);
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

        public static void SelectAction1(string title, Dictionary<Action, string> actions)
        {
            control.SelectAction(title, actions);
        }

        public static void SelectActions2(string title, Dictionary<Action, string> options, ref string[] args)
        {
            control.SelectAction(title, options, ref args);
        }

        public static void SelectAction(string title, Dictionary<string, Action> actions)
        {
            control.SelectAction(title, actions);
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

        public static void SelectList(string title, List<string> options, Action<string> onselect)
        {
            control.SelectList(title, options, onselect);
        }

        public static string Shift(ref string[] args)
        {
            return control.Shift(ref args);
        }

        public static string SingleSelect(string title, IEnumerable<string> options)
        {
            return control.SingleSelect(title, options);
        }

        public static string SingleSelect(string title, IEnumerable<string> options, ref string[] args)
        {
            return control.SingleSelect(title, options, ref args);
        }

        public static string SingleSelect(string title, List<string> options)
        {
            return control.SingleSelect(title, options);
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
    }




}
