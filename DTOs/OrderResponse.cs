namespace MiniOrdersAPI.DTOs
{
    public class OrderResponse
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = null!;

        public string Status { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<OrderLineResponse> Lines { get; set; } = new();
    }

    public class OrderLineResponse
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }
    }
}
