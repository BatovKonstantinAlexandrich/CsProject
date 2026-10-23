using System.ComponentModel;
using System.Drawing.Design;
using WinFormsApp.AppViews.FormModule;

namespace WinFormsApp.DesignerControls
{

    /// <summary>
    /// 
    /// </summary>
    public class DesignerInputPanelEditor: UITypeEditor
    {

        public override UITypeEditorEditStyle GetEditStyle(
           ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(
            ITypeDescriptorContext context,
            IServiceProvider provider,
            object value)
        {
            // Показываем свой интерфейс для редактирования
            Form form = null;
            var cloned = value.ToJsonUniversal().FromJsonUniversal();
            SubmitView submit = new SubmitView();
            var input = new FormInputView(cloned);
            input.OnValidationStateChanged += state => {
                submit.ValidationSuccess = state;
            };
            submit.SetView(input);
            submit.oncompleted += (s, e) => { 
                if(form is not null)
                {
                    form.Close();
                }
            };
            var ctrl = submit.ToControl();
            form = ctrl.ToBaseForm();
            if (form.ShowDialog() == DialogResult.OK)
                return cloned;
            return value;
        }

    }
}
