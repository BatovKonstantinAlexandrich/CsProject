







using System.Reflection;
using Console_CommonModel.CommonLib.LibViews.AppViews.FormModule;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.StatusModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeModule;


using Console_CommonModel.CommonLib.LibViews.AppViews.FormModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.FormModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.LayoutModule;
using Microsoft.Extensions.Options;
using Namotion.Reflection;
using System.Data;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.ComboboxModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.MultiLineModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.CheckedListModule;
using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.DataTable;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
using Console_CommonModel.CommonLib.LibViews.Others;

public interface IViewsFactory
{
    //InputView Create(Type ptype, Action<bool, object> oncomplete = null);
    //InputView<TModel> Create<TModel>(Action<bool, TModel> oncomplete = null) where TModel : class;
    TextView CreateText(string text);
    TextView CreateText(string title, string fontName = "Roboto", int fontSize = 12);
    ButtonView CreateButton(string text = "", Action onclick = null);
    CardView CreateCard(Action<int> scrollChanged);
    CheckboxView CreateCheckBox(string text, Action<bool> onchanged);
    //CheckedListView CreateCheckedListBox(Dictionary<string, bool> options, Action<string, bool> onchanged = null);
    //CheckedListBox CreateCheckedListBox(IEnumerable<string> options, Action<int, bool> onchanged = null);
    //CheckedListView CreateCheckedListView(Action<MethodResult<IEnumerable<string>>> oncompleted, IEnumerable<string> options);
    //ColumnsView CreateColumnsPanel(IEnumerable<Control> controls);


    ListView CreateListView(string name, List<Option> options, Action<object> onselect);
    ListView CreateTableView<T>(string name, List<string> properties, List<T> dataset, Action<object> onselect);
    ListView CreateTreeView<T>(string name, List<string> properties, List<T> dataset, Action<object> onselect) where T : HierTable<T>;



    ButtonView CreateDangerButton(string text, Action onclick);
    //DateTimePickerView CreateDateTimePicker(DateTime value, Action<DateTime> changed);
    AppViewBase CreateEditFormView<TModel>(TModel model, Action<TModel> oncompleted);



    CheckboxView CreateCheckbox(string text, Action<bool> onchanged);
    CheckboxView CreateSelectList(string text, List<string> options, Action<bool> onchanged);

