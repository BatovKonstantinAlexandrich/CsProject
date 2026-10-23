using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class ViewExtensions
{
    public static List<string> GetDeclaredActionEventNames(this ViewItem designer)
    {
        return designer.GetType().GetProperties().Where(p => p.PropertyType.IsExtends(typeof(Action<object, EventArgs>))).Select(p => p.Name).ToList();
    }


}

