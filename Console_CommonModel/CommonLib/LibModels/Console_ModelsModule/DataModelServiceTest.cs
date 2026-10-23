using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;

namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule
{
    [Label("Models: форма полей процедуры DataModelService")]
    [Description("GetProcedureFormFields и InputPanel: отображение диалога с полями процедуры (интерактивный тест).")]
    public class DataModelServiceTest : TestingService<DataModelService>
    {
        public override bool OnServiceTest(DataModelService service)
        {
            LogMessage("GetProcedureFormFields(1)");
            var formFields = service.GetProcedureFormFields(1);
            LogMessage($"Полей формы: {formFields?.Count ?? 0}");
            var inputPanel = new AppViewBase();
            inputPanel.UpdateFormFields(formFields);
            LogMessage("Отображение диалога InputPanel");
            inputPanel.ShowDialog();
            LogMessage("Диалог закрыт");
            return true;
        }
    }
}
