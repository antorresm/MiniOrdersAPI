using Microsoft.EntityFrameworkCore;
using MiniOrdersAPI.Data;
using MiniOrdersAPI.Domain.Entities;
using MiniOrdersAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=orders.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new Product { Name = "Laptop", Price = 1200, Currency = "EUR" },
            new Product { Name = "Mouse", Price = 25, Currency = "EUR" },
            new Product { Name = "Keyboard", Price = 80, Currency = "EUR" }
        );

        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
