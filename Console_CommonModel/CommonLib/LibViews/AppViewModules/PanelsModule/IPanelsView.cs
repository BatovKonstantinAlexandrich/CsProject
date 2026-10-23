public interface IPanelsView
{
    ViewItem Bottom { get; set; }
    ViewItem Left { get; set; }
    ViewItem Right { get; set; }
    ViewItem Top { get; set; }

    void Add(ViewNode item);
    void Remove(ViewNode item);
}