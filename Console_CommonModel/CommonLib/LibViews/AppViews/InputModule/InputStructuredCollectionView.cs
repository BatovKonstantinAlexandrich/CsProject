using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.Others;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.InputModule
{

    [Label("Представление для редактирования коллекции примитивов")]
    public class InputStructuredCollectionView : AppViewBase
    {
        private readonly ToolsView toolsView;
        private ToolsItemButtonView editButton;
        private ToolsItemButtonView deleteButton;

        [Label("table View")]
        public TableView tableView { get; }
        [Label("Label")]

        public string Label { get; set; }
        [Label("Icon")]

        public string Icon { get; set; }


        /// <summary>
        /// имя типа элемента
        /// </summary>
        [Label("Item Type")]
        public string ItemType { get => Get<string>(nameof(ItemType)); set => Set(nameof(ItemType), value); }

        [OnChange(nameof(ItemType))]
        public void OnItemTypeChanged()
        {
            tableView.SetVisibleColumns(ItemType.ToType().GetOwnInputSimplePropertyNames());
        }

        [Label("Items")]
        public ChangingList Items { get; } = new();

        public InputStructuredCollectionView()
        {
            toolsView = new ToolsView();
            tableView = new TableView();
            tableView.MinHeight = 170;
            ItemType = nameof(UserAccount);
            tableView.SetVisibleColumns(ItemType.ToType().GetOwnInputSimplePropertyNames());

            Items.OnAdded((i, p) => { OnItemAdded(i, p); });
            Items.OnRemoved((i, p) => { OnItemRemoved(i, p); });
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
            var contentHeight = toolsHeight + tableHeaderHeight + minTableHeight + 8;
            MinHeight = contentHeight;
            Height = contentHeight;
        }


        [Label("On Item Added")]
        public Action<int, object> OnItemAdded { get; set; } = (i, p) => { };

        [Label("On Item Removed")]
        public Action<int, object> OnItemRemoved { get; set; } = (i, p) => { };


        private void AddCrudControlViews()
        {

            toolsView.tools.Add(CreateButtonTool("добавить", OnAddButtonClicked));
            toolsView.tools.Add(editButton = CreateButtonTool("редактировать", OnEditButtonClicked));
            deleteButton = CreateButtonTool("удалить", OnDeleteButtonClicked);
            tableView.DataRowsSelected.OnChanged += (s, evt) =>
            {
                editButton.Enabled = tableView.DataRowsSelected.Count() > 0;
                deleteButton.Enabled = tableView.DataRowsSelected.Count() > 0;
            };

            editButton.Enabled = false;
            deleteButton.Enabled = false;
            toolsView.tools.Add(deleteButton);
        }

        private ToolsItemButtonView CreateButtonTool(string text, Action todo)
        {
            var button = new ToolsItemButtonView() { Text = text };
            button.OnClick += (p, e) => todo();
            button.OnItemClicked += todo;
            return button;
        }

        private void OnAddButtonClicked()
        {
            //EditView editView = null;
            //Form f = null;
            //var itemType = ItemType.ToType();
            //EditView.WarmFieldCache(itemType);
            //var item = itemType.New();
            //var props = itemType.GetOwnInputPropertyNames();
            //editView = EditView.CreateEditViewBase(item,
            //    props,
            //    (success, model) => {
            //        if (success)
            //            this.Items.Add(model);
            //        editView.RemoveFromParent();
            //        f.Close();

            //        if (!success || model is null)
            //            return;

            //        // Нельзя обновлять DataGridView / BindingSource, пока модальный диалог ещё в стеке сообщений —
            //        // иначе WinForms часто «зависает» на CurrencyManager / перерисовке. После Close — отложенно на UI-потоке.
            //        var host = this.ToControl()?.FindForm() ?? Form.ActiveForm;
            //        void refreshTable()
            //        {
            //            this.LayoutFill(this.tableView);
            //            this.LayoutTop(this.toolsView);
            //            this.tableView.DataRows.Add(model);
            //        }

            //        if (host is { IsHandleCreated: true, IsDisposed: false })
            //            host.BeginInvoke(refreshTable);
            //        else
            //            refreshTable();
            //    });
            //f = editView.ToControl().ToBaseForm(editView.Text);
            //f.ShowDialog();
        }

        private void OnEditButtonClicked()
        {
            //EditView editView = null;
            //Form f = null;
            //var itemType = ItemType.ToType();
            //EditView.WarmFieldCache(itemType);
            //var pitem = this.tableView.DataRowsSelected.FirstOrDefault();
            //if (pitem is null)
            //    return;
            //var item = pitem;
            //var props = itemType.GetOwnInputPropertyNames();
            //editView = EditView.CreateEditViewBase(item,
            //    props,
            //    (success, model) => {
            //        if (success)
            //            this.Items.Add(model);
            //        editView.RemoveFromParent();
            //        f.Close();

            //        if (!success || model is null)
            //            return;

            //        // Нельзя обновлять DataGridView / BindingSource, пока модальный диалог ещё в стеке сообщений —
            //        // иначе WinForms часто «зависает» на CurrencyManager / перерисовке. После Close — отложенно на UI-потоке.
            //        var host = this.ToControl()?.FindForm() ?? Form.ActiveForm;
            //        void refreshTable()
            //        {
            //            this.LayoutFill(this.tableView);
            //            this.LayoutTop(this.toolsView);
            //            this.tableView.UpdateDatarows();
            //        }

            //        if (host is { IsHandleCreated: true, IsDisposed: false })
            //            host.BeginInvoke(refreshTable);
            //        else
            //            refreshTable();
            //    });
            //f = editView.ToControl().ToBaseForm(editView.Text);
            //f.ShowDialog();
        }

        private void OnDeleteButtonClicked()
        {
            //try
            //{
            //    foreach (var item in this.tableView.DataRowsSelected.ToList())
            //    {
            //        this.OnItemRemoved(Items.IndexOf(item), item);
            //        this.Items.Remove(item);
            //        this.tableView.DataRows.Remove(item);
            //    }
            //    this.tableView.DataRowsSelected.Clear();
            //    this.editButton.Enabled = false;
            //    this.deleteButton.Enabled = false;
            //    if (this.tableView.control is DataTableControl dtc && !dtc.IsDisposed)
            //        dtc.ClearRowSelection();
            //}
            //catch (Exception ex)
            //{
            //    this.Error($"Исключение при нажатии кнопки удаления: {ex}");
            //}
        }

    }
}
