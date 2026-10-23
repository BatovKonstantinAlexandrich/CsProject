using Blazor_CommonModel.Shared;

using Microsoft.AspNetCore.Authorization;

namespace Blazor_CommonModel.Components.Pages
{

    [Authorize()]
    public class BaseAuthorizationComponent: BaseComponent
    {
    }
}
