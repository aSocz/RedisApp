using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RedisApp.Infrastructure;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RedisApp.Sandbox
{
    public class RedisUploader : IDisposable
    {
        private readonly Lazy<ConnectionMultiplexer> lazyConnection;
        private ConnectionMultiplexer Connection => lazyConnection.Value;
        private IDatabase Database => Connection.GetDatabase();
        private readonly RedisAppContext dbContext;

        public RedisUploader()
        {
            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { "localhost" }
            };

            lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configurationOptions));
            dbContext = new RedisAppContext();
        }

        public void Test()
        {
            var sqlOrder = dbContext.Orders
                                    .Include(o => o.OrderItems)
                                        .ThenInclude(i => i.Product)
                                        .ThenInclude(p => p.Producer)
                                    .Include(o => o.OrderItems)
                                        .ThenInclude(i => i.Product)
                                        .ThenInclude(p => p.Category)
                                    .First();

            var redisOrder = Database.StringGet("order_" + sqlOrder.OrderId);
            var redisOrderDeserialized = JsonConvert.DeserializeObject<Domain.Entities.Order>(redisOrder);

            var server = lazyConnection.Value.GetServer("localhost:6379");
            var redisItemsCount = server.Keys(0, "order_*").Count();
            var sqlItemsCount = dbContext.Orders.Count();

            Console.WriteLine(sqlItemsCount == redisItemsCount);
            Console.ReadKey();

        }

        public void UploadData()
        {
            Console.WriteLine("Fetching data...");
            var orders = dbContext.Orders
                                  .Include(o => o.OrderItems)
                                      .ThenInclude(i => i.Product)
                                      .ThenInclude(p => p.Producer)
                                  .Include(o => o.OrderItems)
                                      .ThenInclude(i => i.Product)
                                      .ThenInclude(p => p.Category)
                                  .ToList()
                                  .Select(
                                       o => new KeyValuePair<RedisKey, RedisValue>(
                                           "order_" + o.OrderId.ToString(),
                                           JsonConvert.SerializeObject(
                                               o,
                                               Formatting.Indented,
                                               new JsonSerializerSettings
                                               {
                                                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                               })))
                                  .ToArray();

            var sw = new Stopwatch();
            sw.Start();

            Database.StringSet(orders);

            sw.Stop();
            Console.WriteLine($"Adding data to Redis database. Elapsed time: {sw.ElapsedMilliseconds}ms");
        }

        public void Dispose()
        {
            Connection?.Dispose();
        }
    }
}