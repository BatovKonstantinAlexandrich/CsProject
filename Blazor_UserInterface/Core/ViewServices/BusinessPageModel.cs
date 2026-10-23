public class BusinessPageModel : DimensionTable
{
    [Label("URI")]
    [UniqValidation]    
    public string Location { get; set; }

    [NotInput]
    public int FormID { get; set; }

    [NotInput]
    public virtual InputFormModel Form { get; set; }


}