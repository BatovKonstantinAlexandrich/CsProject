namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.AppBusiness
{
    [Label("Delivery: бизнес-функции пользователя Holder")]
    [Description("UserBusinessFunctionsService возвращает зарегистрированные BusinessFunction для роли Holder.")]
    public class UserBusinessFunctionsTest : TestingService<UserBusinessFunctionsService<DeliveryDbContext>>
    {
        public UserBusinessFunctionsTest()
        {
        }

        public UserBusinessFunctionsTest(IServiceProvider sp) : base(sp)
        {
        }

        public override bool OnServiceTest(UserBusinessFunctionsService<DeliveryDbContext> service)
        {
            LogMessage("GetBusinessFunctions(\"Holder\")");
            var functions = service.GetBusinessFunctions("Holder").ToList();
            LogMessage($"Найдено бизнес-функций: {functions.Count}");
            foreach (var (businessFunction, metadata) in functions.Take(5))
                LogMessage($"  — {businessFunction?.GetType().Name ?? "null"} / {metadata?.ProcedureLabel ?? metadata?.ProcedureQualifier ?? "—"}");
            return functions.Count > 0;
        }
    }
}
