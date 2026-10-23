using System.Net;
using BlazorErp.Hubs;
using BlazorErp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorErp.Services;

public sealed class UsersHubConnectionService(
    NavigationManager navigation,
    IHttpContextAccessor httpContextAccessor,
    AuthenticationStateProvider authStateProvider) : IAsyncDisposable
{
    private HubConnection? _connection;
    private string? _connectedUserId;
    private readonly HashSet<int> _deliveredMessageIds = [];

    public event Action<MessageNotificationDto>? MessageReceived;
    public event Action<NewsNotificationDto>? NewsReceived;
    public event Action<OrderDeliveryNotificationDto>? OrderDeliveryReceived;
    public event Action<OrderPreparationNotificationDto>? OrderPreparationReceived;
    public event Action<OrderReadyForTransportNotificationDto>? OrderReadyForTransportReceived;
    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    public async Task EnsureConnectedAsync()
    {
        var auth = await authStateProvider.GetAuthenticationStateAsync();
        var userId = auth.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            await DisconnectAsync();
            return;
        }

        if (_connection != null && _connectedUserId == userId &&
            _connection.State is HubConnectionState.Connected or HubConnectionState.Connecting)
            return;

        await DisconnectAsync();
        _connectedUserId = userId;
        _deliveredMessageIds.Clear();
        _connection = BuildConnection();
        _connection.On<MessageNotificationDto>(UsersHub.ReceiveMessageNotification, notification =>
        {
            try
            {
                if (notification == null || notification.IsRead)
                    return;

                if (!_deliveredMessageIds.Add(notification.MessageId))
                    return;

                MessageReceived?.Invoke(notification);
            }
            catch
            {
            }
        });
        _connection.On<NewsNotificationDto>(UsersHub.ReceiveNewsNotification, notification =>
        {
            try
            {
                if (notification != null)
                    NewsReceived?.Invoke(notification);
            }
            catch
            {
            }
        });
        _connection.On<OrderDeliveryNotificationDto>(UsersHub.ReceiveOrderDeliveryNotification, notification =>
        {
            try
            {
                if (notification != null)
                    OrderDeliveryReceived?.Invoke(notification);
            }
            catch
            {
            }
        });
        _connection.On<OrderPreparationNotificationDto>(UsersHub.ReceiveOrderPreparationNotification, notification =>
        {
            try
            {
                if (notification != null)
                    OrderPreparationReceived?.Invoke(notification);
            }
            catch
            {
            }
        });
        _connection.On<OrderReadyForTransportNotificationDto>(UsersHub.ReceiveOrderReadyForTransportNotification, notification =>
        {
            try
            {
                if (notification != null)
                    OrderReadyForTransportReceived?.Invoke(notification);
            }
            catch
            {
            }
        });

        await _connection.StartAsync();
    }

    public async Task RequestPendingNotificationsAsync()
    {
        if (_connection?.State != HubConnectionState.Connected)
            return;

        await _connection.InvokeAsync("RequestPendingNotifications");
    }

    public async Task DisconnectAsync()
    {
        if (_connection != null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
            _connection = null;
        }
        _connectedUserId = null;
        _deliveredMessageIds.Clear();
    }

    private HubConnection BuildConnection()
    {
        var hubUri = navigation.ToAbsoluteUri("/hubs/users");
        var baseUri = navigation.ToAbsoluteUri("/");

        return new HubConnectionBuilder()
            .WithUrl(hubUri, options =>
            {
                options.HttpMessageHandlerFactory = _ =>
                {
                    var handler = new HttpClientHandler { UseCookies = true, CookieContainer = new CookieContainer() };
                    var httpContext = httpContextAccessor.HttpContext;
                    if (httpContext != null)
                    {
                        foreach (var cookie in httpContext.Request.Cookies)
                        {
                            handler.CookieContainer.Add(baseUri, new Cookie(cookie.Key, cookie.Value));
                        }
                    }
                    return handler;
                };
            })
            .WithAutomaticReconnect()
            .Build();
    }

    public async ValueTask DisposeAsync() => await DisconnectAsync();
}
