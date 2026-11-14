using System.Security.Cryptography.X509Certificates;
using System.Text;

using ClassModelLib.ServicesModule;

using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

using Grpc.Core;
using Grpc.Net.Client;

using Grpc_ProgramsService;

using GrpcPort.Client.Actions;
using GrpcPort.Client.Crypto;
using GrpcPort.Contracts;

using Newtonsoft.Json;

using WinFormsApp.Console.Console_ClassModule.ClassServiceModel;

var client = new GrpcActionsClient("http://localhost:5081", "svc-orders", $"secret-svc-orders");
client.ConnectAsync().Wait();
foreach(WinFormsApp.Console.Console_ClassModule.ClassDataModel.MethodModel method in client.GetType().ToClassModel().Methods)
{
    client.PublishAsync(new()
    {
        Name = method.Name,
        Attributes = method.Attributes,
        
    }, args => null, default);
}
while(true)
{
    InputConsole.Clear();
    var selected = InputConsole.SingleSelect
    (
        "Выберите действие",
        client.GetKnownRemoteActions().Select(m => m.Name),
        ref args
    );

    var m = client.GetKnownRemoteActions().FirstOrDefault(m => m.Name==selected) ;
    //TODO:
}

/*
Console.Write("Port: ");
var port = Console.ReadLine();
var serverAddress = args.ElementAtOrDefault(0) ?? "http://localhost:5081";
var clientId = args.ElementAtOrDefault(1) ?? "svc-orders";
var secret = args.ElementAtOrDefault(2) ?? $"secret-{clientId}";

//Console.WriteLine("=== GrpcPort Client ===");
//Console.WriteLine($"Server : {serverAddress}");
//Console.WriteLine($"Client : {clientId}");
//Console.WriteLine();

//using var channel = GrpcChannel.ForAddress(serverAddress, new GrpcChannelOptions
//{
//    HttpHandler = new SocketsHttpHandler
//    {
//        EnableMultipleHttp2Connections = true,
//        KeepAlivePingDelay = TimeSpan.FromSeconds(30),
//        KeepAlivePingTimeout = TimeSpan.FromSeconds(10)
//    }
//});

//var client = new PortService.PortServiceClient(channel);

//// 1. Авторизация и получение сертификата шифрования
//// (повтор при одновременном старте с сервером из VS Code compound)
//Console.WriteLine(">> Authenticate...");
//AuthResponse? auth = null;
//for (var attempt = 1; attempt <= 20; attempt++)
//{
//    try
//    {
//        auth = await client.AuthenticateAsync(new AuthRequest
//        {
//            ClientId = clientId,
//            Secret = secret
//        });
//        break;
//    }
//    catch (RpcException ex) when (attempt < 20 &&
//        ex.StatusCode is StatusCode.Unavailable or StatusCode.Internal or StatusCode.Cancelled)
//    {
//        Console.WriteLine($"   сервер ещё не готов ({ex.StatusCode}), попытка {attempt}/20...");
//        await Task.Delay(500);
//    }
//}

//if (auth is null)
//{
//    Console.ForegroundColor = ConsoleColor.Red;
//    Console.WriteLine("Не удалось подключиться к серверу.");
//    Console.ResetColor();
//    return 1;
//}

//if (!auth.Success)
//{
//    Console.ForegroundColor = ConsoleColor.Red;
//    Console.WriteLine($"Auth failed: {auth.Message}");
//    Console.ResetColor();
//    return 1;
//}

//using var cert = X509CertificateLoader.LoadCertificate(auth.EncryptionCertificate.ToByteArray());
//Console.WriteLine($"OK  session_token = {auth.SessionToken[..12]}...");
//Console.WriteLine($"OK  cert subject  = {cert.Subject}");
//Console.WriteLine($"OK  thumbprint    = {auth.CertificateThumbprint}");
//Console.WriteLine($"OK  expires       = {DateTimeOffset.FromUnixTimeSeconds(auth.ExpiresUnix):u}");
//Console.WriteLine();

//// 2. Открытие двустороннего потока
//Console.WriteLine(">> OpenStream...");
//using var call = client.OpenStream();



//var receiveTask = Task.Run(async () =>
//{
//    await foreach (var msg in call.ResponseStream.ReadAllAsync())
//    {
//        var payload = msg.Payload.ToStringUtf8();
//        if (payload is not null)
//        {
//            var actionRequest = JsonConvert.DeserializeObject<GatewayActionRequest>(payload);
//            if (IsAuthenticated(actionRequest))
//            {

//            }
//            else
//            {
//                new GatewayActionResponse();
//            }
//        }
//        Console.ForegroundColor = ConsoleColor.Cyan;
//        Console.WriteLine($"<< [{msg.Type}] id={msg.MessageId[..8]} corr={Truncate(msg.CorrelationId)} payload=\"{payload}\"");
//        Console.ResetColor();
//    }
//});

//bool IsAuthenticated(GatewayActionRequest? actionRequest)
//{
//    throw new NotImplementedException();
//}

//async Task SendAsync(string type, string payload, bool encrypt = false)
//{
//    var bytes = Encoding.UTF8.GetBytes(payload);
//    if (encrypt)
//        bytes = PayloadCrypto.Encrypt(bytes, cert);

//    var envelope = new StreamEnvelope
//    {
//        SessionToken = auth.SessionToken,
//        MessageId = Guid.NewGuid().ToString("N"),
//        TimestampUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
//        Type = type,
//        Payload = Google.Protobuf.ByteString.CopyFrom(bytes)
//    };
//    envelope.Headers["client_id"] = clientId;
//    if (encrypt)
//        envelope.Headers["encrypted"] = "true";

//    await call.RequestStream.WriteAsync(envelope);
//    Console.WriteLine($">> [{type}] id={envelope.MessageId[..8]} encrypt={encrypt} payload=\"{payload}\"");
//}

//static string Truncate(string value) =>
//    string.IsNullOrEmpty(value) ? "-" : value.Length <= 8 ? value : value[..8];



//var pinging = Task.Run(async () => {
//    while (true)
//    {
//        await Task.Delay(500);
//        await SendAsync("time", DateTime.Now.ToString(), encrypt: true);
//    }
//});


//var demo = args.Any(a => a is "--demo" or "-d");
//if (demo)
//{
//    await Task.Delay(800);
//}
//else
//{
//    Console.WriteLine();
//    Console.WriteLine("Введите сообщение и Enter (пустая строка / quit — выход).");
//    Console.WriteLine("Префикс ! — отправить без шифрования.");

//    while (true)
//    {
//        var line = Console.ReadLine();
//        if (line is null || line.Equals("quit", StringComparison.OrdinalIgnoreCase) || line.Length == 0)
//            break;

//        var encrypt = !line.StartsWith('!');
//        var text = encrypt ? line : line[1..];
//        await SendAsync("data", text, encrypt);
//    }
//}

//await call.RequestStream.CompleteAsync();
//try
//{
//    await receiveTask.WaitAsync(TimeSpan.FromSeconds(3));
//}
//catch (TimeoutException)
//{
//}

//Console.WriteLine("Клиент завершён.");
//return 0;

*/