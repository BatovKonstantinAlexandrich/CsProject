using BlazorErp.Data;
using BlazorErp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorErp.FunctionalTests;

public sealed class FunctionalTestContext(IServiceProvider services, ILogger logger)
{
    public IServiceProvider Services { get; } = services;
    public ILogger Logger { get; } = logger;

    public async Task<T> ResolveAsync<T>() where T : notnull
    {
        await using var scope = Services.CreateAsyncScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    public async Task<ApplicationUser> GetUserByEmailAsync(string email)
    {
        await using var scope = Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByEmailAsync(email)
            ?? throw new InvalidOperationException($"Пользователь {email} не найден.");
        return user;
    }

    public async Task<string> GetUserIdAsync(string email)
    {
        var user = await GetUserByEmailAsync(email);
        return user.Id;
    }

    public async Task<bool> UserInRoleAsync(string email, string role)
    {
        await using var scope = Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByEmailAsync(email);
        return user != null && await userManager.IsInRoleAsync(user, role);
    }

    public async Task<Product> GetAnyProductAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await db.Products.FirstAsync(p => p.IsActive && p.StockQuantity > 0);
    }
}
