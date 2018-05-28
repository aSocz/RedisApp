using System;
using System.Collections.Generic;

namespace RedisApp.Domain.Entities
{
    public class Producer
    {
        public int ProducerId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}