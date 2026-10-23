using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Namotion.Reflection;


namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeServices
{
    /// <summary>
    /// Формирует иерархические представления на основе структуры данных 
    /// из разных сущностей.    
    /// </summary>
    public class TreeViewEntityBuilder<TDbContext>
        where TDbContext : DbContextBase
    {
        public TreeControlView view = new();
        public List<(Type, Func<object, TreeViewNodeModel>)> types = new();


        public List<List<string>> GetAvailableTreeView()
        {
            List<List<string>> options = new();
            using (var db = typeof(TDbContext).New<DbContextBase>())
            {

                foreach (var entityType in db.GetEntitiesTypes())
                {
                    List<string> ext = new() { entityType.GetTypeName() };
                    options.Add(new() { entityType.GetTypeName() });
                    var relations = db.GetEntityRelationsForTypeWithExpected(entityType, ext);
                    foreach (var rel in relations.ToList())
                    {
                        options.AddRange(rel.GetRelationsList(new List<string>() { entityType.GetTypeName() }));
                    }
                }
            }
            return options;
        }

        public TreeControlView Build()
        {
            var res = view;
            view.root.Children.Clear();
            int level = 1;
            Type prev = null;
            List<(object, TreeViewNodeModel)> cur = new() { (null, view.root) };
            foreach (var type in types)
            {
                if (level == 1)
                {
                    List<object> dataset = GetAllDataset(type.Item1);
                    var next = dataset.Select(record => ((object)record, type.Item2(record))).ToList();
                    view.root.Children.AddRange(next.Select(p => p.Item2));
                    cur = next;
                }
                else
                {
                    List<(object, TreeViewNodeModel)> cur2 = new();
                    foreach (var p in cur)
                    {
                        var id = p.Item1.GetValue("Id").ToString().ToInt();
                        var dataset = GetObjectDataset(prev, type.Item1, id);
                        List<(object, TreeViewNodeModel)> nodes = dataset.Select(p => (p, type.Item2(p))).ToList();
                        p.Item2.Children.AddRange(nodes.Select(p => p.Item2));
                        cur2.AddRange(nodes);
                    }
                    cur = cur2;
                }
                prev = type.Item1;
                level++;
            }
            types.Clear();
            view = new();
            return res;
        }




        public List<object> GetAllDataset(Type type)
        {
            using (var ctx = typeof(TDbContext).New<DbContextBase>())
            {
                var items = (dynamic)ctx.GetType().GetMethods().Where(m => m.Name == nameof(DbContextBase.Set) && m.GetGenericArguments().Length == 1).FirstOrDefault().MakeGenericMethod(type).Invoke(ctx, new object[0]);
                List<object> objects = new();
                foreach (var p in items)
                {
                    objects.Add(p);
                }
                return objects;
            }
        }


        public List<object> GetObjectDataset(Type TOne, Type TMany, int id)
        {
            List<object> objects = new();
            GetType().GetMethods().Select(m => m.Name).ToList().ToJsonOnScreen().WriteToConsole();
            var m = GetType().GetMethods().FirstOrDefault(m => m.Name == "GetDataset");
            m = m.MakeGenericMethod(new Type[] { TOne, TMany });
            var results = (dynamic)m.Invoke(this, new object[] { id });
            foreach (var p in results)
            {
                objects.Add(p);
            }
            return objects;
        }


        public List<TMany> GetDataset<TOne, TMany>(int id) where TMany : BaseEntity
        {
            var ptmt = typeof(TMany);
            var props = ptmt.GetProperties();
            props.Select(p => p.Name).ToList().ToJsonOnScreen().WriteToConsole();
            var propName = props.FirstOrDefault(p => p.PropertyType == typeof(TOne)).Name + "Id";
            using (var ctx = typeof(TDbContext).New<DbContextBase>())
            {
                var ef = typeof(TDbContext).GetEntityFasade<TMany>();
                var tableName = ef.GetTableName();
                var results = ctx.ExecuteQuery<TMany>
                    ($"SELECT * FROM {tableName} WHERE {propName}={id}");
                return results.ToList();

            }
        }

        public void Join(Type manyEntityType)
        {
            if (types.Count() > 0)
            {
                var lastType = types.Last().Item1;
                var hasManyProperty = lastType.GetProperties().Any(p => p.PropertyType.IsCollectionType() && p.PropertyType.GetGenericArguments()[0] == manyEntityType);
                if (!hasManyProperty)
                    throw new Exception($"Тип {lastType} не объявляет свойства типа коллекции {manyEntityType.GetTypeName()}");
            }

            Func<object, TreeViewNodeModel> fx = pp =>
            {
                var p = new TreeViewNodeModel()
                {
                    Title = ((NamedObject)pp).Name
                };
                return p;
            };
            types.Add((manyEntityType, fx));
        }

        public void Join<ManyEntityType>(Func<ManyEntityType, string> getName) where ManyEntityType : BaseEntity
        {
            if (types.Count() > 0)
            {
                var lastType = types.Last().Item1;
                var hasManyProperty = lastType.GetProperties().Any(p => p.PropertyType.IsCollectionType() && p.PropertyType.GetGenericArguments()[0] == typeof(ManyEntityType));
                if (!hasManyProperty)
                    throw new Exception($"Тип {lastType} не объявляет свойства типа коллекции {typeof(ManyEntityType).GetTypeName()}");
            }

            Func<object, TreeViewNodeModel> fx = pp =>
            {
                var name = getName((ManyEntityType)pp);
                var p = new TreeViewNodeModel()
                {
                    Title = name
                };
                return p;
            };
            types.Add((typeof(ManyEntityType), fx));
        }
        public void Join<ManyEntityType>() where ManyEntityType : NamedObject
        {
            if (types.Count() > 0)
            {
                var lastType = types.Last().Item1;
                var hasManyProperty = lastType.GetProperties().Any(p => p.PropertyType.IsCollectionType() && p.PropertyType.GetGenericArguments()[0] == typeof(ManyEntityType));
                if (!hasManyProperty)
                    throw new Exception($"Тип {lastType} не объявляет свойства типа коллекции {typeof(ManyEntityType).GetTypeName()}");
            }

            Func<object, TreeViewNodeModel> fx = pp =>
            {
                try
                {
                    var p = new TreeViewNodeModel()
                    {
                        Title = pp.ToString()
                    };
                    return p;
                }
                catch (Exception ex)
                {
                    throw new Exception("Не удалось создать узел для иерархического представления");
                }
            };
            types.Add((typeof(ManyEntityType), fx));
        }


        public void Join<ManyEntityType>(Func<ManyEntityType, TreeViewNodeModel> convert) where ManyEntityType : class
        {
            if (types.Count() > 0)
            {
                var lastType = types.Last().Item1;
                var hasManyProperty = lastType.GetProperties().Any(p => p.PropertyType.IsCollectionType() && p.PropertyType.GetGenericArguments()[0] == typeof(ManyEntityType));
                if (!hasManyProperty)
                    throw new Exception($"Тип {lastType} не объявляет свойства типа коллекции {typeof(ManyEntityType).GetTypeName()}");
            }
            types.Add((typeof(ManyEntityType), p => convert((ManyEntityType)p)));
        }
    }
}
