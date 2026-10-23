using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.LayoutVIews
{
    /// <summary>
    /// Выполняет связывание источника данных
    /// </summary>
    public class DatasourceLayoutService : LayoutViewService<AppViewBase, AppViewBase>
    {
        public override Action Activate(AppViewBase viewBase)
        {
            viewBase.DatasourceId = Control.DatasourceId;
            Action<PropertyChangedMessage> todo = evt =>
            {
                viewBase.DatasourceId = evt.After?.ToString().ToInt();
            };
            var remove = Control.AddPropertyChangeListener(nameof(Control.DatasourceId), todo);
            remove += CreateDatasourceDesignServices(viewBase);
            return remove;
        }

        public Action CreateDatasourceDesignServices(AppViewBase viewBase)
        {
            Action<PropertyChangedMessage> todo = evt => {

                Action rollback = null;
                if(viewBase.DesignMode)
                {
                    rollback = viewBase.AddContextMenuItems(
                        viewBase.GetProceduresMetadata().Keys.ToList(), 
                        procedureName => {
                        });
                }
                else
                {
                    if (rollback is not null)
                        rollback();
                    
                }
                

            };
            var remove = viewBase.AddPropertyChangeListener(nameof(viewBase.DesignMode), todo);            
            return () => {
                remove();
            };
        }
    }
}
