using Console_CommonModel.CommonLib.LibData.AbstractionModel;

namespace Console_CommonModel.CommonLib.LibData
{

    /// <summary>
    /// Методы расширения класса сущности
    /// </summary>    
    public class BaseEnityService(BaseEntity entity)
    {
        public List<EntityRelations> GetEntityRelations()
            => entity.GetEntityRelations();

        public List<EntityRelations> GetHierTables()
            => GetEntityRelations().Where(r => r.RelationshipType.ToType().IsExtends(typeof(HierTable))).ToList();

        public List<EntityRelations> GetActiveObjects()
            => GetEntityRelations().Where(r => r.RelationshipType.ToType().IsExtends(typeof(ActiveObject))).ToList();

        public List<EntityRelations> GetActionTables()
            => GetEntityRelations().Where(r => r.RelationshipType.ToType().IsExtends(typeof(ActionTable<,,>))).ToList();

        public List<EntityRelations> GetNamedObjectsRelations()
            => GetEntityRelations().Where(r => r.RelationshipType.ToType().IsExtends(typeof(NamedObject))).ToList();
    }
}