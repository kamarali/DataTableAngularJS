using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Iata.IS.Model;
using Iata.IS.Model.Base;

namespace Iata.IS.Data
{
  /// <summary>
  /// Handles the MemCache Related Functions.
  /// </summary>
  public interface ICacheManager
  {
    /// <summary>
    /// Adds the specified entity list.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <param name="entityList">The entity list.</param>
    void Add<E>(List<E> entityList) where E : ModelBase;

    /// <summary>
    /// Removes the specified entity list.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    void Remove<E>() where E : ModelBase;

    /// <summary>
    /// Updates the specified entity list.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <param name="entityList">The entity list.</param>
    void Update<E>(List<E> entityList) where E : ModelBase;

    /// <summary>
    /// Updates the key with the specified value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value for the key.</param>
    void Update(string key, object value);

    /// <summary>
    /// Gets this instance.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <returns></returns>
    List<E> Get<E>() where E : ModelBase;

    /// <summary>
    /// Flushes all.
    /// </summary>
    void FlushAll();

    /// <summary>
    /// Removes the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    void Remove(string key);

    /// <summary>
    /// Gets the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    object Get(string key);

    /// <summary>
    /// Updates the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="entityList">The entity list.</param>
    /// <param name="storeMode">The store mode.</param>
    void Update(string key, object entityList, StoreMode storeMode);

    /// <summary>
    /// Adds the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="entity">The entity.</param>
    void Add(string key, object entity);

  }
}