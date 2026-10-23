using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.Others;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.ComboboxModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.InputModule
{
    [Label("Коллекция объектов по интерфейсу/базовому типу")]
    public class InputInterfaceCollectionView : AppViewBase
    {
        private readonly ToolsView toolsView;
        private ToolsItemButtonView deleteButton;

        public TableView tableView { get; }

        public string Expression { get; set; } = "ToString()";

        public List<string> TypeOptions { get; set; } = new();

        public ChangingList Items { get; } = new();

        public Action<int, object> OnItemAdded { get; set; } = (_, _) => { };

        public Action<int, object> OnItemRemoved { get; set; } = (_, _) => { };

        public InputInterfaceCollectionView()
        {
            toolsView = new ToolsView();
            tableView = new TableView { MinHeight = 170 };
            Items.OnAdded((i, p) => OnItemAdded(i, p));
            Items.OnRemoved((i, p) => OnItemRemoved(i, p));
            Dock = "Fill";
            AddCrudControlViews();
            LayoutFill(tableView);
            LayoutTop(toolsView);
            AutoSize = false;
            ApplyDefaultHeight();
        }

        private void ApplyDefaultHeight()
        {
            const int toolsHeight = 44;
            const int tableHeaderHeight = 32;
            const int minTableHeight = 120;
            MinHeight = toolsHeight + tableHeaderHeight + minTableHeight + 8;
            Height = MinHeight;
        }

        private void AddCrudControlViews()
        {
            //toolsView.tools.Add(CreateButtonTool("добавить", OnAddButtonClicked));
            //deleteButton = CreateButtonTool("удалить", OnDeleteButtonClicked);
            //tableView.DataRowsSelected.OnChanged += (_, _) =>
            //    deleteButton.Enabled = tableView.DataRowsSelected.Count() > 0;
            //deleteButton.Enabled = false;
            //toolsView.tools.Add(deleteButton);
        }

        private static ToolsItemButtonView CreateButtonTool(string text, Action todo)
        {
            var button = new ToolsItemButtonView { Text = text };
            button.OnClick += (_, _) => todo();
            button.OnItemClicked += todo;
            return button;
        }

        public string FormatItem(object item)
        {
            if (item is null)
                return string.Empty;
            try
            {
                return Expressions.Compile(Expression, item).ToString();
            }
            catch
            {
                return item.GetType().GetTypeName();
            }
        }

        //private void OnAddButtonClicked()
        //{
        //    if (TypeOptions.Count == 0)
        //        return;

        //    var labels = TypeOptions.ToDictionary(
        //        t => t,
        //        t => t.ToType().GetLabel());

        //    var combobox = new ComboboxView();
        //    foreach (var label in labels.Values.Distinct())
        //        combobox.Items.Add(label);
        //    if (combobox.Items.Count > 0)
        //        combobox.Value = combobox.Items[0];

        //    var formHost = new AppViewBase();
        //    formHost.LayoutTop(new TextView { Text = "Тип элемента" });
        //    formHost.LayoutTop(combobox);

        //    var ok = new ButtonView { Text = "OK" };
        //    var cancel = new ButtonView { Text = "Отмена" };
        //    var buttons = new AppViewBase();
        //    buttons.LayoutLeft(ok);
        //    buttons.LayoutRight(cancel);
        //    formHost.LayoutBottom(buttons);

        //    Form dialog = null;
        //    var confirmed = false;
        //    ok.OnClick += (_, _) => { confirmed = true; dialog?.Close(); };
        //    cancel.OnClick += (_, _) => dialog?.Close();
        //    dialog = formHost.ToControl().ToBaseForm("Добавить элемент");
        //    dialog.ShowDialog();
        //    if (!confirmed || string.IsNullOrWhiteSpace(combobox.Value))
        //        return;

        //    var typeName = labels.FirstOrDefault(kv => kv.Value == combobox.Value).Key
        //        ?? TypeOptions.First();
        //    var item = typeName.ToType().New();
        //    if (item is null)
        //        return;

        //    EditView editView = null;
        //    Form pickerDialog = null;
        //    editView = EditView.CreateEditViewBase(item, (success, model) =>
        //    {
        //        if (success && model is not null)
        //        {
        //            Items.Add(model);
        //            tableView.DataRows.Add(model);
        //        }
        //        editView?.RemoveFromParent();
        //        pickerDialog?.Close();
        //    });
        //    pickerDialog = editView.ToControl().ToBaseForm(editView.Text);
        //    pickerDialog.ShowDialog();
        //}

        //private void OnDeleteButtonClicked()
        //{
        //    foreach (var item in tableView.DataRowsSelected.ToList())
        //    {
        //        OnItemRemoved(Items.IndexOf(item), item);
        //        Items.Remove(item);
        //        tableView.DataRows.Remove(item);
        //    }
        //    tableView.DataRowsSelected.Clear();
        //    deleteButton.Enabled = false;
        //    if (tableView.control is DataTableControl dtc && !dtc.IsDisposed)
        //        dtc.ClearRowSelection();
        //}
    }
}
