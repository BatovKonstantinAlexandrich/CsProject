using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// Указывает что нужно создать объект через конструктор одного из типов, реализующих GenericType аргумент класс.
/// </summary>    
public class InputConstructorAttribute<GenericType> : BaseInputAttribute where GenericType : class
{
    public InputConstructorAttribute() : base() { }

    public InputConstructorAttribute(string typename) : base()
    {
    }

    /// <summary>
    /// Список типов реализующий базовый
    /// </summary>        
    public IEnumerable<Type> GetExtendedTypes() => 
        ServiceFactory.Get().GetTypesExtended<GenericType>().Where(p => !p.IsAbstract && p.GetGenericArguments().Count()==0);
    public IEnumerable<string> GetExtendedTypesNames() =>
        GetExtendedTypes().Select(p => p.GetTypeName());
}
