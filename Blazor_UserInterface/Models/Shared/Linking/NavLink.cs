using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class NavLink : BaseLink
{
    public string Href { get; set; }
    public string Icon { get; set; }
    public string Tooltip { get; set; }
    public string Label { get; set; }
    public bool IsActive { get; set; } = false;

    public ActionLink toActionLink(Func<object, NavigationManager> GetNavigationManager)
    {
        return new ActionLink()
        {
            Href = Href,
            Icon = this.Icon,
            Label = this.Label,
            Tooltip = this.Tooltip,
            OnMessage = (evt) => {
                GetNavigationManager(this).NavigateTo(Href);
            }
        };
    }
}