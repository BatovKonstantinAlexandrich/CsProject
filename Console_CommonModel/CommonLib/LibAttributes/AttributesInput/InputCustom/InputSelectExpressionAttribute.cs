using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

[Icon("home")]
[Label("Переменная содержит одно из значений заданных через атрибут")]
[Description("Конструктор форм использует этот атрибут для формирования выпадающего списка с выбором значения." +
    "Приложение использует его в процессе выполнения валидации модели для проверки значений переменной. ")]
public class InputSelectExpressionAttribute : BaseInputAttribute
{
    private readonly string expression;

    public InputSelectExpressionAttribute() : base(InputTypes.Custom) { }

    public InputSelectExpressionAttribute(string exp) : base(InputTypes.Custom)
    {
        this.expression = exp.Replace("\"", "");
    }

    public string GetExpression() => this.expression;
    public ChangingList<string> GetOptions(object model)
    {
        var list = new ChangingList<string>();
        dynamic o = Expressions.Compile(this.expression.Replace("\"", ""), model);
        /*var o = model.GetValue(this.expression.Replace("\"", ""));
        if (o is ChangingList)
        {            
            ((ChangingList)o).Bind<string>(list, p => $"{p}");
        }
        list.AddRange((IEnumerable<string>)o);*/
        //var Options = (List<string>)o;
        //this.Info(((object)o).ToJson());

        return list;
    }

    public override string OnValidate(object model, string property, object value)
    {
        if (value is null)
            return null;
        object o = Expressions.Compile(this.expression.Replace("\"",""), model);
        //this.Debug(o);
        var Options = (IEnumerable<string>)o;
        if (Options.Contains(value.ToString()) == false)
            return $"Введите одно из значений: {Options.ToJson()}";
        return null;
    }

    public override string OnGetMessage(object model, string property, object value)
    {
        object o = Expressions.Compile(this.expression, model);
        var Options = (IEnumerable<string>)o;
        return $"Введите одно из значений: {Options.ToJson()}";
    }


}
