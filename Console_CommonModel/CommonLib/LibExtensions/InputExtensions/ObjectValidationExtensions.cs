using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Drawing.Design;

using Console_CommonModel.CommonLib.LibViews.AppViews.XmlModule;
using Console_CommonModel.CommonLib.LibDesigner.DesignerServices;
using Console_CommonModel.CommonLib.LibAttributes.AttributesInput;
using Microsoft.EntityFrameworkCore;

public static class ObjectValidationExtensions
{

    public static void EnsureAgumentsIsValid(this MethodBase methodInfo, object a1, object a2, object a3, object a4)
        => methodInfo.EnsureIsValid(new object[1] { new object[] { a1, a2, a3, a4 } });
    public static void EnsureAgumentsIsValid(this MethodBase methodInfo, object a1, object a2, object a3)
        => methodInfo.EnsureIsValid(new object[1] { new object[] { a1, a2,a3 } });
    public static void EnsureAgumentsIsValid(this MethodBase methodInfo, object a1, object a2)
        => methodInfo.EnsureIsValid(new object[1] { new object[] { a1, a2 } });
    public static void EnsureAgumentsIsValid(this MethodBase methodInfo, object a1)
    => methodInfo.EnsureIsValid(new object[1] { new object[] { a1 } });
    public static void EnsureAgumentsIsValid(this MethodBase methodInfo, object[] argument)
        => methodInfo.EnsureIsValid(argument );
    public static void EnsureIsValid(this MethodBase methodInfo, object[] arguments)
    {
        var validationResults = methodInfo.Validate(arguments);
        if (validationResults != null && validationResults.Count() > 0 && validationResults.Any(res => res.Value.Count() > 0))
        {
            
            throw new ArgumentException(
                $"Фактические аргументы метода {methodInfo.Name} " +
                $"не валидны\n{validationResults.ToJsonOnScreen()}\n");
        }
    }
    public static IEnumerable<object> GetArgumentsFromDictionary(this MethodBase methodInfo, Dictionary<string, object> arguments)
    {
        var parNames = methodInfo.GetParameters().Select(p => p.Name).ToList();
        var argsInvoke = parNames.Select(name => arguments.ContainsKey(name) ? arguments[name] : null).ToArray();
        return argsInvoke;
    }
    public static IDictionary<string, IEnumerable<string>> Validate(this MethodBase methodInfo, Dictionary<string, object> arguments)
    {
        var parNames = methodInfo.GetParameters().Select(p => p.Name).ToList();
        var argsInvoke = parNames.Select(name => arguments.ContainsKey(name) ? arguments[name] : null).ToArray();
        return methodInfo.Validate(argsInvoke);
    }

    public static IDictionary<string, IEnumerable<string>> Validate(this MethodBase methodInfo, object[] arguments)
    {
        if(methodInfo.IsConstructor)
        {
            methodInfo.Info($"{methodInfo.DeclaringType.GetTypeName()} {arguments.ToJson()}");
        }
        else
        {
            methodInfo.Info($"{methodInfo.DeclaringType.GetTypeName()}.{methodInfo.Name} {arguments.ToJson()}");
        }
        
        IDictionary<string, IEnumerable<string>> result = new Dictionary<string, IEnumerable<string>>();

        int argumentIndex = 0;
        foreach (var parameter in methodInfo.GetParameters())
        {

            var validationAttributesData = parameter.GetCustomAttributesData()
                    .Where(data => data.AttributeType.IsExtends(typeof(ValidationAttribute)));
            IEnumerable<ValidationAttribute> validationAttributes = validationAttributesData
                    .Select(data =>
                    {
                        var types = data.ConstructorArguments.Select(a => a.ArgumentType).ToArray();

                        Type attributeType = data.AttributeType;
                        var constructor = attributeType.GetConstructor(types.ToArray());
                        object instance = constructor.Invoke(data.ConstructorArguments.Select(a => a.Value).ToArray());
                        return (ValidationAttribute)instance;

                    });

            string name = parameter.Name;
            string type = parameter.ParameterType.GetTypeName();
            object value = arguments[argumentIndex++];

            var results = new List<string>();
            foreach (ValidationAttribute attribute in validationAttributes)
            {

                if (attribute.IsValid(value) == false)
                    results.Add(attribute.GetType().Name);
            }
            result[parameter.Name] = results;
        }

        return result;

    }



