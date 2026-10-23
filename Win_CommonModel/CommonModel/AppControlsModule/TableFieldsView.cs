using WinFormsDesignerApp.Console_DataModule;
using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;

namespace WinFormsDesignerApp
{

    /// <summary>
    /// Настройка элементов управления в Grid на основе моделей 
    /// поля для ввода
    /// </summary>
    public partial class TableFieldsView : UserControl
    {
        private readonly ChangingList<string> Columns = new() { "Name", "Label", "Type", "Visible", "Order" };
        private readonly FormService formService = new FormService();
        private readonly ChangingList<InputFormField> Fields = new();
        private readonly DataGridView grid;
        
        private bool Initiallized = false;


        public TableFieldsView()
        {
            InitializeComponent();
            this.grid = new DataGridView() { Dock = DockStyle.Fill };
            this.grid.SetupDefaults();
            this.grid.CellValueChanged += OnCellValueChanged;
            this.LayoutFill(grid);
            this.Fields.OnChanged += (s, e) => {
                if (Enabled)
                {                    
                    this.grid.SetInputRows(Fields, this.Columns.ToList());                    
                }
            };
            this.Initiallized = true;
        }


        public void Configure<TDbContext, TModel>()
            where TDbContext : DbContextBase
            where TModel : BaseEntity
        {
            this.Enabled = false;
            this.Columns.Set(new List<string>() { nameof(InputFormField.Name), nameof(InputFormField.Label), nameof(InputFormField.ValueType), nameof(InputFormField.Type) });
            this.grid.SetHeaders<InputFormField>(this.Columns);
            var fields = formService.GetFormFields<TModel>(
                typeof(TModel).GetOwnInputPropertyNames(), this.OnInput);
            this.Fields.Set(fields);
            this.grid.SetInputRows(this.Fields, this.Columns.ToList());
            this.Enabled = true;
        }


        private void OnCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (Initiallized)
            {
                grid.CellValueChanged += (s, e) => {
                    var columnIndex = e.ColumnIndex;
                    var rowIndex = e.RowIndex;
                    InputFormField field = Fields[rowIndex];                 
                    var property = Columns[columnIndex];

                    TextDataSetter.SetValue(field, property, $"{grid.Rows[rowIndex].Cells[columnIndex].Value}");
                    if(field.OnInput is not null)
                    {
                        field.OnInput($"{grid.Rows[rowIndex].Cells[columnIndex].Value}");
                    }


                    this.Info(this.Fields.ToJsonOnScreen());
                };
            }
        }
         
