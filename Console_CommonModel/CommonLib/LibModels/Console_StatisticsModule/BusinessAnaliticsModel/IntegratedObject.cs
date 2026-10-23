using Microsoft.EntityFrameworkCore;

[Index(nameof(Code))]
public class IntegratedObject: DescriptableObject
{
    [Label("Код")]
    [NotNullNotEmpty()]
    //[UniqValue()]
    [InputEngWord()]
    [InputOrder(4)]    
    public virtual string Code { get; set; }
}
