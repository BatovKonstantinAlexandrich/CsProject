using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor_CommonModel.Application.UserAgent
{
    public class TokenStorage : ITokenStorage
    {
        //public static int Counter = StartupApplication.Add((services) => {
        //    Console.WriteLine($"\t {nameof(ITokenStorage)}");
        //    services.AddScoped<ITokenStorage, TokenStorage>();
        //});

        private readonly ILocalStorage _local;

        public TokenStorage(ILocalStorage local)
        {
            _local = local;
        }

        public async Task<string> Get()
        {
            return await _local.GetItem("token");
        }

        public async Task Set(string token)
        {
            await _local.SetItem("token", token);
        }
    }
}
