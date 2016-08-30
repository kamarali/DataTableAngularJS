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
  public class LineItemDetailRepository : Repository<LineItemDetail>, ILineItemDetailRepository
  {
    public override LineItemDetail Single(Expression<Func<LineItemDetail, bool>> where)
    {
      throw new NotImplementedException("Use overloaded Single instead.");
    }

    public LineItemDetail GetLineItemDetailHeaderInformation(Expression<Func<LineItemDetail, bool>> where)
    {
      return EntityObjectSet.SingleOrDefault(where);
    }

    public override IQueryable<LineItemDetail> Get(Expression<Func<LineItemDetail, bool>> where)
    {
      var lineItemDetailList = EntityObjectSet
      .Include("UomCode")
      .Include("TaxBreakdown")
      .Include("AddOnCharges")
      .Include("LineItemDetailAdditionalDetails")
      .Include("LineItem")
      .Include("FieldValues")
      .Where(where);

      return lineItemDetailList;

    }

    public int GetMaxDetailNumber(Guid lineItemId)
    {
      return EntityObjectSet
        .Where(ld => ld.LineItemId == lineItemId)
        .Max(ld => ld.DetailNumber);
    }

    public void UpdateLineItemDetailEndDate(Guid lineItemId, DateTime lineItemEndDate)
    {
      var parameters = new ObjectParameter[2];

      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.LineItemIdParameterName, typeof(Guid))
      {
        Value = lineItemId
      };
      parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.LineItemEndDateParameterName, typeof(DateTime))
      {
        Value = lineItemEndDate
      };

      ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.UpdateLineItemDetailEndDateFunctionName, parameters);
    }

    /// <summary>
    /// Singles the specified line item detail id.
    /// </summary>
    /// <param name="lineItemDetailId">The line item detail id.</param>
    /// <param name="lineItemNo">The line item no.</param>
    /// <returns></returns>
    public LineItemDetail Single(Guid? lineItemDetailId, int? lineItemNo)
    {
      var entities = new[]
                       {
                         LoadStrategy.MiscEntities.LineItemDetails, LoadStrategy.MiscEntities.LineItemDetailUomCode, LoadStrategy.MiscEntities.LineItemDetailTaxBreakdown,
                         LoadStrategy.MiscEntities.LineItemDetailAddOnCharges, LoadStrategy.MiscEntities.LineItemDetailAdditionalDet, LoadStrategy.MiscEntities.LineItem,
                         LoadStrategy.MiscEntities.LineItemDetailsFieldValues
                       };
      var lineItemDetails = GetLineItemsLS(new LoadStrategy(string.Join(",", entities)), null, lineItemDetailId, lineItemNo);
      if (lineItemDetails == null || lineItemDetails.Count == 0) return null;
      if (lineItemDetails.Count > 1) throw new ApplicationException("Multiple records found");
      return lineItemDetails[0];
    }

    public List<LineItemDetail> Get(Guid? lineItemId, int? lineItemNo)
    {
      var entities = new[]
                           {
                               LoadStrategy.MiscEntities.LineItemDetails, LoadStrategy.MiscEntities.LineItemDetailUomCode,
                               LoadStrategy.MiscEntities.LineItemDetailTaxBreakdown,LoadStrategy.MiscEntities.LineItemDetailAddOnCharges,
                               LoadStrategy.MiscEntities.LineItemDetailAdditionalDet,LoadStrategy.MiscEntities.LineItem,
                               LoadStrategy.MiscEntities.LineItemDetailsFieldValues
                           };
      return GetLineItemsLS(new LoadStrategy(string.Join(",", entities)), lineItemId, null, lineItemNo);
    }

    private List<LineItemDetail> GetLineItemsLS(LoadStrategy loadStrategy, Guid? lineItemId, Guid? lineItemDetailId, int? lineItemNo)
    {
      return ExecuteLoadsSP(SisStoredProcedures.GetLineItem,
                                 loadStrategy,
                                 new[]
                                       {
                                           new OracleParameter(LineItemRepositoryConstants.InvoiceIdParameterName, null), 
                                           new OracleParameter(LineItemRepositoryConstants.LineItemIdParameterName, 
                                                                lineItemId != null ? ConvertUtil.ConvertGuidToString(lineItemId.Value) :null),
                                           new OracleParameter(LineItemRepositoryConstants.LineItemDetailIdParameterName,
                                                               lineItemDetailId != null ? ConvertUtil.ConvertGuidToString(lineItemDetailId.Value) : null),
                                           new OracleParameter(LineItemRepositoryConstants.LineItemDetailNoParameterName, lineItemNo)
                                       },
                                 r => this.FetchRecord(r));
    }

    private List<LineItemDetail> FetchRecord(LoadStrategyResult loadStrategyResult)
    {
      var lineItemDetails = new List<LineItemDetail>();
      var muMaterializers = new MuMaterializers();
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemDetails))
      {
        lineItemDetails = LoadEntities(EntityObjectSet, loadStrategyResult, null, LoadStrategy.MiscEntities.LineItemDetails);
      }
      /* LineItem is parent entry to LineItemDetails
       * It will become circular call, if it's binding is implemented into LoadEntities function 
       * so it is better to implement it in this function*/
      
      using (var reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.LineItem))
      {
        // first result set includes the category
        muMaterializers.LineItemMaterializer.Materialize(reader).Bind(EntityObjectSet.Context.CreateObjectSet<LineItem>()).ToList();
        reader.Close();
      }
      return lineItemDetails;
    }


    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result.
    /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <param name="entityName"></param>
    /// <returns></returns>
    public static List<LineItemDetail> LoadEntities(ObjectSet<LineItemDetail> objectSet, LoadStrategyResult loadStrategyResult, Action<LineItemDetail> link, string entityName)
    {
      if (link == null)
        link = new Action<LineItemDetail>(c => { });

      var LineItemDetails = new List<LineItemDetail>();
      var muMaterializers = new MuMaterializers();
      using (var reader = loadStrategyResult.GetReader(entityName))
      {
        // first result set includes the category
        LineItemDetails = muMaterializers.LineItemDetailsMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
        reader.Close();
      }

      //Load ChargeCodeType by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemDetailsFieldValues) && LineItemDetails.Count != 0)
      {
        FieldValueRepository.LoadEntities(objectSet.Context.CreateObjectSet<FieldValue>(), loadStrategyResult, lidfv => lidfv.LineItemDetail = LineItemDetails.Find(lid => lid.Id == lidfv.LineItemDetailId), LoadStrategy.MiscEntities.LineItemDetailsFieldValues);
        //The fetched child records should use the Parent entities.
      }

      //Load ChargeCodeType by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemDetailUomCode) && LineItemDetails.Count != 0)
      {
        UomCodeRepository.LoadEntities(objectSet.Context.CreateObjectSet<UomCode>(), loadStrategyResult, null, LoadStrategy.MiscEntities.LineItemDetailUomCode);
        //The fetched child records should use the Parent entities.
      }

      //Load ChargeCodeType by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemDetailTaxBreakdown) && LineItemDetails.Count != 0)
      {
        LineItemDetailTaxRepository.LoadEntities(objectSet.Context.CreateObjectSet<LineItemDetailTax>(), loadStrategyResult, null, LoadStrategy.MiscEntities.LineItemDetailTaxBreakdown);
        //The fetched child records should use the Parent entities.
      }

      //Load ChargeCodeType by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemDetailAddOnCharges) && LineItemDetails.Count != 0)
      {
        LineItemDetailAddOnChargeRepository.LoadEntities(objectSet.Context.CreateObjectSet<LineItemDetailAddOnCharge>(), loadStrategyResult, null, LoadStrategy.MiscEntities.LineItemDetailAddOnCharges);
        //The fetched child records should use the Parent entities.
      }

      //Load ChargeCodeType by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItemDetailAdditionalDet) && LineItemDetails.Count != 0)
      {
        LineItemDetailAdditionalDetailRepository.LoadEntities(objectSet.Context.CreateObjectSet<LineItemDetailAdditionalDetail>(), loadStrategyResult, null, LoadStrategy.MiscEntities.LineItemDetailAdditionalDet);
        //The fetched child records should use the Parent entities.
      }

      return LineItemDetails;
    }
  }
}
