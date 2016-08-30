using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Core;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Isr.Impl;
using Iata.IS.Model.Pax;
using Devart.Data.Oracle;
using Iata.IS.Model.Pax.AutoBilling;
using Iata.IS.Model.Pax.Sampling;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
  public class CouponRecordRepository : Repository<PrimeCoupon>, ICouponRecordRepository
  {
    public override PrimeCoupon Single(System.Linq.Expressions.Expression<Func<PrimeCoupon, bool>> where)
    {
      throw new NotImplementedException("Use Load Strategy overload of Single method instead.");

    }

    // Review: Needs to be removed.
    public PrimeCoupon GetCouponWithVatList(Guid couponRecordId)
    {
      var couponRecord = EntityObjectSet.Include("VatBreakdown").FirstOrDefault(i => i.Id == couponRecordId);

      return couponRecord;
    }

    // Review: Needs to be removed.
    public PrimeCoupon GetCouponWithTaxList(Guid couponRecordId)
    {
      var couponRecord = EntityObjectSet.Include("TaxBreakdown").FirstOrDefault(i => i.Id == couponRecordId);

      return couponRecord;
    }

    // Review: Needs to be removed.
    public PrimeCoupon GetCouponWithAllDetails(Guid couponRecordId)
    {
      var couponRecord = EntityObjectSet
                         .Include("TaxBreakdown")
                         .Include("VatBreakdown.VatIdentifier")
                         .Include("Attachments").SingleOrDefault(i => i.Id == couponRecordId);

      return couponRecord;
    }

    /// <summary>
    /// Gets the coupon record duplicate count.
    /// </summary>
    /// <param name="ticketCouponNumber">The ticket coupon number.</param>
    /// <param name="ticketDocNumber">The ticket doc number.</param>
    /// <param name="issuingAirline">The issuing airline.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="sourceCodeId"></param>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public long GetCouponRecordDuplicateCount(int ticketCouponNumber, long ticketDocNumber, string issuingAirline, int billingMemberId, int billedMemberId, int billingYear, int billingMonth, int sourceCodeId, Guid invoiceId)
    {
      var parameters = new ObjectParameter[10];
      parameters[0] = new ObjectParameter(CouponRecordRespositoryConstants.TicketCouponNumberParameterName, typeof(int)) { Value = ticketCouponNumber };
      parameters[1] = new ObjectParameter(CouponRecordRespositoryConstants.TicketDocNumberParameterName, typeof(long)) { Value = ticketDocNumber };
      parameters[2] = new ObjectParameter(CouponRecordRespositoryConstants.IssuingAirlineParameterName, typeof(string)) { Value = issuingAirline };
      parameters[3] = new ObjectParameter(CouponRecordRespositoryConstants.BillingMemberParameterName, typeof(int)) { Value = billingMemberId };
      parameters[4] = new ObjectParameter(CouponRecordRespositoryConstants.BilledMemberParameterName, typeof(int)) { Value = billedMemberId };
      parameters[5] = new ObjectParameter(CouponRecordRespositoryConstants.BillingYearParameterName, typeof(int)) { Value = billingYear };
      parameters[6] = new ObjectParameter(CouponRecordRespositoryConstants.BillingMonthParameterName, typeof(int)) { Value = billingMonth };

      parameters[7] = new ObjectParameter(CouponRecordRespositoryConstants.SourceCodeIdParameterName, typeof(int)) { Value = sourceCodeId };
      parameters[8] = new ObjectParameter(CouponRecordRespositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };

      parameters[9] = new ObjectParameter(CouponRecordRespositoryConstants.DuplicateCountParameterName, typeof(long));

      ExecuteStoredProcedure(CouponRecordRespositoryConstants.GetCouponNoDuplicateCountFunctionName, parameters);

      return long.Parse(parameters[9].Value.ToString());
    }

    public List<LinkedCoupon> GetFormDLinkedCouponDetails(Guid invoiceId, int ticketCouponNumber, long ticketDocNumber, string issuingAirline)
    {
      var parameters = new ObjectParameter[4];
      parameters[0] = new ObjectParameter(CouponRecordRespositoryConstants.TicketCouponNumberParameterName, typeof(int)) { Value = ticketCouponNumber };
      parameters[1] = new ObjectParameter(CouponRecordRespositoryConstants.TicketDocNumberParameterName, typeof(long)) { Value = ticketDocNumber };
      parameters[2] = new ObjectParameter(CouponRecordRespositoryConstants.IssuingAirlineParameterName, typeof(string)) { Value = issuingAirline };
      parameters[3] = new ObjectParameter(CouponRecordRespositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };

      var formDCoupons = ExecuteStoredFunction<LinkedCoupon>(CouponRecordRespositoryConstants.GetFormDLinkedCouponDetails, parameters);

      return formDCoupons.ToList();
    }

    public List<LinkedCoupon> GetLinkedCouponsForFormC(int ticketCouponNumber, long ticketDocNumber, string issuingAirline, int provisionalBillingMemberId, int fromBilledMemberId, int provisionalBillingMonth, int provisionalBillingYear)
    {
      var parameters = new ObjectParameter[7];
      parameters[0] = new ObjectParameter(CouponRecordRespositoryConstants.TicketCouponNumberParameterName, typeof(int)) { Value = ticketCouponNumber };
      parameters[1] = new ObjectParameter(CouponRecordRespositoryConstants.TicketDocNumberParameterName, typeof(long)) { Value = ticketDocNumber };
      parameters[2] = new ObjectParameter(CouponRecordRespositoryConstants.IssuingAirlineParameterName, typeof(string)) { Value = issuingAirline };
      parameters[3] = new ObjectParameter(CouponRecordRespositoryConstants.ProvBillingMemberIdParameterName, typeof(int)) { Value = provisionalBillingMemberId };
      parameters[4] = new ObjectParameter(CouponRecordRespositoryConstants.FromMemberIdParameterName, typeof(int)) { Value = fromBilledMemberId };
      parameters[5] = new ObjectParameter(CouponRecordRespositoryConstants.ProvBillingYearParameterName, typeof(int)) { Value = provisionalBillingYear };
      parameters[6] = new ObjectParameter(CouponRecordRespositoryConstants.ProvBillingMonthParameterName, typeof(int)) { Value = provisionalBillingMonth };

      var coupons = ExecuteStoredFunction<LinkedCoupon>(CouponRecordRespositoryConstants.GetLinkedCouponsForFormC, parameters);

      return coupons.ToList();
    }

    #region Load strategy

    /// <summary>
    /// Loadstrategy method overload of Single
    /// </summary>
    /// <param name="couponId">coupon Id</param>
    /// <returns>SamplingFormDRecord</returns>
    public PrimeCoupon Single(Guid couponId)
    {
      var entities = new string[] { LoadStrategy.Entities.Coupon, LoadStrategy.Entities.CouponTax, LoadStrategy.Entities.CouponVat, 
      LoadStrategy.Entities.CouponDataVatIdentifier, LoadStrategy.Entities.CouponAttachment, LoadStrategy.Entities.AttachmentUploadedbyUser };

      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      var couponIdstr = ConvertUtil.ConvertGuidToString(couponId);
      var primeCoupons = GetPrimeCouponRecordsLS(loadStrategy, couponIdstr);

      PrimeCoupon coupon = null;
      if (primeCoupons.Count > 0)
      {
        if (primeCoupons.Count > 1) throw new ApplicationException("Multiple records found");
        coupon = primeCoupons[0];
      }
      return coupon;
    }

    /// <summary>
    /// Gets list of SamplingFormDRecord objects
    /// </summary>
    /// <param name="loadStrategy"></param>
    /// <param name="couponId"></param>
    /// <returns></returns>
    public List<PrimeCoupon> GetPrimeCouponRecordsLS(LoadStrategy loadStrategy, string couponId)
    {
      return base.ExecuteLoadsSP(SisStoredProcedures.GetPrimeCoupon,
                                loadStrategy,
                                  new OracleParameter[] { new OracleParameter(CouponRecordRespositoryConstants.PrimeCouponIdParameterName, couponId)
                                },
                                this.FetchRecords);
    }

    /// <summary>
    /// Returns multiple records extracted from the result set.
    /// This is done by calling the right repository to populate the object set in the repository.
    /// </summary>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    private List<PrimeCoupon> FetchRecords(LoadStrategyResult loadStrategyResult)
    {
      var primeCoupons = new List<PrimeCoupon>();
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.Coupon))
      {
        primeCoupons =  CouponRecordRepository.LoadEntities(base.EntityObjectSet, loadStrategyResult, null);
      }

      return primeCoupons;
    }

    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    public static List<PrimeCoupon> LoadEntities(ObjectSet<PrimeCoupon> objectSet, LoadStrategyResult loadStrategyResult, Action<PrimeCoupon> link)
    {
      if (link == null)
        link = new Action<PrimeCoupon>(c => { });
      List<PrimeCoupon> coupons = new List<PrimeCoupon>();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.Coupon))
      {
        // first result set includes the category
        foreach (var c in
            new PaxMaterializers().CouponMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach<PrimeCoupon>(link)
            )
        {
          coupons.Add(c);
        }
        reader.Close();
      }

      //Load PrimeCouponTax by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CouponTax) && coupons.Count != 0)
        CouponTaxRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<PrimeCouponTax>()
                , loadStrategyResult
                , null);


      //Load PrimeCouponVat by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CouponVat) && coupons.Count != 0)
        CouponVatRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<PrimeCouponVat>()
                , loadStrategyResult
                , null);

      //Load PrimeCouponVat by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CouponAttachment) && coupons.Count != 0)
        PrimeCouponAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<PrimeCouponAttachment>()
                , loadStrategyResult
                , null);

      //Load CouponSourceCode
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CouponSourceCode) && coupons.Count != 0)
        SourceCodeRepository.LoadEntities(objectSet.Context.CreateObjectSet<SourceCode>()
                , loadStrategyResult
                , null
                ,LoadStrategy.Entities.CouponSourceCode);

      //Load PrimeCouponTax by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CouponMarketingDetails) && coupons.Count != 0)
        PrimeCouponMarketingDetailsRepository.LoadEntities(objectSet.Context.CreateObjectSet<PrimeCouponMarketingDetails>()
                , loadStrategyResult
                , null);

      return coupons;
    }

    #endregion

    #region AuditLoadStartegy
    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    public static List<PrimeCoupon> LoadAuditEntities(ObjectSet<PrimeCoupon> objectSet, LoadStrategyResult loadStrategyResult, Action<PrimeCoupon> link)
    {
      if (link == null)
        link = new Action<PrimeCoupon>(c => { });
      List<PrimeCoupon> coupons = new List<PrimeCoupon>();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.PrimeCoupon))
      {
        // first result set includes the category
        foreach (var c in
          new PaxMaterializers().PaxInvoiceCouponAuditMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach<PrimeCoupon>(link)
          )
        {
          coupons.Add(c);
        }
        reader.Close();
      }

      ////Load SamplingFormDTax by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.PrimecouponTax) && coupons.Count != 0) CouponTaxRecordRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<PrimeCouponTax>(), loadStrategyResult, null);

      //Load SamplingFormDVat by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.PrimecouponVat) && coupons.Count != 0) CouponVatRecordRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<PrimeCouponVat>(), loadStrategyResult, null);

      if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.PMAttachment) && coupons.Count != 0)
        PrimeCouponAttachmentRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<PrimeCouponAttachment>()
                , loadStrategyResult
                , null);

      return coupons;
    }

      #endregion
  }
}
