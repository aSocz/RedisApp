using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using RedisApp.Domain.Entities;

namespace RedisApp.Sandbox
{
    public class OrderLoader
    {
        public void CreateOrderItems()
        {
            const int maxRows = 1000000;
            var productsIds = ProductsIdsProvider.GetProductsIds().ToList();
            var rand = new Random();
            var rows = new HashSet<DataRow>(maxRows);

            var dt = new DataTable();
            dt.Columns.Add(nameof(OrderItem.OrderItemId));
            dt.Columns.Add(nameof(OrderItem.OrderId));
            dt.Columns.Add(nameof(OrderItem.ProductId));
            dt.Columns.Add(nameof(OrderItem.Quantity));

            for (var i = 1; i < maxRows; i++)
            {
                var quantity = rand.Next(1, 100);
                var productId = productsIds[rand.Next(0, productsIds.Count)];
                var orderId = rand.Next(1, 110000);
                rows.Add(new DataRow(quantity, productId, orderId)); //ensuring there are no duplicates
            }

            foreach (var row in rows.Select((r, i) => new {r, i}))
            {
                dt.Rows.Add(row.i, row.r.OrderId, row.r.ProductId, row.r.Quantity);
            }

            using (var sqlBulk = new SqlBulkCopy("Data Source=.;Initial Catalog=RedisApp;Integrated Security=SSPI;"))
            {
                sqlBulk.DestinationTableName = "RedisApp.OrderItems";
                sqlBulk.WriteToServer(dt);
            }
        }
    }
}


