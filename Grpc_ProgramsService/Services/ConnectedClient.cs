namespace Grpc_ProgramsService.Services;
using System.Threading.Channels;

using Grpc.Core;

public sealed class ConnectedClient
{
    private readonly Channel<ActionResponse> _queue;

    public string UserName { get; }
    public IServerStreamWriter<ActionResponse> Stream { get; }

    public ConnectedClient(string userName, IServerStreamWriter<ActionResponse> stream)
    {
        UserName = userName;
        Stream = stream;

        _queue = Channel.CreateUnbounded<ActionResponse>(new UnboundedChannelOptions
        {
            SingleReader = true,   
            SingleWriter = false   
        });
    }

    // Потокобезопасная постановка сообщения в очередь
    public ValueTask EnqueueAsync(ActionResponse message, CancellationToken ct = default)
        => _queue.Writer.WriteAsync(message, ct);

    // Завершение: больше сообщений не будет
    public void Complete() => _queue.Writer.TryComplete();

    // Единственная задача, которая пишет в responseStream — последовательно
    public async Task RunWriterAsync(CancellationToken ct)
    {
        await foreach (var message in _queue.Reader.ReadAllAsync(ct))
        {
            await Stream.WriteAsync(message, ct);
        }
    }
}

