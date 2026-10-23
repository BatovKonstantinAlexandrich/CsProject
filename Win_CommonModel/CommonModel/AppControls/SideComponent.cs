using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.AppViews.BaseComponent;

using WinFormsApp.AppViews.SideModule;

namespace WinFormsApp.AppComponents.AppControls
{
    public class SideComponent : ComponentBase<AppViewBase, ContentPanel>
    {
        public SideComponent([NotNullNotEmpty] AppViewBase ViewModel, [NotNullNotEmpty] ContentPanel Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            /*var view = GetViewModel();
            var ctrl = GetControl();
            var factory = new ComponentsFactory();
            if(view.Left is not null)
            {
                var leftCtrl = factory.CreateControl(view.Left);
                ctrl.SetLeft(leftCtrl);
            }
            if (view.Right is not null)
            {
                var leftCtrl = factory.CreateControl(view.Right);
                ctrl.SetRight(leftCtrl);
            }
            if (view.Top is not null)
            {
                var leftCtrl = factory.CreateControl(view.Top);
                ctrl.SetTop(leftCtrl);
            }
            if (view.Bottom is not null)
            {
                var leftCtrl = factory.CreateControl(view.Bottom);
                ctrl.SetBottom(leftCtrl);
            }*/



            ViewModel.OnChange += OnViewModelPropertyChanged;
            /*ViewModel.AddPropertyChangeListener(nameof(AppViewBase.Left), evt => {
                if (evt.After is null)
                {
                    ((SideControl)(this.Control)).SetLeft(null);
                }   
                else
                {
                    var ctrl = this.factory.CreateControl((ViewItem)evt.After);
                    ((SideControl)(this.Control)).SetLeft(ctrl);
                }
            });
            ViewModel.AddPropertyChangeListener(nameof(AppViewBase.Right), evt => {
                if (evt.After is null)
                {
                    ((SideControl)(this.Control)).SetRight(null);
                }
                else
                {
                    var ctrl = this.factory.CreateControl((ViewItem)evt.After);
                    ((SideControl)(this.Control)).SetRight(ctrl);
                }
            });
            ViewModel.AddPropertyChangeListener(nameof(AppViewBase.Top), evt => {
                if (evt.After is null)
                {
                    ((SideControl)(this.Control)).SetTop(null);
                }
                else
                {
                    var ctrl = this.factory.CreateControl((ViewItem)evt.After);
                    ((SideControl)(this.Control)).SetTop(ctrl);
                }
            });
            ViewModel.AddPropertyChangeListener(nameof(AppViewBase.Bottom), evt => {
                if (evt.After is null)
                {
                    ((SideControl)(this.Control)).SetLeft(null);
                }
                else
                {
                    var ctrl = this.factory.CreateControl((ViewItem)evt.After);
                    ((SideControl)(this.Control)).SetLeft(ctrl);
                }
            });
            ViewModel.AddPropertyChangeListener(nameof(AppViewBase.View), evt => {
                if(evt.After is null)
                {
                    ((SideControl)(this.Control)).SetView(null);
                }
                else
                {
                    var ctrl = this.factory.CreateControl((ViewItem)evt.After);
                    ((SideControl)(this.Control)).SetView(ctrl);
                }                
            });*/
        }

        public void OnViewModelPropertyChanged(PropertyChangedMessage evt)
        {
            var ctrl = GetControl();
            var factory = new ComponentsFactory();

            switch (evt.Property)
            {
                case nameof(AppViewBase.Left):
                    if (evt.After is not null)
                    {
                        ctrl.SetLeft(factory.CreateControl((ViewItem)evt.After));
                    }
                    break;
                case nameof(AppViewBase.Right):
                    if (evt.After is not null)
                    {
                        ctrl.SetRight(factory.CreateControl((ViewItem)evt.After));
                    }
                    break;
                case nameof(AppViewBase.Top):
                    if (evt.After is not null)
                    {
                        ctrl.SetTop(factory.CreateControl((ViewItem)evt.After));
                    }
                    break;
                case nameof(AppViewBase.Bottom):
                    if (evt.After is not null)
                    {
                        ctrl.SetBottom(factory.CreateControl((ViewItem)evt.After));
                    }
                    break;
            }
        }
    }
}
