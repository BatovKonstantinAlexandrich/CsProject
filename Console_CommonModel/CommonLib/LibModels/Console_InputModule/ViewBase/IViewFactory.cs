using Console_CommonModel.CommonLib.LibViews.AppViewModules.ComboboxModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.MultiLineModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;
using Console_CommonModel.CommonLib.LibViews.Others;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System.Reflection;

using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;

using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

public interface IViewDialogFactory
{
    public Task<object> CreateEditDialog(object target, Action<object> onsuccess);
}
public interface IViewFactory 
{
   
    
    TextView CreateText(string title, string fontName = "Roboto", int fontSize = 12);
    ButtonView CreateButton(string text = "", Action onclick = null);
    CardView CreateCard(Action<int> scrollChanged);
    CheckboxView CreateCheckbox(string text, Action<bool> onchanged);
    CheckboxView CreateCheckBox(string text, Action<bool> onchanged);
    //CheckedListBox CreateCheckedListBox(IEnumerable<string> options, Action<int, bool> onchanged = null);
    CheckedListMultiView<TNamedObject> CreateCheckList<TNamedObject>(IEnumerable<TNamedObject> items, Action<int, bool> onchanged = null) where TNamedObject : NamedObject;
    //ColumnsView CreateColumnsPanel(IEnumerable<Control> controls);
    ComboboxView CreateCombobox([NotNullNotEmpty] string title, [NotNullNotEmpty] ChangingList<string> options, [NotNullNotEmpty] Action<string> onchanged);
    ChangingList<ContextView> CreateContextMenuItems(List<string> items, Action<string> value);
    ButtonView CreateDangerButton(string text, Action onclick);
    EditView CreateEditFormView<TModel>(TModel model, Action<TModel> oncompleted);
    AppViewBase CreateFormField(object target, string property);
    ViewItem CreateFormView(AppViewBase view);
    GridView CreateGrid(object data);
    GridView CreateGridView(IEnumerable<object> items, IEnumerable<string> columns);
    GridView CreateGridView<TModel>(IEnumerable<TModel> items);
    GridView CreateGridView<TModel>(IEnumerable<TModel> items, IEnumerable<string> columns);
    ImageView CreateImage(byte[] data);
    ImageView CreateImage(string file);
    TextView CreateText(string text, Action onclick = null);
    void CreateLink(List<string> lists, ViewNode pnode);
    ListView CreateListView([NotNullNotEmpty] string title, [NotNullNotEmpty] ChangingList<string> options, [NotNullNotEmpty] Action<string> onchanged);
    ListView CreateListView(string name, List<Option> options, Action<object> onselect);
    public MultiLineTextView CreateMultilineTextBox(string text, Action<string> value = null);

    ListView CreateSelectList(string text, List<string> options, Action<string> onselected);
    TableView CreateTableView<T>(string name, List<string> properties, List<T> dataset, Action<object> onselect);
    TreeControlView CreateTree(Action<object> onselected = null);
    TreeControlView CreateTreeTypedView<T>(TypeNode<T> root, Action<T> selected);
    TreeControlView CreateTreeView<T>(string name, List<string> properties, IQueryable<T> dataset, Action<object> onselect) where T : HierTable<T>;
}