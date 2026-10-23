


using Newtonsoft.Json;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

[Label("Бизнес функция")]
[SearchTerms("Name")]
public class BusinessFunction : BusinessEntity<BusinessFunction>
{
    public Dictionary<string, List<string>> ValidateOptional()
    {
        
        var result = new Dictionary<string, List<string>>();
        if (this.InputId == 0)
        {
            result[nameof(InputId)] = new() { "Свойство не может принимать значение 0" };
        }
        if (this.BusinessResourceId == 0)
        {
            result[nameof(BusinessResourceId)] = new() { "Свойство не может принимать значение 0" };
        }
        return result;
    }


    /// <summary>
    /// Возвращает список всех дочерних функций
    /// </summary>
    /// <returns></returns>
    public List<BusinessFunction> FlatList()
    {        
        List<BusinessFunction> functions = new List<BusinessFunction>();
        this.SubFunctions.ForEach(sf => {
            functions.AddRange(sf.FlatList());
        });
        functions.Add(this);
        return functions;
    }


    [Label("Дочерние функции")]
    [NotMapped( )]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public List<BusinessFunction> SubFunctions { get; set; } = new List<BusinessFunction>();

    [ManyToManyAttribute("BusinessFunctions")]
    [Label("Функция бизнес группы")]
    [NotInput]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual List<global::GroupsBusinessFunctions> GroupsBusinessFunctions { get; set; }


    [Label("Роль пользователей")]    
    [SelectDictionaryAttribute(nameof(BusinessResource) + ",Name")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual BusinessResource BusinessResource { get; set; }

    [Required]
    [NotNullNotEmpty]
    public virtual int? BusinessResourceId { get; set; }

    //[Label("Бизнес логика")]
    //[NotMapped]
    //[NewSelectDictionary(nameof(MessageProtocol) + ",Name")]
    //public virtual BusinessLogic BusinessLogic { get; set; }

    [Label("Протокол запроса")]
    //[InverseProperty("To")]
    [SelectDictionary(nameof(MessageProtocol) + ",Name")]
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual MessageProtocol Input { get; set; }

    [InputNumber()]
    [NotNullNotEmpty]
    public virtual int? InputId { get; set; }





    [Label("Результирующий набор")]
    //[InverseProperty("From")]
    [NotMapped]
    [SelectDictionary(nameof(MessageProtocol) + ",Name")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual MessageProtocol Output { get; set; }
    public virtual int? OutputId { get; set; }



    [Label("Исключительный набор")]
    //[InverseProperty("From")]
    [NotMapped]
    [SelectDictionary(nameof(MessageProtocol) + ",Name")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual MessageProtocol Error { get; set; }
    public virtual int? ErrorId { get; set; }



    /// <summary>
    /// Возвращает протокол отчётных форм, определённый как входящая информация для данной бизнес функции
    /// </summary>
    /// <returns></returns>
    public object ForInput()
    {
        using (var db = new BusinessDbContext())
        {
            var prot = (from p in db.MessageProtocols where p.ToBusinessFunctionId == this.Id select p).SingleOrDefault();
            return null;
        }
    }
    public MessageProtocol GetInputProtocol()
    {
        using (var db = new BusinessDbContext())
        {
            return (from p in db.MessageProtocols where p.ToBusinessFunctionId == this.Id select p).SingleOrDefault();
        }
    }

    /// <summary>
    /// Возвращает протокол отчётных форм, определённый как исходящая информация для данной бизнес функции
    /// </summary>
    /// <returns></returns>
    public MessageProtocol ForOutput()
    {
        using (var db = new BusinessDbContext())
        {
            return (from p in db.MessageProtocols where p.FromBusinessFunctionId == this.Id select p).SingleOrDefault();
        }
    }

    public override string GetPath(string separator)
    {        


        BusinessFunction parentHier = Parent;
        return (Parent != null) ? parentHier.GetPath(separator) + separator + Name : Name;
    }


    

    public List<BusinessFunction> GetSubfunctions()
    {
        if(SubFunctions == null)
        {
            using(var db = new BusinessDbContext())
            {
                SubFunctions = (from p in db.BusinessFunctions where p.ParentId == Id select p).ToList();
            }
            
        }
        return SubFunctions;
    }

    public int? BusinessProcessId { get; set; }
    public BusinessProcess BusinessProcess { get; set; }
}
 