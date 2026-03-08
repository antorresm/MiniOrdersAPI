using Microsoft.EntityFrameworkCore;
using MiniOrdersAPI.Data;
using MiniOrdersAPI.Domain.Entities;
using MiniOrdersAPI.DTOs;
using MiniOrdersAPI.Services;
using System;
using Xunit;

namespace MiniOrdersAPI.Tests.Services;

public class OrderServiceTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateOrder_ShouldCalculateTotalCorrectly()
    {
        var context = CreateDbContext();

        context.Products.Add(new Product
        {
            Id = 1,
            Name = "MacBook",
            Price = 1400,
            Currency = "EUR",
            IsActive = true
        });

        await context.SaveChangesAsync();

        var service = new OrderService(context);

        var request = new CreateOrderRequest
        {
            CustomerName = "Test NewTest",
            Lines = new List<CreateOrderLineRequest>
            {
                new CreateOrderLineRequest
                {
                    ProductId = 1,
                    Quantity = 2
                }
            }
        };

        var order = await service.CreateOrderAsync(request);

        Assert.Equal(2800, order.TotalAmount);
        Assert.Single(order.Lines);
    }

    [Fact]
    public async Task CreateOrder_ShouldThrow_WhenQuantityIsZero()
    {
        var context = CreateDbContext();

        context.Products.Add(new Product
        {
            Id = 1,
            Name = "ASUS Vivobook",
            Price = 1000,
            Currency = "EUR",
            IsActive = true
        });

        await context.SaveChangesAsync();

        var service = new OrderService(context);

        var request = new CreateOrderRequest
        {
            CustomerName = "Test NewSecondTest",
            Lines = new List<CreateOrderLineRequest>
            {
                new CreateOrderLineRequest
                {
                    ProductId = 1,
                    Quantity = 0
                }
            }
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task UpdateOrderStatus_ShouldThrow_WhenSubmittedToCancelled()
    {
        var context = CreateDbContext();

        var order = new Order
        {
            Id = 1,
            CustomerName = "Test NewTestThree",
            Status = Domain.Enums.OrderStatus.Submitted,
            TotalAmount = 100
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var service = new OrderService(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateOrderStatusAsync(1, "Cancelled"));
    }
}