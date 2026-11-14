using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;
using Console_CommonModel.CommonLib.Others;

using System.Reflection;

/// <summary>
/// Модель вызова функции удалённой процедуры
/// </summary>
public class ActionModel: MethodModel
{
    public string Method { get; set; }


    public string OwnerClientId { get; set; }

    private readonly Func<Dictionary<string, object>, object> Todo;

    public ActionModel(string name, Func<Dictionary<string, object>, object> todo)
    {
        Name = name;
        Todo = todo;
    }

    public async Task<object> Exec(Dictionary<string, object> args)
    {
        Console.WriteLine($"[{nameof(ActionModel)}] Name={Name} выполнение процедуры");
        return Todo(args);
    }




    /// <summary>
    /// ввод параметров процедуры
    /// </summary>
    public void Input(IConsoleСontrol control, ref string[] args)
    {
        control.Clear();

        this.EnsureIsValide();

        var ProgramAction = this.Type.ToType().GetMethods().FirstOrDefault(mbox => mbox.Name == this.Name);
        control.WriteLine($"\n{  $" [{ProgramAction.Name}]"}");

        int n = 1;
        var ProgramData = this.Type.ToType().New();

        var invokeParams = new List<object>();
        var ProgramArguments = new Dictionary<string, object>();
        if (ProgramAction == null)
        {
            throw new Exception("Не выбрано действие для выполнения");
        }
        else
        {
            foreach (ParameterInfo par in ProgramAction.GetParameters())
            {

                if (par.ParameterType.IsEnum == false)
                {
                    var attrs = ProgramData.GetType().GetMethodParameterAttributes(ProgramAction.Name, par.Name);
                    string expression = attrs.Any(a => a.Key == nameof(LabelAttribute)) ? attrs.FirstOrDefault(a => a.Key == nameof(LabelAttribute)).Value : null;
                    var label = expression is null ? null : ProgramData.Interpolate(expression);
                    var value = control.InputText(
                        $"{n++}) {label} [{par.Name}]",
                        data => {
                            List<string> errors = new();
                            var attrs = par.GetParameterAttributes();
                            foreach (var kv in attrs)
                            {
                                var attrType = kv.Key.ToType();
                                if (attrType.IsExtends(typeof(BaseValidationAttribute)))
                                {
                                    BaseValidationAttribute validation = attrType.Create<BaseValidationAttribute>(new object[] { kv.Value });
                                    string validationResult = validation.ValidateValue(data);
                                    if (validationResult != null)
                                    {
                                        errors.Add(validationResult);
                                    }
                                }
                            }
                            return errors;
                        },
                        ref args);
                    object result = TextDataSetter.ToType(value, par.ParameterType);
                    ProgramArguments[par.Name] = result;
                    invokeParams.Add(result);
                }
                else
                {

                    string option = InputConsole.SelectOption(System.Enum.GetNames(par.ParameterType));
                    object result = null;
                    System.Enum.TryParse(par.ParameterType, option, true, out result);
                    ProgramArguments[par.Name] = result;
                    invokeParams.Add(result);
                }

            }
        }
        try
        {
            object ActionResult = null;
            control.ConfirmExecute($"Выполнить: {ProgramAction.Name}({invokeParams.ToJson()})", () =>
            {
                control.Info(ActionResult = ProgramAction.Invoke(ProgramData, invokeParams.ToArray()));
                InputConsole.ConfirmContinue();
            });

            
        }
        catch (Exception ex)
        {
            throw new Exception($"Исключение проброшено из метода {ProgramData.GetTypeName()}.{ProgramAction.Name}", ex);
        }
    }

}