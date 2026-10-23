
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Namotion.Reflection;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

/// <summary>
/// Выполняет установку значений свойств
/// </summary>
public static class TextDataSetterExtension
{
    public static void SetProperties(this object target, object model)
    {
        foreach (var property in model.GetInputProperties())
        {
            if(target.HasProperty(property))
            {
                try
                {
                    target.SetProperty(property, model.GetProperty(property));
                }
                catch(Exception ex)
                {
                    target.Warn($"Не удалось установить свойство {property} => {ex.Message}");
                }
            }            
        }        
    }

    public static void Set(this object target, IDictionary<string,object> attrs)
    {
        foreach (var item in attrs)
        {
            if(target.GetPropertyNames().Contains(item.Key))
                target.SetValue(item.Key, item.Value);
        }
            
    }
    public static void SetTextValues(this object target, IDictionary<string,string> attrs)
    {
        foreach (var kv in attrs)
        {           
            var name = kv.Key;         
            try
            {
                if (target.GetType().GetProperties().Any(p => p.Name is not null && p.Name.ToLower() == name.ToLower()))
                {
                    var value = kv.Value;
                    TextDataSetter.SetValue(target, name, value);
                }
            }    
            catch(Exception ex)
            {
                target.Warn($"Типу {target.GetTypeName()} не удалось установить значение свойства: {kv.Key} = {kv.Value}");
            }
        }
    }
     
}

public class TextDataSetter
{


    

    public static void SetValue(object target, string property, object value)
    {
        if(target is null)
            throw new ArgumentNullException("target");
        //target.Debug($"{property}={value}");
        try
        {
            if (target.IsExtends(typeof(Dictionary<string, object>)))
            {
                Dictionary<string, object> dict = (Dictionary<string, object>)target;
                dict[property] = value;
                return;
            }

            var p = target.GetType().GetProperty(property);
            var ptype = p.PropertyType;
            if(ptype.GetTypeName() == typeof(object).GetTypeName())
            {
                p.SetValue(target,value);
            }
            else if (Typing.IsDateTime(ptype))
            {
                DateTime? date = value.ToString().ToDate();
                p.SetValue(target, date);
            }
            else if (Typing.IsNumber(ptype))
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    p.SetValue(target, null);
                    /*if (Typing.IsNullable(ptype) && Typing.IsPrimitive(ptype) == false)
                    {
                        p.SetValue(target, null);
                    }
                    else
                    {
                        throw new Exception($"Свойство {property} не может хранить ссылку на null");
                    }*/
                }
                else
                {
                    string propertyType = Typing.ParsePropertyType(ptype);                 
                    switch (propertyType)
                    {

                        case "Single": { p.SetValue(target, System.Single.Parse(value.ToString())); break; }
                        case "System.Single": { p.SetValue(target, System.Single.Parse(value.ToString())); break; }
                        case "Double": { p.SetValue(target, System.Double.Parse(value.ToString())); break; }
                        case "System.Double": { p.SetValue(target, System.Double.Parse(value.ToString())); break; }
                        case "Decimal": { p.SetValue(target, System.Decimal.Parse(value.ToString())); break; }
                        case "System.Decimal": { p.SetValue(target, System.Decimal.Parse(value.ToString())); break; }
                        case "Int16": { p.SetValue(target, System.Int16.Parse(value.ToString())); break; }
                        case "System.Int16": { p.SetValue(target, System.Int16.Parse(value.ToString())); break; }
                        case "Int32": { p.SetValue(target, System.Int32.Parse(value.ToString())); break; }
                        case "System.Int32": { p.SetValue(target, System.Int32.Parse(value.ToString())); break; }
                        case "Nullable<Int32>": { p.SetValue(target, System.Int32.Parse(value.ToString())); break; }
                        case "Nullable<System.Int32>": { p.SetValue(target, System.Int32.Parse(value.ToString())); break; }
                        case "Int64": { p.SetValue(target, System.Int64.Parse(value.ToString())); break; }
                        case "System.Int64": { p.SetValue(target, System.Int64.Parse(value.ToString())); break; }
                        case "UInt16": { p.SetValue(target, System.UInt16.Parse(value.ToString())); break; }
                        case "System.UInt16": { p.SetValue(target, System.UInt16.Parse(value.ToString())); break; }
                        case "UInt32": { p.SetValue(target, System.UInt32.Parse(value.ToString())); break; }
                        case "System.UInt32": { p.SetValue(target, System.UInt32.Parse(value.ToString())); break; }
                        case "UInt64": { p.SetValue(target, System.UInt64.Parse(value.ToString())); break; }
                        case "System.UInt64": { p.SetValue(target, System.UInt64.Parse(value.ToString())); break; }
                        default:
                            throw new Exception($"Тип свойства {property} {propertyType} неподдеживается = {value}");
                    }
                }
                if (value != null && (value.GetType().Name == "Int64" || value.GetType().Name == "Int32"))
                {
                    value = Int32.Parse(value.ToString());
                }
            }
            else if (Typing.IsText(ptype))
            {
                p.SetValue(target, value.ToString());
            }
            else if (ptype.GetTypeName() == "Byte[]")
            {
                if(value is Byte[])
                {
                    p.SetValue(target, value);
                }
                else
                {
                    p.SetValue(target, null);
                }
            }
            else if (ptype.GetTypeName().StartsWith("Action"))
            {
                var argsCount = ptype.GetGenericArguments().Count();
                switch(argsCount)
                {
                    case 1: target.SetValue(property, (object p) => {
                        target.Info(value);
                        Expressions.Compile($"{value}", target);
                    }); 
                    break;
                    case 2:
                    target.SetValue(property, (object p, EventArgs evt) => {
                        if(target is AppViewBase view)
                        {
                            view.RunOnUiThread(() => {
                                target.Info(value);
                                Expressions.Compile($"{value}", target);
                            });
                        }
                    });
                    break;
                }
                

            }
            else

