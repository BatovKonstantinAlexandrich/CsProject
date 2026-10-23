
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

[Index(nameof(Name),IsUnique = true)]
public class NamedObject: BaseEntity 
{
    
    [Label("Наименование")]
    [NotNullNotEmpty("Необходимо указать наименование")]
    [UniqValidation("Имя должно иметь уникальное значение")]
    [RusText("Используйте русский имена")]        
    public virtual string Name { get; set; }

    [NotMapped()]
    [Label("Определение")]
    [NotNullNotEmpty("Необходимо указать определение")]
    [UniqValidation("Определение должно иметь доказательный характер")]
    [RusText("Используйте русский имена")]
    public virtual string Description { get; set; }
    public void OnNameChanging( string value )
    {
        Writing.ToConsole("Изменение имени сущности: "+value);
    }

        

    
}
 