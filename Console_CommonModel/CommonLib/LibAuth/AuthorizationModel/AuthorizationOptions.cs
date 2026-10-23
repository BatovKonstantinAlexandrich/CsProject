/// <summary>
/// Параметры системы аутентификации.
/// </summary>
[Label("Настройки аутентификации")]
public class AuthorizationOptions
{


    [InputPositiveInt()]
    public long SessionTimeout { get; set; }


    [InputPositiveInt()]
    public int KeyLength { get; set; }



    [NotNullNotEmpty()]
    public string UserCookie { get; set; }



    [NotNullNotEmpty()]
    public string ServiceCookie { get; set; }



    [InputPercent()]
    public int CheckTimeout { get; set; }



    [InputUrl]
    public string ApplicationUrl { get; set; }




    /// <summary>
    /// Роль пользователя по умолчанию,
    /// присваивается пользователям после 
    /// проведеня процедуры регистрации
    /// </summary>
    public string PublicRole { get; set; }
    public string PublicGroup { get; set; }

    public string[] Providers = new string[] { };

    public string LoginPagePath { get; internal set; } = "/Auth/Signin";
    public bool LoggingAuth { get; internal set; }

    public AuthorizationOptions()
    {
        this.SessionTimeout = 1000 * 60 * 10;   // 10 имн
        this.KeyLength = 32;
        this.UserCookie = "UserKey";
        this.ServiceCookie = "ServiceKey";
        this.CheckTimeout = 1000;
        this.PublicRole = "Reader";
        this.PublicGroup = "Reader";
        this.ApplicationUrl = "https://localhost:6161";
    }
}