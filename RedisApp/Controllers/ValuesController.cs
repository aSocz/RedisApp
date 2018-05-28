using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RedisApp.Infrastructure;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Order = RedisApp.Domain.Entities.Order;

namespace RedisApp.Controllers
{
    public class ValuesController : Controller
    {
        private readonly RedisAppContext dbContext;

        private readonly Lazy<ConnectionMultiplexer> lazyConnection;
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

        private ConnectionMultiplexer Connection => lazyConnection.Value;
        private IDatabase Database => Connection.GetDatabase();

        [Route("example1"), HttpGet]
        public IEnumerable<string> Example1()
        {
            var results = new List<string> { "Getting country from which most of orders are made" };

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var sqlCountry = dbContext.Orders.GroupBy(o => o.Country)
                                      .OrderByDescending(g => g.Count())
                                      .Select(g => g.Key)
                                      .First();

            stopWatch.Stop();
            var sqlTime = stopWatch.Elapsed;
            results.Add($"SQL result: Time: {sqlTime}");

            stopWatch.Restart();

            var keys = server.Keys(0, "order_*").ToArray();
            var orders = Database.StringGet(keys).Select(o => JsonConvert.DeserializeObject<Order>(o));

            var redisCountry = orders.GroupBy(o => o.Country)
                                     .OrderByDescending(g => g.Count())
                                     .Select(g => g.Key)
                                     .First();

            var redisTime = stopWatch.Elapsed;
            results.Add($"Redis result: Time: {redisTime}");

            results.Add($"SQL is faster {redisTime.TotalMilliseconds / sqlTime.TotalMilliseconds} times");

            return results;
        }

        [Route("example2"), HttpGet]
        public IEnumerable<string> Example2()
        {
            var results = new List<string> { "Getting name of most frequently bought product" };

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var sqlProductName = dbContext.Products
                                          .Include(p => p.OrderItems)
                                          .OrderByDescending(p => p.OrderItems.Sum(oi => oi.Quantity))
                                          .Select(p => p.Name)
                                          .First();

            stopWatch.Stop();
            var sqlTime = stopWatch.Elapsed;
            results.Add($"SQL result: Time: {sqlTime}");

            stopWatch.Restart();

            var keys = server.Keys(0, "order_*").ToArray();
            var orders = Database.StringGet(keys).Select(o => JsonConvert.DeserializeObject<Order>(o));

            var redisProductName = orders.SelectMany(o => o.OrderItems)
                                   .Select(oi => new { oi.Product.Name, oi.Quantity })
                                   .Distinct()
                                   .GroupBy(i => i.Name)
                                   .OrderByDescending(g => g.Sum(i => i.Quantity))
                                   .First()
                                   .Key;


            var redisTime = stopWatch.Elapsed;
            results.Add($"Redis result: Time: {redisTime}");

            results.Add($"SQL is faster {redisTime.TotalMilliseconds / sqlTime.TotalMilliseconds} times");

            return results;
        }

        [Route("example3"), HttpGet]
        public IEnumerable<string> Example3()
        {
            var results = new List<string>
            {
                "Getting first 1000 orders with all related products/producers/order items and categories"
            };

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
            var orders = Database.StringGet(keys).Select(o => JsonConvert.DeserializeObject<Order>(o));

            var redisTime = stopWatch.Elapsed;
            results.Add($"Redis result: Time: {redisTime}");

            results.Add($"Redis is faster {sqlTime.TotalMilliseconds / redisTime.TotalMilliseconds} times");

            return results;
        }

        [Route("example4"), HttpGet]
        public IEnumerable<string> Example4()
        {
            var results = new List<string>
            {
                "Getting first 50000 orders with all related products/producers/order items and categories"
            };

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
            var orders = Database.StringGet(keys).Select(o => JsonConvert.DeserializeObject<Order>(o));

            var redisTime = stopWatch.Elapsed;
            results.Add($"Redis result: Time: {redisTime}");

            results.Add($"Redis is faster {sqlTime.TotalMilliseconds / redisTime.TotalMilliseconds} times");

            return results;
        }

