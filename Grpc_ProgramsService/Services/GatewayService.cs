using Google.Protobuf;

using Grpc.Core;

using Microsoft.AspNetCore.Authentication;

using static Grpc_ProgramsService.GatewayService;

namespace Grpc_ProgramsService.Services
{
  
    public class MessageGateway: GatewayServiceBase
    {
        private readonly ConnectionPool _room;
        private readonly ILogger<MessageGateway> _logger;

        public MessageGateway(ConnectionPool room, ILogger<MessageGateway> logger)
        {
            _room = room;
            _logger = logger;
        }

        public override async Task Start(IAsyncStreamReader<ActionResponse> requestStream, IServerStreamWriter<ActionRequest> responseStream, ServerCallContext context)
        {
            var ct = context.CancellationToken;

            // Первое сообщение трактуем как «представление» (имя пользователя)
            if (!await requestStream.MoveNext(ct))
                return;
            var hello = requestStream.Current;

            var client = new ConnectedClient(hello.AccessToken, null);// requestStream);
            if (!await _room.TryAddAsync(client))
            {
                await responseStream.WriteAsync(new ActionRequest
                {
                    AccessToken = "server" 
                }, ct);
                return;
            }

            // Задача-писатель: единственная, кто трогает responseStream
            var writerTask = client.RunWriterAsync(ct);

            try
            {
                // Приветствие от сервера
                await client.EnqueueAsync(new ActionResponse
                {
                     
                }, ct);

                // Читаем входящие и рассылаем всем
                while (await requestStream.MoveNext(ct))
                {
                    var incoming = requestStream.Current;
                    incoming.AccessToken = client.UserName; // не доверяем имени от клиента
                    await _room.BroadcastAsync(incoming,ct);
                    //await _room.BroadcastAsync(new ActionRequest()
                    //{
                    //    ActionName      = "OnResponse",
                    //    MessageObject   = Base64UrlTextEncoder.Encode(incoming.ToByteArray())
                    //}, ct);
                }
            }
            catch (OperationCanceledException)
            {
                // клиент отключился — штатная ситуация
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в чате у {User}", client.UserName);
            }
            finally
            {
                // 1) запрещаем новые сообщения в очередь
                client.Complete();
                // 2) дожидаемся, пока писатель допишет остаток
                try { await writerTask; }
                catch (OperationCanceledException) { }
                // 3) убираем из реестра
                await _room.RemoveAsync(client);
            }
        }
    }
}
