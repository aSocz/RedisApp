using System;
using System.Collections.Generic;

namespace RedisApp.Domain.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public string ClientName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Voivodeship { get; set; }
        public string Country { get; set; }
        public DateTime Date { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}