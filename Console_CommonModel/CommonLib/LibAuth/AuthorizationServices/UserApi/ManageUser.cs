//using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

//[Route("/api/manage-user/[action]")]
public class ManageUser<TUserContext> : BaseManager<TUserContext, UserContext>
    where TUserContext : DbContextUser
{
    public ManageUser(TUserContext context) : base(context)
    {
    }
 
}