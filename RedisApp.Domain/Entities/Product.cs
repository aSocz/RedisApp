using System.Collections.Generic;

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
        public Category Category { get; set; }
        public Producer Producer { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

    }
}
