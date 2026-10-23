namespace WinFormsApp.AppComponents.AppControlsModule
{
    public class DragDropTreeView : TreeView
    {
        private TreeNode draggedNode;

        public DragDropTreeView()
        {
            AllowDrop = true;

            // События для начала перетаскивания
            ItemDrag += OnItemDrag;

            // События для приема перетаскиваемых элементов
            DragEnter += OnDragEnter;
            DragOver += OnDragOver;
            DragDrop += OnDragDrop;
        }

        private void OnItemDrag(object sender, ItemDragEventArgs e)
        {
            draggedNode = (TreeNode)e.Item;
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            // Определяем позицию курсора
            Point targetPoint = PointToClient(new Point(e.X, e.Y));

            // Находим узел под курсором
            TreeNode targetNode = GetNodeAt(targetPoint);

            if (targetNode != null && draggedNode != targetNode)
            {
                SelectedNode = targetNode;
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            Point targetPoint = PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = GetNodeAt(targetPoint);

            if (draggedNode != null && targetNode != null)
            {
                // Удаляем из старого места
                draggedNode.Remove();

                // Добавляем в новое место
                targetNode.Nodes.Add(draggedNode);
                targetNode.Expand();

                draggedNode = null;
            }
        }
    }
}
