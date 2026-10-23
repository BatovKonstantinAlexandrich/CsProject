namespace Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule.BusinessActionServices
{


    /// <summary>
    /// /
    /// </summary>
    /// <param name="signin"></param>
    public class BusinessUserService<TDeliveryDbContext>(   ) : IBusinessUserService
        where TDeliveryDbContext : DeliveryDbContext
    {


        [Label("Получаем список бизнес-функций для текущего пользователя")]
        public Dictionary<string, string> GetActions()
        {
            Dictionary<string, string> result = new();
            var signin = AppProviderService.GetInstance().Get<SigninUser<TDeliveryDbContext>>();
            if (!signin.IsSignin())
                throw new Exception("Требуется аутентификация");
            var user = signin.Verify();
            var db = signin.GetDbContext();


            foreach (var role in user.UsersResources)
            {
                var functions = db.BusinessFunctions.Where(f => f.BusinessResourceId == role.RoleId).ToList();

                this.Info($"{role.RoleId} = {functions.Count()}");
                foreach (var function in functions)
                {
                    result[function.Name] = function.Description;
                }
            }

            return result;
        }
         
    }
}
