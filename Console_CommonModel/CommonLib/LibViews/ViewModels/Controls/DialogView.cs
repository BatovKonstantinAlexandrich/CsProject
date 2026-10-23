using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Label("Мастер диалог")]
public class DialogView: AppView
{
    [Label("Step")]
    [InputNumber]
    public int Step { get; set; } = 0;


    
    [Label("Current View")]
    public TypeView CurrentView { get; set; } = new TypeView();


    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]

    public List<ViewItem> WizzardItems = new List<ViewItem>();

    public DialogView() : base()
    {        
    }

    public void SetNextDialog(ViewItem next) {
        WizzardItems.Add(next);
    }

    public void Start()
    {
        if (WizzardItems.Count() == 0)
        {
            throw new Exception("Элементы мастера не заданы");
        }
        Step = 1;
        CurrentView.Item = WizzardItems.ToArray()[Step];
    }


    public void Next()
    {         
        Step++;
        CurrentView.Item = WizzardItems.ToArray()[Step];
    }



    public void Complete()
    {
        Step++;        
    }

}