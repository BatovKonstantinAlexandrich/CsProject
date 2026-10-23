using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.TreeViewServices
{
    /// <summary>
    /// Построитель иерархического представления сущностей для TreeView
    /// </summary>
    /// <typeparam name="TDbContext">Тип DbContext</typeparam>
    [Label("Построитель дерева")]
    [Description("Fluent API построитель для создания иерархической структуры TreeView из сущностей Entity Framework Core через навигационные свойства и связи между таблицами")]
    public class TreeViewBuilder<TDbContext> where TDbContext : DbContext
    {
        private TDbContext _dbContext;

        public List<string> _joinedEntityTypes { get; set; } = new List<string>();

        /// <summary>
        /// Список типов сущностей, которые были добавлены через Join
        /// </summary>
        public IReadOnlyList<Type> JoinedEntityTypes => _joinedEntityTypes.Select(name => name.ToType()).ToList().AsReadOnly();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dbContext">Экземпляр DbContext</param>
        public TreeViewBuilder() : this(typeof(TDbContext).New<TDbContext>())
        {

        }

        public List<List<string>> GetAvailableTreeView()
        {
            List<List<string>> options = new();
            using (var db = typeof(TDbContext).New<DbContextBase>())
            {

                foreach (var entityType in db.GetEntitiesTypes())
                {
                    var exp = new List<string>() { entityType.GetTypeName() };
                    options.Add(exp);

                    var relations = db.GetEntityRelationsForTypeWithExpected(entityType, exp);
                    var relativeEntities = relations.Select(r => r.RelativeEntityType).ToList();
                    foreach (var rel in relations.ToList())
                    {
                        rel.Trace();
                        List<string> relExp = new(exp);
                        options.Add(relExp);
                        foreach (var p in rel.GetRelationsList(relExp))
                        {
                            options.Add(p);
                        }
                    }
                }
            }


            return options.ToHashSet().ToList();
        }


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dbContext">Экземпляр DbContext</param>
        public TreeViewBuilder(TDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Добавляет сущность в иерархию через Join
        /// </summary>
        /// <param name="manyEntityType">Тип сущности для добавления</param>
        /// <returns>Текущий экземпляр для цепочки вызовов</returns>
        public TreeViewBuilder<TDbContext> Join(Type manyEntityType)
        {
            if (manyEntityType == null)
                throw new ArgumentNullException(nameof(manyEntityType));

            // Проверяем, что тип является сущностью в DbContext
            if (!IsEntityType(manyEntityType))
            {
                throw new ArgumentException($"Тип {manyEntityType.Name} не является сущностью в {typeof(TDbContext).Name}", nameof(manyEntityType));
            }

            // Проверяем, что тип еще не был добавлен
            if (_joinedEntityTypes.Contains(manyEntityType.GetTypeName()))
            {
                throw new InvalidOperationException($"Сущность {manyEntityType.Name} уже была добавлена в иерархию");
            }

            // Если это первая сущность, просто добавляем
            if (_joinedEntityTypes.Count == 0)
            {
                _joinedEntityTypes.Add(manyEntityType.GetTypeName());
                return this;
            }

            // Проверяем, что есть связь между последней добавленной сущностью и новой
            Type lastEntityType = _joinedEntityTypes.Last().ToType();
            if (!AreEntitiesRelated(lastEntityType, manyEntityType))
            {
                throw new InvalidOperationException($"Сущность {manyEntityType.Name} не связана с последней добавленной сущностью {lastEntityType.Name}");
            }

            _joinedEntityTypes.Add(manyEntityType.GetTypeName());
            return this;
        }

        /// <summary>
        /// Проверяет, является ли тип сущностью в DbContext
        /// </summary>
        private bool IsEntityType(Type entityType)
        {
            var model = _dbContext.Model;
            return model.FindEntityType(entityType) != null;
        }

        /// <summary>
        /// Проверяет, связаны ли две сущности через навигационные свойства
        /// </summary>
        private bool AreEntitiesRelated(Type entityType1, Type entityType2)
        {
            var model = _dbContext.Model;
            var entityType1Metadata = model.FindEntityType(entityType1);
            var entityType2Metadata = model.FindEntityType(entityType2);

            if (entityType1Metadata == null || entityType2Metadata == null)
                return false;

            // Проверяем навигационные свойства первой сущности
            var navigations1 = entityType1Metadata.GetNavigations();
            foreach (var nav in navigations1)
            {
                if (nav.TargetEntityType.ClrType == entityType2)
                    return true;
            }

            // Проверяем навигационные свойства второй сущности
            var navigations2 = entityType2Metadata.GetNavigations();
            foreach (var nav in navigations2)
            {
                if (nav.TargetEntityType.ClrType == entityType1)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Сбрасывает все добавленные сущности
        /// </summary>S
        public void Reset()
        {
            _joinedEntityTypes.Clear();
        }

        /// <summary>
        /// Получает строковое представление иерархии
        /// </summary>
        public override string ToString()
        {
            return string.Join(" -> ", _joinedEntityTypes.Select(t => t));
        }
    }
}

