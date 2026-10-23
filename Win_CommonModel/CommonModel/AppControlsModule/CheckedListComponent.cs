using MaterialSkin.Controls;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.SharedViewCollections.CheckedListModule;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    public class CheckedListComponent : ComponentBase<CheckedListView, CheckedListControl>
    {
        public CheckedListComponent([NotNullNotEmpty] CheckedListView ViewModel, [NotNullNotEmpty] CheckedListControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();

            ctrl.titleText.Text = view.Text;
            view.AddPropertyChangeListener(nameof(view.Text), evt => ctrl.titleText.Text = evt.After.ToString());
            BindEnabled(view, ctrl);

            ctrl.checkedListBox.Items.Clear();
            ctrl.OnItemCheckedChanged = (index, item, isChecked) =>
            {
                view.ApplyItemChecked(index, item, isChecked);
            };
            var items = view.ListItems.ToList();
            items.Reverse();
            foreach (var item in items)
                AddItem(ctrl, view, item);

            view.ListItems.OnAdded((_, item) => AddItem(ctrl, view, item));
            view.ListItems.OnRemoved((_, item) => RemoveItem(ctrl, view, item));
        }

        private static void AddItem(CheckedListControl ctrl, CheckedListView view, object item)
        {
            var isChecked = view.CheckedItems.Contains(item);
            ctrl.AddItem(item, item?.ToString() ?? "", isChecked);
        }

        private static void RemoveItem(CheckedListControl ctrl, CheckedListView view, object item)
        {
            var checkbox = ctrl.checkedListBox.Items
                .OfType<MaterialCheckbox>()
                .FirstOrDefault(c => ReferenceEquals(c.Tag, item));
            if (checkbox is not null)
                ctrl.checkedListBox.Items.Remove(checkbox);
            view.CheckedItems.Remove(item);
        }

        public void BindEnabled(ControlView buttonView, ControlBase buttonViewControl)
        {
            buttonViewControl.Enabled = buttonView.Enabled;
            buttonView.AddPropertyChangeListener<bool>(nameof(buttonView.Enabled), val => buttonViewControl.Enabled = val);
        }
    }
}
