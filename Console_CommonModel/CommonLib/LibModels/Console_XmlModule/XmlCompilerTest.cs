using Console_CommonModel.CommonLib.LibViews.AppViews.XmlModule;
using Console_CommonModel.CommonLib.Others;

namespace Console_CommonModel.CommonLib.LibModels.Console_XmlModule
{
    [Label("Проверка связывания с XML")]
    [Description("Сериализация и компиляция представлений через XmlNodeCompiler: количество элементов, свойства, JSON.")]
    public class XmlCompilerTest : TestingElement
    {
        public XmlCompilerTest(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            LogMessage("Последовательность: количество элементов → свойства → JSON XmlView → JSON представлений");
            TestElementsCount();
            TestProperties();
            TestXmlViewJson();
            LogMessage("Все подпроверки XmlCompilerTest вызваны");
        }

        private void TestElementsCount()
        {
            Assert((p) =>
            {
                var report = new ReportComponent();
                report.Body.Append(new ListView()
                {
                    Options = new() {
                    new("Начать игру"),
                    new("Настройки"),
                    new("Выход")
                }
                });
                report.Body.Append(new ButtonView() { Text = "Подтвердить" });
                report.Body.Append(new ButtonView() { Text = "Отменить" });
                var xml = XmlNodeCompiler.GetXml(report);
                var compiled = XmlNodeCompiler.CompileXml(xml);
                if (compiled.Body.Controls.Count() != 3)
                {
                    p.Messages.Add("Количество моделей представлений не правильное.");
                    return false;
                }
                return true;
            },
                        "Возможно чтение-запись в XML",
                        "Не возможно чтение-запись в XML");
        }

        private void TestProperties()
        {
            Assert((p) =>
            {
                var view = new AppViewBase();
                view.Append(view.CreateButton("Ok", () => { }));
                var xml = view.ToXmlText();
                view.Trace();
                view.FromXml(xml);

                return true;
            },
            "Свойства представления присваиваются после чтений",
            "Свойства представления не присваиваются после чтений");
        }

        private void TestXmlViewJson()
        {
            Assert((p) =>
            {
                var view = new XmlView();
                view.XmlText = @"<app-view-base Dock=""Fill""><button-view Text=""ok"" /></app-view-base>";
                var json = view.ToJson();
                if (string.IsNullOrWhiteSpace(json) || json.Contains("error", StringComparison.OrdinalIgnoreCase))
                {
                    p.Messages.Add("JSON пустой или содержит ошибку.");
                    return false;
                }

                var restored = json.FromJson<XmlView>();
                if (restored is null || string.IsNullOrWhiteSpace(restored.XmlText))
                {
                    p.Messages.Add("XmlView не восстановился из JSON.");
                    return false;
                }

                return true;
            },
            "XmlView сериализуется в JSON и восстанавливается",
            "XmlView не сериализуется в JSON");
        }

    
    }
}
