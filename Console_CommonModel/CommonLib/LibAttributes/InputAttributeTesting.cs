using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static AttributesInputTest;

namespace Console_CommonModel.CommonLib.LibAttributes
{
    public class TestModel
    {
        public object TestProperty { get; set; }
    }

    public class InputAttributeTesting<TInputAttribute> : TestingElement
        where TInputAttribute : BaseInputAttribute
    {
        public InputAttributeTesting()
        {
        }

        public InputAttributeTesting(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {


            EditView.Fields.Clear();

            foreach (var kv in typeof(TestModel).GetPropertyAttributes(nameof(TestModel.TestProperty)).Keys)
            {
                typeof(TestModel).RemovePropertyAttribute(
                    nameof(TestModel.TestProperty),
                    kv);
            }


            typeof(TestModel).AddPropertyAttribute(nameof(TestModel.TestProperty),
               typeof(TInputAttribute).GetTypeName(), "");
            this.Info(typeof(TestModel).GetPropertyAttributes(nameof(TestModel.TestProperty)).Keys);
            EditView.CreateEditView(new TestModel(), new List<string>() { nameof(TestModel.TestProperty) })
            .ShowDialog(
                $"[{typeof(TInputAttribute).GetTypeName()}]. {typeof(TInputAttribute).GetLabel()}"
            );
            typeof(TestModel).RemovePropertyAttribute(
                nameof(TestModel.TestProperty),
                typeof(TInputAttribute).GetTypeName());


            this.Info(typeof(TestModel).GetPropertyAttributes(nameof(TestModel.TestProperty)));
            TypeAttributesExtensions.useClassDbContext = false;


        }

    }
}
