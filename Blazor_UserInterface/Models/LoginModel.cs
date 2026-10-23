using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RootForms.Models
{
    public class LoginModel: MyValidatableObject
    {
        [NotNullNotEmpty("Укажите адрес электронной почты")]
        [InputEmail("Формат не соответвует требованиям к адресам электронной почты")]
        public string Email { get; set; }

        [NotNullNotEmpty("Укажите адрес электронной почты")]
        [InputPassword( )]
        public string Password { get; set; }
    }
}
