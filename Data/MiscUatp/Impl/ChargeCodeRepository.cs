using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Base;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;
using Iata.IS.Model.Master;

namespace Iata.IS.Data.MiscUatp.Impl
{
    public class ChargeCodeRepository : Repository<ChargeCode>, IChargeCodeRepository
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
        public static List<ChargeCode> LoadEntities(ObjectSet<ChargeCode> objectSet, LoadStrategyResult loadStrategyResult, Action<ChargeCode> link, string entityName)
        {
            if (link == null)
                link = new Action<ChargeCode>(c => { });

            var chargeCodes = new List<ChargeCode>();
            var muMaterializers = new MuMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(entityName))
            {
                // first result set includes the category
                chargeCodes = muMaterializers.ChargeCodeMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }

            // Load ChargeCategoryChargeCode if current entity is ChargeCategoryChargeCode
            if (entityName.CompareTo(LoadStrategy.MiscEntities.ChargeCategoryChargeCode) == 0 && loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.ChargeCategoryChargeCodeType) && chargeCodes.Count != 0)
            {
                ChargeCodeTypeRepository.LoadEntities(objectSet.Context.CreateObjectSet<ChargeCodeType>(), loadStrategyResult, null, LoadStrategy.MiscEntities.ChargeCategoryChargeCodeType);
            }

            //Load ChargeCodeType by calling respective LoadEntities method if current entity is LineItemChargeCode
            if (entityName.CompareTo(LoadStrategy.MiscEntities.LineItemChargeCode) == 0 && loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemChargeCodeType) && chargeCodes.Count != 0)
            {
                ChargeCodeTypeRepository.LoadEntities(objectSet.Context.CreateObjectSet<ChargeCodeType>(), loadStrategyResult, null, LoadStrategy.MiscEntities.LineItemChargeCodeType);
                //The fetched child records should use the Parent entities.
            }

            return chargeCodes;
        }

        //#region Implementation of IRepository<ChargeCode>

        ///// <summary>
        ///// Adds a new entity to the context.
        /////  Note: The entity is not added to the database unless the context is saved.
        ///// </summary>
        ///// <param name="entity">The entity to be added.</param>
        //public void Add(ChargeCode entity)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Attaches an existing entity to the context. The assumption is that the entity being attached has a primary key.
        ///// </summary>
        ///// <param name="entity">The entity to be attached to the context. After attaching the entity is a candidate for change tracking.</param>
        //public void Attach(ChargeCode entity)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Updates an existing entity in the context.
        ///// </summary>
        ///// <param name="entity">The entity to be updated to the context.</param>
        //public ChargeCode Update(ChargeCode entity)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Deletes an entity from the context.
        ///// Note: The entity is not deleted from the database unless the context is saved.
        ///// </summary>
        ///// <param name="entity">The entity to be deleted.</param>
        //public void Delete(ChargeCode entity)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Retrieves an unfiltered list of type E.
        ///// </summary>
        ///// <returns>Returns a list of records of type E.</returns>
        //public IQueryable<ChargeCode> GetAll()
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Retrieves an unfiltered list of type E.
        ///// </summary>
        ///// <returns>Returns a list of records of type E.</returns>
        //public IQueryable<ChargeCode> Get(Expression<Func<ChargeCode, bool>> where)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Gets the count of records of type E filtered by the where clause.
        ///// </summary>
        ///// <param name="where">The predicate to be used for filtering the records.</param>
        ///// <returns>Returns the count of records matching the where clause.</returns>
        //public long GetCount(Expression<Func<ChargeCode, bool>> where)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Gets a single instance of the record type based on the where clause.
        ///// </summary>
        ///// <param name="where">The predicate to be used for filtering the records.</param>
        ///// <returns>Returns a single entity matching the where clause.</returns>
        //public ChargeCode Single(Expression<Func<ChargeCode, bool>> where)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Gets the first instance of the record type based on the where clause.
        ///// </summary>
        ///// <param name="where">The predicate to be used for filtering the records.</param>
        ///// <returns>Returns the first entity matching the where clause.</returns>
        //public ChargeCode First(Expression<Func<ChargeCode, bool>> where)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Retrieves list of type E in descending order.
        ///// </summary>
        ///// <param name="where"></param>
        ///// <returns>Returns a list of records of type E.</returns>
        //public IQueryable<ChargeCode> OrderByDescending(Expression<Func<ChargeCode, bool>> where)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Refresh(ChargeCode obj)
        //{
        //    throw new NotImplementedException();
        //}

        //#endregion

        /// <summary>
        /// This function is used to get misc charge code based on charge category and charge code.
        /// </summary>
        /// <param name="chargeCategoryId"></param>
        /// <param name="chargeCodeId"></param>
        /// <returns></returns>
        //CMP #636: Standard Update Mobilization
        public List<ChargeCodeSearchData> GetMiscChargeCode(Int32 chargeCategoryId, Int32 chargeCodeId)
        {
          var parameters = new ObjectParameter[2];

          parameters[0] = new ObjectParameter("CHARGE_CATEGORY_ID_I", typeof(int)) { Value = chargeCategoryId };
          parameters[1] = new ObjectParameter("CHARGE_CODE_ID_I", typeof(int)) { Value = chargeCodeId };

          //Execute stored procedure and fetch data based on criteria.
          var miscChargeCode = ExecuteStoredFunction<ChargeCodeSearchData>("GetMiscChargeCode", parameters);

          return miscChargeCode.ToList();
        }
    }
}
