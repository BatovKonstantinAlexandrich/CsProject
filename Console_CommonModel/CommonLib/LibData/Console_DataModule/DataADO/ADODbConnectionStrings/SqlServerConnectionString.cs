using Console_CommonModel.CommonLib.LibAttributes.AttributesInput;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Namotion.Reflection;

using System;
using System.ComponentModel.DataAnnotations;

using Console_CommonModel.CommonLib.LibModels.Console_LocationModule;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings
{

    [Label("Модуль проверки соединений с БД")]
    public class SqlServerConnectionString : StateSupport, IDisposable
    {


        [Display(Name = "Сервер")]
        [InputOrder(1)]
        [Required(ErrorMessage = "Обязательное поле")]
        [Label("Server")]
        public string Server { get => Get<string>(nameof(Server)); set => Set(nameof(Server), value); }


        [InputOrder(2)]
        [Display(Name = "База данных")]
        [Required(ErrorMessage = "Обязательное поле")]
        [Label("Database")]
        public string Database { get => Get<string>(nameof(Database)); set => Set(nameof(Database), value); }

        [InputBool]
        [InputOrder(3)]
        [Display(Name = "Проверка подлинности Window")]
        [Required(ErrorMessage = "Обязательное поле")]
        [Label("Trusted Connection")]
        public bool TrustedConnection { get => Get<bool>(nameof(TrustedConnection)); set => Set(nameof(TrustedConnection), value); }

        [InputBool]
        [NotInput]
        [Label("Not Trusted Connection")]
        public bool NotTrustedConnection { get => Get<bool>(nameof(NotTrustedConnection)); set => Set(nameof(NotTrustedConnection), value); }



        [InputOrder(4)]
        [Display(Name = "Пользователь")]
        [InputBindingAttribute("NotTrustedConnection=Visible")]
        [Label("User Id")]
        public string UserId { get => Get<string>(nameof(UserId)); set => Set(nameof(UserId), value); }



        [InputOrder(5)]
        [Display(Name = "Пароль")]
        [InputBindingAttribute("NotTrustedConnection=Visible")]
        [Label("Password")]
        public string Password { get => Get<string>(nameof(Password)); set => Set(nameof(Password), value); }

        public SqlServerConnectionString() : this("DESKTOP-IHJM9RD", "DeliveryDbContext")
        {
            TrustedConnection = true;
            NotTrustedConnection = !TrustedConnection;
        }
        public SqlServerConnectionString(string server, string database) : this(server, database, true, "", "")
        {
            TrustedConnection = true;
            NotTrustedConnection = !TrustedConnection;
        }
        public SqlServerConnectionString(string server, string database, bool trustedConnection, string userId, string password)
        {
            Server = server;
            Database = database;
            TrustedConnection = trustedConnection;
            NotTrustedConnection = !TrustedConnection;
            UserId = userId;
            Password = password;
            AddPropertyChangeListener(nameof(TrustedConnection), evt =>
            {
                NotTrustedConnection = evt.After.ToString().ToLower() != "true";
            });

        }


        public override string ToString()
        {
            //"Data Source=DESKTOP-IHJM9RD;Integrated Security=True;Connect Timeout=30;Encrypt=False;Application Intent=ReadWrite;Multi Subnet Failover=False"
            string constr = $"Data Source={Server};Initial Catalog={Database};";
            if (TrustedConnection)
            {
                constr = constr + "Integrated Security=True;MultipleActiveResultSets=true;Encrypt=False;";
            }
            else
            {
                constr += $"UId={UserId};PWD={Password};MultipleActiveResultSets=true;Encrypt=False;";
            }
            return constr;
        }


        public void Configure(string connectionString)
        {
            TrustedConnection = false;
            foreach (string s in connectionString.Split(";"))
            {
                var arr = s.Split("=");
                var key = arr[0].ToLower();
                string val = arr.Length > 1 ? arr[1] : null;

                if (key == "Data Source".ToLower() || key == "Server".ToLower())
                {
                    Server = val;
                }
                if (key == "UID".ToLower())
                {
                    UserId = val;
                }
                if (key == "PWD".ToLower())
                {
                    Password = val;
                }
                if (key == nameof(TrustedConnection).ToLower() || "Integrated Security".ToLower() == key)
                {
                    TrustedConnection = true;
                }
                if (key == "Initial Catalog".ToLower() || key == "Database".ToLower())
                {
                    Database = val;
                }
            }
        }

        public void Dispose()
        {
        }

        internal void SetFromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            foreach (var message in value.Split(";"))
            {
                var arr = message.SplitSingle("=");
                var key = arr.Key;
                var val = arr.Value;
                if (this.HasProperty(key))
                    this.SetValue(key, val);
            }
        }
    }

}