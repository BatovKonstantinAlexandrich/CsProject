using WinFormsApp.AppViews.InputModule;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Связывает <see cref="InputFilePathView"/> с <see cref="InputFilePathControl"/>.
    /// </summary>
    public class InputFilePathComponent : ComponentBase<InputFilePathView, InputFilePathControl>
    {
        public InputFilePathComponent(InputFilePathView viewModel, InputFilePathControl control)
            : base(viewModel, control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();

            ctrl.Filter = string.IsNullOrWhiteSpace(view.Filter)
                ? "All files (*.*)|*.*"
                : view.Filter;
            ctrl.DialogTitle = string.IsNullOrWhiteSpace(view.DialogTitle)
                ? "Выберите файл"
                : view.DialogTitle;

            ctrl.SetPathValueWithoutNotify(view.Path ?? "");

            ctrl.PathValueChanged += (_, _) =>
            {
                var path = ctrl.PathValue;
                if (view.Path != path)
                    view.Path = path;
            };

            view.AddPropertyChangeListener(nameof(InputFilePathView.Path), evt =>
            {
                var next = evt.After?.ToString() ?? "";
                if (ctrl.PathValue != next)
                    ctrl.SetPathValueWithoutNotify(next);
            });

            view.AddPropertyChangeListener(nameof(InputFilePathView.Filter), evt =>
            {
                ctrl.Filter = string.IsNullOrWhiteSpace(evt.After?.ToString())
                    ? "All files (*.*)|*.*"
                    : evt.After.ToString();
            });

            view.AddPropertyChangeListener(nameof(InputFilePathView.DialogTitle), evt =>
            {
                ctrl.DialogTitle = string.IsNullOrWhiteSpace(evt.After?.ToString())
                    ? "Выберите файл"
                    : evt.After.ToString();
            });
        }
    }
}
