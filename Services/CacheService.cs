using FormsNet.Services.IServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormsNet.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private IConfiguration _config;
        private bool IsUseMemoryCache;

        public CacheService(IConfiguration config, IMemoryCache cache)
        {
            _cache = cache;
            _config = config;

            IsUseMemoryCache = true;
            bool.TryParse(_config["IsUseMemoryCache"], out IsUseMemoryCache);
        }
        public byte[] Get(string key)
        {
            if (IsUseMemoryCache)
            {
                try
                {
                    return _cache.Get<byte[]>(key);
                }
                catch (Exception e)
                {
                    //logModel.LogServerData($"{e.Message}, {e.StackTrace}");
                }
            }

            return null;
        }
        public T Get<T>(string key)
        {
            var cacheJson = Get(key);
            if (cacheJson == null) return default;

            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(cacheJson));
        }

        public void Remove(string key)
        {
            if (IsUseMemoryCache)
            {
                try
                {
                    _cache.Remove(key);
                }
                catch (Exception e)
                {
                    //logModel.LogServerData($"{e.Message}, {e.StackTrace}");
                }
            }
        }

        public void Set(string key, byte[] value, MemoryCacheEntryOptions options)
        {
            if (IsUseMemoryCache)
            {
                try
                {
                    _cache.Set(key, value, options);
                }
                catch (Exception e)
                {
                    //logModel.LogServerData($"{e.Message}, {e.StackTrace}");
                }
            }
        }

        public void Set(string key, object _object, TimeSpan timeSpan)
        {
            Set(key, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(_object)),
                            new MemoryCacheEntryOptions().SetAbsoluteExpiration(timeSpan));
        }

    }
}
