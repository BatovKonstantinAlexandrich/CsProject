using BlazorErp.Data;
using BlazorErp.Models;
using BlazorErp.Models.Enums;
using BlazorErp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorErp.Tests;

public static class TestDataHelper
{
    public const string ClientEmail = "client@demo.ru";
    public const string ClientPassword = "Client123!";
    public const string TransportEmail = "transport@demo.ru";
    public const string TransportPassword = "Transport123!";
    public const string StoreEmail = "store@demo.ru";
    public const string StorePassword = "Store123!";
    public const string SupplierEmail = "supplier@demo.ru";
    public const string SupplierPassword = "Supplier123!";

    public static async Task<int> CreateDeliveredOrderAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var cartService = scope.ServiceProvider.GetRequiredService<CartService>();
        var orderService = scope.ServiceProvider.GetRequiredService<OrderService>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var client = await userManager.FindByEmailAsync(ClientEmail)
            ?? throw new InvalidOperationException("Демо-клиент не найден.");
        var transport = await userManager.FindByEmailAsync(TransportEmail)
            ?? throw new InvalidOperationException("Демо-транспорт не найден.");
        var admin = await userManager.FindByEmailAsync("admin@demo.ru")
            ?? throw new InvalidOperationException("Демо-администратор не найден.");

        var product = db.Products.First(p => p.IsActive && p.StockQuantity > 0);
        await cartService.AddToCartAsync(client.Id, product.Id, 1);

        var order = await orderService.CreateOrderFromCartAsync(
            client.Id,
            DeliveryMethod.TransportCompany,
            "ул. Тестовая, 1",
            PaymentMethod.SBP);

        if (order == null)
            throw new InvalidOperationException("Не удалось создать тестовый заказ.");

        await orderService.ProcessPaymentAsync(order.Id, client.Id);
        await orderService.MarkOrderPreparedAsync(order.Id, admin.Id, isAdmin: true);
        await orderService.TakeOrderForDeliveryAsync(order.Id, transport.Id);
        await orderService.MarkDeliveredAsync(order.Id, transport.Id);

        return order.Id;
    }

    public static async Task<int> CreatePendingPaymentOrderAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var cartService = scope.ServiceProvider.GetRequiredService<CartService>();
        var orderService = scope.ServiceProvider.GetRequiredService<OrderService>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var client = await userManager.FindByEmailAsync(ClientEmail)
            ?? throw new InvalidOperationException("Демо-клиент не найден.");

        var product = db.Products.First(p => p.IsActive && p.StockQuantity > 0);
        await cartService.AddToCartAsync(client.Id, product.Id, 1);

        var order = await orderService.CreateOrderFromCartAsync(
            client.Id,
            DeliveryMethod.Courier,
            "ул. Оплаты, 2",
            PaymentMethod.SBP);

        return order?.Id ?? throw new InvalidOperationException("Не удалось создать заказ для оплаты.");
    }
}
