using Console_CommonModel.API;

namespace ActionModels
{
    public class MouseEventArgs : EventArgs
    {
        public MouseEventArgs() { }
        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
        {
            Button = button;
            Clicks = clicks;
            X = x;
            Y = y;
            Delta = delta;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public MouseButtons Button { get; set; }
        public int Clicks { get; }
        public int Delta { get; }
    }
}