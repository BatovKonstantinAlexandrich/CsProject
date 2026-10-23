using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IButtonView: IControlView, IViewEventSupport
{
    public string Icon { get; set; }
    public bool IconOrText { get; set; }
    public Action<object, EventArgs> OnClick { get; set; }
}