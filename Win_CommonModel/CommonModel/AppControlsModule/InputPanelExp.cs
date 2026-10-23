using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

using WinFormsApp.AppViews.InputModule;



namespace WinDesigner
{
    /// <summary>
    /// 
    /// </summary>
    public partial class InputPanelExp : Panel
    {
        public InputPanelExp(Type innputType, [NotNullNotEmpty]object target, Action<bool, object> oncompleted = null)
        {
            this.Commit(() => {
                if (!target.GetType().IsExtends(innputType))
                    throw new ArgumentException("");
                InitializeComponent();
                Panel panel = null;
                var factory = new ControlsFactory();
                this.input = factory.CreateInputForm(target, (s, e) => {
                    if (oncompleted is not null)
                    {
                        oncompleted(s, e);
                    }
                });
                this.materialCard1.LayoutFill(panel = new Panel());
                panel.AutoScroll = true;
                panel.LayoutTop(input);
                return panel;
            }, new
            {

            });            
        }

        public object target { get { return this.input.target;  } internal set { this.input.target = value;  } }
        public InputPanel input { get; private set; }

        private void InputPanelExp_Load(object sender, EventArgs e)
        {

        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMode"></typeparam>
    public class InputPanelExp<TMode> : InputPanelExp where TMode : class
    {
        public InputPanelExp(TMode model, Action<bool, TMode> oncompleted) : base(typeof(TMode), model, (s, e) => {
            TMode p = e is null? null: (TMode)e;
            if(oncompleted is not null)
            {
                oncompleted(s, p);
            }
        })
        {

        }
    }
}
