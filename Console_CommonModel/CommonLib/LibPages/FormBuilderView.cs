using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.FormModule;

namespace WinFormsApp
{

    [Label("Конструктор форм")]
    public class FormBuilderView : AppViewBase, IFormBuilder
    {
        private readonly EditView editView;
        private readonly EditView target;

        public FormBuilderView()
        {
            var ev = new EditView();
            LayoutFill(this.editView = EditView.CreateEditViewBase(this.target=ev));
            this.MinWidth = this.MinHeight = 250;
            this.contextMenuServices.Add(this);
        }

        [NotInput]
        public List<string> GetInputTypeAttributes() => ServiceFactory.Get().GetTypesExtended<BaseInputAttribute>().Select(x => x.Name).ToList();


        [Label("Добавить кнопку")]
        public void AddActionButtonFormGroup(string label, Action<object> onClick)
        {
            var title = string.IsNullOrWhiteSpace(label) ? "Action" : label;
            var field = CreateField(title.ToLower().Replace(" ", "_"), title, "Text", title, text => onClick?.Invoke(text));
            target.FormFields.Add(field);
            target.scrollView.LayoutTop(field);
        }


        [Label("Добавить кнопки")]
        public void AddActionButtonsFormGroup(IDictionary<string, Action<object>> buttons)
        {
            if (buttons is null)
                return;
            foreach (var button in buttons)
            {
                AddActionButtonFormGroup(button.Key, button.Value);
            }
        }


        [Label("Добавить полу ввода")]
        public void AddInputTextFormGroup(string name, string title, string value, Action<string> onChange)
        {
            var field = CreateField(name, title, "Text", value, onChange);
            target.FormFields.Add(field);
            target.scrollView.LayoutTop(field);
        }

        [Label("Добавить галку")]
        public void AddCheckboxFormGroup(string name, string title, bool value, Action<bool> onChange)
        {
            var field = CreateField(name, title, "Bool", value, text => onChange?.Invoke(text.ToBool()));
            target.FormFields.Add(field);
            target.scrollView.LayoutTop(field);
        }


        [Label("Добавтиь список выбора")]
        public void AddSelectBoxFormGroup(string name, string title, string value, IEnumerable<string> options, Action<string> onChange)
        {
            var selectOptions = options?.ToList() ?? new List<string>();
            var field = CreateField(name, title, "Select", value, onChange);
            field.Attributes[nameof(InputSelectAttribute)] = string.Join(",", selectOptions);
            target.FormFields.Add(field);
            target.scrollView.LayoutTop(field);
        }


        [Label("Добавить выпадающий список")]
        public void AddSelectBoxFormGroup(string name, string title, int value, IDictionary<int, string> options, Action<int, string> onChange)
        {
            target.FormFields ??= new();
            var map = options ?? new Dictionary<int, string>();
            this.target.FormFields.Add(new InputFormField()
            {
                Name = name,
                Label = title,
                Type = "Select",
                Value = map.ContainsKey(value) ? map[value] : null,
                DataType = nameof(Int32),
                OnInput = text =>
                {
                    var selected = map.FirstOrDefault(kv => kv.Value == text);
                    if (!selected.Equals(default(KeyValuePair<int, string>)))
                        onChange?.Invoke(selected.Key, selected.Value);
                },
                Attributes = new()
                {
                    { nameof(InputSelectAttribute), string.Join(",", map.Values) }
                }
            });
        }


        [NotInput]
        private InputFormField CreateField(string name, string title, string type, object value, Action<string> onInput)
        {
            target.FormFields ??= new();
            return new InputFormField()
            {
                Name = string.IsNullOrWhiteSpace(name) ? "field" : name,
                Label = string.IsNullOrWhiteSpace(title) ? name : title,
                Type = type,
                Value = value,
                DataType = value?.GetType().GetTypeName() ?? nameof(String),
                OnInput = text => onInput?.Invoke(text)
            };
        }


        [NotInput]
        public MenuView CreateMenuViews(string name, IDictionary<string, Action> todos)
        {            
            MenuView menu = new() { Text = name };
            foreach (var action in todos)
            {
                Action onclick = action.Value;

                MenuView submenu = new() { Text = action.Key };
                submenu.OnClick += (s, e) => {
                    onclick();
                };
                menu.MenuItems.Add(submenu);
            }
            return menu;
        }


        [NotInput]
        internal MenuView CreateMenuView_FromActions(FormBuilderView form)
            => CreateMenuViews("Меню", GetActionMap(form));

        [NotInput]
        internal MenuView CreateMenuView_FromActions(string menuName, FormBuilderView form)
        {
            var menu = CreateMenuViews(menuName, GetActionMap(form));
            return menu;
        }
    }
}