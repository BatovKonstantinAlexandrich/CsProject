using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RootForms.Shared.Collections.Tree
{
    public partial class TreeView 
    {
        [Parameter]
        public string Item { get; set; }


        [Parameter]
        public IEnumerable<TreeView> Children { get; set; }
    }
}