    /// <summary>
    /// true, РµСЃР»Рё С‚РёРї РѕР±СЉРµРєС‚Р° РЅР°СЃР»РµРґСѓРµС‚СЃСЏ РѕС‚ Р·Р°РґР°РЅРЅРѕРіРѕ С‚РёРїР° РїРѕ РёРјРµРЅРё
    /// </summary>
    /// <param name="baseType"></param>
    /// <returns></returns>
    public static bool IsExtendedFrom(this object target, string baseType)
    {
        Type typeOfObject = new object().GetType();
        Type p = target.GetType();
        while (p != typeOfObject)
        {
            if (p.Name == baseType)
            {
                return true;
            }
            p = p.BaseType;
        }
        return false;
    }

    public static bool IsExtendedFrom(this object target, Type ptype)
    {
        var baseType = ptype.GetTypeName();
        Type typeOfObject = new object().GetType();
        Type p = target.GetType();
        while (p != typeOfObject)
        {
            if (p.Name == baseType)
            {
                return true;
            }
            p = p.BaseType;
        }
        return false;
    }





    public static Dictionary<string, List<string>> Validate(this object target, string[] keys)
    {

        Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
        foreach (var property in target.GetType().GetProperties())
        {
            string key = property.Name;

            if (IsPrimitive(property.PropertyType))
            {
                List<string> errors = target.Validate(key);
                if (errors.Count > 0)
                {
                    result[key] = errors;
                }

            }
        }
        Dictionary<string, List<string>> optional = new Dictionary<string, List<string>>();
        var validateOptional = target.GetType().GetMethod("ValidateOptional");
        if (validateOptional is not null)
        {
            optional = (Dictionary<string, List<string>>)validateOptional.Invoke(target, new object[0]);
        }
        foreach (var p in optional)
        {
            if (result.ContainsKey(p.Key))
            {
                result[p.Key].AddRange(optional[p.Key]);
            }
            else
            {
                result[p.Key] = optional[p.Key];
            }
        }
        return result;
    }



    /// <summary>
    /// Р’Р°Р»РёРґР°С†РёСЏ СЃРІРѕР№СЃС‚РІР° РѕР±СЉСЏРІР»РµРЅРЅРѕРіРѕ Р·Р°РґР°РЅРЅС‹Рј РєР»СЋС‡РµРј
    /// </summary>
    /// <param name="key">РєР»СЋС‡ СЃРІРѕР№СЃС‚РІР°</param>
    /// <returns></returns>
    public static List<string> Validate(this object target, string key)
    {
        object value = GetValue(target, key);
        var attributes = target.GetType().GetPropertyAttributes(key);
        return target.Validate(key, value, attributes);
    }
    public static List<string> Validate(this object target, string key, DbContext dbContext)
    {
        object value = GetValue(target, key);
        var attributes = target.GetType().GetPropertyAttributes(key);
        return target.Validate(key, value, attributes, dbContext);
    }
    public static List<string> Validate(this object target, string key, object value, Dictionary<string, string> attributes)
    {
        List<string> errors = new List<string>();
        foreach (var kv in attributes)
        {
            var keyType = kv.Key.ToType();
            if (keyType.IsImplements(typeof(MyValidation)))
            {
                MyValidation validation = keyType.Create<MyValidation>(new object[] { kv.Value });
                string validationResult = validation.Validate(target, key, value);
                if (validationResult != null)
                {
                    errors.Add(validationResult);
                }
            }
        }
        return errors;
    }
    public static List<string> Validate(this object target, string key, object value, Dictionary<string, string> attributes, DbContext dbContext)
    {
        List<string> errors = new List<string>();
        foreach (var kv in attributes)
        {
            var keyType = kv.Key.ToType();
            
            if (keyType.IsExtends(typeof(InTransactionValidationAttribute)))
            {
                InTransactionValidationAttribute validation = keyType.Create<InTransactionValidationAttribute>(new object[] { kv.Value });
                string validationResult = validation.Validate(target, key, value, dbContext);
                if (validationResult != null)
                {
                    errors.Add(validationResult);
                }
            }
            else if (keyType.IsImplements(typeof(MyValidation)))
            {
                MyValidation validation = keyType.Create<MyValidation>(new object[] { kv.Value });
                string validationResult = validation.Validate(target, key, value);
                if (validationResult != null)
                {
                    errors.Add(validationResult);
                }
            }
        }
        return errors;
    }
    public static List<string> Validate(this object target, string key, object value, Dictionary<string, string> attributes, bool inTransaction)
    {
        List<string> errors = new List<string>();
        foreach (var kv in attributes)
        {
            var keyType = kv.Key.ToType();
            if (keyType.IsImplements(typeof(MyValidation)))
            {
                if(inTransaction && keyType.IsExtends(typeof(InTransactionValidationAttribute)))
                {
                    continue;
                }
                MyValidation validation = keyType.Create<MyValidation>(new object[] { kv.Value });
                string validationResult = validation.Validate(target, key, value);
                if (validationResult != null)
                {
                    errors.Add(validationResult);
                }
            }
        }
        return errors;
    }


