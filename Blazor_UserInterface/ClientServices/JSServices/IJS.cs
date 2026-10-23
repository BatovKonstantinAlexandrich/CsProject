using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Hospital.Client.Services.JSServices
{
    public interface IJS
    {

        public Task<T> Eval<T>(string js);
        public Task<T> EvalAsync<T>(string js);
    }
}
