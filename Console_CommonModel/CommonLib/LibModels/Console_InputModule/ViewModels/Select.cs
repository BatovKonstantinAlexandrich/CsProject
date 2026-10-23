
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Select: AppViewBase
{
    public object Value { get; set; }
    public Dictionary<object, object> Options { get; set; }

    public Select() { }

    //public override Control ToWinFormsControl()
    //{
    //    var factory = new ControlsFactory();
    //    return factory.CreateMaterialList(
    //        Options.Values.Select(p => $"{p}").ToList(), selected => {
                
    //        }
    //    ); 
    //}
}