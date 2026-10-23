using WinFormsApp.AppViews.CollectionModule;

using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;

namespace WinFormsApp.AppComponents.AppControls
{
    [Label("Представление коллекции")]

    public abstract class MultiComponent<TData, TView, TControl> : ComponentBase<TView, TControl>
        where TData : BaseEntity
        where TView : MultiView<TData>
        where TControl : ControlBase
    {
        public ChangingList<TData> dataset { get; set; }


        public MultiComponent([NotNullNotEmpty] TView ViewModel, [NotNullNotEmpty] ChangingList<TData> dataset, [NotNullNotEmpty] TControl Control) : base(ViewModel, Control)
        {
            Init();
            this.dataset = dataset;
        }

        public void Init()
        {
            dataset.OnChanged += OnDataChanged;
        }

        private void OnDataChanged(object sender, IChangingListEvent evt)
        {
            switch (evt.Target.Action)
            {
                case "added":
                    break;
                case "removed":

                    break;
            }

        }

    }



}
