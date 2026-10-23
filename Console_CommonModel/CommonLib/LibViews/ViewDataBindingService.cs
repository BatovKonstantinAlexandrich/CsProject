using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews
{

    /// <summary>
    /// Класс представляет функции для связывания данных спредставллением
    /// </summary>    
    public abstract class ViewDataBindingService<TAppViewBase> : EventSupport
        where TAppViewBase : AppViewBase
    {

        public abstract void Bind(TAppViewBase view);
    }


    /// <summary>
    /// 
    /// </summary>
    public class ListDataBindingService : ViewDataBindingService<ListView>
    {
        public string DbContextType { get => Get<string>(nameof(DbContextType)); set => Set(nameof(DbContextType), value); }
        public string EntityType { get => Get<string>(nameof(EntityType)); set => Set(nameof(EntityType), value); }
        public string TextExpression { get => Get<string>(nameof(TextExpression)); set => Set(nameof(TextExpression), value); }

        [NotInput]
        private ChangingList<string> DbContextTypes { get; set; } = new();

        [NotInput]
        private ChangingList<string> EntityTypes { get; set; } = new();

        public ListDataBindingService()
        {
            DbContextTypes.AddRange(ServiceFactory.Get().GetTypesExtended<DbContextBase>().Where(t => !t.IsAbstract && t.GetGenericArguments().Count() == 0).Select(t => t.GetTypeName()).ToList());
        }


        [OnChange(nameof(DbContextType))]
        public void OnDbContextTypeChanged()
        {
            using (var db = DbContextType.ToType().New<DbContextBase>())
            {
                EntityTypes.Clear();
                EntityTypes.AddRange(db.GetEntitiesTypes().Select(t => t.GetTypeName()).ToList());
            }
        }

        public override void Bind(ListView view)
        {
            throw new NotImplementedException();
        }
    }

}
