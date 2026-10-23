using System.ComponentModel;
using System.Drawing.Design;

using WinFormsApp.AppComponents;
using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.DesignerControls
{
    /// <summary>
    /// Редактор типа для свойства AppViewBase в ViewContainerControl
    /// Предоставляет двухэтапный диалог для выбора и настройки типа AppViewBase
    /// </summary>
    public class AppViewBaseTypeSelectorEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(
            ITypeDescriptorContext context,
            IServiceProvider provider,
            object value)
        {
            using (var dialog = new AppViewBaseEditorDialog(value as AppViewBase))
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.SelectedView;
                }
            }
            return value;
        }
    }
}

