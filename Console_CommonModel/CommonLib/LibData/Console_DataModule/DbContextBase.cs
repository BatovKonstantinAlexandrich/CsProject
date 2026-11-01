using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Data.SqlClient;
using System.Data;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.MetadataServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbModelService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;
using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;
using Console_CommonModel.CommonLib.LibModels.Console_ServiceModule;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.DbSetViewModule;
using Console_CommonModel.CommonLib.LibData.RepositoryPattern;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule
{   
    /// <summary>
    /// Реализует управление атрибутами
    /// OnInsert<T>(data),OnDelete<T>(data),OnUpdate<T>(data),
    /// </summary>
    public partial class DbContextBase : DbContext, ISqlExecutor, IDbMetadata, IDbMigBuilder, IDDLFactory, IDMLFactory,
        IDbConnectionsService, IDbInterface, IDataModelService, ITreeService, IDbContextBase
    {
        public bool EnableDataChangedAttributes = false;
        public string connectionString = null;

        public Dictionary<string, int> InitDb<TDbContext>()
            where TDbContext : DbContextBase
        {
            var initiallizer = this.GetInitializer<TDbContext>();
            if (initiallizer is null)
                return new();
            return initiallizer.Init((TDbContext)this);
        }


        public DbContextBase()
        {
            //this.Debug("Создан новый экземпляр");
        }

        public DbContextBase(DbContextOptions options) : base(options)
        {


        }


        public void EnsureHasData()
        {
            var stat = GetDataInTables();
            if (stat.All(kv => kv.Value == 0))
            {
                 throw new DataMisalignedException($"Отсутствуют какие либо данные в таблицах {GetType().GetTypeName()} {stat.Where(kv => kv.Value == 0).Select(kv => kv.Key).ToJson()}");
            }
        }


        public Dictionary<string, int> GetDataInTables()
        {
            Dictionary<string, int> res = new();
            foreach (var et in this.GetEntitiesTypes())
            {
                try
                {
                    var repository = this.GetRepositoryForType(et);
                    res[et.GetTypeName()] = repository.CountRecords();
                }
                catch(Exception ex)
                {
                    this.Error($"{GetType().GetTypeName()}.{MethodBase.GetCurrentMethod().Name} => исключение при обработки {et.GetTypeName()} {ex.Message}");
                }
            }
            return res;
        }


        /// <summary>
        /// Проверка на наличие зарегистрированной записи по уникальному имени
        /// </summary>
        public TNamedObject CheckRegistration<TNamedObject>(string name)
            where TNamedObject : NamedObject
        {
            var target = Set<TNamedObject>().FirstOrDefault(p => p.Name == name);
            if (target == null)
            {
                target = typeof(TNamedObject).New<TNamedObject>();
                target.Name = name;
                if (target is IntegratedObject io)
                {
                    io.Code = name;
                }
                //target.Code = name;
                target.EnsureIsValide();

                Set<TNamedObject>().Add(target);
                SaveChanges();
            }
            return target;
        }


        public IEnumerable<TNamedObject> CheckRegistration<TNamedObject>(IEnumerable<string> names)
                where TNamedObject : NamedObject
        {
            List<TNamedObject> results = new();
            foreach (var name in names)
            {
                results.Add(CheckRegistration<TNamedObject>(name));
            }
            return results;
        }


        /// <summary>
        /// Проверка на наличие зарегистрированной записи по уникальному имени
        /// </summary>
        public TNamedObject CheckRegistration<TNamedObject>(TNamedObject current)
            where TNamedObject : NamedObject
        {
            try
            {
                if (current is null)
                    return null;
                var target = Set<TNamedObject>().FirstOrDefault(p => p.Name == current.Name);
                if (target == null)
                {
                    if (current is MessageProtocol mp)
                    {
                        mp.ResetPropertiesMessageAttributeRef();
                    }
                    Set<TNamedObject>().Add(current);
                    SaveChanges();
                    return current;
                }
                return target;
            }
            catch (DbUpdateException ex)
            {
                this.Error($"Исключение при CheckRegistration => {current.ToJsonOnScreen()} => {ex.Message + "\n" + ex.InnerException.Message}");
                throw new Exception($"Исключение при CheckRegistration => {current.ToJsonOnScreen()} => {ex.Message + "\n" + ex.InnerException.Message}");
            }
        }


        public IRepository<T> GetRepository<T>()
            where T : BaseEntity
        {
            return typeof(Repository<,>).MakeGenericType(new Type[] {

                typeof(T),
                GetType()
            }).New<IRepository<T>>(new object[] { this });
        }



        public dynamic GetRepositoryForType(Type entityType)
        {
            var ptype = GetType();
            //this.Info($"Repository<{ptype.GetTypeName()},{entityType.GetTypeName()}>");
            return typeof(Repository<,>).MakeGenericType(

                entityType,
                ptype
            ).New(new object[] { this });
        }



        public IEnumerable<dynamic> Search(string query)
        {
            var words = query.SplitWords().Select(w => w.ToLower().Trim()).ToList();
            return Search(words);
        }

        public IEnumerable<dynamic> Search(IEnumerable<string> keywords)
        {
            List<Task> tasks = new();
            ConcurrentBag<dynamic> results = new();
            foreach (var repository in this.GetEntitiesTypes().Select(GetRepositoryForType))
            {


                try
                {
                    //this.Info(((object)repository).GetTypeName());
                    //if ("Repository<UserSettings,DeliveryDbContext>" == ((object)repository).GetTypeName())
                    //{
                    //    this.Info(123);
                    //}
                    var searchResults = repository.SearchByKeywords(keywords);
                    tasks.Add(Task.Run(() =>
                    {
                        foreach (var p in results)
                        {
                            results.Add(p);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    this.Error(ex);
                }
            }
            Task.WaitAll(tasks.ToArray());
            return results;
        }

 


        public int Create(BaseEntity p)
        {
            Add(p);
            SaveChanges();
            return p.Id;
        }



        public bool ValidateOnChanges = true;



        /// <summary>
        /// Создаёт Businessdatasource
        /// </summary>
        /// <returns></returns>
        public BusinessDatasource GetBusinessDatasource()
        {

            using (var db = new DeliveryDbContext())
            {
                var ds = db.BusinessDatasources.FirstOrDefault(ds => ds.ConnectionString == this.GetConnectionString());
                if (ds is null)
                {
                    db.Add(ds = new BusinessDatasource()
                    {
                        Name = GetType().GetTypeName(),
                        Description = GetType().GetDescription(),
                        ConnectionString = Database.GetConnectionString()
                    });
                    db.SaveChanges();
                }
                return ds;
            }
        }

        private SqlServerWebApi api;
        private IDbConnectionsService dbConnectionsService = new DbConnectionsService();
        private IDataModelService dataModelService = new DataModelService();

        public TreeService treeService = new();

        private ISqlExecutor executor { get => GetSqlServerWebApi(); }
        private IDbMetadata metadata { get => GetSqlServerWebApi(); }
        private IDbMigBuilder migbuilder { get => GetSqlServerWebApi(); }
        private IDDLFactory ddlFactory { get => GetSqlServerWebApi(); }
        private IDMLFactory dmlFactory { get => GetSqlServerWebApi(); }

        public dynamic SetEntity(Type entityType)
        {
            return GetType().GetMethod("Set").MakeGenericMethod(entityType).Invoke(this, new object[0]);
        }


        public string SaveDataModel()
        {
            var dms = new DatabaseModelService();
            return dms.TakeModel(this.GetTypeName(), GetSqlServerWebApi());
        }
        public string CopyDataModel(string name)
        {
            var dms = new DatabaseModelService();
            return dms.TakeModel(name, GetSqlServerWebApi());
        }

        public AppViewBase CreateDbSetView(string entityName)
        {
            var viewType = typeof(DbSetView<,>).MakeGenericType(new Type[] { GetType(), entityName.ToType() });
            return viewType.New<AppViewBase>(this);
        }
        public AppViewBase CreateDbSetView<TEntity>()
        {
            var viewType = typeof(DbSetView<,>).MakeGenericType(new Type[] { GetType(), typeof(TEntity) });
            return viewType.New<AppViewBase>(this);
        }

        public List<EntityRelations> GetEntityRelationsForType(Type typeofEntity)
        {
            var res = this.InvokeGenericMethod(
                nameof(GetEntityRelations), new List<Type>() { typeofEntity }, new List<object>());
            return (List<EntityRelations>)res;

        }

        public List<EntityRelations> GetEntityRelationsForTypeWithExpected(Type typeofEntity, List<string> expected)
        {
            return (List<EntityRelations>)this.InvokeGenericMethod(nameof(GetEntityRelationsWithExpected), new List<Type>() { typeofEntity }, new List<object>() { expected });
        }
        public List<EntityRelations> GetEntityRelationsWithExpected<TEntity>(List<string> expected = null)
        {
            if (expected == null)
            {
                expected = new List<string>();
            }
            List<EntityRelations> r = new();
            var relations = typeof(TEntity).New<BaseEntity>().GetEntityRelations();
            foreach (var rel in relations)
            {
                var relativsEntity = rel.RelativeEntityType;
                if (!expected.Contains(relativsEntity))
                {
                    var nextExp = new List<string>(expected);
                    nextExp.Add(relativsEntity);
                    nextExp.Add(typeof(TEntity).GetTypeName());
                    r.Add(rel);

                    rel.RelativeEntityRelations = GetEntityRelationsForTypeWithExpected(relativsEntity.ToType(), nextExp).ToList();
                }
            }
            this.Info(expected);

            this.Info(relations.Select(r => r.RelativeEntityType).ToJson());
            return r;
        }


        public IDictionary<string, EntityRelations> GetEntityRelationsForType2(Type ptype)
        {
            var results = new Dictionary<string, EntityRelations>();
            var entity = Model.GetEntityTypes().FirstOrDefault(p => p.Name == ptype.GetTypeName());
            if (entity is not null)
            {
                var navigations = entity.GetNavigations();

                foreach (var navigation in navigations)
                {
                    var relations = new EntityRelations()
                    {
                        EntityTypeName = entity.Name,
                        ForeignKey = $"{navigation.ForeignKey?.Properties.FirstOrDefault()?.Name}",
                        PropertyName = navigation.Name,
                        PropertyTypeName = navigation.ClrType.GetTypeName(),
                        RelationshipType = $"{(navigation.IsCollection ? "One-to-Many" : "One-to-One")}"
                    };

                    //определение отношений много-ко-многим
                    var attrs = ptype.GetPropertyAttributes(navigation.PropertyInfo.Name);
                    var kvs = attrs.Where(a => a.Key == nameof(ManyToManyAttribute)).ToList();
                    var manyToManyAttribute = kvs is not null && kvs.Count() > 0 ?
                         kvs.First().Key.ToType().New<ManyToManyAttribute>(new object[] { kvs.First().Value }) : null;
                    if (manyToManyAttribute is not null)
                    {
                        relations.RelativeEntityType = manyToManyAttribute.PropertyName;
                        relations.RelationshipType = "Many-to-Many";
                    }


                    var propertyType = navigation.ClrType.GetTypeName().ToType();
                    if (navigation.IsCollection)
                    {
                        var referencxeEntityType = propertyType.GetGenericArguments()[0];
                        var isCollection = propertyType.IsCollectionType();
                    }
                    results[navigation.Name] = relations;
                }
            }
            return results;
        }
        public List<EntityRelations> GetEntityRelations<TEntity>()
        {
            var results = new List<EntityRelations>();
            var entity = Model.GetEntityTypes().FirstOrDefault(p => p.Name == typeof(TEntity).GetTypeName());
            if (entity is not null)
            {
                var navigations = entity.GetNavigations();

                foreach (var navigation in navigations)
                {
                    var relations = new EntityRelations()
                    {
                        EntityTypeName = entity.Name,
                        ForeignKey = $"{navigation.ForeignKey?.Properties.FirstOrDefault()?.Name}",
                        PropertyName = navigation.Name,
                        PropertyTypeName = navigation.ClrType.GetTypeName(),
                        RelationshipType = $"{(navigation.IsCollection ? "One-to-Many" : "One-to-One")}"
                    };

                    //определение отношений много-ко-многим
                    var attrs = typeof(TEntity).GetPropertyAttributes(navigation.PropertyInfo.Name);
                    var kvs = attrs.Where(a => a.Key == nameof(ManyToManyAttribute)).ToList();
                    var manyToManyAttribute = kvs is not null && kvs.Count() > 0 ?
                         kvs.First().Key.ToType().New<ManyToManyAttribute>(new object[] { kvs.First().Value }) : null;
                    if (manyToManyAttribute is not null)
                    {
                        relations.RelativeEntityType = manyToManyAttribute.PropertyName;
                        relations.RelationshipType = "Many-to-Many";
                    }



                    var propertyType = navigation.ClrType.GetTypeName().ToType();
                    if (navigation.IsCollection)
                    {
                        var referencxeEntityType = propertyType.GetGenericArguments()[0];
                        var isCollection = propertyType.IsCollectionType();
                    }
                    results.Add(relations);
                }
            }
            return results;
        }

        public void AddOrUpdate(BaseEntity entity)
        {

            if (entity is NamedObject)
            {
                var nameValue = ((NamedObject)entity).Name;

                dynamic fasade = entity.GetType().GetNamedEntityFasadeForType();
                var current = fasade.FindByName(nameValue);


                if (current is null)
                {
                    Add(entity);
                    SaveChanges();
                }
                else
                {
                    entity.Id = current.Id;
                    ((object)current).SetProperties(entity.ToTextDictionary());
                    //this.Info(((object)current));
                    this.Update(current);
                    SaveChanges();
                }

            }
            else
            {
                if (entity.Id == 0)
                {

                    Add(entity);
                    SaveChanges();
                }
                else
                {
                    var data = entity.GetType().GetEntityFasade<BaseEntity>().Get(entity.Id);
                    data.SetProperties(entity.ToTextDictionary());
                    Update(data);
                    SaveChanges();
                }
            }

        }


        public ProcedureModel GetProcedureModel([NotNullNotEmpty] string schema, [NotNullNotEmpty] string name)
        {
            return GetProcedureModel(api.ToString(), api.Database, schema, name);
        }

        private static ConcurrentDictionary<Type, SqlServerWebApi> APIS = new();

        public SqlServerWebApi GetSqlServerWebApi()
        {
            lock (APIS)
            {
                if (!APIS.ContainsKey(GetType()))
                {
                    APIS[GetType()] = new SqlServerWebApi();
                }
            }
            api = APIS[GetType()];
            api.Configure(GetConnectionString());
            var databaseName = GetDatabaseName();
            api.Database = "master";
            var names = api.GetDatabaseNames();
            //this.Info(api.ToString());
            //this.Info(names);
            //this.Info(databaseName);
            if (!names.Contains(databaseName))
            {
                api.CreateDatabase(databaseName);
            }
            api.Database = databaseName;
            if (api is null)
            {

                if (!api.CanConnect())
                {

                    api.Database = "master";
                    if (!api.GetDatabaseNames().Select(p => p.ToLower()).Contains(databaseName.ToLower()))
                        api.PrepareQuery($"CREATE DATABASE {databaseName}");
                    api.Database = databaseName;
                    if (!api.CanConnect())
                        throw new Exception($"Не удалось соединиться с: {GetConnectionString()}");
                }

            }
            return api;
        }
        public SqlServerWebApi GetSqlServerWebApi(string connectionString)
        {
            var api = new SqlServerWebApi();
            api.Configure(connectionString);
            var parameters = ParseConnectionString(connectionString);
            //this.Info(parameters.ToJsonOnScreen());
            var connected1 = api.CanConnect();
            return api;
        }

        public Dictionary<string, string> ParseConnectionString(string connectionString)
        {
            Dictionary<string, string> r = new();
            foreach (string s in connectionString.Split(";"))
            {
                var arr = s.Split("=");
                r[arr[0]] = arr.Length > 1 ? arr[1] : "";
            }
            return r;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {
                base.OnModelCreating(modelBuilder);
                
                
                var message = $"{GetType().GetTypeName()}.{nameof(OnModelCreating)}";
                this.Debug(message);
                
                var tasks = new List<Task>();

                // Обход всех классов сущностей, зарегистрированных в контексте
                foreach (var entityType in this.GetEntitiesTypes())
                {
                        this.Debug($"{nameof(OnModelCreating)} {entityType.GetTypeName()}");
                        var attributes = entityType.GetAttributes().Where(attr => attr.Key.ToType().IsExtends<EntityAttributeBase>());
                        foreach (var kv in attributes)
                        {
                            var attributeType = kv.Key.ToType();
                            var entityAttribute = attributeType.New<EntityAttributeBase>(new object[] { kv.Value });
                            entityAttribute.OnModelCreating(modelBuilder, entityType, this);
                        }

                        foreach (var property in entityType.GetProperties())
                        {
                            this.Debug($"{nameof(OnModelCreating)} {entityType.GetTypeName()} {property.Name}");
                            var propertyAttributes = entityType.GetPropertyAttributes(property.Name);
                            var entityAttributes = propertyAttributes.Where(attr => attr.Key.ToType().IsExtends<EntityAttributeBase>());
                            foreach (var kv in entityAttributes)
                            {
                                this.Debug($"{nameof(OnModelCreating)} {entityType.GetTypeName()} {property.Name} {kv.Key}");
                                if(property.Name == "ValidationSuccess")
                                {
                                    this.Debug(1);
                                }
                                var attributeType = kv.Key.ToType();
                                if (attributeType is not null)
                                {
                                    var entityAttribute = attributeType.New<EntityAttributeBase>(new object[] { kv.Value });
                                    entityAttribute.OnModelCreating(modelBuilder, entityType, this, property.Name);
                                }
                            }
                        }
                }                
            }
            catch( Exception ex )
            {
                var message = $"{GetType().GetTypeName()}.{nameof(OnModelCreating)} => {ex.Message}";
                this.Error(message);
                throw new Exception(message);
            }
        }

         
         
        public class BaseDbCommandInterceptor : DbCommandInterceptor { }

        public static void OnConfiguring<TDbContextBase>(DbContextOptionsBuilder optionsBuilder)
            where TDbContextBase : DbContextBase
        {
            try
            {
                var message = $"{typeof(TDbContextBase).GetTypeName()}.{nameof(OnConfiguring)}";
                optionsBuilder.Debug(message);
                if (!optionsBuilder.IsConfigured)
                {
                    string connectionString = GetConnectionString<TDbContextBase>();
                    optionsBuilder.UseSqlServer(connectionString);                    
                    optionsBuilder.AddInterceptors(new BaseDbCommandInterceptor());                    
                    
                    var api = new SqlServerWebApi();
                    api.Configure(connectionString);
                    if (string.IsNullOrWhiteSpace(api.Database))
                        throw new Exception("Не задано имя базы данных");

                    var database = api.Database;


                    //v this.Info(api.GetDatabaseNames());                
                    if (!api.GetDatabaseNames().Contains(database))
                    {
                        api.Database = "model";
                        api.CreateDatabase(database);
                        api.Database = database;
                    }
                    api.Database = database;
                }
                optionsBuilder.Debug(message + " COMPLETED");
                //optionsBuilder.EnableSensitiveDataLogging();
                //optionsBuilder.EnableDetailedErrors();
            }
            catch(Exception ex)
            {
                var message = $"{typeof(TDbContextBase).GetTypeName()}.{nameof(OnConfiguring)} => {ex.Message}";
                optionsBuilder.Error(message);
                throw new Exception(message);
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //            this.Info($"Настройка {GetType().GetTypeName()}");

            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = GetConnectionString();
                optionsBuilder.UseSqlServer(connectionString);
                optionsBuilder.AddInterceptors(new BaseDbCommandInterceptor());

                api = GetSqlServerWebApi();
                if (string.IsNullOrWhiteSpace(api.Database))
                    throw new Exception("Не задано имя базы данных");

                var database = GetDatabaseName();


                //v this.Info(api.GetDatabaseNames());                
                if (!api.GetDatabaseNames().Contains(database))
                {
                    api.Database = "model";
                    api.CreateDatabase(database);
                    api.Database = database;
                }
                api.Database = database;
            }
            //optionsBuilder.EnableSensitiveDataLogging();
            //optionsBuilder.EnableDetailedErrors();

        }

        public virtual string GetConnectionString()
        {
            if (connectionString is not null)
                return connectionString;
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"DESKTOP-IHJM9RD";
            var databaseName = GetDatabaseName();
            //this.Info(databaseName);
            builder.InitialCatalog = databaseName;
            builder.TrustServerCertificate = true;
            builder.IntegratedSecurity = true;
            builder.MultipleActiveResultSets = true;
            return builder.ToString();
        }

        public static string GetConnectionString<T>()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"DESKTOP-IHJM9RD";
            var databaseName = typeof(T).GetTypeName().Replace("<", "").Replace(">", "").Replace(",", "");
            //this.Info(databaseName);
            builder.InitialCatalog = databaseName;
            builder.TrustServerCertificate = true;
            builder.IntegratedSecurity = true;
            builder.MultipleActiveResultSets = true;
            return builder.ToString();
        }

        private string GetDatabaseName()
        {
            return GetType().GetTypeName().Replace("<", "").Replace(">", "").Replace(",", "");
        }

        private List<TResult> ExecuteQuery<TRequest, TResult>(TRequest request, string sql) where TResult : class
        {
            using (IDbConnection con = new SqlConnection(api.ToString()))
            {
                con.Open();
                var results = con.QueryAsync<TResult>(sql, request).Result;
                return results.ToList();
            }
        }



        /// <summary>
        /// Фиксация изменений 
        /// </summary>
        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            //ApplyIntegrationServices();
            OnSaveChanges();
            try
            {
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Исключение при сохранении данных: {GetType().GetTypeName()} => {this.Database.GetConnectionString()} => {ex.Message} => {ex.InnerException?.Message}");
            }
        }

        public int SaveChangesWithoutIntegration()
        {
            try
            {
                ChangeTracker.DetectChanges();
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Исключение при сохранении данных: {GetType().GetTypeName()} => {this.Database.GetConnectionString()} => {ex.Message} => {ex.InnerException?.Message}");
            }
        }

        public int SaveChangesWithLateIntegration()
        {
            try
            {
                ChangeTracker.DetectChanges();
                var changes = GetChanges();
                var result = base.SaveChanges();
                ApplyIntegrationEvents(changes);

                return result;

            }
            catch (Exception ex)
            {
                throw new Exception($"Исключение при сохранении данных: {GetType().GetTypeName()} => {this.Database.GetConnectionString()} => {ex.Message} => {ex.InnerException?.Message}");
            }
        }


        private void ApplyIntegrationServices()
        {


            foreach (EntityEntry entry in ChangeTracker.Entries().ToList())
            {
                var entityType = entry.Entity.GetType();
                if (entry.State == EntityState.Unchanged)
                    continue;
                if (entry.State == EntityState.Detached)
                    continue;

                var serviceType = typeof(IntegrationService<,>).MakeGenericType(new Type[] { GetType(), entityType });
                //this.Info(serviceType.GetTypeName());
                var integrationServices = this.GetDbSetServices(entityType.GetTypeName(), serviceType);
                //this.Info(integrationServices.Select(p => p.GetTypeName()).ToJsonOnScreen());
                var serviceRefs = integrationServices.Select(ptype => ptype.New(new object[] { this })).ToList();

                foreach (var serviceRef in serviceRefs)
                {
                    var primary = new Dictionary<string, object>();
                    var props = new Dictionary<string, object>();
                    foreach (PropertyEntry property in entry.Properties)
                    {
                        var pkey = property.Metadata.Name;
                        var _val = property.OriginalValue;
                        var pval = property.CurrentValue;
                        props[pkey] = pval;
                        primary[pkey] = _val;
                    }
                    serviceRef.Invoke("OnDbSetChanged", new List<object>()
                    {
                        entry.State,
                            primary,
                            props
                    });
                }
            }
        }

        public class EntityChangedEvent : EventArgs
        {
            public enum EntityChangedEventType { Add, Delete, Update }
            public EntityChangedEventType Type { get; set; }

            [System.Text.Json.Serialization.JsonIgnore]
            [Newtonsoft.Json.JsonIgnore]
            public Type Entity { get; set; }
            public object Before { get; set; }
            public object After { get; set; }
        }

        public List<EntityChangedEvent> GetChanges()
        {
            List<EntityChangedEvent> changes = new();
            foreach (EntityEntry entry in ChangeTracker.Entries().ToList())
            {
                if (entry.State == EntityState.Unchanged)
                    continue;
                var primary = new Dictionary<string, object>();
                var props = new Dictionary<string, object>();
                foreach (PropertyEntry property in entry.Properties)
                {
                    var pkey = property.Metadata.Name;
                    var _val = property.OriginalValue;
                    var pval = property.CurrentValue;
                    props[pkey] = pval;
                    primary[pkey] = _val;
                }
                var entityType = entry.Entity.GetType();
                var entityRef = entityType.New();
                entityRef.Set(props);

                var changesType = entry.State;
                switch (entry.State)
                {
                    case EntityState.Modified:
                        changes.Add(new EntityChangedEvent()
                        {
                            Type = EntityChangedEvent.EntityChangedEventType.Update,
                            Before = null,
                            After = entityRef,
                            Entity = entityType
                        });
                        break;
                    case EntityState.Added:
                        changes.Add(new EntityChangedEvent()
                        {
                            Type = EntityChangedEvent.EntityChangedEventType.Add,
                            Before = null,
                            After = entityRef,
                            Entity = entityType
                        });
                        break;
                    case EntityState.Deleted:
                        changes.Add(new EntityChangedEvent()
                        {
                            Type = EntityChangedEvent.EntityChangedEventType.Delete,
                            Before = entityRef,
                            After = null,
                            Entity = entityType
                        });
                        break;
                }
                if (ValidateOnChanges)
                {
                    try
                    {
                        var validationResults = ValidateData(entityRef);
                        if (validationResults.Any(kv => kv.Value.Count() > 0))
                        {
                            this.Error($"Объект {entityRef.GetTypeName()} не валидный {entityRef.ToJsonOnScreen()}\n  => \n{validationResults.ToJsonOnScreen()}");
                            throw new Exception($"Объект {entityRef.GetTypeName()} не валидный {entityRef.ToJsonOnScreen()}\n  => \n{validationResults.ToJsonOnScreen()}");
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Error($"Ошибка при валидации: {ex.Message}");
                    }
                }
            }

            return changes;
        }

        private Dictionary<string, List<string>> ValidateData(object p)
        {

            var started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            object target = p;
            if (p is null)
                return result;

            var attrs = p.GetType().GetAttributes();
            List<Task> tasks = new();
            foreach (var name in target.GetType().GetInputProperties())
            {

                var property = target.GetType().GetProperties().FirstOrDefault(p => p.Name == name);
                string key = property.Name;

                if (property.PropertyType.IsPrimitive())
                {
                    object value = p.GetValue(key);
                    var attributes = target.GetType().GetPropertyAttributes(key);

                    List<string> errors = p.Validate(key, value, attributes, true);

                    if (errors.Count > 0)
                    {
                        result[key] = errors;
                    }
                    var stamp = DateTimeOffset.Now.ToUnixTimeMilliseconds() - started;
                    //p.Info(key + " " + stamp);
                }

            }

            //p.Info(result.ToJson());
            Dictionary<string, List<string>> optional = new Dictionary<string, List<string>>();
            var validateOptional = target.GetType().GetMethod("ValidateOptional");
            if (validateOptional is not null)
            {
                optional = (Dictionary<string, List<string>>)validateOptional.Invoke(target, new object[0]);
            }

            foreach (var pkv in optional)
            {
                if (result.ContainsKey(pkv.Key))
                {
                    result[pkv.Key].AddRange(optional[pkv.Key]);
                }
                else
                {
                    result[pkv.Key] = optional[pkv.Key];
                }
            }
            var ended = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            //p.Info($"Валидация объекта {target.GetTypeName()} выполнилась за {ended - started} мс");
            if (p is MyValidatableObject mvb)
            {
                mvb.ValidationResults = result;
                //p.Info(result);
                var success = mvb.ValidationResults.Any(kv => kv.Value is not null && kv.Value.Count() > 0);
                if (success != mvb.ValidationSuccess)
                {
                    mvb.OnValidationStateChanged(mvb.ValidationSuccess = success);
                }
                mvb.ValidationSuccess = success;
            }
            return result;
        }

        private void OnSaveChanges()
        {
            if (EnableDataChangedAttributes == false)
                return;

            ConcurrentDictionary<string, List<MethodInfo>> map = GetMethodsWithDataChangedAttributes();

            foreach (EntityEntry entry in ChangeTracker.Entries().ToList())
            {
                if (entry.State == EntityState.Unchanged)
                    continue;
                var primary = new Dictionary<string, object>();
                var props = new Dictionary<string, object>();
                foreach (PropertyEntry property in entry.Properties)
                {
                    var pkey = property.Metadata.Name;
                    var _val = property.OriginalValue;
                    var pval = property.CurrentValue;
                    props[pkey] = pval;
                    primary[pkey] = _val;
                }

                this.Info(entry.Entity.GetTypeName());

                var entityType = entry.Entity.GetType();
                List<Type> integrationServices = GetIntegrationServices(entityType);
                var serviceRefs = integrationServices.Select(ptype => ptype.New(new object[] { this })).ToList();
                foreach (var serviceRef in serviceRefs)
                {
                    serviceRef.Invoke("OnDbSetChanged", new List<object>()
                    {
                        entry.State,
                        primary,
                        props
                    });
                }

                var entityRef = entityType.New();
                entityRef.Set(props);

                if (ValidateOnChanges)
                {
                    try
                    {
                        var validationResults = ValidateData(entityRef);
                        if (validationResults.Any(kv => kv.Value.Count() > 0))
                        {
                            this.Error($"Объект {entityRef.GetTypeName()} не валидный {entityRef.ToJsonOnScreen()}\n  => \n{validationResults.ToJsonOnScreen()}");
                            throw new Exception($"Объект {entityRef.GetTypeName()} не валидный {entityRef.ToJsonOnScreen()}\n  => \n{validationResults.ToJsonOnScreen()}");
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Error($"Ошибка при валидации: {ex.Message}");
                    }

                }


                List<OnDataChangedAttribute> attrs = new();
                var key = entityType.GetTypeName();
                List<MethodInfo> listeners = new();
                if (map.ContainsKey(key))
                    listeners = map[key];
                var methodNames = listeners.Select(l => l.Name).ToList();

                //this.Debug($"{primary.ToJsonOnScreen()} => {props.ToJson()}");
                //this.Debug($"OnSaveChanges() => {entityType.GetTypeName()} => {methodNames.ToJson()}");
                try
                {
                    switch (entry.State)
                    {
                        case EntityState.Deleted:
                            foreach (var method in listeners)
                            {
                                method.Invoke(this, new object[] { EntityState.Deleted, primary, props });
                            }
                            break;
                        case EntityState.Modified:
                            foreach (var method in listeners)
                            {

                                method.Invoke(this, new object[] { EntityState.Modified, primary, props });
                            }
                            break;
                        case EntityState.Added:
                            foreach (var method in listeners)
                            {
                                //this.Info(method.Name);

                                method.Invoke(this, new object[] { EntityState.Added, primary, props });
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    this.Error();
                }


            }
        }


        private void ApplyIntegrationEvents(List<EntityChangedEvent> events)
        {

            if (EnableDataChangedAttributes == false)
                return;

            ConcurrentDictionary<string, List<MethodInfo>> map = GetMethodsWithDataChangedAttributes();

            foreach (var nextEvent in events)
            {
                try
                {
                    this.Info(nextEvent);
                    List<Type> integrationServices = GetIntegrationServices(nextEvent.Entity);
                    var serviceRefs = integrationServices.Select(ptype => ptype.New(new object[] { this })).ToList();
                    EntityState entityState =
                        nextEvent.Type == EntityChangedEvent.EntityChangedEventType.Update ? EntityState.Modified :
                        nextEvent.Type == EntityChangedEvent.EntityChangedEventType.Delete ? EntityState.Deleted :
                        nextEvent.Type == EntityChangedEvent.EntityChangedEventType.Add ? EntityState.Added :
                        throw new UnsupportedException(nextEvent.Type.ToString());

                    var primary = nextEvent.Before is null ? new Dictionary<string, object>() : nextEvent.Before.ToDictionary();

                    var props = nextEvent.After is null ? new Dictionary<string, object>() : nextEvent.After.ToDictionary();
                    foreach (var serviceRef in serviceRefs)
                    {
                        serviceRef.Invoke("OnDbSetChanged", new List<object>()
                        {
                            entityState,
                            primary,
                            props
                        });
                    }

                    List<OnDataChangedAttribute> attrs = new();
                    var key = nextEvent.Entity.GetTypeName();
                    List<MethodInfo> listeners = new();
                    if (map.ContainsKey(key))
                        listeners = map[key];
                    List<string> methodNames = listeners.Select(l => l.Name).ToList();
                    //this.Debug(methodNames);

                    switch (entityState)
                    {
                        case EntityState.Deleted:
                            foreach (var method in listeners)
                            {
                                try
                                {
                                    method.Invoke(this, new object[] { EntityState.Deleted, primary, props });
                                }
                                catch (Exception ex)
                                {
                                    this.Error($"Исключение при выполнении триггера: {method.Name}: {ex.Message}");
                                    throw;
                                }
                            }
                            break;
                        case EntityState.Modified:
                            foreach (var method in listeners)
                            {

                                try
                                {
                                    method.Invoke(this, new object[] { EntityState.Modified, primary, props });
                                }
                                catch (Exception ex)
                                {
                                    this.Error($"Исключение при выполнении триггера: {method.Name}: {ex.Message}");
                                    throw;
                                }
                            }
                            break;
                        case EntityState.Added:
                            foreach (var method in listeners)
                            {
                                //this.Info(method.Name);

                                try
                                {
                                    method.Invoke(this, new object[] { EntityState.Added, primary, props });
                                }
                                catch (Exception ex)
                                {
                                    this.Error($"Исключение при выполнении триггера: {method.Name}: {ex.Message}");
                                    throw;
                                }
                            }
                            break;
                    }
                }
                catch (TargetInvocationException ex)
                {
                    this.Error($"Исключение при обработки события изменения данных: {nextEvent.ToJson()} => {ex.InnerException.Message}");
                }
                catch (Exception ex)
                {
                    this.Error($"Исключение при обработки события изменения данных: {nextEvent.ToJson()} => {ex.Message}");
                }


            }
        }

        public List<Type> GetIntegrationServices(Type entityType)
        {
            var serviceType = typeof(IntegrationService<,>).MakeGenericType(new Type[] { GetType(), entityType });
            this.Info(serviceType.GetTypeName());
            var integrationServices = this.GetDbSetServices(entityType.GetTypeName(), serviceType);
            return integrationServices;
        }

        public ConcurrentDictionary<string, List<MethodInfo>> GetMethodsWithDataChangedAttributes()
        {
            //ищем методы помеченные атрибутами связывания
            //this.Info($"{GetType().GetTypeName()}");

            ConcurrentDictionary<string, List<MethodInfo>> map = new();
            try
            {
                foreach (var m in GetType().GetMethods().Where(m => !m.IsStatic))
                {
                    if (m.Name.StartsWith("get_")) continue;
                    if (m.Name.StartsWith("set_")) continue;
                    var attrs = GetType().GetMethodAttributes(m.Name);
                    if (attrs is null)
                    {
                        throw new Exception();
                    }
                    //this.Info($"{m.Name}={attrs.ToJson()}");
                    var events = attrs.Where(kv => kv.Key is not null && kv.Key.ToType().IsExtends(typeof(OnDataChangedAttribute))).Select(kv => kv.Key.ToType().New<OnDataChangedAttribute>(new object[] { kv.Value }));
                    events.ToList().ForEach(p =>
                    {
                        var entityType = p.GetType().GetGenericArguments().FirstOrDefault();
                        var key = entityType.GetTypeName();
                        if (!map.ContainsKey(key))
                        {
                            map[key] = new();
                        }
                        map[key].Add(m);
                    });
                }
            }
            catch (Exception ex)
            {
                this.Error(ex);
            }

            return map;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> ValidateOptional()
        {
            var res = new Dictionary<string, List<string>>();
            if (api is null)
            {
                api = new SqlServerWebApi();
                api.Configure(GetConnectionString());
                var databaseName = api.Database;
                api.Database = "model";
                api.CreateDatabase(databaseName);
                api.Database = databaseName;
                if (!api.CanConnect())
                    throw new Exception($"Не удалось соединиться с: {GetConnectionString()}");
            }

            if (!api.CanConnect())
            {
                res["body"] = new List<string>() { $"Не удалось установить соединение: {ToString()}" };
            }
            var ctrl = this;
            List<string> entities = GetType().GetProperties().Where(p => p.PropertyType.GetTypeName().StartsWith("DbSet<")).Select(p => p.Name).ToList();
            //this.Info(entities.ToJsonOnScreen());
            var existing = GetTableNames().Select(p => p.ToLower()).ToList();
            this.Info($"Существуют таблицы:\n{existing.ToJsonOnScreen()}");
            var notCreated = entities.Select(p => p.ToLower()).ToHashSet().Except(existing).ToList();

            if (notCreated.Count() > 0)
            {
                Database.EnsureCreated();
                /*foreach (var entity in notCreated) {
                    res[entity] = new() { $"Таблицы с именем {entity} не существует." };
                }*/
            }

            //InitDbContextExtensible(ctrl);
            return res;
        }

        private void InitDbContextExtensible(DbContextBase ctrl)
        {
            var exts = GetType().GetExtendedTypes().Where(p => p != typeof(DbContextBase) && p.IsExtendsFrom(typeof(DbContextBase)));
            foreach (var ext in exts)
            {
                var method = GetType().GetMethod(nameof(GetDbContextInitializer)).MakeGenericMethod(ext);
                dynamic initiallizer = method.Invoke(ctrl, null);
                initiallizer.Init(this);
            }
        }

        public DbContextInitiliazer<TDbContext> GetDbContextInitializer<TDbContext>() where TDbContext : DbContextBase
        {
            var p = ServiceFactory.Get().GetTypesExtended<DbContextInitiliazer<TDbContext>>().FirstOrDefault();
            if (p is not null)
                return p.New<DbContextInitiliazer<TDbContext>>();
            return null;
        }

        public JArray GetJsonResult(string sql)
        {
            return executor.GetJsonResult(sql);
        }

        public JObject GetSingleJObject(string command)
        {
            return executor.GetSingleJObject(command);
        }

        public DataTable ExecuteQuery(string command)
        {
            return executor.ExecuteQuery(command);
        }

        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string command) where TEntity : class
        {
            return executor.ExecuteQuery<TEntity>(command);
        }

        public IEnumerable<dynamic> ExecuteQuery(string command, Type entity)
        {
            return executor.ExecuteQuery(command, entity);
        }

        public int TryPrepareQuery(string command)
        {
            return executor.TryPrepareQuery(command);
        }

        public int PrepareQuery(string command)
        {
            return executor.PrepareQuery(command);
        }

        public DataTable ExecuteProcedure(string name, IDictionary<string, object> input)
        {
            return executor.ExecuteProcedure(name, input);
        }

        public int ExecuteProcedure(string command, List<object> input, IDictionary<string, string> output)
        {
            return executor.ExecuteProcedure(command, input, output);
        }

        public int ExecuteProcedure(string command, IDictionary<string, string> input, IDictionary<string, string> output)
        {
            return executor.ExecuteProcedure(command, input, output);
        }

        public IEnumerable<string> GetTableNames()
        {
            return GetSqlServerWebApi().GetTableNames();
        }

        public IEnumerable<string> GetSchemasNames()
        {
            return GetSqlServerWebApi().GetSchemasNames();
        }

        public IDictionary<string, TableMetadata> GetTablesMetadata()
        {
            return GetSqlServerWebApi().GetTablesMetadata();
        }

        public EfEntityFasade<TEntity> GetEntityFasade<TEntity>()
            where TEntity : BaseEntity
            => new EfEntityFasade<TEntity>(this);

        public IQueryable<TEntity> GetDataset<TEntity>(params Predicate<TEntity>[] filters)
            where TEntity : BaseEntity
        {
            return GetEntityFasade<TEntity>().Filter(filters);
        }


        public IDictionary<string, ColumnMetadata> GetColumnsMetadata(string TableSchema, string TableName)
        {
            return GetSqlServerWebApi().GetColumnsMetadata(TableSchema, TableName);
        }

        public IEnumerable<KeyMetadata> GetKeysMetadata()
        {
            return GetSqlServerWebApi().GetKeysMetadata();
        }


        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata(string Schema = null)
        {
            return GetSqlServerWebApi().GetProceduresMetadata(Schema);
        }

        public ProcedureMetadata GetProcedureMetadata(string SchemaName, string ProcedureName)
        {
            return GetSqlServerWebApi().GetProcedureMetadata(SchemaName, ProcedureName);
        }
        public IDictionary<string, ParameterMetadata> GetParameters(string SchemaName, string ProcedureName)
        {
            return ((SqlServerDbMetadata)metadata).GetParameters(SchemaName, ProcedureName);
        }
        public IDictionary<string, ParameterMetadata> GetParametersMetadata(string SchemaName, string ProcedureName)
        {
            return GetSqlServerWebApi().GetParametersMetadata(SchemaName, ProcedureName);
        }

        public void UpdateDatabase()
        {
            migbuilder.UpdateDatabase();
        }

        public string CreateForeignkey(string relativeTable, string table, string column, bool? onDeleteCascade = false, bool? onUpdateCascade = null)
        {
            return ddlFactory.CreateForeignkey(relativeTable, table, column, onDeleteCascade, onUpdateCascade);
        }




        public string CreateTable(TableInfo metadata)
        {
            return ddlFactory.CreateTable(metadata);
        }

        public string CreateInsertStatement(string schema, string table, Dictionary<string, object> values)
        {
            return dmlFactory.CreateInsertStatement(schema, table, values);
        }

        public string CreateSelectStatement(string schema, string table)
        {
            return migbuilder.CreateSelectStatement(schema, table);
        }

        public string CreateUpdateStatement(string schema, string table, Dictionary<string, object> values)
        {
            return dmlFactory.CreateUpdateStatement(schema, table, values);
        }

        public string CreateCreateStatement(string schema, string table)
        {
            return migbuilder.CreateCreateStatement(schema, table);
        }

        public override void Dispose()
        {
            //this.Debug("Освобожден экземпляр");
            base.Dispose();
        }

        public override ValueTask DisposeAsync()
        {
            //this.Debug("Освобожден экземпляр");
            return base.DisposeAsync();
        }

        public DatabaseModel GetDatabase(string connectionString)
        {
            return dbConnectionsService.GetDatabase(connectionString);
        }

        public int GetDatabaseId(string connectionString)
        {
            return dbConnectionsService.GetDatabaseId(connectionString);
        }

        public DatabaseConnectionString GetDbConnection(string server)
        {
            return dbConnectionsService.GetDbConnection(server);
        }


        public ProcedureModel GetProcedureModel([NotNullNotEmpty] string connectionString, [NotNullNotEmpty] string database, [NotNullNotEmpty] string schema, [NotNullNotEmpty] string name)
        {
            return dbConnectionsService.GetProcedureModel(connectionString, database, schema, name);
        }

        public EditView CreateExecuterForm(string schema, string name)
        {
            var metadata = GetProcedureMetadata(schema, name);
            var formfields = GetProcedureFormFields(schema, name);
            var view = new EditView();
            view.UpdateFormFields(formfields);
            return view;
        }

        public int AddParameterAttributes(int parameterId, Dictionary<string, string> attributes)
        {
            return dataModelService.AddParameterAttributes(parameterId, attributes);
        }

        public Dictionary<string, string> GetParameterAttributes(int parameterId)
        {
            return dataModelService.GetParameterAttributes(parameterId);
        }

        public Dictionary<string, InputFormField> GetProcedureFormFields(int procedureId)
        {
            return dataModelService.GetProcedureFormFields(procedureId);
        }

        public Dictionary<string, InputFormField> GetProcedureFormFields(string schema, string name)
        {
            var model = GetProcedureModel(schema, name);
            return dataModelService.GetProcedureFormFields(model.Id);
        }

        public TreeControlView CreateTreeView<TEntity>(IQueryable<TEntity> dbset, Action<TEntity> onclicked) where TEntity : HierTable<TEntity>
        {
            return ((ITreeService)treeService).CreateTreeView(dbset, onclicked);
        }

        public TreeControlView CreateTreeView<TEntity>(Action<TEntity> onclicked) where TEntity : HierTable<TEntity>
        {
            return ((ITreeService)treeService).CreateTreeView(Set<TEntity>(), onclicked);
        }


        public ListView CreateSelectList<TEntity>(Action<TEntity> onselected) where TEntity : BaseEntity
        {
            var list = new ListView();
            list.Options.Clear();
            foreach (var p in Set<TEntity>())
            {
                list.Options.Add(p is NamedObject
                    ? new Option() { Value = p.GetValue("Name").ToString(), Item = p }
                    : new Option() { Value = p.ToString(), Item = p });
            }
            list.OnSelected += (p) =>
            {
                if (p is Option o)
                    if (onselected is not null)
                        onselected((TEntity)o.Item);
            };
            return list;
        }
        public TreeControlView CreateTreeViewRelations<TDbContext, TEntity1, TEntity2>(Func<TEntity1, TreeViewNodeModel> c1, Func<TEntity2, TreeViewNodeModel> c2) where TEntity1 : BaseEntity where TEntity2 : BaseEntity where TDbContext : DbContextBase
        {
            var builder = new TreeViewEntityBuilder<TDbContext>();
            builder.Join(c1);
            builder.Join(c2);
            return builder.Build();
        }

        public TreeControlView CreateTreeViewRelations<TDbContext, TEntity1, TEntity2>() where TEntity1 : NamedObject where TEntity2 : NamedObject where TDbContext : DbContextBase
        {
            var builder = new TreeViewEntityBuilder<TDbContext>();
            builder.Join<TEntity1>();
            builder.Join<TEntity2>();
            var view = builder.Build();
            return view;
        }


        public TreeControlView CreateTreeViewRelations<TDbContext, TEntity1, TEntity2, TEntity3>(Func<TEntity1, TreeViewNodeModel> c1, Func<TEntity2, TreeViewNodeModel> c2, Func<TEntity3, TreeViewNodeModel> c3)
            where TEntity1 : BaseEntity
            where TEntity2 : BaseEntity
            where TEntity3 : BaseEntity
            where TDbContext : DbContextBase
        {
            var builder = new TreeViewEntityBuilder<TDbContext>();
            builder.Join(c1);
            builder.Join(c2);
            builder.Join(c3);
            return builder.Build();
        }

        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata()
        {
            return GetSqlServerWebApi().GetProceduresMetadata();
        }

        public IEnumerable<TResult> ExecuteProcedure<TResult>([InputEngText, NotNullNotEmpty] string name, IDictionary<string, string> input) where TResult : class
        {
            return GetSqlServerWebApi().ExecuteProcedure<TResult>(name, input);
        }

        public IDictionary<string, ParameterMetadata> GetParametersMetadata()
        {
            return GetSqlServerWebApi().GetParametersMetadata();
        }



        public IEnumerable<string> GetDatabaseNames()
        {
            return GetSqlServerWebApi().GetDatabaseNames();
        }


        public string CreateTable(Type metadata)
        {
            return ddlFactory.CreateTable(metadata);
        }

        public TableInfo CreateTableMetaData(Type metadata)
        {
            return ddlFactory.CreateTableMetaData(metadata);
        }

        public List<string> GetProcedureSql(string schema, string name)
        {
            return GetSqlServerWebApi().GetProceduresSqlCode($"[{schema}].[{name}]").ToList();
        }





        public IDictionary<string, TableMetadata> UpdateTablesMetadata()
        {
            return migbuilder.UpdateTablesMetadata();
        }

        public IEnumerable<Dictionary<string, object>> GetResultSetDictionary(DataTable dataTable)
        {
            return executor.GetResultSetDictionary(dataTable);
        }



        public string AddColumnToTable(string fullTableName, string propertyName, string type, bool nullable)
        {
            return ddlFactory.AddColumnToTable(fullTableName, propertyName, type, nullable);
        }

        public string CreateForeignKeys(TableInfo metadata)
        {
            return ddlFactory.CreateForeignKeys(metadata);
        }

        public string CreateSchema(string schema)
        {
            return ddlFactory.CreateSchema(schema);
        }

        public string DropColumnFromTable(string fullTableName, string propertyName)
        {
            return ddlFactory.DropColumnFromTable(fullTableName, propertyName);
        }

        public string DropTable(TableInfo metadata)
        {
            return ddlFactory.DropTable(metadata);
        }

        public IDictionary<string, IDictionary<string, ParameterMetadata>> GetProceduresParametersMetadata()
        {
            return GetSqlServerWebApi().GetProceduresParametersMetadata();
        }

        public IDbCommand CreateCommand<TIDbCommand>()
        {
            return GetSqlServerWebApi().CreateCommand<TIDbCommand>();
        }

        public IDbCommand CreateCommand()
        {
            return GetSqlServerWebApi().CreateCommand();
        }

        public void CreateDatabase()
        {
            GetSqlServerWebApi().CreateDatabase();
        }

        public int CreateDatabase(string name)
        {
            return GetSqlServerWebApi().CreateDatabase(name);
        }

        public DbMigCommand[] DropAndCreate()
        {
            return GetSqlServerWebApi().DropAndCreate();
        }

        public void TestProcedures()
        {
            GetSqlServerWebApi().TestProcedures();
        }

        public Type[] GetEntityClasses()
        {
            return GetSqlServerWebApi().GetEntityClasses();
        }

        public void AddEntityType(Type entity)
        {
            GetSqlServerWebApi().AddEntityType(entity);
        }

        public string GetCurrentVersion()
        {
            return GetSqlServerWebApi().GetCurrentVersion();
        }

        public string GetTargetVersion()
        {
            return GetSqlServerWebApi().GetTargetVersion();
        }

        public ChangingList<Type> EntityTypes { get => ((IdbModel)api).EntityTypes; }

        public AppViewBase ToView()
        {
            throw new NotImplementedException();
        }
    }
}
