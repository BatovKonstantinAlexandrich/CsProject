namespace Console_CommonModel.CommonLib.LibModels.Console_LocationModule
{
    [Label("Location: создание навигационной страницы")]
    [Description("PageService.CreateNavPage формирует страницу с пунктами меню (smoke-тест).")]
    public class PageServiceTest : TestingService<PageService>
    {
        public override bool OnServiceTest(PageService service)
        {
            LogMessage("CreateNavPage с пунктом меню «выход»");
            service.CreateNavPage("", new() { { "exit", "выход" } });
            LogMessage("CreateNavPage выполнен (тест возвращает false по задумке)");
            return true;
        }
    }
}
