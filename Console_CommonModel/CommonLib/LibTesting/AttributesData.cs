using Namotion.Reflection;

[Icon("home")]
[Label("")]
[Description("Контроллер предназначен для .")]
public class AttributesData : MyValidatableObject
{
    public AttributesData(object target)
    {
        if (target is null)
            throw new ArgumentNullException(nameof(target));
        var type = target is Type ? (Type)target : target.GetType();
        foreach (var kv in type.GetAttributes())
        {
            var propertyName = kv.Key.Replace("Attribute", "");
            if (this.HasProperty(propertyName))
            {
                this.SetProperty(propertyName, kv.Value);
            }
        }
        this.ValidationResults = this.Validate();
    }
    public AttributesData(Dictionary<string, string> attributes)
    {     
        foreach (var kv in attributes)
        {
            var propertyName = kv.Key.Replace("Attribute", "");
            if (this.HasProperty(propertyName))
            {
                this.SetProperty(propertyName, kv.Value);
            }
        }
        this.ValidationResults = this.Validate();
    }
}