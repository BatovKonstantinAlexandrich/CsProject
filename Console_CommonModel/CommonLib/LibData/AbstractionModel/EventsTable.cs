using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System;
/// <summary>
/// Справочник ( дополнительная информация, не обрабатывается приложением )
/// </summary>
public class EventsTable : BaseEntity
{
    public DateTime Created;

}

[Label("Абстрация для хранения событий")]
public class EventsTable<T>: EventsTable where T: BaseEntity
{
    [NotInput]
    public int? DataId { get; set; }

    [NotInput]
    public T Data { get; set; }

    public EventsTable() : base() { }
    public EventsTable(T login) : base( )
    {
        //this.Name = this.GetTypeName();
        if(login is not null && login.Id != 0)
            this.DataId = login.Id;
    }
}
