using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System.ComponentModel.DataAnnotations.Schema;

[Label("Именованный объект")]
[Description("Содержит уникальное значение Name")]
public abstract class NamedObject: BaseEntity,  INamedObject  
{

    [Label("Наименование")]
    [InputOrder(1)]
    [InputEngWord]
    //[UniqValue]
    [NotNullNotEmpty(ErrorMessage = "Необходимо указать наименование")]
    public virtual string Name { get; set; }
    

    public NamedObject()
    {
        //this.Help = this.GetType().GetDescription();
        this.Name = "";
    }

    public override string ToString()
    {
        return Name;
    }



}
