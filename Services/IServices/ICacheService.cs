using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Services.IServices
{
    public interface ICacheService
    {
        byte[] Get(string key);
        T Get<T>(string key);
        void Set(string key, object _object, TimeSpan timeSpan);
        void Remove(string key);
    }
}
