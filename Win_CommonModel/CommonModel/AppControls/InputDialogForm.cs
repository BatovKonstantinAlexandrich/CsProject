

using MaterialSkin.Controls;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WinFormsApp.AppComponents.AppForms;

namespace MaterialApp.Win_MaterialModule.DialogModule
{
    public partial class InputDialogForm : SubmitDialogControl
    {
        private readonly object _target;

        public InputDialogForm(string title, object target, Action<object> complete) : base(title)
        {
            this._target = target;
            InitializeComponent();
            this.SetContentPanel(new InputPanel<SigninModel>((success, model) =>{
            }));
            this.SetCompleteAction(success => {
                complete(success?target:null);
            });
        }

        private void InputDialogForm_Load(object sender, EventArgs e)
        {

        }
    }
}
