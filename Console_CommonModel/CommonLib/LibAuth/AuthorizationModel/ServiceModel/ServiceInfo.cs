// == UserPerson

using Console_CommonModel.CommonLib.LibData.AbstractionModel;

[Label("Сведения о сервисе")]
public class ServiceInfo:  BaseEntity
{
    
    [NotNullNotEmpty]
    public string Url { get; set; } = "https://localhost";


    [Label("Наименование")]
    [NotNullNotEmpty]
    public string Name { get; set; } = "Api";

    [Label("Версия")]
    [NotNullNotEmpty]
    public string Version { get; set; } = "1.0.0";
    public byte[] Dll { get; internal set; }
}