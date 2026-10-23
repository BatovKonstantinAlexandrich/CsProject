using Microsoft.EntityFrameworkCore;

[Label("Поисковый индекс")]
[Description("Создаёт скрипт CREATE INDEX [IX_Entities_Name_Version] ON [Entities] ([Name], [Version]);")]
public class SearchTermsAttribute: EntityAttributeBase
{

    [Label("Колонки по которым выполняется поиск")]
    private readonly string[] _columns;

    public SearchTermsAttribute(string terms) : base()
    {
        try
        {
            _columns = terms.Split(",");
        }
        catch (Exception ex)
        {
            throw new Exception("Не удалось создать экземпляр типа " + GetType().GetTypeName() + " " + ex.Message);
        }
    }

    public override void OnModelCreating(ModelBuilder modelBuilder, Type entityType, DbContext messageDbContext, string property = null)
    {
        //// 1. Определяем полнотекстовый каталог
        //modelBuilder.HasFullTextCatalog("ftCatalog");

        //// 2. Настраиваем индекс для сущности Article
        //var entityBuilder = modelBuilder.Entity(entityType)
        //    .HasFullTextIndex(a => a.Contents) // Указываем свойство для индекса
        //    .UseKeyIndex("PK_Articles")        // Указываем уникальный индекс (обычно PK)
        //    .UseCatalog("ftCatalog");          // Привязываем к каталогу

        var entityBuilder = modelBuilder.Entity(entityType);
        entityBuilder.HasIndex(_columns);
    }
}
 