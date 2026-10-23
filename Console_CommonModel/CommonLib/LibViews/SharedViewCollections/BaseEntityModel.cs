using System.Reflection;

using Console_CommonModel.CommonLib.LibViews.AppViews.FlexModule;

namespace Console_CommonModel.CommonLib.LibViews.SharedViewCollections
{

    [Label("Моделирование сущностей")]
    public abstract class BaseEntityModel : CardView
    {



        public virtual void OnEntity([NotNullNotEmpty] object target)
        {
            MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(target);
            var propertyNames = target.GetType().GetProperties().Select(p => p.Name).ToList();
            OnEntity(target, propertyNames);
        }

        public virtual void OnEntity<ExtType>([NotNullNotEmpty] object target)
        {
            //MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(target);
            var type = typeof(ExtType);
            var propertyNames = typeof(ExtType).GetProperties().Where(p => p.DeclaringType == type).Select(p => p.Name).ToList();
            OnEntity(target, propertyNames);
        }

        public virtual void OnEntity([NotNullNotEmpty] object target, [NotNullNotEmpty] List<string> properties)
        {
            //MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(target, properties);
            foreach (var property in properties)
            {
                var value = target.GetValue(property);
                var attributes = target.GetType().GetPropertyAttributes(property);
                OnProperty(target, property, value, attributes);
            }
        }

        public abstract void OnProperty(object target, string property, object value, Dictionary<string, string> attributes);

    }
}
