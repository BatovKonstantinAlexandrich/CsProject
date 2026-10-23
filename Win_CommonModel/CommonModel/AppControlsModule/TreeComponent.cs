using WinFormsApp.AppViews.ApiServices;
using WinFormsApp.DesignerServices;

namespace WinFormsApp.AppComponents.AppControlsModule
{


    public class TreeComponent : ComponentBase<TreeControlView, TreeControl>
    {

        public TreeComponent([NotNullNotEmpty] TreeControlView ViewModel, [NotNullNotEmpty] TreeControl Control) : base(ViewModel, Control)
        {
        }

        public override List<DesignerBase> GetViewServices()
        {
            var result = base.GetViewServices();
            result.Add(new TreeControlViewDesignerService(GetViewModel()));
            return result;
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();
           
            Func<TreeNode, List<int>> getPath = pnode =>
            {
                var r = new List<int>();
                var newpath = new List<int>();
                Action<List<int>, TreeNodeCollection> todo = null;
                todo = (path, nodes) =>
                {
                    foreach (TreeNode item in nodes)
                    {
                        var p = new List<int>(path);
                        p.Add(nodes.IndexOf(item));
                        if (item == pnode)
                        {
                            r = p;
                        }
                        else
                        {
                            todo(p, item.Nodes);
                        }

                    }
                };
                todo(newpath, ctrl.treeView1.Nodes);
                return r;
            };
                                     
            ctrl.treeView1.Nodes.Add(createTreeNode(view.root));
            ctrl.OnTreeNodeSelected += (pnode) =>
            {
                view.Find(getPath(pnode)).OnClick(pnode, new());
            };
            ctrl.treeView1.Click += (s, e) => {
                if (ctrl.treeView1.SelectedNode is not null)
                {
                    var path = getPath(ctrl.treeView1.SelectedNode);
                    var viewNode = view.Find(path);
                    viewNode.OnClick(viewNode, e);
                } 
                //view.OnClick(ctrl.treeView1.SelectedNode, e);
            };

            ctrl.treeView1.ItemDrag += (s, e) =>
            {
                if (view.Draggable)
                {
                    if (e.Item is not TreeNodeControl node || string.IsNullOrEmpty(node.Text))
                        return;
                   
                    var pnode = node.Item;
                    var text = view.DoDrag(pnode);
                    if (text is null || (text is string dragText && string.IsNullOrWhiteSpace(dragText)))
                        return;
                    this.Info($"drag text: {text}");
                    ctrl.treeView1.DoDragDrop(text, DragDropEffects.Copy);
                }


            };

            //0ctrl.AllowDrop = true;
            //ctrl.treeView1.AllowDrop = true;

        }

        private string GetNodePath(TreeNode node)
        {
            List<string> path = new List<string>();
            while (node != null)
            {
                path.Insert(0, node.Text);
                node = node.Parent;
            }
            return string.Join("\\", path);
        }

        private TreeNode createTreeNode(TreeViewNodeModel proot)
        {
            TreeNodeControl node = new();
            node.Item = proot;
            proot.TreeNode = node;



            TreeViewNodeModel pnode = null;
            proot.AddPropertyChangeListener(nameof(proot.Visible), evt => {
                this.Info(proot.Title+" " + proot.GetPath().ToJson());
                var isVisible = (bool)evt.After;
                if(!isVisible)
                {
                    if (proot.ParentNode is not null)
                    {                        
                        proot.ParentNode.TreeNode.Nodes.Remove(proot.TreeNode);
                    }
                    
                    //node = GetControl().FindByPath(proot.GetPath());
                    /*pnode = proot.ParentNode;
                    if(pnode is not null)
                        pnode.Children.Remove(proot);*/
                }
                else
                {
                    if (proot.ParentNode is not null)
                    {
                        proot.ParentNode.TreeNode.Nodes.Add(proot.TreeNode);
                    }
                    /*if (pnode is not null && proot.ParentNode is null)
                    {
                        pnode.Children.Add(proot);
                    } */
                }
            });
            var factory = new ComponentsFactory();
            factory.BindContextMenu(proot, node);
            node.ContextMenuStrip = factory.CreateContextMenuStrip(proot.ContextMenu/*, command =>
            {
                //var clickedMenu = proot.ContextMenu.Find(command.Split(".").ToList());
                //clickedMenu.OnClick(this, new EventArgs());
            }*/);
                        
            node.Expand();
            node.Text = proot.Title;
            proot.AddPropertyChangeListener("Expanded", evt =>
            {
                node.Expand();
            });
            foreach (var p in proot.Children)
            {
                var childNode = createTreeNode(p);
                 
                node.Nodes.Add(childNode);
            }
            proot.Children.OnChanged += (s, e) =>
            {

                switch (e.Target.Action)
                {
                    case "added":
                        var pitem = createTreeNode((TreeViewNodeModel)e.Target.Item);                        
                        node.Nodes.Add(pitem);
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