    public static void EnsureIsValide(this object target)
    {
        target.InfoTime($"Валидация {target.GetTypeName()}", () => {
            var r = target.Validate();
            if (r.Count() > 0)
            {
                var invalideData = target.GetValues(r.Where(kv => kv.Value is not null && kv.Value.Count() > 0).Select(kv => kv.Key));

                string message = target.GetType().Name + " " + target.ToJsonOnScreen() + $" не валидный: \n{invalideData.ToJsonOnScreen()}\n" + r.ToJson();
                throw new ValidationException(message);
            }
        });
    }

    public static void EnsureIsValide(this object target, DbContext dbContext)
    {
        target.InfoTime($"Валидация {target.GetTypeName()}", () => {
            var r = target.Validate(dbContext);
            if (r.Count() > 0)
            {
                var invalideData = target.GetValues(r.Where(kv => kv.Value is not null && kv.Value.Count() > 0).Select(kv => kv.Key));

                string message = target.GetType().Name + " " + target.ToJsonOnScreen() + $" не валидный: \n{invalideData.ToJsonOnScreen()}\n" + r.ToJson();
                throw new ValidationException(message);
            }
        });
    }

    public static void EnsurePropertyIsValide(this object target, string property)
    {
        var r = target.Validate(property);
        if (r.Count() > 0)
        {
            string message = $"Свойство {property} объекта " + target.GetType().Name + " не валидно: \n" + r.ToJson();
            throw new ValidationException(message);
        }
    }

    public static IEnumerable<ValidationResult> Validate(this object target, ValidationContext validationContext)
    {
        List<ValidationResult> results = new List<ValidationResult>();
        Dictionary<string, List<string>> errors = target.Validate();
        foreach (var errorEntry in errors)
        {
            string propertyName = errorEntry.Key;
            List<string> propertyErrors = errorEntry.Value;
            foreach (string propertyError in propertyErrors)
            {
                ValidationResult result = new ValidationResult(propertyError, new List<string>() { propertyName });
                results.Add(result);
            }
        }
        return results;
    }


    /// <summary>
    /// РџРѕР»СѓС‡РµРЅРёРµ Р·РЅР°С‡РµРЅРёСЏ СЃРІРѕР№СЃС‚РІР° Р·Р°РґР°РЅРЅС‹Рј РєР»СЋС‡РµРј
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static object GetValue(this object target, string name)
    {
        try
        {
            
            if (target is null)
                return null;
            if (name is null)
                throw new ArgumentNullException("target");
            PropertyInfo propertyInfo = target.GetType().GetProperties().FirstOrDefault(p => p.Name == name);
            FieldInfo fieldInfo = target.GetType().GetField(name);
            if (name == "ValidationSuccess")
            {
                
            }
            if(propertyInfo is null && fieldInfo is null)
            {
                return target.Compile(name);
            }
            return
                fieldInfo != null ? fieldInfo.GetValue(target) :
                propertyInfo != null ? propertyInfo.GetValue(target) :
                throw new Exception($"Свойство {name} Не найдено в {target.GetType()}");
        }
        catch(Exception ex) 
        {
            if (target is null)
                return null;
            if (name is null)
                throw new ArgumentNullException("target");
            PropertyInfo propertyInfo = target.GetType().GetProperties().FirstOrDefault(p => p?.Name == name);
            var val = propertyInfo.GetValue(target);
            FieldInfo fieldInfo = target.GetType().GetField(name);
            target.Error($"Не удалось получить свойство: {name} типу {target.GetTypeName()} => {ex.Message}");
            //throw new Exception($"Не удалось получить свойство: {name} типу {target.GetTypeName()} => {ex.Message}");
            return null;
        }
    }