    //ExpansionsView CreateExpansionPanel(string text, Action<bool, object> onsubmit);
    GridView CreateGrid(object data);
    GridView CreateGridView(IEnumerable<object> items, IEnumerable<string> columns);
    GridView CreateGridView<TModel>(IEnumerable<TModel> items, IEnumerable<string> columns);
    ImageView CreateImage(byte[] data);
    ImageView CreateImage(string file);
    /*
    Control CreateInputColor(object value, Action<string> onInput);
    DateTimePicker CreateInputDate(DateTime? value, Action<DateTime> changed);
    Control CreateInputDateTime(object value, Action<string> onInput);
    InputDialogForm CreateInputDialogForm(string title, object target, Action<object> complete);
    InputPanel CreateInputForm(object target, Action<bool, object> oncomplete = null);
    InputPanel<TModel> CreateInputForm<TModel>(Action<bool, TModel> oncomplete = null) where TModel : class;
    InputPanelExp CreateInputFormExt(object target, Action<bool, object> oncomplete);
    InputPanel CreateInputFormForType(Action<bool, object> oncomplete, Type type);
    MaterialTextBox2 CreateInputNumber(object value, Action<string> oninput);
    Control CreateInputPasswordControl(InputFormField formField);
    Control CreateInputPhone(object value, Action<string> onInput);
    InputSearch CreateInputSearch(Action<string> oninput, Action<string> onsearch);
    Control CreateInputText(string text, Action<string> onchanged);
    Control CreateInputTextControl(InputFormField formField, List<Func<string, string>> filters = null);
    Control CreateInputUrl(object value, Action<string> onInput);
    MaterialLabel CreateLabel(string title, string fontName = "Roboto", int fontSize = 12);
    MaterialLabel CreateLabel(string title, string fontName, int fontSize, Color color);
    CartesianChart CreateLineChart(string oxlabel, Dictionary<string, int> data);
    CartesianChart CreateLineChart(string oxlabel, Dictionary<string, List<float>> data);
    Label CreateLink(string text, Action onclick);
    ListBox CreateListBoxView(IEnumerable<string> items, Action<string> onselected);
    Panel CreateListEditor<TModel>(List<TModel> items, Action<TModel> onselect, Action<TModel> oncreated, Action<TModel> ondeleted) where TModel : class;
    ColumnsPanel CreateListPanel(IList<string> options, Func<string, Control> constructor, Action<string> onselect);
    MasterEditor CreateMasterEditor(Action<bool, Dictionary<Type, object>> oncompleted = null, params Type[] types);
    MaterialButton CreateMaterialButton(string text = "", Action onclick = null);
    MaterialListBox CreateMaterialList(IList<string> options, Action<string> onselect);
    MaterialListBox CreateMaterialList<TModel>(IList<TModel> items, Func<TModel, MaterialListBoxItem> convert, Action<TModel> onselect) where TModel : class;
    MaterialTextBox CreateMaterialTextBox(string text, Action<string> oninput);
    MaterialMultiLineTextBox CreateMultilineTextBox(string text, Action<string> changed);
    Panel CreatePanel(params Control[] controls);
    Panel CreateScrollPanel(Action<int> scrollChanged);
    SearchControl CreateSearchControl();
    ComboBox CreateSelectBox(List<string> Options);
    Panel CreateSelectListControl<TModel>(List<TModel> datasets, Func<TModel, MaterialListBoxItem> convert, Action<TModel> oncomplete) where TModel : class;
    MaterialListBox CreateSelectListWithTitle(string title, List<string> options, Action<string> onselect);
    ComboBox CreateSelectObjectbyIdBox(Dictionary<int, string> Options);
    MaterialCard CreateSplitCard(Control left, Control right);
    StatusControl CreateStatusPanel(Action<string, int> changed);
    SubmitDialogForm CreateSubmitDialog(string title, Control target, Action<bool> complete);
    MaterialCard CreateSubmitDialog(string title, string text, Action<bool> value);
    MaterialCard CreateSubmitPanel(string title, Control content, Action<bool> value);
    TabControl CreateTabbedPane(Dictionary<string, Control> model);
    TabControl CreateTabbedPaneProvider(Dictionary<string, Func<object, Control>> model);
    DataGridView CreateTable<TModel>(string title, IEnumerable<string> columns, IEnumerable<TModel> values, Action<DataGridView, int, int, object, object, string> onchanged = null);
    DataGridViewController<TModel> CreateTableController<TModel>(string title, IEnumerable<string> headers, IEnumerable<TModel> values, Action<object, string, int> onchanged) where TModel : BaseEntity;
    DataGridView CreateTableDictionary(string title, IEnumerable<string> columnsLabels, IEnumerable<string> columnsKeys, IEnumerable<Dictionary<string, object>> values, Action<DataGridView, int, int, object, object, string> onchanged = null);
    Panel CreateTestPanel(AppUnit unit);
    NodeModel CreateTestTreeNode(TestingElement testing);
    MaterialTextBox CreateTextbox(FormField field);
    MaterialTextBox2 CreateTextbox(object value, Action<string> oninput);
    MaterialTextBox2 CreateTextbox(object value, Action<string> oninput, Action<List<string>> onerrors, params Func<string, string>[] filters);
    Control CreateTextBox(string value, Action<string> onChange);
    Label CreateTextLabel(string text, string fontName = "Roboto", int fontSize = 12);
    System.Windows.Forms.TreeView CreateTreeDraggableView(NodeModel root, Action<string> selected);
    System.Windows.Forms.TreeView CreateTreeSelectByNameView(NodeModel root, Action<string> selected);
    TreeNode CreateTreeSelectByNameView(TreeControlView.TreeViewNodeModel root, Action<string> selected);
    System.Windows.Forms.TreeView CreateTreeSelectView(NodeModel root, Action<string> selected, Func<TreeNode, TreeNode> handler = null);
    System.Windows.Forms.TreeView CreateTreeTypedView<T>(TypeNode<T> root, Action<T> selected);
    void Edit<TModel>(MaterialForm form, TModel model, Action<TModel> oncomplete) where TModel : class;
    InputPanel<TModel> Edit<TModel>(TModel model, Action<bool, TModel> oncomplete = null) where TModel : class;
    InputPanel<TModel> Edit<TModel>(TModel model, List<string> properties, Action<bool, TModel> oncomplete = null) where TModel : class;
    InputPanel<TModel> EditObject<TModel>(TModel model, Action<bool, TModel> oncomplete = null) where TModel : class;
    MaterialCard FixTop(MaterialLabel label, MaterialCheckedListBox checkList);
    List<ControlsFactory.FormFieldModel> GetInputFields(object target);
    void InfoTime(string text, Action todo);
    void LayoutFill(Control panel, Control control);
    Panel LayoutFlexHorizontal(Control left, Control body, Control right);
    Panel LayoutFlexVertical(Control top, Control body, Control bottom);
    MaterialCard LayoutIntoCard(Control control);
    void SingleSelect<TModel>(MaterialForm form, IEnumerable<TModel> items, Func<TModel, MaterialListBoxItem> convert, Action<TModel> onselected) where TModel : class;
    */
}
[Label("Создание представлений")]
public class ViewFactory : ViewNode, IViewFactory
{
    [Label("Идентификатор области памяти")]
    [NotInput()]
    public int Scope { get; set; }

