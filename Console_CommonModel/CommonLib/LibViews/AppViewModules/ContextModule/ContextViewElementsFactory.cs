using Org.BouncyCastle.Asn1.X509;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.ContextModule
{
  

    public class ContextViewElementsFactory : IContextViewElementsFactory
    {

        public ContextViewButton CreateButton(string Icon, string Label, Action onclick)
            => new ContextViewButton()
            {
                Icon = Icon,
                Label = Label,
                OnClick = (p, s) =>
                {
                    if (onclick is not null)
                        onclick();
                }
            };

        public ContextViewCheckbox CreateCheckbox(string Icon, string Label, bool Checked, Action<bool> onchanged)
            => new ContextViewCheckbox
            {
                Icon = Icon,
                Label = Label,
                IsChecked = Checked,
                OnCheckedChanged = p =>
                {
                    if (onchanged is not null)
                        onchanged(p);
                }
            };

        public ContextViewTextbox CreateTextbox(string Icon, string Label, string Value, Action<string> onchanged)
            => new ContextViewTextbox
            {
                Icon = Icon,
                Label = Label,
                Value = Value,
                OnChanged = p =>
                {
                    if (onchanged is not null)
                        onchanged(p);
                }
            };


        public ContextView CreateRedioGroup(string Icon, string Label, string value, object service, List<string> options, Action<string> onchanged)
        {
            ContextView res = new()
            {
                Icon = Icon,
                Label = Label
            };
            if (service is ChangeSupport)
            {
            }
            foreach (var item in options)
            {
                ContextViewCheckbox p = new()
                {
                    IsChecked = item == value,
                    Label = item,
                    OnCheckedChanged = p =>
                    {
                        if (onchanged is not null)
                            onchanged(item);
                        foreach (var next in res.Items)
                        {
                            ((ContextViewCheckbox)next).IsChecked = next.Label == value;
                        }
                    }
                };
                res.Items.Add(p);
            }
            return res;
        }


    }
}
