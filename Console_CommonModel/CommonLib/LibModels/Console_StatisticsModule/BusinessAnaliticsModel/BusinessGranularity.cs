using System;
using System.Collections.Generic;
using System.Text;


[Label("Паралельность(периодичность)")]

public class BusinessGranularities : DictionaryTable 
{

    [Label("код")]
    [UniqValue()]
    public string Code { get; set; }

}