    public static Dictionary<string,object> GetValues(this object target, IEnumerable<string> name)
    {
        var res = new Dictionary<string, object>();
        foreach(var p in name)
        {
            res[p] = target.GetValue(p);
        }
        return res;
    }

    /// <summary>
    /// РџРѕР»СѓС‡РµРЅРёРµ Р·РЅР°С‡РµРЅРёСЏ СЃРІРѕР№СЃС‚РІР° Р·Р°РґР°РЅРЅС‹Рј РєР»СЋС‡РµРј
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T GetValue<T>(this object target, string name)
    
    {

        if (name is null)
            throw new ArgumentNullException("name");
        PropertyInfo propertyInfo = target.GetType().GetProperty(name);
        FieldInfo fieldInfo = target.GetType().GetField(name);
        return
            fieldInfo != null ? (T)fieldInfo.GetValue(target) :
            propertyInfo != null ? (T)propertyInfo.GetValue(target) :
            throw new Exception($"Свойство {name} Не найдено в {target.GetType()}");
    }

    /// <summary>
    /// РЈСЃС‚Р°РЅРѕРІРєР° Р·РЅР°С‡РµРЅРёСЏ СЃРІРѕР№СЃС‚РІР°
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetTextValue(this object target, string key, string value)
    {
        TextDataSetter.SetValue(target, key, value);
    }

    /// <summary>
    /// РЈСЃС‚Р°РЅРѕРІРєР° Р·РЅР°С‡РµРЅРёСЏ СЃРІРѕР№СЃС‚РІР°
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void SetValue(this object target, string key, object value)
    {
        PropertyInfo prop = target.GetType().GetProperties().FirstOrDefault(p=>p.Name==key);
        if (prop != null)
        {
            prop.SetValue(target, value);
        }
        FieldInfo field = target.GetType().GetField(key);
        if (field != null)
        {
            field.SetValue(target, value);
        }
    }
    /// <summary>
    /// Р’Р°Р»РёРґР°С†РёСЏ РјРѕРґРµР»Рё РїРѕ РїСЂР°РІРёР»Р°Рј РѕРїСЂРµРґРµР»С‘РЅРЅС‹Рј С‡РµСЂРµР· Р°С‚СЂРёР±СѓС‚С‹
    /// </summary>
    /// <returns></returns>
    /// GetInputProperties(ptype)
    public static Dictionary<string, List<string>> Validate(this object p, IEnumerable<string> properties)
    {
        //p.Info("Приступаю к валидации: " + p.GetType().GetProperties().Select(p => p.Name).ToJsonOnScreen());
        object target = p;

        Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
        foreach (var property in properties.Select(name => p.GetType().GetProperty(name)))
        {
            string key = property.Name;

            //p.Info($"Валидация свойства: {key}");
            if (IsPrimitive(property.PropertyType))
            {
                List<string> errors = p.Validate(key);
                if (errors.Count > 0)
                {
                    result[key] = errors;
                }

            }
        }
        Dictionary<string, List<string>> optional = new Dictionary<string, List<string>>();
        var validateOptional = target.GetType().GetMethod("ValidateOptional");
        if (validateOptional is not null)
        {
            optional = (Dictionary<string, List<string>>)validateOptional.Invoke(target, new object[0]);
        }
        foreach (var pkv in optional)
        {
            if (result.ContainsKey(pkv.Key))
            {
                result[pkv.Key].AddRange(optional[pkv.Key]);
            }
            else
            {
                result[pkv.Key] = optional[pkv.Key];
            }
        }


        return result;
    }

