using StackExchange.Redis;

namespace synthesis.api.Services.Cache;

public class ConnectionHelper
{
    private static readonly string redisUrl = "redis-17173.c251.east-us-mz.azure.cloud.redislabs.com:17173,password=Jep6jr2pIXtfumyWxr5m87vWJe1LbvbV";
    static ConnectionHelper()
    {
        lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(redisUrl);
        });
    }
    private static Lazy<ConnectionMultiplexer> lazyConnection;
    public static ConnectionMultiplexer Connection
    {
        get
        {
            return lazyConnection.Value;
        }
    }
}