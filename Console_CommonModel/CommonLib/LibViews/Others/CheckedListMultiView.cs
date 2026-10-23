using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.CollectionModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.Others
{
    [Label("Список с выборов элементов")]
    public class CheckedListMultiView<T> : MultiViewConstructor<T> where T : BaseEntity
    {
        [Label("Entities")]
        public override ChangingList<T> Entities { get => base.Entities; set => base.Entities = value; }

        public CheckedListMultiView()
        {
            CreateSignleView = item => new CheckedListViewItem<T>(item)
            {
                dataModel = item
            };
        }


        /// <summary>
        /// Элемент с возможностью выбора
        /// </summary>
        [Label("Элемент списока выбора")]
        public class CheckedListViewItem<TT> : SingleView<TT> where TT : BaseEntity
        {
            public CheckedListViewItem() : base() { }

            public CheckedListViewItem(TT dataModel) : base(dataModel)
            {

            }

            [Label("Checked")]
            public override bool Checked { get => base.Checked; set => base.Checked = value; }

            protected override void ApplyViewModel(TT after)
            {

            }
        }



        public override void UpdateView(List<string> properties, ChangingList<T> entities)
        {
        }
    }
}
