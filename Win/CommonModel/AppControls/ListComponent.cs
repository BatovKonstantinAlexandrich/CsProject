using MaterialSkin;
using MaterialSkin.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.AppViews.ApiServices;
using WinFormsApp.AppViews.ListModule;
using WinFormsApp.DesignerServices;
using WinFormsApp.SharedViewCollections.DataTable;

namespace WinFormsApp.AppComponents.AppControls
{
    public class ListComponent : ComponentBase<ListView, ListViewControl>
    {


        public ListComponent([NotNullNotEmpty] ListView ViewModel, [NotNullNotEmpty] ListViewControl Control) : base(ViewModel, Control)
        {
        }

        public override List<DesignerBase> GetViewServices()
        {
            var list = base.GetViewServices();
            list.Add(new ListViewDesigner(GetViewModel()));
            list.Add(new ListViewDesignerService(GetViewModel()));
            return list;
        }

        public override void Bind()
        {
            var view = GetViewModel();
            MaterialListBox list = GetControl().list;//CreateMaterialList(((ListView)ViewModel).Options.Select(o => o.Value).ToList(), ((ListView)ViewModel).OnSelected);
            var ctrl = GetControl();
            ctrl.titleText.Text = view.Text;
            view.AddPropertyChangeListener(nameof(view.Text), evt => ctrl.titleText.Text = $"{evt.After}");
            ((ListView)ViewModel).Options.Select(o => new MaterialListBoxItem()
            {
                Text = o.Value,
                SecondaryText = o.Value
            }).ToList().ForEach(list.Items.Add);
            ((ListView)ViewModel).Options.OnChanged += (s, e) =>
            {
                list.Items.Clear();
                ((ListView)ViewModel).Options.Select(o => new MaterialListBoxItem()
                {
                    Text = o.Value,
                    SecondaryText = o.Value
                }).ToList().ForEach(list.Items.Add);
            };
            list.MinimumSize = new(320, 280);
            list.Size = new(640, 480);
            list.SelectedIndexChanged += (s, e) =>
            {

                if (list.SelectedIndex >= 0)
                {
                    var selectedItem = view.Options[list.SelectedIndex];
                    view.OnSelected(selectedItem);
                }

            };
            //list.Dock = DockStyle.Fill;
        }

        public MaterialListBox CreateMaterialList(IList<string> options, Action<string> onselect)
        {
            var listBox = new MaterialListBox() { Dock = DockStyle.Fill };
            Dictionary<object, MaterialListBoxItem> map = new();
            options.ToList().ForEach(phone =>
            {
                var item = new MaterialListBoxItem();
                item.Tag = phone;
                item.Text = phone;
                map[item.Tag] = item;
                listBox.Items.Add(item);
            });
            string temp = null;
            listBox.MouseDown += (sender, evt) =>
            {
                string val = listBox.SelectedItem is not null ? listBox.SelectedItem.Text : null;
                if (val != temp)
                {
                    temp = val;
                    onselect(val);
                }

            };
            listBox.SelectedValueChanged += (sender, evt) =>
            {
                string val = listBox.SelectedItem is not null ? listBox.SelectedItem.Text : null;
                if (val != temp)
                {
                    temp = val;
                    onselect(val);
                }
            };
            return listBox;
        }
    }
}
