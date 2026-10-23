using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 
public class LoginRequest: MyValidatableObject
{
    [Required(ErrorMessage = "Необходимо ввести имя пользователя")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Необходимо ввести пароль")]
    public string Password { get; set; }
    public bool RememberMe { get; set; }

    public LoginRequest(string userName, string password, bool rememberMe=false)
    {
        UserName = userName;
        Password = password;
        RememberMe = rememberMe;
    }
}
 