using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.AppViews.CollectionModule;

namespace WinFormsApp.AppComponents.AppControls
{
    public abstract class SingleComponent<TData, TView, TControl> : ComponentBase<TView, TControl>
        where TData : BaseEntity
        where TView : SingleView<TData>
        where TControl : ControlBase
    {
        public TData data { get => Get<TData>(nameof(data)); set => Set(nameof(data), value); }

        public SingleComponent([NotNullNotEmpty] TView ViewModel, [NotNullNotEmpty] TControl Control) : base(ViewModel, Control)
        {
            Init();
        }

        public void Init()
        {
            AddPropertyChangeListener(nameof(data), OnDataChanged);
        }

        private void OnDataChanged(PropertyChangedMessage evt)
        {
            GetViewModel().SetDataModel(evt.After is null ? null : (TData)evt.After);
        }

        public abstract void BindView();
        public abstract void BindControl();

        public override void Bind()
        {
            BindView();
            BindControl();
        }
    }



}
