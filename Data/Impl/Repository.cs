using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Iata.IS.Core.DI;
using Iata.IS.Model;
using Iata.IS.Model.Base;
using Devart.Data.Oracle;
using Iata.IS.Model.Cargo.BillingHistory;
using Iata.IS.Model.Pax.BillingHistory;
using log4net;
using Microsoft.Data.Extensions;
using System.Data;

namespace Iata.IS.Data.Impl
{
  /// <summary>
  /// Generic repository implementation. Depends on a unit of work instance.
  /// </summary>
  /// <typeparam name="E">Any sub-type of <see cref="ModelBase"/> can be a valid entity for the generic repository.</typeparam>
  public class Repository<E> : IRepository<E> where E : ModelBase
  {
    private const string CommandTimeout = "CommandTimeout";

    /// <summary>
    /// context for Entity Framework.
    /// </summary>
    protected readonly IObjectContext Context;

    /// <summary>
    /// 
    /// </summary>
    protected readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// MemCache Manager Instance.
    /// </summary>
    protected readonly ICacheManager CacheManager;

    /// <summary>
    /// Set of Entities.
    /// </summary>
    protected ObjectSet<E> EntityObjectSet;

    /// <summary>
    /// Default constructor. Resolves the unit of work using the default container.
    /// </summary>
    public Repository()
    {
      var unitOfWork = Ioc.Resolve<IUnitOfWork>();

      // CacheManger resolved explicitly so that all the other calls for the repository will remain unaffected.
      CacheManager = Ioc.Resolve<ICacheManager>();

      Context = unitOfWork.Context;
      InitializeObjectSet();
    }

    /// <summary>
    /// Initializes the object set.
    /// </summary>
    public virtual void InitializeObjectSet()
    {
      EntityObjectSet = Context.CreateObjectSet<E>();
    }

    /// <summary>
    /// Constructor. Expects the unit of work to be injected.
    /// </summary>
    /// <param name="unitOfWork">Instance that hosts the shared context using which the unit of work will be saved.</param>
    public Repository(IUnitOfWork unitOfWork)
    {
      Context = unitOfWork.Context;

      // CacheManger resolved explicitly so that all the other calls for the repository will remain unaffected.
      CacheManager = Ioc.Resolve<ICacheManager>();

      InitializeObjectSet();
    }

    /// <summary>
    /// Adds the specified entity.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    public virtual void Add(E entity)
    {
      entity.LastUpdatedOn = DateTime.UtcNow;

      EntityObjectSet.AddObject(entity);

      // Remove call for E Type List from the MemCache. 
      // This is treated as flag for refreshing the MemCache as E Type Records in DB are getting updated.
      // MemCache will be refreshed when first call for any Select function i.e. Get,GetAll,Single,First will be given.
      // TODO: This is candidate for the optimization : Will be done later. 
      CacheManager.Remove<E>();
    }

    /// <summary>
    /// Attaches the specified entity.
    /// </summary>
    /// <param name="entity">The entity to be attached.</param>
    public virtual void Attach(E entity)
    {
      EntityObjectSet.Attach(entity);
      // Remove call for E Type List from the MemCache. 
      // This is treated as flag for refreshing the MemCache as E Type Records in DB are getting updated.
      // MemCache will be refreshed when first call for any Select function i.e. Get,GetAll,Single,First will be given.
      // TODO: This is candidate for the optimization : Will be done later.  
      CacheManager.Remove<E>();
    }

    // SCP249528: Changes done to improve performance of output
    /// <summary>
    /// Detaches the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public virtual void Detach(E entity)
    {
      EntityObjectSet.Detach(entity);
      // Remove call for E Type List from the MemCache. 
      // This is treated as flag for refreshing the MemCache as E Type Records in DB are getting updated.
      // MemCache will be refreshed when first call for any Select function i.e. Get,GetAll,Single,First will be given.
      // TODO: This is candidate for the optimization : Will be done later.  
      CacheManager.Remove<E>();
    }

    /// <summary>
    /// Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    /// <returns></returns>
    public virtual E Update(E entity)
    {
      entity.LastUpdatedOn = DateTime.UtcNow;

      EntityObjectSet.ApplyCurrentValues(entity);

      // Remove call for E Type List from the MemCache. 
      // This is treated as flag for refreshing the MemCache as E Type Records in DB are getting updated.
      // MemCache will be refreshed when first call for any Select function i.e. Get,GetAll,Single,First will be given.
      // TODO: This is candidate for the optimization : Will be done later. 
      CacheManager.Remove<E>();

      return entity;
    }

