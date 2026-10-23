using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API;
using Console_CommonModel.CommonLib.LibViews.AppViews.FlexModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO
{
    [Label("Позиционирование сущностей")]
    public class FlexViewList<TEntity> : FlexView
        where TEntity : BaseEntity, IChangeSupport
    {
        public SearchDataList<TEntity> entities;
        public Func<TEntity, ViewItem> factory;

        public FlexViewList()
        {
            factory = _ => new FlexView();
        }

        public FlexViewList(IApi api)
        {
            entities = new(api.GetFasade<TEntity>());
            entities.visibleEntities.OnChanged += OnDataViewChanged;
            factory = data =>
            {
                var view = new FlexView();
                foreach (var property in typeof(TEntity).GetOwnPropertyNames())
                {
                    view.Add(new ButtonView() { Text = property });
                }
                view.Add(new ButtonView() { Text = "OK" });
                return view;
            };
        }

        private void OnDataViewChanged(object arg1, IChangingListEvent @event)
        {
            var entity = (TEntity)@event.Target.Item;
            switch (@event.Target.Action)
            {
                case "added": Add(factory(entity)); break;
            }

        }
    }
}
