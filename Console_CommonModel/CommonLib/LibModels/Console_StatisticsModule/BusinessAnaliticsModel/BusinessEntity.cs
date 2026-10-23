/// <summary>
/// Бизнес-сущность
/// </summary>
public abstract class BusinessEntity<T>: HierTable<T> where T: class
{
    [ManyToMany("Datasets")]
    public virtual List<BusinessDataset> BusinessDataset { get; set; }
}
 