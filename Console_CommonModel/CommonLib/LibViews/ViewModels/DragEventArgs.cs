namespace ActionModels
{
    public class DragEventArgs : EventArgs
    {
        public object Effect { get; internal set; }
        public object Data { get; internal set; }
        public int X { get; internal set; }
        public int Y { get; internal set; }

        internal string GetData(object unicodeText, bool v)
        {
            throw new NotImplementedException();
        }
    }
}