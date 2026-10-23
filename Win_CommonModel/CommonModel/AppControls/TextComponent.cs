using WinFormsApp.AppViews.TextModule;

namespace WinFormsApp.AppComponents.AppControls
{
    public class TextComponent : ComponentBase<TextView, TextControl>
    {
        public TextComponent([NotNullNotEmpty] TextView ViewModel, [NotNullNotEmpty] TextControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();

            void ApplyAll()
            {
                ctrl.SetLabelText(view.Text);
                ctrl.ApplyTextAlign(view.TextAlign);
                ctrl.ApplyFontSize(view.FontSize);
                ctrl.ApplyForeColor(view.Color);
                ctrl.ApplyTextIndent(view.TextIndent);
                ctrl.SyncLabelBounds(view.Width, view.Height);
            }

            ApplyAll();

            ctrl.Click += (s, e) => view.OnClick(s, e);

            view.AddPropertyChangeListener(nameof(TextView.Text), _ => ctrl.SetLabelText(view.Text));
            view.AddPropertyChangeListener(nameof(TextView.TextAlign), _ => ctrl.ApplyTextAlign(view.TextAlign));
            view.AddPropertyChangeListener("TextAlignment", _ => ctrl.ApplyTextAlign(view.TextAlign));
            view.AddPropertyChangeListener(nameof(TextView.FontSize), _ => ctrl.ApplyFontSize(view.FontSize));
            view.AddPropertyChangeListener(nameof(TextView.Color), _ => ctrl.ApplyForeColor(view.Color));
            view.AddPropertyChangeListener(nameof(TextView.TextIndent), _ => ctrl.ApplyTextIndent(view.TextIndent));
            view.AddPropertyChangeListener(nameof(TextView.Width), _ => ctrl.SyncLabelBounds(view.Width, view.Height));
            view.AddPropertyChangeListener(nameof(TextView.Height), _ => ctrl.SyncLabelBounds(view.Width, view.Height));

            ctrl.Resize += (_, _) =>
            {
                if (labelUsesFillWidth(view.TextAlign))
                    ctrl.ApplyTextAlign(view.TextAlign);
            };
        }

        static bool labelUsesFillWidth(string align)
        {
            var n = (align ?? "left").Trim().ToLowerInvariant();
            return n is "center" or "right";
        }
    }
}
