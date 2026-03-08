namespace MiniOrdersAPI.DTOs
{
    public class CreateOrderRequest
    {
        public string CustomerName { get; set; } = null!;

        public List<CreateOrderLineRequest> Lines { get; set; } = new();
    }

    public class CreateOrderLineRequest
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
