using Couchbase;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for MemCacheAdapter
/// </summary>
public class MemCachedAdapter : IWebCache
{
    // for Memcached product
    #region IWebCache Members

    private CouchbaseClient client = new CouchbaseClient();

    public void Remove(string key)
    {
        client.Remove(key);
    }

    public void Store(string key, object obj)
    {
        client.Store(StoreMode.Set, key, obj, TimeSpan.FromMinutes(1));
    }

    public T Retrieve<T>(string key)
    {
        return client.Get<T>(key);
    }

    #endregion
}