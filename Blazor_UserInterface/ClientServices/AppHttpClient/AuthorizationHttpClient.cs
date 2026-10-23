using ApplicationDb.Entities;
using Blazor.Hospital.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppHttpClient
{
    public class AuthorizationHttpClient: IAuthorizationAPI
    {
        private readonly ITokenStorage _storage;
        private readonly ControllerHttpClient http;
        public AuthorizationHttpClient(ITokenStorage storage, string ApplicationURL) 
        {
            _storage = storage;
            http = new ControllerHttpClient(storage,$"{ApplicationURL}api/AuthorizationApi");
        }
        public AuthorizationHttpClient(ITokenStorage storage, string ApplicationURL, bool Logging): this(storage,ApplicationURL)
        {
            this.http._logging = Logging;
        }

        public async Task<bool> HasUserWithEmail(string Email)
        {
            
            return await http.Get<bool>("HasUserWithEmail",
  
                new Dictionary<string, string>() {
                { "Email", Email }
            });
        }

        public async Task<bool> HasUserWithTel(string Tel)
        {

            return await http.Get<bool>("HasUserWithTel",
         
            new Dictionary<string, string>() {
                { "Tel", Tel }
            });
        }

   

        public async Task<bool> IsSignin()
        {
            return await http.Get<bool>("IsSignin");
        }

        public async Task<string> Signin(string Email, string Password)
        {
            return await http.Get<string>("Signin",
                 
                new Dictionary<string, string>() {
                    { "Email", Email },
                    { "Password", Password }
                }
            );
        }

     
        public async Task<bool> Signout()
        {
            return await http.Get<bool>("Signout");
        }

        public async Task<bool> Signup(
                string Email, string Password, string Confirmation, 
                string SurName, string FirstName, string LastName, string Birthday, string Tel)
        {
            return await http.Post<bool>("Signup",
             new Dictionary<string, string>() { },
            new Dictionary<string, string>() {
                { "Email", Email },
                { "Password", Password },                
                { "SurName", SurName },
                { "FirstName", FirstName },
                { "LastName", LastName },
                { "Birthday", Birthday },
                { "Tel", Tel }
            });
        }

        public async Task<User> Verify()
        {
            return await http.Get<User>("Verify");
        }
    }
}
