namespace Console_CommonModel.CommonLib.LibModels.Console_XmlModule
{
    [Label("Тест генерации компонентов по XML")]
    [Description("XmlComponentsFactory создаёт представления из ComponentParams с сохранением свойств (например TextAlign).")]
    public class ComponentFactoryTest : TestingService<XmlComponentsFactory>
    {
        public ComponentFactoryTest(IServiceProvider sp) : base(sp)
        {
        }

        public override bool OnServiceTest(XmlComponentsFactory service)
        {
            LogMessage("Создание ButtonView из ComponentParams через XmlComponentsFactory");
            int counter = 0;
            Assert(p =>
            {
                var button = new ButtonView()
                {
                    TextAlign = "left"
                };
                var buttonParams = button.ToComponentParams();
                var created = service.Create(buttonParams, new ComponentModel()
                {
                    Target = button,
                    Parameters = buttonParams
                }, new DataReport());
                bool result = ((ButtonView)created).TextAlign == "left";
                LogMessage($"TextAlign после Create: {((ButtonView)created).TextAlign}, ожидалось left");
                if (result) counter++;
                return result;
            },
            "Создание объектов из XML возможно",
            "Создание объектов из XML не возможно");
            return counter == 1;
        }
    }
}
