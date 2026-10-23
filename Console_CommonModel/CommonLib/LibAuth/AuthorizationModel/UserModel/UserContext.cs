using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;




/// <summary>
/// Объект модели пользователя сеансов
/// </summary>
[Label("Пользователь")] 
[Icon("build")]
public class UserContext : ActiveObject
{
    //[NotMapped]
    [ServiceInjection()]    
    private DbContext dbContext;

    /// <summary>
    /// Бизнес-функции
    /// </summary>
    [NotMapped]
    public ConcurrentDictionary<string, Action> ActiveFunctions = new();




    public ClaimsPrincipal ToClaimsPrincipal()
    {
        var claims = new Claim[]
        {
            //new Claim(ClaimTypes.NameIdentifier, Account is not null? Account.Email: ""),
            //new Claim(ClaimTypes.Name, Account?.Email),
            //new Claim(ClaimTypes.Hash, Account?.Hash),
        };
        var roles = (BusinessResources is null ? new List<BusinessResource>() : BusinessResources).Select(r => new Claim(ClaimTypes.Role, r.Code));
        
        var identity = new ClaimsIdentity(claims.Concat(roles) );
        return new ClaimsPrincipal(identity);
    }

    public static UserContext FromClaimsPrincipal(ClaimsPrincipal principal) => new()
    {
        Account = new UserAccount()
        {
            Email = principal.FindFirst(ClaimTypes.Name)?.Value ?? "",
            Password = principal.FindFirst(ClaimTypes.Hash)?.Value ?? "",
        },


        BusinessResources = principal.FindAll(ClaimTypes.Role).Select(c => new BusinessResource()
        {
            Code = c.Value
        }).ToList()
    };


    [Label("Учетная запись")]
    [NotInput]
    public int AccountId { get; set; }

    [InputHidden("true")]
    [Label("Учетная запись")]
    public virtual UserAccount Account { get; set; } = new();

    [NotInput]    
    public int WalletId { get; set; }

    [NotInput]
    public UserWallet Wallet { get; set; } = new();


    //[Label("Роль")]
    //public int? BusinessResourceId { get; set; }

    //[InputHidden("true")]
    //[Label("Роль")]
    //public virtual BusinessResource Role { get; set; }


    [Label("Настройки")]
    [NotInput]
    public int SettingsId { get; set; }

    [Label("Настройки")]
    [NotInput]
    public virtual UserSettings Settings { get; set; } = new();


    [Label("Личная инф.")]
    [NotInput]
    public int PersonId { get; set; }

    [Label("Личная инф.")]
    [NotInput]
    public virtual UserPerson Person { get; set; } = new();


    [NotMapped]
    [NotInput]
    [Label("Группы")]
    public virtual List<UserGroup> Groups { get; set; } = new List<UserGroup>();


    [Label("Группы")]
    [NotMapped]
    public int UserGroupsId { get; set; }

    [Label("Группы")]
    [ManyToManyAttribute("Groups")]
    [InputHidden("true")]
    [NotInput]
    public virtual List<UserGroupBinding> UserGroups { get; set; } = new List<UserGroupBinding>();


    [Label("Кол-во посещений")]
    public int LoginCount { get; set; }





    [Label("Входящие сообщения")]

    [InverseProperty("ToUser")]
    [NotMapped]
    [NotInput]
    public virtual List<UserMessage> Inbox { get; set; } = new List<UserMessage>();



    [Label("Исходящие сообщения")]
    [InverseProperty("FromUser")]
    [NotMapped]
    [NotInput]
    public virtual List<UserMessage> Outbox { get; set; } = new List<UserMessage>();


    [Label("Теущий заказ")]
    [NotInput]
    public int? OrderId { get; set; }

 
    [Label("Выполняемые функции")]
    [NotInput]
    [ManyToMany(nameof(BusinessResources))]
    public virtual List<UsersResource> UsersResources { get; set; } = new List<UsersResource>();

    [Label("Выполняемые функции")]
    [NotMapped]
    public virtual List<BusinessResource> BusinessResources { get; set; } = new List<BusinessResource>();

    public UserContext(): base()
    {
        this.Info(this);
    }

    public string GetHomeUrl()
    {
        return "";// $"/{this.Role.Code}Face/{this.Role.Code}/{this.Role.Code}Home";
    }

    public UsersResource AddBusinessFunctions(BusinessResource webUserRole)
    {
        UsersResource r = null;
        UsersResources.Add(r=new UsersResource()
        {
            UserId = Id,
            RoleId = webUserRole.Id
        });
        return r;
    }
}



