using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

[Label("Модель пункта меню")]
[Description("Модель элемента меню с поддержкой иерархической структуры, меток, иконок, команд и вложенных элементов для построения навигационных меню")]
public class MenuItemModel: ITreeNode<MenuItemModel>
{
    private MenuItemModel parentNode;
    private List<MenuItemModel> childNodes = new List<MenuItemModel>();
    private MenuItemModel item;

    private string fontSize;
    private string labelText;
    private string iconCode;
    private string hotKeys;
    private Action onClicked;
    private Action onOver;
    private Action onLeave;
    private MenuItemModel parent;
    private string text;
    private List<MenuItemModel> items;
    private MenuItemModel model;

    public MenuItemModel(MenuItemModel parent, string text, params MenuItemModel[] items)
    {
        this.parent = parent;
        this.text = text;
        this.items = new List<MenuItemModel>(items);
    }

    public MenuItemModel() { }
    public MenuItemModel(MenuItemModel parent, MenuItemModel model)
    {
        this.parent = parent;
        this.model = model;
    }

    public MenuItemModel(string text, params MenuItemModel[] items)
    {
        this.text = text;
        this.items = new List<MenuItemModel>(items);
    }

    public MenuItemModel(string Text, Action OnClick)
    {
        this.onClicked = onClicked;
        this.text = text;
    }

    public string GetSize() => fontSize;    // font-size
    public string GetText() => labelText;    // label text
    public string GetIcon() => iconCode;    // icon-code
    public string GetHotKeys() => hotKeys; // hotkey code

    private Action OnClick() => onClicked;    // action on click
    private Action OnOver() => onOver;       // action on mouseover
    private Action OnLeave() => onLeave;    // action on mouseleave

    public ITreeNode<MenuItemModel> GetParent() => parentNode;
    public MenuItemModel GetItem() => item;
    public List<ITreeNode<MenuItemModel>> GetChildren() => childNodes.ToList<ITreeNode<MenuItemModel>>();

    public string ToXml()
    {
        var serializer = new XmlSerializer(GetType());
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(stream, this);
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

}
