using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Label("Массив структурированного типа")]
public class InputStructureCollectionAttribute<TBaseEntity> : InputStructureCollectionAttribute where TBaseEntity : class
{
    public InputStructureCollectionAttribute(): base(typeof(TBaseEntity).GetTypeName())
    {
    }

    public InputStructureCollectionAttribute(string arguments) : base(typeof(TBaseEntity).GetTypeName())
    {
    }
}


[Label("Массив структурированного типа")]
public class InputStructureCollectionAttribute : BaseInputAttribute
{

    public Type ItemType { get; }

    public override bool IsValidValue(object value)
    {
        if (value is null)
            return true;
        if (value.GetType().IsImplements(typeof(IEnumerable)) == false)
            return false;
        string typeName = value.GetType().GetTypeName();
        if (typeName.IndexOf("<") == -1)
            return false;
        if (typeName.ParseSubstring("<", ">").Split(",").All(name => name.ToType().IsPrimitiveForType()==false))
            return true;
        return false;
    }
    public Type ProviderType { get; }

 
    public InputStructureCollectionAttribute( ) : base(InputTypes.StructureCollection){}
    public InputStructureCollectionAttribute( string arguments ) : base(InputTypes.StructureCollection){}
    public InputStructureCollectionAttribute(string type="System.String", string typeOptionsProvider=null) : base(InputTypes.StructureCollection)
    {
        /*try
        {
            this.ItemType = type.ToType();
            if (ItemType == null)
            {
                throw new Exception($"Тип {type} элемент коллекции не найден"); ; ;
            }
        }
        catch (Exception ex)
        {
            throw new ArgumentException("type",ex);
        }
        try
        {
            this.ProviderType = type.ToType();
            if (ProviderType == null)
            {
                throw new Exception($"Тип {typeOptionsProvider} поставщика свойств элемент коллекции не найден");
            }
            if (IsExtendsFrom(typeof(InputStructureCollectionAttribute.IOptionsProvider))==false)
            {
                throw new Exception($"Тип {typeOptionsProvider} поставщика должен реализовать интерфейс "+nameof(InputStructureCollectionAttribute.IOptionsProvider));

            }
        }
        catch (Exception ex)
        {
            throw new ArgumentException("type", ex);
        }*/
    }
       
    public override string OnValidate(object model, string property, object value)
    {
        return null;
    }

    public override string OnGetMessage(object model, string property, object value)
    {
        return null;
    }
}
