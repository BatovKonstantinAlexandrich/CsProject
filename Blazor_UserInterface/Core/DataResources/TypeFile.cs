using NetCoreConstructorAngular.Data.DataAttributes.AttributeInput;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

public class TypeFile: BaseEntity
{
     
    [NotNullNotEmpty("Необходимо ввести задать тип ресурса (MimeType)")]
    public string Mime { get; set; }
        
    
    [NotNullNotEmpty("Необходимо указать наименование ресурса")]
    public string Name { get; set; }

    [InputFile("*.*")]
    
    [NotNullNotEmpty("Необходимо ввести бинарные данные ресурса")]
    public byte[] Data { get; set; }


    [InputDateTime( )]
    [NotNullNotEmpty("Необходимо указать время создания ресурса")]
    public DateTime Changed { get; set; }

}
