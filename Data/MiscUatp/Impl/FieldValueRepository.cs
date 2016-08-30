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
    public class FieldValueRepository : IRepository<FieldValue>, IFieldValueRepository
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
        public static List<FieldValue> LoadEntities(ObjectSet<FieldValue> objectSet, LoadStrategyResult loadStrategyResult, Action<FieldValue> link, string entityName)
        {
            if (link == null)
                link = new Action<FieldValue>(c => { });

            var fieldValues = new List<FieldValue>();
            var muMaterializers = new MuMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                fieldValues = muMaterializers.FieldValueMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }

            //Load LIDetFieldValuesFieldMetaData by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LIDetFieldValuesFieldMetaData) && fieldValues.Count != 0)
            {
                FieldMetaDataRepository.LoadEntities(objectSet.Context.CreateObjectSet<FieldMetaData>(), loadStrategyResult, null, LoadStrategy.MiscEntities.LIDetFieldValuesFieldMetaData);
                //The fetched child records should use the Parent entities.
            }

            //Load LineItemDetailFieldValueAttrValue //TODO: Check this 
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemDetailFieldValueAttrValue) && fieldValues.Count != 0)
            {
                using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.LineItemDetailFieldValueAttrValue))
                {
                    // first result set includes the category
                    muMaterializers.FieldValueMaterializer.Materialize(reader).Bind(objectSet).ToList();
                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            //Load LineItemDetailFieldValueParentValue//TODO: Check this 
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemDetailFieldValueParentValue) && fieldValues.Count != 0)
            {
                using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.LineItemDetailFieldValueParentValue))
                {
                    // first result set includes the category
                    muMaterializers.FieldValueMaterializer.Materialize(reader).Bind(objectSet).ToList();
                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            return fieldValues;
        }

        #region Implementation of IRepository<FieldValue>

        /// <summary>
        /// Adds a new entity to the context.
        ///  Note: The entity is not added to the database unless the context is saved.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        public void Add(FieldValue entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attaches an existing entity to the context. The assumption is that the entity being attached has a primary key.
        /// </summary>
        /// <param name="entity">The entity to be attached to the context. After attaching the entity is a candidate for change tracking.</param>
        public void Attach(FieldValue entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates an existing entity in the context.
        /// </summary>
        /// <param name="entity">The entity to be updated to the context.</param>
        public FieldValue Update(FieldValue entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes an entity from the context.
        /// Note: The entity is not deleted from the database unless the context is saved.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        public void Delete(FieldValue entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves an unfiltered list of type E.
        /// </summary>
        /// <returns>Returns a list of records of type E.</returns>
        public IQueryable<FieldValue> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves an unfiltered list of type E.
        /// </summary>
        /// <returns>Returns a list of records of type E.</returns>
        public IQueryable<FieldValue> Get(Expression<Func<FieldValue, bool>> where)
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
        public long GetCount(Expression<Func<FieldValue, bool>> where)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a single instance of the record type based on the where clause.
        /// </summary>
        /// <param name="where">The predicate to be used for filtering the records.</param>
        /// <returns>Returns a single entity matching the where clause.</returns>
        public FieldValue Single(Expression<Func<FieldValue, bool>> where)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the first instance of the record type based on the where clause.
        /// </summary>
        /// <param name="where">The predicate to be used for filtering the records.</param>
        /// <returns>Returns the first entity matching the where clause.</returns>
        public FieldValue First(Expression<Func<FieldValue, bool>> where)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves list of type E in descending order.
        /// </summary>
        /// <param name="where"></param>
        /// <returns>Returns a list of records of type E.</returns>
        public IQueryable<FieldValue> OrderByDescending(Expression<Func<FieldValue, bool>> where)
        {
            throw new NotImplementedException();
        }

        public void Refresh(FieldValue obj)
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
        public void Detach(FieldValue entity)
        {

        }
    }
}
