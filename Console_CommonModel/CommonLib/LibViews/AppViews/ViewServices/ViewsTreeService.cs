

namespace Console_CommonModel.CommonLib.LibViews.AppViews.ViewServices
{

    /// <summary>
    /// Работа с иерархией представления
    /// -Поиск компонента созданного на основе представления
    /// -Обработка узлов поддерева (иерархия представлений) вертикально вниз
    /// </summary>
    public class ViewsTreeService
    {

        /// <summary>
        /// Обработка узлов поддерева вертикально вниз
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="handle"></param>
        public void ForEachViewNode(ViewItem pnode, Action<ViewItem> todo)
        {
            todo(pnode);
            foreach (var pchild in pnode.Controls)
            {
                ForEachViewNode(pnode, todo);
            }
        }



    }

}
