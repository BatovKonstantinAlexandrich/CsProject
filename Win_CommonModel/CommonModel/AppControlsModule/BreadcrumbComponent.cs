using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViewModules.BreadcrumbModule;

using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    /// <summary>
    /// Связывает <see cref="BreadcrumbView"/> с <see cref="BreadcrumbControl"/>.
    /// </summary>
    public class BreadcrumbComponent : ComponentBase<BreadcrumbView, BreadcrumbControl>
    {
        private bool _subscribedItemsList;
        private readonly HashSet<BreadcrumbItemView> _subscribedItems = new();

        public BreadcrumbComponent(BreadcrumbView viewModel, BreadcrumbControl control) : base(viewModel, control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();

            ctrl.ItemClick += (_, args) =>
            {
                if (args.Index < 0 || args.Index >= view.Items.Count)
                    return;

                var item = view.Items[args.Index];
                if (item.IsActive)
                    return;

                view.OnItemClick?.Invoke(item, args.Index);
            };

            view.AddPropertyChangeListener(nameof(BreadcrumbView.Separator), _ => Rebuild());
            Rebuild();

            if (!_subscribedItemsList)
            {
                view.Items.OnChanged += OnItemsChanged;
                _subscribedItemsList = true;
            }
        }

        private void OnItemsChanged(object sender, IChangingListEvent evt)
        {
            var view = GetViewModel();

            switch (evt.Target.Action)
            {
                case "added":
                    if (evt.Target.Item is BreadcrumbItemView added)
                        SubscribeItem(added);
                    Rebuild();
                    break;
                case "removed":
                    Rebuild();
                    break;
            }
        }

        private void SubscribeItem(BreadcrumbItemView item)
        {
            if (item is null || !_subscribedItems.Add(item))
                return;

            void onChanged(PropertyChangedMessage _) => Rebuild();

            item.AddPropertyChangeListener(nameof(BreadcrumbItemView.Label), onChanged);
            item.AddPropertyChangeListener(nameof(BreadcrumbItemView.Href), onChanged);
            item.AddPropertyChangeListener(nameof(BreadcrumbItemView.IsActive), onChanged);
        }

        private void Rebuild()
        {
            var view = GetViewModel();
            var ctrl = GetControl();

            foreach (var item in view.Items)
                SubscribeItem(item);

            var segments = view.Items
                .Select(item => new BreadcrumbSegment
                {
                    Label = item.Label ?? string.Empty,
                    Href = item.Href ?? string.Empty,
                    IsActive = item.IsActive
                })
                .ToList();

            view.RunOnUiThread(() => ctrl.Render(segments, view.Separator));
        }
    }
}
