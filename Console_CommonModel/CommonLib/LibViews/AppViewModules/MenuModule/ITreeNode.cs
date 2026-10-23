using System.Collections.Generic;

public interface ITreeNode<T>
{
    public ITreeNode<T> GetParent();
    public List<ITreeNode<T>> GetChildren();
}
