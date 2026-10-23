using Console_CommonModel.CommonLib.LibViews.AppViews.FormModule;

using Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputCustom;

using static AttributesInputTest;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Console_CommonModel.CommonLib.Others;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.AttributeTests
{
    /// <summary>
    /// Проверка атрибута ввода методом создания формы ввода
    /// и установка и удаления атрибута на свойство.
    /// </summary>
    [Label("Проверка атрибута '\"{{AttributeTypeName}}\"'")]
    [Description("{{GetAttributeDescription()}}")]
    public class GenericInputAttributeTest<TAttribute> : TestingElement
        where TAttribute : BaseInputAttribute
    {
        public string AttributeTypeName { get; set; }
        public string TypeName { get; set; }

        public GenericInputAttributeTest() : base(AppProviderService.GetInstance())
        {
            Name = this.GetTypeName();
            TypeName = this.GetTypeName();
            AttributeTypeName = typeof(TAttribute).GetTypeName();
        }

        public GenericInputAttributeTest(IServiceProvider provider) : base(provider)
        {
            Name = this.GetTypeName();
            TypeName = this.GetTypeName();
            AttributeTypeName = typeof(TAttribute).GetTypeName();
        }
        public override string GetTitle() =>
            $"{typeof(TAttribute).GetLabel()}";

        public string GetAttributeTypeName()
            => typeof(TAttribute).GetTypeName();

        public Type GetAttributeType()
            => typeof(TAttribute);

        public string GetAttributeDescription()
            => $"{typeof(TAttribute).GetLabel()}. {typeof(TAttribute).GetDescription()}";

        public override TestingReport DoTest(bool interactive = false, bool force = false)
        {
            return base.DoTest(interactive, force);
        }

        public override void OnTest()
        {
            LogMessage($"Атрибут ввода: {typeof(TAttribute).GetTypeName()}");
            Assert(p =>
            {
               
                EditView.Fields.Clear();

                var attributeValue = GetAttributeTestValue(typeof(TAttribute));
                LogMessage($"Тестовое значение атрибута: {attributeValue}");
                typeof(TestModel).AddPropertyAttribute(
                    nameof(TestModel.TestProperty),
                    typeof(TAttribute).GetTypeName(),
                    attributeValue
                );
                var model = CreateTestModel();

                if (attributeValue !=
                    model.GetType().GetPropertyAttribute(
                        nameof(TestModel.TestProperty),
                        typeof(TAttribute).GetTypeName()))
                {
                    throw new Exception("Не удалось изменить значение для атрибута свойства");
                }

                
                var form = EditView.CreateEditViewBase(model);
                //form.TraceFormFields();
                var formControl = form.ToControl();
                form.ToSubmit((success, data) =>
                {
                    if (success)
                    {

                        CreateSubmitDialog(
                            "Проверьте данные",
                            data.control,
                            (s) =>
                            {
                                //result = true;
                            }
                        );

                    }
                }).ShowDialog($"Проверка атрибута {typeof(TAttribute).GetTypeName()}");

                typeof(TestModel).RemovePropertyAttribute(
                    nameof(TestModel.TestProperty),
                    typeof(TAttribute).GetTypeName()
                );

                return true;
            }, $"{typeof(TAttribute).GetTypeName()} успешно прошел проверку"
             , $"{typeof(TAttribute).GetTypeName()} не прошел проверку");
        }

        private void CreateSubmitDialog(string title, object control, Action<object> value)
        {
            InputConsole.CreateSubmitDialog(title,control,value);
        }

        private static string GetAttributeTestValue(Type attributeType)
        {
            if (attributeType.IsExtends(typeof(InputStructureCollectionAttribute)))
                return typeof(DateModel).GetTypeName();
            if (attributeType.IsExtends(typeof(InputObjectAttribute)))
                return typeof(DateModel).GetTypeName();
            if (attributeType.Name.StartsWith(nameof(InputInterfaceCollectionAttribute), StringComparison.Ordinal))
                return "GetLabel()";
            if (attributeType.IsExtends(typeof(InputPrimitiveCollectionAttribute)))
                return typeof(string).GetTypeName();
            if (attributeType.IsExtends(typeof(SelectDictionaryAttribute)))
                return $"{nameof(MessageProtocol)},Name";
            
            if (attributeType.IsExtends(typeof(InputComboboxAttribute)))
                return $"{nameof(MessageProtocol)},{nameof(MessageProtocol.Name)}";
            if (attributeType.IsExtends(typeof(InputDictionaryAttribute)))
                return $"{nameof(MessageProtocol)},{nameof(MessageProtocol.Name)}";
            if (attributeType.IsExtends(typeof(InputEnumAttribute)))
                return nameof(CommandType);
            if (attributeType.IsExtends(typeof(InputSelectEnumAttribute)))
                return nameof(CommandType);
            if (attributeType.IsExtends(typeof(InputSelectAttribute)))
                return "a,b,c";
            return attributeType.GetDescription();
        }

        private static TestModel CreateTestModel()
        {
            var model = new TestModel();
            if (typeof(TAttribute).IsExtends(typeof(InputStructureCollectionAttribute)))
                model.TestProperty = new List<DateModel>();
            else if (typeof(TAttribute).IsExtends(typeof(InputPrimitiveCollectionAttribute)))
                model.TestProperty = new List<string>();
            else if (typeof(TAttribute).IsExtends(typeof(InputObjectAttribute)))
                model.TestProperty = new DateModel();
            else if (typeof(TAttribute).Name.StartsWith(nameof(InputInterfaceCollectionAttribute), StringComparison.Ordinal))
                model.TestProperty = new List<AppViewBase>();
            else if (typeof(TAttribute).IsExtends(typeof(InputFileDataAttribute))
                     || typeof(TAttribute).IsExtends(typeof(InputFileAttribute))
                         && !typeof(TAttribute).IsExtends(typeof(InputImageAttribute)))
                model.TestProperty = Array.Empty<byte>();
            return model;
        }

        /// <summary>
        /// На нём буду тестировать
        /// </summary>
        public class TestModel
        {
            public object TestProperty { get; set; }
        }
    }
}
