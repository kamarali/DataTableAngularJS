using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Enyim.Caching.Memcached;
using Iata.IS.Core.DI;
using Enyim.Caching;
using Iata.IS.Model.Base;

namespace Iata.IS.Data.Impl
{
  public class CacheManager : ICacheManager
  {
    /// <summary>
    /// MemCache Client Instance.
    /// </summary>
    private readonly IMemcachedClient _memCachedClient;

    public bool IsInitialized { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheManager"/> class.
    /// </summary>
    public CacheManager()
    {
      _memCachedClient = Ioc.Resolve<IMemcachedClient>(typeof(IMemcachedClient));
    }

    /// <summary>
    /// Adds the specified entity list.
    /// </summary>
    /// <typeparam name="E">Type for which cache copy to be added.</typeparam>
    /// <param name="entityList">The entity list.</param>
    public void Add<E>(List<E> entityList) where E : ModelBase
    {
      var key = typeof(E).Name;

      // Cache the value for the given key.
      _memCachedClient.Store(StoreMode.Set, key, entityList.ToList());

      // Update the list of keys that are cached.
      UpdateKeyList(key);
    }

    private void UpdateKeyList(string key, bool removeMode = false)
    {
      const string keyListKey = "KeyList";

      if (key == keyListKey)
      {
        return;
      }

      // Get the list of keys stored in the cache.
      var keysAlreadyStored = Get(keyListKey) as StringCollection;

      // If key not found, then add the list of keys.
      if (keysAlreadyStored == null) 
      {
        Add(keyListKey, new StringCollection { key });
      }
      else
      {
        // Check if the key being added is already contained in the list of keys.
        if (!keysAlreadyStored.Contains(key))
        {
          // Add if not found.
          keysAlreadyStored.Add(key);

          // Update the key list in the cache.
          Update(keyListKey, keysAlreadyStored);
        }
        else
        {
          // Key is to be removed.
          if (removeMode)
          {
            // Remove from the key collection.
            keysAlreadyStored.Remove(key);

            // Update the key list in the cache.
            Update(keyListKey, keysAlreadyStored);
          }
        }
      }
    }

    /// <summary>
    /// Removes the specified entity list.
    /// </summary>
    /// <typeparam name="E">Type for which cache copy to be removed.</typeparam>
    public void Remove<E>() where E : ModelBase
    {
      var key = typeof(E).Name;

      // Remove the value for the given key from the cache.
      _memCachedClient.Remove(key);

      // Update the list of keys that are cached.
    //  UpdateKeyList(key, true);
    }

    /// <summary>
    /// Updates the specified entity list.
    /// </summary>
    /// <typeparam name="E">Type for which cache copy to be updated.</typeparam>
    /// <param name="entityList">The entity list.</param>
    public void Update<E>(List<E> entityList) where E : ModelBase
    {
      var key = typeof(E).Name;

      // Update the value in the cache.
      _memCachedClient.Store(StoreMode.Set, key, entityList);

      // Update the list of keys that are cached.
      UpdateKeyList(key);
    }

    /// <summary>
    /// Updates the specified entity list.
    /// </summary>
    public void Update(string key, object value)
    {
      _memCachedClient.Store(StoreMode.Set, key, value);

      // Update the list of keys that are cached.
      UpdateKeyList(key);
    }

    /// <summary>
    /// Gets this instance.
    /// </summary>
    /// <typeparam name="E">Type for which cache copy to be retrieved.</typeparam>
    /// <returns></returns>
    public List<E> Get<E>() where E : ModelBase
    {
      return (List<E>)_memCachedClient.Get(typeof(E).Name);
    }

    /// <summary>
    /// Flushes all Data in Cache.
    /// </summary>
    public void FlushAll()
    {
      _memCachedClient.FlushAll();
    }

    /// <summary>
    /// Removes the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    public void Remove(string key)
    {
      object fetchCheck;
      if (_memCachedClient.TryGet(key, out fetchCheck))
      {
        _memCachedClient.Remove(key);

        // Update the list of keys that are cached.
       // UpdateKeyList(key, true);
      }
    }

    /// <summary>
    /// Gets the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public object Get(string key)
    {
      object fetchCheck;
      _memCachedClient.TryGet(key, out fetchCheck);

      //
      //ServerStats ms = _memCachedClient.Stats();
      //ms.GetValue(ServerStats.All,
      //)

      return fetchCheck;
    }

    /// <summary>
    /// Updates the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="entity">The entity.</param>
    /// <param name="storeMode">The store mode.</param>
    public void Update(string key, object entity, StoreMode storeMode)
    {
      _memCachedClient.Store(storeMode, key, entity);

      // Update the list of keys that are cached.
      UpdateKeyList(key);
    }

    /// <summary>
    /// Adds the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="entity">The entity.</param>
    public void Add(string key, object entity)
    {
      _memCachedClient.Store(StoreMode.Set, key, entity);

      // Update the list of keys that are cached.
      UpdateKeyList(key);
    }
  }
}