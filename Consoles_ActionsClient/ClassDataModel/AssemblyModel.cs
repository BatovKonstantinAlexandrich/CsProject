using ActionModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class AssemblyModel: HierTable<AssemblyModel>
{
    public override string Name { get => base.Name; set => base.Name = value; }
    public override string Label { get; set; }
    public override string Description { get; set; }
    public int Version { get; set; }
    public string Platform { get; set; }
    public byte[] Dll { get; set; }
}

