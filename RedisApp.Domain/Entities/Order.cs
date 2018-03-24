using System.Collections.Generic;

namespace SportsStore.Domain.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public string ClientName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Voivodeship { get; set; }
        public virtual ICollection<OrderItem> Items { get; }
    }
}