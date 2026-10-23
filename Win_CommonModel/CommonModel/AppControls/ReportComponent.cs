using Microsoft.Extensions.Logging;

using WinFormsApp.AppViews.ColumnsModule;

namespace WinFormsApp.AppComponents.AppControls
{
    [Label("Компонент представления интерфейса пользователя")]
    public class ReportComponent : ComponentModel
    {
        [NotInput]
        private ILogger logger = LoggerFactory.Create((builder) => { builder.AddConsole(); }).CreateLogger(nameof(DataReport));

        
        [NotInput]
        public XmlNode<ComponentModel> XmlRoot;

        [Label("Источники данных")]
        [InputStructureCollection<BusinessDatasource>()]
        public DataSources DataSources { get; set; } = new();

        [Label("Наборы данных")]
        [InputStructureCollection<BusinessDataset>()]
        public DataSets DataSets { get; set; } = new();

        [NotInput]
        public Body Body { get; set; } = new();

        public ReportComponent()
        {
            XmlRoot = new XmlNode<ComponentModel>("root", new ComponentModel(), null);
        }
        public ReportComponent(XmlNode<ComponentModel> root)
        {
            XmlRoot = root;
            logger.LogInformation("CREATED");
        }



        public XmlNode<ComponentModel> GetBody()
        {
            XmlNode<ComponentModel> body = null;
            XmlRoot.DoBroadcastToHierElements(pnode =>
            {
                if (pnode.Item is not null && pnode.Item.Parameters is not null && pnode.Item.Parameters.Tag is not null && pnode.Item.Parameters.Tag.ToLower() == "body")
                {
                    body = pnode;
                }
            });
            return body;
        }

        /// <summary>
        /// Печать объектов в консоль
        /// </summary>
        public void WritetoLog()
        {
            XmlRoot.DoBroadcastToHierElements(pnode =>
            {
                int level = pnode.GetLevel();
                for (int i = 0; i < level; i++)
                {
                    System.Console.Write("   ");
                }
                System.Console.WriteLine(pnode.Item.Target.GetTypeName());
            });
        }
    }

    [NotInput]
    public class DataSources : List<BusinessDatasource>
    {
    }

    [NotInput]
    public class DataSets : List<BusinessDataset>
    {
    }

    [NotInput]
    public class DataList : List<dynamic>
    {
    }



    [Label("Главная форма приложения")]
    public class Body : ColumnsView
    {


    }

}