using Console_CommonModel.CommonLib.LibViews.ViewEvents.Dictionary;

[Label("Список навигации")]
public class NavListView: NavView
{
    private ListView navList;

    public NavListView( )
    {
        this.navList = new ListView();
        this.navList.OnSelected += (p) => {
            if (p is not null)
            {
                var op = (Option)p;
                OnNavSelected(op.Value);
            }
        }; ;
        this.Left = navList;
        this.Left.Width = 222;
        this.Config.OnChanged += OnConfigChanged;
    }

    public void OnConfigChanged(object sender, ChangingDictionaryEvent<string, ViewItem> e)
    {
        switch (e.Target.Action)
        {
            case "added":
                this.navList.Options.Add(new()
                {
                    Value = e.Target.Key
                });
                break;

            case "removed":
                this.navList.Options.Remove(
                    this.navList.Options.First(o => o.Value == e.Target.Key));
                break;
        }
    }
}
