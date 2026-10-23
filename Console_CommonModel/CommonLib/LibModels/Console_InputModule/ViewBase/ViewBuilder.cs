using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using System.Reflection;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.MultiLineModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;

public interface IViewBuilder
{
}


[Label("Настройки представления")]
public class ViewBuilder : ViewFactory, IViewBuilder, IViewFactory
{

    public void AddText(string text, string fontName = "Roboto", int fontSize = 12)
    {
        this.Controls.Add(this.CreateText(text, fontName, fontSize));
    }

    public void AddButton(string text = "", Action onclick = null)
    {
        this.Controls.Add(this.CreateButton(text, onclick));

    }
     

    public void AddCheckbox(string text, Action<bool> onchanged)
    {
        this.Controls.Add(this.CreateCheckbox(text, onchanged));

    }

    public void AddCheckBox(string text, Action<bool> onchanged)
    {
        this.Controls.Add(this.CreateCheckBox(text, onchanged));

    }

    
    public void CheckList<TNamedObject>(
        IEnumerable<TNamedObject> items, Action<int, bool> onchanged = null) 
        where TNamedObject : NamedObject
    {
        this.Controls.Clear();
        this.Controls.Add(this.CreateCheckList<TNamedObject>(items, onchanged));
    }


    public void SelectList<TNamedObject>(
        string text,
        List<TNamedObject> items, Action<TNamedObject> onchanged = null)
        where TNamedObject : NamedObject
    {
        this.Controls.Clear();
        this.Controls.Add(this.CreateSelectList<TNamedObject>(text,items, onchanged));
    }


    public void SelectEntityList<TNamedObject, TDbContext>(
        string text,
        Action<TNamedObject> onchanged = null)
        where TDbContext : DbContextBase
        where TNamedObject : NamedObject
    {
        this.Controls.Clear();

        var list = this.CreateSelectEntityList<TNamedObject, TDbContext>(text, onchanged);
        var submit = list.ToSubmit((success, p) => {
            if(onchanged is not null)
                onchanged((TNamedObject)list.SelectedOption?.Item);
        });
        submit.Dock = "Fill";
        this.Controls.Add(submit);
    }

}

