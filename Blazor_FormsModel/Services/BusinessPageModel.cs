public class BusinessPageModel : DimensionTable
{
    [Label("URI")]
    //[UniqValue]    
    public string Location { get; set; }

    [NotInput]
    public int FormID { get; set; }

    [NotInput]
    public virtual InputFormModel Form { get; set; }


}