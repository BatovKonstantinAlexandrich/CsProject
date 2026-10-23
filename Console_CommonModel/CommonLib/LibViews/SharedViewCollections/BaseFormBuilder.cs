using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Namotion.Reflection;

namespace Console_CommonModel.CommonLib.LibViews.SharedViewCollections
{
    [Label("Стандартный конструктор форм")]
    public class BaseFormBuilder : BaseEntityModel
    {
        public readonly ChangingList<InputFormField> formFields = new ChangingList<InputFormField>();

        public BaseFormBuilder()
        {
            LayoutFill(EditView.CreateEditViewBase(new UserAccount(), (s, p) => { }));
        }

        public override void OnEntity([NotNullNotEmpty] object target, [NotNullNotEmpty] List<string> properties)
        {

            base.OnEntity(target, properties);
        }

        public InputFormField CreateFormField(object target, string property, object value, Dictionary<string, string> attributes)
        {
            var result = new InputFormField();
            result.Name = property;
            result.Value = value;
            result.OnInput += (evt) =>
            {
                target.SetTextValue(property, evt is null ? null : evt.ToString());
            };
            foreach (var attribute in attributes)
            {
                var name = attribute.Key.Replace("Attribute", "");
                if (result.HasProperty(name))
                {
                    result.SetTextValue(name, attribute.Value);
                }
            }
            return result;
        }

        public override void OnProperty(object target, string property, object value, Dictionary<string, string> attributes)
        {
            var field = CreateFormField(target, property, value, attributes);
            //field.EnsureIsValide();
            formFields.Add(field);
        }
    }
}
