
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

[Label("Бизнес-процесс")]
public class BusinessProcess: BusinessEntity<BusinessProcess>
{
    [Label("Вход")]
    [InputDictionary("MessageProtocol" + ",Name")]
    [NotMapped]
    public virtual int InputID { get; set; }

    [NotInput]
    [NotMapped]
    public virtual MessageProtocol Input { get; set; }

    [Label("Ошибка")]
    [InputDictionary("MessageProtocol" + ",Name")]
    [NotMapped]
    public virtual int ErrorID { get; set; }

    [NotInput]
    [NotMapped]
    public virtual MessageProtocol Error { get; set; }

    [Label("Выход")]
    [NotMapped]
    [InputDictionary(nameof(MessageProtocol)+",Name")]
    public virtual int OutputID { get; set; }


    [NotMapped]
    [NotInput]
    public virtual MessageProtocol Output { get; set; }


    [Label("Бизнес функции")]
    [InputStructureCollection<BusinessFunction>()]
    public virtual ChangingList<BusinessFunction> BusinessFunctions { get; set; } = new();

    public BusinessProcess()
    {
        this.BusinessFunctions.OnAdded(this.OnBusinessFunctionAdded);
        this.BusinessFunctions.OnRemoved(this.OnBusinessFunctionRemoved);
    }

    private void OnBusinessFunctionRemoved(int i, object p)
    {
        throw new NotImplementedException();
    }

    private void OnBusinessFunctionAdded(int i, object p)
    {
        if(p is BusinessFunction bf)
        {
            bf.Label = bf.Name;
            if (bf.BusinessResourceId is null)
                throw new InvalidOperationException();
        }
    }
}
 