    public ViewFactory() : base()
    {
        this.Scope = GetHashCode();

    }

    public object Create(string typeName)
    {
        return ReflectionService.CreateWithDefaultConstructor<object>(typeName);
    }

    [Label("Создание выпадающего списка")]
    public ListView CreateListView(
        [NotNullNotEmpty] string title,
        [NotNullNotEmpty] ChangingList<string> options,
        [NotNullNotEmpty] Action<string> onchanged)
    {
        //MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(title,options,onchanged);
        var lr = new global::ListView()
        {
            Text = title,
            Options = new(options.Select(p => new Option()
            {
                Value = p
            })),
            OnSelected = p => onchanged($"{(p is Option? ((Option)p).Value: p)}")
        };

        return lr;
    }


    [Label("Создание выпадающего списка")]
    public ListView CreateListView<T>(
        [NotNullNotEmpty] string title,
        [NotNullNotEmpty] ChangingList<T> options,
        [NotNullNotEmpty] Action<T> onchanged)
        where T: BaseEntity
    {
        var lr = new global::ListView()
        {
            Text = title,
            Options = new(options.Select(p => new Option()
            {
                Value = p is null? "": p.HasProperty("Label") && $"{p.GetValue("Label")}"!= "Надпись" ? $"{p.GetValue("Label")}" : (p is NamedObject no) ? no.Name : p.ToString(),
                Item = p
            })),
            OnSelected = (Option p) =>
            {
                T pt = (p is Option ? ((T)(((Option)p).Item)) : ((T)p.Item));
                onchanged(pt);
            }
        };
        options.OnAdded((i, p) => {
            lr.Options.Add(new Option()
            {
                Value = (p is NamedObject no) ? no.Name : p.ToString(),
                Item = p
            });
        });
        options.OnRemoved((i, p) => { lr.Options.Remove(lr.Options.FirstOrDefault(o=>o.Value== ((p is NamedObject no) ? no.Name : p.ToString()))); });

        return lr;
    }


    [Label("Создание выпадающего списка")]
    public ComboboxView CreateCombobox(
        [NotNullNotEmpty] string title,
        [NotNullNotEmpty] ChangingList<string> options,
        [NotNullNotEmpty] Action<string> onchanged)
    {
        //MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(title,options,onchanged);
        return new ComboboxView()
        {
            Text = title,
            Options = options,
            OnSelected = onchanged
        };
    }

