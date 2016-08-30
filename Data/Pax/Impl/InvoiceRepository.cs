using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MemberProfile.Impl;
using Iata.IS.Data.MiscUatp.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.AutoBilling;
using Iata.IS.Model.Pax.BillingHistory;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.ParsingModel;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Model.Reports;
using Microsoft.Data.Extensions;
using log4net;

namespace Iata.IS.Data.Pax.Impl
{
  public class InvoiceRepository : RepositoryEx<PaxInvoice, InvoiceBase>, IInvoiceRepository
  {
    public override PaxInvoice Single(Expression<Func<PaxInvoice, bool>> where)
    {
      throw new NotImplementedException("Use Overloaded Single instead.");
    }



    /// <summary>
    /// Singles the specified invoice id.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="id">The id.</param>
    /// <param name="invoiceStatusId"></param>
    /// <returns></returns>
    public PaxInvoice Single(string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null, Guid? id = null, int? invoiceStatusId = null)
    {
      var entities = new string[] { LoadStrategy.Entities.BilledMember,LoadStrategy.Entities.BillingMember, LoadStrategy.Entities.MemberLocation, LoadStrategy.Entities.InvoiceTotal, 
      LoadStrategy.Entities.ListingCurrency, LoadStrategy.Entities.SamplingFormEDetails, LoadStrategy.Entities.SamplingFormDRecord, LoadStrategy.Entities.SamplingFormDVat};

      var loadStrategy = new LoadStrategy(string.Join(",", entities));

      string invoiceId = null;
      if (id.HasValue) invoiceId = ConvertUtil.ConvertGuidToString(id.Value);
      string invoiceStatusIdstr = null;
      if (invoiceStatusId.HasValue) invoiceStatusIdstr = invoiceStatusId.Value.ToString();
      var invoices = GetInvoiceLS(loadStrategy, invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, invoiceId, invoiceStatusIdstr);
      PaxInvoice invoice = null;
      if (invoices.Count > 0)
      {
        //TODO: Need to throw exception if result count > 1
        invoice = invoices[0];
      }
      return invoice;
    }

    //SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
    ///// <summary>
    ///// Updates expiry period of current transaction and transactions prior to it.
    ///// </summary>
    ///// <param name="transactionId"></param>
    ///// <param name="transactionTypeId"></param>
    ///// <param name="expiryPeriod"></param>
    //public void UpdateExpiryDatePeriod(Guid transactionId, int transactionTypeId, DateTime expiryPeriod)
    //{
    //  var parameters = new ObjectParameter[3];

    //  parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.PaxInvoiceTransactionIdParameterName, typeof(Guid)) { Value = transactionId };
    //  parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.PaxInvoiceTransactionTypeParameterName, typeof(int)) { Value = transactionTypeId };
    //  parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.ExpiryPeriodParameterName, typeof(DateTime)) { Value = expiryPeriod };

    //  ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateExpiryDatePeriodFunctionName, parameters);
    //}

    /// <summary>
    /// Initializes a new instance of the <see cref="InvoiceRepository"/> class.
    /// </summary>
    public InvoiceRepository()
    {
      InitializeObjectSet();
    }

    /// <summary>
    /// Initializes the object set.
    /// </summary>
    public override sealed void InitializeObjectSet()
    {
      EntityBaseObjectSet = Context.CreateObjectSet<InvoiceBase>();
      EntityObjectQuery = EntityBaseObjectSet.OfType<PaxInvoice>();
    }

    //Review : Not used anywhere , can be deleted.
    /// <summary>
    /// Gets all.
    /// </summary>
    /// <returns></returns>
    public override IQueryable<PaxInvoice> GetAll()
    {
      return EntityObjectQuery
      .Include("BilledMember")
      .Include("ListingCurrency")
      .Include("IsInputFile")
      .Include("InvoiceTotalRecord")
      .Include("SamplingFormEDetails")
      .Include("InvoiceOwner");

      //var miscCodeRepository = new Repository<MiscCode>();
      //miscCodeRepository.Get(rec=>rec.Group == MiscGroups.)

      //BillingCurrency.HasValue ? EnumList.GetBillingCurrencyDisplayValue(BillingCurrency.Value) : string.Empty;
    }

    /// <summary>
    /// Get all payables
    /// </summary>
    /// <returns></returns>
    public IQueryable<PaxInvoice> GetAllPayables()
    {
      return EntityObjectQuery
      .Include("BillingMember")
      .Include("ListingCurrency")
      .Include("IsInputFile")
      .Include("InvoiceTotalRecord")
      .Include("SamplingFormEDetails");
    }
    /// <summary>
    /// Populates the Invoice object with its child model
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    public List<PaxInvoice> GetInvoiceHierarchy(System.Linq.Expressions.Expression<Func<PaxInvoice, bool>> where)
    {
      throw new NotImplementedException("Use overloaded GetInvoiceHierarchy instead.");
    }

    /// <summary>
    /// Old IDEC : Populates the Invoice object with its child model (using Load strategy)
    /// </summary>
    /// <param name="billedMemberId"></param>
    /// <param name="billingPeriod"></param>
    /// <param name="billingMonth"></param>
    /// <param name="billingYear"></param>
    /// <param name="invoiceStatusId"></param>
    /// <returns></returns>
    public List<PaxInvoice> GetOldIdecInvoiceHierarchy(int? billedMemberId = null, int? billingPeriod = null, int? billingMonth = null, int? billingYear = null, int? invoiceStatusId = null)
    {
      var entities = new string[] { LoadStrategy.Entities.BilledMember, LoadStrategy.Entities.BillingMember, LoadStrategy.Entities.Coupon,
                                    LoadStrategy.Entities.InvoiceTotal};

      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      string invoiceStatusIdstr = null;
      if (invoiceStatusId.HasValue) invoiceStatusIdstr = invoiceStatusId.Value.ToString();
      var invoices = GetInvoiceLS(loadStrategy: loadStrategy, billingMonth: billingMonth, billingYear: billingYear, billingPeriod: billingPeriod, billedMemberId: billedMemberId, invoiceStatusIds: invoiceStatusIdstr);

      return invoices;
    }

    /// <summary>
    /// Populates the Invoice object with its child model (using Load strategy)
    /// </summary>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="invoiceStatusIds">The invoice status ids.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="submissionMethodId">The submission method id.</param>
    /// <param name="couponSearchCriteriaString">SCP215457: Daily RRF Query. To include only those coupons in DRR report that haveing 'INCLUDE_IN_DAILY_REV_RECOGN' flag is set to zero.</param>
    /// <param name="isOutput">if set to <c>true</c> [is output].</param>
    /// <returns></returns>
    public List<PaxInvoice> GetInvoiceHierarchy(int? billedMemberId = null, int? billingPeriod = null, int? billingMonth = null, int? billingYear = null, string invoiceStatusIds = null, int? billingCode = null, string invoiceId = null, int? billingMemberId = null, int? submissionMethodId = null, string couponSearchCriteriaString = null, bool isOutput = false)
    {
      // SCP249528: Changes done to improve performance of output
      string[] entities;
      if(isOutput)
      {
        entities = new string[] { LoadStrategy.Entities.BilledMember, LoadStrategy.Entities.BillingMember, LoadStrategy.Entities.MemberLocation, 
        //LoadStrategy.Entities.Coupon, LoadStrategy.Entities.CouponTax, LoadStrategy.Entities.CouponVat, LoadStrategy.Entities.CouponDataVatIdentifier,
        LoadStrategy.Entities.BillingMemo, LoadStrategy.Entities.BillingMemoVat, LoadStrategy.Entities.BillingMemoVatIdentifier,
        LoadStrategy.Entities.BillingMemoCoupon, LoadStrategy.Entities.BillingMemoCouponTax, LoadStrategy.Entities.BillingMemoCouponVat,
        LoadStrategy.Entities.RejectionMemo, LoadStrategy.Entities.RejectionMemoVat, LoadStrategy.Entities.RejectionMemoVatIdentifier,
        LoadStrategy.Entities.RejectionMemoCoupon, LoadStrategy.Entities.RejectionMemoCouponTax, LoadStrategy.Entities.RejectionMemoCouponVat, LoadStrategy.Entities.RejectionMemoCouponVatIdentifier,
        LoadStrategy.Entities.CreditMemo, LoadStrategy.Entities.CreditMemoVat, LoadStrategy.Entities.CreditMemoVatIdentifier,  
        LoadStrategy.Entities.CreditMemoCoupon, LoadStrategy.Entities.CreditMemoCouponTax, LoadStrategy.Entities.CreditMemoCouponVat, 
        LoadStrategy.Entities.SamplingFormDRecord, LoadStrategy.Entities.SamplingFormDTax, LoadStrategy.Entities.SamplingFormDVat,LoadStrategy.Entities.SamplingFormDVatIdentifier, 
        LoadStrategy.Entities.SamplingFormEDetails, LoadStrategy.Entities.SamplingFormEDetailVat,LoadStrategy.Entities.SamplingFormEDetailVatIdentifier, LoadStrategy.Entities.ProvisionalInvoiceDetails,
        LoadStrategy.Entities.InvoiceTotal, LoadStrategy.Entities.InvoiceTotalVat, LoadStrategy.Entities.InvoiceTotalVatIdentifier, 
        LoadStrategy.Entities.SourceCodeTotal, LoadStrategy.Entities.SourceCodeTotalVat, LoadStrategy.Entities.SourceCodeVatIdentifier, 
        LoadStrategy.Entities.ListingCurrency,
        LoadStrategy.Entities.AutoBillCoupon,LoadStrategy.Entities.AutoBillCouponTax,LoadStrategy.Entities.AutoBillCouponVat,LoadStrategy.Entities.AutoBillCouponAttach,
        LoadStrategy.Entities.CouponMarketingDetails };
      }
      else
      {
        entities = new string[] { LoadStrategy.Entities.BilledMember, LoadStrategy.Entities.BillingMember, LoadStrategy.Entities.MemberLocation, 
        LoadStrategy.Entities.Coupon, LoadStrategy.Entities.CouponTax, LoadStrategy.Entities.CouponVat, LoadStrategy.Entities.CouponDataVatIdentifier,
        LoadStrategy.Entities.BillingMemo, LoadStrategy.Entities.BillingMemoVat, LoadStrategy.Entities.BillingMemoVatIdentifier,
        LoadStrategy.Entities.BillingMemoCoupon, LoadStrategy.Entities.BillingMemoCouponTax, LoadStrategy.Entities.BillingMemoCouponVat,
        LoadStrategy.Entities.RejectionMemo, LoadStrategy.Entities.RejectionMemoVat, LoadStrategy.Entities.RejectionMemoVatIdentifier,
        LoadStrategy.Entities.RejectionMemoCoupon, LoadStrategy.Entities.RejectionMemoCouponTax, LoadStrategy.Entities.RejectionMemoCouponVat, LoadStrategy.Entities.RejectionMemoCouponVatIdentifier,
        LoadStrategy.Entities.CreditMemo, LoadStrategy.Entities.CreditMemoVat, LoadStrategy.Entities.CreditMemoVatIdentifier,  
        LoadStrategy.Entities.CreditMemoCoupon, LoadStrategy.Entities.CreditMemoCouponTax, LoadStrategy.Entities.CreditMemoCouponVat, 
        LoadStrategy.Entities.SamplingFormDRecord, LoadStrategy.Entities.SamplingFormDTax, LoadStrategy.Entities.SamplingFormDVat,LoadStrategy.Entities.SamplingFormDVatIdentifier, 
        LoadStrategy.Entities.SamplingFormEDetails, LoadStrategy.Entities.SamplingFormEDetailVat,LoadStrategy.Entities.SamplingFormEDetailVatIdentifier, LoadStrategy.Entities.ProvisionalInvoiceDetails,
        LoadStrategy.Entities.InvoiceTotal, LoadStrategy.Entities.InvoiceTotalVat, LoadStrategy.Entities.InvoiceTotalVatIdentifier, 
        LoadStrategy.Entities.SourceCodeTotal, LoadStrategy.Entities.SourceCodeTotalVat, LoadStrategy.Entities.SourceCodeVatIdentifier, 
        LoadStrategy.Entities.ListingCurrency,
        LoadStrategy.Entities.AutoBillCoupon,LoadStrategy.Entities.AutoBillCouponTax,LoadStrategy.Entities.AutoBillCouponVat,LoadStrategy.Entities.AutoBillCouponAttach,
        LoadStrategy.Entities.CouponMarketingDetails };
      }

      LoadStrategy loadStrategy = new LoadStrategy(string.Join(",", entities));
      // SCP215457: Daily RRF Query. To include only those coupons in DRR report that haveing 'INCLUDE_IN_DAILY_REV_RECOGN' flag is set to zero.
      var invoices = GetInvoiceLS(loadStrategy: loadStrategy, billingMonth: billingMonth, billingYear: billingYear, billingPeriod: billingPeriod, billedMemberId: billedMemberId, invoiceStatusIds: invoiceStatusIds, billingCode: billingCode, invoiceId: invoiceId, billingMemberId: billingMemberId, submissionMethodId: submissionMethodId, couponSearchCriteriaString: couponSearchCriteriaString);
        

      return invoices;
    }


    public List<PaxInvoice> GetInvoiceHierarchy(string invoiceId)
    {
      var entities = new string[] { LoadStrategy.Entities.BilledMember, LoadStrategy.Entities.BillingMember, LoadStrategy.Entities.MemberLocation, 
        LoadStrategy.Entities.ProvisionalInvoiceDetails,
        LoadStrategy.Entities.InvoiceTotal, LoadStrategy.Entities.InvoiceTotalVat, LoadStrategy.Entities.InvoiceTotalVatIdentifier, 
        LoadStrategy.Entities.SourceCodeTotal, LoadStrategy.Entities.SourceCodeTotalVat, LoadStrategy.Entities.SourceCodeVatIdentifier, 
        LoadStrategy.Entities.ListingCurrency };

      LoadStrategy loadStrategy = new LoadStrategy(string.Join(",", entities));
      var invoices = GetInvoiceLS(loadStrategy: loadStrategy, invoiceId: invoiceId);

      return invoices;
    }

