using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.AppBusiness;

namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.Authorization
{
    /// <summary>
    /// Построитель бизнес-процессов
    /// </summary>
    public class BusinessProcessBuilder<TBusinessDbContext> : IBusinessProcessBuilder
        where TBusinessDbContext : DeliveryDbContext
    {

        private BusinessProcess process = new();

        private UserBusinessFunctionsService<DeliveryDbContext> _businessFunctionsService;

        public BusinessProcessBuilder()
        {

            _businessFunctionsService = new UserBusinessFunctionsService<DeliveryDbContext>();
        }

        public void StartWith<InputModel, OutputModel, ErrorModel>(string name)
        {
            process.Name = name;
            process.Input = _businessFunctionsService.CreateMessageProtocol<InputModel>();
            process.Error = _businessFunctionsService.CreateMessageProtocol<ErrorModel>();
            process.Output = _businessFunctionsService.CreateMessageProtocol<OutputModel>();
            process.EnsureIsValide();
        }



        public BusinessProcess BuildBusiness()
        {
            try
            {
                process.Create<TBusinessDbContext>();
                var messages = AppProviderService.GetInstance().Get<BusinessProcessValidator<DeliveryDbContext>>().Validate(process);
                //var businessResourceName = typeof(TBusinessResource).GetTypeName();
                using (var db = typeof(TBusinessDbContext).New<DeliveryDbContext>())
                {
                    //var resource = db.CheckRegistration<BusinessResource>(businessResourceName);
                    var input = db.CheckRegistration(process.Input);
                    var output = db.CheckRegistration(process.Output);
                    foreach (var function in process.BusinessFunctions)
                    {
                        db.CheckRegistration<BusinessResource>(function.BusinessResource);
                        function.Output?.ResetPropertiesMessageAttributeRef();
                        var foutput = db.CheckRegistration<MessageProtocol>(function.Output);
                        function.BusinessResource = null;
                        db.CheckRegistration(function);
                    }
                }
                return process;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public IBusinessProcessBuilder CreateUserBusinessFunction<TBusinessResource, TInputMessageProtocol>(Dictionary<string, BusinessFunction> logic)
        {
            var created = _businessFunctionsService.CreateUserBusinessFunction<TInputMessageProtocol>(logic);
            using (var db = typeof(TBusinessDbContext).New<TBusinessDbContext>())
            {
                var resourceName = typeof(TBusinessResource).GetTypeName();
                created.BusinessResource = db.CheckRegistration<BusinessResource>(resourceName);
                created.BusinessResourceId = created.BusinessResource.Id;


                db.CheckRegistration(created);

                created.EnsureIsValide();
                if (process.BusinessFunctions.Count() == 0)
                {
                    created.Input = process.Input;
                }
                else
                {
                    var lastBf = process.BusinessFunctions.Last();
                    lastBf.Output = created.Input;
                }

                process.BusinessFunctions.Add(created);
            }
            return this;
        }
    }

}
