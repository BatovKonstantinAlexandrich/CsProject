using Console_CommonModel.CommonLib.LibAttributes;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

using System;
using System.Collections.Generic;

public class InputInterfaceCollectionAttributeTest : InputAttributeTesting<InputInterfaceCollectionAttribute<AppViewBase>>
{
    public InputInterfaceCollectionAttributeTest(IServiceProvider provider) : base(provider)
    {
    }

    public override void OnTest()
    {
        EditView.Fields.Clear();

        foreach (var kv in typeof(TestModel).GetPropertyAttributes(nameof(TestModel.TestProperty)).Keys)
        {
            typeof(TestModel).RemovePropertyAttribute(nameof(TestModel.TestProperty), kv);
        }

        typeof(TestModel).AddPropertyAttribute(
            nameof(TestModel.TestProperty),
            typeof(InputInterfaceCollectionAttribute<AppViewBase>).GetTypeName(),
            "GetLabel()");

        var model = new TestModel { TestProperty = new List<AppViewBase>() };

        EditView.CreateEditView(model, new List<string> { nameof(TestModel.TestProperty) })
            .ShowDialog($"[{typeof(InputInterfaceCollectionAttribute<AppViewBase>).GetTypeName()}]. {typeof(InputInterfaceCollectionAttribute<AppViewBase>).GetLabel()}");

        typeof(TestModel).RemovePropertyAttribute(
            nameof(TestModel.TestProperty),
            typeof(InputInterfaceCollectionAttribute<AppViewBase>).GetTypeName());
    }
}
