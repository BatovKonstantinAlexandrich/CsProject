using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
 
[Label("Набор данных")]
[SearchTerms("Name,Description")]
public class BusinessDataset: HierTable<MessageProtocol> 
{        
    //public int? BusinessFunctionId { get; set; }
    //public int? BusinessLoginId { get; set; }
    //public int? BusinessProcessId { get; set; }
    //public int? BusinessReportId { get; set; }
    //public int? MessageProtocolId { get; set; }


    [NotMapped]
    [NotInput]
    public int DatasourceID
    {
        get
        {
            return BusinessDatasourceId;
        }
        set
        {
            BusinessDatasourceId=value;
        }
    }

    [Label("Источник данных")]
    [NotNullNotEmpty("Необходимо выбрать источник")]
    [InputDictionary(nameof(BusinessDatasource)+",Name")]
    [InputOrder(3)]
    public int BusinessDatasourceId { get; set; }

    [Label("Источник данных")]
    [NotInput]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public virtual BusinessDatasource BusinessDatasource { get; set; }


    [Label("Скрипт")]
    [NotNullNotEmpty("Введите скрипт")]
    [InputMultilineText()]
    [InputOrder(4)]
    public string Expression { get; set; }

     

    public string ExpressionType { get; set; }


    public int? BusinessIndicatorId { get; set; }
    public BusinessIndicator BusinessIndicator { get; set; }
    public int? BusinessReportId { get; set; }
    public BusinessReport BusinessReport { get; set; }

    public int? BusinessProcessId { get; set; }
    public BusinessProcess BusinessProcess { get; set; }


    public int? BusinessFunctionId { get; set; }
    public BusinessFunction BusinessFunction { get; set; }

    public int? BusinessLogicId { get; set; }
    public BusinessLogic BusinessLogic { get; set; }

    [NotMapped()]
    public List<string> Tables = new();

               
    public BusinessDataset() {       
        this.Tables = new List<string>();
    }         
}
 