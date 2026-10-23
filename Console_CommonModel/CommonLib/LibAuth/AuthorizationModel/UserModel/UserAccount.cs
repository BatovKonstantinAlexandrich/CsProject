
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using Newtonsoft.Json;

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;


public class PrivateData<TActiveObject>: BaseEntity
{
    
}


/// <summary>
/// Ученая запись пользователя
/// </summary>
[Label("Учетная запись")]
[Icon("account_box")]
public class UserAccount: PrivateData<UserContext>
{

    

    [InputEmail("Электронный адрес задан некорректно")]
    [Label("Электронный адрес")]
    [NotNullNotEmpty("Не указан электронный адрес")]
    [Icon("email")]
    [InputOrder(1)]
    //[UniqValue(nameof(DbContextUser), "Этот адрес электронной почты уже зарегистрирован")]
    //[UniqValue(nameof(BusinessDbContext), "Этот адрес электронной почты уже зарегистрирован")]
    public string Email { get; set; } = "customer@gmail.com";


    [Label("Пароль")]
    [NotNullNotEmpty]
    [InputPassword()]
    [InputOrder(2)]
    public string Password { get; set; } = "sgdf1423";


    /// <summary>
    /// Время активации
    /// </summary>
    [AllowNull]
    [InputDate( )]
    [InputHidden("true")]
    [NotInput("Свойство " + nameof(Activated) + " не вводится пользователем, оно устанавливается системой после ввода ключа активации")]
    public DateTime? Activated { get; set; } = null;

    [InputHidden("true")]
    [NotInput("Свойство " + nameof(ActivationKey) + " не вводится пользователем, оно устанавливается системой перед созданием сообщения на эл. почту пользорвателя с инструкциями по активации")]
    public string ActivationKey { get; set; } = "";

    [Label("Хэш-ключ")]
    [InputHidden("true")]
    [NotNull]
    [NotNullNotEmpty]
    [NotInput("Свойство " + nameof(Hash) + " не вводится пользователем, оно устанавливается системой при регистрации")]
    public string Hash { get; set; }

    [Label("Радио метка")]
    [InputHidden("true")]
    [NotInput("Свойство " + nameof(RFId) + " не вводится пользователем, оно устанавливается системой при регистрации служебного билета")]
    public string RFId { get; set; } = "";


    public UserAccount() :base()
    {        
        
        this.Email = $"user_@gmail.com";        
    }

    public UserAccount(string email, string password):base(   )
    {                   
        this.Email = email;
        this.Password = password;
        this.Hash = GetHashSha256(password);
    }

 


    /// <summary>
    /// Хэширование текстых данных
    /// </summary>
    /// <param name="text"> текст </param>
    /// <returns> результат хэширования </returns>
    public static string GetHashSha256(string text)
    {
        if (String.IsNullOrWhiteSpace(text))
            return text;
        byte[] bytes = Encoding.Unicode.GetBytes(text);
        SHA256Managed hashstring = new SHA256Managed();
        byte[] hash = hashstring.ComputeHash(bytes);
        string hashString = string.Empty;
        foreach (byte x in hash)
        {
            hashString += String.Format("{0:x2}", x);
        }
        return hashString;
    }

}
