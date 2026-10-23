using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System.ComponentModel.DataAnnotations;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings
{

    [Label("Параметры соединения с Postgres")]
    public class PostgresConnectionString : AppViewBase
    {

        [Display(Name = "Сервер")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Host { get; set; }

        [Display(Name = "Порт")]
        [Required(ErrorMessage = "Обязательное поле")]
        public int Port { get; set; }


        [Display(Name = "База данных")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Database { get; set; }

        [Display(Name = "Пользователь")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string UserId { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Password { get; set; }

        //Host=127.0.0.1;Port=5432;User Id=postgres;Password=sgdf1423;Database=postgres
        public PostgresConnectionString() : this("127.0.0.1", 5433, "postgres", "postgres", "p@ssword")
        {
        }

        public PostgresConnectionString(string host, int port, string database, string userId, string password)
        {
            Host = host;
            Port = port;
            Database = database;
            UserId = userId;
            Password = password;
        }

        public override string ToString()
        {
            return $"Host={Host};Port={Port};User Id={UserId};Password={Password};Database={Database}";
        }
    }
}
