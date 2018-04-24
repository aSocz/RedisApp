using System;

namespace RedisApp.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var orderLoader = new OrderLoader();
            orderLoader.CreateOrderItems();
        }
    }
}
