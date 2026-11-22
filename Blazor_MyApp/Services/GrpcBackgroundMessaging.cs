using Blazor_MyApp.Services.ContextServices;

using Grpc.Core;
using Grpc.Net.Client;

using Grpc_ProgramsService;

using GrpcPort.Client.Crypto;
using GrpcPort.Contracts;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

using MySqlX.XDevAPI;

using Newtonsoft.Json;

using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Blazor_MyApp.Services
{
    public class GrpcBackgroundMessaging : BackgroundService
    {
        private readonly ConnectionsCollection _connections;
        string clientId = "svc-orders";
        string secret = $"secret-svc-orders";
        string serverAddress = "http://localhost:5081";

        public GrpcBackgroundMessaging(ConnectionsCollection connections)
        {
            this._connections = connections;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using var channel = GrpcChannel.ForAddress(serverAddress, new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(30),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(10)
                }
            });

            var client = new PortService.PortServiceClient(channel);

            // 1. Авторизация и получение сертификата шифрования
            // (повтор при одновременном старте с сервером из VS Code compound)
            Console.WriteLine(">> Authenticate...");
            AuthResponse? auth = null;
            for (var attempt = 1; attempt <= 20; attempt++)
            {
                try
                {
                    auth = await client.AuthenticateAsync(new AuthRequest
                    {
                        ClientId = clientId,
                        Secret = secret
                    });
                    break;
                }
                catch (RpcException ex) when (attempt < 20 &&
                    ex.StatusCode is StatusCode.Unavailable or StatusCode.Internal or StatusCode.Cancelled)
                {
                    Console.WriteLine($"   сервер ещё не готов ({ex.StatusCode}), попытка {attempt}/20...");
                    await Task.Delay(500);
                }
            }

            if (auth is null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Не удалось подключиться к серверу.");
                Console.ResetColor();
                throw new Exception();
            }

            if (!auth.Success)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Auth failed: {auth.Message}");
                Console.ResetColor();
                throw new Exception();

            }




            using var cert = X509CertificateLoader.LoadCertificate(auth.EncryptionCertificate.ToByteArray());
            Console.WriteLine($"OK  session_token = {auth.SessionToken[..12]}...");
            Console.WriteLine($"OK  cert subject  = {cert.Subject}");
            Console.WriteLine($"OK  thumbprint    = {auth.CertificateThumbprint}");
            Console.WriteLine($"OK  expires       = {DateTimeOffset.FromUnixTimeSeconds(auth.ExpiresUnix):u}");
            Console.WriteLine();

            // 2. Открытие двустороннего потока
            Console.WriteLine(">> OpenStream...");
            using var call = client.OpenStream();


            ConcurrentQueue<ActionResponse> responses = new();

            var receiveTask = Task.Run(async () =>
            {
                await foreach (var msg in call.ResponseStream.ReadAllAsync())
                {
                    var payload = msg.Payload.ToStringUtf8();
                    if (payload is not null)
                    {
                        this.Info(payload);
                        var actionRequest = JsonConvert.DeserializeObject<GatewayActionRequest>(payload);
                        if (IsAuthenticated(actionRequest))
                        {



                        }
                        else
                        {
                            new GatewayActionResponse();
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"<< [{msg.Type}] id={msg.MessageId[..8]} corr={Truncate(msg.CorrelationId)} payload=\"{payload}\"");
                    Console.ResetColor();
                }
            });

            bool IsAuthenticated(GatewayActionRequest? actionRequest)
            {
                return true;
            }

            async Task SendAsync(string type, string payload, bool encrypt = false)
            {
                var bytes = Encoding.UTF8.GetBytes(payload);
                if (encrypt)
                    bytes = PayloadCrypto.Encrypt(bytes, cert);

                var envelope = new StreamEnvelope
                {
                    SessionToken = auth.SessionToken,
                    MessageId = Guid.NewGuid().ToString("N"),
                    TimestampUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    Type = type,
                    Payload = Google.Protobuf.ByteString.CopyFrom(bytes)
                };
                envelope.Headers["client_id"] = clientId;
                if (encrypt)
                    envelope.Headers["encrypted"] = "true";

                await call.RequestStream.WriteAsync(envelope);
                Console.WriteLine($">> [{type}] id={envelope.MessageId[..8]} encrypt={encrypt} payload=\"{payload}\"");
            }

            static string Truncate(string value) =>
                string.IsNullOrEmpty(value) ? "-" : value.Length <= 8 ? value : value[..8];


            var responding = Task.Run(async () =>
            {
                while (true)
                {
                    if (responses.TryDequeue(out var p))
                    {
                        await SendAsync("OnResponse", p.ToString(), encrypt: true);
                    }
                    else
                    {
                        await SendAsync("time", DateTime.Now.ToString(), encrypt: true);
                    }
                    await Task.Delay(500);

                }
            });


            var demo = false;
            if (demo)
            {
                await Task.Delay(800);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("Введите сообщение и Enter (пустая строка / quit — выход).");
                Console.WriteLine("Префикс ! — отправить без шифрования.");

                while (true)
                {
                    var line = Console.ReadLine();
                    if (line is null || line.Equals("quit", StringComparison.OrdinalIgnoreCase) || line.Length == 0)
                        break;

                    var encrypt = !line.StartsWith('!');
                    var text = encrypt ? line : line[1..];
                    await SendAsync("data", text, encrypt);
                }
            }

            await call.RequestStream.CompleteAsync();
            try
            {
                await receiveTask.WaitAsync(TimeSpan.FromSeconds(3));
            }
            catch (TimeoutException)
            {
            }


        }
    }
}
