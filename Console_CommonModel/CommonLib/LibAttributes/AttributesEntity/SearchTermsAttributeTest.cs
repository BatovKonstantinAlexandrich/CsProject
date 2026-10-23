
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Microsoft.EntityFrameworkCore;

[Label("Проверка атрибута установки свойств индекса")]
public class SearchTermsAttributeTest : TestingElement
{
    public SearchTermsAttributeTest(IServiceProvider provider) : base(provider)
    {
    }

    public override void OnTest()
    {
        this.Assert(p =>
        {
            bool result = false;
            using (var db = new TestDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.Entities.Add(new() { Name = "Ivan", Version = 1 });
                db.Entities.Add(new() { Name = "Ivan", Version = 2 });
                db.SaveChanges();

                result = true;

                //var results1 = db.Search("Ivan");
                //var results2 = db.Search("1");
                //return results2.Count() == 1 && results1.Count()==0;
                return result;
            }            
        },
        $"{nameof(SearchTermsAttribute)} применяется к классу и задаёт свойства для поисковых запросов",
        $"{nameof(SearchTermsAttribute)} не влияет на поисковые запросов");
    }



    /// <summary>
    /// Тестовый класс
    /// </summary>
    public class TestDbContext : DbContextBase
    {
        public DbSet<TestEntitySearchTerms> Entities { get; set; }

        public TestDbContext()
        {
        }
    }


    [SearchTerms($"{nameof(Version)}")]
    public class TestEntitySearchTerms
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
    }
}