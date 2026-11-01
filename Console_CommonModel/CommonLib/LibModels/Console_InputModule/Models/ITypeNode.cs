using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public interface ITypeNode<T> : TreeWalker<T>
{
    T GetParentNode();
    List<T> GetChildrenNodes();
    int GetLevel();
    List<string> GetPath();
    T FindPath(List<string> path);
    void ForEach(Action<TypeNode<T>> value);
}
