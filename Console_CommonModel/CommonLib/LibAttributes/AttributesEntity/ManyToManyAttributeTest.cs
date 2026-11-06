using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Microsoft.EntityFrameworkCore;

public class ManyToManyAttributeTest : TestingElement
{
    public ManyToManyAttributeTest() : base() { }
    public ManyToManyAttributeTest(IServiceProvider provider) : base(provider)
    {
    }

    public override void OnTest()
    {
        this.Assert(p => {
            using (var db = new DeliveryDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();                
                db.InitDb<DbContextUser>();
                var userContext = db.UserContexts.Include(u => u.Groups).FirstOrDefault();

                return userContext.Groups.Count() > 0;
            }
        },
        "Прописываем отношения много ко многим",
        "Отношения много ко многим не работают");
        this.Assert(p => {          
            using (var db = new TestDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.Rooms.Add(new() {  });
                db.Rooms.Add(new() {  });
                db.Rooms.Add(new() {  });
                db.Students.Add(new() {  });
                db.Students.Add(new() {  });
                db.Students.Add(new() {  });
                db.SaveChanges();
                db.Bindinds.Add(new()
                {
                    StudentId = db.Students.First().Id,
                    RoomId = db.Rooms.First().Id
                });
                db.Bindinds.Add(new()
                {
                    StudentId = db.Students.First().Id,
                    RoomId = db.Rooms.ToList().Last().Id
                });
                db.Bindinds.Add(new()
                {
                    StudentId = db.Students.ToList().Last().Id,
                    RoomId = db.Rooms.ToList().Last().Id
                });
                db.Bindinds.Add(new()
                {
                    StudentId = db.Students.ToList().Last().Id,
                    RoomId = db.Rooms.ToList().First().Id
                });
               

                var student = db.Students.Include(r => r.Rooms).First();
                var room = db.Rooms.Include(r => r.Students).First();
                var result = room.Students.Count() > 0 || student.Rooms.Count()>0;
                this.Info($"{room.Students.Count()} {student.Rooms.Count()}");
                return result;
            }
        }, 
        "Прописываем отношения много ко многим",
        "Отношения много ко многим не работают");
        

    }

    /// <summary>
    /// Тестовый класс
    /// </summary>
    public class TestDbContext : DbContextBase
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Bindind> Bindinds { get; set; }

        public TestDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Rooms)
                .WithMany(r => r.Students)
                .UsingEntity<Bindind>();
        }
    }
    public class Bindind
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int RoomId { get; set; }
        public Student Student { get; set; }
        public Room Room { get; set; }        
    }

    public class Student
    {
        public int Id { get; set; }
        public IEnumerable<Room> Rooms { get; set; }
    }

    public class Room
    {
        public int Id { get; set; }
        public IEnumerable<Student> Students { get; set; }
    }
}
