public interface IHierTable<T>
{
    T Parent { get; set; }

    string GetPath(string separator);
}