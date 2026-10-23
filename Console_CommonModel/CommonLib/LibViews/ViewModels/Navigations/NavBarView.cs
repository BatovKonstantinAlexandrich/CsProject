using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
[ViewUsageExample("var view = new NavBarView { Text = \"Панель навигации\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<nav-bar-view Text=\"Панель навигации\" Dock=\"Top\" />")]
[Label("Панель навигации")]
public class NavBarView: ViewItem, ILink
{
    public string Brand { get; set; }
    
    public string Icon { get; set; }
    public string Label { get; set; }
    public string Href { get; set; }
    public bool IsActive { get; set; }
    public object Item { get; set; }
    public string Tooltip { get; set; }

    public LinkViewModel Active { get; set; }
    public List<ILink> ChildLinks { get; set; }


    public NavBarView()
    {
        Brand = "EckUmOC";
        Item = new FlexContainer();


        Init();
        Changed = false;
    }


    public void AddLink(ILink link)
    {
        if(ChildLinks == null)
        {
            ChildLinks = new List<ILink>();
        }
        ChildLinks.Add(link);
    }

}