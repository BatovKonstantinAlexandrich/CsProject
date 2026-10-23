using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.FlexModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.CollectionModule
{

    [Label("Единичное представление сущности")]
    public abstract class SingleView<TEntity> : AppViewBase
        where TEntity : BaseEntity
    {
        [Label("data Model")]
        public TEntity dataModel { get => Get<TEntity>(nameof(dataModel)); set => Set(nameof(dataModel), value); }
        [Label("flex View")]
        public FlexView flexView { get; set; }

        protected SingleView(TEntity dataModel)
        {
            this.dataModel = dataModel;
            this.dataModel.EnsureIsValide();
        }
        protected SingleView()
        {
            dataModel = typeof(TEntity).New<TEntity>();
        }


        private void OnPropertyChanged(PropertyChangedMessage message)
        {
            if (dataModel is IChangeSupport)
            {
                ((IChangeSupport)dataModel).ApplyPropertyChangedEvent(message);
            }
            UpdateViewModel(dataModel);
        }

        protected void OnDataModelChanged(PropertyChangedMessage message)
        {
            UpdateViewModel(message.After is null ? null : (TEntity)message.After);
        }

        protected abstract void ApplyViewModel(TEntity after);
        protected void UpdateViewModel(TEntity after)
        {
            dataModel = dataModel;
            AddPropertyChangeListener(nameof(dataModel), OnDataModelChanged);
            if (dataModel is IChangeSupport)
            {
                ((IChangeSupport)dataModel).AddPropertyChangeListener(OnPropertyChanged);
            }

            DoDrag += view => new
            {
                type = view.GetTypeName(),
                data = view
            }.ToJson();
            DoDrop += (text, x, y) =>
            {
                if (text is not null)
                {
                    /*var message = text.FromJson<Dictionary<string, object>>();
                    var type = message["type"].ToString();
                    var dataType = type.ToType();
                    var data = message["data"].ToString();
                    var target = data.FromJson(dataType);
                    if (target is ViewItem)
                    {
                        Append((ViewItem)target);
                    }*/
                }
            };
            LayoutFill(flexView = new FlexView());
            flexView.Add(new ButtonView() { Text = "dataModel" });
        }

        public void SetDataModel(TEntity entity)
        {
            dataModel = entity;
        }
    }
}