            {
                if (ptype.IsCollectionType())
                {
                    if(value is string)
                    {
                        try
                        {
                            var propertyValue = value.ToString().FromJson(ptype);
                            p.SetValue(property, propertyValue);
                        }
                        catch (Exception e)
                        {
                            e.Error($"{target.GetTypeName()}, Не удалось установить свойство {property} = {value}");
                        }
                    }
                    else if(value.GetType().IsCollectionType())
                    {
                        p.SetValue(target, value);
                    }
                    else
                    {
                        throw new Exception("Не реализован способ установки свойств типа коллекций через объект общего типа");
                    }
                    
                }
                else
                {
                    if(value is null)
                    {                        
                        p.SetValue(target, value);
                    }
                    else
                    {
                        if(String.IsNullOrWhiteSpace($"{value}"))
                        {
                            p.SetValue(target, null);
                        }
                        else
                        {
                            var pvalue = FromText(value, ptype.Name);
                            p.SetValue(target, pvalue);
                        }
                        
                    }
                    
                }
                    
            }
        }
        catch (Exception ex)
        {
            ex.Error($"Не удалось установить значение объекта {target.GetType().Name} для свойства: {property} => {value}: {ex.Message}");
            
            
            throw new Exception($"Не удалось установить значение объекта {target.GetType().Name} " +
                $"для свойства: {property} => {value}: {ex.Message}");
        }
    }

     

    public static object ToType(string textValue, Type propertyType)
    {
        if(!propertyType.IsPrimitiveForType())
        {
            return textValue.FromJson(propertyType);
        }
        else
        {
            return FromText(textValue, propertyType.GetNameOfType());
        }        
    }

    /// <summary> 
    /// </summary>
    public static object FromText(object value, string propertyType)
    {
        switch (propertyType)
        {
            case "Boolean": { return value.ToString().ToBool(); }
            case "System.Boolean": { return value.ToString().ToBool(); }
            case "String": { return value.ToString(); }
            case "System.String": { return value.ToString(); }
            case "Single": { return System.Single.Parse(value.ToString().Replace(".", ",")); }
            case "System.Single": { return System.Single.Parse(value.ToString().Replace(".",",")); }
            case "Double": { return System.Double.Parse(value.ToString()); }
            case "System.Double": { return System.Double.Parse(value.ToString()); }
            case "Decimal": { return System.Decimal.Parse(value.ToString()); }
            case "System.Decimal": { return System.Decimal.Parse(value.ToString()); }
            case "Int16": { return System.Int16.Parse(value.ToString()); }
            case "System.Int16": { return System.Int16.Parse(value.ToString()); }
            case "Int32": { return System.Int32.Parse(value.ToString()); }
            case "System.Int32": { return System.Int32.Parse(value.ToString()); }
            case "Nullable<Int32>": { return System.Int32.Parse(value.ToString()); }
            case "Nullable<System.Int32>": { return System.Int32.Parse(value.ToString()); }
            case "Int64": { return System.Int64.Parse(value.ToString()); }
            case "System.Int64": { return System.Int64.Parse(value.ToString()); }
            case "UInt16": { return System.UInt16.Parse(value.ToString()); }
            case "System.UInt16": { return System.UInt16.Parse(value.ToString()); }
            case "UInt32": { return System.UInt32.Parse(value.ToString()); }
            case "System.UInt32": { return System.UInt32.Parse(value.ToString()); }
            case "UInt64": { return System.UInt64.Parse(value.ToString()); }
            case "System.UInt64": { return System.UInt64.Parse(value.ToString()); }
            case "Type": { return value.ToString().ToType();  }
            default:
                return value.ToString().FromJson(propertyType.ToType());
                


                //throw new Exception($"Тип  {propertyType} неподерживается");
        }
    }

    public static void SetValues(object result, IDictionary<string, object> data) 
    {
        foreach(var name in result.GetInputProperties())
        {
            TextDataSetter.SetValue(result, name, data.ContainsKey(name)? data[name]: null);
        }
    }
     
}
/// <summary>
/// Выполняет установку значений свойств
/// </summary>
public class Setter
{
    public static object FromText(object value,string propertyType)
    {
        if (value == null)
            return null;
        switch (propertyType)
        {
            case "Boolean": {
                    var sv = value.ToString();
                    if (sv.ToLower() == "нет") return false;
                    if (sv.ToLower() == "no") return false;
                    if (sv.ToLower() == "0") return false;
                    if (sv.ToLower() == "yes") return true;
                    if (sv.ToLower() == "да") return true;
                    if (sv.ToLower() == "1") return true;
                    return value.ToString().ToBool();
            }
            case "System.Boolean": { return value.ToString().ToBool(); }
            case "String": { return value.ToString(); }
            case "System.String": { return value.ToString(); }
            case "Single": { return System.Single.Parse(value.ToString()); }
            case "System.Single": { return System.Single.Parse(value.ToString()); }
            case "Nullable<Single>": { return System.Single.Parse(value.ToString()); }
            case "Nullable<System.Single>": { return System.Single.Parse(value.ToString()); }
            case "Double": { return System.Double.Parse(value.ToString()); }
            case "System.Double": { return System.Double.Parse(value.ToString()); }
            case "Decimal": { return System.Decimal.Parse(value.ToString()); }
            case "System.Decimal": { return System.Decimal.Parse(value.ToString()); }
            case "Int16": { return System.Int16.Parse(value.ToString()); }
            case "System.Int16": { return System.Int16.Parse(value.ToString()); }
            case "Int32": { return System.Int32.Parse(value.ToString()); }
            case "System.Int32": { return System.Int32.Parse(value.ToString()); }
            case "Nullable<Int32>": { return System.Int32.Parse(value.ToString()); }
            case "Nullable<System.Int32>": { return System.Int32.Parse(value.ToString()); }
            case "Int64": { return System.Int64.Parse(value.ToString()); }
            case "System.Int64": { return System.Int64.Parse(value.ToString()); }
            case "UInt16": { return System.UInt16.Parse(value.ToString()); }
            case "System.UInt16": { return System.UInt16.Parse(value.ToString()); }
            case "UInt32": { return System.UInt32.Parse(value.ToString()); }
            case "System.UInt32": { return System.UInt32.Parse(value.ToString()); }
            case "UInt64": { return System.UInt64.Parse(value.ToString()); }
            case "System.UInt64": { return System.UInt64.Parse(value.ToString()); }
            default:
                throw new Exception($"Тип  {propertyType} не поддрживается");
        }
    }
    public static void SetValue( object target, string property, object value )
    {
        var p = target.GetType().GetProperty(property);
        if (Typing.IsDateTime(p))
        {
            string text = value == null ? "" :value.ToString();
            DateTime? date = text.ToDate();
            p.SetValue(target, date);
        }
        else if (Typing.IsNumber(p))
        {
            if( value == null || string.IsNullOrEmpty(value.ToString()))
            {
                /*if(Typing.IsNullable(p) && Typing.IsPrimitive(p.PropertyType)==false)
                {
                    p.SetValue(target, null);
                }
                else
                {
                    throw new Exception($"Свойство {property} не может хранить ссылку на null");
                }*/

                p.SetValue(target, null);
            }
            else
            {
                string propertyType = Typing.ParsePropertyType(p.PropertyType);
                switch (propertyType)
                {
                  
                    case "Single": { p.SetValue(target, System.Single.Parse(value.ToString())); break; }
                    case "System.Single": { p.SetValue(target, System.Single.Parse(value.ToString())); break; }
                    case "Double": { p.SetValue(target, System.Double.Parse(value.ToString())); break; }
                    case "System.Double": { p.SetValue(target, System.Double.Parse(value.ToString())); break; }
                    case "Decimal": { p.SetValue(target, System.Decimal.Parse(value.ToString())); break; }
                    case "System.Decimal": { p.SetValue(target, System.Decimal.Parse(value.ToString())); break; }
                    case "Int16": { p.SetValue(target, System.Int16.Parse(value.ToString())); break; }
                    case "System.Int16": { p.SetValue(target, System.Int16.Parse(value.ToString())); break; }
                    case "Int32": { p.SetValue(target, System.Int32.Parse(value.ToString())); break; }
                    case "System.Int32": { p.SetValue(target, System.Int32.Parse(value.ToString())); break; }
                    case "Nullable<Int32>": { p.SetValue(target, System.Int32.Parse(value.ToString())); break; }
                    case "Nullable<System.Int32>": { p.SetValue(target, System.Int32.Parse(value.ToString())); break; }
                    case "Int64": { p.SetValue(target, System.Int64.Parse(value.ToString())); break; }
                    case "System.Int64": { p.SetValue(target, System.Int64.Parse(value.ToString())); break; }
                    case "UInt16": { p.SetValue(target, System.UInt16.Parse(value.ToString())); break; }
                    case "System.UInt16": { p.SetValue(target, System.UInt16.Parse(value.ToString())); break; }
                    case "UInt32": { p.SetValue(target, System.UInt32.Parse(value.ToString())); break; }
                    case "System.UInt32": { p.SetValue(target, System.UInt32.Parse(value.ToString())); break; }
                    case "UInt64": { p.SetValue(target, System.UInt64.Parse(value.ToString())); break; }
                    case "System.UInt64": { p.SetValue(target, System.UInt64.Parse(value.ToString())); break; }
                    default:
                        throw new Exception($"Тип свойства {property} {propertyType} неподдрживается");
                }


            }
            /*if (value != null && (value.GetType().Name == "Int64" || propertyTypeName == "Int32"))
            {
                value = Int32.Parse(value.ToString());
            }*/
        }
        else if (Typing.IsText(p)) 
        {
            p.SetValue(target, value.ToString());
        }
        else
        {
            if (Typing.IsCollectionType(p.PropertyType) == false)
            {
                var pvalue = FromText(value, p.PropertyType.GetTypeName());
                p.SetValue(target, pvalue);
            }
            
        }

    }
}