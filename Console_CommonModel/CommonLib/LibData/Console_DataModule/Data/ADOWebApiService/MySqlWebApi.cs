using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.ProductsFilters;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService
{

    [Label("Сервис WebApi для MySQL")]
    public class MySqlWebApi : MySqlMigBuilder, IWebApi
    {
        public HashSet<IEntityFasade> Services { get; set; } = new();
        public HashSet<IProcedureExecuter> Procedures { get; set; } = new();

        public MySqlWebApi() : base()
        {
            Services = new HashSet<IEntityFasade>();
        }

        public MySqlWebApi(string server, string database) : base(server, database)
        {
            Services = new HashSet<IEntityFasade>();
        }



        public bool can(string url, Dictionary<string, string> queryParams, Dictionary<string, string> headers)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            var ids = url.Split('/').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            if (ids.Count == 0)
                return true; // Root path is always accessible

            object p = new List<object>(Services);
            ((List<object>)p).AddRange(Procedures);

            foreach (var id in ids)
            {
                bool found = false;
                if (Typing.IsCollectionType(p.GetType()))
                {
                    foreach (var item in (IEnumerable)p)
                    {
                        if (item.ToString().ToLower() == id.ToLower())
                        {
                            p = item;
                            found = true;
                            break;
                        }
                    }
                }
                else
                {
                    var prop = p.GetType().GetProperties().FirstOrDefault(pr => pr.Name.ToLower() == id.ToLower());
                    if (prop != null)
                    {
                        p = prop.GetValue(p);
                        found = true;
                        continue;
                    }
                }

                if (!found)
                    return false;
            }

            return true;
        }

        public bool match(string url, Dictionary<string, string> queryParams)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            var ids = url.Split('/').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            if (ids.Count == 0)
                return true;

            var firstSegment = ids[0].ToLower();
            var allItems = new List<object>(Services);
            allItems.AddRange(Procedures);

            return allItems.Any(item => item.ToString().ToLower() == firstSegment);
        }

        public Dictionary<string, string> GetNavigation(string url)
        {
            var navigation = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(url))
            {
                var allItems = new List<object>(Services);
                allItems.AddRange(Procedures);
                foreach (var item in allItems)
                {
                    navigation[item.ToString()] = $"/{item.ToString().ToLower()}";
                }
                return navigation;
            }

            var ids = url.Split('/').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            object p = new List<object>(Services);
            ((List<object>)p).AddRange(Procedures);

            string currentPath = "";

            foreach (var id in ids)
            {
                currentPath += $"/{id}";
                bool found = false;

                if (Typing.IsCollectionType(p.GetType()))
                {
                    foreach (var item in (IEnumerable)p)
                    {
                        if (item.ToString().ToLower() == id.ToLower())
                        {
                            p = item;
                            found = true;
                            break;
                        }
                    }
                }
                else
                {
                    var prop = p.GetType().GetProperties().FirstOrDefault(pr => pr.Name.ToLower() == id.ToLower());
                    if (prop != null)
                    {
                        p = prop.GetValue(p);
                        found = true;
                        continue;
                    }
                }

                if (!found)
                    break;
            }

            if (Typing.IsCollectionType(p.GetType()))
            {
                foreach (var item in (IEnumerable)p)
                {
                    navigation[item.ToString()] = $"{currentPath}/{item.ToString().ToLower()}";
                }
            }
            else if (p != null)
            {
                foreach (var prop in p.GetType().GetProperties())
                {
                    navigation[prop.Name] = $"{currentPath}/{prop.Name.ToLower()}";
                }
            }

            return navigation;
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
                }

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
            }
            return fasade.ToFasade<T>();
        }

        public ITableManager GetFasade(string schema, string table)
        {
            // Для MySQL можно использовать прямое подключение через MySql.Data
            // В отличие от SQL Server, здесь не используется ODBC
            throw new NotImplementedException("GetFasade(string schema, string table) для MySQL требует дополнительной реализации через MySql.Data");
        }

        public IDbMetadata GetModel()
        {
            // Возвращаем текущий экземпляр как IDbMetadata, так как MySqlMigBuilder наследуется от MySqlDbMetadata
            return this;
        }

        IDbMetadata IWebApi.GetModel()
        {
            // Возвращаем текущий экземпляр, так как MySqlWebApi наследуется от MySqlMigBuilder,
            // который наследуется от MySqlDbModel -> MySqlDbMetadata -> IDbMetadata
            return this;
        }

        internal void SetConnectionString(string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}
