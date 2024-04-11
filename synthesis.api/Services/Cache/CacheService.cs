
using System.Runtime.Caching;

namespace synthesis.api.Services.Cache;


public interface ICacheService
{

    T GetData<T>(string key);

    bool SetData<T>(string key, T value, DateTimeOffset expirationTime);

    object RemoveData(string key);
}

public class CacheService : ICacheService
{
    private ObjectCache _memoryCache = MemoryCache.Default;


    public T GetData<T>(string key)
    {
        try
        {
            T item = (T)_memoryCache.Get(key);

            return item;
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        var res = true;
        try
        {
            if (!string.IsNullOrEmpty(key))
                _memoryCache.Set(key, value, expirationTime);

            else
                res = false;

            return res;
        }
        catch (Exception e)
        {
            throw;
        }
    }
    public object RemoveData(string key)
    {
        var res = true;
        try
        {
            if (!string.IsNullOrEmpty(key))
            {
                var result = _memoryCache.Remove(key);
            }
            else
                res = false;
        }
        catch (Exception e)
        {
            throw;
        }
        return res;
    }
}