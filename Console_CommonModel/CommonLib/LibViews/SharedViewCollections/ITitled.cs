namespace Console_CommonModel.CommonLib.LibViews.SharedViewCollections
{
    public interface ITitled : IChangeSupport
    {
        public string Title { get; set; }
    }
}
