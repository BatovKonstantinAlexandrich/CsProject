using MaterialSkin.Controls;

using WinFormsApp.AppComponents.AppControls;

namespace WinFormsApp.SharedViewCollections.CheckedListModule
{
    public partial class CheckedListControl : ControlBase
    {
        public readonly MaterialLabel titleText;

        /// <summary>Смена состояния пункта: индекс, <see cref="MaterialCheckbox.Tag"/>, отмечен.</summary>
        [NotInput]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Action<int, object, bool> OnItemCheckedChanged { get; set; } = (_, _, _) => { };

        public CheckedListControl()
        {                 
            InitializeComponent();
            this.Top = this.titleText = this.CreateTitleText("");
        }

        public MaterialCheckbox AddItem(object tag, string text, bool isChecked = false)
        {
            var checkbox = new MaterialCheckbox
            {
                Text = text,
                Tag = tag,
                Checked = isChecked
            };
            WireItemChecked(checkbox);
            checkedListBox.Items.Add(checkbox);
            return checkbox;
        }

        private void WireItemChecked(MaterialCheckbox checkbox)
        {
            checkbox.CheckedChanged += (_, _) =>
            {
                var index = checkedListBox.Items.IndexOf(checkbox);
                if (index < 0)
                    return;
                OnItemCheckedChanged(index, checkbox.Tag, checkbox.Checked);
            };
        }

        public List<object> GetCheckedTags()
            => checkedListBox.Items
                .OfType<MaterialCheckbox>()
                .Where(x => x.Checked)
                .Select(x => x.Tag)
                .ToList();
    }
}  