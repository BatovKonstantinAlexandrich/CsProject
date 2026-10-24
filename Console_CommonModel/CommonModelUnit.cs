using Console_CommonModel.CommonLib;
using Console_CommonModel.CommonLib.LibExtensions;

namespace Console_CommonModel
{
    [Label("Модуль тестирования сборки Console_CommonModel")]
    public class CommonModelUnit : TestingUnit
    {
        public CommonModelUnit() : base(AppProviderService.GetInstance())
        {
            Append(new LibUnit(provider));
            Append(new LibUnit(provider));
            Append(new TypeDocumentationTest(provider));            
            Append(new LibExtensionsUnit(provider));
        }

   
    }
}
