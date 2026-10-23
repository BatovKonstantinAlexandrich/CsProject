using MaterialSkin.Controls;

using WinFormsApp.AppViews.ControlsFactory;
using WinFormsApp.AppViews.NodeModule;

namespace WinFormsApp.AppComponents.AppControlsModule
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeViewControl<T> : UserControl
    {


        private InputControlsFactory inputControlsFactory = new();

        private TypeNodeView root = new();


        public class TypeNodeView
        {
            public NodeViewBase<T> Model { get; set; }
            public MaterialCheckbox View { get; set; }
            public bool Checked { get; set; }
            public TypeNodeView Parent { get; set; }
            public List<TypeNodeView> Children { get; set; } = new();
        }

        public void OnNode(Action<TypeNodeView> todo)
        {
            OnNode(root, todo);
        }
        public void OnNode(TypeNodeView pnode, Action<TypeNodeView> todo)
        {
            todo(pnode);
            foreach (var child in pnode.Children)
            {
                OnNode(child, todo);
            }
        }



        public TreeViewControl()
        {
        }


        public void SetModel(NodeViewBase<T> model, Action<T> onselected)
        {
            root.Model = model;
            root.View = inputControlsFactory.CreateCheckboxControl(model.NodeName, val =>
            {
                root.Checked = val;
                onselected(root.Model.NodeItem);
            });

            model.GoFromChildren(node =>
            {

                var pnode = new TypeNodeView();
                pnode.Model = node;
                pnode.View = inputControlsFactory.CreateCheckboxControl(node.NodeName, val =>
                {
                    pnode.Checked = val;
                    onselected(node.NodeItem);
                });
                if (node.Parent is not null)
                {
                    var parentPath = node.Parent.GetPath();
                    FindByPath(parentPath).Children.Add(pnode);
                    pnode.Parent = FindByPath(parentPath);
                }

            });
            UpdateView(selected =>
            {
                onselected((T)selected);
            });
        }


        public void UpdateView(Action<object> onselected)
        {
            Controls.Clear();
            UpdateView(root, 0, onselected);
        }

        public void UpdateView(TypeNodeView node, int level, Action<object> onselected)
        {

            node.View.Dock = DockStyle.Top;
            var line = new Panel() { Dock = DockStyle.Top };
            var left = new Panel() { Dock = DockStyle.Left };

            node.View.Dock = DockStyle.Left;
            left.Width = level * 20;
            line.Height = 30;
            line.Controls.Add(node.View);
            line.Controls.Add(left);
            Controls.Add(line);
            foreach (var nodeView in node.Children)
            {
                UpdateView(nodeView, level + 1, onselected);
            }
            List<Control> ctrls = new();
            foreach (Control ctrl in line.Controls)
            {
                ctrls.Add(ctrl);
                Controls.Remove(ctrl);
            }
            ctrls.Reverse();
            foreach (var ctrl in ctrls)
            {
                Controls.Add(line);
            }
        }

        public TypeNodeView FindByPath(List<string> path)
        {
            if (path.Count() == 0)
                return null;
            var p = root;
            path.RemoveAt(0);
            foreach (var name in path)
            {
                p = p.Children.FirstOrDefault(pchildren => pchildren.Model.NodeName == name);
            }
            return p;
        }


    }
}
