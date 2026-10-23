
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ViewComponentGenerator
{

    /*public NavBar CreateControllerNavigartion(Microsoft.AspNetCore.Mvc.Rendering.ViewContext ctx)
    {
        
        string area = ctx.RouteData.Values.ContainsKey("area") ? ctx.RouteData.Values["area"].ToString() : null;
        string controller = ctx.RouteData.Values["controller"].ToString();
        var ctype = ReflectionService.TypeForName(controller + "Controller");
        
        return CreateControllerNavigation(   ctype   );
    }*/

    /*public static NavBar CreateControllerNavigation( Type ctype )
    {
        if (ctype == null)
            throw new Exception("Ссылка на Null");
        MyControllerModel model = new ControllerTypeConverter().CreateModel(ctype);
        
        string controller = ctype.Name.Replace("Controller","");
        var res = new NavBar();
        res.Label = Attrs.LabelFor(ctype);
        res.Tooltip = Attrs.ForDescription(ctype);
        res.Href = model.Path;
        List<string> names = ReflectionService.GetOwnMethodNames(ctype);
        foreach (var action in model.Actions)
        {
            if (names.Contains(action.Key))
            {
                var link = new Link() {
                    Tooltip = action.Value.Attributes.ContainsKey(nameof(DescriptionAttribute)) ?
                        action.Value.Attributes[nameof(DescriptionAttribute)].ToString() :
                        action.Key,
                    Label = action.Value.Attributes.ContainsKey(nameof(LabelAttribute)) ?
                        action.Value.Attributes[nameof(LabelAttribute)].ToString() :
                        action.Key,
                    Href = ControllerHelp.PathFor(ctype) + "/" + action.Key
                };
                res.AddLink(link);
            }

        }
        return res;
    }*/
}