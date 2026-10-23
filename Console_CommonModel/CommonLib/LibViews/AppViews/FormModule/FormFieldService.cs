using Console_CommonModel.CommonLib.LibViews.AppViewModules.ComboboxModule;

using System.Linq;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.FormModule
{
    public class FormFieldService
    {
        public ComboboxView CreateInputType(InputFormField field)
        {
            var combobox = new ComboboxView() { Text = field.Label };
            var types = ServiceFactory.Get()
                .GetTypesExtended<BaseInputAttribute>()
                .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
                .Select(t => t.GetTypeName())
                .Distinct()
                .OrderBy(x => x)
                .ToList();
            combobox.Options.AddRange(types);

            var selected = $"{field.Value}";
            if (string.IsNullOrWhiteSpace(selected))
            {
                selected = field.Attributes.Keys
                    .FirstOrDefault(key => key.ToType().IsExtends(typeof(BaseInputAttribute)));
            }
            if (!string.IsNullOrWhiteSpace(selected) && types.Contains(selected))
            {
                combobox.Value = selected;
            }

            combobox.OnSelected += typeName =>
            {
                if (string.IsNullOrWhiteSpace(typeName))
                    return;
                field.Value = typeName;
                field.Attributes ??= new Dictionary<string, string>();
                foreach (var key in field.Attributes.Keys.ToList())
                {
                    if (key.ToType().IsExtends(typeof(BaseInputAttribute)))
                        field.Attributes.Remove(key);
                }
                field.Attributes[typeName] = "";
                field.OnInput?.Invoke(typeName);
            };
            return combobox;
        }

        public InputFormField CreateFormField(object target, string property)
        {
            try
            {
                var field = new InputFormField(property, target.GetType(), target.GetType().GetPropertyAttributes(property));
                return field;

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public InputFormField CreateFormField2(object target, string property)
        {
            try
            {
                var field = new InputFormField(property, target.GetType(), target.GetType().GetPropertyAttributes(property));
                return field;

            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }

}
