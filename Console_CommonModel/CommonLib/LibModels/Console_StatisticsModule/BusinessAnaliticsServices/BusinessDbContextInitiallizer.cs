using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;

using Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule.BusinessAnaliticsServices;

/// <summary>
/// Выполняет инициализация данных приложения, 
/// проверяет наличия данных в каждой таблице.
/// </summary>
[Label("Инициализация данных бизнес-модели")]
public class BusinessDbContextInitiallizer : DbContextInitiliazer<BusinessDbContext>, IBusinessDbContextInitiallizer<BusinessDbContext>
{




    /// <summary>
    /// Конструктор получает контекст данных
    /// </summary>
    /// <param name="context"></param>
    public BusinessDbContextInitiallizer()
    {

        //Info(context.GetEntityTypeNames());
    }



    public override Dictionary<string, int> Init(BusinessDbContext context)
    {
        try
        {
            Dictionary<string, int> result = new();
            result[nameof(BusinessResource)] = InitBusinessResource(context);
            result[nameof(BusinessFunction)] = InitBusinessFunctions(context);
            result[nameof(BusinessDatasource)] = InitBusinessDatasources(context);
            result[nameof(BusinessDataset)] = InitBusinessDatasets(context);
            result[nameof(BusinessDataset)] = InitGranularities(context);
            result[nameof(BusinessData)] = InitBusinessData(context);
            return result;
        }
        catch (Exception ex)
        {
            this.Error($"Исключение при инициализации данных BusinessDbContextInitiallizer => Init => {ex}");
            throw;
        }
    }

    [Label("Регистрация источников данных")]
    public int InitBusinessDatasources(BusinessDbContext context)
    {
        if (typeof(BusinessDatasource).GetEntityFasade<BusinessDatasource>().Count() == 0)
        {
            var datasources = ServiceFactory.Get().GetTypesExtended<DbContextBase>().Select(t => new BusinessDatasource()
            {
                Name = t.GetTypeName(),
                Description = t.GetDescription(),
                Label = t.GetLabel(),
                Icon = t.GetIcon(),
                ConnectionString = $@"Data Source=DESKTOP-IHJM9RD;" +
                    $"Initial Catalog={t.Name};" +
                    "Integrated Security=True;" +
                    "Connect Timeout=30;" +
                    "Encrypt=False;" +
                    "" +
                    "Application Intent=ReadWrite;" +
                    "Multi Subnet Failover=False"
            });

            this.Info(datasources.ToJsonOnScreen());
            foreach (var datasource in datasources)
            {
                this.Info(datasource.ToJsonOnScreen());
                context.SaveChanges();
                context.BusinessDatasources.Add(datasource);
                context.SaveChanges();
            }

            return datasources.Count();


        }
        return 0;
    }


    [Label("Регистрация бизнес-русурсов")]
    public int InitBusinessResource(BusinessDbContext context)
    {
        if (context.BusinessResources.Count() == 0)
        {
            context.Add(new BusinessResource() { Label = "Пользователи", Name = "Users", Code = "Users" });
            context.Add(new BusinessResource() { Label = "Службы", Name = "Services", Code = "Services" });

            return context.SaveChanges();
        }
        return 0;
    }

