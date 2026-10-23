using Google.Protobuf.WellKnownTypes;

using MaterialSkin.Controls;

using Org.BouncyCastle.Asn1.X509;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViews.BaseComponent;
using WinFormsApp.AppViews.FlexModule;

namespace WinFormsApp.AppForm.FormView
{



    public class InputFormFieldComponent : ComponentBase<InputFormField, InputFormFieldControl>
    {
        public InputFormFieldComponent([NotNullNotEmpty] InputFormField ViewModel, [NotNullNotEmpty] InputFormFieldControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            this.GetControl().BindInputFormField(this.GetViewModel());
        }
    }
    public partial class InputFormFieldControl : ControlBase
    {
        public Label errorsLabel;
        public Label fieldLabel;
        public Panel panel1;
        private InputFormFieldService inputFormFieldService = new();
        private int _lastAppliedTotalHeight;
        private bool _heightSyncPending;
        private int _heightSyncPasses;
        private InputFormField _boundFieldForHeightSync;
        public InputFormFieldControl()
        {
            this.Dock = DockStyle.Top;                        
        }

        private void OnResized(object sender, EventArgs e)
        {
             
        }

        public void BindInputFormField(InputFormField inputFormField)
        {
            this.Controls.Clear();
            this.BorderStyle = BorderStyle.Fixed3D;

            //this.LayoutFill(this.panel1 = new Panel());
            this.LayoutBottom(this.errorsLabel = new Label());
           
            this.LayoutTop(this.fieldLabel = new Label() { Text = new List<string>(){ "bool",
             "boolean",
             "logic",
             "checkbox"}.Contains(inputFormField.Type.ToLower())?"":inputFormField.Label });
            
                this.BorderStyle = BorderStyle.FixedSingle;
            this.fieldLabel.Text = new List<string>(){ "bool",
             "boolean",
             "logic",
             "checkbox"}.Contains(inputFormField.Type.ToLower()) ? "" : inputFormField.Label;
            inputFormField.AddPropertyChangeListener(nameof(Label), evt => this.fieldLabel.Text = new List<string>(){ "bool",
             "boolean",
             "logic",
             "checkbox"}.Contains(inputFormField.Type.ToLower()) ? "" : evt.After.ToString());
            this.errorsLabel.Text = inputFormField.GetErrorMessage();
            inputFormField.AddPropertyChangeListener(nameof(inputFormField.Errors), evt => this.errorsLabel.Text = inputFormField.GetErrorMessage());
            this.errorsLabel.ForeColor = Color.Red;
            this.errorsLabel.Text = " ";
            this.errorsLabel.Dock = DockStyle.Bottom;
            //this.Controls.Add(this.errorsLabel);
            inputFormField.Errors.OnChanged += (s, r) => {
                bool resize = false;
                if(this.errorsLabel.Height == 0)
                    resize = true;
                this.errorsLabel.Text = inputFormField.GetErrorMessage();
                if(resize)
                    this.Height += this.errorsLabel.Height;
            };
            //if(inputFormField.Control is null)
            //{
                InitFormField(inputFormField);
            //}
            if (inputFormField.Control is not null)
            {
                //this.panel1.Controls.Clear();
                inputFormField.Control.Parent = inputFormField;
                var c = ((AppViewBase)inputFormField.Control).ToControl();
                this.LayoutTop(this.errorsLabel);
                this.LayoutTop(c);
                this.LayoutTop(this.fieldLabel);       
                this.Padding = new(2);
                AdjustHeightToContent(inputFormField, c);
                SubscribeContentHeightSync(inputFormField, c);
            }            
            inputFormField.control = this;            
        }

        private void SubscribeContentHeightSync(InputFormField inputFormField, Control content)
        {
            _boundFieldForHeightSync = inputFormField;

            void scheduleSync()
            {
                if (_heightSyncPending || IsDisposed)
                    return;
                _heightSyncPending = true;
                if (IsHandleCreated)
                    BeginInvoke(PerformScheduledHeightSync);
                else
                    HandleCreated += OnFirstHandleCreatedForHeightSync;
            }

            content.HandleCreated += (_, __) => scheduleSync();
            content.Resize += (_, __) => scheduleSync();
            content.ControlAdded += (_, __) => scheduleSync();
            content.ControlRemoved += (_, __) => scheduleSync();
        }

        private void OnFirstHandleCreatedForHeightSync(object sender, EventArgs e)
        {
            HandleCreated -= OnFirstHandleCreatedForHeightSync;
            PerformScheduledHeightSync();
        }

        private void PerformScheduledHeightSync()
        {
            _heightSyncPending = false;
            if (IsDisposed || _boundFieldForHeightSync is null)
                return;
            if (++_heightSyncPasses > 24)
                return;
            SyncContentHeight(_boundFieldForHeightSync);
        }

        public void SyncContentHeight(InputFormField inputFormField)
        {
            Control content = null;
            foreach (Control child in this.Controls)
            {
                if (child != fieldLabel && child != errorsLabel)
                {
                    content = child;
                    break;
                }
            }
            if (content is not null)
                AdjustHeightToContent(inputFormField, content);
        }

        private static int MeasureControlHeight(Control control)
        {
            if (control is null || !control.Visible)
                return 0;

            var height = Math.Max(control.Height, control.MinimumSize.Height);

            if (control.Controls.Count > 0)
            {
                var maxBottom = 0;
                foreach (Control child in control.Controls)
                {
                    if (!child.Visible)
                        continue;
                    maxBottom = Math.Max(maxBottom, child.Bottom + child.Margin.Bottom);
                }
                if (maxBottom > 0)
                    height = Math.Max(height, maxBottom + control.Padding.Vertical);
            }

            try
            {
                var width = control.ClientSize.Width > 0 ? control.ClientSize.Width : control.Width;
                if (width <= 0)
                    width = 400;
                height = Math.Max(height, control.GetPreferredSize(new Size(width, 0)).Height);
            }
            catch
            {
            }

            return height;
        }

        private void AdjustHeightToContent(InputFormField inputFormField, Control content)
        {
            var contentHeight = Math.Max(
                inputFormField.Control?.MinHeight ?? 0,
                MeasureControlHeight(content));

            if (contentHeight < 55)
                contentHeight = 55;

            content.Dock = DockStyle.Top;
            if (content.Height != contentHeight)
                content.Height = contentHeight;

            var labelHeight = fieldLabel?.PreferredHeight ?? fieldLabel?.Height ?? 20;
            var errorsHeight = string.IsNullOrWhiteSpace(errorsLabel?.Text?.Trim())
                ? 4
                : Math.Max(errorsLabel?.PreferredHeight ?? 0, errorsLabel?.Height ?? 0);

            var total = labelHeight + contentHeight + errorsHeight + Padding.Vertical + 4;
            if (total == _lastAppliedTotalHeight && Height == total)
                return;

            _lastAppliedTotalHeight = total;
            Height = total;
            MinimumSize = new Size(Math.Max(MinimumSize.Width, 120), total);

            inputFormField.Height = total;
            inputFormField.MinHeight = total;
        }

        private void InitFormField(InputFormField inputFormField)
        {
            try
            {
                var factory = new FormFieldFactory();
                inputFormField.Control = this.inputFormFieldService.CreateControl(inputFormField);
            }
            catch (Exception ex)
            {
                this.Error($"Исключение при формировании элементов управления свойством {inputFormField.Name}: {ex.Message}");
            }
        }
    }
}
