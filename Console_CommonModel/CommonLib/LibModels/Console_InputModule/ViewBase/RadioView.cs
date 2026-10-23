using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

[Label("Переключатель")]
public class RadioView: AppViewBase
{
    public RadioView()
    {
    }

    [Label("Name")]
    
    public string Name { get; set; }
    
    [Label("Options")]
    public List<string> Options { get; set; }
    
    public Action<object> OnSelected { get; set; }
}