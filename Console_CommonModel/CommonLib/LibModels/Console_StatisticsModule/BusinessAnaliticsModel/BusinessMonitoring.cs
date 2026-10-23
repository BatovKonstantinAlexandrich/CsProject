using System.Collections.Generic;

[Label("Мониторинг")]
public class BusinessMonitoring: BusinessEntity<BusinessMonitoring>  {

    public virtual List<BusinessDataset> BusinessDatasets { get; set; }
   
}