    [Label("Создание выпадающего списка")]
    public ComboboxView CreateComboboxForDictionary(
            [NotNullNotEmpty] string title,
            [NotNullNotEmpty] Dictionary<string, string> options,
            [NotNullNotEmpty] Action<string> onchanged)
    {
        //MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(title,options,onchanged);
        return new ComboboxView()
        {
            Text = title,
            Options = new(options.Keys.ToList()),
            OnSelected = (key) => {
                if(onchanged is not null && key is not null)
                {
                    onchanged(options[key]);
                }                    
            }
        };
    }

    public TextView CreateText(string text, Action onclick)
    {
        var view = new Console_CommonModel.CommonLib.LibViews.AppViews.TextModule.TextView();
        view.Text = text;
        view.OnClick += (s, e) =>
        {
            if (onclick is not null)
            {
                onclick();
            }
        };
        view.FontSize = 14;
        return view;
    }

    
    public TextView CreateTitleCentral(string text, Action onclick=null)
    {
        var view = new Console_CommonModel.CommonLib.LibViews.AppViews.TextModule.TextView();
        view.Text = text;
        view.OnClick += (s, e) =>
        {
            if (onclick is not null)
            {
                onclick();
            }
        };
        view.FontSize = 22;
        view.TextAlign = "center";
        view.Dock = "Top";
        view.Height = view.MinHeight = 33;
        view.OnLoad += (s, e) => {
            view.TextAlign = "center";
        };
        return view;
    }

    public TextView CreateTitle(string text, Action onclick = null)
    {
        var view = new Console_CommonModel.CommonLib.LibViews.AppViews.TextModule.TextView();
        view.Text = text;
        view.OnClick += (s, e) =>
        {
            if (onclick is not null)
            {
                onclick();
            }
        };
        view.FontSize = 22;
        view.TextAlign = "center";
        view.Dock = "Top";
        view.MinHeight = 40;
        return view;
    }

    public void CreateLink(List<string> lists, ViewNode pnode)
    {
        if (lists.Count() >= 1)
        {
            string name = lists.Last();
            lists.RemoveAt(lists.Count() - 1);
            ViewNode node = Find(lists);

            node.Append(name, pnode);
        }
    }

    public ChangingList<ContextView> CreateContextMenuItems(List<string> items, Action<string> value)
    {
        var r = new ChangingList<ContextView>();
        foreach (var item in items)
        {
            var p = item;
            r.Add(new()
            {
                Label = item,
                OnClick = (s, e) =>
                {
                    if (value is not null) value(p);
                }
            });
        }
        return r;
    }

    public Action AddContextMenuItems(List<string> items, Action<string> value)
    {
        var r = CreateContextMenuItems(items,value);
        this.Cast<AppViewBase>().ContextMenu.Items.AddRange(r);
        return () => {
            this.Cast<AppViewBase>().ContextMenu.Items.RemoveRange(r);
        };
    }


    public TreeControlView CreateTree(Action<object> onselected = null)
    {
        var view = new TreeControlView();

        var node = view.root;
        Action<TreeViewNodeModel> updateNode = null;
        updateNode = (pnode) =>
        {
            pnode.OnClick += (s, e) =>
            {
                if (onselected is not null)
                    onselected(pnode);
            };
            foreach (var pchild in pnode.Children)
            {
                updateNode(pchild);
            }
            pnode.Children.OnChanged += (s, e) =>
            {
                if (e.Target.Action == "added")
                {
                    updateNode((TreeViewNodeModel)e.Target.Item);
                }
            };
        };
        updateNode(node);
        return view;
    }

    public string GetHelp()
    {
        return CommonUtils.LabelFor(GetType());
    }


    //public virtual Control ToWinFormsControl()
    //    => new global::ComponentsFactory().CreateControl((ViewItem)this);


    public TextView CreateTextCenter(string text)
    {
        return new TextView() { Text = text, TextAlign = "center" };
    }

    public TextView CreateTextCenterLink(string text, Action onclick)
    {
        return new TextView() { Text = text,
            OnClick = (s, e) => {
                onclick();
            },
            FontSize=16
        , TextAlign = "center"};
    }