    //public List<PaxInvoice> GetInvoiceHierarchy(string invoiceId)
    //{
    //  var entities = new string[] { LoadStrategy.Entities.BilledMember, LoadStrategy.Entities.BillingMember, LoadStrategy.Entities.MemberLocation, 
    //    LoadStrategy.Entities.Coupon, LoadStrategy.Entities.CouponTax, LoadStrategy.Entities.CouponVat, LoadStrategy.Entities.CouponDataVatIdentifier,
    //    LoadStrategy.Entities.BillingMemo, LoadStrategy.Entities.BillingMemoVat, LoadStrategy.Entities.BillingMemoVatIdentifier,
    //    LoadStrategy.Entities.BillingMemoCoupon, LoadStrategy.Entities.BillingMemoCouponTax, LoadStrategy.Entities.BillingMemoCouponVat,
    //    LoadStrategy.Entities.RejectionMemo, LoadStrategy.Entities.RejectionMemoVat, LoadStrategy.Entities.RejectionMemoVatIdentifier,
    //    LoadStrategy.Entities.RejectionMemoCoupon, LoadStrategy.Entities.RejectionMemoCouponTax, LoadStrategy.Entities.RejectionMemoCouponVat, 
    //    LoadStrategy.Entities.CreditMemo, LoadStrategy.Entities.CreditMemoVat, LoadStrategy.Entities.CreditMemoVatIdentifier,  
    //    LoadStrategy.Entities.CreditMemoCoupon, LoadStrategy.Entities.CreditMemoCouponTax, LoadStrategy.Entities.CreditMemoCouponVat, 
    //    LoadStrategy.Entities.SamplingFormDRecord, LoadStrategy.Entities.SamplingFormDTax, LoadStrategy.Entities.SamplingFormDVat, 
    //    LoadStrategy.Entities.SamplingFormEDetails, LoadStrategy.Entities.SamplingFormEDetailVat, LoadStrategy.Entities.ProvisionalInvoiceDetails,
    //    LoadStrategy.Entities.InvoiceTotal, LoadStrategy.Entities.InvoiceTotalVat, LoadStrategy.Entities.InvoiceTotalVatIdentifier, 
    //    LoadStrategy.Entities.SourceCodeTotal, LoadStrategy.Entities.SourceCodeTotalVat, LoadStrategy.Entities.SourceCodeVatIdentifier, 
    //    LoadStrategy.Entities.ListingCurrency, };

    //  LoadStrategy loadStrategy = new LoadStrategy(string.Join(",", entities));
    //  var invoices = GetInvoiceLS(loadStrategy: loadStrategy, invoiceId:invoiceId);

    //  return invoices;
    //}

    /// <summary>
        /// Gets the invoice offline collection data.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <returns></returns>
    public PaxInvoice GetInvoiceOfflineCollectionDataWithISWebAttachments(string invoiceId = null, string invoiceNumber = null)
    {
      var entities = new[]
                       {
                          LoadStrategy.Entities.BilledMember, 
                          LoadStrategy.Entities.BillingMember,
                          LoadStrategy.Entities.CouponISWeb,
                          LoadStrategy.Entities.CouponAttachmentISWeb,
                          LoadStrategy.Entities.RejectionMemoISWeb,
                          LoadStrategy.Entities.SamplingFormDRecordISWeb,
                          LoadStrategy.Entities.SamplingFormDAttachmentISWeb,
                          LoadStrategy.Entities.BillingMemoISWeb,
                          LoadStrategy.Entities.BillingMemoAttachmentsISWeb,
                          // LoadStrategy.Entities.BillingMemoVat,
                          // LoadStrategy.Entities.BillingMemoCouponTax,
                          // LoadStrategy.Entities.BillingMemoCouponVat ,
                          LoadStrategy.Entities.BillingMemoCouponISWeb,
                          LoadStrategy.Entities.BMCouponAttachmentsISWeb,
                          LoadStrategy.Entities.CreditMemoISWeb,
                          LoadStrategy.Entities.CreditMemoAttachmentsISWeb,
                          // LoadStrategy.Entities.CreditMemoVat,
                          LoadStrategy.Entities.CreditMemoCouponISWeb,
                          // LoadStrategy.Entities.CreditMemoCouponTax,
                          // LoadStrategy.Entities.CreditMemoCouponVat,
                          LoadStrategy.Entities.CreditMemoCouponAttachmentsISWeb,
                          LoadStrategy.Entities.RejectionMemoCouponISWeb,
                          LoadStrategy.Entities.RejectionMemoCouponTax ,
                          LoadStrategy.Entities.RejectionMemoCouponVat,
                          LoadStrategy.Entities.RejectionMemoCouponAttachmentsISWeb,
                          LoadStrategy.Entities.RejectionMemoAttachmentsISWeb, 
                          // LoadStrategy.Entities.RejectionMemoVat,
                          // LoadStrategy.Entities.ListingCurrency
                       };
      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      var invoices = GetInvoiceLS(loadStrategy: loadStrategy, invoiceId: invoiceId, invoiceNumber: invoiceNumber);
      return invoices != null && invoices.Count > 0 ? invoices[0] : null;
    }

        /// <summary>
    /// Gets the derived vat details for an Invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// List of derived vat details for the Invoice.
    /// </returns>
    public IList<DerivedVatDetails> GetDerivedVatDetails(Guid invoiceId)
    {
      var parameter = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid))
      {
        Value = invoiceId
      };

      var derivedVatDetails = ExecuteStoredFunction<DerivedVatDetails>(InvoiceRepositoryConstants.GetDerivedVatDetailsFunctionName, parameter);

