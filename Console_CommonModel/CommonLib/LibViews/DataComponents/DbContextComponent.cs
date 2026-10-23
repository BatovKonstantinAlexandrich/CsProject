using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using System.ComponentModel;

namespace Console_CommonModel.CommonLib.LibViews.DataComponents
{
    /// <summary>
    /// Компонент контекста данных EF
    /// </summary>
    /// <typeparam name="TDbContextBase"></typeparam>
    [Label("Контекст данных")]
    [Description("Компонент для работы с Entity Framework Core DbContext, предоставляющий методы для получения иерархических таблиц, именованных объектов и таблиц действий")]
    public class DbContextComponent<TDbContextBase> : Component
        where TDbContextBase : DbContextBase
    {


        /// <summary>
        /// 
        /// </summary>
        public DbContextInitiliazer<DbContextUser> Initiallizer { get; set; }







        //////////////////////////////////////////////////////////////////////
        public List<Type> GetHierTables() => typeof(TDbContextBase).GetEntitiesTypes().Where(et => et.IsExtends(typeof(HierTable<>))).ToList();
        public List<Type> GetNamedObject() => typeof(TDbContextBase).GetEntitiesTypes().Where(et => et.IsExtends(typeof(NamedObject))).ToList();
        public List<Type> GetActionTables() => typeof(TDbContextBase).GetEntitiesTypes().Where(et => et.IsExtends(typeof(ActionTable<,,>))).ToList();
        //////////////////////////////////////////////////////////////////////

    }
}
