namespace Console_CommonModel.CommonLib.LibViews.AppViews.ViewAttribute
{
    /// <summary>
    /// Шаблон XML модели представления
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class XmlTemplateAttribute : Attribute
    {
        private readonly string xml;

        public XmlTemplateAttribute(string xml)
        {
            this.xml = xml;
        }
    }
}