/// <summary>
/// Объект модели пользователя сеансов
/// </summary>
[Label("Пользователь")]
[Icon("build")]
public class UserBase<TAuthenticationTicket> : ActiveObject
    where TAuthenticationTicket : BaseEntity
{
    public ClaimsPrincipal ToClaimsPrincipal()
    {
        var claims = new Claim[]
        {
            //new Claim(ClaimTypes.NameIdentifier, Account is not null? Account.Email: ""),
            //new Claim(ClaimTypes.Name, Account?.Email),
            //new Claim(ClaimTypes.Hash, Account?.Hash),
        };
        var roles = (BusinessResources is null ? new List<BusinessResource>() : BusinessResources).Select(r => new Claim(ClaimTypes.Role, r.Code));

        var identity = new ClaimsIdentity(claims.Concat(roles));
        return new ClaimsPrincipal(identity);
    }

    public static UserContext FromClaimsPrincipal(ClaimsPrincipal principal) => new()
    {
        Account = new UserAccount()
        {
            Email = principal.FindFirst(ClaimTypes.Name)?.Value ?? "",
            Password = principal.FindFirst(ClaimTypes.Hash)?.Value ?? "",
        },


        BusinessResources = principal.FindAll(ClaimTypes.Role).Select(c => new BusinessResource()
        {
            Code = c.Value
        }).ToList()
    };


    [Label("Учетная запись")]
    public int AccountId { get; set; }

    [InputHidden("true")]
    [Label("Учетная запись")]
    public virtual TAuthenticationTicket Account { get; set; } = typeof(TAuthenticationTicket).New<TAuthenticationTicket>();

    [NotInput]
    public int WalletId { get; set; }

    [NotInput]
    public UserWallet Wallet { get; set; } = new();


    //[Label("Роль")]
    //public int? BusinessResourceId { get; set; }

    //[InputHidden("true")]
    //[Label("Роль")]
    //public virtual BusinessResource Role { get; set; }


    [Label("Настройки")]
    public int SettingsId { get; set; }

    [Label("Настройки")]
    public virtual UserSettings Settings { get; set; } = new();


    [Label("Личная инф.")]
    public int PersonId { get; set; }

    [Label("Личная инф.")]
    public virtual UserPerson Person { get; set; } = new();


    [NotMapped]
    [NotInput]
    [Label("Группы")]
    public virtual List<UserGroup> Groups { get; set; } = new List<UserGroup>();


    [Label("Группы")]
    [NotMapped]
    public int UserGroupsId { get; set; }

    [Label("Группы")]
    [ManyToManyAttribute("Groups")]
    [InputHidden("true")]
    [NotInput]
    public virtual List<UserGroupBinding> UserGroups { get; set; } = new List<UserGroupBinding>();


    [Label("Кол-во посещений")]
    public int LoginCount { get; set; }





    [Label("Входящие сообщения")]

    //[InverseProperty("ToUser")]
    [NotMapped]
    [NotInput]
    public virtual List<UserMessage> Inbox { get; set; } = new List<UserMessage>();



    [Label("Исходящие сообщения")]
    //[InverseProperty("FromUser")]
    [NotMapped]
    [NotInput]
    public virtual List<UserMessage> Outbox { get; set; } = new List<UserMessage>();


    [Label("Теущий заказ")]
    [NotInput]
    public int? OrderId { get; set; }


    [Label("Выполняемые функции")]
    [NotInput]
    [ManyToMany(nameof(BusinessResource))]
    public virtual List<UsersResource> UsersResources { get; set; } = new List<UsersResource>();

    [Label("Выполняемые функции")]
    [NotMapped]
    public virtual List<BusinessResource> BusinessResources { get; set; } = new List<BusinessResource>();
    [NotMapped]
    public virtual List<BusinessFunction> BusinessFunctions { get; set; } = new List<BusinessFunction>();


    
    public UserBase() : base()
    {
        this.Info(this);
    }

    public string GetHomeUrl()
    {
        return "";// $"/{this.Role.Code}Face/{this.Role.Code}/{this.Role.Code}Home";
    }

    public UsersResource AddBusinessFunctions(BusinessResource webUserRole)
    {
        UsersResource r = null;
        UsersResources.Add(r = new UsersResource()
        {
            UserId = Id,
            RoleId = webUserRole.Id
        });
        return r;
    }
}


