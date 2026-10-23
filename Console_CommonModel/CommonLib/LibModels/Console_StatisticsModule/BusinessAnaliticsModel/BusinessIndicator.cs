 

using System;
 
 
[Label("Бизнес показатель")]
[SearchTerms("Name")]
public class BusinessIndicator : BusinessEntity<BusinessIndicator>
{

    [Label("Единицы измерения")]
    [NotNullNotEmpty("Укажите единицы измерения")]
    public string Unit { get; set; } = "штуки";

    [NotNullNotEmpty("Необходимо ввести описание")]
    public override string Description { get; set; } = "";

    [Label("Негативная оценка")]
    public bool IsNegative { get; set; } = false;

}
 