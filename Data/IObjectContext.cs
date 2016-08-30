using System;
using System.Data.Objects;
using Devart.Data.Oracle;
using Iata.IS.Model.Base;
using System.Data.Common;
using System.Data;

namespace Iata.IS.Data
{
  /// <summary>
  /// Wraps an <see cref="ObjectContext"/> instance.
  /// </summary>
  public interface IObjectContext : IDisposable
  {
    /// <summary>
    /// Gets the connection.
    /// </summary>
    /// <value>The connection.</value>
    DbConnection Connection { get; }

    /// <summary>
    /// Create an object set with the help of the <see cref="ObjectContext"/>.
    /// </summary>
    /// <typeparam name="E">Any sub-type of <see cref="ModelBase"/> can be a valid entity for the generic repository.</typeparam>
    /// <returns>Returns an <see cref="IObjectSet{TEntity}"/> instance using the <see cref="ObjectContext"/>.</returns>
    ObjectSet<E> CreateObjectSet<E>() where E : ModelBase;

    /// <summary>
    /// Saves the changes for the <see cref="ObjectContext"/> that it wraps.
    /// </summary>
    void SaveChanges();

    /// <summary>
    /// Executes the function.
    /// </summary>
    /// <typeparam name="T">Type of entity whose collection will be returned through the specified function name.</typeparam>
    /// <param name="functionName">Name of the function.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>Collection of the records returned by the specified function.</returns>
    ObjectResult<T> ExecuteFunction<T>(string functionName, params ObjectParameter[] parameters);

    /// <summary>
    /// Executes the function.
    /// </summary>
    /// <param name="functionName">Name of the function.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>Number of rows affected by the specified function.</returns>
    int ExecuteFunction(string functionName, ObjectParameter[] parameters);

    DbCommand CreateStoreCommand(string commandText, CommandType commandType, params object[] parameters);

    void Refresh<E>(E obj) where E: ModelBase;

  }
}