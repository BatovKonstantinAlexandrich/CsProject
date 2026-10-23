using Console_CommonModel;

using WinFormsApp.AppComponents.AppForms;

using WinFormsDesignerApp;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Функции для интеграции XML-представлений
    /// </summary>
    public partial class ReportControl : FileControl
    {
        public ReportControl()
        {
            //MinimumSize = new(640, 480);
        }


        /// <summary>
        /// Выполнить XML документ из файла
        /// </summary>
        /// <param name="filename">имя файла</param>        
        public static ReportControl Open(Control control, string filename)
        {
            var form = new ReportControl();
            control.Size = new(1200, 968);

            form.OnFileOpen(control, filename);
            return form;
        }



        /// <summary>
        /// Обновление представления
        /// </summary>
        /// <param name="document"></param>
        public void UpdateView(Control control, ReportComponent document)
        {
            this.control = control;
            control.Controls.Clear();
            if (document is null)
                return;
            var designerForm = new Form() { Dock = DockStyle.Fill };
            control.Controls.Add(designerForm);

            designerForm.Controls.Clear();
            var body = document.GetBody();
            foreach (var kv in body.HierElements)
            {
                try
                {
                    this.Info(kv.Key);
                    var view = (AppViewBase)kv.Value.Item.Target;
                    designerForm.Controls.Add(view.ToControl());
                }
                catch (Exception ex)
                {
                    this.Error(ex);
                }
            }
        }

    }
}
