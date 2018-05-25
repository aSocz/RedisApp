using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RedisApp.Infrastructure;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RedisApp.Controllers
{
    public class ValuesController : Controller
    {

        private readonly Lazy<ConnectionMultiplexer> lazyConnection;
        private ConnectionMultiplexer Connection => lazyConnection.Value;
        private IDatabase Database => Connection.GetDatabase();
        private readonly RedisAppContext dbContext;
        private readonly IServer server;

        public ValuesController(RedisAppContext dbContext)
        {
            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { "localhost" },
                ConnectTimeout = 60000,
                ResponseTimeout = 60000,
                SyncTimeout = 60000
            };

            lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configurationOptions));
            this.dbContext = dbContext;
            server = lazyConnection.Value.GetServer("localhost:6379");
        }

        [Route("example1")]
        [HttpGet]
        public IEnumerable<string> Example1()
        {
            var results = new List<string> { "Get country from which most of orders is made" };

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var sqlCountry = dbContext.Orders.GroupBy(o => o.Country)
                                   .OrderByDescending(g => g.Count())
                                   .Select(g => g.Key)
                                   .First();

            stopWatch.Stop();
            var sqlTime = stopWatch.Elapsed;
            results.Add($"SQL result: Country: {sqlCountry} Time: {sqlTime}");

            stopWatch.Restart();

            var keys = server.Keys(0, "order_*").ToArray();
            var orders = Database.StringGet(keys).Select(o => JsonConvert.DeserializeObject<Domain.Entities.Order>(o));

            var redisCountry = orders.GroupBy(o => o.Country)
                                     .OrderByDescending(g => g.Count())
                                     .Select(g => g.Key)
                                     .First();

            var redisTime = stopWatch.Elapsed;
            results.Add($"Redis result: Country: {redisCountry} Time: {redisTime}");

            results.Add($"SQL faster {redisTime.TotalMilliseconds / sqlTime.TotalMilliseconds} times");

            return results;
        }

        [Route("example2")]
        [HttpGet]
        public IEnumerable<string> Example2()
        {
            var results = new List<string> { "Get most frequently bought product name" };

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var products = dbContext.Products.Include(p => p.OrderItems)
                                    .OrderByDescending(p => p.OrderItems.Sum(oi => oi.Quantity))
                                    .Select(p => new { p.Name, sum = p.OrderItems.Sum(oi => oi.Quantity) });
            var sqlProductName = dbContext.Products
                                          .Include(p => p.OrderItems)
                                          .OrderByDescending(p => p.OrderItems.Sum(oi => oi.Quantity))
                                          .Select(p => p.Name)
                                          .First();

            stopWatch.Stop();
            var sqlTime = stopWatch.Elapsed;
            results.Add($"SQL result: Product name: {sqlProductName} Time: {sqlTime}");

            stopWatch.Restart();

            var keys = server.Keys(0, "order_*").ToArray();
            var orders = Database.StringGet(keys).Select(o => JsonConvert.DeserializeObject<Domain.Entities.Order>(o));

            var orderItems = orders.SelectMany(o => o.OrderItems).Distinct().GroupBy(oi => oi.Product);
            var redisProductName = orderItems.OrderByDescending(g => g.Sum(oi => oi.Quantity)).First().Key.Name;

            var redisTime = stopWatch.Elapsed;
            results.Add($"Redis result: Product Name: {redisProductName} Time: {redisTime}");

            results.Add($"SQL faster {redisTime.TotalMilliseconds / sqlTime.TotalMilliseconds} times");

            return results;
        }

        [Route("example3")]
        [HttpGet]
        public IEnumerable<string> Example3()
        {
            var results = new List<string> { "Getting first 1000 orders with all related products/producers/order items and categories" };

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var sqlItems = dbContext.Orders
                                    .Include(o => o.OrderItems)
                                    .ThenInclude(i => i.Product)
                                    .ThenInclude(p => p.Producer)
                                    .Include(o => o.OrderItems)
                                    .ThenInclude(i => i.Product)
                                    .ThenInclude(p => p.Category)
                                    .Take(1000)
                                    .ToList();

            stopWatch.Stop();
            var sqlTime = stopWatch.Elapsed;
            results.Add($"SQL result: Time: {sqlTime}");

            stopWatch.Restart();

            var keys = server.Keys(0, "order_*").Take(1000).ToArray();
            var orders = Database.StringGet(keys).Select(o => JsonConvert.DeserializeObject<Domain.Entities.Order>(o));

            var redisTime = stopWatch.Elapsed;
            results.Add($"Redis result: Time: {redisTime}");

            results.Add($"Redis faster {sqlTime.TotalMilliseconds / redisTime.TotalMilliseconds} times");

            return results;
        }

        [Route("example4")]
        [HttpGet]
        public IEnumerable<string> Example4()
        {
            var results = new List<string> { "Getting first 50000 orders with all related products/producers/order items and categories" };

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var sqlItems = dbContext.Orders
                                    .Include(o => o.OrderItems)
                                    .ThenInclude(i => i.Product)
                                    .ThenInclude(p => p.Producer)
                                    .Include(o => o.OrderItems)
                                    .ThenInclude(i => i.Product)
                                    .ThenInclude(p => p.Category)
                                    .Take(50000)
                                    .ToList();

            stopWatch.Stop();
            var sqlTime = stopWatch.Elapsed;
            results.Add($"SQL result: Time: {sqlTime}");

            stopWatch.Restart();

            var keys = server.Keys(0, "order_*").Take(50000).ToArray();
            var orders = Database.StringGet(keys).Select(o => JsonConvert.DeserializeObject<Domain.Entities.Order>(o));

            var redisTime = stopWatch.Elapsed;
            results.Add($"Redis result: Time: {redisTime}");

            results.Add($"Redis faster {sqlTime.TotalMilliseconds / redisTime.TotalMilliseconds} times");

            return results;
        }
    }
}
