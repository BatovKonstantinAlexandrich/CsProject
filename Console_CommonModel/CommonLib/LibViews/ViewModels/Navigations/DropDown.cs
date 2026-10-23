using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Icon("apps")]
[Label("Выпадающий список")]
public class DropDown: LinkViewModel
{
    
    [Label("Links")]
    public List<ViewNode> Links { get; set; }
   
    public DropDown() : base( )
    {
        Label = "Выпадающий список";
        MinWidth = 160;
        MinHeight = 40;
        Width = MinWidth;
        Height = MinHeight;
        Changed = false;
    }

    public DropDown(string name, ILink item, TypeNode<ILink> parent) : base()
    {
        Label = name;
        Item = item;
        Changed = false;
    }
}

