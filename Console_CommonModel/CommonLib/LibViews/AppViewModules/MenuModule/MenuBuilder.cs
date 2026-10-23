using Google.Protobuf.WellKnownTypes;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

[Label("Построитель меню")]
[Description("Класс для построения иерархических меню из XML-описаний, обеспечивающий создание структурированных меню приложения с поддержкой вложенности")]
public class MenuBuilder : IMenuBuilder
{
    public List<MenuItemModel> Menus = new List<MenuItemModel>();

    public MenuItemModel CreateMenu(string xml)
    {
        var p = ToMenuItem(xml);
        Menus.Add(p);
        return p;
    }

    public MenuItemModel CreateMenu(string Text, params MenuItemModel[] Items)
    {
        var p = new MenuItemModel(Text, Items);
        Menus.Add(p);
        return p;
    }

    public MenuItemModel CreateMenuSelect(MenuItemModel Parent, string Text, params MenuItemModel[] Items)
        => new MenuItemModel(Parent, Text, Items);


    public MenuItemModel ToMenuItem(string xml)
    {
        var serializer = new XmlSerializer(typeof(MenuItemModel));
        using (var reader = new StringReader(xml))
        {
            return (MenuItemModel)serializer.Deserialize(reader);
        }
    }

}
