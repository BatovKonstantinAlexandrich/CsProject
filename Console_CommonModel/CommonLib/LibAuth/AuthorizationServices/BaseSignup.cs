using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using System.Collections.Generic;


/// <summary>
/// Класс абстрация для регистрации/удаления учетных записей активных объектов.
/// </summary>
/// <typeparam name="TActiveObject">Учетные данные, необходимо для аутентификации</typeparam>
/// <typeparam name="TAuthenticationMember">Персональные данные</typeparam>
/// <typeparam name="TObjectInfo">Персональные данные</typeparam>
public abstract class BaseSignup<TDbContext, TActiveObject, TAuthenticationMember, TObjectInfo> : 
    IBaseSignup<TActiveObject, TAuthenticationMember, TObjectInfo> 
        where TActiveObject : ActiveObject
        where TObjectInfo : BaseEntity
        where TDbContext : DbContextBase
{

    public abstract MethodResult<TActiveObject> Signout(TAuthenticationMember item);
    public abstract MethodResult<TActiveObject> Signup(TAuthenticationMember item, TObjectInfo info);
    public abstract bool HasWith(TAuthenticationMember item);
    public abstract bool Compare(TAuthenticationMember stored, TAuthenticationMember input);
    public abstract TActiveObject GetBy(TAuthenticationMember item);
}
