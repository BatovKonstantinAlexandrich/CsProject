using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.LayoutVIews
{

    /// <summary>
    /// Сервис для управления представлениями размещенных на контейнере
    /// </summary>
    public abstract class LayoutViewService<TContainerView, TViewBase> : IControlService<TContainerView>

        where TContainerView : AppViewBase
        where TViewBase : AppViewBase
    {
        public TContainerView Control { get; set; }
        private Dictionary<TViewBase, Action> rollback = new();
        private Dictionary<ViewNode, Action> rollbackBase = new();

        public LayoutViewService() { }


        public void SetControl(TContainerView view)
        {
            BindLayoutEvents(Control = view);
        }


        /// <summary>
        /// Подключает обработчики событий на позиционирование в контейнере для активации
        /// и на отключение при удалении позиции.
        /// Работает рекурсия
        /// </summary>
        private Action BindLayoutEvents(ViewNode view)
        {
            Action<ViewNode> onappended = pnode =>
            {
                var deactivate = BindLayoutEvents(pnode);
                rollbackBase[pnode] = deactivate;
                if (pnode is TViewBase vb)
                {
                    rollback[vb] = Activate(vb);
                }
            };
            view.OnAppendedChild += onappended;
            Action<ViewNode> onremoved = pnode =>
            {
                if(rollbackBase.ContainsKey(pnode))
                    rollbackBase[pnode]();
                pnode.DoBroadcastFromHierElements<ViewNode>(pChildNode =>
                {
                    if (!rollbackBase.ContainsKey(pChildNode))
                    {
                        this.Warn("Дочернее представление не было активировано");
                    }
                    else
                    {
                        rollbackBase[pChildNode]();
                        rollbackBase.Remove(pChildNode);
                    }
                    if (pChildNode is TViewBase vb)
                    {
                        if (!rollback.ContainsKey(vb))
                        {
                            this.Warn("Дочернее представление не было активировано");
                        }
                        else
                        {
                            rollback[vb]();
                            rollback.Remove(vb);
                        }
                    }
                });
                if (!rollbackBase.ContainsKey(pnode))
                {
                    this.Warn("Дочернее представление не было активировано");
                }
                else
                {
                    rollbackBase[pnode]();
                    rollbackBase.Remove(pnode);
                }
                if (pnode is TViewBase vb)
                {
                    if (!rollback.ContainsKey(vb))
                    {
                        this.Warn("Дочернее представление не было активировано");
                    }
                    else
                    {
                        rollback[vb]();
                        rollback.Remove(vb);
                    }
                }
            };
            view.OnRemovedChild += onremoved;
            return () =>
            {
                view.OnRemovedChild -= onremoved;
                view.OnAppendedChild -= onappended;
            };
        }


        public abstract Action Activate(TViewBase viewBase);
    }

}
