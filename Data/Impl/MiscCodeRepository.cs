using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;

namespace Iata.IS.Data.Impl
{
  public class MiscCodeRepository : Repository<MiscCode>, IMiscCodeRepository
  {
    /// <summary>
    /// Deletes the specified entity.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    public override void Delete(MiscCode entity)
    {
      entity.LastUpdatedOn = DateTime.UtcNow;

      EntityObjectSet.DeleteObject(entity);

      // Remove specific MiscCode from cache
      CacheManager.Remove(GetCacheKey(entity.Group));
    }

    /// <summary>
    /// Not Implemented , Strictly use IMiscCodeRepository Functions.
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    public override IQueryable<MiscCode> Get(Expression<Func<MiscCode, bool>> where)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Not Implemented , Strictly use IMiscCodeRepository Functions.
    /// </summary>
    /// <returns></returns>
    public override long GetCount()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Not Implemented , Strictly use IMiscCodeRepository Functions.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public override long GetCount(Expression<Func<MiscCode, bool>> where)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Not Implemented , Strictly use IMiscCodeRepository Functions.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public override MiscCode Single(Expression<Func<MiscCode, bool>> where)
    {
      var miscCode = EntityObjectSet.Include("MiscCodeGroup").SingleOrDefault(where);
      return miscCode;
    }

    /// <summary>
    /// Not Implemented , Strictly use IMiscCodeRepository Functions.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public override MiscCode First(Expression<Func<MiscCode, bool>> where)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Not Implemented , Strictly use IMiscCodeRepository Functions.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public override IQueryable<MiscCode> OrderByDescending(Expression<Func<MiscCode, bool>> where)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Gets all.
    /// </summary>
    /// <returns></returns>
    public override IQueryable<MiscCode> GetAll()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public override MiscCode Update(MiscCode entity)
    {
      entity.LastUpdatedOn = DateTime.UtcNow;
      EntityObjectSet.ApplyCurrentValues(entity);

      // Remove specific MiscCode from cache
      CacheManager.Remove(GetCacheKey(entity.Group));

      return entity;
    }

    /// <summary>
    /// Gets the misc codes.
    /// </summary>
    public IList<MiscCode> GetMiscCodes(MiscGroups miscGroup)
    {
      var miscCodes = TryGetCachedCopy((int)miscGroup);
      return miscCodes.ToList();
    }

    /// <summary>
    /// Gets the misc code.
    /// </summary>
    public MiscCode GetMiscCode(MiscGroups miscGroup, string miscCodeName)
    {
      var miscCodes = TryGetCachedCopy((int)miscGroup);
      var miscCode = miscCodes.SingleOrDefault(rec => rec.Name == miscCodeName);

      return miscCode;
    }

    /// <summary>
    /// Tries the get cached copy.
    /// </summary>
    /// <param name="miscCodeGroup">The MiscCode id.</param>
    /// <returns></returns>
    private IList<MiscCode> TryGetCachedCopy(int miscCodeGroup)
    {
      try
      {
        //MiscCode Id is given or could retrieve from the IdNumericCode Map 
        //Then only try to fetch it from memcache
        var key = GetCacheKey(miscCodeGroup);

        // Try Retrieving it from MemCache.
        var miscCodes = (IList<MiscCode>)CacheManager.Get(key);

        // if MiscCode is not null then return it. 
        if (miscCodes != null)
        {
          return miscCodes;
        }

        // If MemCache don't have the requires cacheable entity.
        // Then retrieve it from DB; Update the MemCache.
        // CacheManager.Add(key, object);
        // Return db output as it is.
        miscCodes = EntityObjectSet.Where(rec => rec.Group == miscCodeGroup).ToList();

        // if MiscCode found in db - Update cache 
        if (miscCodes.Count() > 0)
        {
          key = GetCacheKey(miscCodeGroup);
          CacheManager.Add(key, miscCodes);
        }

        return miscCodes;
      }
      catch (InvalidOperationException invalidOperationException)
      {
        // This exception will occur if MiscCode id is invalid.
        Logger.Error("MiscCode id is invalid.", invalidOperationException);
      }
      catch (IOException ioException)
      {
        // If any exception from memCache then log it.
        Logger.Error("I/O exception", ioException);
      }

      return null;
    }

    /// <summary>
    /// Gets the cache key.
    /// </summary>
    /// <param name="memberId">The MiscCode id.</param>
    /// <returns></returns>
    private static string GetCacheKey(int memberId)
    {
      return (typeof(MiscCode).Name + "_" + memberId);
    }

    /// <summary>
    /// Gets all misc codes.
    /// </summary>
    /// <returns></returns>
    public IQueryable<MiscCode> GetAllMiscCodes()
    {
      var miscCodeList = EntityObjectSet.Include("MiscCodeGroup");
      
      return miscCodeList;
    }

    /// <summary>
    /// Gets the UOM Code type list from misc code
    /// </summary>
    public IList<MiscCode> GetUomCodeTypeList()
    {
        var miscCode = EntityObjectSet.Where(rec => rec.Group == (int)MiscGroups.UomCodeType);
        return miscCode.ToList();
    }
  }
}