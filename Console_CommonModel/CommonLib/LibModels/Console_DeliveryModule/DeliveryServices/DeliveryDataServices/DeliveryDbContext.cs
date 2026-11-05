using System.ComponentModel.DataAnnotations;
using static System.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Console_CommonModel.CommonLib.LibAuth.AppAuth.Keycloak;
using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryApi;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel.BusinessResources;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices.DeliveryDataServices;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices.DeliveryRolesServices;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices.Deps;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using ClassModelLib.ActionsClient;


namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices.DeliveryDataServices
{
    public static class TextExtension
    {

        public static bool IsEngChar(char ch) => ("qwertyuiopasdfghjklzxcvbnm" + "qwertyuiopasdfghjklzxcvbnm".ToUpper()).Contains(ch);
        public static bool IsRusChar(char ch) => ("абвгджеёжзйиклмнпорстуфхцчшщъыьэюя" + "абвгджеёжзйиклмнпорстуфхцчшщъыьэюя".ToUpper()).Contains(ch);
        public static List<string> SplitWords(this string text)
        {
            List<string> words = new List<string>();

            string cur = "";
            bool first = true;
            string lang = "RU";
            foreach (char ch in text)
            {
                if (first == true)
                {
                    lang = IsEngChar(ch) ? "ENG" : IsRusChar(ch) ? "RU" : "?";
                    if (lang != "?")
                    {
                        cur = ch + "";
                        first = false;
                    }
                }
                else
                {
                    string curlang = IsEngChar(ch) ? "ENG" : IsRusChar(ch) ? "RU" : "?";
                    if (lang == curlang)
                    {
                        cur += ch;
                    }
                    else
                    {
                        words.Add(cur.ToUpper());
                        cur = "";
                        first = true;
                    }
                }
            }
            if (cur != "")
            {
                words.Add(cur.ToUpper());
            }

            foreach (var word in words)
            {
                foreach (var ch in word)
                {
                    if (IsEngChar(ch) == false && IsRusChar(ch) == false)
                    {
                        throw new Exception("Парсим некорректно: \n" + text);
                    }
                }
            }
            return words;
        }

    }

}

namespace pickpoint_delivery_service
{

    public class OrderCheckoutModel<TEntity> : SearchModel<TEntity>
    {
        public List<TEntity> Selected { get; set; } = new List<TEntity>();

    }
    public class SearchModel<TEntity>
    {

        public string SearchQuery { get; set; }
        public IEnumerable<string> SearchOptions { get; set; } = new List<string>();
        public List<TEntity> SearchResults { get; set; } = new List<TEntity>();
        public int TotalResults { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
    }

    public abstract class SearchController<TResult> where TResult : BaseEntity
    {
        public SearchModel<TResult> searchModel = new();

        public SearchModel<TResult> DoSearch()
        {
            if (searchModel is null)
            {
                searchModel = new SearchModel<TResult>();
            }
            searchModel.SearchResults = GetSearchResults(searchModel.SearchQuery).ToList();
            return searchModel;
        }
        public IEnumerable<string> GetOptions(string query)
        {
            var fasade = typeof(TResult).GetEntityFasade<TResult>();
            return fasade.GetOptions(query);
        }
        public IEnumerable<TResult> GetSearchResults(string query)
        {
            var fasade = typeof(TResult).GetEntityFasade<TResult>();
            return fasade.Search(query);
        }
    }
    public abstract class AbstractOrderCheckoutController<TResult> : SearchController<TResult> where TResult : BaseEntity
    {
        protected DeliveryDbContext _deliveryDbContext;
        protected IWebHostEnvironment _env;
        protected IEntityFasade<ProductImage> _images;
        protected IEntityFasade<Product> _products;

        public virtual IActionResult Index() => Redirect("/User/OrderCheckout/PurchaseOrder");

        private IActionResult Redirect(string v)
        {
            throw new NotImplementedException();
        }



        //[HttpGet]
        public IActionResult PurchaseOrder()
            => View("PurchaseOrder", new SearchModel<Product>());

        private IActionResult View(string v, SearchModel<Product> searchModel)
        {
            throw new NotImplementedException();
        }

        //[HttpPost]
        public IActionResult PurchaseOrder(SearchModel<Product> model)
            => View("PurchaseOrder", model);

        public AbstractOrderCheckoutController() { }
        public void Setup(DeliveryDbContext deliveryDbContext, IWebHostEnvironment env, IEntityFasade<Product> products, IEntityFasade<ProductImage> images)
        {
            _deliveryDbContext = deliveryDbContext;
            _env = env;
            _products = products;
        }

        public IEnumerable<Order> GetOrders(int customerId)
        {
            return _deliveryDbContext.Orders.Include(o => o.Holder).Include(o => o.OrderItems).Where(o => o.CustomerId == customerId).OrderByDescending(o => o.OrderCreated);
        }

        private IEnumerable<OrderItem> GetOrderItems(int orderId)
        {
            return _deliveryDbContext.OrderItems.Where(o => o.OrderId == orderId);
        }

        private int UpdateOrder(Order order)
        {
            var current = _deliveryDbContext.Orders.Find(order.Id);
            foreach (var propertyInfo in order.GetType().GetProperties().ToList())
            {
                propertyInfo.SetValue(current, propertyInfo.GetValue(order));
            }
            return _deliveryDbContext.SaveChanges();
        }


        public int AddToOrder(int orderId, int productId, int productCount)
        {

            var item = new OrderItem()
            {
                ProductId = productCount,
                ProductCount = productCount,
                OrderId = orderId
            };
            _deliveryDbContext.OrderItems.Add(item);
            return _deliveryDbContext.SaveChanges();
        }

        public int RemoveFromOrder(int orderItemId)
        {
            _deliveryDbContext.OrderItems.Remove(_deliveryDbContext.OrderItems.Find(orderItemId));
            return _deliveryDbContext.SaveChanges();
        }
        public object CancelTheOrder(int orderId)
        {
            var order = _deliveryDbContext.Orders.Find(orderId);
            if (order == null)
            {
                return NotFound();
            }
            else
            {
                order.OnOrderCanceled();
                return Json(_deliveryDbContext.SaveChanges());
            }
        }

