 

using MaterialApp.Win_MaterialModule.DialogModule;

using MaterialSkin;
using MaterialSkin.Controls;

using System.Reflection;


using LiveCharts.Wpf;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

public interface IControlsFactory
{
    Label CreateBaseLabel(string text);
    Label CreateBaseLabel(string title, string fontName = "Roboto", int fontSize = 12);
    MaterialButton CreateButton(string text = "", Action onclick = null);
    MaterialCard CreateCard(Action<int> scrollChanged);
    MaterialCheckbox CreateCheckBox(string text, Action<bool> onchanged);
    MaterialCheckedListBox CreateCheckedListBox(Dictionary<string, bool> options, Action<string, bool> onchanged = null);
    MaterialCheckedListBox CreateCheckedListBox(IEnumerable<string> options, Action<int, bool> onchanged = null);
    Panel CreateCheckedListView(Action<MethodResult<IEnumerable<string>>> oncompleted, IEnumerable<string> options);
    ToolStripMenuItem CreateContextDropDownMenu(string text, IEnumerable<string> items, Action<string> onclick);
    ContextMenuStrip CreateContextMenu(ContextView model, Action<string> onclick);
    ContextMenuStrip CreateContextMenu(IEnumerable<string> items, Action<string> onclick);
    ToolStripItem CreateContextMenu(List<string> path, ContextView model, Action<string> onclick);
    ContextMenuStrip CreateContextMenu(params ToolStripMenuItem[] items);
    ToolStripMenuItem CreateContextMenu(string text, IEnumerable<ToolStripMenuItem> items);
    ContextMenuStrip CreateContextMenu2(IEnumerable<string> model, Action<string> onclick);
    ToolStripMenuItem CreateContextMenuItem(string title, Action onclick);
    IEnumerable<ToolStripMenuItem> CreateContextMenuItems(IEnumerable<string> items, Action<string> onclick);
    MaterialButton CreateDangerButton(string text, Action onclick);
    DateTimePicker CreateDateTimePicker(DateTime value, Action<DateTime> changed);
    MaterialCard CreateEditFormView<TModel>(TModel model, Action<TModel> oncompleted);
    MaterialExpansionPanel CreateExpansionPanel(string text, Action<bool, object> onsubmit);
    Panel CreateFlexRow(Control left, Control stretch, Control right);
    Panel CreateFormField(PropertyInfo property, Action<object> onChange);
    DataGridView CreateGrid(object data);
    DataGridView CreateGridView(IEnumerable<object> items, IEnumerable<string> columns);
    DataGridView CreateGridView<TModel>(IEnumerable<TModel> items, IEnumerable<string> columns);
    PictureBox CreateImage(byte[] data);
    Control CreateImage(string file);
    Control CreateInputColor(object value, Action<string> onInput);
    DateTimePicker CreateInputDate(DateTime? value, Action<DateTime> changed);
    Control CreateInputDateTime(object value, Action<string> onInput);
    MaterialTextBox2 CreateInputNumber(object value, Action<string> oninput);
    Control CreateInputPasswordControl(InputFormField formField);
    Control CreateInputPhone(object value, Action<string> onInput);
    Control CreateInputText(string text, Action<string> onchanged);
    Control CreateInputTextControl(InputFormField formField, List<Func<string, string>> filters = null);
    Control CreateInputUrl(object value, Action<string> onInput);
    MaterialLabel CreateLabel(string title, string fontName = "Roboto", int fontSize = 12);
    MaterialLabel CreateLabel(string title, string fontName, int fontSize, Color color);
    CartesianChart CreateLineChart(string xLabel, List<NamedSeriesArray> series, string yLabel = "");
    CartesianChart CreateLineChart(string oxlabel, Dictionary<string, int> data);
    CartesianChart CreateLineChart(string oxlabel, Dictionary<string, List<float>> data);
    CartesianChart CreateColumnChart(string xLabel, List<NamedSeriesArray> series, string yLabel = "");
    CartesianChart CreateBarChart(string xLabel, List<NamedSeriesArray> series, string yLabel = "");
    Control CreatePieChart(List<NamedSeriesArray> series, string chartTitle = "");
    CartesianChart CreateOrgChart(string xLabel, List<NamedSeriesArray> series, string yLabel = "");
    Label CreateLink(string text, Action onclick);
    ListBox CreateListBoxView(IEnumerable<string> items, Action<string> onselected);
    Panel CreateListEditor<TModel>(List<TModel> items, Action<TModel> onselect, Action<TModel> oncreated, Action<TModel> ondeleted) where TModel : class;
    MaterialButton CreateMaterialButton(string text = "", Action onclick = null);
    MaterialListBox CreateMaterialList(IList<string> options, Action<string> onselect);
    MaterialListBox CreateMaterialList<TModel>(IList<TModel> items, Func<TModel, MaterialListBoxItem> convert, Action<TModel> onselect) where TModel : class;
    MaterialTextBox CreateMaterialTextBox(string text, Action<string> oninput);
    MaterialMultiLineTextBox CreateMultilineTextBox(string text, Action<string> changed);
    Panel CreatePanel(params Control[] controls);
    Panel CreateScrollPanel(Action<int> scrollChanged);
    ComboBox CreateSelectBox(List<string> Options);
    Panel CreateSelectListControl<TModel>(List<TModel> datasets, Func<TModel, MaterialListBoxItem> convert, Action<TModel> oncomplete) where TModel : class;
    MaterialListBox CreateSelectListWithTitle(string title, List<string> options, Action<string> onselect);
    MaterialComboBox CreateSelectObjectbyIdBox(Dictionary<int, string> Options);
    MaterialCard CreateSplitCard(Control left, Control right);
    SubmitDialogForm CreateSubmitDialog(string title, Control target, Action<bool> complete);
    MaterialCard CreateSubmitDialog(string title, string text, Action<bool> value);
    MaterialCard CreateSubmitPanel(string title, Control content, Action<bool> value);
    TabControl CreateTabbedPane(Dictionary<string, Control> model);
    TabControl CreateTabbedPaneProvider(Dictionary<string, Func<object, Control>> model);
    DataGridView CreateTable<TModel>(string title, IEnumerable<string> columns, IEnumerable<TModel> values, Action<DataGridView, int, int, object, object, string> onchanged = null);
    DataGridView CreateTableDictionary(string title, IEnumerable<string> columnsLabels, IEnumerable<string> columnsKeys, IEnumerable<Dictionary<string, object>> values, Action<DataGridView, int, int, object, object, string> onchanged = null);
    MaterialTextBox CreateTextbox(InputFormField field);
    MaterialTextBox2 CreateTextbox(object value, Action<string> oninput);
    MaterialTextBox2 CreateTextbox(object value, Action<string> oninput, Action<List<string>> onerrors, params Func<string, string>[] filters);
    Control CreateTextBox(string value, Action<string> onChange);
    Label CreateTextLabel(string text, string fontName = "Roboto", int fontSize = 12);
    TreeNode CreateTreeSelectByNameView(TreeViewNodeModel root, Action<string> selected);
    System.Windows.Forms.TreeView CreateTreeTypedView<T>(TypeNode<T> root, Action<T> selected);
    void Edit<TModel>(MaterialForm form, TModel model, Action<TModel> oncomplete) where TModel : class;
    MaterialCard FixTop(MaterialLabel label, MaterialCheckedListBox checkList);
    List<ControlsFactory.FormFieldModel> GetInputFields(object target);
    void InfoTime(string text, Action todo);
    void LayoutFill(Control panel, Control control);
    Panel LayoutFlexHorizontal(Control left, Control body, Control right);
    Panel LayoutFlexVertical(Control top, Control body, Control bottom);
    MaterialCard LayoutIntoCard(Control control);
    void SingleSelect<TModel>(MaterialForm form, IEnumerable<TModel> items, Func<TModel, MaterialListBoxItem> convert, Action<TModel> onselected) where TModel : class;
}