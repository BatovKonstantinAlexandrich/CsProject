using System;
using System.Linq;
using System.Reflection;

namespace Console_CommonModel.CommonLib.LibExtensions.TextExtensions
{
    public class B { }
}
public class TextExtensionsTest : TestingElement
{
    public TextExtensionsTest(IServiceProvider provider) : base(provider)
    {
    }

    public void GetFilePathTest()
    {
        try
        {
            throw new NotImplementedException($"{GetType().GetTypeName()}");
        }
        catch (Exception ex)
        {
            this.Error(ex);
        }
    }

    public void GetTypesTest()
    {
        try
        {
            if ("A".GetTypes().Count() == 0)
                throw new Exception("Получить типы определённые в пространстве имён по имени namespace'а не удалось");
            Messages.Add("Получить типы определённые в пространстве имён по имени namespace'а удалось");
        }
        catch (Exception ex)
        {
            Messages.Add(ex.Message);
        }
    }

    public override void OnTest()


    {
        GetFilePathTest();
        GetTypesTest();
    }
}
