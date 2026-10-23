using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibModels.Console_LocationModule;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Console_CommonModel.CommonLib.LibModels.Console_LocationModule
{
    public partial class LocationDbContext
    {
        /// <summary>
        /// Компонент страницы
        /// </summary>
        [Label("Компонент интерфейса")]
        public class PageComponent : BaseEntity
        {
            public AppPage AsPage()
            {
                AppPage res = new();
                res.PageComponents.Add(this);
                return res;
            }
            public int? AppPageId { get; set; }

            [Newtonsoft.Json.JsonIgnore]
            [NotInput]
            [NotMapped]
            public AppPage AppPage { get; set; }

            [Label("Иконка")]
            [InputIcon]
            [NotNullNotEmpty]
            public string Icon { get; set; } = "home";

            [Label("Надпись")]
            public string Label { get; set; } = "Неизвестно";

            [InputPositiveInt()]
            [NotNullNotEmpty()]
            [Label("Приоритет вывода")]
            public int Order { get; set; }

            [Label("Видно на экране")]
            public bool Visible { get; set; } = true;

            [Label("Наименование")]
            [NotNullNotEmpty]
            [InputEngWord]
            public string Name { get; set; } = "Undefined";

            [Label("Описание")]
            [InputMultilineText]
            public string Description { get; set; } = "Нет подробного описания";

            
            public string Type { get; set; } = "Text";


            [Label("Разрешено изменение")]
            public bool Edited { get; set; } = true;

            public bool IsPrimitive { get; set; }
            public bool IsCollection { get; set; } = false;

            [NotMapped]
            [NotInput]
            public CollectionSettings CollectionSetup { get; set; }
            public class CollectionSettings
            {
                public string ItemType { get; set; }

            }

            //public string TextValue { get; set; }
            //public string ValueType { get; set; }

            [NotMapped]
            [Newtonsoft.Json.JsonIgnore]
         
            [System.Text.Json.Serialization.JsonIgnore]
            [NotInput]
            public object Control { get; set; }

            [Label("Размер")]
            [InputSelect("small,normal,big")]
            public string Size { get; set; } = "small";


            [Label("Состояние")]
            [InputSelect("valid,invalid,undefined")]
            public string State { get; set; } = "undefined";

            [Label("Подсказка")]
            public string Help { get; set; } = "Нет справочной информации";

            [Label("Ошибки")]
            [NotMapped]
            [NotInput]
            
            [Newtonsoft.Json.JsonIgnore]
            [System.Text.Json.Serialization.JsonIgnore]
            public Dictionary<string, List<string>> Errors { get; set; } = new();

            [Label("Атрибуты")]
            [NotMapped]
            [NotInput]
            [Newtonsoft.Json.JsonIgnore]
            [System.Text.Json.Serialization.JsonIgnore]
            public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

            [NotMapped]
            [Newtonsoft.Json.JsonIgnore]
            [System.Text.Json.Serialization.JsonIgnore]
            [NotInput]

            public Func<object> Getter { get; set; }

            [NotMapped]
            [Newtonsoft.Json.JsonIgnore]
            [System.Text.Json.Serialization.JsonIgnore]
            [NotInput]
            public Action<object> Setter { get; set; }

            [NotMapped]
            [Newtonsoft.Json.JsonIgnore]
            [System.Text.Json.Serialization.JsonIgnore]
            [NotInput]
            public object Value
            {
                get;
                set;
            }
        }
    }
}