    public TextView CreateText(string title, string fontName = "Roboto", int fontSize = 12)
    {
        return new TextView() { Text = title, FontSize = fontSize };
    }
    public TextView CreateRedText(string title, string fontName = "Roboto", int fontSize = 12)
    {
        return new TextView() { Text = title, FontSize = fontSize, Height=20, Color = "#FF0000", TextAlign = "center"};
    }

    public TextView CreateRedTextLink(string title, Action onclick, string fontName = "Roboto", int fontSize = 12)
    {
        return new TextView() { Text = title, FontSize = fontSize, Color = "#FF0000", TextAlign = "center", OnClick=(s,e)=> {
            onclick();
        } };
    }

    public AppViewBase CreateFormField(object target, string property)
    {
        var container = new AppViewBase();
        container.AutoSize = true;
        container.CreateButton(property).Dock = "Top";
        //container.CreateInput(target, property).Dock = "Top";
        this.Append(container);
        return container;
    }

    public ToolsItemButtonView CreateButtonTool(string text, Action todo)
    {
        var button = new ToolsItemButtonView() { Text = text };
        button.OnClick += (p, e) => todo();
        button.OnItemClicked += todo;
        return button;
    }

    public ToolsView CreateTools(Dictionary<string,Action> todos)
    {
        var tools = new ToolsView();
        foreach(var kv in todos)
        {
            tools.tools.Add(this.CreateButtonTool(kv.Key, kv.Value));
        }
        return tools;
    }

    public ButtonView CreateButton(string text = "", Action onclick = null)
    {
        var button = new ButtonView() { Text = text };
        button.OnClick += (s, e) =>
        {
            if (onclick is not null)
            {
                onclick();
            }
        };
        if (this is ILayoutViewNode)
        {
            ((ILayoutViewNode)this).Add(button);
        }
        return button;
    }

    public CardView CreateCard(Action<int> scrollChanged)
    {
        return new CardView();
    }

    public CheckboxView CreateCheckBox(string text, Action<bool> onchanged)
    {
     
        var checkbox = new CheckboxView() { Text = text };
 
        checkbox.OnCheckedChanged += (s, e) =>
        {
            onchanged(checkbox.Checked.ToString().ToBool());
        };
        return checkbox;
    }
   

    public CheckedListMultiView<TNamedObject> CreateCheckList<TNamedObject>(IEnumerable<TNamedObject> items, Action<int, bool> onchanged = null) where TNamedObject : NamedObject
    {
        var view = new CheckedListMultiView<TNamedObject>();
        return view;
    }
     
    public ButtonView CreateDangerButton(string text, Action onclick)
    {
        var view = CreateButton(text, onclick);        
        return view;
    }



    public EditView CreateEditFormView<TModel>(TModel model, Action<TModel> oncompleted=null)
    {        
        var edit = EditView.CreateEditViewBase(model, (success, ptarget) =>
        {
            if (success)
            {
                if (oncompleted is not null && ptarget is not null)
                {
                    oncompleted((TModel)ptarget);
                }
            }
        });
        return edit;
    }



    public ColumnsView CreateFlexRow(AppViewBase left, AppViewBase stretch, AppViewBase right)
    {
        var view = new ColumnsView();
        return view;
    }

   

    public GridView CreateGrid(object data)
    {
        var view = new GridView();
        return view;
    }

    public GridView CreateGridView(IEnumerable<object> items, IEnumerable<string> columns)
    {
        var view = new GridView();
        return view;
    }

    public GridView CreateGridView<TModel>(IEnumerable<TModel> items, IEnumerable<string> columns)
    {
        var view = new GridView();
        return view;
    }

    public GridView CreateGridView<TModel>(IEnumerable<TModel> items)
    {
        return CreateGridView<TModel>(items, typeof(TModel).GetOwnInputPropertyNames());
    }

    public ImageView CreateImage(byte[] data)
    {
        return new ImageView()
        {
            Data = data,
            Dock = "Fill"
        };
    }

    public ImageView CreateImage(string file)
    {
        return new ImageView()
        {
            Data = System.IO.File.ReadAllBytes(file)
        };
    }

