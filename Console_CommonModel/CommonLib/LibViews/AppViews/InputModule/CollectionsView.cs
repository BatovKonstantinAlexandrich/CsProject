
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.LineModule;
using Console_CommonModel.CommonLib.LibViews.Others;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Console_CommonModel.CommonLib.LibViews.SharedViewCollections.FormView;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.InputModule
{
    [Label("Управление коллекцией объектов")]
    public class CollectionsView : AppViewBase

    {
        public string Type = "email";
        public Dictionary<string, string> Attributes = new() {
            { nameof(InputEmailAttribute), "" }
        };


        public Action<int, object> OnValueChanged = (i, p) => { };
        public Action<int, object> OnAdded = (i, p) => { };
        public Action<int, object> OnRemoved = (i, p) => { };

        ChangingList<LineView> selected = new();
        public ChangingList<string> values = new();
        ChangingList<LineView> lines = new();

        public CollectionsView()
        {
            MinHeight = 280;
            MinWidth = 280;
            Width = 320;
            Height = 300;

            AppViewBase pane = this;
            ScrollView scroll = new();
            ToolsView tools = InitTools();

            MinHeight = 300;
            ContextMenu.Items.OnAdded((s, p) => scroll.ContextMenu.Items.Add((ContextView)p));
            ContextMenu.Items.OnRemoved((s, p) => scroll.ContextMenu.Items.Remove((ContextView)p));

            pane.LayoutFill(scroll);
            pane.LayoutTop(tools);

            lines.OnAdded((i, p) =>
            {
                var button = (LineView)p;
                var controls = scroll.Controls.ToList();
                scroll.Controls.Clear();
                scroll.Controls.Add(button);
                foreach (var ctrl in controls)
                {
                    scroll.Controls.Add(ctrl);
                }
            });
            lines.OnRemoved((i, p) => { scroll.Remove((LineView)p); });
            values.OnAdded((i, p) =>
            {
                try
                {
                    OnAdded(i, $"{p}");
                    CreateEditView(p);
                }
                catch (Exception ex)
                {
                    this.Error($"Исключение при обработки события OnAdded() => {ex.Message}");
                }
            });
            values.OnRemoved((i, p) =>
            {
                lines.Remove(lines[i]);
            });
            MinHeight = 320;
            MinWidth = 320;
        }

        private void CreateEditView(object p)
        {
            //var erline = new LineView() { AutoSize = false, MinHeight = 50 };
            var line = new LineView() { AutoSize = false, Dock = "Top", MinHeight = 50 };
            line.Add(new CheckboxView()
            {
                Text = "",
                Padding = 10,
                MinWidth = 50,
                MinHeight = 50,
                Width = 50,
                Height = 50,
                AutoSize = false,
                OnCheckedChanged = (s, p) =>
                {
                    if (selected.Contains(line))
                    {
                        selected.Remove(line);
                    }
                    else
                    {
                        selected.Add(line);
                    }
                }
            }, false);
            /*erline.Add(new AppViewBase()
            {              
                MinWidth = 50,
                MinHeight = 20,
                Width = 50,
                Height = 20,
                AutoSize = false
            }, false);*/
            var target = new Dictionary<string, object>();
            ChangingList<string> errors = new();

            /*line.Add((AppViewBase)
            new InputFormFieldService().CreateControl(new InputFormField()
            {
                Label = "",
                Errors = errors,
                Type = this.Type,
                Attributes = this.Attributes,
                OnInput = text =>
                {
                    foreach (var kv in Attributes)
                    {
                        var keyType = kv.Key.ToType();
                        if (keyType.IsImplements(typeof(MyValidation)))
                        {
                            MyValidation validation = keyType.Create<MyValidation>(new object[] { kv.Value });
                            string validationResult = validation.Validate(target, "item_"+this.values.Count(), text);
                            if (validationResult != null)
                            {
                                errors.Add(validationResult);
                            }
                        }
                    }
                    
                    OnChanged(lines.IndexOf(line), values[lines.IndexOf(line)] = text);
                    line.Info(values.ToJsonOnScreen());
                }
            }), true);*/

            //erline.MinHeight = 22;
            //erline.Height = 22;
            errors.OnChanged += (p, evt) =>
            {
                /*(erline.Controls.Clear();
                foreach (var error in errors)
                {
                    erline.Add(new TextView() { Text = error, Color = "#"+ColorTranslator.ToHtml(System.Drawing.Color.Red), FontSize = 10, MinHeight=20, Height=20 }, true);
                }*/
            };
            var service = new InputFormFieldService();
            var field = new InputFormField();
            field.Type = Type;
            field.Value = p;

            var input = (AppViewBase)service.CreateControl(field);
            //var input = new TextBoxView()
            //{
            //    MinHeight = 50,
            //    Dock = "Fill",
            //    Text = $"{p}",
            //    AutoSize = false,
            //    OnTextChanged = text =>
            //    {
            //        OnValueChanged(lines.IndexOf(line), values[lines.IndexOf(line)] = text);
            //        line.Info(values.ToJsonOnScreen());
            //    }
            //};
            line.Add(input, true);
            lines.Add(line);
            field.OnInput += (text) =>
            {
                if (values[lines.IndexOf(line)] != text)
                {
                    OnValueChanged(lines.IndexOf(line), values[lines.IndexOf(line)] = text);
                    line.Info(values.ToJsonOnScreen());
                }
            };
            //lines.Add(erline);
        }

        private ToolsView InitTools()
        {
            var tools = new ToolsView();
            tools.tools.Add(new ToolsItemButtonView()
            {
                Text = "добавить",
                OnClick = (s, e) =>
                {
                    values.Add($"{values.Count() + 1}");
                }
            });
            ToolsItemButtonView delete = null;
            tools.tools.Add(delete = new ToolsItemButtonView()
            {
                Text = "удалить",
                OnClick = (s, e) =>
                {
                    foreach (var p in selected)
                    {
                        OnRemoved(lines.IndexOf(p), values[lines.IndexOf(p)]);
                        values.RemoveAt(lines.IndexOf(p));
                    }
                    selected.Clear();
                }
            });
            delete.Enabled = false;
            selected.OnChanged += (s, e) => delete.Enabled = selected.Count() > 0;
            return tools;
        }

    }





}
