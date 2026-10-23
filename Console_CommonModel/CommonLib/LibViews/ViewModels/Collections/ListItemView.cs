using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
[ViewUsageExample("var view = new ListItemView { Text = \"Элемент списка\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<list-item-view Text=\"Элемент списка\" Dock=\"Top\" />")]
[Label("Элемент списка")]
public class ListItemView: ViewItem
{

    [NotNullNotEmpty("Необходимо задать значение для заголовка")]
    public string Title { get; set; }


    [NotNullNotEmpty("Необходимо указать ссылку")]
    public string Href { get; set; }
    
    public ListItemView()
    {
        Selectable = true;
        Title = "Item";
    }
 
}