        [Route("example5"), HttpGet]
        public IEnumerable<string> Example5()
        {
            var results = new List<string>
            {
                "Getting first 10 orders with all related products/producers/order items and categories"
            };

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var sqlItems = dbContext.Orders
                                    .Include(o => o.OrderItems)
                                    .ThenInclude(i => i.Product)
                                    .ThenInclude(p => p.Producer)
                                    .Include(o => o.OrderItems)
                                    .ThenInclude(i => i.Product)
                                    .ThenInclude(p => p.Category)
                                    .Take(10)
                                    .ToList();

            stopWatch.Stop();
            var sqlTime = stopWatch.Elapsed;
            results.Add($"SQL result: Time: {sqlTime}");

            stopWatch.Restart();

            var keys = server.Keys(0, "order_*").Take(10).ToArray();
            var orders = Database.StringGet(keys).Select(o => JsonConvert.DeserializeObject<Order>(o));

            var redisTime = stopWatch.Elapsed;
            results.Add($"Redis result: Time: {redisTime}");

            results.Add($"Redis is faster {sqlTime.TotalMilliseconds / redisTime.TotalMilliseconds} times");

            return results;
        }

        [Route("example6"), HttpGet]
        public IEnumerable<string> Example6()
        {
            var results = new List<string>
            {
                "Getting count of orders"
            };

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var sqlCount = dbContext.Orders.Count();
            stopWatch.Stop();
            var sqlTime = stopWatch.Elapsed;
            results.Add($"SQL result: Count: {sqlCount} Time: {sqlTime}");

            stopWatch.Restart();

            var redisCount = server.Keys(0, "order_*").Count();

            var redisTime = stopWatch.Elapsed;
            results.Add($"Redis result: Count: {redisCount} Time: {redisTime}");

            results.Add($"SQL is faster {redisTime.TotalMilliseconds / sqlTime.TotalMilliseconds} times");

            return results;
        }

        [Route("example7"), HttpGet]
        public IEnumerable<string> Example7()
        {
            var results = new List<string>
            {
                "Orders grouped by product name"
            };

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var sqlProducers = dbContext.OrderItems
                                        .Include(i => i.Product)
                                        .GroupBy(oi => oi.Product.Name)
                                        .Select(g => g.Key)
                                        .ToList();

            stopWatch.Stop();
            var sqlTime = stopWatch.Elapsed;
            results.Add($"SQL result: Time: {sqlTime}");

            stopWatch.Restart();

            var keys = server.Keys(0, "order_*").ToArray();
            var orders = Database.StringGet(keys).Select(o => JsonConvert.DeserializeObject<Order>(o));
            var redisProducers = orders
                                .SelectMany(o => o.OrderItems)
                                .GroupBy(oi => oi.Product.Name)
                                .Select(g => g.Key)
                                .ToList();

            var redisTime = stopWatch.Elapsed;
            results.Add($"Redis result: Time: {redisTime}");

            results.Add($"Redis is faster {sqlTime.TotalMilliseconds / redisTime.TotalMilliseconds} times");

            return results;
        }

        [Route("example8"), HttpGet]
        public IEnumerable<string> Example8()
        {
            var results = new List<string>
            {
                "Orders grouped by producer name"
            };

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var sqlProducers = dbContext.OrderItems
                                        .Include(i => i.Product)
                                        .ThenInclude(i => i.Producer)
                                        .GroupBy(oi => oi.Product.Producer.Name)
                                        .Select(g => g.Key)
                                        .ToList();
            stopWatch.Stop();
            var sqlTime = stopWatch.Elapsed;
            results.Add($"SQL result: Time: {sqlTime}");

            stopWatch.Restart();

            var keys = server.Keys(0, "order_*").ToArray();
            var orders = Database.StringGet(keys).Select(o => JsonConvert.DeserializeObject<Order>(o));
            var redisProducers = orders
                                .SelectMany(o => o.OrderItems)
                                .GroupBy(oi => oi.Product.Producer.Name)
                                .Select(g => g.Key)
                                .ToList();

            var redisTime = stopWatch.Elapsed;
            results.Add($"Redis result: Time: {redisTime}");

            results.Add($"Redis is faster {sqlTime.TotalMilliseconds / redisTime.TotalMilliseconds} times");

            return results;
        }
    }
}