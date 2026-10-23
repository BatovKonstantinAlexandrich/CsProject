
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.ActionModels
{
    public class KeyEventArgs: EventArgs
    {
        public Keys KeyCode { get; set; }
        public string KeyCodeStr { get; set; }
        public bool Control { get; set; }
        public bool Alt { get; set; }
        public bool Shift { get; set; }
        public object KeyValue { get; set; }
        public bool SuppressKeyPress { get; set; }
        public bool Handled { get; set; }
    }
}