    public TreeControlView CreateTreeTypedView<T>(TypeNode<T> root, Action<T> selected)
    {
        var view = new TreeControlView();



        Func<TypeNode<T>, TreeViewNodeModel> convert = null;
        convert = (tnode) =>
        {
            var node = new TreeViewNodeModel();
            node.Title = tnode.NodeName;
            foreach (var pchild in tnode.ChildNodes)
            {
                var nodeChild = convert(pchild.Value);
                node.Children.Add(nodeChild);
            }
            return node;
        };
        Func<TreeViewNodeModel, T> find = (pnode) =>
        {
            var p = pnode;
            var path = new List<string>();
            while (p is not null)
            {
                path.Add(p.Title);
                p = p.ParentNode;
            }
            path.Reverse();
            var tp = root;
            path.RemoveAt(0);
            foreach (var name in path)
            {
                tp = tp.ChildNodes[name];
            }
            return tp.NodeItem;
        };
        view.root.Children.Add(convert(root));

        view.root.OnClick += (sender, e) =>
        {
            T item = find(view.root);
            selected(item);
        };
        return view;
    }

    public MultiLineTextView CreateMultilineTextBox(string text, Action<string> value=null)
    {
        return new MultiLineTextView()
        {
            Text = text,
            OnTextChanged = text => value(text)
        };
    }

    public ViewItem CreateFormView(AppViewBase view)
    {
        return new InputFormModel(view);
    }

    internal ViewItem CreateInputView(AppViewBase view)
    {
        return new InputFormModel(view);
    }

    internal ViewItem CreateEditView(AppViewBase view)
    {
        return EditView.CreateEditViewBase(view);
    }

    public ListView CreateListView(string name, List<Option> options, Action<object> onselect)
    {
        var res = new ListView();
        res.Text = name;
        res.Options = new(options);
        res.OnSelected = onselect;
        return res;
    }

    public TableView CreateTableView<T>(string name, List<string> properties, List<T> dataset, Action<object> onselect)
    {
        var tableView = new TableView();
        tableView.Title = name;
        tableView.Columns = properties;
        tableView.DataRows = new(dataset.Select(x => (object)x).ToList());
        return tableView;
    }

    public TreeControlView CreateTreeView<T>(string name, List<string> properties, IQueryable<T> dataset, Action<object> onselect)
        where T : HierTable<T>
    {
        var factory = new TreeService();
        return factory.CreateTreeView<T>(dataset, onselect);
    }

    public CheckboxView CreateCheckbox(string text, Action<bool> onchanged)
    {
        return new CheckboxView()
        {
            Text = text,
            OnCheckedChanged = (s, val) => onchanged(val.After)
        };
    }

    public ListView CreateSelectList(string text, List<string> options, Action<string> onselected)
    {
        var list = CreateListView(text, new(options), onselected);
        var scroll = list.ToScrollView();
        scroll.Dock = "Fill";
        list.Dock = "Fill";
        this.Controls.Clear();
        this.Controls.Add(scroll);
        return list;
    }

    public ListView CreateSelectList<T>(string text, List<T> options, Action<T> onselected) 
        where T: BaseEntity
    {
        var listView = CreateListView<T>(text, new(options), onselected);        
        return listView;
    }

    public ListView CreateSelectEntityList<T, TDbContext>(string text, Action<T> onselected) 
        where T : NamedObject
        where TDbContext: DbContextBase
    {
        using (var db = typeof(TDbContext).New<DbContextBase>()) 
            
            return CreateListView<T>(text, new(db.Set<T>().ToList()), onselected);
    }



    public ListView CreateSelectEntityList<T, TDbContext>(string text, IEnumerable<T> data, Action<T> onselected)
        where T : NamedObject
        where TDbContext : DbContextBase
    {
        using (var db = typeof(TDbContext).New<DbContextBase>())

            return CreateListView<T>(text, new(data), onselected);
    }

