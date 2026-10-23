using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO
{

    public class ChangingDataList<TEntity> : ChangingList<TEntity>
        where TEntity : BaseEntity, IChangeSupport
    {
        private readonly IEntityFasade<TEntity> _entityFasade;
        private readonly Dictionary<int, Action<PropertyChangedMessage>> _listeners = new();

        public virtual ChangingList<TEntity> visibleEntities { get; set; }
        protected bool enabledChanging = false;
        protected Predicate<TEntity> filter = item => true;

        public ChangingDataList(IEntityFasade<TEntity> entityFasade)
        {
            OnChanged += OnDataChanged;
            visibleEntities = new();
            var p = this;

            var dataset = entityFasade.GetAll();
            lock (p)
            {
                enabledChanging = false;
                AddRange(dataset);
                visibleEntities.AddRange(dataset.Take(10));
                enabledChanging = true;
            }

            visibleEntities.OnChanged += OnVisibleDataChanged;
            _entityFasade = entityFasade;
            enabledChanging = true;
        }

        private void OnDataChanged(object sender, IChangingListEvent e)
        {
            try
            {
                var entity = (TEntity)e.Target.Item;
                switch (e.Target.Action)
                {
                    case "added":
                        entity.EnsureIsValide();
                        if (enabledChanging)
                        {
                            _entityFasade.Create(entity);
                        }
                        if (filter(entity))
                        {
                            visibleEntities.Add(entity);
                        }
                        break;
                    case "removed":
                        if (enabledChanging)
                        {
                            _entityFasade.Remove(entity.Id);
                            if (visibleEntities.Contains(entity))
                            {
                                visibleEntities.Remove(entity);
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                this.Error(ex);
                throw new Exception("Исключение при обновлении данных", ex);
            }
        }

        public void OnVisibleDataChanged(object sender, IChangingListEvent e)
        {
            try
            {
                IChangeSupport changeSupport = (IChangeSupport)e.Target.Item;
                TEntity entity = (TEntity)e.Target.Item;
                BaseEntity be = (BaseEntity)e.Target.Item;
                switch (e.Target.Action)
                {
                    case "added":
                        SubscribeEvents(entity, changeSupport);
                        break;
                    case "removed":
                        UnsubscribeEvents(entity, changeSupport);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Error($"Исключение при добавлении сущности к отоброаемым данным: {ex.Message}");
                throw new Exception($"Исключение при добавлении сущности к отоброаемым данным: {ex.Message}", ex);
            }
        }

        private void UnsubscribeEvents(TEntity entity, IChangeSupport changeSupport)
        {
            changeSupport.RemovePropertyChangeListener(_listeners[entity.Id]);
        }

        private void SubscribeEvents(TEntity entity, IChangeSupport changeSupport)
        {
            _listeners[entity.Id] = evt =>
            {
                _entityFasade.Update(entity);
            };
            changeSupport.AddPropertyChangeListener(_listeners[entity.Id]);
        }

    }
}
