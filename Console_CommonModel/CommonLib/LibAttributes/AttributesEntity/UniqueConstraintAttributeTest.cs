using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Microsoft.EntityFrameworkCore;

[Label("Проверка атрибута проверки уникальности свойств")]
public class UniqueConstraintAttributeTest : TestingElement
{
    public UniqueConstraintAttributeTest(IServiceProvider provider) : base(provider)
    {
    }

    public override void OnTest()
    {
        this.Assert(p =>
        {
            bool result = false;
            using (var db = new TestDbContextConstraint())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                               
                db.Entities.Add(new() { Name = "Ivan", Version = 1 });
                db.Entities.Add(new() { Name = "Ivan", Version = 2 });
                db.SaveChanges();

                result = true;

                try
                {
                    db.Entities.Add(new() { Name = "Ivan", Version = 1 });
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return result;
                }
                result = false;
            }
            return result;
        },
        $"{nameof(UniqueConstraintAttribute)} применяется к классу и задаёт правила проверки уникальности",
        $"{nameof(UniqueConstraintAttribute)} применяется к классу, но не задаёт правила проверки уникальности");
    }



    /// <summary>
    /// Тестовый класс
    /// </summary>
    public class TestDbContextConstraint: DbContextBase
    {
        public DbSet<TestEntityUniqueConstraint> Entities { get; set; }

        public TestDbContextConstraint()
        {
        }
    }


    [UniqueConstraint($"{nameof(Name)},{nameof(Version)}")]
    public class TestEntityUniqueConstraint
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
    }
}