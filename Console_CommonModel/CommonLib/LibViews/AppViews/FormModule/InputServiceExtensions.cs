using System;
using System.Collections.Generic;
using System.Linq;


public static class InputServiceExtensions
{
    public static FormModel ToFormModel(this object target)
    {
        var form = new FormModel(target);
        form.FormFields = target.GetType().GetFormFields().ToList();
        return form;
    }
    public static FormViewModel ToFormViewModel(this object target)
    {
        var form = new FormViewModel(target);
        return form;
    }

    public static IEnumerable<InputFormField> GetFormFields(this Type ptype)
        => ptype.GetFormFields(ptype.GetInputProperties());


    public static IEnumerable<InputFormField> GetFormFields(this Type ptype, IEnumerable<string> properties, Action<string, string> oninput=null)
    {
        var fields = new List<InputFormField>();
        var started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
         
        InputFormField pfield = null;
        fields = new List<InputFormField>();

        foreach (var property in properties)
        {
            pfield = GetFormField(ptype, property, val => { if (oninput is not null) oninput(property, val); });
            fields.Add(pfield);
        }
        long ms = DateTimeOffset.Now.ToUnixTimeMilliseconds() - started;
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


    public static InputFormField GetFormField(Type ptype, string property, Action<string> onchange)
    {
        InputFormField pfield;
        InputFormField p = null;
        p = pfield = new InputFormField() { };
        pfield.Name = property;
        pfield.Help = ptype.GetPropertyHelp(property);
        pfield.Description = ptype.GetPropertyDescription(property);
        pfield.IsCollection = ptype.GetProperty(property).PropertyType.IsCollectionType();
        pfield.Order = 0;
        pfield.Icon = ptype.GetPropertyIcon(property);
        pfield.Label = ptype.GetPropertyLabel(property);
        pfield.Attributes = ptype.GetPropertyAttributes(property);
        pfield.OnInput = text =>
        {
        
            /*if (target.GetValue(p.Name).ToString() != text)
            {
                TextDataSetter.SetValue(target, p.Name, text);
            }*/
            onchange(text);
        };
        pfield.Type = GetInputType(ptype, property);
        pfield.ValueType = GetValueType(ptype, property);
        //pfield.Value = target.GetValue(property);
        pfield.CtrlType = ptype.GetPropertyControl(property);
        if (pfield.CtrlType is not null && pfield.CtrlType.GetTypeName() != nameof(ControlAttribute))
            p.Control = (AppViewBase)pfield.CtrlType.CreateControl(p);
        var orederStr = ptype.GetPropertyAttributeValue<InputOrderAttribute>(property, "0");
        pfield.Order = orederStr.ToInt();
        pfield.OnInput += (text) => { };
        return pfield;
    }


    private static string GetValueType(Type ptype, string property)
    {
        return ptype.GetProperty(property).PropertyType.IsCollectionType() ?
            CommonUtils.ParseCollectionType(ptype.GetProperty(property).PropertyType) :
            ptype.GetProperty(property).PropertyType.GetTypeName();
    }


    public static string GetInputType(Type type, string property)
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
