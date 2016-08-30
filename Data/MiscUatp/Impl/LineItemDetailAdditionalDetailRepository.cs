using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
    public class LineItemDetailAdditionalDetailRepository : IRepository<LineItemDetailAdditionalDetail>, ILineItemDetailAdditionalDetailRepository
    {
        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static List<LineItemDetailAdditionalDetail> LoadEntities(ObjectSet<LineItemDetailAdditionalDetail> objectSet, LoadStrategyResult loadStrategyResult, Action<LineItemDetailAdditionalDetail> link, string entityName)
        {
            if (link == null)
                link = new Action<LineItemDetailAdditionalDetail>(c => { });

            var lineItemDetailAdditionalDetails = new List<LineItemDetailAdditionalDetail>();
            var muMaterializers = new MuMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                lineItemDetailAdditionalDetails = muMaterializers.LineItemDetailAdditionalDetailMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }
            return lineItemDetailAdditionalDetails;
        }

        #region Implementation of IRepository<LineItemDetailAdditionalDetail>

        /// <summary>
        /// Adds a new entity to the context.
        ///  Note: The entity is not added to the database unless the context is saved.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        public void Add(LineItemDetailAdditionalDetail entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attaches an existing entity to the context. The assumption is that the entity being attached has a primary key.
        /// </summary>
        /// <param name="entity">The entity to be attached to the context. After attaching the entity is a candidate for change tracking.</param>
        public void Attach(LineItemDetailAdditionalDetail entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates an existing entity in the context.
        /// </summary>
        /// <param name="entity">The entity to be updated to the context.</param>
        public LineItemDetailAdditionalDetail Update(LineItemDetailAdditionalDetail entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes an entity from the context.
        /// Note: The entity is not deleted from the database unless the context is saved.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        public void Delete(LineItemDetailAdditionalDetail entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves an unfiltered list of type E.
        /// </summary>
        /// <returns>Returns a list of records of type E.</returns>
        public IQueryable<LineItemDetailAdditionalDetail> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves an unfiltered list of type E.
        /// </summary>
        /// <returns>Returns a list of records of type E.</returns>
        public IQueryable<LineItemDetailAdditionalDetail> Get(Expression<Func<LineItemDetailAdditionalDetail, bool>> where)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the count of all records of type E.
        /// </summary>
        /// <returns>Returns the count of all records of type E.</returns>
        public long GetCount()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the count of records of type E filtered by the where clause.
        /// </summary>
        /// <param name="where">The predicate to be used for filtering the records.</param>
        /// <returns>Returns the count of records matching the where clause.</returns>
        public long GetCount(Expression<Func<LineItemDetailAdditionalDetail, bool>> where)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a single instance of the record type based on the where clause.
        /// </summary>
        /// <param name="where">The predicate to be used for filtering the records.</param>
        /// <returns>Returns a single entity matching the where clause.</returns>
        public LineItemDetailAdditionalDetail Single(Expression<Func<LineItemDetailAdditionalDetail, bool>> where)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the first instance of the record type based on the where clause.
        /// </summary>
        /// <param name="where">The predicate to be used for filtering the records.</param>
        /// <returns>Returns the first entity matching the where clause.</returns>
        public LineItemDetailAdditionalDetail First(Expression<Func<LineItemDetailAdditionalDetail, bool>> where)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves list of type E in descending order.
        /// </summary>
        /// <param name="where"></param>
        /// <returns>Returns a list of records of type E.</returns>
        public IQueryable<LineItemDetailAdditionalDetail> OrderByDescending(Expression<Func<LineItemDetailAdditionalDetail, bool>> where)
        {
            throw new NotImplementedException();
        }

        public void Refresh(LineItemDetailAdditionalDetail obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Dummy method implemented to avoid compile time error
        /// </summary>
        public void DisposeContext()
        {

        }
        public void Detach(LineItemDetailAdditionalDetail entity)
        {

        }
    }
}
