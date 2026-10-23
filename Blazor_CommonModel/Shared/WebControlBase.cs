using Blazor_CommonModel.Shared;

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ViewControl;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
using Console_CommonModel.CommonLib.Others;

using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Blazor_CommonModel.Components.Shared
{

    


    public abstract class WebControlBase<TViewModel, TViewControl> : BaseComponent, 
          IComponentBase<TViewModel, TViewControl>
        where TViewModel : AppViewBase
        where TViewControl : IControlBase
    {

        protected IComponentBase<TViewModel, TViewControl>  componentBase { get; set; }


    




        [Parameter]
        public override AppViewBase ViewModel { get; set; }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TControl"></typeparam>
        /// <param name="view"></param>
        /// <returns></returns>
        public Type GetWebControlType<TControl>(AppViewBase view)
            where TControl : IControlBase
        {
            
             
            var viewType = view.GetType();
            var controlType = typeof(TControl);
            var componentType = typeof(WebControlBase<,>).MakeGenericType(new Type[] { viewType, controlType });

            return ServiceFactory.Get().GetTypesExtended(componentType).FirstOrDefault();
        }

         
        public virtual void BindComponent()
        {
            try
            {                  
                if (ViewModel is null)
                    return;
                var genericArguments = this.GetType().BaseType.GetGenericArguments();


                var type = GetWebControlType<TViewControl>(ViewModel);
                string name = "I" + ViewModel.GetTypeName().Replace("View","Control");
                var controlInterface = name.ToType();

                var componentBaseType = typeof(IComponentBase<,>).MakeGenericType(new Type[] { genericArguments[0], controlInterface });
                var types = ServiceFactory.Get().GetTypesImplements(componentBaseType);
                var componentType = types.Where(t => t != type && !t.IsGenericType).FirstOrDefault();

                this.Info(this.GetTypeName() + " =-> " + componentType.GetTypeName());
                if (componentType is not null)
                {
                    var ctrl = this.Cast<TViewControl>();
                    var comp = componentType.New(new object[] { ViewModel, ctrl });
                    this.Debug(comp.GetTypeName());

                }
                else
                {
                    throw new Exception($"Не найден компонент связывания реализующий {typeof(IComponentBase<TViewModel, TViewControl>).GetTypeName()}");
                }
                
            }
            catch (Exception ex)
            {
                this.Error(ex.Message);
                throw;
            }
        }
         
        /// <summary>
        /// 
        /// </summary>        
        public IComponentBase<TViewModel, TViewControl> TryBindComponent(TViewModel viewModel)
        {
            try
            {
                var types = ServiceFactory.Get().GetTypesImplements<IComponentBase<TViewModel, TViewControl>>();
                var componentType = types.Where(t => t != typeof(TViewControl)).FirstOrDefault();

                this.Info(componentType.GetTypeName() + $" выбран для связывания {typeof(IComponentBase<TViewModel, TViewControl>).GetTypeName()}");
                if (componentType is not null)
                {
                    TViewControl viewControl = this.Cast<TViewControl>();
                    return componentType.New<IComponentBase<TViewModel, TViewControl>>(new object[] { viewModel, viewControl });
                }
                else
                {
                    throw new Exception($"Не удалось найти подходящий компонент для связывания {typeof(IComponentBase<TViewModel, TViewControl>).GetTypeName()}");
                }
            }
            catch (Exception ex)
            {
                this.Error($"");
                return null;
            }
        }

        public ChangingList<DesignerBase> viewServices { get => componentBase.viewServices; set => componentBase.viewServices = value; }




        public void Bind()
        {
            componentBase.Bind();
        }

        public void BindContextMenu()
        {
            componentBase.BindContextMenu();
        }

        public void BindControlEvents()
        {
            componentBase.BindControlEvents();
        }

        public void BindSides(PanelsView view, IControlBase side)
        {
            componentBase.BindSides(view, side);
        }

        public TViewControl GetControl()
        {
            return componentBase.GetControl();
        }

        public TViewModel GetViewModel()
        {
            return componentBase.GetViewModel();
        }

        public void InitViewServices()
        {
            componentBase.InitViewServices();
        }
    }
}
