namespace RedisApp.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            //var orderLoader = new OrderLoader();
            //orderLoader.CreateOrderItems();
            var redisUploader = new RedisUploader();
            //redisUploader.UploadData();
            redisUploader.Test();
        }
    }
}
