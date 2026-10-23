namespace Console_CommonModel.CommonLib.LibModels.XmlAppModel
{
    [Label("Модель приложения XML")]
    public class XmlAppStartup : DescriptableObject
    {
        [Label("XML код")]
        [InputXml]
        [NotNullNotEmpty]
        public string Xml { get; set; }
    }
}
