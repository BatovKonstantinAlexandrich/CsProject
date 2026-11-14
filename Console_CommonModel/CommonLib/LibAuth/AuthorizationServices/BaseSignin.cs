using Console_CommonModel.CommonLib.LibData.AbstractionModel;

/// <summary>
/// Абстракция к аутентификации в системе
/// </summary>
/// <typeparam name="TActiveObject">тип объекта</typeparam>
/// <typeparam name="TAuthenticationMember">модель учётных данныз</typeparam>
/// <typeparam name="TObjectInfo">персональные данные</typeparam>
public abstract class BaseSignin<TActiveObject, TAuthenticationMember, TObjectInfo> : 
    IBaseSignin<TActiveObject, TAuthenticationMember, TObjectInfo>

    where TObjectInfo : BaseEntity
    where TActiveObject : ActiveObject
    where TAuthenticationMember : PrivateData<TActiveObject>
{
    public readonly ITokenProvider _tokenProvider;
    public DbContextAuth<TActiveObject, TAuthenticationMember, TObjectInfo> _model;
    public static AuthorizationCollection<TActiveObject> _context = null;

    public BaseSignin(ITokenProvider tokenProvider)
    {
        if(_context is null)
        {
            _context = AppProviderService.GetInstance().Get<AuthorizationCollection<TActiveObject>>();
        }
        
        this._tokenProvider = tokenProvider;
    }

    public void SetDbContext(DbContextAuth<TActiveObject, TAuthenticationMember, TObjectInfo> db)
    {
        this._model = db;
    }
    public abstract bool Signout(string key);
    public abstract TActiveObject Verify();
    public abstract MethodResult<TActiveObject> Signin(TAuthenticationMember item);
    public abstract bool Validate(string key);
    public abstract void PutIntoSession(string key, object item);
    public abstract T GetFromSession<T>(string key);


}
