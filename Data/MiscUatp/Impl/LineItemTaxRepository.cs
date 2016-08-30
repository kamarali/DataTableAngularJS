using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
    public class LineItemTaxRepository : IRepository<LineItemTax>, ILineItemTaxRepository
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
        public static List<LineItemTax> LoadEntities(ObjectSet<LineItemTax> objectSet, LoadStrategyResult loadStrategyResult, Action<LineItemTax> link, string entityName)
        {
            if (link == null)
                link = new Action<LineItemTax>(c => { });

            var lineItemTaxList = new List<LineItemTax>();
            var muMaterializers = new MuMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                lineItemTaxList = muMaterializers.LineItemTaxMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load LineItemTaxAdditionalDetails by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemTaxAdditionalDetails) && lineItemTaxList.Count != 0)
            {
                LineItemTaxAdditionalDetailRepository.LoadEntities(objectSet.Context.CreateObjectSet<LineItemTaxAdditionalDetail>(), loadStrategyResult, null);
                //The fetched child records should use the Parent entities.
            }

            //Load LineItemTaxCountry
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemTaxCountry) && lineItemTaxList.Count != 0)
            {
                CountryRepository.LoadEntities(objectSet.Context.CreateObjectSet<Country>(), loadStrategyResult, null, LoadStrategy.MiscEntities.LineItemTaxCountry);
            }

            return lineItemTaxList;
        }
        #region Implementation of IRepository<LineItemTax>

        /// <summary>
        /// Adds a new entity to the context.
        ///  Note: The entity is not added to the database unless the context is saved.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        public void Add(LineItemTax entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attaches an existing entity to the context. The assumption is that the entity being attached has a primary key.
        /// </summary>
        /// <param name="entity">The entity to be attached to the context. After attaching the entity is a candidate for change tracking.</param>
        public void Attach(LineItemTax entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates an existing entity in the context.
        /// </summary>
        /// <param name="entity">The entity to be updated to the context.</param>
        public LineItemTax Update(LineItemTax entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes an entity from the context.
        /// Note: The entity is not deleted from the database unless the context is saved.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        public void Delete(LineItemTax entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves an unfiltered list of type E.
        /// </summary>
        /// <returns>Returns a list of records of type E.</returns>
        public IQueryable<LineItemTax> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves an unfiltered list of type E.
        /// </summary>
        /// <returns>Returns a list of records of type E.</returns>
        public IQueryable<LineItemTax> Get(Expression<Func<LineItemTax, bool>> where)
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
        public long GetCount(Expression<Func<LineItemTax, bool>> where)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a single instance of the record type based on the where clause.
        /// </summary>
        /// <param name="where">The predicate to be used for filtering the records.</param>
        /// <returns>Returns a single entity matching the where clause.</returns>
        public LineItemTax Single(Expression<Func<LineItemTax, bool>> where)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the first instance of the record type based on the where clause.
        /// </summary>
        /// <param name="where">The predicate to be used for filtering the records.</param>
        /// <returns>Returns the first entity matching the where clause.</returns>
        public LineItemTax First(Expression<Func<LineItemTax, bool>> where)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves list of type E in descending order.
        /// </summary>
        /// <param name="where"></param>
        /// <returns>Returns a list of records of type E.</returns>
        public IQueryable<LineItemTax> OrderByDescending(Expression<Func<LineItemTax, bool>> where)
        {
            throw new NotImplementedException();
        }

        public void Refresh(LineItemTax obj)
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
        public void Detach(LineItemTax entity)
        {

        }
    }
}
