

using Org.BouncyCastle.Asn1.Crmf;

using System;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.ContextModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.CheckboxModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;
using Console_CommonModel.CommonLib.LibViews.SharedViewControls;

using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.LineModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.LayoutModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
using Console_CommonModel.CommonLib.LibViews.Others;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.PrimitiveCollectionModule
{
    [Label("Представление для редактирования коллекции примитивов")]
    public class PrimitiveCollectionView : AppViewBase
    {
        private AppLayoutView layoutView;

        private readonly ToolsView toolsView;
        private readonly CheckboxView checkboxView;
        private readonly InputFormField formField;
        private readonly AppLayoutView fieldsLayoutView;
        private int selectedIndex { get => Get<int>(nameof(selectedIndex)); set => Set(nameof(selectedIndex), value); }

        [Label("Value")]
        public object Value { get => Get<object>(nameof(Value)); set => Set(nameof(Value), value); }

        [Label("Label")]
        public string Label { get => Get<string>(nameof(Label)); set => Set(nameof(Label), value); }
        private ChangingList<string> list = new ChangingList<string>();
        private ChangingList<AppViewBase> controls = new ChangingList<AppViewBase>();
        private ToolsItemButtonView deleteButton;
        private readonly List<AppViewBase> contextMenuTargets = new();

        public PrimitiveCollectionView() : this(new()
        {
            DataType = typeof(List<string>).GetTypeName()
        })
        { }
        public PrimitiveCollectionView(InputFormField view)
        {
            if (string.IsNullOrWhiteSpace(view.DataType))
                throw new ArgumentException("DataType должен быть заполнену");
            formField = view;
            LayoutFill(layoutView = new AppLayoutView() { IsVertical = true, FillSize = true, WrapContent = false });
            fieldsLayoutView = new AppLayoutView() { IsVertical = true, FillSize = true, WrapContent = true };
            fieldsLayoutView.AutoSize = false;
            layoutView.Add(checkboxView = new CheckboxView() { Text = view.Label, Width = 25 });
            view.AddPropertyChangeListener(nameof(view.Label), evt => checkboxView.Text = evt.After.ToString());

            toolsView = new ToolsView();
            AddCrudControlViews();
            list.OnAdded(OnAddButtonClicked);
            list.OnRemoved((i, p) =>
            {

                var ctrl = controls[i];
                var h = ctrl.ToControl().Height;
                if (ctrl.ToControl().Parent is not null)
                {
                    ctrl.ToControl().Parent.Controls.Remove(ctrl.ToControl());
                }
                controls.Remove(ctrl);
                fieldsLayoutView.Height -= h;
                deleteButton.Enabled = false;
            });
            var itemType = formField.ValueType.ToType();
            if (formField.Value is not null)
                foreach (var item in (dynamic)formField.Value)
                {
                    list.Add(item.ToString());
                }



            selectedIndex = -1;
            checkboxView.OnCheckedChanged += OnCheckedChanged;
            InitializeContextMenuSharing();
            AutoSize = true;
            Height = 500;
        }

        /// <summary>
        /// Пробрасывает пункты <see cref="ContextView"/> на вложенные представления и обновляет WinForms-меню.
        /// </summary>
        private void InitializeContextMenuSharing()
        {
            ContextMenu.Items.OnAdded((_, p) =>
            {
                if (p is ContextView item)
                {
                    foreach (var target in contextMenuTargets)
                        target.ContextMenu.Items.Add(item);
                    RefreshHostContextMenu();
                }
            });
            ContextMenu.Items.OnRemoved((_, p) =>
            {
                if (p is ContextView item)
                {
                    foreach (var target in contextMenuTargets)
                        target.ContextMenu.Items.Remove(item);
                    RefreshHostContextMenu();
                }
            });

            ShareContextMenuWith(layoutView);
            ShareContextMenuWith(checkboxView);
            ShareContextMenuWith(toolsView);
            ShareContextMenuWith(fieldsLayoutView);

            fieldsLayoutView.Controls.OnAdded((_, child) =>
            {
                if (child is AppViewBase childView)
                    ShareContextMenuWith(childView);
            });
        }

        private void ShareContextMenuWith(AppViewBase target)
        {
            if (target is null || contextMenuTargets.Contains(target))
                return;

            contextMenuTargets.Add(target);
            foreach (var item in ContextMenu.Items)
            {
                if (item is ContextView contextItem)
                    target.ContextMenu.Items.Add(contextItem);
            }
        }

        private void RefreshHostContextMenu()
        {
            //try
            //{
            //    if (control is System.Windows.Forms.Control host)
            //        factory.BindContextMenu(this, host);
            //}
            //catch
            //{
            //    /* контрол ещё не создан */
            //}
        }

        private void OnCheckedChanged(object sender, PropertyChangedMessage<bool> evt)
        {
            var state = evt.After;
            if (state)
            {
                layoutView.Add(toolsView);
                layoutView.Add(fieldsLayoutView);
                if (formField.Value is null)
                {
                    try
                    {
                        formField.Value = formField.DataType.ToType().New();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Не удалось создать значение по-умолчанию для свойства {formField.Name} с типом {formField.DataType} => {ex.Message}");
                    }
                }





            }
            else
            {
                layoutView.Remove(toolsView);
                layoutView.Remove(fieldsLayoutView);
            }
        }


        private void AddCrudControlViews()
        {
            //toolsView.tools.Add(CreateButtonTool("обновить", () =>
            //{
            //    list.Add("Новый элемент");
            //    var lc = (LayoutControl)fieldsLayoutView.ToControl();
            //    lc.UpdateLayout();
            //}));
            //toolsView.tools.Add(CreateButtonTool("добавить", () => { list.Add("Новый элемент"); }));
            //deleteButton = CreateButtonTool("удалить", () => { list.RemoveAt(selectedIndex); selectedIndex = -1; });
            //deleteButton.Enabled = false;
            //toolsView.tools.Add(deleteButton);
        }

        private ToolsItemButtonView CreateButtonTool(string text, Action todo)
        {
            var button = new ToolsItemButtonView() { Text = text };
            button.OnClick += (p, e) => todo();
            button.OnItemClicked += todo;
            return button;
        }

        private void OnAddButtonClicked(int index, object text)
        {
            int pi = index;
            var textBox = CreateTextbox($"{text}", text =>
            {
            });
            bool active = true;
            LineView line = null;
            var checkbox = CreateCheckbox($"", text =>
            {
                if (!active)
                    return;
                if (text)
                {

                    selectedIndex = controls.IndexOf(line);
                    deleteButton.Enabled = true;

                }
                else
                {
                    selectedIndex = -1;
                    deleteButton.Enabled = false;
                }
            });
            checkbox.Width = 25;
            line = new LineView(new (AppViewBase, bool)[] {
                (checkbox,false),(textBox, true)
            });
            //line.Height = textBox.Height;
            controls.Add(line);
            fieldsLayoutView.Add(line);
            ShareContextMenuWith(line);
            fieldsLayoutView.Height += line.ToControl().Height;






        }

    }
}
