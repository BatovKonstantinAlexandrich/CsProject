namespace Grpc_ProgramsService.Services
{
    public sealed class ConnectionPool
    {
        private readonly Dictionary<string, ConnectedClient> _clients = new();
        private readonly SemaphoreSlim _lock = new(1, 1);
        private readonly ILogger<ConnectionPool> _logger;

        public ConnectionPool(ILogger<ConnectionPool> logger) => _logger = logger;

        public async Task<bool> TryAddAsync(ConnectedClient client)
        {
            await _lock.WaitAsync();
            try
            {
                if (!_clients.TryAdd(client.UserName, client))
                    return false; // имя занято
                _logger.LogInformation("{User} подключился", client.UserName);
                return true;
            }
            finally { _lock.Release(); }
        }

        public async Task RemoveAsync(ConnectedClient client)
        {
            await _lock.WaitAsync();
            try
            {
                if (_clients.TryGetValue(client.UserName, out var existing) && existing == client)
                    _clients.Remove(client.UserName);
            }
            finally { _lock.Release(); }
            _logger.LogInformation("{User} отключился", client.UserName);
        }

        // Рассылка всем: только ставим в очереди, ничего не пишем напрямую
        public async Task BroadcastAsync(ActionResponse message, CancellationToken ct)
        {
            ConnectedClient[] snapshot;
            await _lock.WaitAsync(ct);
            try { snapshot = _clients.Values.ToArray(); }
            finally { _lock.Release(); }

            foreach (var client in snapshot)
            {
                // TryWrite не блокирует; для надёжности можно await EnqueueAsync
                await client.EnqueueAsync(message, ct);
            }
        }
    }
}
