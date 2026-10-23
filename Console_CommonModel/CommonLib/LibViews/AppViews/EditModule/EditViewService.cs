
using Console_CommonModel.CommonLib.LibAttributes.AttributesInput;
using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.EditModule
{
    internal class EditViewService
    {
        internal EditView CreateEditView(MethodModel actionModel, Action<bool, object> oncompleted)
        {
            var view = new EditView();
            if (actionModel.Parameters is null)
                actionModel.Parameters = new();
            var factory = new FormFieldFactory();
            var target = new Dictionary<string, object>();
            foreach (var par in actionModel.Parameters)
            {
                var name = par.Name;
                var type = par.Type;
                var attributes = new Dictionary<string, string>(par.Attributes.Select(a => new KeyValuePair<string, string>(a.Name, a.Value)));
                //var ctrl = AttrsUtil.GetControl(attributes);
                //factory.CreateParameterField(name, type, null, attributes, ctrl, text => { });
                var field = createFieldTemp(view, par, name, attributes);
                field.ValidateOnInput = true;
                field.Label = par.Label;
                field.Icon = par.Icon;
                field.Description = par.Description;
                target.SetValue(field.Name, field.Value);
                field.OnInput += (text) =>
                {
                    view.PutAction("Обновление данных", () =>
                    {
                        TextDataSetter.SetValue(target, name, text);
                        if (field.ValidateOnInput)
                        {
                            field.Errors.Clear();
                            foreach (var kv in field.Attributes)
                            {

                                var keyType = kv.Key.ToType();
                                if (keyType.IsImplements(typeof(MyValidation)))
                                {
                                    MyValidation validation = keyType.Create<MyValidation>(new object[] { kv.Value });
                                    string validationResult = validation.Validate(target, name, text);
                                    if (validationResult != null)
                                    {
                                        field.Errors.Add(validationResult);
                                    }
                                }
                            }
                        }
                    });
                };
            }
            view.UpdateEditView();
            /*Action<bool, AppViewBase> toremove = (s, v) =>
            {
                if (oncompleted is not null)
                    oncompleted(s, target);
            };
            ClearBindings = () => { this.OnEditCompleted -= toremove; };*/
            view.OnEditCompleted += (s, v) =>
            {
                oncompleted(s, target);
            };

            var button = new ButtonView();
            view.scrollView.LayoutTop(button);
            var titleText = view.scrollView.CreateText(actionModel.Label);
            view.scrollView.LayoutTop(titleText);
            var controlsList = view.scrollView.Controls.ToList();
            controlsList.Reverse();
            view.scrollView.Controls.Clear();
            foreach (var ctrl in controlsList)
            {
                view.scrollView.Add(ctrl);
            }
            view.scrollView.Remove(button);
            return view;
        }

        private InputFormField createFieldTemp(EditView view, MethodParameterModel par, string name, Dictionary<string, string> attributes)
        {
            var formFile = new InputFormField();
            formFile.Value = par.Value;
            formFile.Name = par.Name;
            formFile.Help = attributes.ContainsKey(nameof(HelpMessageAttribute)) ? attributes[nameof(HelpMessageAttribute)] : "";
            formFile.Description = attributes.ContainsKey(nameof(DescriptionAttribute)) ? attributes[nameof(DescriptionAttribute)] : "";
            formFile.Label = attributes.ContainsKey(nameof(LabelAttribute)) ? attributes[nameof(LabelAttribute)] : name;
            formFile.Icon = attributes.ContainsKey(nameof(IconAttribute)) ? attributes[nameof(IconAttribute)] : name;
            formFile.DataType = par.Type;
            formFile.IsCollection = par.Type.ToType().IsCollectionType();
            formFile.Order = 0;
            formFile.Attributes = attributes;
            formFile.OnInput = text =>
            {

            };
            formFile.Type = GetInputType(par.Type.ToType(), attributes);
            formFile.ValueType = formFile.IsCollection ?
                ParseItemType(formFile.DataType) : null;


            formFile.CtrlType = AttrsUtil.GetControl(attributes);
            if (formFile.CtrlType is not null && formFile.CtrlType.GetTypeName() != nameof(ControlAttribute))
                formFile.Control = (AppViewBase)formFile.CtrlType.CreateControl(formFile);
            var orederStr = attributes.ContainsKey(nameof(InputOrderAttribute)) ? attributes[nameof(InputOrderAttribute)] : "0";

            var bindingValue = attributes.ContainsKey(nameof(InputBindingAttribute)) ? attributes[nameof(InputBindingAttribute)] : null;

            formFile.Order = orederStr.ToInt();
            formFile.OnInput += (text) => { };
            if (formFile.Type.Contains("collection") && formFile.Type.Contains("<"))
            {
                var a = formFile.Attributes.Select(kv => kv.Key).FirstOrDefault(kv => kv.ToType().IsExtends(typeof(InputStructureCollectionAttribute)));
                var b = formFile.Attributes.Select(kv => kv.Key).FirstOrDefault(kv => kv.ToType().IsExtends(typeof(InputPrimitiveCollectionAttribute)));
                string typeName = a is not null ? a : b;
                typeName = typeName.Substring(typeName.IndexOf("<"));
                typeName = typeName.Substring(0, typeName.IndexOf(">"));
                typeName = typeName.Replace("<", "").Replace(">", "");
                formFile.ValueType = typeName;

            }
            view.FormFields.Add(formFile);
            return formFile;
        }

        private string ParseItemType(string dataType)
        {
            if (dataType.IndexOf("<") > 0 && dataType.LastIndexOf(">") > 0)
            {
                dataType = dataType.Substring(dataType.IndexOf("<") + 1);
                dataType = dataType.Substring(0, dataType.LastIndexOf(">"));
                return dataType;
            }
            else
            {
                this.Warn(dataType);
                return dataType;
            }

        }

        private string GetInputType(Type propertyType, Dictionary<string, string> attrs)
        {
            var key = attrs.Keys.FirstOrDefault(key => key.ToType().IsExtends(typeof(BaseInputAttribute)));
            if (key != null)
            {
                var type = key.ToType();
                var attrValue = attrs[key];
                if (string.IsNullOrWhiteSpace(attrValue))
                    type.New<BaseInputAttribute>(Array.Empty<object>());
                else
                    type.New<BaseInputAttribute>(new object[] { attrValue });
                return key.Replace("Input", "").Replace("Attribute", "").ToLower();
            }

            if (propertyType.IsCollectionType())
            {
                var gargs = propertyType.GetGenericArguments();
                var elementType = gargs.Length > 0 ? gargs[0] :
                    Typing.ParseCollectionType(propertyType).ToType();
                this.Info($"{elementType.GetTypeName()}");
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

    }
}
