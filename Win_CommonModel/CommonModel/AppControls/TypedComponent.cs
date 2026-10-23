using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.TypedModule;
using Console_CommonModel.CommonLib.Others;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.AppComponents.AppControls;

using WinFormsDesignerApp;


[Label("Типизированный компонент")]
[Description("Компонент с типизированным представлением и контролом, обеспечивающий строгую типизацию и связь между моделью представления и элементом управления")]
public class TypedComponent:  ComponentBase<TypedView, TypedControlDefault>
{
    public TypedComponent([NotNullNotEmpty] TypedView ViewModel, [NotNullNotEmpty] TypedControlDefault Control) : base(ViewModel, Control)
    {
    }

    public override void Bind()
    {
        var view = this.GetViewModel();
        if(view.ViewModel is not null)
        {
            GetControl().LayoutFill((Control)view.ViewModel.ToControl());
        }
        view.AddPropertyChangeListener(nameof(view.ViewModel), (PropertyChangedMessage evt) =>
        {
            if(evt.After is null)
            {
                GetControl().LayoutFill(new Panel());                 
            }
            else
            {                
                GetControl().LayoutFill((Control)((AppViewBase)evt.After).ToControl());
            }
        });
    }
}