        private object Json(int v)
        {
            throw new NotImplementedException();
        }

        private object NotFound()
        {
            throw new NotImplementedException();
        }
    }



    [Label("Продажа и доставка")]
    public partial class DeliveryDbContext : ActionDbContext
    {
        public virtual DbSet<Transport> Transports { get; set; }

        public virtual DbSet<ProductCatalog> ProductCatalogs { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ProductVideo> ProductVideos { get; set; }
        public virtual DbSet<ProductComment> ProductComments { get; set; }

        public virtual DbSet<Holder> Holders { get; set; }
        public virtual DbSet<HolderStorage> HolderStorages { get; set; }
        public virtual DbSet<ProductsInStock> ProductsInStock { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerCar> Cars { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }




        /// <summary>
        /// 
        /// </summary>        
        public List<ContextView> CreateBusinessContext<TEntity>()
            where TEntity : BaseEntity
        {
            List<ContextView> menu = new();
            var businessFunctions = business.GetBusinessFunctionsForEntity<TEntity>();
            foreach (var pFunction in businessFunctions)
            {
                menu.Add(new ContextView()
                {
                    Label = pFunction.Label,
                    OnClick = (p, e) =>
                    {
                        Action todo = business.TodoBusinessFunction<DeliveryDbContext, TEntity>(this, pFunction);
                        try
                        {
                            todo();
                        }
                        catch (Exception ex)
                        {
                            this.Error(ex);
                        }
                    }
                });
            }
            return menu;
        }

        public DeliveryDbContext(DbContextOptions<DbContextBase> options) : base(options)
        {


        }
        public DeliveryDbContext() : base()
        {


            /*SavedChanges += (p, evt) =>
            {
                WriteLine($"SavedChanges Success: {evt.EntitiesSavedCount}");
            };
            SaveChangesFailed += (p, evt) =>
            {
                WriteLine($"SavedChanges Failed: {evt.Exception.Message}");
            };  */
        }


        public DeliveryDbContext(IConfiguration configuration) : base()
        {
            /*SavedChanges += (p, evt) =>
            {
                WriteLine($"SavedChanges Success: {evt.EntitiesSavedCount}");
            };
            SaveChangesFailed += (p, evt) =>
            {
                WriteLine($"SavedChanges Failed: {evt.Exception.Message}");
            };*/

        }

        public void Init()
        {
            AddDbContextAuth<UserContext, UserAccount, UserPerson>();
            AddDbContextAuth<ServiceContext, ServiceSertificate, ServiceInfo>();
            //this.AddDbContextAuth<NetworkContext, NetworkToken, NetworkConfiguration>();
            //this.AddDbContextAuth<ProgramContext, ProgramSerialKey, ProgramDescription>();
        }

        public class NetworkContext : ActiveObject
        {
            public int NetworkTokenId { get; set; }
            public NetworkToken NetworkToken { get; set; }
            public int NetworkConfigurationId { get; set; }
            public NetworkToken NetworkConfiguration { get; set; }
        }

        public class NetworkToken : PrivateData<NetworkContext>
        {
            public byte[] Authorization { get; set; }
            public string Token { get; set; }
        }

        public class NetworkConfiguration : BaseEntity
        {
            public string Ip { get; set; }
            public string Host { get; set; }
            public string Port { get; set; }
        }



        public class ProgramContext : ActiveObject
        {
            public int ProgramSerialKeyId { get; set; }
            public ProgramSerialKey ProgramSerialKey { get; set; }
            public int ProgramDescriptionId { get; set; }
            public ProgramDescription ProgramDescription { get; set; }
        }

        public class ProgramSerialKey : PrivateData<ProgramContext>
        {
            public byte[] hash { get; set; }
            public long size { get; set; }
            public byte[] dll { get; set; }
        }

        public class ProgramDescription : BaseEntity
        {
            public string Name { get; set; }
            public string Resource { get; set; }
            public string Version { get; set; }
        }

        private List<Func<DeliveryDbContext, bool>> authenticationServices = new();
        private List<Action<DeliveryDbContext, IConsoleСontrol>> signupService = new();

        public bool AuthenticateSystem()
        {
            return authenticationServices.All(auth => auth(this));
        }

        /// <summary>
        /// 
        /// </summary>    
        public void AddDbContextAuth<TActiveObject, TAuthMember, TObjectInfo>()
            where TAuthMember : PrivateData<TActiveObject>
            where TActiveObject : ActiveObject
            where TObjectInfo : BaseEntity
        {

            authenticationServices.Add(context =>
            {
                using (var db = new DbContextAuth<TActiveObject, TAuthMember, TObjectInfo>(GetConnectionString()))
                {
                    ITokenProvider tokenProvider = new FileTokenProvider<TActiveObject>();
                    tokenProvider.SetDirectory(typeof(TActiveObject).GetTypeName());
                    var signinType = typeof(BaseSignin<TActiveObject, TAuthMember, TObjectInfo>);
                    var serviceType = ServiceFactory.Get().GetTypesExtended(signinType).FirstOrDefault();
                    BaseSignin<TActiveObject, TAuthMember, TObjectInfo> signin = (BaseSignin<TActiveObject, TAuthMember, TObjectInfo>)
                        serviceType.New(new object[] { });
                    signin.SetDbContext(db);
                    var token = tokenProvider.Get();
                    return signin.Validate(token);
                }
            });


            using (var db = new DbContextAuth<TActiveObject, TAuthMember, TObjectInfo>())
            {
                ITokenProvider tokenProvider = new FileTokenProvider<TActiveObject>();
                tokenProvider.SetDirectory(typeof(TActiveObject).GetTypeName());
                var signinType = typeof(BaseSignin<TActiveObject, TAuthMember, TObjectInfo>);
                var serviceType = ServiceFactory.Get().GetTypesExtended(signinType).FirstOrDefault();
                BaseSignin<TActiveObject, TAuthMember, TObjectInfo> signin = (BaseSignin<TActiveObject, TAuthMember, TObjectInfo>)
                    serviceType.New(new object[] { tokenProvider });
                signin.SetDbContext(db);

                this.Info(db.GetConnectionString());
                var entities = db.GetEntitiesTypes();

                //db.Database.EnsureDeleted();
                //db.Database.EnsureCreated();

                GetSqlServerWebApi().EntityTypes.AddRange(entities);
                GetSqlServerWebApi().CreateOnlyDatabase();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //using(var classes = this.GetService<ClassDbContext>())
            //{
            //    //this.GetSqlServerWebApi().EntityTypes.Add();     
            //    this.GetSqlServerWebApi().CreateDatabase(); 
            //}
        }



        public void AddProduct<TProduct>() where TProduct : HierTable<Product>
        {
            GetSqlServerWebApi().EntityTypes.Add(typeof(TProduct));
        }





        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            OnConfiguringDbContext(optionsBuilder);
            base.OnConfiguring(optionsBuilder);
        }



        /*public IDictionary<string, int> InitPrimaryData(IWebHostEnvironment env)
        {
            string contentPath = env.ContentRootPath.ReplaceAll(@"\\", @"/").ReplaceAll(@"\", @"/");
            WriteLine(contentPath);
            this.Database.EnsureDeleted();
            this.Database.EnsureCreated();
            return InitPrimaryData(contentPath);
        }*/

        public IDictionary<string, int> InitPrimaryData(string contentPath = @"D:\System-Config\MyExpirience\Console_BlazorAPp\Console_UserInterface\Resources")
        {
            WriteLine(contentPath);
            Database.EnsureDeleted();
            Database.EnsureCreated();

            var initiallizer = new DeliveryDbContextInitiallizer();
            return initiallizer.InitFromFiles(this, contentPath);
        }


        public static void ConfigureUnitTestingDeliveryServices(IServiceCollection services, IConfiguration config)
        {
            WriteLine(config["Environment"]);

            services.AddDbContext<DeliveryDbContext>(options =>
                options.UseInMemoryDatabase(nameof(DeliveryDbContext)));
            services.AddSingleton(typeof(IEntityFasade<Holder>), sp => new EfEntityFasade<Holder>(sp.GetService<DeliveryDbContext>()));
            services.AddSingleton(typeof(IEntityFasade<Product>), sp => new EfEntityFasade<Product>(sp.GetService<DeliveryDbContext>()));
            services.AddSingleton(typeof(IEntityFasade<ProductImage>), sp => new EfEntityFasade<ProductImage>(sp.GetService<DeliveryDbContext>()));
            services.AddSingleton(typeof(IEntityFasade<ProductsInStock>), sp => new EfEntityFasade<ProductsInStock>(sp.GetService<DeliveryDbContext>()));
            services.AddSingleton(typeof(IEntityFasade<Order>), sp => new EfEntityFasade<Order>(sp.GetService<DeliveryDbContext>()));
            services.AddSingleton(typeof(IEntityFasade<OrderItem>), sp => new EfEntityFasade<OrderItem>(sp.GetService<DeliveryDbContext>()));
            services.AddSingleton(typeof(IEntityFasade<Customer>), sp => new EfEntityFasade<Customer>(sp.GetService<DeliveryDbContext>()));

            services.AddSingleton<IKeywordsParserService, StupidKeywordsParserService>();
            services.AddSingleton<IDeliveryDbContextInitiallizer, DeliveryDbContextInitiallizer>();
            services.AddSingleton<DeliveryUnitOfWork>();

            services.AddTransient<IOrdersService, OrdersService<DeliveryDbContext>>();
            //OrderConsumer.AddOrderConsumer(services, "http://localhost:8080");
        }
        public static void CreateDeliveryData(IServiceCollection services, ConfigurationManager configuration)
        {
            using (var db = new DeliveryDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                db.Info("Создание минимального набора данных");
                db.Info
                (
                    $"Создание минимального набора данных \n {db.InitPrimaryData(@"D:\System-Config\MyExpirience\Console_BlazorAPp\Console_UserInterface\Resources").ToJsonOnScreen()}"
                );
            }
        }
        public static void ConfigureDeliveryServices(IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DeliveryDbContext>(OnConfiguringDbContext);
            services.AddScoped<DbContextUser>(sp => sp.Get<DeliveryDbContext>());

            services.AddScoped<SigninUser<DeliveryDbContext>>();
            services.AddScoped<IKeywordsParserService, StupidKeywordsParserService>();

            services.AddScoped(typeof(IEntityFasade<Customer>), sp => new EfEntityFasade<Order>(sp.GetService<DeliveryDbContext>()));
            services.AddScoped(typeof(IEntityFasade<Holder>), sp => new EfEntityFasade<Holder>(sp.GetService<DeliveryDbContext>()));
            services.AddScoped(typeof(IEntityFasade<Product>), sp => new EfEntityFasade<Product>(sp.GetService<DeliveryDbContext>()));
            services.AddScoped(typeof(IEntityFasade<ProductImage>), sp => new EfEntityFasade<ProductImage>(sp.GetService<DeliveryDbContext>()));
            services.AddScoped(typeof(IEntityFasade<ProductsInStock>), sp => new EfEntityFasade<ProductsInStock>(sp.GetService<DeliveryDbContext>()));
            services.AddScoped(typeof(IEntityFasade<Order>), sp => new EfEntityFasade<Order>(sp.GetService<DeliveryDbContext>()));
            services.AddScoped(typeof(IEntityFasade<OrderItem>), sp => new EfEntityFasade<OrderItem>(sp.GetService<DeliveryDbContext>()));
            services.AddScoped(typeof(IEntityFasade<Customer>), sp => new EfEntityFasade<Customer>(sp.GetService<DeliveryDbContext>()));
            services.AddScoped<IOrdersService, OrdersService<DeliveryDbContext>>();
            //services.AddScoped<UnitOfWork>();
            services.AddScoped<IDeliveryDbContextInitiallizer, DeliveryDbContextInitiallizer>();
            services.AddTransient<IProductsInStockService, ProductsInStockService>();
            services.AddScoped<IKeywordsParserService, StupidKeywordsParserService>();
            services.AddTransient<ITransportService, TransportService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IHolderService, HolderService>();
            services.AddTransient<ITransportUser, TransportUser>();

            //OrderConsumer.AddOrderConsumer(services,"http://localhost:8080");
        }

        private static void OnConfiguringDbContext(DbContextOptionsBuilder optionsBuilder)
        {

            //optionsBuilder.UseInMemoryDatabase(nameof(DeliveryDbContext));
            optionsBuilder.UseSqlServer($@"Data Source=DESKTOP-IHJM9RD;Initial Catalog={nameof(DeliveryDbContext)};Integrated Security=True;Connect Timeout=30;Encrypt=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            //



        }


        public static void UseDeliveryServices(IApplicationBuilder app)
        {

        }


    }

    public static class StringNormalizationExtensions
    {
        public static string ReplaceAll(this string text, string s1, string s2)
        {
            while (text.IndexOf(s1) != -1)
            {
                text = text.Replace(s1, s2);
            }
            return text;
        }
    }





    /// <summary>
    /// несколько групп чисел с разделителем
    /// 
    /// </summary>
    public class SerialNumberAttribute : ValidationAttribute
    {
        private readonly string _pattern;

        public SerialNumberAttribute()
        {
        }

        public SerialNumberAttribute(string pattern)
        {
            _pattern = pattern;
        }

        bool isNumber(char ch) => "0123456789".Contains(ch);

        public override bool IsValid(object value)
        {
            string message = "" + value;
            if (_pattern.Length != message.Length)
                return false;
            for (int i = 0; i < _pattern.Length; i++)
            {
                switch (_pattern[i])
                {
                    case 'x':
                    case 'X':
                        if (!isNumber(message[i]))
                            return false;
                        break;
                    case '-':

                        if (message[i] != '-')
                            return false;
                        break;


                    default: throw new Exception("Неправельно указан аргумент " + nameof(SerialNumberAttribute));

                }
            }
            return true;
        }
    }

    public class PhoneNumberAttribute : ValidationAttribute
    {
        public PhoneNumberAttribute()
        {
        }
        public PhoneNumberAttribute(string exp)
        {
        }

        bool isNumber(char ch) => "0123456789".Contains(ch);


        public override bool IsValid(object value)
        {
            string message = "" + value;
            if (message[1] != '-' || message[5] != '-' || message[9] != '-')
            {
                return false;
            }
            else
            {
                if (isNumber(message[0]) == false ||
                    isNumber(message[2]) == false || isNumber(message[3]) == false || isNumber(message[4]) == false ||
                    isNumber(message[6]) == false || isNumber(message[7]) == false || isNumber(message[8]) == false ||
                    isNumber(message[10]) == false || isNumber(message[11]) == false ||
                    isNumber(message[12]) == false || isNumber(message[13]) == false)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class CustomerGenerator
    {


        private static string MANS_NAMES_INPUT = "А АаронАбрамАвазАвгустинАвраамАгапАгапитАгатАгафонАдамАдрианАзаматАзатАзизАидАйдарАйратАкакийАкимАланАлександрАлексейАлиАликАлимАлиханАлишерАлмазАльбертАмирАмирамАмиранАнарАнастасийАнатолийАнварАнгелАндрейАнзорАнтонАнфимАрамАристархАркадийАрманАрменАрсенАрсенийАрсланАртёмАртемийАртурАрхипАскарАсланАсханАсхатАхметАшот Б БахрамБенджаминБлезБогданБорисБориславБрониславБулат В ВадимВалентинВалерийВальдемарВарданВасилийВениаминВикторВильгельмВитВиталийВладВладимирВладиславВладленВласВсеволодВячеслав Г ГавриилГамлетГарриГеннадийГенриГенрихГеоргийГерасимГерманГерманнГлебГордейГригорийГустав Д ДавидДавлатДамирДанаДаниилДанилДанисДаниславДаниэльДаниярДарийДауренДемидДемьянДенисДжамалДжанДжеймсДжереми ИеремияДжозефДжонатанДикДинДинарДиноДмитрийДобрыняДоминик Е ЕвгенийЕвдокимЕвсейЕвстахийЕгорЕлисейЕмельянЕремейЕфимЕфрем Ж ЖданЖерарЖигер З ЗакирЗаурЗахарЗенонЗигмундЗиновийЗурабЗуфар И ИбрагимИванИгнатИгнатийИгорьИероним ДжеромИисусИльгизИльнурИльшатИльяИльясИмранИннокентийИраклийИсаакИсаакийИсидорИскандерИсламИсмаилИтан К КазбекКамильКаренКаримКарлКимКирКириллКлаусКлимКонрадКонстантинКореКорнелийКристианКузьма Л ЛаврентийЛадоЛевЛенарЛеонЛеонардЛеонидЛеопольдЛоренсЛукаЛукиллианЛукьянЛюбомирЛюдвигЛюдовикЛюций М МаджидМайклМакарМакарийМаксимМаксимилианМаксудМансурМарМаратМаркМарсельМартин МартынМатвейМахмудМикаМикулаМилославМиронМирославМихаилМоисейМстиславМуратМуслимМухаммедМэтью Н НазарНаильНариманНатанНесторНикНикитаНикодимНиколаНиколайНильсНурлан О ОгюстОлегОливерОрестОрландоОсип ИосифОскарОсманОстапОстин П ПавелПанкратПатрикПедроПерриПётрПлатонПотапПрохор Р РавильРадийРадикРадомирРадославРазильРаильРаифРайанРаймондРамазанРамзесРамизРамильРамонРанельРасимРасулРатиборРатмирРаушанРафаэльРафикРашидРинат РенатРичардРобертРодимРодионРожденРоланРоманРостиславРубенРудольфРусланРустамРэй С СавваСавелийСаидСалаватСаматСамвелСамирСамуилСанжарСаниСаянСвятославСевастьянСемёнСерафимСергейСидорСимбаСоломонСпартакСтаниславСтепанСулейманСултанСурен Т ТагирТаирТайлерТалгатТамазТамерланТарасТахирТигранТимофейТимурТихонТомасТрофим У УинслоуУмарУстин Ф ФазильФаридФархадФёдорФедотФеликсФилиппФлорФомаФредФридрих Х ХабибХакимХаритонХасан Ц ЦезарьЦефасЦецилий СесилЦицерон Ч ЧарльзЧеславЧингиз Ш ШамильШарльШерлок Э ЭдгарЭдуардЭльдарЭмильЭминЭрикЭркюльЭрминЭрнестЭузебио Ю ЮлианЮлийЮнусЮрийЮстинианЮстус Я ЯковЯнЯромирЯрослав";
        private static string WOMANS_NAMES_INPUT = "А АваАвгустаАвгустинаАвдотьяАврораАгапияАгатаАгафьяАглаяАгнияАгундаАдаАделаидаАделинаАдельАдиляАдрианаАзаАзалияАзизаАидаАишаАйАйаруАйгеримАйгульАйлинАйнагульАйнурАйсельАйсунАйсылуАксиньяАланаАлевтинаАлександраАлексияАлёнаАлестаАлинаАлисаАлияАллаАлсуАлтынАльбаАльбинаАльфияАляАмалияАмальАминаАмираАнаитАнастасияАнгелинаАнжелаАнжеликаАнисьяАнитаАннаАнтонинаАнфисаАполлинарияАрабеллаАриаднаАрианаАриандаАринаАрияАсельАсияАстридАсяАфинаАэлитаАяАяна Б БаженаБеатрисБелаБелиндаБелла БэллаБертаБогданаБоженаБьянка В ВалентинаВалерияВандаВанессаВарвараВасилинаВасилисаВенераВераВероникаВестаВетаВикторинаВикторияВиленаВиолаВиолеттаВитаВиталина ВиталияВладаВладанаВладислава Г ГабриэллаГалинаГалияГаянаГаянэГенриеттаГлафираГоарГретаГульзираГульмираГульназГульнараГульшатГюзель Д ДалидаДамираДанаДаниэлаДанияДараДаринаДарьяДаянаДжамиляДженнаДженниферДжессикаДжиневраДианаДильназДильнараДиляДилярамДинаДинараДолоресДоминикаДомнаДомника Е ЕваЕвангелинаЕвгенияЕвдокияЕкатеринаЕленаЕлизаветаЕсенияЕя Ж ЖаклинЖаннаЖансаяЖасминЖозефинаЖоржина З ЗабаваЗаираЗалинаЗамираЗараЗаремаЗаринаЗемфираЗинаидаЗитаЗлатаЗлатославаЗорянаЗояЗульфияЗухра И Иветта ИветаИзабеллаИлинаИллирикаИлонаИльзираИлюзаИнгаИндираИнессаИннаИоаннаИраИрадаИраидаИринаИрмаИскраИя К КамилаКамиллаКараКареКаримаКаринаКаролинаКираКлавдияКлараКонстанцияКораКорнелияКристинаКсения Л ЛадаЛанаЛараЛарисаЛаураЛейлаЛеонаЛераЛесяЛетаЛианаЛидияЛизаЛикаЛилиЛилианаЛилитЛилияЛинаЛиндаЛиораЛираЛияЛолаЛолитаЛораЛуизаЛукерьяЛукияЛунаЛюбаваЛюбовьЛюдмилаЛюсильЛюсьенаЛюцияЛючеЛяйсанЛяля М МавилеМавлюдаМагдаМагдалeнаМадинаМадленМайяМакарияМаликаМараМаргаритаМарианнаМарикаМаринаМарияМариямМартаМарфаМеланияМелиссаМехриМикаМилаМиладаМиланаМиленМиленаМилицаМилославаМинаМираМирославаМирраМихримахМишельМияМоникаМуза Н НадеждаНаиляНаимаНанаНаомиНаргизаНатальяНеллиНеяНикаНикольНинаНинельНоминаНоннаНораНурия О ОдеттаОксанаОктябринаОлесяОливияОльгаОфелия П ПавлинаПамелаПатрицияПаулаПейтонПелагеяПеризатПлатонидаПолинаПрасковья Р РавшанаРадаРазинаРаиляРаисаРаифаРалинаРаминаРаянаРебеккаРегинаРезедаРенаРенатаРианаРианнаРикардаРиммаРинаРитаРогнедаРозаРоксанаРоксоланаРузалияРузаннаРусалинаРусланаРуфинаРуфь С СабинаСабринаСажидаСаидаСалимаСаломеяСальмаСамираСандраСанияСараСатиСаулеСафияСафураСаянаСветланаСевараСеленаСельмаСерафимаСильвияСимонаСнежанаСоняСофьяСтеллаСтефанияСусанна Т ТаисияТамараТамилаТараТатьянаТаяТаянаТеонаТерезаТеяТинаТиффаниТомирисТораТэмми У УльянаУмаУрсулаУстинья Ф ФазиляФаинаФаридаФаризаФатимаФедораФёклаФелиситиФелицияФерузаФизалияФирузаФлораФлорентинаФлоренция ФлоренсФлорианаФредерикаФрида Х ХадияХилариХлояХюррем Ц ЦаганаЦветанаЦецилия СесилияЦиара Сиара Ч ЧелсиЧеславаЧулпан Ш ШакираШарлоттаШахинаШейлаШеллиШерил Э ЭвелинаЭвитаЭлеонораЭлианаЭлизаЭлинаЭллаЭльвинаЭльвираЭльмираЭльнараЭляЭмилиЭмилияЭммаЭнжеЭрикаЭрминаЭсмеральдаЭсмираЭстерЭтельЭтери Ю ЮлианнаЮлияЮнаЮнияЮнона Я ЯдвигаЯнаЯнинаЯринаЯрославаЯсмина";
        private static string MANS_SECONDNAMES_INPUT = "АлексеевичАнатольевичАндреевичАнтоновичАркадьевичАртемовичБедросовичБогдановичБорисовичВалентиновичВалерьевичВасильевичВикторовичВитальевичВладимировичВладиславовичВольфовичВячеславовичГеннадиевичГеоргиевичГригорьевичДаниловичДенисовичДмитриевичЕвгеньевичЕгоровичЕфимовичИвановичИванычИгнатьевичИгоревичИльичИосифовичИсааковичКирилловичКонстантиновичЛеонидовичЛьвовичМаксимовичМатвеевичМихайловичНиколаевичОлеговичПавловичПалычПетровичПлатоновичРобертовичРомановичСанычСевериновичСеменовичСергеевичСтаниславовичСтепановичТарасовичТимофеевичФедоровичФеликсовичФилипповичЭдуардовичЮрьевичЯковлевичЯрославович";
        private static string MANS_SURNAMES_INPUT = "СмирновИвановКузнецовСоколовПоповЛебедевКозловНовиковМорозовПетровВолковСоловьёвВасильевЗайцевПавловСемёновГолубевВиноградовБогдановВоробьёвФёдоровМихайловБеляевТарасовБеловКомаровОрловКиселёвМакаровАндреевКовалёвИльинГусевТитовКузьминКудрявцевБарановКуликовАлексеевСтепановЯковлевСорокинСергеевРомановЗахаровБорисовКоролёвГерасимовПономарёвГригорьевЛазаревМедведевЕршовНикитинСоболевРябовПоляковЦветковДаниловЖуковФроловЖуравлёвНиколаевКрыловМаксимовСидоровОсиповБелоусовФедотовДорофеевЕгоровМатвеевБобровДмитриевКалининАнисимовПетуховАнтоновТимофеевНикифоровВеселовФилипповМарковБольшаковСухановМироновШиряевАлександровКоноваловШестаковКазаковЕфимовДенисовГромовФоминДавыдовМельниковЩербаковБлиновКолесниковКарповАфанасьевВласовМасловИсаковТихоновАксёновГавриловРодионовКотовГорбуновКудряшовБыковЗуевТретьяковСавельевПановРыбаковСуворовАбрамовВороновМухинАрхиповТрофимовМартыновЕмельяновГоршковЧерновОвчинниковСелезнёвПанфиловКопыловМихеевГалкинНазаровЛобановЛукинБеляковПотаповНекрасовХохловЖдановНаумовШиловВоронцовЕрмаковДроздовИгнатьевСавинЛогиновСафоновКапустинКирилловМоисеевЕлисеевКошелевКостинГорбачёвОреховЕфремовИсаевЕвдокимовКалашниковКабановНосковЮдинКулагинЛапинПрохоровНестеровХаритоновАгафоновМуравьёвЛарионовФедосеевЗиминПахомовШубинИгнатовФилатовКрюковРоговКулаковТерентьевМолчановВладимировАртемьевГурьевЗиновьевГришинКононовДементьевСитниковСимоновМишинФадеевКомиссаровМамонтовНосовГуляевШаровУстиновВишняковЕвсеевЛаврентьевБрагинКонстантиновКорниловАвдеевЗыковБирюковШараповНиконовЩукинДьячковОдинцовСазоновЯкушевКрасильниковГордеевСамойловКнязевБеспаловУваровШашковБобылёвДоронинБелозёровРожковСамсоновМясниковЛихачёвБуровСысоевФомичёвРусаковСтрелковГущинТетеринКолобовСубботинФокинБлохинСеливерстовПестовКондратьевСилинМеркушевЛыткинТуров";


        public static List<string> MANS_NAMES = GetManNames();
        public static List<string> MANS_SURNAMES = GetManSurnames();
        public static List<string> MANS_LASTNAMES = GetManLastnames();

        public static Customer CreateCustomer()
        {
            int i1 = GetRandom(MANS_NAMES.Count() - 1);
            int i2 = GetRandom(MANS_SURNAMES.Count() - 1);
            int i3 = GetRandom(MANS_LASTNAMES.Count() - 1);
            if (i1 < 0 || i2 < 0 || i3 < 0)
            {
                throw new Exception("Индекс не может быть меньше нуля");
            }
            //this.Error($"{i1},{i2},{i3}");
            string[] names = MANS_NAMES.ToArray();
            string name = names[i1];
            string[] surnames = MANS_SURNAMES.ToArray();
            string surname = surnames[i2];
            string[] lastnames = MANS_LASTNAMES.ToArray();
            string lastname = lastnames[i3];
            return new Customer()
            {
                FirstName = name,
                PhoneNumber = $"7-{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}-{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}-{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}",
                LastName = lastname

            };
        }

        static int GetRandom(int max)
        {
            int res = new Random().Next(max);
            return res == 0 ? 1 : res;
        }

        public static List<string> GetManNames()
        {
            List<string> names = new List<string>();
            foreach (string text in MANS_NAMES_INPUT.Split(" "))
            {
                if (text.Length > 1)
                {
                    names.AddRange(new List<string>(Naming.SplitName(text)));
                }
            }
            return names;
        }
        public static List<string> GetManLastnames()
        {
            List<string> names = new List<string>();
            foreach (string text in MANS_SECONDNAMES_INPUT.Split(" "))
            {
                if (text.Length > 1)
                {
                    names.AddRange(new List<string>(Naming.SplitName(text)));
                }
            }
            return names;
        }
        public static List<string> GetManSurnames()
        {
            List<string> names = new List<string>();
            foreach (string text in MANS_SURNAMES_INPUT.Split(" "))
            {
                if (text.Length > 1)
                {
                    names.AddRange(new List<string>(Naming.SplitName(text)));
                }
            }
            return names;
        }
        public static List<string> GetWomanNames()
        {
            List<string> names = new List<string>();
            foreach (string text in WOMANS_NAMES_INPUT.Split(" "))
            {
                if (text.Length > 1)
                {
                    names.AddRange(new List<string>(Naming.SplitName(text)));
                }
            }
            return names;
        }

        /// <summary>
        /// Перечисление стилей записи идентификаторов
        /// </summary>
        public enum NamingStyles
        {
            Capital, Kebab, Snake, Camel
        }

        /// <summary>
        /// Реализует методы работы с идентификаторами и стилями записи
        /// </summary>
        public class Naming
        {
            private static string SPEC_CHARS = ",.?~!@#$%^&*()-=+/\\[]{}'\";:\t\r\n";
            private static string RUS_CHARS = "ЁЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ" + "ёйцукенгшщзхъфывапролджэячсмитьбю";
            private static string DIGIT_CHARS = "0123456789";
            private static string ENG_CHARS = "qwertyuiopasdfghjklzxcvbnm" + "QWERTYUIOPASDFGHJKLZXCVBNM";




            /// <summary>
            /// Метод разбора идентификатора на модификаторы 
            /// </summary>
            /// <param name="name"> идентификатор </param>
            /// <returns> модификаторы </returns>
            public static string[] SplitName(string name)
            {
                NamingStyles style = ParseStyle(name);
                switch (style)
                {
                    case NamingStyles.Kebab: return SplitKebabName(name);
                    case NamingStyles.Snake: return SplitSnakeName(name);
                    case NamingStyles.Capital: return SplitCapitalName(name);

                    case NamingStyles.Camel: return SplitCamelName(name);
                    default:
                        throw new Exception($"Не удалось разобрать идентификатор {name}.");
                }
            }


            /// <summary>
            /// Запись идентификатора в CapitalStyle
            /// </summary>
            /// <param name="lastname"> идентификатор </param>
            /// <returns>идентификатор в CapitalStyle</returns>
            public static string ToCapitalStyle(string lastname)
            {
                if (string.IsNullOrEmpty(lastname)) return lastname;
                string[] ids = SplitName(lastname);
                return ToCapitalStyle(ids);
            }
            public static string ToCapitalStyle(string[] ids)
            {
                string name = "";
                foreach (string id in ids)
                {
                    name += id.Substring(0, 1).ToUpper() + id.Substring(1).ToLower();
                }
                return name;
            }


            /// <summary>
            /// Запись идентификатора в CamelStyle
            /// </summary>
            /// <param name="lastname"> идентификатор </param>
            /// <returns>идентификатор в CamelStyle</returns>
            public static string ToCamelStyle(string lastname)
            {
                string name = ToCapitalStyle(lastname);
                return name.Substring(0, 1).ToLower() + name.Substring(1);
            }




            /// <summary>
            /// Запись идентификатора в KebabStyle
            /// </summary>
            /// <param name="lastname"> идентификатор </param>
            /// <returns>идентификатор в KebabStyle</returns>
            public static string ToKebabStyle(string lastname)
            {
                string name = "";
                foreach (string id in SplitName(lastname))
                {
                    name += "-" + id.ToLower();
                }
                return name.Substring(1);
            }





            /// <summary>
            /// Запись идентификатора в SnakeStyle
            /// </summary>
            /// <param name="lastname"> идентификатор </param>
            /// <returns>идентификатор в SnakeStyle</returns>
            public static string ToSnakeStyle(string lastname)
            {
                string name = "";
                string[] names = SplitName(lastname);
                foreach (string id in names)
                {
                    name += "_" + id.ToLower();
                }
                return name.Substring(1);
            }


            /// <summary>
            /// Метод разбора идентификатора записанного в CapitalStyle на модификаторы 
            /// </summary>
            /// <param name="name"> идентификатор записанный в CapitalStyle </param>
            /// <returns> модификаторы </returns>
            public static string[] SplitCapitalName(string name)
            {
                List<string> ids = new List<string>();
                string word = "";
                bool WasUpper = false;
                foreach (char ch in name)
                {
                    if (IsUpper(ch) && WasUpper == false)
                    {
                        if (word != "")
                        {
                            ids.Add(word);
                        }
                        word = "";
                        WasUpper = true;
                    }
                    WasUpper = false;
                    word += ch + "";
                }
                if (word != "")
                {
                    ids.Add(word);
                }
                word = "";
                return ids.ToArray();
            }


            /// <summary>
            /// Метод разбора идентификатора записанного в DollarStyle на модификаторы 
            /// </summary>
            /// <param name="name"> идентификатор записанный в DollarStyle </param>
            /// <returns> модификаторы </returns>
            public static string[] SplitDollarName(string name)
            {
                List<string> ids = new List<string>();
                string word = "";
                bool first = true;
                foreach (char ch in name)
                {
                    if (first)
                    {
                        first = false;
                        continue;
                    }
                    if (IsUpper(ch))
                    {
                        if (word != "")
                        {
                            ids.Add(word);
                        }
                        word = "";
                    }
                    word += ch + "";
                }
                if (word != "")
                {
                    ids.Add(word);
                }
                word = "";
                return ids.ToArray();
            }


            /// <summary>
            /// Метод разбора идентификатора записанного в CamelStyle на модификаторы 
            /// </summary>
            /// <param name="name"> идентификатор записанный в CamelStyle </param>
            /// <returns> модификаторы </returns>
            public static string[] SplitCamelName(string name)
            {
                List<string> ids = new List<string>();
                string word = "";
                foreach (char ch in name)
                {
                    if (IsUpper(ch))
                    {
                        if (word != "")
                        {
                            ids.Add(word);
                        }
                        word = "";
                    }
                    word += ch + "";
                }
                if (word != "")
                {
                    ids.Add(word);
                }
                word = "";
                return ids.ToArray();
            }


            /// <summary>
            /// Метод разбора идентификатора записанного в SnakeStyle на модификаторы 
            /// </summary>
            /// <param name="name"> идентификатор записанный в SnakeStyle </param>
            /// <returns> модификаторы </returns>
            public static string[] SplitSnakeName(string name)
            {
                return name.Split("_");
            }


            /// <summary>
            /// Метод разбора идентификатора записанного в KebabStyle на модификаторы 
            /// </summary>
            /// <param name="name"> идентификатор записанный в KebabStyle </param>
            /// <returns> модификаторы </returns>
            public static string[] SplitKebabName(string name)
            {
                return name.Split("-");
            }


            /// <summary>
            /// Метод определния стиля записи идентификатора
            /// </summary>
            /// <param name="name"> идентификатор </param>
            /// <returns> стиль записи </returns>
            public static NamingStyles ParseStyle(string name)
            {
                if (IsCapitalStyle(name))
                    return NamingStyles.Capital;
                if (IsKebabStyle(name))
                    return NamingStyles.Kebab;
                if (IsSnakeStyle(name))
                    return NamingStyles.Snake;

                if (IsCamelStyle(name))
                    return NamingStyles.Camel;

                throw new Exception($"Стиль идентификатора {name} не определён.");
            }


            /// <summary>
            /// Проверка сивола на принадлежность с множеству цифровых символов
            /// </summary>
            /// <param name="ch"> символ </param>
            /// <returns>true, если символ цифровой</returns>
            public static bool IsDigit(char ch)
            {
                return Contains(DIGIT_CHARS, ch);
            }


            /// <summary>
            /// Проверка сивола на принадлежность с множеству символов русского алфавита
            /// </summary>
            /// <param name="ch"> символ </param>
            /// <returns>true, если символ из русского алфавита </returns>
            public static bool IsCharacter(char ch)
            {
                return IsRussian(ch) || IsEnglish(ch);
            }


            /// <summary>
            /// Проверка сивола на принадлежность с множеству символов русского алфавита
            /// </summary>
            /// <param name="ch"> символ </param>
            /// <returns>true, если символ из русского алфавита </returns>
            public static bool IsRussian(char ch)
            {
                return Contains(RUS_CHARS, ch);
            }


            /// <summary>
            /// Проверка сивола на принадлежность с множеству символов русского алфавита
            /// </summary>
            /// <param name="ch"> символ </param>
            /// <returns>true, если символ из русского алфавита </returns>
            public static bool IsEnglish(char ch)
            {
                return Contains(ENG_CHARS, ch);
            }


            /// <summary>
            /// Проверка принадлежности символа к строке
            /// </summary>
            /// <param name="text"></param>
            /// <param name="ch"></param>
            /// <returns></returns>
            public static bool Contains(string text, char ch)
            {
                bool result = false;
                foreach (char rch in text)
                {
                    if (rch == ch)
                    {
                        result = true;
                        break;
                    }
                }
                return result;
            }


            /// <summary>
            /// Метод проверки символа на принадлежность к верхнему регистру
            /// </summary>
            /// <param name="ch"> символ </param>
            /// <returns> true, если принадлежит верхнему регистру </returns>
            public static bool IsUpper(char ch)
            {
                return ch + "" == (ch + "").ToUpper();
            }


            /// <summary>
            /// Проверка стиля записи CapitalStyle( UserId )
            /// </summary>
            /// <param name="name"> идентификатор </param>
            /// <returns> true, если идентификатор записан в CapitalStyle </returns>
            public static bool IsCapitalStyle(string name)
            {
                bool startedWithUpper = name[0] + "" == (name[0] + "").ToUpper();
                bool containsSpecCharaters = name.IndexOf("_") != -1 || name.IndexOf("$") != -1;
                return startedWithUpper && !containsSpecCharaters;
            }


            /// <summary>
            /// Проверка стиля записи SnakeStyle( user_id, USER_Id )
            /// </summary>
            /// <param name="name"> идентификатор </param>
            /// <returns> true, если идентификатор записан в SnakeStyle </returns>
            public static bool IsSnakeStyle(string name)
            {
                bool upperCase = IsUpper(name[0]);
                bool startsWithCharacter = IsCharacter(name[0]);
                char separatorCharacter = '_';
                string anotherChars = new string(SPEC_CHARS).Replace(separatorCharacter + "", "");
                bool containsAnotherSpecChars = false;
                bool containsAnotherCase = false;
                bool containsDoubleSeparator = false;
                bool lastCharWasSeparator = false;
                if (startsWithCharacter == false)
                {
                    return !containsDoubleSeparator && !containsAnotherCase && startsWithCharacter && !containsAnotherSpecChars && !containsAnotherCase;
                }
                else
                {
                    for (int i = 1; i < name.Length; i++)
                    {
                        if (Contains(anotherChars, name[i]))
                        {
                            containsAnotherSpecChars = true;
                            break;
                        }
                        if (name[i] != separatorCharacter)
                        {
                            if (IsUpper(name[i]) != upperCase)
                            {
                                containsAnotherCase = true;
                                break;
                            }
                            lastCharWasSeparator = false;
                        }
                        else
                        {
                            if (lastCharWasSeparator)
                            {
                                containsDoubleSeparator = true;
                                break;
                            }
                            lastCharWasSeparator = true;
                        }
                    }
                }
                return !containsDoubleSeparator && !containsAnotherCase && startsWithCharacter && !containsAnotherSpecChars && !containsAnotherCase;
            }


            /// <summary>
            /// Проверка стиля записи CamelStyle( userId  )
            /// </summary>
            /// <param name="name"> идентификатор </param>
            /// <returns> true, если идентификатор записан в CamelStyle </returns>
            public static bool IsCamelStyle(string name)
            {
                return IsCapitalStyle(name.Substring(0, 1).ToUpper() + name.Substring(1)) && !IsUpper(name[0]) && IsCharacter(name[0]);
            }


            /// <summary>
            /// Проверка стиля записи DollarStyle( $userId  )
            /// </summary>
            /// <param name="name"> идентификатор </param>
            /// <returns> true, если идентификатор записан в DollarStyle </returns>
            public static bool IsDollarStyle(string name)
            {
                return IsCamelStyle(name.Substring(1)) && name[0] == '$';
            }


            /// <summary>
            /// Проверка стиля записи KebabStyle( user-id, USER-Id )
            /// </summary>
            /// <param name="name"> идентификатор </param>
            /// <returns> true, если идентификатор записан в KebabStyle </returns>
            public static bool IsKebabStyle(string name)
            {
                bool upperCase = IsUpper(name[0]);
                bool startsWithCharacter = IsCharacter(name[0]);
                char separatorCharacter = '-';
                string anotherChars = new string(SPEC_CHARS).Replace(separatorCharacter + "", "");
                bool containsAnotherSpecChars = false;
                bool containsAnotherCase = false;
                bool containsDoubleSeparator = false;
                bool lastCharWasSeparator = false;
                if (startsWithCharacter == false)
                {
                    return !containsDoubleSeparator && !containsAnotherCase && startsWithCharacter && !containsAnotherSpecChars && !containsAnotherCase;
                }
                else
                {
                    for (int i = 1; i < name.Length; i++)
                    {
                        if (Contains(anotherChars, name[i]))
                        {
                            containsAnotherSpecChars = true;
                            break;
                        }
                        if (name[i] != separatorCharacter)
                        {
                            if (IsUpper(name[i]) != upperCase)
                            {
                                containsAnotherCase = true;
                                break;
                            }
                            lastCharWasSeparator = false;
                        }
                        else
                        {
                            if (lastCharWasSeparator)
                            {
                                containsDoubleSeparator = true;
                                break;
                            }
                            lastCharWasSeparator = true;
                        }
                    }
                }
                return !containsDoubleSeparator && !containsAnotherCase && startsWithCharacter && !containsAnotherSpecChars && !containsAnotherCase;
            }
        }
    }

}






namespace pickpoint_delivery_service
{








}
