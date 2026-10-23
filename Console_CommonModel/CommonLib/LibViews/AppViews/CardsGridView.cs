using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.CollectionModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews
{


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class CardsGridView<TEntity> : MultiViewConstructor<TEntity>
        where TEntity : BaseEntity
    {


        [Label("Properties")]
        public ChangingList<string> Properties { get; }

        public CardsGridView() : base()
        {

            CreateSignleView = data => CreateSingleView(data);
            Properties = new ChangingList<string>(
                typeof(TEntity).GetOwnPropertyNames()
            );
        }

        public SingleView<TEntity> CreateSingleView(TEntity data)
        {
            var card = new Card<TEntity>(data);
            card.BorderWidth = 1;
            card.Margin = 10;
            card.MinWidth = card.MinHeight = 320;
            foreach (var property in Properties)
            {
                card.LayoutTop(card.CreateTextCenter($"{data.GetValue(property)}"));
            }
            return card;
        }

        internal void AddData(TEntity userAccount)
        {
            Entities.Add(userAccount);
        }


        //public Control ToControl()
        //    => Layout.ToControl();

        public void AddRange(IEnumerable<TEntity> enumerable)
        {
            foreach (TEntity p in enumerable)
            {
                AddData(p);
            }
        }
    }
}