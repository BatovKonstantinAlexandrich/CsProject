
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


[Label("Группировка сервисов")]
public class ServiceGroupBindings : BaseEntity 
{
    [NotInput]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ServiceContext ServiceContext { get; set; }

    
    [Label("Служба")]
    public int ServiceContextId { get; set; }


    [NotInput]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ServiceGroup ServiceGroup { get; set; }

    [Label("Группа")]
    public int ServiceGroupId { get; set; }

}