    public int InitBusinessDatasets(BusinessDbContext context)
    {
        foreach(var dataset in context.BusinessDatasets.ToList())
        {
            context.Remove(dataset);
        }
        context.SaveChanges();
        int counter = 0;
        foreach( var ds in context.BusinessDatasources.ToList())
        {
            var service = new BusinessDatasourceService();
            service.Setup(ds);
            List<BusinessDataset> datasets = new();
            foreach (var kv in service.GetTablesMetadata())
            {
                datasets.Add(new BusinessDataset()
                {
                    Name = kv.Value.Fullname,
                    Label = kv.Value.TableLabel != "Надпись" ? kv.Value.TableLabel : kv.Value.TableName,
                    Description = service.CreateSelectStatement(kv.Value.TableSchema, kv.Value.TableName),
                    Expression = service.CreateSelectStatement(kv.Value.TableSchema, kv.Value.TableName),
                    DatasourceID = ds.Id
                });
            }
            foreach (var pr in service.GetProceduresMetadata().Values)
            {
                datasets.Add(new BusinessDataset()
                {
                    Name = pr.ProcedureName,
                    Label = pr.ProcedureLabel != "Надпись" ? pr.ProcedureLabel : pr.ProcedureName,
                    Description = pr.FullName,
                    Expression = $"EXEC {pr.FullName}",
                    DatasourceID = ds.Id
                });                
            }
            context.AddRange(datasets);
            context.SaveChanges();

                           
        }
        /*var activeObjects = ServiceFactory.Get().GetTypesExtended<ActiveObject>();
        foreach (var pactiveObject in activeObjects)
        {
            var actionType = typeof(Action<>).MakeGenericType(pactiveObject);
            var actionTypes = ServiceFactory.Get().GetTypesExtended(actionType);
            foreach (var pactionType in actionTypes)
            {
                try
                {
                    var dataset = new BusinessDataset()
                    {
                        Name = $"{pactiveObject.GetLabel()}/{pactionType.GetLabel()}",
                        Description = "Кол-во",
                        Expression = "select * from dataset"
                    };
                    dataset.Create();
                    counter++;
                }
                catch (Exception ex)
                {

                }
            }
        }*/
        return counter;
         
    }
    /// <summary>
    /// Конструктор получает контекст данных
    /// </summary>
    /// <param name="context"></param>
    public BusinessDbContextInitiallizer(BusinessDbContext ctx)
    {

        //Info(context.GetEntityTypeNames());
    }

    /// <summary>
    /// Права для ИТ-отдела
    /// </summary>
    /// <returns></returns>
    public IEnumerable<BusinessResource> GetItDepartmentRoles()
    {
        return new BusinessResource[] {
            new BusinessResource(){
                Name="DBA",
                Description=""
            },
            new BusinessResource(){
                Name="DBA", Description="" },
            new BusinessResource(){ Name="Dev",     Description="" },
            new BusinessResource(){ Name="SA",      Description="" },
            new BusinessResource(){ Name="Si",      Description="" },
            new BusinessResource(){ Name="User",    Description="" },
            new BusinessResource(){ Name="Boss",    Description="" },
        };
    }

    public void InitMessageAttributes(BusinessDbContext db)
    {
        IEnumerable<Type> attributes = GetBaseInputAttributes();
        if (db.MessageAttributes.Count() != attributes.Count())
        {
            foreach (var attribute in attributes)
            {
                foreach (var m in db.MessageAttributes.ToList())
                {
                    db.MessageAttributes.Remove(m);
                }
                db.MessageAttributes.Add(ConvertToMessageAttribute(attribute));
            }
        }
        db.SaveChanges();
    }

    public static IEnumerable<Type> GetBaseInputAttributes()
    {
        return ServiceFactory.Get().GetTypesExtended<BaseInputAttribute>();
    }
    class InitiallizaerUtils
    {
        internal static string LabelFor(Type attribute)
        {
            return attribute.GetType().GetLabel();
        }

        internal static string DescriptionFor(Type attribute)
        {
            return attribute.GetType().GetDescription();
        }

        internal static string IconFor(Type attribute)
        {
            return attribute.GetType().GetIcon();
        }
    }
    public static MessageAttribute ConvertToMessageAttribute(Type attribute)
    {
        string atrName = (attribute.GetType().Name.StartsWith("Input") ? attribute.GetType().Name.Substring("Input".Length) : attribute.GetType().Name).Replace("Attribute", "");
        //var icons = new MaterialIconsService();

        BaseInputAttribute attr = (BaseInputAttribute)attribute.GetConstructors().First().Invoke(new object[attribute.GetConstructors().First().GetParameters().Count()]);
        //ReflectionService.CreateWithDefaultConstructor<BaseInputAttribute>(attribute);
        var res = new MessageAttribute()
        {
            Code = attribute.GetTypeName().ToUpper().ToCapitalStyle(),
            //SQLType = attr.GetSqlServerDataType(),
            //CSharpType = attr.GetCSTypeName(),
            SqlServerDataType = attr.GetSqlServerDataType(),
            //MySQLDataType = attr.GetMySQLDataType(),
            PostgreDataType = attr.GetPostgreDataType(),
            OracleDataType = attr.GetOracleDataType(),
            Description = InitiallizaerUtils.LabelFor(attribute) + ":\n" + InitiallizaerUtils.DescriptionFor(attribute),
            Icon = InitiallizaerUtils.IconFor(attribute),
            Name = InitiallizaerUtils.LabelFor(attribute),
            InputType = (attribute.Name.StartsWith("Input") ? attribute.Name.Substring("Input".Length) : attribute.Name).Replace("Attribute", "")
        };
        if (res.Name == null)
        {
            int x = 0;
            string label = InitiallizaerUtils.LabelFor(attribute);
        }
        return res;
    }


