
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System.Collections;
using System.Collections.Generic;

[Icon("home")]
[Label("Переменная содержит одно из значений заданных через атрибут")]
[Description("Конструктор форм использует этот аттрибут для формирования выпадающего списка с выбором значения." +
    "Приложение использует его в процессе выполнения валидации модели для проверки значений переменной. ")]
public class SelectEntityAttribute : InputDictionaryAttribute
{
    public SelectEntityAttribute() : base() { }
    public SelectEntityAttribute(string exp) : base()
    {
    }
}


public class InputDictionaryAttribute : ControlAttribute
{
    public readonly string Entity;

    public List<string> Options { get; private set; } = new();
    public Dictionary<string,string> Map { get; set; } = new();



    public InputDictionaryAttribute() : base(ControlTypes.Select)
    {
    }

    public InputDictionaryAttribute(string exp) : base(ControlTypes.Select)
    {

        var ws = exp.Split(',');
        if (ws.Length < 2)
            throw new ArgumentException();
        this.Entity = ws[0];
        var items = Entity.ToType().GetEntityFasadeForType().GetAll();
        var property = ws[1];
        foreach ( var item in items )
        {
            Map[$"{((object)item).GetValue(property)}"] = ((object)item).GetValue("Id").ToString();
            this.Options.Add($"{((object)item).GetValue(property)}");
        }        
    }


    public override bool IsValidValue(object value)
    {
        return true;
    }


     
    public override string OnValidate(object model, string property, object value)
    {
        return null;
    }
    public override string OnGetMessage(object model, string property, object value)
    {
        return $"{nameof(InputSelectAttribute)} утверждает что свойству {property} может быть задано только одно из следующих значений {this.Options?.ToJson()}.";
    }

    public override AppViewBase CreateControl(InputFormField field)
    {
        var table = new TableView();
        return table;
    }

   

     

    public override AppViewBase CreateView(InputFormField field)
    {
        throw new NotImplementedException();
    }
}

public class InputDbContextDictionaryAttribute<TDbContext> : ControlAttribute
    where TDbContext : DbContextBase
{
    public readonly string Entity;

    public List<string> Options { get; private set; } = new();
    public Dictionary<string, string> Map { get; set; } = new();



    public InputDbContextDictionaryAttribute() : base(ControlTypes.Select)
    {
    }

    public InputDbContextDictionaryAttribute(string exp) : base(ControlTypes.Select)
    {
        
        var ws = exp.Split(',');
        if (ws.Length < 2)
            throw new ArgumentException();
        this.Entity = ws[0];
        using (var db = typeof(TDbContext).New<DbContextBase>())
        {
            var items = db.GetRepositoryForType(this.Entity.ToType()).Search("");
            var property = ws[1];
            foreach (var item in items)
            {
                Map[$"{((object)item).GetValue(property)}"] = ((object)item).GetValue("Id").ToString();
                this.Options.Add($"{((object)item).GetValue(property)}");
            }
        }

    }


    public override bool IsValidValue(object value)
    {
        return true;
    }



    public override string OnValidate(object model, string property, object value)
    {
        return null;
    }
    public override string OnGetMessage(object model, string property, object value)
    {
        return $"{nameof(InputSelectAttribute)} утверждает что свойству {property} может быть задано только одно из следующих значений {this.Options?.ToJson()}.";
    }

    public override AppViewBase CreateControl(InputFormField field)
    {
        var table = new TableView();
        return table;
    }



    

    public override AppViewBase CreateView(InputFormField field)
    {
        throw new NotImplementedException();
    }
}
