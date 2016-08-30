using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
    public class LineItemRepository : Repository<LineItem>, ILineItemRepository
    {

        public override IQueryable<LineItem> Get(Expression<Func<LineItem, bool>> where)
        {
            var lineItems = EntityObjectSet
              .Include("ChargeCode")
              .Include("UomCode")
              .Include("ChargeCodeType")
              .Include("LineItemDetails.FieldValues.FieldMetaData")
              .Where(where);
            return lineItems;
        }

        public override LineItem Single(Expression<Func<LineItem, bool>> where)
        {
            throw new NotImplementedException("Use overloaded Single instead.");
        }

        /// <summary>
        /// Get all LineItems for InvoiceId
        /// </summary>
        /// <param name="invoiceId">The Invoice Id</param>
        /// <returns>List of LineItems for input Invoice Id</returns>
        public List<LineItem> Get(Guid invoiceId)
        {
            var entities = new[] 
                        { 
                            LoadStrategy.MiscEntities.LineItem,LoadStrategy.MiscEntities.LineItemChargeCode,
                            LoadStrategy.MiscEntities.LineItemUomCode,LoadStrategy.MiscEntities.LineItemChargeCodeType
                            //SCP334966 - SRM: Invoice takes too long to display line items 
                            // removed extrs LS call
                            //LoadStrategy.MiscEntities.LineItemDetails,LoadStrategy.MiscEntities.LineItemDetailsFieldValues,
                            //LoadStrategy.MiscEntities.LIDetFieldValuesFieldMetaData, LoadStrategy.MiscEntities.LineItemDetailAddOnCharges
                        };
            return GetLineItemsLS(new LoadStrategy(string.Join(",", entities)), invoiceId);
        }

        /// <summary>
        /// Singles the specified invoice id.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="lineItemId">The line item id.</param>
        /// <returns></returns>
        public LineItem Single(Guid? invoiceId = null, Guid? lineItemId = null)
        {
            var entities = new[]
                         {
                           LoadStrategy.MiscEntities.LineItem, LoadStrategy.MiscEntities.LineItemChargeCode, LoadStrategy.MiscEntities.LineItemUomCode, LoadStrategy.MiscEntities.LineItemChargeCodeType
                           , LoadStrategy.MiscEntities.LineItemDetails, LoadStrategy.MiscEntities.LineItemTaxBreakdown, LoadStrategy.MiscEntities.LineItemAddOnCharges,
                           LoadStrategy.MiscEntities.LineItemAdditionalDetails
                         };
            var lineItems = GetLineItemsLS(new LoadStrategy(string.Join(",", entities)), invoiceId, lineItemId);
            if (lineItems == null || lineItems.Count == 0) return null;
            if (lineItems.Count > 1) throw new ApplicationException("Multiple records found");
            return lineItems[0];
        }

        /// <summary>
        /// Gets the line items LS.
        /// </summary>
        /// <param name="loadStrategy">The load strategy.</param>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="lineItemId">The line item id.</param>
        /// <returns></returns>
        public List<LineItem> GetLineItemsLS(LoadStrategy loadStrategy, Guid? invoiceId = null, Guid? lineItemId = null)
        {
            if (invoiceId == null && lineItemId == null) throw new ArgumentNullException("Both invoiceId and lineItemId are missing");
            return base.ExecuteLoadsSP(SisStoredProcedures.GetLineItem,
                                       loadStrategy,
                                       new[]
                                         {
                                             new OracleParameter(LineItemRepositoryConstants.InvoiceIdParameterName,
                                                                 invoiceId.HasValue ? ConvertUtil.ConvertGuidToString(invoiceId.Value) : null),
                                             new OracleParameter(LineItemRepositoryConstants.LineItemIdParameterName,
                                                                 lineItemId.HasValue ? ConvertUtil.ConvertGuidToString(lineItemId.Value) : null),
                                             new OracleParameter(LineItemRepositoryConstants.LineItemDetailIdParameterName, null),
                                             new OracleParameter(LineItemRepositoryConstants.LineItemDetailNoParameterName, null)
                                         },
                                       r => this.FetchRecord(r));


        }

        private List<LineItem> FetchRecord(LoadStrategyResult loadStrategyResult)
        {
            var lineItems = new List<LineItem>();
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItem))
            {
                lineItems = LoadEntities(EntityObjectSet, loadStrategyResult, null);
            }
            return lineItems;
        }


    public LineItem GetLineItemHeaderInformation(Guid lineItemId)
    {
      var lineItem =
        EntityObjectSet
        .Include("ChargeCode")
        //.Include("UomCode")
        .Include("ChargeCodeType")
        .SingleOrDefault(li => li.Id == lineItemId);

            return lineItem;
        }

        public int GetMaxLineItemNumber(Guid invoiceId)
        {
            return EntityObjectSet
                  .Where(li => li.InvoiceId == invoiceId)
                  .Max(li => li.LineItemNumber);
        }

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<LineItem> LoadEntities(ObjectSet<LineItem> objectSet, LoadStrategyResult loadStrategyResult, Action<LineItem> link)
        {
            if (link == null)
                link = new Action<LineItem>(c => { });

            var lineItems = new List<LineItem>();

            var muMaterializers = new MuMaterializers();
            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.LineItem))
            {
                // first result set includes the category
                lineItems = muMaterializers.LineItemMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }
            //Load ChargeCode by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemChargeCode) && lineItems.Count != 0)
            {
                ChargeCodeRepository.LoadEntities(objectSet.Context.CreateObjectSet<ChargeCode>(), loadStrategyResult, null, LoadStrategy.MiscEntities.LineItemChargeCode);
                //The fetched child records should use the Parent entities.
            }

            //Load UomCode by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemUomCode) && lineItems.Count != 0)
            {
                UomCodeRepository.LoadEntities(objectSet.Context.CreateObjectSet<UomCode>(), loadStrategyResult, null, LoadStrategy.MiscEntities.LineItemUomCode);
                //The fetched child records should use the Parent entities.
            }

            //Load ChargeCodeType by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemTaxBreakdown) && lineItems.Count != 0)
            {
                LineItemTaxRepository.LoadEntities(objectSet.Context.CreateObjectSet<LineItemTax>(), loadStrategyResult, null, LoadStrategy.MiscEntities.LineItemTaxBreakdown);
                //The fetched child records should use the Parent entities.
            }

            //Load ChargeCodeType by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemAddOnCharges) && lineItems.Count != 0)
            {
                LineItemAddOnChargeRepository.LoadEntities(objectSet.Context.CreateObjectSet<LineItemAddOnCharge>(), loadStrategyResult, null, LoadStrategy.MiscEntities.LineItemAddOnCharges);
                //The fetched child records should use the Parent entities.
            }

            //Load ChargeCodeType by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemAdditionalDetails) && lineItems.Count != 0)
            {
                LineItemAdditionalDetailRepository.LoadEntities(objectSet.Context.CreateObjectSet<LineItemAdditionalDetail>(), loadStrategyResult, null, LoadStrategy.MiscEntities.LineItemAdditionalDetails);
                //The fetched child records should use the Parent entities.
            }

            //Load ChargeCodeType by calling respective LoadEntities method
            if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemDetails) && lineItems.Count != 0)
            {
                LineItemDetailRepository.LoadEntities(objectSet.Context.CreateObjectSet<LineItemDetail>(), loadStrategyResult, lid => lid.LineItem = lineItems.Find(li => li.Id == lid.LineItemId), LoadStrategy.MiscEntities.LineItemDetails);
                //The fetched child records should use the Parent entities.
            }
            return lineItems;
        }
    }
}
