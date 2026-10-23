using Console_CommonModel.CommonLib.Others;


namespace WinFormsApp.AppComponents.AppControls
{
    public interface IComboboxControl : IControlBase, IComboboxView
    {
        void SetSelectedIndex(int selectedIndex);
    }

    /// <summary>
    /// Связывает представление выпадающего списка с элементов управления.    
    /// </summary>
    public class ComboboxComponent : ComponentBase<ComboboxView, IComboboxControl>
    {
        public ComboboxComponent([NotNullNotEmpty] ComboboxView ViewModel, [NotNullNotEmpty] IComboboxControl Control) : base(ViewModel, Control)
        {

        }

        public override void Bind()
        {
            var comboboxView = GetViewModel();
            var comboboxControl = GetControl();

            comboboxControl.Text = comboboxView.Text;
            comboboxView.AddPropertyChangeListener(nameof(comboboxView.Text), (Action<PropertyChangedMessage>)(evt =>
            {
                comboboxControl.Text = (evt.After.ToString());
            }));

            
            comboboxView.AddPropertyChangeListener(nameof(comboboxView.Value), evt =>
            {
                try
                {
                    var selectedIndex = comboboxView.Options.IndexOf(comboboxView.Value);
                    comboboxControl.SetSelectedIndex(selectedIndex);
                }
                catch
                {
                }
            });


            foreach (var option in comboboxView.Options)
            {
                comboboxControl.Options.Add($"{option}");
            }
            if (comboboxView.Value is not null)
            {
                try
                {


                    var selectedIndex = comboboxView.Options.IndexOf(comboboxView.Value);
                    comboboxControl.SetSelectedIndex(selectedIndex);
                }
                catch
                {
                }
            }
            comboboxView.Options.OnChanged += (s, e) =>
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