    /// <summary>
    /// Deletes the specified entity.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    public virtual void Delete(E entity)
    {
      entity.LastUpdatedOn = DateTime.UtcNow;

      EntityObjectSet.DeleteObject(entity);

      // Remove call for E Type List from the MemCache. 
      // This is treated as flag for refreshing the MemCache as E Type Records in DB are getting updated.
      // MemCache will be refreshed when first call for any Select function i.e. Get,GetAll,Single,First will be given.
      // TODO: This is candidate for the optimization : Will be done later. 
      CacheManager.Remove<E>();
    }

    /// <summary>
    /// Gets all.
    /// </summary>
    /// <returns>E Type collection.</returns>
    public virtual IQueryable<E> GetAll()
    {
      // Try fetching the Cacheable Entity List from the MemCache.
      var entityList = TryGetCachedCopy();

      // If the Entity List is not there in  MemCache then fetch it from DB.
      return entityList ?? EntityObjectSet;
    }

    /// <summary>
    /// Gets the specified where.
    /// </summary>
    /// <param name="where">Search Criteria.[Lambda Expression]</param>
    /// <returns>E Type collection.</returns>
    public virtual IQueryable<E> Get(Expression<Func<E, bool>> where)
    {
      // Try fetching the Cacheable Entity List from the MemCache.
      var entityList = TryGetCachedCopy();

      // If the Entity List is not there in  MemCache then fetch it from DB.
      return entityList != null ? entityList.Where(where) : EntityObjectSet.Where(where);
    }

    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <returns></returns>
    public virtual long GetCount()
    {
      // Try fetching the Cacheable Entity List from the MemCache.
      var entityList = TryGetCachedCopy();

      // If the Entity List is not there in  MemCache then fetch it from DB.
      return entityList != null ? entityList.LongCount() : EntityObjectSet.LongCount();
    }

    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <param name="where">Search Criteria.[Lambda Expression]</param>
    /// <returns>Number of record count for the given search criteria.</returns>
    public virtual long GetCount(Expression<Func<E, bool>> where)
    {
      // Try fetching the Cacheable Entity List from the MemCache.
      var entityList = TryGetCachedCopy();

      // If the Entity List is not there in  MemCache then fetch it from DB.
      return entityList != null ? entityList.LongCount(where) : EntityObjectSet.LongCount(where);
    }

    /// <summary>
    /// Singles the specified where.
    /// </summary>
    /// <param name="where">Search Criteria.[Lambda Expression]</param>
    /// <returns>Single Record matching the search criteria.</returns>
    public virtual E Single(Expression<Func<E, bool>> where)
    {
      // Try fetching the Cacheable Entity List from the MemCache.
      var entityList = TryGetCachedCopy();

      // If the Entity List is not there in  MemCache then fetch it from DB.
      return entityList != null ? entityList.SingleOrDefault(where) : EntityObjectSet.SingleOrDefault(where);
    }

    /// <summary>
    /// Firsts the specified where.
    /// </summary>
    /// <param name="where">Search Criteria.[Lambda Expression]</param>
    /// <returns>First Record matching the search criteria.</returns>
    public virtual E First(Expression<Func<E, bool>> where)
    {
      // Try fetching the Cacheable Entity List from the MemCache.
      var entityList = TryGetCachedCopy();

      // If the Entity List is not there in  MemCache then fetch it from DB.
      return entityList != null ? entityList.FirstOrDefault(where) : EntityObjectSet.FirstOrDefault(where);
    }

