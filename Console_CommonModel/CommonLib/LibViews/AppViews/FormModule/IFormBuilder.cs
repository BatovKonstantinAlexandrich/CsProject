using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;


namespace Console_CommonModel.CommonLib.LibViews.AppViews.FormModule
{

    public interface IFormBuilder
    {
        public void AddActionButtonFormGroup(string label, Action<object> onClick);
        public void AddActionButtonsFormGroup(IDictionary<string, Action<object>> buttons);
        public void AddInputTextFormGroup(string name, string title, string value, Action<string> onChange);
        public void AddCheckboxFormGroup(string name, string title, bool value, Action<bool> onChange);
        public void AddSelectBoxFormGroup(string name, string title, string value, IEnumerable<string> options, Action<string> onChange);
        public void AddSelectBoxFormGroup(string name, string title, int value, IDictionary<int, string> options, Action<int, string> onChange);
    }
}
