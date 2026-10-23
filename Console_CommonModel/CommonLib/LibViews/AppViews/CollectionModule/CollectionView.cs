using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.CollectionModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

[Label("Представление коллекции")]
public abstract class CollectionView<TEntity> : MultiView<TEntity> where TEntity : BaseEntity
{
    public override ChangingList<TEntity> Entities { get; set; }

}






namespace Console_CommonModel.CommonLib.LibViews.AppViews.CollectionModule
{



    [Label("Карточка протокола")]
    public class MessageProtocolCard : Card<MessageProtocol>
    {
        public MessageProtocolCard() : base() { }
        public MessageProtocolCard(MessageProtocol item) : base(item)
        {
        }

        protected override void ApplyViewModel(MessageProtocol after)
        {
            Controls.Clear();
            Controls.Add(CreateText(after.Name));
        }
    }
}


public class CollectionViewTest : TestingElement
{
    public override void OnTest()
    {
        /*this.Info(typeof(MessageProtocolCollectionView).GetExtendedTypes().Select(t => t.GetTypeName()).ToJsonOnScreen());
        this.Info(typeof(MessageProtocolCollectionView).IsGenericFrom(typeof(MultiView<>)));
        var view = new MessageProtocolCollectionView();
        var ctrl = view.ToControl();
        using (var db = new DeliveryDbContext())
        {
            foreach(var entity in db.MessageProtocols.ToList())
            {
                view.Entities.Add(entity);
            }
            
        }
        
        ctrl.RunAtForm();*/
    }
}
