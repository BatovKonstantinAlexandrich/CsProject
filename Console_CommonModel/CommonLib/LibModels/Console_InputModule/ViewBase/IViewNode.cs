

public interface IViewNode: ITreeContainer<ViewNode>
{
    ViewNode Append(string name, ViewNode pchild);
    ViewNode Append(ViewNode pchild);
    void Do(Action<ViewNode> action);
    void DoUpper(Action<ViewNode> action);
    void Drop(ViewNode draggable);
    ViewNode Find(List<string> path);
    ViewNode Find(string path);
    void ForEach(Action<ViewNode> action);
    List<string> GetPath();
    ViewNode GetRoot();
    void RemoveFromParent();
    void SetParent(ViewNode parent);





}