    public int InitBusinessResources(BusinessDbContext db)
    {
        if (db.BusinessResources.Count() == 0)
        {
            var user = new BusinessResource()
            {
                Name = "Пользователь",
                Description = "Пользователи имеют возможность воспроизводить свои функциональные обязанности согласно информационной модели.",
                Code = "User"
            };
            var dba = new BusinessResource()
            {
                Name = "Database Administrator (DBA)",
                Description = "Разрабатывает базы данных, процедуры и функции... ",
                Code = "DBA",
                Parent = user
            };
            var analitic = new BusinessResource()
            {
                Name = "System Analist(SA)",
                Description = "Аналитик создает отчёты",
                Code = "SA",
                Parent = user
            };
            var integrator = new BusinessResource()
            {
                Name = "System Integrator(SI)",
                Description = "Аналитик создает отчёты",
                Code = "SI",
                Parent = user
            };


            var dev = new BusinessResource()
            {
                Name = "Разработчик",
                Description = "Разработчик исследует бизнес процессы предприятия (IDEF0), устанавливает связи между функциональными единицами и информационными ресурсами (IDEFO, IDEF3 и DFD). " +
                                "Проектирует инфо-логическую и дата-логическую связанность функций информационной системы с внутренними и внешними бизнес процессами предприятия. ",
                Code = "Dev",
                Parent = user
            };
            var boss = new BusinessResource()
            {
                Name = "Директор",
                Description = "",
                Code = "Boss",
                Parent = user
            };

            db.Add(user);
            db.Add(dba);
            db.Add(integrator);
            db.Add(analitic);
            db.Add(boss);

            db.Add(dev);
            db.SaveChanges();
            dba.Parent = user;
            analitic.Parent = user;
            dev.Parent = user;
            db.SaveChanges();










            //регистрация пользователя для этапа разработки и тестирования
            /*var devRole = db.BusinessResources.Where(r => r.Code == "Dev").SingleOrDefault();
            if (db.Users.Count() == 0)
            {
                UserPerson person = PersonNamesProvider.GetRandomPerson();
                var registration = new RegistrationService(db, new AuthorizationOptions(), new EmailService(new Authorization.Services.Email.EmailOptions()));
                registration.Signup(new UserAccount("eckumoc@gmail.com", "sgdf1423"), person, devRole);
            }*/
        }
        return 1;

    }

    public Dictionary<string, int> InitData(BusinessDbContext db)
    {
        //db.Database.EnsureCreated();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        Dictionary<string, int> res = new();
        res["Granularities"] = InitGranularities(db);
        res["BusinessIndicators"] = InitBusinessIndicators(db);
        res["BusinessFunction"] = InitBusinessFunctions(db);
        res["BusinessDatasources"] = InitBusinessDatasources(db);
        res["BusinessReports"] = InitBusinessReports(db);
        res["BusinessResources"] = InitBusinessResources(db);
        res["BusinessOLAP"] = InitBusinessOLAP(db);
        res["BusinessDatasets"] = InitBusinessDatasets(db);
        res["RatingBusinessDatasets"] = InitRatingBusinessDatasets(db);
        res["BusinessData"] = InitBusinessData(db);
        return res;
    }

    public int InitGranularities(BusinessDbContext db)
    {
        if (db.Granularities.Count() != 0)
            return 0;
        foreach (var code in "day,week,quarter,month,year".Split(","))
        {
            db.Granularities.Add(new()
            {
                Code = code,
                Name =
                    code == "day" ? "ежедневный" :
                    code == "week" ? "недельный" :
                    code == "month" ? "месячный" :
                    code == "quarter" ? "квартальный" :
                    code == "year" ? "годовой" :
                    "",
                Description = "Периодичность отчёта"
            });
        }

        return db.SaveChanges();
    }

