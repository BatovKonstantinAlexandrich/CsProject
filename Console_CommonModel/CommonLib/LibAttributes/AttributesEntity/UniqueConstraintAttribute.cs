using Microsoft.EntityFrameworkCore;

using System.Reflection;

[Label("Уникальное значение")]
[Description("В конструктор передаётся строка с наименованием свойств входящий в правила проверки,перечисленных через запятую")]
[AttributeUsage(AttributeTargets.Class)]
public class UniqueConstraintAttribute: EntityAttributeBase
{

    [Label("Колонки значения которых должны быть уникальными")]
    private readonly string[] _columns;

    public UniqueConstraintAttribute( [NotNullNotEmpty] string sequence )
    {
        try
        {
            _columns = sequence.Split(",");
        }                      
        catch( Exception ex )
        {
            throw new Exception("Не удалось создать экземпляр типа "+GetType().GetTypeName() + " "+ex.Message);
        }
    }



    public override void OnModelCreating(ModelBuilder modelBuilder, Type entityType, DbContext messageDbContext, string property = null)
    {
        try
        {
            this.Debug($"{nameof(OnModelCreating)}");
            if (_columns.Length > 0)
            {
                // Проверяем существование свойств в типе сущности
                var validProperties = _columns
                    .Where(propName => entityType.GetProperty(propName, BindingFlags.Public | BindingFlags.Instance) != null)
                    .ToArray();

                if (validProperties.Length > 0)
                {
                    // Создаем уникальное ограничение через modelBuilder
                    var entityBuilder = modelBuilder.Entity(entityType);

                    if (validProperties.Length == 1)
                    {
                        // Для одного свойства используем простой HasIndex
                        entityBuilder.HasIndex(validProperties[0]).IsUnique();
                    }
                    else
                    {
                        // Для нескольких свойств создаем составной уникальный индекс
                        // EF Core поддерживает HasIndex с массивом строк для составных индексов
                        entityBuilder.HasIndex(validProperties).IsUnique();
                    }
                }
            }
             
            this.Debug(nameof(OnModelCreating)+" COMPLETED");
        }
        catch (Exception ex)
        {            
            throw new Exception($"Не удалось выполнить {nameof(OnModelCreating)} " + GetType().GetTypeName() + " " + ex.Message);
        }
    }
}