    public static bool IsValide(this object p)
    {
        var r = p.Validate();
        return r.Count() == 0 || r.Where(kv => kv.Value.Count() > 0).Count() == 0;
    }


    public static Dictionary<string, List<string>> Validate(this object p)
    {
        var started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

        object target = p;
        if (p is null)
            return result;

        var attrs = p.GetType().GetAttributes();
        List<Task> tasks = new();
        foreach (var name in target.GetType().GetInputProperties())
        {

            var property = target.GetType().GetProperties().FirstOrDefault(p => p.Name == name);
            string key = property.Name;

            if (IsPrimitive(property.PropertyType))
            {
                List<string> errors = p.Validate(key);
                if (errors.Count > 0)
                {
                    result[key] = errors;
                }
                var stamp = DateTimeOffset.Now.ToUnixTimeMilliseconds() - started;
                //p.Info(key + " " + stamp);
            }

        }

        //p.Info(result.ToJson());
        Dictionary<string, List<string>> optional = new Dictionary<string, List<string>>();
        var validateOptional = target.GetType().GetMethod("ValidateOptional");
        if (validateOptional is not null)
        {
            optional = (Dictionary<string, List<string>>)validateOptional.Invoke(target, new object[0]);
        }

        foreach (var pkv in optional)
        {
            if (result.ContainsKey(pkv.Key))
            {
                result[pkv.Key].AddRange(optional[pkv.Key]);
            }
            else
            {
                result[pkv.Key] = optional[pkv.Key];
            }
        }
        var ended = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        //p.Info($"Валидация объекта {target.GetTypeName()} выполнилась за {ended - started} мс");
        if (p is MyValidatableObject mvb)
        {
            mvb.ValidationResults = result;
            //p.Info(result);
            var success = !mvb.ValidationResults.Any(kv => kv.Value is not null && kv.Value.Count() > 0);
            if (success != mvb.ValidationSuccess)
            {
                mvb.OnValidationStateChanged(mvb.ValidationSuccess = success);
            }
      
        }
        return result;
    }

    public static Dictionary<string, List<string>> Validate(this object p, DbContext dbContext)
    {
        var started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

        object target = p;
        if (p is null)
            return result;

        var attrs = p.GetType().GetAttributes();
        List<Task> tasks = new();
        foreach (var name in target.GetType().GetInputProperties())
        {

            var property = target.GetType().GetProperties().FirstOrDefault(p => p.Name == name);
            string key = property.Name;

            if (IsPrimitive(property.PropertyType))
            {
                List<string> errors = p.Validate(key, dbContext);
                if (errors.Count > 0)
                {
                    result[key] = errors;
                }
                var stamp = DateTimeOffset.Now.ToUnixTimeMilliseconds() - started;
                //p.Info(key + " " + stamp);
            }

        }

        //p.Info(result.ToJson());
        Dictionary<string, List<string>> optional = new Dictionary<string, List<string>>();
        var validateOptional = target.GetType().GetMethod("ValidateOptional");
        if (validateOptional is not null)
        {
            optional = (Dictionary<string, List<string>>)validateOptional.Invoke(target, new object[0]);
        }

        foreach (var pkv in optional)
        {
            if (result.ContainsKey(pkv.Key))
            {
                result[pkv.Key].AddRange(optional[pkv.Key]);
            }
            else
            {
                result[pkv.Key] = optional[pkv.Key];
            }
        }
        var ended = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        //p.Info($"Валидация объекта {target.GetTypeName()} выполнилась за {ended - started} мс");
        if (p is MyValidatableObject mvb)
        {
            mvb.ValidationResults = result;
            //p.Info(result);
            var success = mvb.ValidationResults.Any(kv => kv.Value is not null && kv.Value.Count() > 0);
            if (success != mvb.ValidationSuccess)
            {
                mvb.OnValidationStateChanged(mvb.ValidationSuccess = success);
            }
            mvb.ValidationSuccess = success;
        }
        return result;
    }


    private static bool IsPrimitive(Type propertyType)
        => Typing.IsPrimitive(propertyType);
}

