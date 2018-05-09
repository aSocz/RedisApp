namespace RedisApp.Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int ProducerId { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual Producer Producer { get; set; }
        //public virtual ICollection<OrderItem> OrderItems { get; set; }

    }
}
