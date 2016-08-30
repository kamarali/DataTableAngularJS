using System.Configuration;
using System.Data.Objects;
using System.Reflection;
using Iata.IS.Core.DI;
using Iata.IS.Model.Base;
using log4net;
using System.Data.Common;
using System.Data;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Impl
{
  /// <summary>
  /// Implements the <see cref="IObjectContext"/> in order to wrap an instance of <see cref="ObjectContext"/>.
  /// </summary>
  public class ObjectContextAdapter : IObjectContext
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private readonly ObjectContext _context;

    /// <summary>
    /// Gets Database connection.
    /// </summary>
    public DbConnection Connection
    {
      get
      {
        return _context.Connection;
      }
    }

    /// <summary>
    /// Creates an instance of the EF <see cref="ObjectContext"/>, which is wrapped by the <see cref="IObjectContext"/>.
    /// </summary>
    public ObjectContextAdapter()
    {
      _context = new ObjectContext(Core.Configuration.ConnectionString.Instance.ISDataContextContainer);
      _context.DefaultContainerName = "ISDataContextContainer";

    }

    /// <summary>
    /// Creates an <see cref="IObjectSet{TEntity}"/> instance for the specified E.
    /// </summary>
    /// <typeparam name="E">The entity for which the <see cref="IObjectSet{TEntity}"/> should be returned.</typeparam>
    /// <returns>Returns an <see cref="IObjectSet{TEntity}"/>.</returns>
    public ObjectSet<E> CreateObjectSet<E>() where E : ModelBase
    {
      return _context.CreateObjectSet<E>();
    }

    /// <summary>
    /// Creates an <see cref="IObjectSet{TEntity}"/> instance for the specified E.
    /// </summary>
    /// <typeparam name="E">The entity for which the <see cref="IObjectSet{TEntity}"/> should be returned.</typeparam>
    /// <returns>Returns an <see cref="IObjectSet{TEntity}"/>.</returns>
    public IObjectSet<E> CreateObjectSet<E>(string entityName) where E : ModelBase
    {
      return _context.CreateObjectSet<E>(entityName);
    }

    /// <summary>
    /// Saves the changes in the context.
    /// </summary>
    public void SaveChanges()
    {
      Logger.Debug(string.Format("About to save changes in the context."));

      // Save changes.
      var changesSaved = _context.SaveChanges();

      Logger.Debug(string.Format("Number of changes saved in the context: [{0}]", changesSaved));
    }

    public void Dispose()
    {
      if (_context != null)
      {
        _context.Dispose();
      }
    }

    public ObjectResult<T> ExecuteFunction<T>(string functionName, params ObjectParameter[] parameters)
    {
      return _context.ExecuteFunction<T>(functionName, parameters);
    }

    public int ExecuteFunction(string functionName, ObjectParameter[] parameters)
    {
      return _context.ExecuteFunction(functionName, parameters);
    }

    public DbCommand CreateStoreCommand(string commandText, CommandType commandType, params object[] parameters)
    {
      return _context.CreateStoreCommand(commandText, commandType, parameters);
    }

    public void Refresh<E>(E obj) where E : ModelBase
    {
      _context.Refresh(RefreshMode.StoreWins, obj);
    }
  }
}