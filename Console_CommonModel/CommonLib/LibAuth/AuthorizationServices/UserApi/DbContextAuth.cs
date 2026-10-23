using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;

public class DbContextAuth<TActiveObject, TAuthMember, TObjectInfo> : DbContextService
    where TAuthMember : PrivateData<TActiveObject>
    where TActiveObject : ActiveObject
    where TObjectInfo : BaseEntity
{
    public virtual DbSet<UserAccount> UserAccounts { get; set; }
    public virtual DbSet<UserContext> UserContexts { get; set; }
    public virtual DbSet<UserWallet> UserWallets { get; set; }
    public virtual DbSet<TransferTransaction> TransferTransactions { get; set; }
    public virtual DbSet<UsersResource> UsersResources { get; set; }
    public virtual DbSet<BusinessResource> BusinessResources { get; set; }
    public virtual DbSet<UserLogin> UserLogins { get; set; }
    public virtual DbSet<UserGroupBinding> UserGroupsUserGroup { get; set; }
    public virtual DbSet<UserGroupMessage> UserGroupMessages { get; set; }
    public virtual DbSet<UserGroup> UserGroups { get; set; }
    public virtual DbSet<UserMessage> UserMessages { get; set; }
    public virtual DbSet<UserMessageFile> UserMessageFiles { get; set; }
    public virtual DbSet<UserPerson> UserPersons { get; set; }
    public virtual DbSet<UserSettings> UserSettings { get; set; }

    public DbContextAuth() { }
    public DbContextAuth(string connectionString) {
        this.connectionString = connectionString;
    }
    public DbContextAuth(DbContextOptions<DbContextBase> options): base(options)
    {
        this.connectionString = connectionString;
    }

}
