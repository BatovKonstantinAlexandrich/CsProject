using Console_CommonModel.CommonLib.LibData.AbstractionModel;

/// <summary>
/// Таблица для регистрации операций
/// </summary>
/// <typeparam name="TRequestModel">входные данные для операции</typeparam>
/// <typeparam name="ActiveObjectType">выходные данные для операции</typeparam>
public class ActionTable<ActiveObjectType, TRequestModel, TResultModel> : EventsTable<TRequestModel>
    where ActiveObjectType : ActiveObject
    where TRequestModel : BaseEntity
    where TResultModel : BaseEntity
{
    public ActionTable() : base(null)
    {
    }

    public ActionTable(TRequestModel data, ActiveObjectType login=null) : base(data)
    {
    }
}
public class ActionTable<ActiveObjectType, TRequestModel> : EventsTable<TRequestModel>
    where ActiveObjectType : ActiveObject
    where TRequestModel : BaseEntity
{
    public ActionTable() : base(null)
    {
    }

    public ActionTable(TRequestModel data, ActiveObjectType login = null) : base(data)
    {
    }
}