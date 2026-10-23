using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

namespace WinFormsApp
{

    /// <summary>
    /// Редактор бизнес модели
    /// </summary>
    [Label("Редактор бизнес-модели")]
    public class BusinessModelView : AppViewBase
    {
       
        public BusinessModelView()
        {
            this.MinHeight = 200;
            this.MinWidth = 200;
            this.LayoutFill(
                this.CreateEntityListEditor<BusinessProcess, DeliveryDbContext>("Бизнес процессы", OnBusinessProcessSelected)
            );
        }

        private void OnBusinessProcessSelected(BusinessProcess process)
        {
            this.LayoutFill(EditView.CreateEditViewBase(process, OnEditBusinessProcessCompleted));
        }

        private void OnEditBusinessProcessCompleted(bool success, object process)
        {
            
        }

        
    }
}