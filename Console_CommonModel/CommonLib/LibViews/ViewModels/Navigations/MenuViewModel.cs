[Label("Пункт меню")]
public class MenuViewModel : LinkViewModel
{
    
    [Label("Menu Items")]
    public List<LinkViewModel> MenuItems { get; set;  }



    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public Action<string> OnCommand;

    public MenuViewModel()
    {
        this.MenuItems = new();
        this.OnCommand = (command) => {
            this.Info($"OnCommand: {command}");

        };
    }
}