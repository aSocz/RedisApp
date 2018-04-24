using System;

namespace RedisApp.Sandbox
{
    public class DataRow : IEquatable<DataRow>
    {
        public DataRow(int quantity, int productId, int orderId)
        {
            Quantity = quantity;
            ProductId = productId;
            OrderId = orderId;
        }

        public int Quantity { get; }
        public int ProductId { get; }
        public int OrderId { get; }

        public bool Equals(DataRow other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ProductId == other.ProductId
                && OrderId == other.OrderId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((DataRow) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ProductId;
                hashCode = (hashCode * 397) ^ OrderId;
                return hashCode;
            }
        }
    }
}