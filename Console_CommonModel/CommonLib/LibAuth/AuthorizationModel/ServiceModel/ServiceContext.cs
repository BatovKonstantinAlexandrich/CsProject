using Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationModel.ServiceModel;

[Icon("build")]
[Label("Микрослужба")]
public class ServiceContext : ActiveObject
{
    [Label("Информация")]
    public int ServiceInfoId { get; set; }
    public ServiceInfo ServiceInfo { get; set; }

    [Label("Настройки")]
    public int ServiceSettingsId { get; set; }
    public ServiceSettings ServiceSettings { get; set; }

    [Label("Сертификат")]
    public int ServiceSertificateId { get; set; }
    public ServiceSertificate ServiceSertificate { get; set; }

    [Label("Сообщения")]
    public List<ServiceMessage> ServiceMessages { get; set; }

    [Label("Группы")]
    public List<ServiceGroupBindings> ServiceGroups { get; set; }

    [Label("Группы")]
    public List<ServiceResource> ServiceResources { get; set; }


}
