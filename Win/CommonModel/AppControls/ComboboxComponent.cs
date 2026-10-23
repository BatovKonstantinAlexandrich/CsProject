using WinFormsApp.AppViewModules.ComboboxModule;

namespace WinFormsApp.AppComponents.AppControls
{

    /// <summary>
    /// Связывает представление выпадающего списка с элементов управления.    
    /// </summary>
    public class ComboboxComponent : ComponentBase<ComboboxView, ComboboxControl>
    {
        public ComboboxComponent([NotNullNotEmpty] ComboboxView ViewModel, [NotNullNotEmpty] ComboboxControl Control) : base(ViewModel, Control)
        {

        }

        public override void Bind()
        {
            var comboboxView = GetViewModel();
            var comboboxControl = GetControl();

            comboboxControl.SetText(comboboxView.Text);
            comboboxView.AddPropertyChangeListener(nameof(comboboxView.Text), evt =>
            {
                comboboxControl.SetText(evt.After.ToString());
            });

            
            comboboxView.AddPropertyChangeListener(nameof(comboboxView.Value), evt =>
            {
                try
                {
                    var selectedIndex = comboboxView.Items.IndexOf(comboboxView.Value);
                    comboboxControl.SetSelectedIndex(selectedIndex);
                }
                catch
                {
                }
            });


            foreach (var option in comboboxView.Items)
            {
                comboboxControl.Options.Add($"{option}");
            }
            if (comboboxView.Value is not null)
            {
                try
                {


                    var selectedIndex = comboboxView.Items.IndexOf(comboboxView.Value);
                    comboboxControl.SetSelectedIndex(selectedIndex);
                }
                catch
                {
                }
            }
            comboboxView.Items.OnChanged += (s, e) =>
            {
                switch (e.Target.Action)
                {
                    case "added": comboboxControl.Options.Add($"{e.Target.Item}"); break;
                    case "removed": comboboxControl.Options.Remove($"{e.Target.Item}"); break;
                }
            };

            comboboxControl.OnSelected += comboboxView.OnSelected;
        }
    }
}
