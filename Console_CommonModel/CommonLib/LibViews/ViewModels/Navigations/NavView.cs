using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Namotion.Reflection;

using System.Text;

/// <summary>
/// Содержит список навигации
/// </summary>
[ViewUsageExample("var view = new NavView { Text = \"Панель маршрутизации\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<nav-view Text=\"Панель маршрутизации\" Dock=\"Top\" />")]
[Label("Панель маршрутизации")]
public class NavView: AppViewBase, INavView
{
    //[InputCollectionDictionary()]
    [Label("Допустимые состояния")]
    public ChangingDictionary<string, ViewItem> Config { get; set; } = new();
    
    public readonly AppViewBase contentPane;

    public List<string> GetNavOptions() => Config.Keys.ToList();

    public NavView()
    {                
        this.LayoutFill(this.contentPane = new AppViewBase());
        var nvd = new NavViewDesigner(this);
         
        this.ViewDesigner = nvd;
        
    }

    protected override void OnDesignModeChanged(PropertyChangedMessage message)
    {
        base.OnDesignModeChanged(message);
       
    }


    public override IControlBase ToControl()
        => base.ToControl<PanelsView>();


    public virtual void OnNavSelected(string selected)
    {
        if (selected is null)
            return;
        var key = selected.ToString();
        //this.Info(key);
        
        if (Config.ContainsKey(key))
        {
            var view = Config[key];
            if(view.HasProperty("Text"))
                this.Info(view.GetProperty("Text"));
            this.contentPane.LayoutFill(view);
            this.contentPane.MinHeight = 320;
            this.contentPane.MinWidth = 320;
        }
        else
        {
            this.Warn($"Не найдено представления {key}");
        }        
    }

    [Label("Добавить представление навигации")]
    public void AddNavOption(string selected, AppViewBase view)
    {
        this.Config[selected] = view;
    }

    [Label("Удалить представление навигации")]
    public void RemoveNavOption([InputSelectExpression("{{GetNavKeys()}}")]string selected)
    {
        this.Config.Remove(selected);
    }

    public string GetNavKeys()
    {
        StringBuilder sb = new();
        foreach(var key in this.Config.Keys)
        {
            sb.Append(key);
            sb.Append(",");
        }
        var res = sb.ToString();
        res = res.Substring(0, res.Length - 1);
        return res;
    }
}