    public int InitRatingBusinessDatasets(BusinessDbContext db)
    {

        string con = null;

        BusinessDatasource p = null;
        db.BusinessDatasources.Add(p = new()
        {
            Name = db.GetType().GetTypeName(),
            Description = db.GetType().GetDescription(),
            ConnectionString = db.Database.GetConnectionString(),
            Label = "Бизнес модель"
        });
        db.SaveChanges();
        db.BusinessDatasets.Add(new()
        {
            Name = "Rating",
            Label = "Рейтинг подключения",

            Description = "Оценка эффективности информационных ресурсов",
            Expression = "exec add_resource_statistics",
            BusinessDatasourceId = p.Id

        });
        return db.SaveChanges();
    }

    public int InitBusinessIndicators(BusinessDbContext db)
    {
        if (db.BusinessIndicators.Count() > 0)
        {
            return 0;
        }
        db.BusinessIndicators.Add(new BusinessIndicator()
        {
            Name = "Производительность",
            Description = "Производительность",
            IsNegative = false
        });
        db.BusinessIndicators.Add(new BusinessIndicator()
        {
            Name = "Работоспособность",
            Description = "Работоспособность",
            IsNegative = false
        });
        db.BusinessIndicators.Add(new BusinessIndicator()
        {
            Name = "Своевременность",
            Description = "Своевременность",
            IsNegative = false
        });
        return db.SaveChanges();
    }

    public int InitBusinessFunctions(BusinessDbContext db)
    {
        if (db.BusinessFunctions.Count() > 0) return 0;
        foreach (var role in db.BusinessResources.ToList())
        {
            switch (role.Code)
            {
                case "Customer":
                    {
                        db.BusinessFunctions.Add(new()
                        {
                            Name = "Покупка запчастей",
                            Description = "Покупка на сайте, покупка в магазинах.",
                            BusinessResourceId = role.Id
                        });
                        db.BusinessFunctions.Add(new()
                        {
                            Name = "Запис на ремонт",
                            Description = "Запись по телефону, запись через бота.",
                            BusinessResourceId = role.Id
                        });
                        db.SaveChanges();
                        break;
                    }
            }
        }
        return db.SaveChanges();
    }

    public int InitOdbcBusinessDatasources(BusinessDbContext db)
    {
        var manager = new OdbcDriverManager();
        this.Info(manager.GetOdbcDatasources().ToJsonOnScreen());
        foreach (var datasource in manager.GetOdbcDatasources())
        {
            db.BusinessDatasources.Add(new()
            {
                Name = datasource.Name,
                Description = manager.GetOdbcDatasources().ToJsonOnScreen(),
                ConnectionString = $"driver={datasource.DriverName};dsn={datasource.Name};uid=;pwd=;"
            });
        }
        return db.SaveChanges();
    }

    public int InitBusinessReports(BusinessDbContext db)
    {
        /*db.BusinessReports.Add(new()
        {
            
        });*/
        return 0;
    }

    public int InitBusinessOLAP(BusinessDbContext db)
    {
        return 0;
    }

    public int InitBusinessData(BusinessDbContext db)
    {
        var service = new BusinessAnaliticsService(db);
        service.Info(service.GetIndicators().ToJsonOnScreen());
        service.AddIndicator("Эффективность", "Performance", "Тест");
        service.Info(service.GetIndicators().ToJsonOnScreen());
        InitRatingBusinessDatasets(db);
        var dataset = service.GetDatasetByName("Рейтинг подключения");
        if (dataset is null)
        {
            BusinessDatasource p = null;
            db.BusinessDatasources.Add(p = new()
            {
                Name = db.GetType().GetTypeName(),
                Description = db.GetType().GetDescription(),
                ConnectionString = db.Database.GetConnectionString(),
                Label = "Бизнес модель"
            });
            db.SaveChanges();
            db.BusinessDatasets.Add(dataset = new()
            {
                Name = "Rating",
                Label = "Рейтинг подключения",

                Description = "Оценка эффективности информационных ресурсов",
                Expression = "exec add_resource_statistics",
                BusinessDatasourceId = p.Id

            });
            db.SaveChanges();
        }
        var datasetId = dataset.Id;

        foreach (var resource in service.GetResources())
        {
            foreach (var indicator in service.GetIndicators())
            {
                var indicatorId = indicator.Id;
                var resourceId = resource.Id;

                service.AddData(resourceId, indicatorId, datasetId, "2024-01-01", 1, 4f);
            }

            this.Info(service.GetDatasetValues(resource.Id, datasetId, "2024-01-01", 1).ToJsonOnScreen());
        }
        return db.SaveChanges();
    }
}