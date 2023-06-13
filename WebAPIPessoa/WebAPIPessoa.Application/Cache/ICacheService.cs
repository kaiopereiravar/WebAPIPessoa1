using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Application.Cache
{
    public interface ICacheService
    {
        void Set<T>(string key, T value, int ttl);

        T Get<T>(string key);
    }
}
