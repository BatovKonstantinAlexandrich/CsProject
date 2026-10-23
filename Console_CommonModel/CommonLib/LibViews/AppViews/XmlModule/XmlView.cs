using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibModels.Console_XmlModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.Others;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.XmlModule
{
    [Label("Представление XML")]
    public class XmlView : ColumnsView
    {
        public XmlView()
        {
            XmlText = "";
        }

        /// <summary>Скомпилированный отчёт (только runtime, в JSON не пишется).</summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Label("Компонент Отчёт")]
        public ReportComponent report { get; set; } = new();

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Label("XML-документ")]
        [NotInput]
        public XmlNode<ComponentModel> XmlRoot => report?.XmlRoot;

        [Label("Источники данных")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputStructureCollection<BusinessDatasource>]
        public DataSources DataSources => report?.DataSources;

        [Label("Наборы данных")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputStructureCollection<BusinessDataset>]
        public DataSets DataSets => report?.DataSets;

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotInput]
        public Body Body => report?.Body;

        /// <summary>XML-разметка для сохранения и загрузки через JSON.</summary>
        [Label("XML разметка")]
        [InputXml()]
        public string XmlText
        {
            get
            {
                try
                {
                    if (Controls.Count > 0 || report?.GetBody() is not null)
                        return ToXml();
                }
                catch
                {
                    /* report ещё не собран */
                }

                return Get<string>(nameof(XmlText)) ?? "";
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Set(nameof(XmlText), "");
                    Controls.Clear();
                    return;
                }

                Set(nameof(XmlText), value);
                FromXml(value);
            }
        }

        /// <summary>Дочерние представления восстанавливаются из <see cref="XmlText"/>, не дублируются в JSON.</summary>
        public bool ShouldSerializeControls() => false;

        public void OpenFile(string filename = "Projects\\test.xml")
        {
            var xml = File.ReadAllText(filename);
            FromXml(xml);
            Set(nameof(XmlText), xml);
        }

         

        //public override Control ToControl() => ToControl<AppViewBase>();

        public string ToXml()
        {
            if (report?.Body is not null)
            {
                report.Body.Controls.Clear();
                report.Body.Controls.AddRange(Controls.Select(p => (ViewItem)p).ToArray());
            }

            return XmlNodeCompiler.GetXml(report);
        }
    }
}
