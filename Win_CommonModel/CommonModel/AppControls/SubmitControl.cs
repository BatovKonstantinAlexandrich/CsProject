using Console_CommonModel.CommonLib.LibViews.AppViewModules.LayoutModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.Others;

using MaterialSkin.Controls;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp.AppComponents.AppControls
{

    public class SubmitComponent: ComponentBase<SubmitView,SubmitControl>
    {
        public SubmitComponent([NotNullNotEmpty] SubmitView ViewModel, [NotNullNotEmpty] SubmitControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
           
        }
    }

    /// <summary>
    /// Панель для подтверждения или отмены операции
    /// </summary>
    [Label("Панель для подтверждения выполнения операции пользователем")]
    public partial class SubmitControl : ControlBase, IPanelsControl, ILayoutControl
    {
        public Action<bool> OnSubmitCompleted { get; set; } = delegate { };

        public bool ShowButtons { get => Get<bool>(nameof(ShowButtons)); set => Set<bool>(nameof(ShowButtons), value); }
        public bool ValidationSuccess { get => Get<bool>(nameof(ValidationSuccess)); set => Set<bool>(nameof(ValidationSuccess), value); }



        /// <summary>
        /// Сюда нужно размещать контент
        /// </summary>
        public PanelsControl panelsControl { get; }

        public SubmitControl()
        {
            this.InitializeComponent();
            this.ShowButtons = true;
            this.AddPropertyChangeListener(nameof(ShowButtons), evt => { if ((bool)evt.After) ButtonsVisible(); else ButtonsHide(); });
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.MinimumSize = new(320, 280);
            this.ValidationSuccess = false;
            this.AddPropertyChangeListener(nameof(ValidationSuccess), OnValidationStateChanged);
            this.contentPane.LayoutFill(this.panelsControl = new PanelsControl() { Margin = new(10) });
        }


        public void ButtonsVisible()
        {
            this.materialButton1.Visible = true;
            this.cancelButton.Visible = true;
        }
        public void ButtonsHide()
        {
            this.materialButton1.Visible = false;
            this.cancelButton.Visible = false;
        }



        private void OnValidationStateChanged(PropertyChangedMessage message)
        {
            var is_valid = (bool)message.After;
            this.materialButton1.Enabled = is_valid;
        }

        internal void SetSubmitContent(Control content)
        {
            panelsControl.ContentView.LayoutFill(content);
        }

        private void onSubmitButtonClicked(object sender, EventArgs e)
        {
            OnSubmitCompleted(true);
        }

        private void onCancelButtoinClicked(object sender, EventArgs e)
        {
            OnSubmitCompleted(false);
        }



        IControlBase IPanelsControl.Bottom { get => ((IPanelsControl)panelsControl).Bottom; set => ((IPanelsControl)panelsControl).Bottom = value; }
        IControlBase IPanelsControl.Left { get => ((IPanelsControl)panelsControl).Left; set => ((IPanelsControl)panelsControl).Left = value; }
        IControlBase IPanelsControl.Right { get => ((IPanelsControl)panelsControl).Right; set => ((IPanelsControl)panelsControl).Right = value; }
        IControlBase IPanelsControl.Top { get => ((IPanelsControl)panelsControl).Top; set => ((IPanelsControl)panelsControl).Top = value; }

    
        public void SetLeft(IControlBase leftButton) => ((IPanelsControl)panelsControl).SetLeft(leftButton);
        public void SetRight(IControlBase leftButton) => ((IPanelsControl)panelsControl).SetRight(leftButton);
        public void SetTop(IControlBase leftButton) => ((IPanelsControl)panelsControl).SetTop(leftButton);
        public void SetBottom(IControlBase leftButton) => ((IPanelsControl)panelsControl).SetBottom(leftButton);

        public void Add(Control item)
        {
            this.panelsControl.Add(item);
        }

        public void Remove(Control item)
        {
            this.panelsControl.Add(item);
        }

        public void Add(IControlBase item)
        {
            ((ILayoutControl)panelsControl).Add(item);
        }

        public void Remove(IControlBase item)
        {
            ((ILayoutControl)panelsControl).Remove(item);
        }
    }
}
