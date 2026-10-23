using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class AuthState
{
    public bool Signed { get; set; }
}
public class AuthStateService
{

    public AuthState State { get; set; } = new AuthState();
    public Action<AuthState> StateChanged { get; set; } = (state) => {
        Console.WriteLine(state.Signed);
    };

}