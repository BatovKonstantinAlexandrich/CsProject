using Console_CommonModel.CommonLib.LibModels.Console_ServiceModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule
{
    public class DataModelIntegration : IntegrationService<ModelsDbContext, ProcedureModel>
    {
        public DataModelIntegration()
        {

        }
        public DataModelIntegration(ModelsDbContext dbContext) : base(dbContext)
        {

        }

        protected override void OnUpdated(
            Dictionary<string, PropertyChangedMessage> changes,
            ProcedureModel beforeChanges,
            ProcedureModel afterChanges)
        {
            this.Info("OnUpdated");
            this.Info(changes.ToJsonOnScreen());
        }

        protected override void OnDeleted(ProcedureModel afterChanges)
        {
        }

        protected override void OnAdded(ProcedureModel afterChanges)
        {
        }
    }
}