      return derivedVatDetails.ToList();
    }

    /// <summary>
    /// Gets the non applied vat details.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// List of non-applied vat details for the Invoice.
    /// </returns>
    public IList<NonAppliedVatDetails> GetNonAppliedVatDetails(Guid invoiceId)
    {
      var parameter = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid))
      {
        Value = invoiceId
      };

      var nonAppliedVatDetails = ExecuteStoredFunction<NonAppliedVatDetails>(InvoiceRepositoryConstants.GetNonAppliedVatDetailsFunctionName, parameter);

      return nonAppliedVatDetails.ToList();
    }

    /// <summary>
    /// Updates the Prime Billing Invoice total.
    /// </summary>
    /// <param name="invoiceId">The Invoice id.</param>
    /// <param name="sourceId">The Source id.</param>
    /// <param name="userId">The user id.</param>
    public void UpdatePrimeInvoiceTotal(Guid invoiceId, int sourceId, int userId)
    {
      var parameters = new ObjectParameter[3];

      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.SourceIdParameterName, typeof(int)) { Value = sourceId };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.UserIdParameterName, typeof(int)) { Value = userId };

      ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdatePrimeInvoiceTotalFunctionName, parameters);
    }
    /// <summary>
    /// Gets the ach invoice count.
    /// </summary>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="clearanceHouse">The clearance house.</param>
    /// <returns></returns>
    public int GetAchInvoiceCount(int billingMemberId, int billingCategoryId, int billingYear, int billingMonth, int billingPeriod, int settlementMethodId, string clearanceHouse)
    {
      var parameters = new ObjectParameter[8];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.BillingMemberId, typeof(int)) { Value = billingMemberId };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.BillingCategoryId, typeof(int)) { Value = billingCategoryId };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.AchBillingYear, typeof(int)) { Value = billingYear };
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.AchBillingMonth, typeof(int)) { Value = billingMonth };
      parameters[4] = new ObjectParameter(InvoiceRepositoryConstants.AchBillingPeriod, typeof(int)) { Value = billingPeriod };
      parameters[5] = new ObjectParameter(InvoiceRepositoryConstants.SettlementMethodId, typeof(int)) { Value = settlementMethodId };
      parameters[6] = new ObjectParameter(InvoiceRepositoryConstants.Clearinghouse, typeof(string)) { Value = clearanceHouse };
      parameters[7] = new ObjectParameter(InvoiceRepositoryConstants.AchResultParameterName, typeof(int));

      ExecuteStoredProcedure(InvoiceRepositoryConstants.AchInvoiceCountFunctionName, parameters);

      return int.Parse(parameters[7].Value.ToString());
    }

    /// <summary>
    /// Updates the Rejection memo Invoice total.
    /// </summary>
    /// <param name="invoiceId">The Invoice id.</param>
    /// <param name="sourceId">The Source id.</param>
    /// <param name="rejectionMemoId">The rejection memo id.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="isCouponDelete">if set to true [is coupon delete].</param>
    public void UpdateRMInvoiceTotal(Guid invoiceId, int sourceId, Guid rejectionMemoId, int userId, bool isCouponDelete = false)
    {
      var parameters = new ObjectParameter[5];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.SourceIdParameterName, typeof(int)) { Value = sourceId };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.RejectionMemoIdParameterName, typeof(Guid)) { Value = rejectionMemoId };
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.IsCouponDeleteParameterName, typeof(int)) { Value = isCouponDelete ? 1 : 0 };
      parameters[4] = new ObjectParameter(InvoiceRepositoryConstants.UserIdParameterName, typeof(Guid)) { Value = userId };

      ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateRMInvoiceTotalFunctionName, parameters);
    }

    /// <summary>
    /// Deletes the RM coupon and re-sequences the breakdown serial numbers of the subsequent coupons.
    /// </summary>
    /// <param name="rmCouponId">The rm coupon id.</param>
    public void DeleteRejectionMemoCoupon(Guid rmCouponId)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.RejectionMemoCouponId, typeof(Guid)) { Value = rmCouponId };

      ExecuteStoredProcedure(InvoiceRepositoryConstants.DeleteRMCoupon, parameters);
    }

    public void DeleteBillingMemoCoupon(Guid bmCouponId)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.BillingMemoCouponId, typeof(Guid)) { Value = bmCouponId };

      ExecuteStoredProcedure(InvoiceRepositoryConstants.DeleteBMCoupon, parameters);
    }

    public void DeleteCreditMemoCoupon(Guid cmCouponId)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.CreditMemoCouponId, typeof(Guid)) { Value = cmCouponId };

      ExecuteStoredProcedure(InvoiceRepositoryConstants.DeleteCMCoupon, parameters);
    }

    public void UpdateInvoiceOnReadyForBilling(Guid invoiceId, int billingCatId, int billingMemberId, int billedMemberId, int billingCodeId)
    {
      var parameters = new ObjectParameter[5];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdOnReadyForBillingParameterName, typeof(Guid)) { Value = invoiceId };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceBillingCatIdParameterName, typeof(int)) { Value = billingCatId };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceBillingMemberIdParameterName, typeof(int)) { Value = billingMemberId };
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceBilledMemberIdParameterName, typeof(int)) { Value = billedMemberId };
      parameters[4] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceBillingCodeIdParameterName, typeof(int)) { Value = billingCodeId };

      ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateInvoiceOnReadyForBillingFunctionName, parameters);
    }

    //SCP 152109: as discussed
    //Desc: False alert was generated for correspondence to raise BM even when BM was already raised. 
    //Problem identified to be because of future invoices not calling the SP to close the respective correspondences when marked RFB by the Job.
    //Actual call to SP, in order to close corr.
    //Date: 24-July-2013
    public void CloseCorrespondenceOnInvoiceReadyForBilling(Guid invoiceId, int billingCatId)
    {
        var parameters = new ObjectParameter[2];
        parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdForCorrClosingParam, typeof(Guid)) { Value = invoiceId };
        parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.BillingCategoryForCorrClosingParam, typeof(int)) { Value = billingCatId };

        ExecuteStoredProcedure(InvoiceRepositoryConstants.CloseCorrespondenceOnInvoiceReadyForBilling, parameters);
    }

    /// <summary>
    /// Updates the Billing Memo Invoice total.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="sourceId">The source id.</param>
    /// <param name="billingMemoId">The Billing Memo id.</param>
    /// <param name="userId"></param>
    /// <param name="isCouponDelete"></param>
    public void UpdateBMInvoiceTotal(Guid invoiceId, int sourceId, Guid billingMemoId, int userId, bool isCouponDelete = false)
    {
      var parameters = new ObjectParameter[5];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.SourceIdParameterName, typeof(int)) { Value = sourceId };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.BillingMemoIdParameterName, typeof(Guid)) { Value = billingMemoId };
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.IsCouponDeleteParameterName, typeof(Guid)) { Value = isCouponDelete ? 1 : 0 };
      parameters[4] = new ObjectParameter(InvoiceRepositoryConstants.UserIdParameterName, typeof(Guid)) { Value = userId };

      ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateBMInvoiceTotalFunctionName, parameters);
    }

    /// <summary>
    /// This method is added to fix issue of IS_BILLING_MEMBER of InvoiceLocationInfo was set to 0 for both entries.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="isLateSubmission"></param>
    /// <param name="dsRequiredBy"></param>
    /// <param name="clearingHouse"></param>
    /// <param name="sponsoredBy"></param>
    /// <param name="isValidBillingPeriod"></param>
    public void SubmitMiscInvoice(Guid invoiceId, bool isLateSubmission, string dsRequiredBy, string clearingHouse, int? sponsoredBy, bool isValidBillingPeriod)
    {
      var parameters = new ObjectParameter[6];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.IsLateSubmissionParameterName, typeof(int)) { Value = isLateSubmission ? 1 : 0 };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.DsRequiredByParameterName, typeof(string)) { Value = dsRequiredBy };
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.ClearingHouseParameterName, typeof(string)) { Value = clearingHouse };
      parameters[4] = new ObjectParameter(InvoiceRepositoryConstants.SponsoredByMemberIdParameterName, typeof(int)) { Value = sponsoredBy.HasValue ? sponsoredBy.Value : 0 };
      parameters[5] = new ObjectParameter(InvoiceRepositoryConstants.IsValidBillingPeriodParameterName, typeof(int)) { Value = isValidBillingPeriod ? 1 : 0 };

      ExecuteStoredProcedure(InvoiceRepositoryConstants.SubmitMiscInvoiceFunctionName, parameters);
    }

    /// <summary>
    /// Finalization of Supporting Document
    /// </summary>
    /// <param name="billingperiod">billingperiod</param>
    /// <param name="billingMonth">billingMonth</param>
    /// <param name="billingYear">billingYear</param>
    /// <param name="createNilFileForMiscLocation">The create nil file for misc location.</param>
    //CMP#622: MISC Outputs Split as per Location ID
    public void FinalizeSupportingDocument(int billingperiod, int billingMonth, int billingYear, bool createNilFileForMiscLocation = false)
    {
      var parameters = new ObjectParameter[4];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.ParameterNameBillingPeriod, typeof(int)) { Value = billingperiod };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.ParameterNameBillingYear, typeof(int)) { Value = billingYear };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.ParameterNameBillingMonth, typeof(int)) { Value = billingMonth };
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.ParameterNameCreateNilFileMiscLocation, typeof(int)) { Value = createNilFileForMiscLocation ? 1 : 0 };

      ExecuteStoredProcedure(InvoiceRepositoryConstants.FinalizeSupportingDocumentFunctionName, parameters);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="billingperiod">billingperiod</param>
    /// <param name="billingMonth">billingMonth</param>
    /// <param name="billingYear">billingYear</param>
    public void FinalizeSuppDocLinking(int billingYear, int billingMonth, int billingperiod)
    {
        var parameters = new ObjectParameter[3];
        parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.ParameterNameBillingYearForLinking, typeof(int)) { Value = billingYear };
        parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.ParameterNameBillingMonthForLinking, typeof(int)) { Value = billingMonth };
        parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.ParameterNameBillingPeriodForLinking, typeof(int)) { Value = billingperiod };

        ExecuteStoredProcedure(InvoiceRepositoryConstants.FinalizeSuppDocLinkFunctionName, parameters);
    }
    /// <summary>
    /// Updates the CM invoice total.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="sourceId">The source id.</param>
    /// <param name="creditMemoId">The credit memo id.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="isCouponDelete">if set to true [is coupon delete].</param>
    public void UpdateCMInvoiceTotal(Guid invoiceId, int sourceId, Guid creditMemoId, int userId, bool isCouponDelete = false)
    {
      var parameters = new ObjectParameter[5];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.SourceIdParameterName, typeof(int)) { Value = sourceId };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.CreditMemoIdParameterName, typeof(Guid)) { Value = creditMemoId };
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.IsCouponDeleteParameterName, typeof(int)) { Value = isCouponDelete ? 1 : 0 };
      parameters[4] = new ObjectParameter(InvoiceRepositoryConstants.UserIdParameterName, typeof(Guid)) { Value = userId };

      ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateCMInvoiceTotalFunctionName, parameters);
    }

    /// <summary>
    /// Determines whether invoice exists for the specified invoice number.
    /// </summary>
    /// <param name="yourInvoiceNumber">Your Invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="invoiceStatusId"></param>
    /// <returns></returns>
    public int IsExistingInvoice(string yourInvoiceNumber, int billingMonth, int billingYear, int billingPeriod, int billingMemberId, int billedMemberId, int invoiceStatusId)
    {
      var parameters = new ObjectParameter[8];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.YourInvoiceNumberParameterName, typeof(string)) { Value = yourInvoiceNumber };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.BillingMonthParameterName, typeof(int)) { Value = billingMonth };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.BillingYearParameterName, typeof(int)) { Value = billingYear };
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.BillingPeriodParameterName, typeof(int)) { Value = billingPeriod };
      parameters[4] = new ObjectParameter(InvoiceRepositoryConstants.BillingMemberIdParameterName, typeof(int)) { Value = billingMemberId };
      parameters[5] = new ObjectParameter(InvoiceRepositoryConstants.BilledMemberIdParameterName, typeof(int)) { Value = billedMemberId };
      parameters[6] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceStatusIdParameterName, typeof(int)) { Value = invoiceStatusId };
      parameters[7] = new ObjectParameter(InvoiceRepositoryConstants.ResultParameterName, typeof(int));

      ExecuteStoredProcedure(InvoiceRepositoryConstants.IsInvoiceExistsFunctionName, parameters);

      return int.Parse(parameters[7].Value.ToString());
    }

    /// <summary>
    /// Determines whether is reference correspondence exists for the specified correspondence number.
    /// </summary>
    /// <param name="correspondenceNumber">The correspondence number.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <returns></returns>
    public int IsRefCorrespondenceNumberExists(long correspondenceNumber, int billingMemberId, int billedMemberId)
    {
      var parameters = new ObjectParameter[4];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.CorrespondenceNumberParameterName, typeof(long)) { Value = correspondenceNumber };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.BillingMemberIdParameterName, typeof(int)) { Value = billingMemberId };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.BilledMemberIdParameterName, typeof(int)) { Value = billedMemberId };
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.ResultParameterName, typeof(int));

      ExecuteStoredProcedure(InvoiceRepositoryConstants.IsRefCorrespondenceNumberExistsFunctionName, parameters);

      return int.Parse(parameters[3].Value.ToString());
    }

    /// <summary>
    /// Determines whether invoice number exists for given input parameters.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <returns>
    /// Count of Invoice matched against the input parameters
    /// </returns>
    public long IsInvoiceNumberExists(string invoiceNumber, int billingYear, int billingMemberId)
    {
      var parameters = new ObjectParameter[4];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceNumberParameterName, typeof(string)) { Value = invoiceNumber };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.BillingMemberIdParameterName, typeof(int)) { Value = billingMemberId };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceBillingYearParameterName, typeof(int)) { Value = billingYear };
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.DuplicateCountParameterName, typeof(long));

      ExecuteStoredProcedure(InvoiceRepositoryConstants.IsInvoiceNumberExistsFunctionName, parameters);

      return long.Parse(parameters[3].Value.ToString());
    }

    /// <summary>
    /// Gets the invoice with RM.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public PaxInvoice GetInvoiceWithCoupons(System.Linq.Expressions.Expression<Func<PaxInvoice, bool>> where)
    {
      throw new NotImplementedException("Use overloaded GetInvoiceWithCoupons instead.");
    }

    public PaxInvoice GetInvoiceWithFormDRecord(System.Linq.Expressions.Expression<Func<PaxInvoice, bool>> where)
    {
      throw new NotImplementedException("Use overloaded GetInvoiceWithFormDRecord instead.");
    }

    public PaxInvoice GetInvoiceWithFormDRecord(string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null)
    {
      var entities = new string[] { LoadStrategy.Entities.SamplingFormDRecord };

      LoadStrategy loadStrategy = new LoadStrategy(string.Join(",", entities));
      var invoices = GetInvoiceLS(loadStrategy, invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, invoiceStatusIds: ((int)InvoiceStatusType.Presented).ToString());
      PaxInvoice invoice = null;
      if (invoices.Count > 0)
      {
        //TODO: Need to throw exception if result count > 1
        invoice = invoices[0];
      }
      return invoice;
    }

    public IList<PaxBillingHistorySearchResult> GetBillingHistorySearchResult(InvoiceSearchCriteria invoiceSearchCriteria, CorrespondenceSearchCriteria corrSearchCriteria, int? pageSize = null, int? pageNo = null, string sortColumn = null, string sortOrder = null,int rowCountRequired=1)
    {
      var parameters = new ObjectParameter[21];

      parameters[0] = new ObjectParameter("INVOICE_NO_I", typeof(String)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.InvoiceNumber : null };
      parameters[1] = new ObjectParameter("BILLING_YEAR_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingYear : 0 };
      parameters[2] = new ObjectParameter("BILLING_MONTH_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingMonth : 0 };
      parameters[3] = new ObjectParameter("BILLING_PERIOD_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingPeriod : 0 };
      parameters[4] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int)) { Value = corrSearchCriteria != null ? corrSearchCriteria.CorrBilledMemberId : invoiceSearchCriteria != null ? invoiceSearchCriteria.BilledMemberId : 0 };
      parameters[5] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value = corrSearchCriteria != null ? corrSearchCriteria.CorrBillingMemberId : invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingMemberId : 0 };
      parameters[6] = new ObjectParameter("MEMO_TYPE_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.MemoTypeId == 0 ? -1 : invoiceSearchCriteria.MemoTypeId : -1 };
      parameters[7] = new ObjectParameter("REJECTION_STAGE_I", typeof(int?)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.RejectionStageId : null };
      parameters[8] = new ObjectParameter("SOURCE_CODE_I", typeof(int?)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.SourceCodeId : null };
      parameters[9] = new ObjectParameter("REASON_CODE_I", typeof(String)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.ReasonCodeId : null };
      parameters[10] = new ObjectParameter("DOC_NO_I", typeof(long?)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.DocumentNumber : null };
      parameters[11] = new ObjectParameter("COUPON_NO_I", typeof(int?)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.CouponNumber : null };
      parameters[12] = new ObjectParameter("MEMO_NO_I", typeof(String)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.MemoNumber : null };
      parameters[13] = new ObjectParameter("ISSUING_AIRLINE_ID_I", typeof(String)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.IssuingAirline : null };
      parameters[14] = new ObjectParameter("BILLING_TYPE_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingTypeId : 0 };
      parameters[15] = new ObjectParameter("BILLING_CODE_ID_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingCode : 0 };

      parameters[16] = new ObjectParameter("PAGE_SIZE_I", typeof(int)) { Value = pageSize != null ? pageSize : 5 };
      parameters[17] = new ObjectParameter("PAGE_NO_I", typeof(int)) { Value = pageNo != null ? pageNo : 1 };

      parameters[18] = new ObjectParameter("SORT_COLUMN_I", typeof(string)) { Value = sortColumn };
      parameters[19] = new ObjectParameter("SORT_ORDER_I", typeof(string)) { Value = sortOrder };
      //SCP85039: IS Web Performance Feedback / Billing History & Correspondence / Other issues
      //below parameter added
      parameters[20] = new ObjectParameter("ROWCOUNT_REQUIRED_I", typeof(int)) { Value = rowCountRequired };
      var sourceTotals = ExecuteStoredFunction<PaxBillingHistorySearchResult>("GetPaxBillingHistorySearchResult", parameters);

      return sourceTotals.ToList();
    }


    public List<PaxBillingHistorySearchResult> GetBillingHistoryCorrSearchResult(CorrespondenceSearchCriteria correspondenceSearchCriteria, int? pageSize = null, int? pageNo = null, string sortColumn = null, string sortOrder = null, int rowCountRequired = 1)
    {
      var parameters = new ObjectParameter[17];

      parameters[0] = new ObjectParameter("FROM_DATE_I", typeof(String)) { Value = correspondenceSearchCriteria.FromDate };
      parameters[1] = new ObjectParameter("TO_DATE_I", typeof(int)) { Value = correspondenceSearchCriteria.ToDate };
      parameters[2] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value = correspondenceSearchCriteria.CorrBillingMemberId };
      parameters[3] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int)) { Value = correspondenceSearchCriteria.CorrBilledMemberId };
      parameters[4] = new ObjectParameter("CORRESPONDENCE_NO_I", typeof(int)) { Value = correspondenceSearchCriteria.CorrespondenceNumber };
      parameters[5] = new ObjectParameter("CORRESPONDENCE_STATUS_I", typeof(int)) { Value = correspondenceSearchCriteria.CorrespondenceStatusId };
      parameters[6] = new ObjectParameter("CORRESPONDENCE_SUB_STATUS_I", typeof(int)) { Value = correspondenceSearchCriteria.CorrespondenceSubStatusId };
      parameters[7] = new ObjectParameter("AUTHORITY_TO_BILL_I", typeof(int?)) { Value = correspondenceSearchCriteria.AuthorityToBill == false ? 0 : 1 };
      parameters[8] = new ObjectParameter("CORR_INIT_MEM_I", typeof(int?)) { Value = correspondenceSearchCriteria.InitiatingMember };

      parameters[9] = new ObjectParameter("NO_OF_DAYS_TO_EXPIRE_I", typeof(int?)) { Value = correspondenceSearchCriteria.NoOfDaysToExpiry };
      parameters[10] = new ObjectParameter("CORR_OWNER_ID_I", typeof(int?)) { Value = correspondenceSearchCriteria.CorrespondenceOwnerId };
      //CMP526 - Passenger Correspondence Identifiable by Source Code
      parameters[11] = new ObjectParameter("SOURCE_CODE_I", typeof(int?)) { Value = correspondenceSearchCriteria.SourceCode };

      parameters[12] = new ObjectParameter("PAGE_SIZE_I", typeof(int)) { Value = pageSize != null ? pageSize : 5 };
      parameters[13] = new ObjectParameter("PAGE_NO_I", typeof(int)) { Value = pageNo != null ? pageNo : 1 };
      parameters[14] = new ObjectParameter("SORT_COLUMN_I", typeof(string)) { Value = sortColumn };
      parameters[15] = new ObjectParameter("SORT_ORDER_I", typeof(string)) { Value = sortOrder };
      parameters[16] = new ObjectParameter("ROWCOUNT_REQUIRED_I", typeof(int)) { Value = rowCountRequired };

      var sourceTotals = ExecuteStoredFunction<PaxBillingHistorySearchResult>("GetPaxBillingHistoryCorrSearchResult", parameters);

      return sourceTotals.ToList();
    }

    /// <summary>
    /// This function is used to get linked correspondence rejection memo list.
    /// </summary>
    /// <param name="CorrespondenceRefNo"></param>
    /// <returns></returns>
    //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
    public List<PaxLinkedCorrRejectionSearchData> GetLinkedCorrRejectionSearchResult(Guid correspondenceId)
    {
      var parameters = new ObjectParameter[1];

      parameters[0] = new ObjectParameter("CORRESPONDENCE_ID_I", typeof(Guid)) { Value = correspondenceId };

      //Execute stored procedure and fetch data based on criteria.
      var linkedCorrRejectionList = ExecuteStoredFunction<PaxLinkedCorrRejectionSearchData>("GetPaxLinkedCorrRejectionSearchResult", parameters);

      return linkedCorrRejectionList.ToList();
    }

    public List<CorrespondenceTrailSearchResult> GetCorrespondenceTrailSearchResult(CorrespondenceTrailSearchCriteria correspondenceTrailSearchCriteria)
    {
      var parameters = new ObjectParameter[7];
      parameters[0] = new ObjectParameter("FROM_DATE_I", typeof(String)) { Value = correspondenceTrailSearchCriteria.FromDate };
      parameters[1] = new ObjectParameter("TO_DATE_I", typeof(int)) { Value = correspondenceTrailSearchCriteria.ToDate };
      parameters[2] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value = correspondenceTrailSearchCriteria.CorrBillingMemberId };
      parameters[3] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int)) { Value = correspondenceTrailSearchCriteria.CorrBilledMemberId };
      parameters[4] = new ObjectParameter("CORRESPONDENCE_STATUS_I", typeof(int)) { Value = correspondenceTrailSearchCriteria.CorrespondenceStatusId };
      parameters[5] = new ObjectParameter("CORRESPONDENCE_SUB_STATUS_I", typeof(int)) { Value = correspondenceTrailSearchCriteria.CorrespondenceSubStatusId };
      parameters[6] = new ObjectParameter("CORR_INIT_MEM_I", typeof(int?)) { Value = correspondenceTrailSearchCriteria.InitiatingMember };

      var correspondenceTrailSearchResult = ExecuteStoredFunction<CorrespondenceTrailSearchResult>("GetPaxCorrespondenceTrailSearchResult", parameters);
      return correspondenceTrailSearchResult.ToList();

    }
    public List<Transaction> GetRejectedTransactionDetails(string memoId, string couponIds)
    {
      var parameters = new ObjectParameter[2];

      parameters[0] = new ObjectParameter("MEMO_ID_I", typeof(string)) { Value = memoId };
      parameters[1] = new ObjectParameter("COUPON_IDS_I", typeof(String)) { Value = couponIds };

      var rejectedTransactions = ExecuteStoredFunction<Transaction>(InvoiceRepositoryConstants.GetRejectedTransactionDetailsFunctionName, parameters);

      return rejectedTransactions.ToList();
    }

    //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
    //Desc: More details about existing BM are added, hence returned cursor is changed.
    public List<ExistingBMTransaction> GetBillingMemosForCorrespondence(long correspondenceNumber, int billingMemberId)
    {
      var parameters = new ObjectParameter[2];

      parameters[0] = new ObjectParameter("CORRESPONDENCE_REF_NO_I", typeof(string)) { Value = correspondenceNumber };
      parameters[1] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value = billingMemberId };

      var billingMemos = ExecuteStoredFunction<ExistingBMTransaction>(InvoiceRepositoryConstants.GetBillingMemosForCorrespondenceFunctionName, parameters);

      return billingMemos.ToList();
    }

    public List<ProcessingDashboardInvoiceActionStatus> GetClaimFailedInvoices()
    {
      var lstInvoices = ExecuteStoredFunction<ProcessingDashboardInvoiceActionStatus>(InvoiceRepositoryConstants.GetClaimFailedInvoices);
      return lstInvoices.ToList();
    }

    /// <summary>
    /// Gets the invoice LS.
    /// </summary>
    /// <param name="loadStrategy">The load strategy.</param>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="invoiceId">The id.</param>
    /// <param name="invoiceStatusIds">The invoice status id.</param>
    /// <param name="couponSearchCriteriaString">To load only selected coupons</param>
    /// <param name="submissionMethodId"></param>
    /// <param name="rejectionMemoNumber">Added the new parameter for SCP51931: File stuck in Production. If value provided then data would be fetched for the provided RM only.</param> 
    /// <returns>Returns list of Passenger invoice.</returns>
    public List<PaxInvoice> GetInvoiceLS(LoadStrategy loadStrategy, string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null, string invoiceId = null, string invoiceStatusIds = null, string couponSearchCriteriaString = null, int? submissionMethodId = null, string rejectionMemoNumber = null)
    {
      //Logger.InfoFormat("Call to {0} stored procedure.", SisStoredProcedures.GetInvoice.Name);
      
      return ExecuteLoadsSP(SisStoredProcedures.GetInvoice,
                                loadStrategy,
                                new[] { new OracleParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, invoiceId ?? null),  
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameInvoiceNo, invoiceNumber ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBillingMonth, billingMonth ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBillingYear, billingYear ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBillingPeriod, billingPeriod ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBillingMemberId, billingMemberId ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBilledMemberId, billedMemberId ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBillingCode, billingCode ?? null), 
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameInvoiceStatusId, invoiceStatusIds ?? null),
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameCouponSearchCriteriaString,couponSearchCriteriaString ?? null), 
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameSubmissionMethod,submissionMethodId ?? null),
                                  // Added the new parameter for SCP51931: File stuck in Production. If value provided then data would be fetched for the provided RM only.
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameRejectionMemoNumber,rejectionMemoNumber ?? null),
                                },
                                r => this.FetchRecords(r));
    }

    /// <summary>
    /// Gets the invoices LS.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <param name="loadStrategy">The load strategy.</param>
    /// <returns></returns>
    public List<PaxInvoice> GetInvoicesLS(SearchCriteria criteria, LoadStrategy loadStrategy)
    {
      return base.ExecuteLoadsSP(SisStoredProcedures.GetInvoices //name of stored proc
                                 ,
                                 loadStrategy //specification of child entities to be loaded
                                 ,
                                 new OracleParameter[]
                                   {
                                     // INPUT parameters expected by the stored proc
                                     new OracleParameter(InvoiceRepositoryConstants.BilledMemberIdStrategyLoadParameterName, criteria.BilledMemberId),
                                     new OracleParameter(InvoiceRepositoryConstants.BillingPeriodStrategyLoadParameterName, criteria.BillingPeriod),
                                     new OracleParameter(InvoiceRepositoryConstants.BillingMonthStrategyLoadParameterName, criteria.BillingMonth),
                                     new OracleParameter(InvoiceRepositoryConstants.BillingYearStrategyLoadParameterName, criteria.BillingYear),
                                     new OracleParameter(InvoiceRepositoryConstants.BillingCodeIdStrategyLoadParameterName, criteria.BillingCode),
                                     new OracleParameter(InvoiceRepositoryConstants.InvoiceStatusIdStrategyLoadParameterName, criteria.InvoiceStatusId)
                                   },
                                 r => this.FetchRecords(r) //action to be done with the set of results
        );
    }

    /// <summary>
    /// Gets the pax old idec invoice LS.
    /// </summary>
    /// <param name="loadStrategy">The load strategy.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <returns></returns>
    public List<PaxInvoice> GetPaxOldIdecInvoiceLS(LoadStrategy loadStrategy, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? checkValueConfurmation = null)
    {
      return ExecuteLoadsSP(SisStoredProcedures.GetPaxOldIdecInvoices,
                              loadStrategy,
                              new[] { new OracleParameter(InvoiceRepositoryConstants.ParameterNameBillingMonth, billingMonth ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBillingYear, billingYear ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBillingPeriod, billingPeriod ?? null) ,
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameBillingMemberId, billingMemberId ?? null),
                                  new OracleParameter(InvoiceRepositoryConstants.ParameterNameCheckValueConfurmationStatus, checkValueConfurmation ?? null)
                                },
                              r => this.FetchRecords(r));
    }

    /// <summary>
    ///  Get  distinct Billing Member List for Pax Old Idec File generation
    /// </summary>
    /// <param name="billingMemberId"></param>
    /// <param name="billingYear"></param>
    /// <param name="billingMonth"></param>
    /// <returns></returns>
    public List<PaxOldIdecBillingMember> GetPaxOldIdecBillingMember(int? billingMemberId, int billingYear, int billingMonth)
    {
      try
      {

        var parameters = new ObjectParameter[3];
        parameters[0] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int))
        {
          Value = billingMemberId
        };
        parameters[1] = new ObjectParameter("BILLING_YEAR_I", typeof(int))
        {
          Value = billingYear
        };

        parameters[2] = new ObjectParameter("BILLING_MONTH_I", typeof(int))
        {
          Value = billingMonth
        };
        var billingMembers =
      ExecuteStoredFunction<PaxOldIdecBillingMember>("GetPaxOldIdecBillingMember", parameters).ToList();

        return billingMembers;
      }
      catch (Exception)
      {

        return null;
      }



    }


    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result.
    /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    public static List<PaxInvoice> LoadEntities(ObjectSet<InvoiceBase> objectSet, LoadStrategyResult loadStrategyResult, Action<PaxInvoice> link)
    {
      var logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

      List<PaxInvoice> invoices = new List<PaxInvoice>();
      
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.Invoice))
      {
        // first result set includes the category
        invoices = new PaxMaterializers().InvoiceMaterializer.Materialize(reader).Bind(objectSet).ToList();
        if (!reader.IsClosed)
          reader.Close();
      }

      //Load CouponRecord by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.Coupon) && invoices.Count != 0)
      {
        CouponRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<PrimeCoupon>(), loadStrategyResult, c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
        //The fetched child records should use the Parent entities.
      }

      //Load AutoBill CouponRecord by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.AutoBillCoupon) && invoices.Count != 0)
      {
        AutoBillingCouponRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<AutoBillingPrimeCoupon>(), loadStrategyResult, c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
        //The fetched child records should use the Parent entities.
      }

      //Load MemberLocationInformation by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.MemberLocation) && invoices.Count != 0)
      {
        MemberLocationInformationRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<MemberLocationInformation>(),
                                                               loadStrategyResult,
                                                               c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
      }

      //Load SourceCodeTotal by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SourceCodeTotal) && invoices.Count != 0)
      {
        SourceCodeTotalRepository.LoadEntities(objectSet.Context.CreateObjectSet<SourceCodeTotal>(), loadStrategyResult, c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
      }

      //Load InvoiceTotalRecord by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.InvoiceTotal) && invoices.Count != 0)
      {
        InvoiceTotalRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<InvoiceTotal>(), loadStrategyResult, i => i.Invoice = invoices.Find(j => j.Id == i.Id));
      }
    
      //Load InvoiceTotalVat by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.InvoiceTotalVat) && invoices.Count != 0)
      {
        InvoiceTotalVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<InvoiceVat>(), loadStrategyResult, null);
      }

      //Load RM by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.RejectionMemo) && invoices.Count != 0)
      {
        RejectionMemoRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<RejectionMemo>(), loadStrategyResult, c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
      }

      //Load BM by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.BillingMemo) && invoices.Count != 0)
      {
        BillingMemoRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<BillingMemo>(), loadStrategyResult, c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
      }

      //Load CM by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CreditMemo) && invoices.Count != 0)
      {
        CreditMemoRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<CreditMemo>(), loadStrategyResult, c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
      }

      //Load Billed Members by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.BilledMember) && invoices.Count != 0)
      {
        MemberRepository.LoadEntities(objectSet.Context.CreateObjectSet<Member>(),
                                                               loadStrategyResult,
                                                               null,
                                                               LoadStrategy.Entities.BilledMember);
      }

      //Load Billing Members by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.BillingMember) && invoices.Count != 0)
      {
        MemberRepository.LoadEntities(objectSet.Context.CreateObjectSet<Member>(),
                                                               loadStrategyResult,
                                                               null,
                                                               LoadStrategy.Entities.BillingMember);
      }

      //Load ListingCurrency by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.ListingCurrency) && invoices.Count != 0)
      {
        CurrencyRepository.LoadEntities(objectSet.Context.CreateObjectSet<Currency>(),
                                                               loadStrategyResult,
                                                               null,
                                                               LoadStrategy.Entities.ListingCurrency);
      }

      //Load Sampling Form E Details by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormEDetails) && invoices.Count != 0)
      {
        SamplingFormERepository.LoadEntities(objectSet.Context.CreateObjectSet<SamplingFormEDetail>(),
                                                               loadStrategyResult, c => c.Invoice = invoices.Find(i => i.Id == c.Id));
      }

      //Load Provisional Invoice Details by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.ProvisionalInvoiceDetails) && invoices.Count != 0)
      {
        ProvisionalInvoiceRepository.LoadEntities(objectSet.Context.CreateObjectSet<ProvisionalInvoiceRecordDetail>(),
                                                               loadStrategyResult, c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
      }

      //Load Sampling Form E Detail Vat by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormEDetailVat) && invoices.Count != 0)
      {
        SamplingFormEVatRepository.LoadEntities(objectSet.Context.CreateObjectSet<SamplingFormEDetailVat>(),
                                                               loadStrategyResult, null);
      }

      //Load Sampling Form D Record by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormDRecord) && invoices.Count != 0)
      {
        SamplingFormDRepository.LoadEntities(objectSet.Context.CreateObjectSet<SamplingFormDRecord>(),
                                                               loadStrategyResult, c => c.Invoice = invoices.Find(i => i.Id == c.InvoiceId));
      }

      return invoices;
    }

    /// <summary>
    /// Fetches the record.
    /// </summary>
    /// <param name="loadStrategyResult">The load strategy result.</param>
    /// <returns></returns>
    private PaxInvoice FetchRecord(LoadStrategyResult loadStrategyResult)
    {
      PaxInvoice invoice = null;
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.Invoice))
      {
        var invoices = InvoiceRepository.LoadEntities(base.EntityBaseObjectSet, loadStrategyResult, null);
        if (invoices.Count > 0)
        {
          invoice = invoices[0];
        }
      }

      return invoice;
    }

    /// <summary>
    /// Returns multiple records extracted from the result set.
    /// This is done by calling the right repository to populate the object set in the repository.
    /// </summary>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    private List<PaxInvoice> FetchRecords(LoadStrategyResult loadStrategyResult)
    {
      //Logger.Info("Populate Load Stratergy Result - Start.");

      List<PaxInvoice> invoices = new List<PaxInvoice>();
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.Invoice))
      {
        invoices = InvoiceRepository.LoadEntities(base.EntityBaseObjectSet, loadStrategyResult, null);
      }

      //Logger.Info("Populate Load Stratergy Result - End.");

      return invoices;
    }

    /// <summary>
    /// Gets the invoice with RM.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public IQueryable<PaxInvoice> GetInvoicesWithCoupons(System.Linq.Expressions.Expression<Func<PaxInvoice, bool>> where)
    {
      throw new NotImplementedException("Use overloaded GetInvoicesWithCoupons instead.");
    }

    /// <summary>
    /// Gets the invoices with coupons.
    /// </summary>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <returns></returns>
    public List<PaxInvoice> GetInvoicesWithCoupons(int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null)
    {
      var entities = new string[] { LoadStrategy.Entities.Coupon};

      LoadStrategy loadStrategy = new LoadStrategy(string.Join(",", entities));
      var invoices = GetInvoiceLS(loadStrategy, billingMonth: billingMonth, billingYear: billingYear, billingPeriod: billingPeriod, billingMemberId: billingMemberId, billedMemberId: billedMemberId, billingCode: billingCode, invoiceStatusIds: ((int)InvoiceStatusType.Presented).ToString());
      return invoices;
    }


    /// <summary>
    /// Gets the invoices with invoice total record.
    /// </summary>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <returns></returns>
    public List<PaxInvoice> GetInvoicesWithTotal(int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null)
    {
      var entities = new string[] { LoadStrategy.Entities.InvoiceTotal };

      LoadStrategy loadStrategy = new LoadStrategy(string.Join(",", entities));
      var invoices = GetInvoiceLS(loadStrategy, billingMonth: billingMonth, billingYear: billingYear, billingPeriod: billingPeriod, billingMemberId: billingMemberId, billedMemberId: billedMemberId, billingCode: billingCode, invoiceStatusIds: ((int)InvoiceStatusType.Presented).ToString());
      return invoices;
    }

    /// <summary>
    /// Gets the derived vat details for an Form D/E invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>List of derived vat details.</returns>
    public IList<DerivedVatDetails> GetFormDDerivedVatDetails(Guid invoiceId)
    {
      var parameter = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid))
      {
        Value = invoiceId
      };

      var derivedVatDetails = ExecuteStoredFunction<DerivedVatDetails>(InvoiceRepositoryConstants.GetFormDDerivedVatFunctionName, parameter);

      return derivedVatDetails.ToList();
    }

    /// <summary>
    /// Gets if Invoice contains coupons where attachement ind org is "Y"
    /// </summary>
    /// <param name="invoiceId">InvoiceId</param>
    /// <returns>bool</returns>
    public bool GetAttachmentIndOrgForInvoice(Guid invoiceId)
    {
      var attchementIndOrg = false;
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.AttachmentIndOrg, typeof(int));

      ExecuteStoredProcedure(InvoiceRepositoryConstants.GetAttachmentIndOrgForInvoiceFunctionName, parameters);

      if (int.Parse(parameters[1].Value.ToString()) > 0)
        attchementIndOrg = true;

      return attchementIndOrg;
    }

    /// <summary>
    /// Gets the non applied vat details for Form D/E invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>List of non-applied vat details.</returns>
    public IList<NonAppliedVatDetails> GetFormDNonAppliedVatDetails(Guid invoiceId)
    {
      var parameter = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid))
      {
        Value = invoiceId
      };

      var nonAppliedVatDetails = ExecuteStoredFunction<NonAppliedVatDetails>(InvoiceRepositoryConstants.GetFormDNonAppliedVatFunctionName, parameter);

      return nonAppliedVatDetails.ToList();
    }

    /// <summary>
    /// Gets the member location information for a particular invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="isBillingMember">Set to true if Location information for BillingMember is to fetched.
    /// Otherwise, false.</param>
    /// <returns></returns>
    public List<MemberLocationInformation> GetInvoiceMemberLocationInformation(Guid invoiceId, bool isBillingMember)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid))
      {
        Value = invoiceId
      };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.BillingMemberIdParameterName, typeof(bool))
      {
        Value = isBillingMember
      };

      var memberLocationInfo = ExecuteStoredFunction<MemberLocationInformation>(InvoiceRepositoryConstants.GetInvoiceMemberLocationInformationFunctionName, parameters);

      return memberLocationInfo.ToList();
    }

    /// <summary>
    /// Gets the member location  details in Member location information format.
    /// </summary>
    /// <param name="locationId">The location id.</param>
    /// <returns></returns>
    public List<MemberLocationInformation> GetMemberLocationInformation(int locationId)
    {
      var parameter = new ObjectParameter(InvoiceRepositoryConstants.LocationIdParameterName, typeof(int))
      {
        Value = locationId
      };

      var memberLocationInfo = ExecuteStoredFunction<MemberLocationInformation>(InvoiceRepositoryConstants.GetMemberLocationInformationFunctionName, parameter);


      return memberLocationInfo.ToList();
    }

    /// <summary>
    /// Gets the invoice with rejection memo record.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    public List<PaxInvoice> GetInvoicesWithRejectionMemoRecord(string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null, string id = null)
    {
      var entities = new string[] { LoadStrategy.Entities.RejectionMemo };

      LoadStrategy loadStrategy = new LoadStrategy(string.Join(",", entities));
      var invoices = GetInvoiceLS(loadStrategy, invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, id);
      return invoices;
    }


    /// <summary>
    /// Gets the invoices with rejection memo record for report.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    public List<PaxInvoice> GetInvoicesWithRejectionMemoRecordForReport(string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null, string id = null)
    {
      var entities = new string[] { LoadStrategy.Entities.RejectionMemo,
                                    LoadStrategy.Entities.RejectionMemoCoupon ,
                                    LoadStrategy.Entities.RejectionMemoCouponTax ,
                                    LoadStrategy.Entities.RejectionMemoCouponVat,
                                    LoadStrategy.Entities.RejectionMemoCouponAttachments,
                                    LoadStrategy.Entities.BillingMember, 
                                    LoadStrategy.Entities.BilledMember,
                                    LoadStrategy.Entities.RejectionMemoAttachments, 
                                    LoadStrategy.Entities.RejectionMemoVat
      };

      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      var invoices = GetInvoiceLS(loadStrategy, invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, id);
      return invoices;
    }

    /// <summary>
    /// Gets the invoices with credit memo record for report.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    public List<PaxInvoice> GetInvoicesWithCreditMemoRecordForReport(string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null, string id = null)
    {
      var entities = new string[]
                       {
                         LoadStrategy.Entities.CreditMemo, 
                         LoadStrategy.Entities.BillingMember,
                         LoadStrategy.Entities.BilledMember,
                         LoadStrategy.Entities.CreditMemoAttachments,
                         LoadStrategy.Entities.CreditMemoVat,
                         LoadStrategy.Entities.CreditMemoCoupon,
                         LoadStrategy.Entities.CreditMemoCouponTax,
                         LoadStrategy.Entities.CreditMemoCouponVat,
                         LoadStrategy.Entities.CreditMemoCouponAttachments
                       };
      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      var invoices = GetInvoiceLS(loadStrategy, invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, id);
      return invoices;
    }

    /// <summary>
    /// Gets the invoice with rejection memo record.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public IQueryable<PaxInvoice> GetInvoicesWithRejectionMemoRecord(System.Linq.Expressions.Expression<Func<PaxInvoice, bool>> where)
    {
      throw new NotImplementedException("Use overloaded GetInvoicesWithRejectionMemoRecord instead.");
    }

    /// <summary>
    /// Updates the file log and invoice status depending on Validation Exception details.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="billingMemberId"></param>
    /// <param name="isBadFileExists"></param>
    /// <param name="processId"></param>
    /// <param name="laFlag"></param>
    public void UpdateFileInvoiceStatus(string fileName, int billingMemberId, bool isBadFileExists, string processId, bool laFlag)
    {
      var parameters = new ObjectParameter[5];
      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.FileNameParameterName, typeof(string)) { Value = fileName };
      parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.BillingMemeberIdParameterName, typeof(int)) { Value = billingMemberId };
      parameters[2] = new ObjectParameter(MiscInvoiceRepositoryConstants.ProcessId, typeof(string)) { Value = processId };
      parameters[3] = new ObjectParameter(MiscInvoiceRepositoryConstants.IsBadFileExists, typeof(int)) { Value = isBadFileExists ? 1 : 0 };
      parameters[4] = new ObjectParameter(MiscInvoiceRepositoryConstants.LaFlag, typeof(int)) { Value = laFlag ? 1 : 0 };
      ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.UpdateInvoiceAndFileStatusFunctionName, parameters);

    }

    /// <summary>
    /// Deletes/truncates the table partitions for PAX Staging tables.
    /// </summary>
    /// <param name="processId"></param>
    public void DeleteFileInvoiceStats(string processId)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.ProcessId, typeof(string)) { Value = processId };

      ExecuteStoredProcedure(InvoiceRepositoryConstants.DeleteFileInvoiceStatsFunctionName, parameters);

    }

    /// <summary>
    /// To update the senderReceiver and fileProcessStartDate in is_file_log
    /// </summary>
    /// <param name="isFileLogId"></param>
    /// <param name="senderReceiver"></param>
    /// <param name="fileProcessStartDate"></param>
    /// <param name="fileStatusId"></param>
    /// <param name="lastUpdatedBy"></param>
    public void UpdateIsFileLog(Guid isFileLogId, int senderReceiver, DateTime? fileProcessStartDate = null, int fileStatusId = 0, int lastUpdatedBy = 0)
    {
      var parameters = new ObjectParameter[5];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.ISFileLogId, typeof(Guid)) { Value = isFileLogId };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.FileSenderReciver, typeof(int)) { Value = senderReceiver };
      parameters[2] = fileProcessStartDate == null ? new ObjectParameter(InvoiceRepositoryConstants.FileProcessStartTime, typeof(System.DateTime)) : new ObjectParameter(InvoiceRepositoryConstants.FileProcessStartTime, fileProcessStartDate.Value);
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.FileStatus, typeof(int)) { Value = fileStatusId };
      parameters[4] = new ObjectParameter(InvoiceRepositoryConstants.LastUpdateBy, typeof(int)) { Value = lastUpdatedBy };
      ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateISFileLogFunctionName, parameters);
    }

    /// <summary>
    /// To inert into Corr Report table
    /// </summary>
    public void InsertToCorrReport(string filePath)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.FilePathParam, typeof(string)) { Value = filePath };

      ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateCorrReportFunctionName, parameters);
    }
    //SCP210204: IS-WEB Outage (done Changes to improve performance)
    /// <summary>
    /// Get invoice using load strategy to be used in readonly header
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="includeBillingBilled">if set to <c>true</c> [include billing billed].</param>
    /// <returns></returns>
    public PaxInvoice GetInvoiceHeader(Guid id,bool includeBillingBilled = false)
    {
     /* var invoiceParameter = new OracleParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, invoiceId)
      {
        OracleDbType = OracleDbType.Raw
      };
      return ExecuteSP(SisStoredProcedures.GetInvoiceHeader //name of stored proc
              , new[] { // INPUT parameters expected by the stored proc
                invoiceParameter
            }
          , r => this.FetchInvoiceHeader(r) //action to be done with the set of results
          );
      //return new List<FieldMetaData>();*/
        //Commented above code and implemented loadStrategy call to get invoice details .
        //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
        var entities = new string[] { LoadStrategy.Entities.Invoice};
        //SCP210204: IS-WEB Outage (done Changes to improve performance)
      if(includeBillingBilled)
      {
        entities = new string[] { LoadStrategy.Entities.Invoice, LoadStrategy.Entities.BillingMember, LoadStrategy.Entities.BilledMember };
      }

        var loadStrategy = new LoadStrategy(string.Join(",", entities));

        string invoiceId = null;
         invoiceId = ConvertUtil.ConvertGuidToString(id);
         var invoices = GetInvoiceLS(loadStrategy, null, null, null, null, null, null, null, invoiceId);
        PaxInvoice invoice = null;
        if (invoices.Count > 0)
        {
            //TODO: Need to throw exception if result count > 1
            invoice = invoices[0];
        }
        return invoice;
    }

    /// <summary>
    /// Execute stored procedure 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sp"></param>
    /// <param name="oraInputParameters"></param>
    /// <param name="fetch"></param>
    /// <returns></returns>
    private T ExecuteSP<T>(StoredProcedure sp, OracleParameter[] oraInputParameters, Func<LoadStrategyResult, T> fetch)
    {
      using (var result = new LoadStrategyResult())
      {
        using (var cmd = Context.CreateStoreCommand(sp.Name, CommandType.StoredProcedure) as OracleCommand)
        {
          cmd.Parameters.AddRange(oraInputParameters);

          // Add result parameters to Oracle Parameter Collection
          foreach (SPResultObject resObj in sp.GetResultSpec())
          {
            var resultParam = new OracleParameter(resObj.ParameterName, OracleDbType.Cursor);
            resultParam.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(resultParam);

            //if the entity is requested, add it to the result
            result.Add(resObj.EntityName, resultParam);
          }

          using (cmd.Connection.CreateConnectionScope())
          {
            //Execute SP

            //Set CommandTimeout value to value given in the Config file 
            //if it NOT in the config then it will be set to default value 0.
            cmd.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
            cmd.ExecuteNonQuery();

            //Allow the caller to populate results
            return fetch(result);
          }
        }
      }
    }

    /// <summary>
    /// Fet data for invoice header
    /// </summary>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    public PaxInvoice FetchInvoiceHeader(LoadStrategyResult loadStrategyResult)
    {
      PaxInvoice invoice = null;
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.Invoice))
      {
        invoice = LoadHeaderEntities(base.EntityBaseObjectSet, loadStrategyResult, null)[0];
      }

      return invoice;
    }

    /// <summary>
    /// Load entities for invoice to be used in readonly header
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <param name="link"></param>
    /// <returns></returns>
    public static List<PaxInvoice> LoadHeaderEntities(ObjectSet<InvoiceBase> objectSet, LoadStrategyResult loadStrategyResult, Action<PaxInvoice> link)
    {
      List<PaxInvoice> invoices = new List<PaxInvoice>();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.Invoice))
      {
        invoices = new PaxMaterializers().InvoiceMaterializer.Materialize(reader).Bind(objectSet).ToList();
        if (!reader.IsClosed)
          reader.Close();
      }

      //Load InvoiceTotalRecord by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.InvoiceTotal) && invoices.Count != 0)
      {

        InvoiceTotalRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<InvoiceTotal>(), loadStrategyResult, null);
      }

      //Load InvoiceTotalVat by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(GetInvoiceHeaderSP.ListingCurrencyEntity) && invoices.Count != 0)
      {
        CurrencyRepository.LoadEntities(objectSet.Context.CreateObjectSet<Currency>(), loadStrategyResult, null, GetInvoiceHeaderSP.ListingCurrencyEntity);
      }
      //Load MemberLocationInformation by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(GetInvoiceHeaderSP.BilledMemberEntity) && invoices.Count != 0)
      {
        MemberRepository.LoadEntities(objectSet.Context.CreateObjectSet<Member>(),
                                                               loadStrategyResult,
                                                               null,
                                                               GetInvoiceHeaderSP.BilledMemberEntity);
      }
      return invoices;
    }

    /// <summary>
    /// Funtion used to add listing currency object in pax Invoice object
    /// </summary>
    /// <param name="cur"></param>
    /// <param name="invoices"></param>
    public static void AddCurrency(Currency cur, List<PaxInvoice> invoices)
    {
      invoices.First(i => i.ListingCurrencyId == cur.Id).ListingCurrency = cur;
    }


    /// <summary>
    /// Updates the multiple invoice status.
    /// </summary>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCategory">The billing category.</param>
    /// <param name="miscLocationCode">The misc location code.</param>
    public void UpdateInvoiceStatus(int billingYear, int billingMonth, int billingPeriod, int billedMemberId, int billingCategory, string miscLocationCode = null)
    {
        var parameters = new ObjectParameter[6];
        parameters[0] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int)) { Value = billedMemberId };
        parameters[1] = new ObjectParameter("BILLING_CATEGORY_ID_I", typeof(int)) { Value = billingCategory };
        parameters[2] = new ObjectParameter("BILLING_YEAR_I", typeof(int)) { Value = billingYear };
        parameters[3] = new ObjectParameter("BILLING_MONTH_I", typeof(int)) { Value = billingMonth };
        parameters[4] = new ObjectParameter("BILLING_PERIOD_I", typeof(int)) { Value = billingPeriod };
        parameters[5] = new ObjectParameter("MISC_LOCATION_CODE_I", typeof(string)) { Value = miscLocationCode };
        ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateInvoiceStatusFunctionName, parameters);
    }

    /// <summary>
    /// Get form DE Invoice Details.
    /// </summary>
    /// <param name="where"> This will add the condition to linq.</param>
    /// <returns>Invoice.</returns>
    public PaxInvoice GetFormDEInvoice(Expression<Func<PaxInvoice, bool>> where)
    {
      var invoice = EntityObjectQuery
        .Include("SamplingFormEDetails")
        .FirstOrDefault(where);

      return invoice;

    }

    /// <summary>
    /// Get the sampling constatnt of the linked form F
    /// </summary>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <param name="provisionalBillingMonth"></param>
    /// <param name="provisionalBillingYear"></param>
    /// <returns></returns>
    public SamplingConstantDetails GetFormFSamplingConstant(int billingMemberId, int billedMemberId, int provisionalBillingMonth, int provisionalBillingYear)
    {
      var parameters = new ObjectParameter[7];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.BillingMemberIdParameterName, typeof(int)) { Value = billingMemberId };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.BilledMemberIdParameterName, typeof(int)) { Value = billedMemberId };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.ProvBillingMonthParameterName, typeof(int)) { Value = provisionalBillingMonth };
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.ProvBillingYearParameterName, typeof(int)) { Value = provisionalBillingYear };

      parameters[4] = new ObjectParameter(InvoiceRepositoryConstants.ErrorCodeParameterName, typeof(string));
      parameters[5] = new ObjectParameter(InvoiceRepositoryConstants.SamplingConstantParameterName, typeof(double));
      parameters[6] = new ObjectParameter(InvoiceRepositoryConstants.IsLinkingSucessfulParameterName, typeof(int));

      ExecuteStoredProcedure(InvoiceRepositoryConstants.GetFormFSamplingConstantFunctionName, parameters);

      var samplingConstantDetail = new SamplingConstantDetails();
      samplingConstantDetail.ErrorMessage = parameters[4].Value.ToString();
      samplingConstantDetail.SamplingConstant = parameters[5].Value.ToString() == "" ? 0 : double.Parse(parameters[5].Value.ToString());
      samplingConstantDetail.IsFormDataFound = int.Parse(parameters[6].Value.ToString()) == 0 ? false : true;
      return samplingConstantDetail;
    }

    #region PAX INVOICE Audit

    /// <summary>
    /// Singles the specified transaction id.
    /// </summary>
    /// <param name="transactionId"></param>
    /// <param name="transactionType"></param>
    /// <returns></returns>
    public PaxAuditTrail AuditSingle(Guid transactionId, string transactionType = null)
    {
      var entities = new[]
                       {
                         LoadStrategy.PaxEntities.PrimeCoupon, LoadStrategy.PaxEntities.RejectionMemo, LoadStrategy.PaxEntities.Correspondence, LoadStrategy.PaxEntities.BillingMemo,
                         LoadStrategy.PaxEntities.PaxInvoice, LoadStrategy.PaxEntities.FormDCoupon, LoadStrategy.PaxEntities.RejectionMemoCoupon, LoadStrategy.PaxEntities.BillingMemoCoupon,
                         LoadStrategy.PaxEntities.Members, LoadStrategy.PaxEntities.PrimecouponTax, LoadStrategy.PaxEntities.PrimecouponVat, LoadStrategy.PaxEntities.PMAttachment, 
                         LoadStrategy.PaxEntities.RejectionMemoVAT, LoadStrategy.PaxEntities.RMAttachment, LoadStrategy.PaxEntities.RMCouponTax, LoadStrategy.PaxEntities.RMCouponVAT, 
                         LoadStrategy.PaxEntities.RMCouponAttachment, LoadStrategy.PaxEntities.BMVAT, LoadStrategy.PaxEntities.BMAttachment, LoadStrategy.PaxEntities.BillingMemoCoupon, 
                         LoadStrategy.PaxEntities.BMCouponTax, LoadStrategy.PaxEntities.BMCouponVAT, LoadStrategy.PaxEntities.BMCouponATTACHMENT, LoadStrategy.PaxEntities.Currency,
                         LoadStrategy.Entities.SamplingFormC, LoadStrategy.Entities.SamplingFormCDetails,LoadStrategy.Entities.SamplingFormCRecordAttachment,
                         LoadStrategy.Entities.CreditMemo, LoadStrategy.Entities.CreditMemoCoupon, LoadStrategy.Entities.CreditMemoAttachments, LoadStrategy.Entities.CreditMemoCouponAttachments,
                         LoadStrategy.Entities.SamplingFormDAttachment, LoadStrategy.Entities.CorrespondenceAttachment
                       };

      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      var invoiceIdStr = ConvertUtil.ConvertGuidToString(transactionId);

      return GetPaxInvoiceAuditLs(loadStrategy, invoiceIdStr, transactionType);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="loadStrategy"></param>
    /// <param name="transactionId"></param>
    /// <param name="transactionType"></param>
    /// <returns></returns>
    private PaxAuditTrail GetPaxInvoiceAuditLs(LoadStrategy loadStrategy, string transactionId, string transactionType)
    {

      return ExecuteLoadsAuditSP(SisStoredProcedures.GetAuditPaxInvoice,
                            loadStrategy,
                            new[]
                              {
                                new OracleParameter(InvoiceRepositoryConstants.PaxInvoiceTransactionIdParameterName, transactionId ?? null),
                                new OracleParameter(InvoiceRepositoryConstants.PaxInvoiceTransactionTypeParameterName, transactionType ?? null)
                              },
                            this.FetchAuditRecord);
    }

    /// <summary>
    /// Returns multiple records extracted from the result set.
    /// This is done by calling the right repository to populate the object set in the repository.
    /// </summary>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    private PaxAuditTrail FetchAuditRecord(LoadStrategyResult loadStrategyResult)
    {
      PaxAuditTrail auditTrail = null;
      if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.PaxInvoice))
      {
        auditTrail = LoadAuditEntities(EntityBaseObjectSet, loadStrategyResult, null);
      }

      return auditTrail;
    }


    public static PaxAuditTrail LoadAuditEntities(ObjectSet<InvoiceBase> objectSet, LoadStrategyResult loadStrategyResult, Action<PaxInvoice> link)
    {
      var auditTrail = new PaxAuditTrail();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.PaxInvoice))
      {
        // first result set includes the category
        auditTrail.Invoices = new PaxMaterializers().PaxInvoiceAuditMaterializer.Materialize(reader).Bind(objectSet).ToList();
        if (!reader.IsClosed)
          reader.Close();
      }


      //invoices.Add(new PaxInvoice());
      //Load PrimeCouponRecord by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.PrimeCoupon) && auditTrail.Invoices.Count != 0)
      {
        CouponRecordRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<PrimeCoupon>(), loadStrategyResult,
                                            c => c.Invoice = auditTrail.Invoices.Find(i => i.Id == c.InvoiceId));
        //The fetched child records should use the Parent entities.
      }

      //Load SourceCode by calling respective LoadEntities method

      if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.FormDCoupon) && auditTrail.Invoices.Count != 0)
      {
        SamplingFormDRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<SamplingFormDRecord>(),
                                                               loadStrategyResult,
                                                               null,
                                                               LoadStrategy.PaxEntities.FormDCoupon);
      }



      if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.RejectionMemo) && auditTrail.Invoices.Count != 0)
      {
        RejectionMemoRecordRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<RejectionMemo>(),
                                                               loadStrategyResult,
                                                               null,
                                                               LoadStrategy.PaxEntities.RejectionMemo);
      }

      if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.BillingMemo) && auditTrail.Invoices.Count != 0)
      {
        BillingMemoRecordRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<BillingMemo>(),
                                                      loadStrategyResult,
                                                      null,
                                                      LoadStrategy.PaxEntities.BillingMemo);
      }

      //Load BilledMember by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.Members) && auditTrail.Invoices.Count != 0)
      {
        MemberRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<Member>(),
                                                               loadStrategyResult,
                                                               null,
                                                               LoadStrategy.PaxEntities.Members);
      }

      //Load ListingCurrency by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.PaxEntities.Currency) && auditTrail.Invoices.Count != 0)
      {
        CurrencyRepository.LoadAuditEntities(objectSet.Context.CreateObjectSet<Currency>(),
                                                               loadStrategyResult,
                                                               null,
                                                               LoadStrategy.PaxEntities.Currency);
      }

      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormC))
      {
        auditTrail.SamplingFormC = SamplingFormCRepository.LoadEntities(objectSet.Context.CreateObjectSet<SamplingFormC>(), loadStrategyResult, null);
      }


      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CreditMemo) && auditTrail.Invoices.Count != 0)
      {
        CreditMemoRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<CreditMemo>(), loadStrategyResult, null);
      }

      return auditTrail;
    }

    #endregion

    /// <summary>
    /// This method will return list of processed invoice details for given criteria.
    /// </summary>
    /// <param name="memberId">ID of member who is creating invoices</param>
    /// <param name="clearanceMonth">clearance month</param>
    /// <param name="period">period</param>
    /// <returns>list of processed invoice details</returns>
    public List<ProcessedInvoiceDetail> GetProcessedInvoiceDetails(int memberId, string clearanceMonth, int period)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter(ProcessedInvoiceDetailConstants.MemberIdParameterName, memberId);
      parameters[1] = new ObjectParameter(ProcessedInvoiceDetailConstants.PeriodNoParameterName, period);
      parameters[2] = new ObjectParameter(ProcessedInvoiceDetailConstants.ClearanceMonthParameterName, clearanceMonth);

      var invoiceDetailList = ExecuteStoredFunction<ProcessedInvoiceDetail>(ProcessedInvoiceDetailConstants.GetProcessedInvoiceDetailsFunctionName, parameters);
      return invoiceDetailList.ToList();
    }


    /// <summary>
    /// Gets the invoices with billing memo record for report.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    public List<PaxInvoice> GetInvoicesWithBillingMemoRecordForReport(string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null, string id = null)
    {
      var entities = new string[] { 
        LoadStrategy.Entities.BillingMemo, 
        LoadStrategy.Entities.BillingMember,
        LoadStrategy.Entities.BilledMember, 
        LoadStrategy.Entities.BillingMemoAttachments ,
        LoadStrategy.Entities.BillingMemoVat,
        LoadStrategy.Entities.BillingMemoCoupon,
        LoadStrategy.Entities.BillingMemoCouponTax,
        LoadStrategy.Entities.BillingMemoCouponVat 
      };

      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      var invoices = GetInvoiceLS(loadStrategy, invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, id);
      return invoices;
    }

    /// <summary>
    ///Gets  Pax invoices from InvoiceModelList
    /// </summary>
    /// <param name="invoiceModelList"></param>
    /// <returns></returns>
    public List<PaxInvoice> GetPaxInvoicesFromModel(IEnumerable<InvoiceModel> invoiceModelList)
    {
      var paxInvoices = new List<PaxInvoice>();
      if (invoiceModelList != null)
      {
        foreach (InvoiceModel invoiceModel in invoiceModelList)
        {
          if (invoiceModel.Invoice != null)
          {
            paxInvoices.Add(invoiceModel.Invoice);
          }
        }
      }
      return paxInvoices;
    }

    /// <summary>
    /// Gets  Pax Sampling Form C List from InvoiceModelList
    /// </summary>
    /// <param name="invoiceModelList"></param>
    /// <returns></returns>
    public List<SamplingFormC> GetSamplingFormCListFromModel(IEnumerable<InvoiceModel> invoiceModelList)
    {
      var samplingFormCs = new List<SamplingFormC>();
      if (invoiceModelList != null)
      {
        foreach (InvoiceModel invoiceModel in invoiceModelList)
        {
          if (invoiceModel.SamplingFormC != null)
          {
            samplingFormCs.Add(invoiceModel.SamplingFormC);
          }
        }
      }
      return samplingFormCs;
    }

    /// <summary>
    /// Checks whether invoices are blocked due to some pending processes
    /// </summary>
    /// <param name="paxInvoiceBases"></param>
    /// <returns></returns>
    public bool ValidatePaxInvoices(IEnumerable<InvoiceBase> paxInvoiceBases)
    {
      return (from paxInvoice in paxInvoiceBases
              where
                paxInvoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling ||
                paxInvoice.InvoiceStatus == InvoiceStatusType.Claimed
              select paxInvoice).Count() <= 0;

    }

    public string ValidateMemo(Guid invoiceId, int billingCode)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.BillingCodeParameterName, typeof(int)) { Value = billingCode };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.ErrorCodeParameterName, typeof(string));

      ExecuteStoredProcedure(InvoiceRepositoryConstants.ValidateMemoFunctionName, parameters);
      return parameters[2].Value.ToString();
    }

    public void UpdateSourceCodeTotalVat(Guid invoiceId)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };

      ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateSourceCodeVatFunctionName, parameters);
    }

    /// <summary>
    /// Get Invoice Legal PDF path 
    /// </summary>
    /// <param name="invoiceId">Invoice Number </param>
    /// <returns> string of InvoiceLegalPdf </returns>
    public string GetInvoiceLegalPdfPath(Guid invoiceId)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter("INVOICE_NO_I", typeof(Guid))
      {
        Value = invoiceId
      };
      parameters[1] = new ObjectParameter("R_PATH_INV_O", typeof(string));

      ExecuteStoredProcedure("GetLegalInvoicePDFPath", parameters);
      return parameters[1].Value.ToString();
    }

    /// <summary>
    /// This function is used to set invoice status, validation status, is future submission, clearing house, total amount currency. 
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="invoiceStatus"></param>
    /// <param name="validationStatus"></param>
    /// <param name="isFutureSubmission"></param>
    /// <param name="clearingHouse"></param>
    /// <param name="totalAmountInCurrency"></param>
    /// <param name="billingCategory"></param>
    /// <param name="exchangeRate"></param>
    public void SetInvoiceAndValidationStatus(Guid invoiceId, int invoiceStatus, int validationStatus, bool isFutureSubmission = false, string clearingHouse = "", decimal? totalAmountInCurrency = null, int billingCategory = 0, decimal? exchangeRate = null)
    {
        var parameters = new ObjectParameter[8];
        parameters[0] = new ObjectParameter("INVOICE_ID_I", typeof(Guid)) { Value = invoiceId };
        parameters[1] = new ObjectParameter("INVOICE_STATUS_ID_I", typeof(Int32)) { Value = invoiceStatus };
        parameters[2] = new ObjectParameter("VALIDATION_STATUS_ID_I", typeof(Int32)) { Value = validationStatus };
        parameters[3] = new ObjectParameter("IS_FUTURE_SUBMISSION_I", typeof(Int32)) { Value = isFutureSubmission ? 1 : 0 };
        parameters[4] = new ObjectParameter("CLEARING_HOUSE_I", typeof(string)) { Value = clearingHouse };
        //SCP345230: ICH Settlement Error - SIS Production
        parameters[5] = new ObjectParameter("TOTAL_AMT_IN_CLEAR_CURRENCY_I", typeof(decimal?)) { Value = totalAmountInCurrency };
        parameters[6] = new ObjectParameter("BILLING_CATEGORY_I", typeof(Int32)) { Value = billingCategory };
        parameters[7] = new ObjectParameter("EXCHANGE_RATE_I", typeof(decimal?)) { Value = exchangeRate };
        ExecuteStoredProcedure("SetInvoiceAndValidationStatus", parameters);
    }

    public void AddWebValiadtionErrorEntry(WebValidationError webValidationError)
       {
           var parameters = new ObjectParameter[4];
           parameters[0] = new ObjectParameter("INVOICE_ID_I", typeof(Guid)) { Value = webValidationError.InvoiceId };
           parameters[1] = new ObjectParameter("ERR_CODE_I", typeof(string)) { Value = webValidationError.ErrorCode };
           parameters[2] = new ObjectParameter("ERR_DESC_I", typeof(string)) { Value = webValidationError.ErrorDescription };
           parameters[3] = new ObjectParameter("LAST_UPDATED_BY_I", typeof(Int32)) { Value = webValidationError.LastUpdatedBy };

           ExecuteStoredProcedure("InsertWebValidationError", parameters);
       }


    public void DeleteWebValiadtionError(Guid webValidationErrorId)
    {
        var parameters = new ObjectParameter[1];
        parameters[0] = new ObjectParameter("VALIDATION_ERROR_ID_I", typeof(Guid)) { Value = webValidationErrorId };
        ExecuteStoredProcedure("DeleteWebValidationError", parameters);
    }


    public int IsValidBatchSequenceNo(Guid invoiceId, int batchRecordSequenceNo, int batchSequenceNo, Guid memoId, int sourceCodeId)
    {
      var parameters = new ObjectParameter[6];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.BatchRecordSequenceNoParameterName, typeof(int)) { Value = batchRecordSequenceNo };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.BatchSequenceNoParameterName, typeof(int)) { Value = batchSequenceNo };
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.MemoId, typeof(Guid)) { Value = memoId };
      parameters[4] = new ObjectParameter("SOURCE_CODE_ID_I", typeof(int)) { Value = sourceCodeId };
      parameters[5] = new ObjectParameter(InvoiceRepositoryConstants.IsUniqueNoParameterName, typeof(int));

      ExecuteStoredProcedure(InvoiceRepositoryConstants.IsValidBatchSequenceNoFunctionName, parameters);

      return Convert.ToInt32(parameters[5].Value);
    }

    public void AddFileLogEntry(IsInputFile isInputFile, bool isConsolidatedFile = false, int usadaDataExpRespHours = 0)
    {
      var parameters = new ObjectParameter[24];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.ISFileLogId, typeof(Guid)) { Value = isInputFile.Id };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.FileName, typeof(string)) { Value = isInputFile.FileName };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.InCommingOutGoing, typeof(int)) { Value = isInputFile.IsIncoming };
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.FileSenderReceiver, typeof(int)) { Value = isInputFile.SenderReceiver };
      parameters[4] = new ObjectParameter(InvoiceRepositoryConstants.FileDateTime, typeof(DateTime)) { Value = isInputFile.FileDate };
      parameters[5] = new ObjectParameter(InvoiceRepositoryConstants.BillingPeriod, typeof(int)) { Value = isInputFile.BillingPeriod };
      parameters[6] = new ObjectParameter(InvoiceRepositoryConstants.BillingMonth, typeof(int)) { Value = isInputFile.BillingMonth };
      parameters[7] = new ObjectParameter(InvoiceRepositoryConstants.FileReceivedSentOn, typeof(DateTime)) { Value = isInputFile.ReceivedDate };
      parameters[8] = new ObjectParameter(InvoiceRepositoryConstants.FileSenderReceiverIp, typeof(string)) { Value = isInputFile.SenderReceiverIP };
      parameters[9] = new ObjectParameter(InvoiceRepositoryConstants.FileLocation, typeof(string)) { Value = isInputFile.FileLocation };
      parameters[10] = new ObjectParameter(InvoiceRepositoryConstants.FileStatusId, typeof(int)) { Value = isInputFile.FileStatusId };
      parameters[11] = new ObjectParameter(InvoiceRepositoryConstants.FileFormatId, typeof(int)) { Value = isInputFile.FileFormatId };
      parameters[12] = new ObjectParameter(InvoiceRepositoryConstants.BillingCatId, typeof(int)) { Value = isInputFile.BillingCategory };
      parameters[13] = new ObjectParameter(InvoiceRepositoryConstants.BillingYear, typeof(int)) { Value = isInputFile.BillingYear };
      parameters[14] = new ObjectParameter(InvoiceRepositoryConstants.SenderReciverType, typeof(int)) { Value = isInputFile.SenderRecieverType };
      parameters[15] = new ObjectParameter(InvoiceRepositoryConstants.FileVersion, typeof(int)) { Value = isInputFile.FileVersion };
      parameters[16] = new ObjectParameter(InvoiceRepositoryConstants.OutputDeleveryMethod, typeof(int)) { Value = isInputFile.OutputFileDeliveryMethodId };
      parameters[17] = new ObjectParameter(InvoiceRepositoryConstants.ExpectedRespTime, typeof(int)) { Value = isInputFile.ExpectedResponseTime};
      parameters[18] = new ObjectParameter(InvoiceRepositoryConstants.FileStartTime, typeof(DateTime)) { Value = isInputFile.FileProcessStartTime };
      parameters[19] = new ObjectParameter(InvoiceRepositoryConstants.FileEndTime, typeof(DateTime)) { Value = isInputFile.FileProcessEndTime };
      parameters[20] = new ObjectParameter(InvoiceRepositoryConstants.IsConsolidated, typeof(int)) { Value = isConsolidatedFile ? 1 : 0 };
      parameters[21] = new ObjectParameter(InvoiceRepositoryConstants.UsageData, typeof(int)) { Value = usadaDataExpRespHours };
      //CMP#622 : MISC Outputs Split as per Location ID
      parameters[22] = new ObjectParameter(InvoiceRepositoryConstants.MiscLocationforFile, typeof(string)) { Value = isInputFile.MiscLocationCode };
      //CMP#608 : Pass value of LAST_UPDATED_BY
      parameters[23] = new ObjectParameter(InvoiceRepositoryConstants.LastUpdatedBy, typeof(string)) { Value = isInputFile.LastUpdatedBy };
      ExecuteStoredProcedure(InvoiceRepositoryConstants.InsertoIsFileLogFunction, parameters);

    }

    /// <summary>
    /// Method to add/update file processing time to IS_FILE_LOG_DETAIL table for the given file and for the given process name.
    /// </summary>
    /// <param name="isInputFileId"> File Id</param>
    /// <param name="processName">Process Name</param>
    /// <param name="fileName">File Name</param>
    /// <param name="billingCategory">Billing Category</param>
    /// <param name="fileSize">File Size</param>
    /// <param name="invoiceCount">Invoice Count</param>
    /// <param name="primecouponCount">Prime Coupon count</param>
    /// <param name="primeCouponTaxCount">Coupon Tax Breakdown count</param>
    /// <param name="rejectionMemocount">Rejection Memo count</param>
    /// <param name="rmCouponBdnCount">Rejection Memo Coupon Breakdown count</param>
    
    public void PopulateFileProcessingStats(Guid isInputFileId, string processName, string fileName, string billingCategory, string fileSize, int invoiceCount, int primecouponCount,
                                             int primeCouponTaxCount, int rejectionMemocount, int rmCouponBdnCount,int lineItemCnt =0,int lineItemDetCnt=0, int fieldValueCnt=0)
    {
      var parameters = new ObjectParameter[13];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.FileLogId, typeof(Guid)) { Value = isInputFileId };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.ProcessName, typeof(string)) { Value = processName };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.Filename, typeof(string)) { Value = fileName };
      parameters[3] = new ObjectParameter(InvoiceRepositoryConstants.BillingCategory, typeof(string)) { Value = billingCategory };
      parameters[4] = new ObjectParameter(InvoiceRepositoryConstants.FileSize, typeof(string)) { Value = fileSize };
      parameters[5] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceCount, typeof(int)) { Value = invoiceCount };
      parameters[6] = new ObjectParameter(InvoiceRepositoryConstants.PrimeCouponCount, typeof(int)) { Value = primecouponCount };
      parameters[7] = new ObjectParameter(InvoiceRepositoryConstants.CpnTaxBreakdownCount, typeof(int)) { Value = primeCouponTaxCount };
      parameters[8] = new ObjectParameter(InvoiceRepositoryConstants.RmCount, typeof(int)) { Value = rejectionMemocount };
      parameters[9] = new ObjectParameter(InvoiceRepositoryConstants.RmCpnBreakdownCount, typeof(int)) { Value = rmCouponBdnCount };
      parameters[10] = new ObjectParameter(InvoiceRepositoryConstants.LineItemCnt, typeof(int)) { Value = lineItemCnt };
      parameters[11] = new ObjectParameter(InvoiceRepositoryConstants.LineItemDetCnt, typeof(int)) { Value = lineItemDetCnt };
      parameters[12] = new ObjectParameter(InvoiceRepositoryConstants.FieldValueCnt, typeof(int)) { Value = fieldValueCnt };
      ExecuteStoredProcedure(InvoiceRepositoryConstants.InsertUpdateFileProcessTime, parameters);

    }// End AddFileProcessingTime()

    /// <summary>
    /// Gets the invoice offline collection data.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <returns></returns>
    public PaxInvoice GetInvoiceDetailsData(string invoiceId = null, string invoiceNumber = null)
    {
      var entities = new string[]
                       {
                          LoadStrategy.Entities.BilledMember, 
                          LoadStrategy.Entities.BillingMember,
                          //SCP#419599 - SRM: SIS: Admin Alert - Error in creating Legal Invoice Archive zip file of CDC - SISPROD -16oct2016
                          //Uncomment below code to load coupon records to generate listing pdf
                          LoadStrategy.Entities.Coupon,
                          LoadStrategy.Entities.RejectionMemo,
                          LoadStrategy.Entities.RejectionMemoCoupon,
                          LoadStrategy.Entities.SamplingFormDRecord,
                          LoadStrategy.Entities.BillingMemo,
                          LoadStrategy.Entities.BillingMemoCoupon,
                          LoadStrategy.Entities.CreditMemo,
                          LoadStrategy.Entities.CreditMemoCoupon,
                          LoadStrategy.Entities.SourceCodeTotal,
                          LoadStrategy.Entities.InvoiceTotal,
                          LoadStrategy.Entities.Invoice,
                          LoadStrategy.Entities.ListingCurrency
                       };
      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      // var invoices = GetInvoiceLS(loadStrategy, invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, invoiceId, invoiceStatusIdstr);
      var invoices = GetInvoiceLS(loadStrategy: loadStrategy, invoiceId: invoiceId, invoiceNumber: invoiceNumber);
      return invoices != null && invoices.Count > 0 ? invoices[0] : null;
    }

    // Added by Priya R. for Auto-Billing invoices 

    /// <summary>
    /// Gets the invoice offline collection data.
    /// </summary>
    /// <param name="invoiceId">The invoice ID.</param>
    /// <returns></returns>
    public PaxInvoice GetInvoiceCouponsData(Guid invoiceId)
    {
      var entities = new string[]
                       {
                          LoadStrategy.Entities.BilledMember, 
                          LoadStrategy.Entities.BillingMember,
                          LoadStrategy.Entities.Coupon,
                          LoadStrategy.Entities.RejectionMemo,
                          LoadStrategy.Entities.SamplingFormDRecord,
                          LoadStrategy.Entities.BillingMemo,
                          LoadStrategy.Entities.CreditMemo,
                          LoadStrategy.Entities.SourceCodeTotal,
                          LoadStrategy.Entities.InvoiceTotal,
                          LoadStrategy.Entities.Invoice,
                          LoadStrategy.Entities.ListingCurrency
                       };
      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      // var invoices = GetInvoiceLS(loadStrategy, invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, invoiceId, invoiceStatusIdstr);
      //SCP178233 - AB invoices not submitted
      var invoices = GetInvoiceLS(loadStrategy: loadStrategy, invoiceId: ConvertUtil.ConvertGuidToString(invoiceId));
      return invoices != null && invoices.Count > 0 ? invoices[0] : null;
    }


    public PaxInvoice GetFormDetailsData(string invoiceId = null, string invoiceNumber = null, int? billingCode = 0)
    {
      var entities = new string[]
                       {
                          LoadStrategy.Entities.BilledMember, 
                          LoadStrategy.Entities.BillingMember,
                          LoadStrategy.Entities.SourceCodeTotal,
                            LoadStrategy.Entities.SamplingFormDRecord,
                           
                          LoadStrategy.Entities.InvoiceTotal,
                          LoadStrategy.Entities.Invoice,
                           LoadStrategy.Entities.ListingCurrency
                       };
      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      var invoices = GetInvoiceLS(loadStrategy, invoiceNumber, null, null, null, null, null, billingCode, invoiceId, null);
      return invoices != null && invoices.Count > 0 ? invoices[0] : null;
    }

    ///// <summary>
    ///// Updates the file log and invoice status depending on Validation Exception details.
    ///// </summary>
    ///// <param name="invoiceId"></param> 
    //public void UpdateInvoiceAndSetLaParameters(Guid invoiceId)
    //{
    //  var parameters = new ObjectParameter[1];
    //  parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, invoiceId);
    //  ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateInvoiceSetLaParametersFunctionName, parameters);

    //}

    public List<AutoBillingPerformanceReportSearchResult> GetAutoBillingPerformanceReportData(int logInMemberid,int entityId, int currencyId, int clearanceMonth, int clearanceYear)
    {
      var parameters = new ObjectParameter[5];
      parameters[0] = new ObjectParameter("ENTITY_CODE_I", typeof(int)) { Value = entityId };
      parameters[1] = new ObjectParameter("BILLING_MONTH_I", typeof(int)) { Value = clearanceMonth };
      parameters[2] = new ObjectParameter("BILLING_YEAR_I", typeof(int)) { Value = clearanceYear };
      parameters[3] = new ObjectParameter("CURRENCY_CODE_I", typeof(int)) { Value = currencyId };
      parameters[4] = new ObjectParameter("LOGIN_ENTITY_ID_I", typeof(int)) { Value = logInMemberid };


      var autoBillingPerformanceReportSearchResult = ExecuteStoredFunction<AutoBillingPerformanceReportSearchResult>("GetAutoBillingPerformanceReportSearchResult", parameters);
      return autoBillingPerformanceReportSearchResult.ToList();

    }

    /// <summary>
    /// To update the IsPurged status of IsFileLogId and unlinked supporting documents for Specified Ids.
    /// </summary>
    /// <param name="fileLogIds"></param>
    /// <param name="purgedStatus"></param>
    /// <param name="isFileLogPurged"></param>
    public void UpdateFileLogPurgedStatus(string fileLogIds, int purgedStatus,int isFileLogPurged)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.FileLogIdsParameterName, typeof(string)) { Value = fileLogIds };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.FileLogIdPurgedStatusParameterName, typeof(int)) { Value = purgedStatus };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.IsFileLogPurgedParameterName, typeof(int)) { Value = isFileLogPurged };

      ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateFileLogPurgedStatusFunctionName, parameters);
    }


    /// <summary>
    /// Get next sequence number for Auto Billing Invoice
    /// </summary>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <returns>
    /// Invoice Sequence Number
    /// </returns>
    public int GetAutoBillingInvoiceNumberSeq(int billingMemberId, int billingYear)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.BillingMemberParam, typeof(int)) { Value = billingMemberId };
      parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.BillingYearParam, typeof(int)) { Value = billingYear };
      parameters[2] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceSequenceNumberParam, typeof(int));

      ExecuteStoredProcedure(InvoiceRepositoryConstants.GetNextOfInvoiceNoSeqParam, parameters);

      return Int32.Parse(parameters[2].Value.ToString());
    }


      public List<InvoiceDeletionAuditReport> GetInvoiceDeletionAuditDetails(AuditDeletedInvoice auditDeletedInvoice)
      {
          var parameters = new ObjectParameter[10];
          parameters[0] = new ObjectParameter("BILLING_CATEGORY_I", typeof(int)) { Value = auditDeletedInvoice.BillingCategoryId };
          parameters[1] = new ObjectParameter("BILLING_YEAR_I", typeof(int)) { Value = auditDeletedInvoice.BillingYear };
          parameters[2] = new ObjectParameter("BILLING_MONTH_I", typeof(int)) { Value = auditDeletedInvoice.BillingMonth };
          parameters[3] = new ObjectParameter("BILLING_PERIOD", typeof(int)) { Value = auditDeletedInvoice.BillingPeriod };
          parameters[4] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value = auditDeletedInvoice.BillingMemberId };
          parameters[5] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int)) { Value = auditDeletedInvoice.BilledMemberId };
          parameters[6] = new ObjectParameter("INVOICE_NO_I", typeof(string)) { Value = auditDeletedInvoice.InvoiceNo };
          parameters[7] = new ObjectParameter("DELETED_BY_I", typeof(string)) { Value = auditDeletedInvoice.DeletedBy };
          parameters[8] = new ObjectParameter("DELETED_FROM_I", typeof(string)) { Value = auditDeletedInvoice.DeletionDateFrom };
          parameters[9] = new ObjectParameter("DELETED_TO_I", typeof(string)) { Value = auditDeletedInvoice.DeletionDateTo };

          var getInvoiceDeletedAudit = ExecuteStoredFunction<InvoiceDeletionAuditReport>("GetInvoiceDeletedAudit", parameters);
          return getInvoiceDeletedAudit.ToList();
      }

    /// <summary>
    /// Update Duplicate Coupon as DU mark. Ref.SCP ID : 94742 - coupons marked as DU duplicate billing
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="billingMemberId"></param>
      public void UpdateDuplicateCouponByInvoiceId(Guid invoiceId, int billingMemberId)
      {

        var parameters = new ObjectParameter[2];
        parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdparamName, typeof(Guid)) { Value = invoiceId };
        parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.BillingMemberIdParamName, typeof(int)) { Value = billingMemberId };

        ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateduplicateCouponDuMarkFunctionName, parameters);
      }


      /// <summary>
      /// SCP85837: PAX CGO Sequence Number
      /// </summary>
      /// <param name="invoiceId"></param>
     
      public bool UpdateTransSeqNoWithInBatch(Guid invoiceId)
      {
          try
          {
              var parameters = new ObjectParameter[2];
              parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdparam, typeof (Guid))
                                  {Value = invoiceId};
              parameters[1] = new ObjectParameter(InvoiceRepositoryConstants.IsUpdate, typeof (int));
              ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdatePaxTransSeqNoWithInBatchFunctionName, parameters);

              var returnValue = parameters[1].Value.ToString();

              return returnValue == "1" ? true : false;
          }
          catch(Exception exception)
          {
              return false;
          }
      }

      //SCP149711:Incorrect Form E UA to 3M
      public bool RecalculateFormE(Guid invoiceId)
      {
        try
        {
          var parameters = new ObjectParameter[2];
          parameters[0] = new ObjectParameter("INVOICE_ID_I", typeof(Guid)) { Value = invoiceId };
          parameters[1] = new ObjectParameter("R_IS_VALIDATE", typeof(int));
          ExecuteStoredProcedure("RecalculateFormE", parameters);
          return parameters[1].Value.ToString() == "1" ? true : false;
        }
        catch (Exception exception)
        {
          return false;
        }
      }


      /// <summary>
      /// Finalize supporting documents for Daily output i.e. update TargetDate, AttachmentIndVal and No.OfAttachment for each invoice to be included in daily output. 
      /// En-queue members who have opted for misc daily output to Daily Output Gen queues.
      /// </summary>
      /// <param name="targetDate"></param>
      /// CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3 [Remove Nill file parameters]
      public List<InvoicePendingForDailyDelivery> FinalizeSupportingDocumentForDailyOutput(DateTime targetDate)
      {
        var parameters = new ObjectParameter[3];
        parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.ParameterNameTargetDate, typeof(DateTime)) { Value = targetDate };

        var invoicesPendingForDailyDelivery = ExecuteStoredFunction<InvoicePendingForDailyDelivery>(InvoiceRepositoryConstants.FinalizeSupportingDocumentForDailyOutputFunctionName, parameters);

        return invoicesPendingForDailyDelivery.ToList();
      }

      public void PopulateInvoiceReportStats(Guid invoiceId)
      {
        var parameters = new ObjectParameter[1];
        parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdInputParameterName, typeof(Guid)) { Value = invoiceId };
        ExecuteStoredProcedure(InvoiceRepositoryConstants.PopulateInvoiceReportStats, parameters);
      }

      /// <summary>
      /// This function is used to en-queue message for offline collection download
      /// </summary>
      /// <param name="memberId"></param>
      /// <param name="billingPeriod"></param>
      /// <param name="billingMonth"></param>
      /// <param name="billingYear"></param>
      /// <param name="invoiceStatusId"></param>
      /// <param name="billingCategory"></param>
      /// <param name="delays"></param>
      /// SCP419599: SIS: Admin Alert - Error in creating Legal Invoice Archive zip file of CDC - SISPROD -16oct2016
      public void EnqueueOfflineCollectionDownload(int memberId, int billingPeriod, int billingMonth, int billingYear, int invoiceStatusId, string billingCategory, int delays)
      {
          var parameters = new ObjectParameter[7];
          parameters[0] = new ObjectParameter("MEMBER_ID_I", typeof(int)) { Value = memberId };
          parameters[1] = new ObjectParameter("BILLING_PERIOD_I", typeof(int)) { Value = billingPeriod };
          parameters[2] = new ObjectParameter("BILLING_MONTH_I", typeof(int)) { Value = billingMonth };
          parameters[3] = new ObjectParameter("BILLING_YEAR_I", typeof(int)) { Value = billingYear };
          parameters[4] = new ObjectParameter("INVOICE_STATUS_I", typeof(int)) { Value = invoiceStatusId };
          parameters[5] = new ObjectParameter("OPTIONS_I", typeof(string)) { Value = billingCategory };
          parameters[6] = new ObjectParameter("DELAY_I", typeof(int)) { Value = delays };

          ExecuteStoredProcedure("QueueOARDownlaod", parameters);
      }

      /// <summary>
      /// This function is used to validate source code for rejection memo using stored procedure.
      /// </summary>
      /// <param name="criteria"></param>
      /// <returns></returns>
      //CMP614: Source Code Validation for PAX RMs
      public String ValidateRMSourceCode(RMSourceCodeValidationCriteria criteria)
      {
        var parameters = new ObjectParameter[13];

        parameters[0] = new ObjectParameter("YOUR_INVOICE_NUMBER_I", typeof(String)) { Value = criteria.InvoiceNumber };
        parameters[1] = new ObjectParameter("BILLING_MONTH_I", typeof(Int32)) { Value = criteria.BillingMonth };
        parameters[2] = new ObjectParameter("BILLING_YEAR_I", typeof(Int32)) { Value = criteria.BillingYear };
        parameters[3] = new ObjectParameter("BILLING_PERIOD_I", typeof(Int32)) { Value = criteria.BillingPeriod };
        parameters[4] = new ObjectParameter("FIM_BM_CM_NUMBER_I", typeof(Int64)) { Value = criteria.FimBMCMNumber };
        parameters[5] = new ObjectParameter("FIM_COUPON_NUMBER_I", typeof(Int32)) { Value = criteria.FimCouponNumber };
        parameters[6] = new ObjectParameter("YOUR_RM_NUMBER_I", typeof(String)) { Value = criteria.RejectionMemoNumber };
        parameters[7] = new ObjectParameter("REJECTION_STAGE_I", typeof(Int32)) { Value = criteria.RejectionStage };
        parameters[8] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(Int32)) { Value = criteria.BillingMemberId };
        parameters[9] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(Int32)) { Value = criteria.BilledMemberId };
        parameters[10] = new ObjectParameter("SOURCE_CODE_I", typeof(Int32)) { Value = criteria.SourceCode };
        parameters[11] = new ObjectParameter("IGNORE_VALIDATION_SOURCE_CODE", typeof(Int32)) { Value = criteria.IgnoreValidationOnRMSourceCodes };
        parameters[12] = new ObjectParameter("ERROR_CODE_O", typeof(string));

        //Execute stored procedure and fetch data based on criteria.
        ExecuteStoredProcedure("RMSourceCodeValidation", parameters);

        return parameters[12].Value.ToString();
      }

      /// <summary>
      /// This function is used to check csv loader, given file has been processed or not
      /// </summary>
      /// <param name="filePath"></param>
      /// <param name="senderReceiverId"></param>
      /// <returns>If IS_SUCCESS_O is "1" then given file didn't process before otherwise already processed</returns>
      //SCP390945: Error in prime file validation - WestJet.
      public int CheckCSVLoaderInProcess(string filePath, int senderReceiverId)
      {
        var parameters = new ObjectParameter[3];

        parameters[0] = new ObjectParameter("FILE_PATH_I", typeof(String)) { Value = filePath };
        parameters[1] = new ObjectParameter("SENDER_RECEIVER_ID_I", typeof(Int32)) { Value = senderReceiverId };
        parameters[2] = new ObjectParameter("IS_SUCCESS_O", typeof(Int32));

        //Execute stored procedure and fetch data based on criteria.
        ExecuteStoredProcedure("CheckLoaderInProcess", parameters);

        return Convert.ToInt32(parameters[2].Value);
      }

      /// <summary>
      /// SCP391025 : Validation and Loader process segregation based on billing category
      /// Enqueue message for CSV to Upload into database, Queue will be chosen based on billing category.
      /// This method will be used to call "PROC_ENQUEUE_LOADER_FILE" stored procedure.
      /// </summary>
      /// <param name="message">message object.</param>
      /// <returns>ture if successfully enqueued/false in case of error</returns>
      public bool Enqueue(CsvUploadQueueMessage message, Guid isFileLogId)
      {
        var parameters = new ObjectParameter[13];

        parameters[0] = new ObjectParameter("FILE_PATH_I", typeof(String)) { Value = message.InputFilePath };
        parameters[1] = new ObjectParameter("SENDER_RECEIVER_ID_I", typeof(Int32)) { Value = message.SenderRecieverId };
        parameters[2] = new ObjectParameter("OUTPUT_CSV_PATH_I", typeof(String)) { Value = message.OutPutCsvFilePath };
        parameters[3] = new ObjectParameter("BILLING_CATEGORY_I", typeof(String)) { Value = message.BillingCategory };
        parameters[4] = new ObjectParameter("BILLING_PERIOD_I", typeof(Int32)) { Value = message.BillingPeriod };
        parameters[5] = new ObjectParameter("BILLING_MONTH_I", typeof(Int32)) { Value = message.BillingMonth };
        parameters[6] = new ObjectParameter("BILLING_YEAR_I", typeof(Int32)) { Value = message.BillingYear };
        parameters[7] = new ObjectParameter("BILLING_CATEGORY_ID_I", typeof(Int32)) { Value = message.BillingCategoryId };
        parameters[8] = new ObjectParameter("FILE_DATE_I", typeof(DateTime)) { Value = message.FileDateTime };
        parameters[9] = new ObjectParameter("FILE_FORMAT_I", typeof(Int32)) { Value = message.FileFormatId };
        parameters[10] = new ObjectParameter("FILE_SIZE_I", typeof(Int64)) { Value = message.FileSize };
        parameters[11] = new ObjectParameter("IS_FILE_LOG_ID_I", typeof(Guid)) { Value = isFileLogId };
        parameters[12] = new ObjectParameter("SUCCESS_O", typeof(Int32));

        //Execute stored procedure and fetch data based on criteria.
        ExecuteStoredProcedure("EnqueueCSVUploadMsg", parameters);

        return Convert.ToInt32(parameters[12].Value) > 0 ? true : false;
      }

      /// <summary>
      /// Method to update FILE_PROCESS_END_TIME in IS_FILE_LOG table.
      /// SCP#432666: SRM: Long time for loading the file
      /// </summary>
      /// <param name="inputIsFileLogId">IS_FILE_LOG_ID of input file</param>
      public void UpdateFileProcessEndTimeInIsFileLog(Guid inputIsFileLogId)
      {
          var parameters = new ObjectParameter[1];
          parameters[0] = new ObjectParameter("IS_FILE_LOG_ID_I", typeof(Guid)) { Value = inputIsFileLogId };
          ExecuteStoredProcedure("UpdateFileProcessEndTime", parameters);
      }
  }
}

