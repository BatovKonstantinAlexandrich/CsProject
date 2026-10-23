[Label("Сертификат доступа")]
[Description("Устанавливается на клиента для получения доступа к сервису")]
public class ServiceSertificate: PrivateData<ServiceContext>
{
    [Label("Открытый ключ")]
    [NotNullNotEmpty("Необходимо указать открытый ключ")]
    [InputFileData()]
    public List<byte> PublicKey { get; set; } = new();

    [Label("Закрытый ключ")]
    [NotNullNotEmpty("Необходимо указать закрытый ключ")]
    [InputFileData()]
    public List<byte> PrivateKey { get; set; } = new( );
}