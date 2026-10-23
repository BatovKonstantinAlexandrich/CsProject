using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule
{

    /// <summary>
    /// Упрощённая модель используется для сериализации
    /// </summary>
    public class MenuModel
    {
        public string Label { get; set; } = "Создать новое меню";

        public string Command { get; set; }

        [Label("Иконка (имя файла в папке Icons)")]
        public string Icon { get; set; }

        [Label("Сочетание клавиш (например Ctrl+O или F1)")]
        public string Shortcut { get; set; }

        [NotInput]
        public List<MenuModel> Links { get; set; } = new List<MenuModel>() { };

        public MenuModel() { }
        public MenuModel(string path)
        {
            var names = path.Split('/');
            Label = names.Last();
            Command = path;
        }


        public static List<MenuModel> FromJson(string json)
            => JsonConvert.DeserializeObject<List<MenuModel>>(json);

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
