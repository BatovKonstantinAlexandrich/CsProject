using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WinFormsApp.Console.Console_ClassModule.ClassServiceModel;



namespace WinFormsApp.ClassViewModel
{
    public partial class ClassModelControl : UserControl
    {
        private readonly TreeNode root;
        private ClassDbContext dbContext = new();
        public ClassModelControl()
        {
            InitializeComponent();
            this.root = new TreeNode() { Text = "Классы" };
            this.treeView1.Nodes.Add(root);

            this.UpdateClassList();
        }

        private void UpdateClassList()
        {
            foreach(var cl in dbContext.Classes.ToList())
            {
                root.Nodes.Add(new TreeNode() { Text = cl.Name });
            }
        }
    }
}
