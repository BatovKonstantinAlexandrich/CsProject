using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent
{
    public interface ITreeContainer<T> 
        where T : ITreeContainer<T>
    {

        [Parameter]
        public T Parent { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotInput]
        public ChangingList<T> Controls { get; set; }
    }
}
