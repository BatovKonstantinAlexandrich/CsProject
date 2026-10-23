using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

[Icon("home")]
[Label("Переменная содержит одно из значений заданных через атрибут")]
[Description("Конструктор форм использует этот атрибут для формирования выпадающего списка с выбором значения."+
    "Приложение использует его в процессе выполнения валидации модели для проверки значений переменной. " +
    "В режиме визуальной разработки Visual Studio автоматически отображает выпадающий список с доступными значениями.")]
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class InputSelectAttribute : BaseInputAttribute {

    [Parameter]
    public List<string> Options { get; set; } = new List<string>();
    public InputSelectAttribute() : base() { }

    public InputSelectAttribute(string exp) : base()
    {
        if (string.IsNullOrWhiteSpace(exp))
            return;

        if (exp.Length > 0 && exp[0].IsEng() == false && exp[0].IsRus() == false)
            exp = exp.Substring(1);
        if (exp.Length > 0 && exp[exp.Length - 1].IsEng() == false && exp[exp.Length - 1].IsRus() == false)
            exp = exp.Substring(0, exp.Length - 1);
        if (String.IsNullOrWhiteSpace(exp) == false)
            this.Options = exp.Split(",").Select(x => x.Trim()).ToList();
         
    }
    public static IEnumerable<string> GetControlTypes() => AttrsUtil.GetControlTypes();
    public static IEnumerable<string> GetInputTypes() => BaseInputAttribute.GetInputTypes();
    public override bool IsValidValue(object value)
    {
        return true;
    }

    public InputSelectAttribute(params string[] OptionsForSelection)  {
        if (OptionsForSelection == null || OptionsForSelection.Length == 1)
            throw new System.ArgumentException("IndexIsActive");
        this.Options = OptionsForSelection.ToList();
    }
    public override string OnValidate(object model, string property, object value)
    {
        if (value is null)
            return null;
        if (Options.Contains(value.ToString()) == false)
            return $"Введите одно из значений: {Options.ToJson()}";
        return null;
    }

    public override string OnGetMessage(object model, string property, object value)
    {
        return $"Введите одно из значений: {Options.ToJson()}";
    }

    
}