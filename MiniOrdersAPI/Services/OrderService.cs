using Microsoft.EntityFrameworkCore;
using MiniOrdersAPI.Data;
using MiniOrdersAPI.Domain.Entities;
using MiniOrdersAPI.Domain.Enums;
using MiniOrdersAPI.DTOs;

namespace MiniOrdersAPI.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
        {
            if (request.Lines == null || !request.Lines.Any())
                throw new ArgumentException("Order must contain at least one line");

            var productIds = request.Lines.Select(l => l.ProductId).ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id) && p.IsActive)
                .ToListAsync();

            if (products.Count != productIds.Distinct().Count())
                throw new ArgumentException("One or more products do not exist");

            var order = new Order
            {
                CustomerName = request.CustomerName,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var line in request.Lines)
            {
                if (line.Quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than 0");

                var product = products.First(p => p.Id == line.ProductId);

                var orderLine = new OrderLine
                {
                    ProductId = product.Id,
                    Quantity = line.Quantity,
                    UnitPrice = product.Price,
                    LineTotal = product.Price * line.Quantity
                };

                order.TotalAmount += orderLine.LineTotal;
                order.Lines.Add(orderLine);
            }

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Where(x => x.Id == id)
                .Select(o => new OrderResponse
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt,
                    Lines = o.Lines.Select(l => new OrderLineResponse
                    {
                        ProductId = l.ProductId,
                        Quantity = l.Quantity,
                        UnitPrice = l.UnitPrice,
                        LineTotal = l.LineTotal
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<OrderResponse>> GetOrdersAsync(string? status, int page, int pageSize)
        {
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(status) &&
                Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(o => o.Status == parsedStatus);
            }

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderResponse
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt,
                    Lines = new List<OrderLineResponse>()
                })
                .ToListAsync();

            return new PagedResult<OrderResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = orders
            };
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, string newStatus)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return false;

            if (!Enum.TryParse<OrderStatus>(newStatus, true, out var parsedStatus))
                throw new ArgumentException("Invalid status");

            if (order.Status == OrderStatus.Draft)
            {
                if (parsedStatus == OrderStatus.Submitted ||
                    parsedStatus == OrderStatus.Cancelled)
                {
                    order.Status = parsedStatus;
                }
                else
                {
                    throw new InvalidOperationException("Invalid status transition");
                }
            }
            else if (order.Status == OrderStatus.Submitted)
            {
                if (parsedStatus == OrderStatus.Cancelled)
                {
                    throw new InvalidOperationException("Submitted orders cannot be cancelled");
                }
                else
                {
                    throw new InvalidOperationException("Invalid status transition");
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