    public AppViewBase CreateEntityCheckList<T, TDbContext>(string text, Action<List<T>> onsuccess)
        where T : DescriptableObject
        where TDbContext : DbContextBase
    {
        var checkedListView = new CheckedListView();
        checkedListView.Text = text;
        using (var db = typeof(TDbContext).New<DbContextBase>())
        {
            checkedListView.ListItems.AddRange(db.Set<T>().ToList());
        }
        return checkedListView.ToSubmit((success, p) => {
            if(success)
            {
                var checkedItems = checkedListView.CheckedItems.Select(p => (T)p).ToList();
                if(onsuccess is not null)
                    onsuccess(checkedItems);
            }
        });        
    }

    /// <summary>
    /// Выбор с возможностью добавления
    /// </summary>
    public AppViewBase CreateEntityListEditor<T, TDbContext>(string text, Action<T> onselected)
        where T : DescriptableObject
        where TDbContext : DbContextBase
    {
        using (var db = typeof(TDbContext).New<DbContextBase>())
        {
            ChangingList<T> datalist = new(db.Set<T>().ToList());
            var pane = new AppViewBase();
            var list = CreateListView<T>(text, datalist, onselected);
            var tools = new MenuView();
            tools.MenuItems.Add(new()
            {
                Text = "Создать",
                OnClick = (s, e) =>
                {
                    pane.LayoutFill(
                        EditView.CreateEditViewBase(typeof(T).New(), (success, target) =>
                        {
                            if (success)
                            {
                                datalist.Add((T)target);
                            }
                            pane.LayoutFill(list);
                        })
                    );
                }
            });
            pane.LayoutFill(list);
            pane.Top = tools;
            return pane;
        }
        
    }

    internal ControlView CreateTextbox(string text, Action<string> onInput)
    {
        var ctrl = new TextBoxView()
        {
            Text = text,
            OnTextChanged = onInput
        };

        return ctrl;
    }
    internal ControlView CreateSelectbox(string title, List<string> extnames, Action<string> value)
    {
        var view = new ComboboxView();
        return view;
    }
    internal ControlView CreateSelectbox(List<string> extnames, Action<string> value)
    {
        var view = new ComboboxView();
        return view;
    }

    internal CheckedListView CreateCheckedListView(List<string> list, Action<int, bool> onchanged = null)
    {
        var view = new CheckedListView();
        foreach (var item in list)
            view.ListItems.Add(item);
        if (onchanged is not null)
            view.OnItemCheckedChanged = (index, _, isChecked) => onchanged(index, isChecked);
        return view;
    }

    internal ControlView CreateSelectbox(string v, ChangingList<string> selectOptions, Action<string> onInput)
    {
        var view = new ComboboxView();
        return view;
    }

   
   
    internal ControlView CreateTextbox2(string text, Action<string> onInput, Action<List<string>> onOptions, Func<string, string> convert)
    {
        var ctrl = new TextBoxView()
        {
            Text = text,
            OnTextChanged = onInput
        };
       
        return ctrl;
    }

    internal ViewItem CreateButtons(string[] strings, Action<object> value)
    {
        var textbox = new AppLayoutView();
        return textbox;
    }

    public RadioView CreateRadioGroup(string name, List<string> states, Action<string> onchanged)
    {
        return new RadioView()
        {
            Name = name,
            Options = states,
            OnSelected = TextBox => { }
        };
        
        
    }

    public DataTableView CreateDataTable(DataTable dataTable)
    {
        return new DataTableView()
        {
            Data = dataTable
        };
    }
    internal TableView CreateTable(string label, List<string> list, List<object> dataset, Action<object, object, object, object, object, object> value=null)
    {
        var tableView = new TableView();
        tableView.SetDataRows(list, dataset);
        return tableView;
    }

    internal TextBoxView CreateTextbox(object value1, Action<string> onInput, Action<List<string>> value2, Func<object, object> value3)
    {
        var textbox = new TextBoxView();
        return textbox;
    }

    

    internal ControlView CreateInputDate(DateTime? dateVal, Action<DateTime> value)
    {
        return new TextBoxView();
    }

    internal ControlView CreateInputNumber(string? value, Action<string> onchanged)
    {
        return new TextBoxView()
        {
            Text = value,
            OnTextChanged = text => {
                if (onchanged is not null)
                    onchanged(text);
            }
        };


    }

 
}
