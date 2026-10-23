using Console_CommonModel.CommonLib.LibAttributes.AttributesInput;
using Console_CommonModel.CommonLib.LibData;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.RepositoryPattern;
using Console_CommonModel.CommonLib.LibModels.Console_DbContextModule;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections;
using System.Reflection;

/// <summary>
/// Проверка уникальности значения атрибута сущности
/// </summary>
public class UniqValueAttribute : InTransactionValidationAttribute, MyValidation
{
    protected string _error;
    private string _dbContextTypeName;
    public bool Enabled { get; set; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="ErrorMessage">сообщение после отрицательной проверки</param>    
    public UniqValueAttribute(string dbContextTypeName = null, string ErrorMessage =null){
        _error = ErrorMessage;
        _dbContextTypeName = dbContextTypeName;
    }

   
    public UniqValueAttribute(string dbContextTypeName = null)
    {
        _dbContextTypeName = dbContextTypeName;
    }
    public UniqValueAttribute( )
    {
    }

    private object GetDbSet(DbContext subject, string dbset)
    {
        Type type = subject.GetType();        
        foreach (MethodInfo info in type.GetMethods())
        {
            if (info.Name.StartsWith("get_"+ dbset) == true && info.ReturnType.Name.StartsWith("DbSet"))
            {
                return info.Invoke(subject, new object[0]);
            }
        }
        throw new Exception("Коллекция сущностей "+dbset +" не найдена");
    }

    public override string Validate(object model, string property, object value)
    {
        throw new UnsupportedException("");
    }

    public override string GetMessage(object model, string property, object value)
    {
        if (string.IsNullOrEmpty(_error))
        {
            return "Свойство "+property+" должно иметь уникальное значение";
        }
        else
        {
            return _error;
        }
    }

    public override string Validate(object model, string property, object value, DbContext dbContext)
    {
        try
        {            
            if (value is null)
                return GetMessage(model, property, value);
            var adapter = new DbContextAdapter(dbContext);
            var repository = adapter.GetRepositoryForType(model.GetType());

            var p = repository.PropertyIn(property, new List<string>() { value.ToString() });
            var results = (IList)p;
            if (results.Count > 0)
            {
                return GetMessage(model, property, value);
            }
            else
            {
                return null;
            }         
        }
        catch (Exception ex)
        {
            return $"Метод проверки упал с исключением: {ex.Message}";
        }
    }


    /*
    protected  override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {        
        using (ApplicationDbContext db = new ApplicationDbContext())
        {
            string dbsetPropertyName = null;
            if(_dbset == null)
            {
                string entityName = validationContext.ObjectType.Name;
                dbsetPropertyName = Counting.GetMultiCountName(validationContext.ObjectType.Name);
            }
            else
            {
                dbsetPropertyName = _dbset;
            }
            ReflectionService reflection = new ReflectionService();
            object dbsetObj = GetDbSet(db, dbsetPropertyName);
            string propertyName = validationContext.MemberName;            
            bool result = (from i in ((IEnumerable<dynamic>)dbsetObj)
                           where reflection.GetValue((object)i, propertyName) == value
                           select i).Count() == 0;
            return result? null: new ValidationResult(_error);
        }
            
    }*/



}
 