        private void OnInput(string property, string value)
        {
        }
    }


     


    public class FormService
    {
        public IEnumerable<FieldView> GetFormFields<TModel>(IEnumerable<string> properties = null, Action<string, string> oninput=null)
        {
            return GetFormFields(typeof(TModel), properties, oninput);
        }
        public IEnumerable<FieldView> GetFormFields(Type ptype, IEnumerable<string> properties = null, Action<string, string> oninput=null)
        {            
            if (ptype is null)
                throw new ArgumentNullException(nameof(ptype));
            if (properties is null)
                properties = ptype.GetInputPropertiesSorted();
            List<FieldView>  fields = new List<FieldView>();
            var target = ptype.New();
            foreach (var property in properties)
            {                
                string propertyName = property;
                FieldView pfield = GetFormField(target,ptype, property, val =>
                {
                    if (target is not null)
                        TextDataSetter.SetValue(target, property, val);
                    if(oninput is not null)
                        oninput(property, val);
                });
                fields.Add(pfield);                    
                              
            }            
            fields.Sort((p1, p2) =>
            {
                return
                p1.Order != p2.Order ? p1.Order - p2.Order :
                p1.Order == p2.Order ?
                p1.IsCollection && !p2.IsCollection ? -1 :
                !p1.IsCollection && p2.IsCollection ? 1 :
                    0 : 0;
            });
            return fields;
        }


        public FieldView GetFormField(object target, Type ptype, string property, Action<string> onchange)
        {

            this.Info($"{ptype.GetTypeName()}.{property}");
            FieldView pfield = new();
     

            pfield.Name = property;
            pfield.Help = ptype.GetPropertyHelp(property);
            pfield.Description = ptype.GetPropertyDescription(property);
            var pp = ptype.GetProperty(property);
            if (pp is null)
                throw new Exception($"Не найдено свойство {property} в типе {ptype.GetTypeName()}");
            pfield.IsCollection = pp.PropertyType.IsCollectionType();
            pfield.Order = 0;
            pfield.Icon = ptype.GetPropertyIcon(property);
            pfield.Label = ptype.GetPropertyLabel(property);
            pfield.Attributes = ptype.GetPropertyAttributes(property);
            pfield.OnInput = text =>
            {
                pfield.SetErrors("");
                if ((target is not null) && $"{target.GetValue(property)}" != text)
                {
                    TextDataSetter.SetValue(target, property, text);
                    onchange(text);
                }
            };
            pfield.Type = GetInputType(ptype, property);
            pfield.ValueType = GetValueType(ptype, property);
            pfield.Value = target.GetValue(property);
            pfield.CtrlType = ptype.GetPropertyControl(property);
            if (pfield.CtrlType is not null && pfield.CtrlType.GetTypeName() != nameof(ControlAttribute))
                pfield.Control = pfield.CtrlType.CreateControl(pfield);
            var orederStr = ptype.GetPropertyAttributeValue<InputOrderAttribute>(property, "0");
            pfield.Order = orederStr.ToInt();
            pfield.OnInput += (text) => { };

            return pfield;
        }

        private string GetValueType(Type ptype, string property)
        {
            return ptype.GetProperty(property).PropertyType.IsCollectionType() ?
                CommonUtils.ParseCollectionType(ptype.GetProperty(property).PropertyType) :
                ptype.GetProperty(property).PropertyType.GetTypeName();
        }


        public string GetInputType(Type type, string property)
        {
            var attribute = type.GetPropertyAttributes(property).Where(kv => kv.Key.ToType().IsExtends(typeof(BaseInputAttribute))).Select(kv => kv.Key.Replace("Input", "").Replace("Attribute", "").ToLower()).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(attribute))
            {
                var propertyType = type.GetProperty(property).PropertyType;
                if (propertyType.IsCollectionType())
                {
                    var elementType = propertyType.GetElementType();
                    return elementType.IsPrimitiveForType() ?
                        nameof(InputPrimitiveCollectionAttribute).Replace("Input", "").Replace("Attribute", "").ToLower() :
                        nameof(InputStructureCollectionAttribute).Replace("Input", "").Replace("Attribute", "").ToLower();
                }
                else
                {
                    if (!propertyType.IsPrimitiveForType())
                    {
                        return "custom";
                    }
                    else
                    {
                        switch (propertyType.GetTypeName())
                        {
                            case "System.Byte[]": return "upload";
                            case "Byte[]": return "upload";
                            case "String": return "text";
                            case "Boolean": return "checkbox";
                            case "System.String": return "text";
                            case "string": return "text";
                            case "int": return "number";
                            case "long": return "number";
                            case "float": return "number";
                            case "Nullable<System.Boolean>": return "number";
                            case "Double": return "number";
                            case "Nullable<System.Double>": return "number";
                            case "Int16": return "number";
                            case "Nullable<Int16>": return "number";
                            case "Int32": return "number";
                            case "Nullable<System.Int32>": return "number";
                            case "UInt16": return "number";
                            case "UInt32": return "number";
                            case "UInt64": return "number";
                            case "DateTime": return "datetime";
                            case "Nullable<System.DateTime>": return "datetime";
                            case "text": return "text";
                            default: return "text";
                        }
                    }
                }
            }
            else
            {
                return attribute;
            }
        }
    }
}
