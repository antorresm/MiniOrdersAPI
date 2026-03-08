using MiniOrdersAPI.Domain.Enums;

namespace MiniOrdersAPI.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = null!;

        public OrderStatus Status { get; set; } = OrderStatus.Draft;

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderLine> Lines { get; set; } = new();
    }
}