    /// <summary>
    /// Gets the cache copy.
    /// </summary>
    /// <returns></returns>
    private IQueryable<E> TryGetCachedCopy()
    {
      try
      {
        // Check weather Type E is cacheable.
        if (typeof(ICacheable).IsAssignableFrom(typeof(E)))
        {
          // Initial List.
          List<E> entityList;

          // Try Retrieving it from MemCache.
          entityList = CacheManager.Get<E>();

          // if entityList is not null then return it. 
          if (entityList != null)
          {
            return entityList.AsQueryable();
          }

          // If MemCache don't have the requires cacheable entity.
          // Then retrieve it from DB; Update the MemCache.
          CacheManager.Add(EntityObjectSet.ToList());

          // If caching is enabled then after caching return the entity list from cache to avoid next database call to get required entity.
          // Try Retrieving it from MemCache again.
          entityList = CacheManager.Get<E>();

          // if entityList is not null then return it. 
          if (entityList != null)
          {
            return entityList.AsQueryable();
          }
          
          // Return db output as it is.
          return EntityObjectSet;
        }
      }
      catch (IOException ioException)
      {
        // If any exception from memCache then log it.
        Logger.Error(ioException.Message, ioException);
      }
      catch (ObjectDisposedException objectDisposedException)
      {
        // ObjectDisposedException is thrown from memCache client, from Enyim.Caching.Memcached.DefaultNodeLocator.callback_isAliveTimer(System.Object)
        // log it.
        Logger.Error(objectDisposedException.Message, objectDisposedException);
      }
      return null;
    }

    /// <summary>
    /// Executes the load SP.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sp">The sp.</param>
    /// <param name="loadStrategy">The load strategy.</param>
    /// <param name="oraInputParameters">The Oracle input parameters.</param>
    /// <param name="fetch">The fetch.</param>
    /// <returns></returns>
    private T ExecuteLoadSP<T>(StoredProcedure sp, LoadStrategy loadStrategy, OracleParameter[] oraInputParameters, Func<LoadStrategyResult, T> fetch)
    {
        using (var result = new LoadStrategyResult())
        {
            using (var cmd = Context.CreateStoreCommand(sp.Name, CommandType.StoredProcedure) as OracleCommand)
            {
                try
                {
                    cmd.Parameters.AddRange(oraInputParameters);
                    cmd.Parameters.Add(new OracleParameter("LOAD_STRATEGY_I", loadStrategy.ToString()));

                    // Add result parameters to Oracle Parameter Collection
                    foreach (var resObj in sp.GetResultSpec())
                    {
                        var resultParam = new OracleParameter(resObj.ParameterName, OracleDbType.Cursor) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(resultParam);

                        // If the entity is requested, add it to the result
                        if (loadStrategy.EntityNames.Contains(resObj.EntityName) || loadStrategy.EntityNames.Contains(resObj.EntityName + "ISWeb") || resObj.IsMain)
                        {
                            result.Add(resObj.EntityName, resultParam);
                        }
                    }

                    using (cmd.Connection.CreateConnectionScope())
                    {
                        // Execute SP

                        // Set CommandTimeout value to value given in the config file 
                        // if it NOT in the config then it will be set to default value 0.
                        cmd.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings[CommandTimeout]);
                        
                        //Logger.InfoFormat("Execute {0} stored procedure - Start.", cmd.CommandText);

                        cmd.ExecuteNonQuery();

                        //Logger.InfoFormat("Execute {0} stored procedure - End.", cmd.CommandText);

                        // Allow the caller to populate results.
                        return fetch(result);
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("Error : ", ex);
                    Logger.InfoFormat("Connection State :{0} ", Enum.GetName(typeof(ConnectionState), cmd.Connection.State));
                    throw;
                }
            }
        }
    }

      /// <summary>
    /// Executes the loads SP.
    /// </summary>
    /// <param name="sp">The sp.</param>
    /// <param name="loadStrategy">The load strategy.</param>
    /// <param name="oraInputParameters">The Oracle input parameters.</param>
    /// <param name="fetch">The fetch.</param>
    protected List<E> ExecuteLoadsSP(StoredProcedure sp, LoadStrategy loadStrategy, OracleParameter[] oraInputParameters, Func<LoadStrategyResult, List<E>> fetch)
    {
      return ExecuteLoadSP(sp, loadStrategy, oraInputParameters, fetch);
    }

    /// <summary>
    /// Executes the loads SP.
    /// </summary>
    /// <param name="sp">The sp.</param>
    /// <param name="loadStrategy">The load strategy.</param>
    /// <param name="oraInputParameters">The Oracle input parameters.</param>
    /// <param name="fetch">The fetch.</param>
    /// <returns></returns>
    protected PaxAuditTrail ExecuteLoadsAuditSP(StoredProcedure sp, LoadStrategy loadStrategy, OracleParameter[] oraInputParameters, Func<LoadStrategyResult, PaxAuditTrail> fetch)
    {
      return ExecuteLoadSP(sp, loadStrategy, oraInputParameters, fetch);
    }


