using System;
using System.Linq;
using System.Linq.Expressions;
using Iata.IS.Model.Base;

namespace Iata.IS.Data
{
  /// <summary>
  /// A generic repository that allows the client to perform any operation on an entity.
  /// </summary>
  /// <typeparam name="E">Any sub-type of <see cref="ModelBase"/> can be a valid entity for the generic repository.</typeparam>
  public interface IRepository<E> where E : ModelBase
  {
    /// <summary>
    /// Adds a new entity to the context.
    ///  Note: The entity is not added to the database unless the context is saved.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    void Add(E entity);

    /// <summary>
    /// Attaches an existing entity to the context. The assumption is that the entity being attached has a primary key.
    /// </summary>
    /// <param name="entity">The entity to be attached to the context. After attaching the entity is a candidate for change tracking.</param>
    void Attach(E entity);

    // SCP249528: Changes done to improve performance of output
    /// <summary>
    /// Detaches the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Detach(E entity);
    /// <summary>
    /// Updates an existing entity in the context.
    /// </summary>
    /// <param name="entity">The entity to be updated to the context.</param>
    E Update(E entity);
    
    /// <summary>
    /// Deletes an entity from the context.
    /// Note: The entity is not deleted from the database unless the context is saved.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    void Delete(E entity);

    /// <summary>
    /// Retrieves an unfiltered list of type E.
    /// </summary>
    /// <returns>Returns a list of records of type E.</returns>
    IQueryable<E> GetAll();

    /// <summary>
    /// Retrieves an unfiltered list of type E.
    /// </summary>
    /// <returns>Returns a list of records of type E.</returns>
    IQueryable<E> Get(Expression<Func<E, bool>> where);

    /// <summary>
    /// Gets the count of all records of type E.
    /// </summary>
    /// <returns>Returns the count of all records of type E.</returns>
    long GetCount();

    /// <summary>
    /// Gets the count of records of type E filtered by the where clause.
    /// </summary>
    /// <param name="where">The predicate to be used for filtering the records.</param>
    /// <returns>Returns the count of records matching the where clause.</returns>
    long GetCount(Expression<Func<E, bool>> where);

    /// <summary>
    /// Gets a single instance of the record type based on the where clause.
    /// </summary>
    /// <param name="where">The predicate to be used for filtering the records.</param>
    /// <returns>Returns a single entity matching the where clause.</returns>
    E Single(Expression<Func<E, bool>> where);

    /// <summary>
    /// Gets the first instance of the record type based on the where clause.
    /// </summary>
    /// <param name="where">The predicate to be used for filtering the records.</param>
    /// <returns>Returns the first entity matching the where clause.</returns>
    E First(Expression<Func<E, bool>> where);

    /// <summary>
    /// Retrieves list of type E in descending order.
    /// </summary>
    /// <param name="where"></param>
    /// <returns>Returns a list of records of type E.</returns>
    IQueryable<E> OrderByDescending(Expression<Func<E, bool>> where);

    void Refresh(E obj);

    /* Method for disposing context object */
    void DisposeContext();

  }
}