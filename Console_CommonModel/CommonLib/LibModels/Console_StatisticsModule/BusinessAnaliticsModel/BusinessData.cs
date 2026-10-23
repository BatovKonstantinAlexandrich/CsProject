using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System;
using System.Text.Json.Serialization;

//Загрузка данных начинаетя с этой таблицы"
[Label("Входящая информация")]
public class BusinessData : BaseEntity
{

    [NotNullNotEmpty("Показатель является обязательным полем")]
    [InputDictionary(nameof(BusinessIndicator) + ",Name")]
    public int BusinessIndicatorID { get; set; }

    [Label("Бизнес показатель")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual BusinessIndicator BusinessIndicator { get; set; }


    [NotNullNotEmpty("Набор данных является обязательным полем")]
    [InputDictionary(nameof(BusinessDataset) + ",Name")]
    public int BusinessDatasetID { get; set; }
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual BusinessDataset BusinessDataset { get; set; }

    [Label("Периодичность")]
    [NotNullNotEmpty("Периодичность является обязательным полем")]
    [InputDictionary(nameof(BusinessGranularities) + ",Name")]
    public int GranularityID { get; set; }

    [Label("Объект мониторинга")]
    [NotNullNotEmpty("Объект мониторинга является обязательным полем")]
    [InputDictionary(nameof(BusinessResource) + ",Name")]
    public int BusinessResourceID { get; set; }

    [Label("Бизнес-ресурс")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual BusinessResource BusinessResource { get; set; }

    [Label("Дата")]
    [NotNullNotEmpty("Начало периода")]
    [InputDate()]    
    public DateTime BeginDate { get; set; }

    [Label("Значение показателя")]
    [InputNumber()]
    [NotNullNotEmpty()]
    public float? IndValue { get; set; }

    [Label("Изменено")]
    [NotInput()]
    public DateTime Changed { get; set; } = DateTime.Now;
}

