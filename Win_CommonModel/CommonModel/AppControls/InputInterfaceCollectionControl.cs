using MaterialSkin;
using MaterialSkin.Controls;

using System.Data;

using WinFormsApp.AppComponents.AppControls;

using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;

namespace WinFormsDesignerApp.AppComponents.InputCollections
{
    public partial class InputInterfaceCollectionControl : PanelsControl
    {
        private ChangingList<object> items = new();
        private MaterialListBox ListView;
        private ColumnsPanel ToolsPane;
        private SearchControl SearchControl;

        public InputInterfaceCollectionControl()
        {
            InitializeComponent();
            //this.HideSides();
          
        }

        public void Bind<GenericTypeArgument>(string expression="ToString()")
        {
            this.GenericType = typeof(GenericTypeArgument);
            this.Expression = expression;
            this.EnsurePropertyIsValide(nameof(Expression));
            this.EnsurePropertyIsValide(nameof(GenericType));
            var factory = new global::ControlsFactory();
            items.OnChanged += this.OnCollectionChanged;
            var listItems = GetListItems();
            
            this.ListView = factory.CreateMaterialList(listItems, selected =>
            {
                var target = this.ListView.SelectedItem.Tag;
                this.Info(target.GetTypeName());
                var editor = new InputPanel(target, target.GetType(), (s, e) => { 
                });
                editor.ShowDialog();
                SetRight(editor);
            });
            this.ToolsPane = new ColumnsPanel() { Mode = false };
            this.ToolsPane.Add(this.SearchControl = factory.CreateInputSearch(OnTypeSelected, OnAddClicked ));
            this.SearchControl.SearchOptions = GetOptions();
            this.ListView.Dock = DockStyle.Fill;
            this.LayoutFill(this.ListView);
            this.SetTop(this.ToolsPane);
            this.SetTopHeight(ToolsPane.Height=30);            
        }

        private void OnCollectionChanged(object sender, IChangingListEvent e)
        {
            switch (e.Target.Action)
            {
                case "added":
                    var item = new MaterialListBoxItem();                    
                    var text = GetStringValue(e.Target.Item);
                    item.Tag = e.Target.Item;
                    item.Text = text;                                      
                    this.ListView.Items.Add(item);
                    break;
                case "removed":
                    var listItem = this.ListView.Items.FirstOrDefault(p => p.Tag == e.Target.Item);
                    this.ListView.Items.Remove(listItem);
                    break;
            }
        }

        private void OnTypeSelected(string text)
        {

        }


        private void OnAddClicked(string typeName)
        {
            this.AddItem(typeName);
            //this.SetView();
        }

        private void AddItem(string type)
        {
            var item = type.ToType().New();
            this.items.Add(item);
            this.LayoutFill(ListView);
        }

         

        [NotNullNotEmpty]
        public Type GenericType { get; set; }


        [NotNullNotEmpty]
        public string Expression { get; set; } = "ToString()";

        public List<string> GetListItems()
        {
            return this.items.Select(item => GetStringValue(item)).ToList();
        }

        private string GetStringValue(object item)
        {
            return global::Expressions.Compile(this.Expression, item).ToString();
        }

        public ChangingList<string> GetOptions()
        {           
            if (GenericType.IsInterface)
            {
                return new(ServiceFactory.Get().GetTypesImplements(GenericType).Where(t => !t.IsAbstract).Select(t => t.GetTypeName()).ToList());
            }
            else
            {
                return new(ServiceFactory.Get().GetTypesExtended(GenericType).Where(t => !t.IsAbstract).Select(t => t.GetTypeName()).ToList());
            }
        }
    }
}
