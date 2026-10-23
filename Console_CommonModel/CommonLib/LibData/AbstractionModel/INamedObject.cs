using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public interface INamedObject
{
    [DisplayName("Наименование")]
    [Required(ErrorMessage = "Необходимо указать наменование")]
    public string Name { get; set; }// = "наименование";


}

public class DescriptableObject : NamedObject, IDescriptableObject
{
    [NotInput]
    [InputOrder(4)]
    [NotMapped]
    [Label("Подсказка при вводе")]
    [InputIcon()]
    public virtual string? Help { get; set; }

    [NotInput]
    [Label("Иконка")]
    [NotMapped]
    [InputOrder(3)]
    public virtual string? Icon { get; set; }

    [Label("Надпись")]
    [InputText]
    [InputOrder(2)]
    public virtual string? Label { get; set; }


    [Label("Описание")]
    [InputOrder(5)]
    [InputMultilineText()]
    //[InputRusText()]
    public virtual string? Description { get; set; }

    public DescriptableObject() : base()
    {
        this.Icon = "folder.png";
        this.Label = "Надпись"; ;
        this.Description = "This is description";
    }
}