    /// <summary>
    /// Executes the loads SP.
    /// </summary>
    /// <param name="sp">The sp.</param>
    /// <param name="loadStrategy">The load strategy.</param>
    /// <param name="oraInputParameters">The Oracle input parameters.</param>
    /// <param name="fetch">The fetch.</param>
    /// <returns></returns>
    protected CargoAuditTrail ExecuteCargoLoadsAuditSP(StoredProcedure sp, LoadStrategy loadStrategy, OracleParameter[] oraInputParameters, Func<LoadStrategyResult, CargoAuditTrail> fetch)
    {
      return ExecuteLoadSP(sp, loadStrategy, oraInputParameters, fetch);
    }

    /// <summary>
    /// Executes the load SP.
    /// </summary>
    /// <param name="sp">The sp.</param>
    /// <param name="loadStrategy">The load strategy.</param>
    /// <param name="oraInputParameters">The Oracle input parameters.</param>
    /// <param name="fetch">The fetch.</param>
    /// <returns></returns>
    protected E ExecuteLoadSP(StoredProcedure sp, LoadStrategy loadStrategy, OracleParameter[] oraInputParameters, Func<LoadStrategyResult, E> fetch)
    {
      return ExecuteLoadSP<E>(sp, loadStrategy, oraInputParameters, fetch);
    }


    /// <summary>
    /// Executes the stored function.
    /// Use this method to call Stored Procedure / Function which returns list of records.
    /// </summary>
    /// <param name="storedFunctionName">Name of the stored function.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>List of Entity records fetched by execution of the function</returns>
    /// <remarks>
    /// The difference between <see cref="ExecuteStoredFunction"/> and <see cref="ExecuteStoredProcedure"/>
    /// is that ExecuteStoredFunction returns list of Entities, while ExecuteStoredProcedure will not return anything. 
    /// </remarks>
    protected List<E> ExecuteStoredFunction(string storedFunctionName, params ObjectParameter[] parameters)
    {
      var result = Context.ExecuteFunction<E>(storedFunctionName, parameters);
      return result.ToList();
    }

    /// <summary>
    /// Executes the stored function.
    /// Use this method to call Stored Procedure / Function which returns list of records
    /// which are other than Entity Type.
    /// </summary>
    /// <typeparam name="T">Type of Complex Entity object.</typeparam>
    /// <param name="storedFunctionName">Name of the stored function.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>
    /// List of Entity records fetched by execution of the function
    /// </returns>
    protected ObjectResult<T> ExecuteStoredFunction<T>(string storedFunctionName, params ObjectParameter[] parameters)
    {
      return Context.ExecuteFunction<T>(storedFunctionName, parameters);
    }

    /// <summary>
    /// Executes the stored procedure.
    /// Use this method to call Stored Procedure / Function which does not return list of records.
    /// The Stored Procedure / Function may return value through Out Parameter.
    /// </summary>
    /// <param name="storedProcedureName">Name of the stored procedure.</param>
    /// <param name="parameters">The parameters.</param>
    /// <remarks>
    /// The difference between <see cref="ExecuteStoredFunction"/> and <see cref="ExecuteStoredProcedure"/>
    /// is that ExecuteStoredFunction returns list of Entities, while ExecuteStoredProcedure will not return anything. 
    /// </remarks>
    protected void ExecuteStoredProcedure(string storedProcedureName, params ObjectParameter[] parameters)
    {
      // This function won't return any value. 
      // However, return values can be fetched through Out Parameters.
      var result = Context.ExecuteFunction(storedProcedureName, parameters);
    }

    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <param name="where">The where.</param>
    public virtual IQueryable<E> OrderByDescending(Expression<Func<E, bool>> where)
    {
      return EntityObjectSet.OrderByDescending(where);
    }

    public void Refresh(E obj)
    {
      Context.Refresh(obj);
    }

    /// <summary>
    /// Method for disposing context object
    /// </summary>
    public void DisposeContext()
    {
        try
        {
            Context.Dispose();
        }
        catch (Exception ex)
        {
            Logger.Info("Handled error disposing context object");
            Logger.Error(ex);
        }
    }
  
  }
}
