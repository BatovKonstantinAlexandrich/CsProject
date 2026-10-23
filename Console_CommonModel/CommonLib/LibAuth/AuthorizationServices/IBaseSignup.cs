
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

public interface IBaseSignup<TActiveObject, TAuthenticationMember, TObjectInfo>
    where TActiveObject : ActiveObject
    where TObjectInfo : BaseEntity
{
    bool Compare(TAuthenticationMember stored, TAuthenticationMember input);
    TActiveObject GetBy(TAuthenticationMember item);
    bool HasWith(TAuthenticationMember item);
    MethodResult<TActiveObject> Signout(TAuthenticationMember item);
    MethodResult<TActiveObject> Signup(TAuthenticationMember item, TObjectInfo info);
}