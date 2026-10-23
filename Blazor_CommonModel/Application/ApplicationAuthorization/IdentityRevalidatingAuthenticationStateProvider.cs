using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor_CommonModel.Application.Auth
{
    public class IdentityRevalidatingAuthenticationStateProvider<TAuthenticationService, TActiveObject, TAuthenticationMember, TObjectInfo>: AuthenticationStateProvider
        where TAuthenticationService: IBaseSignin<TActiveObject, TAuthenticationMember, TObjectInfo>
        where TActiveObject : ActiveObject
        where TAuthenticationMember : PrivateData<TActiveObject>
        where TObjectInfo : BaseEntity
    {
        private readonly IServiceProvider serviceProvider;

        public IdentityRevalidatingAuthenticationStateProvider( IServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
