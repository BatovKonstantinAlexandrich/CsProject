using System;
using System.Collections.Generic;

[Label("Тестирование атрибутов для ввода коллекции структурированных объектов")]
public class InputStructureCollectionAttributeTest : TestingService<InputStructureCollectionAttribute>
{
    public InputStructureCollectionAttributeTest(IServiceProvider sp) : base(sp)
    {
    }

    public override void OnTest()
    {
        var attr = new InputStructureCollectionAttribute();
        this.Assert(test => false == attr.IsValidValue(new List<string>()),
            "Удалось определить стукртурированную коллекцию",
            "Не удалось определить стукртурированную коллекцию");
        this.Assert(test => false == attr.IsValidValue(new List<int>()),
            "Удалось определить стукртурированную коллекцию",
            "Не удалось определить стукртурированную коллекцию");
        this.Assert(test => true == attr.IsValidValue(new List<object>()),
            "Удалось определить стукртурированную коллекцию",
            "Не удалось определить стукртурированную коллекцию");
        this.Assert(test => true == attr.IsValidValue(new List<Attribute>()),
            "Удалось определить стукртурированную коллекцию",
             "Не удалось определить стукртурированную коллекцию");
    }

    public static void Run(string[] args)
    {
        new InputPrimitiveCollectionAttributeTest().DoTest(false).ToDocument().WriteToConsole();
    }

    public override bool OnServiceTest(InputStructureCollectionAttribute service)
    {
        throw new NotImplementedException();
    }
}