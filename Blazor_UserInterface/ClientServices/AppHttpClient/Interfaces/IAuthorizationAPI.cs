using ApplicationDb.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppHttpClient
{
    public interface IAuthorizationAPI
    {
        Task<bool> HasUserWithEmail(string Email);
        Task<bool> HasUserWithTel(string Tel);
        Task<bool> IsSignin();
        Task<string> Signin(string Email, string Password);
        Task<bool> Signout();
        Task<bool> Signup(string Email, string Password, string Confirmation, string SurName, string FirstName, string LastName, string Birthday, string Tel);
        Task<User> Verify(); 
    }

}
