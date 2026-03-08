namespace MiniOrdersAPI.Domain.Entities
{
    public class OrderLine
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }

        public Order Order { get; set; } = null!;
    }
}
