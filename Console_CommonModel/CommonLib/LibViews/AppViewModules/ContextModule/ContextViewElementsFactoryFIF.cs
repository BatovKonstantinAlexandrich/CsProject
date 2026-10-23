using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.AppViews.FormModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.ContextModule
{




    internal class ContextViewElementsFactoryFIF : IContextViewElementsFactoryGeneric<InputFormField>
    {
        private IContextViewElementsFactory contextViewElementsFactory = new ContextViewElementsFactory();

        public ContextView CreateContextView(InputFormField formInputField)
        {

            var baseInputType = formInputField.Attributes.Select(kv => kv.Key).FirstOrDefault(kv =>
                ServiceFactory.Get().GetTypesExtended<BaseInputAttribute>().Select(T => T.GetTypeName())
                  .Contains(kv));

            if (baseInputType is not null && (baseInputType.ToType().IsExtends(typeof(InputSelectAttribute)) ||
                baseInputType.ToType() == typeof(InputSelectAttribute)))
            {
                return CreateRadioGroup(formInputField);

            }
            else if (!Typing.IsPrimitive(formInputField.ValueType))
            {
                return CreateButton(formInputField);
            }
            else
            {

                if (Typing.IsLogicalType(formInputField.ValueType.ToType()))
                {
                    return CreateCheckbox(formInputField);
                }
                else
                {
                    return CreateTextbox(formInputField);
                }
            }



        }


        public ContextViewButton CreateButton(InputFormField formInputField)
        {

            return contextViewElementsFactory.CreateButton(formInputField.Icon, formInputField.Label, () =>
            {
                var ctrl = formInputField.Control is not null ?
                    formInputField.Control : formInputField;
                ctrl.Cast<AppViewBase>().ToSubmit(formInputField.Label, (s, p) =>
                {
                    if (s)
                    {
                        formInputField.OnInput(p.ToJsonUniversal());
                    }
                });
            });
        }

        public ContextViewCheckbox CreateCheckbox(InputFormField formInputField)
        {
            var isChecked = formInputField.Target.GetValue<bool>(formInputField.Name);
            return contextViewElementsFactory.CreateCheckbox("home",
                formInputField.Target.GetType().GetPropertyLabel(formInputField.Name),
                isChecked,
                val =>
                {
                    formInputField.Target.SetValue(formInputField.Name, val);
                }
            );
        }

        public ContextView CreateRadioGroup(InputFormField formInputField)
        {
            var kv = formInputField.Attributes.FirstOrDefault(kv =>
                ServiceFactory.Get().GetTypesExtended<InputSelectAttribute>().Select(T => T.GetTypeName())
                  .Contains(kv.Key));
            var options = kv.Key.ToType().New<InputSelectAttribute>(kv.Value).Options;
            return contextViewElementsFactory.CreateRedioGroup(formInputField.Icon, formInputField.Label, $"{formInputField.Value}",
                formInputField.Target, options, text =>
                {
                    formInputField.OnInput(text);
                }
            );

        }

        public ContextViewTextbox CreateTextbox(InputFormField formInputField)
        {
            var val = formInputField.Target.GetValue(formInputField.Name);
            return contextViewElementsFactory.CreateTextbox("home",
                formInputField.Target.GetType().GetPropertyLabel(formInputField.Name),
                val is not null ? val.ToString() : "",
                nval =>
                {
                    formInputField.Target.SetValue(formInputField.Name, nval);
                }
            );
        }
    }
}
