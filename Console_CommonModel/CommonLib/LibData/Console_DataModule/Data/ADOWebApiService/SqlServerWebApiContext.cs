using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbModelService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.ProductsFilters;
using Console_CommonModel.CommonLib.LibModels.Console_WebModule;

using Microsoft.EntityFrameworkCore;

using System.Collections;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.Connectors;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using static Dapper.SqlMapper;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService
{

    public class SqlServerWebApiContextBase : BaseEntity
    {
        [InputSelectExpression("DbContextTypes")]
        public string dbContextType { get => Get<string>(nameof(dbContextType)); set => Set(nameof(dbContextType), value); }


        [NotInput]
        public List<string> DbContextTypes { get; set; }
            = ServiceFactory.Get().GetTypesExtended<DbContextBase>().Where(t => !t.IsAbstract && t.GetGenericArguments().Count() == 0).Select(t => t.GetTypeName()).ToList();


        [OnChange(nameof(dbContextType))]
        public void DbContextTypeUpdated()
        {
            var apiType = typeof(SqlServerWebApiContext<>).MakeGenericType(dbContextType.ToType()).New<SqlServerMigBuilder>();

        }

    }


    [Label("Api DbContext источника SqlServer ")]
    public class SqlServerWebApiContext<TDbContextBase> : SqlServerMigBuilder
    {
        public SqlServerWebApiContext() : base("DESKTOP-IHJM9RD", "model")
        {
            using (var db = typeof(TDbContextBase).New<DbContext>())
            {
                Configure(db.Database.GetConnectionString());
            }
        }
    }


    [Label("Api источника SqlServer")]
    public class SqlServerWebApi : SqlServerMigBuilder, IWebApi, IdbModel, IApi
    {
        private readonly IAuthenticationApi authentication;


        [Label("Services")]
        public HashSet<IEntityFasade> Services { get; set; } = new();

        [Label("Procedures")]
        public HashSet<IProcedureExecuter> Procedures { get; set; } = new();

        public SqlServerWebApi() : base("DESKTOP-IHJM9RD", "model")
        {
            Services = new HashSet<IEntityFasade>();
        }

        public SqlServerWebApi(string server, string database) : base(server, database)
        {
            Services = new HashSet<IEntityFasade>();
        }

        public SqlServerWebApi(string server, string database, bool trustedConnection, string userId, string password) : base(server, database, trustedConnection, userId, password)
        {
            Services = new HashSet<IEntityFasade>();
            GetTablesMetadata();
        }

        [Label("Настройки")]
        public void Setup([InputPassword] string password)
        {

        }

        //public override void OnDatasourceChanged(PropertyChangedMessage message)
        //{
        //}

        //public override void OnDatasourceIdChanged(PropertyChangedMessage message)
        //{
        //}


        public void Init()
        {
            try
            {
                foreach (var cmd in DropAndCreate())
                {
                    try
                    {
                        PrepareQuery(cmd.Down);

                    }
                    catch (Exception ex)
                    {
                        this.Error($"Выполнение: {cmd.Down} привело к исключению: {ex.Message}\n{ex.StackTrace}");
                    }

                    try
                    {
                        PrepareQuery(cmd.Up);
                    }
                    catch (Exception ex)
                    {
                        this.Error($"Выполнение: {cmd.Up} привело к исключению: {ex.Message}\n{ex.StackTrace}");
                    }
                };

                UpdateTablesMetadata();
                InitServices();

                foreach (var pmd in GetProceduresMetadata())
                {
                    Procedures.Add(new ProcedureExecuter(this, pmd.Value));
                }
            }
            catch (Exception ex)
            {
                this.Error(ex);
            }

        }

        public void InitServices()
        {
            Services.Clear();
            TablesMetadata = InitTablesMetadata();
            foreach (Type entity in EntityTypes)
            {
                var rmd = GetTablesMetadata();

                
                if (rmd.ContainsKey(entity.GetTypeName()) == false)
                {

                    throw new Exception($"Не найдены метаданные сущности {entity.GetTypeName()} => {rmd.Keys.ToJsonOnScreen()}");
                }
                var tmd = rmd[entity.GetTypeName()];
                var tableName = tmd.TableName.ToSingleCount();
                this.Info(tableName);
                this.Info(EntityTypes.Select(entity => entity.GetTypeName().ToSingleCount()).ToJsonOnScreen());
                var et = EntityTypes.FirstOrDefault(entity => entity.GetTypeName().ToLower().Equals(tableName));
                Services.Add(new EntityFasadeBase(this, this, tmd, entity));
                Services.Select(s => s.GetEntityName()).ToJsonOnScreen().WriteToConsole();
            }
        }



        public ITableManager GetFasade(string schema, string table)
        {
            var driverManager = new OdbcDriverManager();
            this.Info(driverManager.GetOdbcDrivers().ToJsonOnScreen());

            var connectionString = $"Server={Server};Database={Database};DRIVER={{SQL Server}};";
            var dataSource = new OdbcDataSource(connectionString);
            if (!dataSource.canConnect())
                throw new Exception($"Не удалось установить подключение к источнику данных по ODBC, connectionString={connectionString}");
            var dataManager = new OdbcDatabaseManager(dataSource);
            var tableManager = dataManager.GetTableManager(table);
            if (tableManager.GetMetadata().schema.ToLower() != schema.ToLower())
                throw new Exception("Схемы данных отличаются");
            return tableManager;
        }


        public IEntityFasade<TConvertableType> GetFasade<TConvertableType>(Type ptype) where TConvertableType : BaseEntity
        {

            var fasade = Services.FirstOrDefault(f => f.GetEntityName().ToSingleCount() == ptype.GetTypeName());
            if (fasade is null)
            {
                if (!EntityTypes.Contains(ptype))
                {
                    EntityTypes.Add(ptype);
                    Init();
                }
                return Services.FirstOrDefault(s => s.GetEntityName() == ptype.GetTypeName()).ToFasade<TConvertableType>();
                //throw new Exception("Не удалось найти " + typeof(T).GetTypeName() + " В " + Services.Select(f => f.GetEntityName()).ToJsonOnScreen());
            }
            return fasade.ToFasade<TConvertableType>();
        }

        public IEntityFasade<T> GetFasade<T>() where T : BaseEntity
        {

            var fasade = Services.FirstOrDefault(f => f.GetEntityName().ToSingleCount() == typeof(T).GetTypeName());
            if (fasade is null)
            {
                if (!EntityTypes.Contains(typeof(T)))
                {
                    EntityTypes.Add(typeof(T));
                    Init();
                }
                return Services.FirstOrDefault(s => s.GetEntityName() == typeof(T).GetTypeName()).ToFasade<T>();
                //throw new Exception("Не удалось найти " + typeof(T).GetTypeName() + " В " + Services.Select(f => f.GetEntityName()).ToJsonOnScreen());
            }
            return fasade.ToFasade<T>();
        }


        public async Task<Tuple<int, object>> Request(string url, Dictionary<string, string> args = null, Dictionary<string, string> headers = null, string body = null)
        {
            this.Info(url);

            await Task.CompletedTask;

            var ids = url.Split('/').ToList();
            ids.RemoveAt(0);

            object p = new List<object>(Services);

            ((List<object>)p).AddRange(Procedures);
            var success = new List<string>();
            if (ids.Count() == 0)
            {
                return new Tuple<int, object>(404, new
                {
                    success,
                    args,
                    available = ((List<object>)p).Select(p => p.ToString())
                });
            }

            foreach (var id in ids)
            {

                bool completed = false;
                List<string> available = new();
                if (id.IsInt())
                {
                    success.Add(id);
                    p = await ((EntityFasadeBase)p).Find<Dictionary<string, object>>(id.ToInt());
                    completed = true;
                    break;
                }
                else
                if (Typing.IsCollectionType(p.GetType()))
                {
                    available = new();
                    foreach (var item in (IEnumerable)p)
                    {
                        available.Add(item.ToString().ToLower());
                        if (item.ToString().ToLower() == id.ToLower())
                        {
                            success.Add(id);
                            p = item;
                            completed = true;
                            break;
                        }
                    }
                }
                else
                {
                    available = new();
                    available.AddRange(p.GetType().GetProperties().Select(p => p.Name));
                    foreach (var prop in p.GetType().GetProperties())
                    {
                        if (prop.Name.ToString().ToLower() == id.ToLower())
                        {
                            success.Add(id);
                            p = prop.GetValue(p);
                            completed = true;
                            continue;
                        }
                    }
                    available.AddRange(p.GetType().GetMethods().Select(p => p.Name));
                    foreach (var prop in p.GetType().GetMethods())
                    {
                        if (prop.Name.ToString().ToLower() == id.ToLower())
                        {
                            success.Add(id);
                            Func<Dictionary<string, object>, object> ap = (parameters) =>
                            {
                                return prop.Invoke(p, parameters.Values.ToArray());
                            };
                            completed = true;
                            p = ap;
                            break;
                        }
                    }
                }
                if (completed)
                    continue;
                return new Tuple<int, object>(404, new
                {
                    success,
                    failed = id,
                    args,

                    available
                });
            }
            if (p is IProcedureExecuter)
            {
                p = ((IProcedureExecuter)p).GetExecFunc<Dictionary<string, object>>();
            }
            if (p is Func<Dictionary<string, object>, object>)
            {
                var argsv = new Dictionary<string, object>(args.Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value)));
                try
                {
                    var exec = (Func<Dictionary<string, object>, object>)p;
                    var result = exec(argsv);
                    return new Tuple<int, object>(200, new
                    {
                        success,
                        input = argsv,
                        data = result
                    });
                }
                catch (ArgumentException ex)
                {
                    return new Tuple<int, object>(422, new
                    {
                        success,
                        param = ex.ParamName,
                        args,
                        message = ex.Message
                    });
                }
                catch (Exception ex)
                {
                    return new Tuple<int, object>(500, new
                    {
                        args,
                        success,
                        message = ex.Message
                    });
                }
            }
            else if (p is EntityFasadeBase)
            {
                var result = ((EntityFasadeBase)p).GetAll().Result;
                return new Tuple<int, object>(200, new
                {
                    args,
                    success,
                    data = result
                });
            }
            else
            {
                return new Tuple<int, object>(404, new
                {
                    args,
                    success,
                    data = p
                });
            }
            return new Tuple<int, object>(200, p);
        }

        public bool can(string url, Dictionary<string, string> queryParams, Dictionary<string, string> headers)
            => authentication is null ? true : authentication.can(url, queryParams, headers);



        public bool match(string url, Dictionary<string, string> queryParams)
        {
            object p = this;
            var tables = GetTableNames();
            List<object> entities = new();
            if (url.StartsWith("/"))
            {
                var pars = new Dictionary<string, string>();
                string key = null;
                string value = null;
                bool state = false;
                foreach (var code in url.Split("/"))
                {
                    if (!state)
                    {
                        key = code;
                    }
                    else
                    {
                        pars[key] = value = code;
                    }
                    state = state ? false : true;
                }
                foreach (var kv in pars)
                {
                    var types = entities.Count == 0 ? GetEntityClasses().ToList() :
                        entities.Last().GetType().New<BaseEntity>().GetEntityRelations().Select(r => r.RelativeEntityType.ToType()).ToList();
                    var entityType = types.FirstOrDefault(t => t.GetTypeName().ToLower() == kv.Key.ToLower());
                    entities.Add(GetFasade<BaseEntity>(entityType).Get(kv.Value.ToInt()));
                }
            }
            return true;
        }

        public Dictionary<string, string> GetNavigation(string url)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, IFilter> GetQueryParams<TEntity>(string url)
        {
            var filters = new Dictionary<string, IFilter>();

            if (string.IsNullOrWhiteSpace(url))
                return filters;

            var queryIndex = url.IndexOf('?');
            if (queryIndex < 0)
                return filters;

            var queryString = url.Substring(queryIndex + 1);
            var pairs = queryString.Split('&');

            foreach (var pair in pairs)
            {
                var parts = pair.Split('=');
                if (parts.Length == 2)
                {
                    var key = Uri.UnescapeDataString(parts[0]);
                    var value = Uri.UnescapeDataString(parts[1]);
                    var names = typeof(TEntity).GetPropertyNames().Select(p => p.ToLower());
                    if (names.Contains(key.ToLower()))
                    {

                        if (value.Trim().StartsWith("[") && value.Trim().EndsWith("]"))
                        {
                            filters[key] = new Filter()
                            {
                                name = key,
                                action = "in",
                                param = value
                            };
                        }
                        else
                        {
                            filters[key] = new Filter()
                            {
                                name = key,
                                //action = "equals",
                                action = "contains",
                                param = value
                            };
                        }
                    }
                    else if (names.Contains(key.ToLower() + "!"))
                    {
                        filters[key] = new Filter()
                        {
                            name = key,
                            action = "not",
                            param = value
                        };
                    }


                }
            }

            return filters;
        }

        public IDbMetadata GetModel()
        {
            throw new NotImplementedException();
        }

        public IEntityFasade CreateFasade<T>(Type entity)
        {
            return new EntityFasadeBase(this, this, GetTablesMetadata()[entity.GetTypeName()], entity);
        }
    }
}
