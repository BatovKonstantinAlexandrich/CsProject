
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

public interface IBaseSignin<TActiveObject, TAuthenticationMember, TObjectInfo>
    where TActiveObject : ActiveObject
    where TAuthenticationMember : PrivateData<TActiveObject>
    where TObjectInfo : BaseEntity
{
    T GetFromSession<T>(string key);
    void PutIntoSession(string key, object item);
    void SetDbContext(DbContextAuth<TActiveObject, TAuthenticationMember, TObjectInfo> db);
    MethodResult<TActiveObject> Signin(TAuthenticationMember item);
    bool Signout(string key);
    bool Validate(string key);
}