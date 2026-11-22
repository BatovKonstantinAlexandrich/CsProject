using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

 
public interface ITokenStorage
{
    public Task<string> Get();
    public Task Set(string token);
}
 
