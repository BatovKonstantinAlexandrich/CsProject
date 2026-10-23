using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

namespace Console_CommonModel.CommonLib.LibDesigner.DesignerServices
{
    /// <summary>
    /// Управление формой ввода:
    /// 1) выбор свойств объекта
    /// 1) переключение режима редактирования/отображение
    /// </summary>
    [Label("Параметры ввода")]
    public class EditViewDesignerService : DesignerBase, IControlService<EditView>
    {
        public EditView Control { get; set; }

        public void SetControl(EditView item)
        {
            this.Control = item;
        }

        protected override void Activating(ViewNode view)
        {
            using (var db = new DeliveryDbContext())
            {
                var procedureActions
                    = db.GetProceduresMetadata().Values.Select(p => p.FullName).ToDictionary<string,Action>(name => () => { });
                this.rollback.Add(Control.ContextMenu.AddContextMethods("Процедуры", procedureActions));
                var tableActions = db.GetEntitiesTypes().Select(p => p.GetTypeName()).ToDictionary<string,Action>(name => () => {
                    Control.SetTargetValue(name.ToType().New(), (success, target) => { });
                });                
                this.rollback.Add(Control.ContextMenu.AddContextMethods("Сущности", tableActions));
            }
            
        }
    }
}