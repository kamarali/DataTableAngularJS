using System;
using System.Data.Objects;
using System.Linq.Expressions;
using Iata.IS.Model.Base;
using System.Linq;

namespace Iata.IS.Data.Impl
{
  public abstract class RepositoryEx<E, B> : Repository<E>, IRepositoryEx<E,B>
    where B : ModelBase
    where E : B
  {

    /// <summary>
    /// Object query used for the Type E - Which is Derived Entity (MISC, PAX Invoice).
    /// </summary>
    protected ObjectQuery<E> EntityObjectQuery;

    /// <summary>
    /// Object set used for the  type B - Which is Base Type (Invoice Base).
    /// </summary>
    protected ObjectSet<B> EntityBaseObjectSet;

    /// <summary>
    /// Initializes the object set.
    /// </summary>
    public override abstract void InitializeObjectSet();
    //{
    //  //Will need to be overridden in Child Class.
    //}

    /// <summary>
    /// Adds the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public override void Add(E entity)
    {
      entity.LastUpdatedOn = DateTime.UtcNow;
      EntityBaseObjectSet.AddObject(entity);
    }

    /// <summary>
    /// Attaches the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public override void Attach(E entity)
    {
      EntityBaseObjectSet.Attach(entity);
    }

    // SCP249528: Changes done to improve performance of output
    /// <summary>
    /// Detaches the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public override void Detach(E entity)
    {
      EntityBaseObjectSet.Detach(entity);
    }
    /// <summary>
    /// Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    public override E Update(E entity)
    {
      entity.LastUpdatedOn = DateTime.UtcNow;
      EntityBaseObjectSet.ApplyCurrentValues(entity);
      return entity;
    }

    /// <summary>
    /// Deletes the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    public override void Delete(E entity)
    {
      entity.LastUpdatedOn = DateTime.UtcNow;
      EntityBaseObjectSet.DeleteObject(entity);
    }

    /// <summary>
    /// Gets all.
    /// </summary>
    /// <returns></returns>
    public override IQueryable<E> GetAll()
    {
      return EntityObjectQuery;
    }

    /// <summary>
    /// Gets the specified where.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public override IQueryable<E> Get(Expression<System.Func<E, bool>> where)
    {
      return EntityObjectQuery.Where(where);
    }

    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <returns></returns>
    public override long GetCount()
    {
      return EntityObjectQuery.LongCount();
    }

    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public override long GetCount(Expression<System.Func<E, bool>> where)
    {
      return EntityObjectQuery.LongCount(where);
    }

    /// <summary>
    /// Singles the specified where.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public override E Single(Expression<System.Func<E, bool>> where)
    {
      return EntityObjectQuery.SingleOrDefault(where);
    }

    /// <summary>
    /// Firsts the specified where.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public override E First(Expression<System.Func<E, bool>> where)
    {
      return EntityObjectQuery.FirstOrDefault(where);
    }

    /// <summary>
    /// Orders the by descending.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public override IQueryable<E> OrderByDescending(Expression<System.Func<E, bool>> where)
    {
      return EntityObjectQuery.OrderByDescending(where);
    }
  }
}