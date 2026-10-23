using Microsoft.EntityFrameworkCore;
using System.Reflection;

[Label("Отношения много-ко-многим")]
[Description("Используется при объединении данных в Repository")]
public class ManyToManyAttribute: EntityAttributeBase
{
    public string PropertyName { get; set; }

    /// <param name="includeToProperty">
    /// Имя свойства коллекцией связанных объектов
    /// </param>
    public ManyToManyAttribute( [NotNullNotEmpty] string includeToProperty )
    {
        MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(includeToProperty);
        PropertyName = includeToProperty;
    }

    public override void OnModelCreating(ModelBuilder modelBuilder, Type entityType, 
        DbContext dbContext, string property)
    {
        try
        {
            this.Debug($"{GetType().GetTypeName()}[{PropertyName}] {nameof(OnModelCreating)} {entityType.GetTypeName()} {property}");
            var manyProperty = entityType.GetProperties().FirstOrDefault(p => p.Name == property);
            if (manyProperty is null)
                throw new ArgumentException($"{GetType().GetTypeName()} {nameof(OnModelCreating)} => {entityType.GetTypeName()} " +
                    $"не объявляет свойство и именем: {property}");
            var manyArguments = manyProperty.PropertyType.GetGenericArguments();

            var manyProperty2 = entityType.GetProperties().FirstOrDefault(p => p.Name == PropertyName);
            if (manyProperty2 is null)
                throw new ArgumentException($"{GetType().GetTypeName()} {nameof(OnModelCreating)} => {entityType.GetTypeName()} " +
                    $"не объявляет свойство и именем: {PropertyName}");
            var manyArguments2 = manyProperty2.PropertyType.GetGenericArguments();
            if (manyArguments.Length > 0 && manyArguments2.Length > 0)
            {
                var arg = manyArguments[0];  //BINDING
                var arg2 = manyArguments2[0];//ENTITY-MANY
                this.Debug(arg2.GetTypeName());
                var property2 = arg2.GetProperties().FirstOrDefault(p => p.PropertyType.IsCollectionType() && p.PropertyType.GetGenericArguments()[0]==entityType);
                var entityBuilder = modelBuilder.Entity(entityType);          
                if (property2 is not null)
                {
                    entityBuilder
                        .HasMany(PropertyName)
                        .WithMany(property2.Name)
                        .UsingEntity(arg);
                }
                 
            }
            else
            {
                throw new Exception();
            }
        }
        catch (Exception ex)
        {
            this.Error(ex);
            throw;
        }        
    }
}