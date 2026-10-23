using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.ContextModule;

namespace Console_CommonModel.CommonLib.LibDesigner.DesignerServices
{

    /// <summary>
    /// Сервисы настройки базовых представлений
    /// </summary>
    [Label("Сервис стилизации представления")]
    public class StyledItemIntegratedControlService : IntegratedDesignControlService<StyledItem> { }


    [Label("Сервис связывания свойств представления")]
    public class ViewOptionsIntegratedControlService : IntegratedDesignControlService<ViewOptions> { }



    /// <summary>
    /// Встраиваемый сервис настройку управления разработкой
    /// </summary>
    /// <typeparam name="T">тип управление которым необходимо</typeparam>
    [Label("Сервис связывания внедрения проектировщика")]
    public class IntegratedDesignControlService<T> : DesignerBase, IControlService<T>
        where T : ViewOptions
    {
        public T Control { get; set; }
        public void SetControl(T item)
            => Control = item;
        protected override void Activating(ViewNode view)
        {
            var viewBase = view.Cast<AppViewBase>();

            var factory = new ContextViewElementsFactoryFIF();
            var inputProperties = typeof(T).GetOwnInputPropertyNames();
            var contextView = new ContextView()
            {
                Label = typeof(T).GetLabel()
            };
            foreach (var property in inputProperties)
            {
                var formInputField = view.GetInputFormField(property, textValue =>
                {
                    TextDataSetter.SetValue(view, property, textValue);
                });
                contextView.Items.Add(factory.CreateContextView(formInputField));
            }
            viewBase.ContextMenu.Items.Add(contextView);
            rollback.Add(() => viewBase.ContextMenu.Items.Remove(contextView));
        }
    }
}
