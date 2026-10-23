using System.Windows.Forms;

using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViewModules.TreeModule;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    public class TreeViewModelComponent : ComponentBase<TreeViewModel, TreeControl>
    {
        public TreeViewModelComponent([NotNullNotEmpty] TreeViewModel viewModel, [NotNullNotEmpty] TreeControl control)
            : base(viewModel, control)
        {
        }

       

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();

            ctrl.treeView1.Nodes.Clear();
            ctrl.treeView1.Nodes.Add(CreateTreeNode(view));

            ctrl.OnTreeNodeSelected += pnode =>
            {
                var model = Find(view, ctrl.GetPath(pnode));
                model.OnClick(model, new EventArgs());
            };

            ctrl.treeView1.Click += (s, e) =>
            {
                if (ctrl.treeView1.SelectedNode is null)
                    return;

                var model = Find(view, ctrl.GetPath(ctrl.treeView1.SelectedNode));
                model.OnClick(model, e);
            };

            ctrl.treeView1.ItemDrag += (s, e) =>
            {
                if (!view.Draggable)
                    return;

                if (e.Item is not TreeNodeControl node || string.IsNullOrEmpty(node.Text))
                    return;

                var text = view.DoDrag(node.Item);
                ctrl.treeView1.DoDragDrop(text, DragDropEffects.Copy);
            };
        }

        private static TreeViewModel Find(TreeViewModel root, List<int> path)
        {
            var list = new List<int>(path);
            if (list.Count > 0)
                list.RemoveAt(0);

            var node = root;
            foreach (var index in list)
                node = (TreeViewModel)node.Controls[index];

            return node;
        }

        private static string GetNodeText(TreeViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Title))
                return model.Title;

            if (model.Item is NamedObject named && !string.IsNullOrWhiteSpace(named.Name))
                return named.Name;

            return "Узел";
        }

        private TreeNode CreateTreeNode(TreeViewModel model)
        {
            var node = new TreeNodeControl<TreeViewModel>
            {
                Item = model,
                Text = GetNodeText(model)
            };

            componentsFactory.BindContextMenu(model, node);
            node.ContextMenuStrip = componentsFactory.CreateContextMenuStrip(model.ContextMenu);

            model.AddPropertyChangeListener(nameof(TreeViewModel.Title), _ => node.Text = GetNodeText(model));
            model.AddPropertyChangeListener(nameof(TreeViewModel.Item), _ => node.Text = GetNodeText(model));
            model.AddPropertyChangeListener(nameof(TreeViewModel.Visible), evt =>
            {
                var isVisible = (bool)evt.After;
                if (!isVisible)
                    node.Remove();
                else if (node.Parent is not null)
                    node.Parent.Nodes.Add(node);
            });
            model.AddPropertyChangeListener(nameof(TreeViewModel.Expanded), evt =>
            {
                if (evt.After.ToString().ToBool())
                    node.Expand();
            });

            if (model.Expanded)
                node.Expand();

            foreach (ViewItem child in model.Controls)
            {
                if (child is TreeViewModel childModel)
                    node.Nodes.Add(CreateTreeNode(childModel));
            }

            model.Controls.OnChanged += (s, e) =>
            {
                switch (e.Target.Action)
                {
                    case "added":
                        if (e.Target.Item is TreeViewModel childModel)
                            node.Nodes.Add(CreateTreeNode(childModel));
                        break;

                    case "removed":
                        node.Nodes.RemoveAt(e.Target.Index);
                        break;
                }
            };

            return node;
        }
    }
}
