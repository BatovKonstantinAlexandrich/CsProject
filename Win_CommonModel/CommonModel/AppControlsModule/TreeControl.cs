using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp.AppViewModules.TreeModule
{
    public partial class TreeControl : ControlBase
    {
        public Action<TreeNode> OnTreeNodeSelected { get; set; } = (p) => { };
        public Action<List<int>> OnNodeSelected { get; set; } = (p) => { };

        public TreeControl()
        {
            this.InitializeComponent();
            //this.BindMouseEvents(this.treeView1);
            this.SetNodes(new List<TreeNode>());
            this.treeView1.AfterSelect += (s, e) => {
                try
                {
                    var path = this.GetPath(e.Node);
                    OnNodeSelected(path);
                }
                catch(Exception ex)
                {
                    this.Error($"Исключение при выборе узла дерева");
                }
            };
        }


          

        public System.Windows.Forms.TreeView GetTreeView() => this.treeView1;
        public void SetNodes(IEnumerable<TreeNode> children)
        {
            this.treeView1.Nodes.Clear();
            this.treeView1.Nodes.AddRange(children.ToArray());
        }

         
        public List<int> GetPath(TreeNode pnode)
        {
            var r = new List<int>();
            var newpath = new List<int>();
            Action<List<int>, TreeNodeCollection> todo = null;
            todo = (path, nodes) => {
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
            todo(newpath, this.treeView1.Nodes);
            return r;
        }

 

        public void Bind<TEntity>(Action<TEntity> onclicked = null) where TEntity : HierTable<TEntity>
        {
            var service = new TreeService();
            var dbset = typeof(TEntity).GetEntityFasade<TEntity>().GetDbSet();
            var view = service.CreateTreeView<TEntity>(dbset, (TEntity p) => {
                if (onclicked is not null) onclicked(p);
            });
            var component = new TreeComponent(view, this);
        }

        public TreeNodeControl FindByPath(List<string> list)
        {
            TreeNodeControl pnode = (TreeNodeControl)treeView1.Nodes[0];
            list.RemoveAt(0);
            foreach (var id in list)
            {
                bool finded = false;
                foreach(TreeNodeControl node in pnode.Nodes)
                {
                    if(node.Text.ToLower() == id)
                    {
                        pnode = node;
                        break;
                    }
                }
                if (!false)
                    throw new Exception();
            }
            return pnode;
        }
    }
}
