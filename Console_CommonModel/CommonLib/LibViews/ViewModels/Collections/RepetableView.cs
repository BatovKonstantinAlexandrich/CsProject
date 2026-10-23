[Label("Коллекция представлений")]
public class RepetableView: TypeView
{
    public RepetableView()
    {
        Item = new ListView ();
    }

    public ListSelectionView GetItems()
    {
        return (ListSelectionView)Item;
    }

    public void Remove(ViewItem model)
    {
        GetItems().ListItems.Remove(model);
    }

    public void Add(ViewItem model)
    {
        GetItems().ListItems.Add(model);
    }
}