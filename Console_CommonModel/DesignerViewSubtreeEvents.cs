 

namespace WinFormsApp.AppViews.AppRootModule
{
    /// <summary>
    /// Подписка на события мыши для всего поддерева представления (вложенные ScrollView, ToolsView и т.д.).
    /// </summary>
    internal static class DesignerViewSubtreeEvents
    {
        internal sealed class Subscription
        {
            private readonly List<AppViewBase> _wired = new();
            private readonly List<Action> _unsubscribers = new();

            public IReadOnlyList<AppViewBase> WiredViews => _wired;

            public void Wire(AppViewBase root,
                Action<object, MouseEventArgs> onMouseDown,
                Action<object, MouseEventArgs> onMouseMove,
                Action<object, MouseEventArgs> onMouseUp,
                Action<object, EventArgs> onMouseLeave = null)
            {
                if (root is null)
                    return;

                void Attach(AppViewBase node)
                {
                    if (node is null || _wired.Contains(node))
                        return;

                    _wired.Add(node);
                    node.OnMouseDown += onMouseDown;
                    node.OnMouseMove += onMouseMove;
                    node.OnMouseUp += onMouseUp;
                    if (onMouseLeave is not null)
                        node.OnMouseLeave += onMouseLeave;
                }

                void WireRecursive(AppViewBase node)
                {
                    if (node is null)
                        return;

                    Attach(node);

                    void OnChildrenChanged(object _, ListChangedEvent e)
                    {
                        if (e.Target.Action == "added" && e.Target.Item is AppViewBase child)
                            WireRecursive(child);
                    }

                    node.Controls.OnChanged += OnChildrenChanged;
                    _unsubscribers.Add(() => node.Controls.OnChanged -= OnChildrenChanged);

                    foreach (AppViewBase child in node.Controls)
                        WireRecursive(child);

                    if (node is MenuView menu)
                    {
                        foreach (MenuView item in menu.MenuItems)
                            WireRecursive(item);
                    }
                }

                WireRecursive(root);
            }

            public void Unwire(
                Action<object, MouseEventArgs> onMouseDown,
                Action<object, MouseEventArgs> onMouseMove,
                Action<object, MouseEventArgs> onMouseUp,
                Action<object, EventArgs> onMouseLeave = null)
            {
                foreach (var node in _wired)
                {
                    node.OnMouseDown -= onMouseDown;
                    node.OnMouseMove -= onMouseMove;
                    node.OnMouseUp -= onMouseUp;
                    if (onMouseLeave is not null)
                        node.OnMouseLeave -= onMouseLeave;
                }

                foreach (var unsub in _unsubscribers)
                    unsub();

                _wired.Clear();
                _unsubscribers.Clear();
            }
        }
    }
}
