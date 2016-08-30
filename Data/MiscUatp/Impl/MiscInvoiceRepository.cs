using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Data.Cargo.Impl;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MemberProfile.Impl;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.BillingHistory;
using Iata.IS.Model.MiscUatp.Common;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MiscUatp.Impl
{
  public class MiscInvoiceRepository : RepositoryEx<MiscUatpInvoice, InvoiceBase>, IMiscInvoiceRepository
  {

    /// <summary>
    /// Initializes a new instance of the <see cref="MiscInvoiceRepository"/> class.
    /// </summary>
    public MiscInvoiceRepository()
    {
      InitializeObjectSet();
    }

    /// <summary>
    /// Initializes the object set.
    /// </summary>
    public override sealed void InitializeObjectSet()
    {
      EntityBaseObjectSet = Context.CreateObjectSet<InvoiceBase>();
      EntityObjectQuery = EntityBaseObjectSet.OfType<MiscUatpInvoice>();
    }
    /// <summary>
    /// Get Billing History for search
    /// </summary>
    /// <param name="invoiceSearchCriteria">Invoice search criteria</param>
    /// <param name="billingCategoryId">Billing catogory</param>
    /// <returns>List of Billing history for search criteria and billingcatogoryId</returns>
    public List<MiscBillingHistorySearchResult> GetBillingHistorySearchResult(InvoiceSearchCriteria invoiceSearchCriteria, int billingCategoryId)
    {
      var parameters = new ObjectParameter[12];

      parameters[0] = new ObjectParameter("INVOICE_NO_I", typeof(String)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.InvoiceNumber : string.Empty };
      parameters[1] = new ObjectParameter("BILLING_YEAR_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingYear : 0 };
      parameters[2] = new ObjectParameter("BILLING_MONTH_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingMonth : 0 };
      parameters[3] = new ObjectParameter("BILLING_PERIOD_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingPeriod : 0 };
      parameters[4] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BilledMemberId : 0 };
      parameters[5] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingMemberId : 0 };
      parameters[6] = new ObjectParameter("CHARGE_CATEGORY_ID_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.ChargeCategoryId : 0 };
      parameters[7] = new ObjectParameter("REJECTION_STAGE_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.RejectionStageId : 0 };
      parameters[8] = new ObjectParameter("TRANSACTION_STATUS_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.TransactionStatusId : 0 };
      parameters[9] = new ObjectParameter("BILLING_TYPE_I", typeof(int)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.BillingTypeId : 0 };
      parameters[10] = new ObjectParameter("BILLING_CATEGORY_ID_I", typeof(int)) { Value = billingCategoryId };
      parameters[11] = new ObjectParameter("SELECTED_LOCATION_I", typeof(String)) { Value = invoiceSearchCriteria != null ? invoiceSearchCriteria.MemberLocation : null }; //CMP #655: IS-WEB Display per Location ID
      var invoices = ExecuteStoredFunction<MiscBillingHistorySearchResult>("GetMiscBillingHistorySearchResult", parameters);

      return invoices.ToList();
    }

    /// <summary>
    /// To generate Atcan report for Receivables
    /// </summary>
    /// <param name="billingperiod">billingperiod</param>
    /// <param name="billingMonth">billingMonth</param>
    /// <param name="billingYear">billingYear</param>
    public void GenerateAtcanStatmentForReceivable(int billingperiod, int billingMonth, int billingYear)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter("BILLING_PERIOD_I", typeof(int)) { Value = billingperiod };
      parameters[1] = new ObjectParameter("BILLING_YEAR_I", typeof(int)) { Value = billingYear };
      parameters[2] = new ObjectParameter("BILLING_MONTH_I", typeof(int)) { Value = billingMonth };

      ExecuteStoredProcedure("AtcanStatmentForReceivable", parameters);
    }

    /// <summary>
    /// Get Billing History for Corresponence search
    /// </summary>
    /// <param name="corrSearchCriteria">Search criteria for correspondence search</param>
    /// <param name="billingCategoryId">Billing history</param>
    /// <returns>List of Billing history for correspondence</returns>
    public List<MiscBillingHistorySearchResult> GetBillingHistoryCorrSearchResult(CorrespondenceSearchCriteria corrSearchCriteria, int billingCategoryId)
    {
      var parameters = new ObjectParameter[12];
     
      parameters[0] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int)) { Value = corrSearchCriteria != null ? corrSearchCriteria.CorrBilledMemberId : 0 };
      parameters[1] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value = corrSearchCriteria != null ? corrSearchCriteria.CorrBillingMemberId : 0 };
      parameters[2] = new ObjectParameter("FROM_DATE_I", typeof(DateTime?)) { Value = corrSearchCriteria != null ? corrSearchCriteria.FromDate : null };
      parameters[3] = new ObjectParameter("TO_DATE_I", typeof(DateTime?)) { Value = corrSearchCriteria != null ? corrSearchCriteria.ToDate : null };
      parameters[4] = new ObjectParameter("CORRESPONDENCE_STATUS_I", typeof(int)) { Value = corrSearchCriteria != null ? corrSearchCriteria.CorrespondenceStatusId : 0 };
      parameters[5] = new ObjectParameter("CORRESPONDENCE_SUB_STATUS_I", typeof(int)) { Value = corrSearchCriteria != null ? corrSearchCriteria.CorrespondenceSubStatusId : 0 };
      parameters[6] = new ObjectParameter("CORRESPONDENCE_NO_I", typeof(Int64)) { Value = corrSearchCriteria != null ? corrSearchCriteria.CorrespondenceNumber : 0 };
      parameters[7] = new ObjectParameter("AUTHORITY_TO_BILL_I", typeof(int)) { Value = corrSearchCriteria != null ? (corrSearchCriteria.AuthorityToBill ? 1 : 0) : 0 };
      parameters[8] = new ObjectParameter("NO_OF_DAYS_TO_EXPIRE_I", typeof(int)) { Value = corrSearchCriteria != null ? corrSearchCriteria.NoOfDaysToExpiry : 0 };
      parameters[9] = new ObjectParameter("CORR_INIT_MEM_I", typeof(int?)) { Value = corrSearchCriteria != null ? corrSearchCriteria.InitiatingMember : 0 };
      parameters[10] = new ObjectParameter("CORR_OWNER_ID_I", typeof(int?)) { Value = corrSearchCriteria != null ? corrSearchCriteria.CorrespondenceOwnerId : 0 };
      parameters[11] = new ObjectParameter("BILLING_CATEGORY_ID_I", typeof(int)) { Value = billingCategoryId };

      var correspondences = ExecuteStoredFunction<MiscBillingHistorySearchResult>("GetBillingHistoryCorrSearchResult", parameters);

      return correspondences.ToList();
    }

    /// <summary>
    /// This method Gets correspondences fro either Misc or Uatp depending on billing category Id
    /// This is to be used in Corr Trail Report for Misc and Uatp
    /// </summary>
    /// <param name="corrSearchCriteria"></param>
    /// <param name="billingCategoryId"></param>
    /// <returns></returns>
    public List<CorrespondenceTrailSearchResult> GetCorrespondenceTrailSearchResult(CorrespondenceTrailSearchCriteria corrSearchCriteria, int billingCategoryId)
    {
      var parameters = new ObjectParameter[8];

      parameters[0] = new ObjectParameter("FROM_DATE_I", typeof(DateTime?)) { Value = corrSearchCriteria != null ? corrSearchCriteria.FromDate : null };
      parameters[1] = new ObjectParameter("TO_DATE_I", typeof(DateTime?)) { Value = corrSearchCriteria != null ? corrSearchCriteria.ToDate : null };
      parameters[2] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value = corrSearchCriteria != null ? corrSearchCriteria.CorrBillingMemberId : 0 };
      parameters[3] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int)) { Value = corrSearchCriteria != null ? corrSearchCriteria.CorrBilledMemberId : 0 };
      parameters[4] = new ObjectParameter("CORRESPONDENCE_STATUS_I", typeof(int)) { Value = corrSearchCriteria != null ? corrSearchCriteria.CorrespondenceStatusId : 0 };
      parameters[5] = new ObjectParameter("CORRESPONDENCE_SUB_STATUS_I", typeof(int)) { Value = corrSearchCriteria != null ? corrSearchCriteria.CorrespondenceSubStatusId : 0 };
      parameters[6] = new ObjectParameter("CORR_INIT_MEM_I", typeof(int?)) { Value = corrSearchCriteria != null ? corrSearchCriteria.InitiatingMember : 0 };
      parameters[7] = new ObjectParameter("BILLING_CATEGORY_ID_I", typeof(int)) { Value = billingCategoryId };

      var correspondences = ExecuteStoredFunction<CorrespondenceTrailSearchResult>("GetMuCorrespondenceTrailSearchResult", parameters);
      return correspondences.ToList();
    }
    /// <summary>
    /// Returns a flag indicating if line item detail is expected within an invoice, but not present.
    /// Also, returns the line item number for which line item detail is expected but not present.
    /// This check is required while validating an invoice.
    /// </summary>
    /// <param name="invoiceId">Id of the invoice to be validated.</param>
    /// <param name="isLineItemDetailExpected">Flag indicating line item detail present or not.</param>
    /// <param name="lineItemNumber">Line Item number for which expected line item detail is not present.</param>
    public void IsLineItemDetailExpected(Guid invoiceId,int billingCategoryId, out bool isLineItemDetailExpected, out int lineItemNumber)
    {
      var parameters = new ObjectParameter[4];

      parameters[0] = new ObjectParameter("INVOICE_ID_I", typeof(Guid)) { Value = invoiceId };
      //CMP #636: TFS 9323: System is allowing user to submit invoice with Nil Line Item Detail .
      parameters[1] = new ObjectParameter("BILLING_CATEGORY_ID_I", typeof(int)) { Value = billingCategoryId };
      parameters[2] = new ObjectParameter("IS_LI_DETAIL_EXPECTED_O", typeof(int));
      parameters[3] = new ObjectParameter("LINE_ITEM_NO_O", typeof(int));

      ExecuteStoredProcedure("IsMandatoryLineItemDetailNotPresent", parameters);

      isLineItemDetailExpected = Convert.ToBoolean(parameters[2].Value);
      lineItemNumber = Convert.ToInt32(parameters[3].Value);
    }

    /// <summary>
    /// Singles the specified where.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public override MiscUatpInvoice Single(Expression<Func<MiscUatpInvoice, bool>> where)
    {
      throw new NotImplementedException("Use overloaded Single instead.");
    }

    /// <summary>
    /// Singles the specified invoice id.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="billingMemberId">The billing member id</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="invoiceStatusId">The invoice status id.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingCategoryId"></param>
    /// <param name="rejectionStage"></param>
    /// <returns></returns>
    public MiscUatpInvoice Single(Guid? invoiceId = null, string invoiceNumber = null, int? billingMemberId = null, int? billedMemberId = null, int? billingPeriod = null, int? billingMonth = null, int? invoiceStatusId = null, int? billingYear = null, int? billingCategoryId = null, int? rejectionStage = null)
    {
      var entities = new string[] { LoadStrategy.MiscEntities.MiscInvoice, LoadStrategy.MiscEntities.MiscInvoiceAddOnCharge,LoadStrategy.MiscEntities.InvoiceSummary,
        LoadStrategy.Entities.MemberLocation,LoadStrategy.Entities.MemberLocationInfoAddDetail,LoadStrategy.Entities.BillingMember,LoadStrategy.Entities.BilledMember,
        LoadStrategy.Entities.ListingCurrency,LoadStrategy.MiscEntities.ChargeCategory,LoadStrategy.MiscEntities.MiscTaxBreakdown,
        LoadStrategy.MiscEntities.MiscUatpAttachment,LoadStrategy.MiscEntities.MiscUatpInvoiceAdditionalDetail,
        LoadStrategy.MiscEntities.MemberContact,LoadStrategy.MiscEntities.PaymentDetail,LoadStrategy.MiscEntities.InvoiceOwner,LoadStrategy.Entities.AttachmentUploadedbyUser,LoadStrategy.MiscEntities.OtherOrganizationInformation,LoadStrategy.MiscEntities.OtherOrganizationAdditionalDetails,LoadStrategy.MiscEntities.OtherOrganizationContactInformations
      };

      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      string invoiceIdStr = null;
      if (invoiceId.HasValue)
        invoiceIdStr = ConvertUtil.ConvertGuidToString(invoiceId.Value);

      string invoiceStatusIdstr = null;
      if (invoiceStatusId.HasValue) invoiceStatusIdstr = invoiceStatusId.Value.ToString();
      var invoices = GetMiscInvoiceLS(loadStrategy, invoiceIdStr, invoiceNumber, billingMemberId, billedMemberId, billingPeriod, billingMonth, invoiceStatusIdstr, billingYear);
      if (invoices.Count > 0)
      {
        if (invoices.Count > 1) throw new ApplicationException("Multiple records found");
        return invoices[0];
      }
      return null;
    }

    /// <summary>
    /// Gets the MU linked invoice header.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="invoiceStatusId">The invoice status id.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <param name="rejectionStage">The rejection stage.</param>
    /// <returns></returns>
    public MiscUatpInvoice GetMUlinkedInvoiceHeader(Guid? invoiceId = null, string invoiceNumber = null, int? billingMemberId = null, int? billedMemberId = null, int? billingPeriod = null, int? billingMonth = null, int? invoiceStatusId = null, int? billingYear = null, int? billingCategoryId = null, int? rejectionStage = null)
    {
      var entities = new string[] { LoadStrategy.MiscEntities.MiscInvoice,LoadStrategy.Entities.BillingMember,LoadStrategy.Entities.BilledMember,
        LoadStrategy.Entities.ListingCurrency,LoadStrategy.MiscEntities.ChargeCategory
      };

      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      string invoiceIdStr = null;
      if (invoiceId.HasValue)
        invoiceIdStr = ConvertUtil.ConvertGuidToString(invoiceId.Value);

      string invoiceStatusIdstr = null;
      if (invoiceStatusId.HasValue) invoiceStatusIdstr = invoiceStatusId.Value.ToString();
      var invoices = GetMiscInvoiceLS(loadStrategy, invoiceIdStr, invoiceNumber, billingMemberId, billedMemberId, billingPeriod, billingMonth, invoiceStatusIdstr, billingYear);
      if (invoices.Count > 0)
      {
        if (invoices.Count > 1) throw new ApplicationException("Multiple records found");
        return invoices[0];
      }
      return null;
    }

    /// <summary>
    /// Load Misc Invoice hierarchy
    /// </summary>
    /// <param name="loadStrategy"></param>
    /// <param name="invoiceId"></param>
    /// <param name="invoiceNumber"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <param name="billingPeriod"></param>
    /// <param name="billingMonth"></param>
    /// <param name="invoiceStatusIds"></param>
    /// <param name="billingYear"></param>
    /// <param name="billingCategoryId"></param>
    /// <param name="rejectionStage"></param>
    /// <param name="chargeCategoryId"></param>
    /// <param name="chargeCodeId"></param>
    /// <param name="inclusionStatus"></param>
    /// <param name="isWebGenerationDate"></param>
    /// <param name="submissionMethodId"></param>
    /// <param name="onBehalfTransmitterCode"></param>
    /// <param name="dailyDeliveryStatus"></param>
    /// <param name="targetDate"></param>
    /// <returns>List of MiscUatpInvoices</returns>
    //CMP#622 : add Output type parameter 1=Regular output 2= Location specific output 
    private List<MiscUatpInvoice> GetMiscInvoiceLS(LoadStrategy loadStrategy, string invoiceId = null, string invoiceNumber = null, int? billingMemberId = null, int? billedMemberId = null, int? billingPeriod = null, int? billingMonth = null, string invoiceStatusIds = null, int? billingYear = null, int? billingCategoryId = null, int? rejectionStage = null, int? chargeCategoryId = null, int? chargeCodeId = null, int? inclusionStatus = null, DateTime? isWebGenerationDate = null, int? submissionMethodId = null, string onBehalfTransmitterCode = null, int? dailyDeliveryStatus = null, DateTime? targetDate = null, int? outputType = null, string locationId = null)
    {
      return base.ExecuteLoadsSP(SisStoredProcedures.GetMiscInvoice,
                                loadStrategy,
                                new OracleParameter[] { new OracleParameter(MiscInvoiceRepositoryConstants.InvoiceIdParameterName, invoiceId ?? null),  
                                    new OracleParameter(MiscInvoiceRepositoryConstants.InvoiceNoParameterName, invoiceNumber ?? null) ,
                                     new OracleParameter(MiscInvoiceRepositoryConstants.BillingMemeberIdParameterName, billingMemberId ?? null) ,
                                    new OracleParameter(MiscInvoiceRepositoryConstants.BilledMemberParameterName, billedMemberId ?? null) ,
                                    new OracleParameter(MiscInvoiceRepositoryConstants.BillingPeriodParameterName, billingPeriod ?? null) ,
                                    new OracleParameter(MiscInvoiceRepositoryConstants.BillingMonthParameterName, billingMonth ?? null) ,
                                    new OracleParameter(MiscInvoiceRepositoryConstants.InvoiceStatusParameterName, invoiceStatusIds ?? null) ,
                                    new OracleParameter(MiscInvoiceRepositoryConstants.BillingYearParameterName, billingYear ?? null),
                                    new OracleParameter(MiscInvoiceRepositoryConstants.BillingCategoryIdParameterName, billingCategoryId ?? null) ,
                                    new OracleParameter(MiscInvoiceRepositoryConstants.RejectionStageParameterName, rejectionStage ?? null) ,
                                    new OracleParameter(MiscInvoiceRepositoryConstants.ChargeCategoryIdParameterName,chargeCategoryId ?? null) ,
                                    new OracleParameter(MiscInvoiceRepositoryConstants.ChargeCodeIdParameterName,chargeCodeId ?? null), 
                                    new OracleParameter(MiscInvoiceRepositoryConstants.InclusionStatusIdParameterName,inclusionStatus ?? null), 
                                    new OracleParameter(MiscInvoiceRepositoryConstants.IsWbGenDateParameterName,isWebGenerationDate.HasValue ? isWebGenerationDate.Value.ToString("dd-MM-yyyy") : null), 
                                    new OracleParameter(MiscInvoiceRepositoryConstants.SubmissionMethodParameterName,submissionMethodId ?? null), 
                                    new OracleParameter(MiscInvoiceRepositoryConstants.OnBehalfTransmitterParameterName,onBehalfTransmitterCode ?? null), 
                                    new OracleParameter(MiscInvoiceRepositoryConstants.DailyDeliveryStatusParameterName,dailyDeliveryStatus ?? null),
                                    new OracleParameter(MiscInvoiceRepositoryConstants.TargetDateParameterName,targetDate ?? null),
                                    new OracleParameter(MiscInvoiceRepositoryConstants.OutputType,outputType ?? null),
                                    new OracleParameter(MiscInvoiceRepositoryConstants.LocationId,locationId ?? null)
                                  },
                                this.FetchRecord);
    }

      /// <summary>
      /// Load Misc Is Web Invoice hierarchy
      /// </summary>
      /// <param name="loadStrategy"></param>
      /// <param name="billingMemberId"></param>
      /// <param name="invoiceStatusIds"></param>
      /// <param name="billingCategoryId"></param>
      /// <param name="isWebGenerationDate"></param>
      /// <param name="outputType"></param>
      /// <param name="locationId"></param>
      /// <returns>List of Misc Is Web Invoices</returns>
      //CMP#622: Add Output type parameter 1= Regular output, 2= Location specific output 
    private List<MiscUatpInvoice> GetMiscIsWebInvLS(LoadStrategy loadStrategy, int? billingMemberId = null, string invoiceStatusIds = null, int? billingCategoryId = null, DateTime? isWebGenerationDate = null, int? isReprocessing = null, int? outputType = null, string locationId = null)
    {
        return base.ExecuteLoadsSP(SisStoredProcedures.GetMiscIsWebInvoice,
                                  loadStrategy,
                                   new OracleParameter[] 
                                   { 
                                       new OracleParameter(MiscInvoiceRepositoryConstants.BillingMemberParameter, billingMemberId ?? null),  
                                       new OracleParameter(MiscInvoiceRepositoryConstants.InvoiceStatusIdParameter, invoiceStatusIds ?? null) ,
                                       new OracleParameter(MiscInvoiceRepositoryConstants.BillingCategoryIdParameter, billingCategoryId ?? null) ,
                                       new OracleParameter(MiscInvoiceRepositoryConstants.IsWbGenDateParameter, isWebGenerationDate.HasValue ? isWebGenerationDate.Value.ToString("dd-MM-yyyy") : null) ,
                                       new OracleParameter(MiscInvoiceRepositoryConstants.IsRegenerateParameter, isReprocessing ?? null) ,
                                       new OracleParameter(MiscInvoiceRepositoryConstants.OutputTypeParameter, outputType ?? null) ,
                                       new OracleParameter(MiscInvoiceRepositoryConstants.LocationIdParameter, locationId ?? null)
                                  },
                                  this.FetchRecord);
    }

    /// <summary>
    /// Fetches the record.
    /// </summary>
    /// <param name="loadStrategyResult">The load strategy result.</param>
    /// <returns></returns>
    private List<MiscUatpInvoice> FetchRecord(LoadStrategyResult loadStrategyResult)
    {
      var invoices = new List<MiscUatpInvoice>();
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.MiscInvoice))
      {
        invoices = MiscInvoiceRepository.LoadEntities(base.EntityBaseObjectSet, loadStrategyResult, null);
      }
      return invoices;
    }

    /// <summary>
    /// Load the given object set with entities from the Load Strategy Result.
    /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
    /// </summary>
    /// <param name="objectSet"></param>
    /// <param name="loadStrategyResult"></param>
    /// <returns></returns>
    public static List<MiscUatpInvoice> LoadEntities(ObjectSet<InvoiceBase> objectSet, LoadStrategyResult loadStrategyResult, Action<MiscUatpInvoice> link)
    {
      var invoices = new List<MiscUatpInvoice>();
      var muMaterializers = new MuMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.MiscEntities.MiscInvoice))
      {
        if (reader != null)
        {
          // first result set includes the category
          invoices = muMaterializers.MiscInvoiceMaterializer.Materialize(reader).Bind(objectSet).ToList();
          if (!reader.IsClosed)
            reader.Close();


        }
      }

      //Load MemberLocationInformation  by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.MemberLocation) && invoices.Count > 0)
      {
        //The fetched child records should use the Parent entities.
        MemberLocationInformationRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<MemberLocationInformation>(), loadStrategyResult, memberLocationInfo => memberLocationInfo.Invoice = invoices.Find(i => i.Id == memberLocationInfo.InvoiceId));
      }

     //Load LineItems by calling respective LoadEntities method
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.LineItem) && invoices.Count > 0)
      {
        //The fetched child records should use the Parent entities.
        LineItemRepository.LoadEntities(objectSet.Context.CreateObjectSet<LineItem>(), loadStrategyResult, lineItem => lineItem.Invoice = invoices.Find(i => i.Id == lineItem.InvoiceId));
      }

      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.MiscInvoiceAddOnCharge) && invoices.Count > 0)
      {
        //The fetched child records should use the Parent entities.
        InvoiceAddOnChargeRepository.LoadEntities(objectSet.Context.CreateObjectSet<InvoiceAddOnCharge>(), loadStrategyResult, null);
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

      //Load ChargeCategories by calling respective LoadEntities method  
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.ChargeCategory) && invoices.Count != 0)
      {
        ChargeCategoryRepository.LoadEntities(objectSet.Context.CreateObjectSet<ChargeCategory>(), loadStrategyResult, null);
      }

      //Load InvoiceSummary by calling respective LoadEntities method  
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.InvoiceSummary) && invoices.Count != 0)
      {
        InvoiceSummaryRepository.LoadEntities(objectSet.Context.CreateObjectSet<InvoiceSummary>(), loadStrategyResult, invoiceSummary => invoiceSummary.MiscUatpInvoice = invoices.Find(i => i.Id == invoiceSummary.InvoiceId));
      }

      //Load MiscTaxBreakdown 
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.MiscTaxBreakdown) && invoices.Count != 0)
      {
        MiscUatpInvoiceTaxRepository.LoadEntities(objectSet.Context.CreateObjectSet<MiscUatpInvoiceTax>(), loadStrategyResult, null);
      }

      //Load MiscUatpAttachment 
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.MiscUatpAttachment) && invoices.Count != 0)
      {
        MiscUatpInvoiceAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<MiscUatpAttachment>(), loadStrategyResult, null);
      }

      //Load MiscUatpInvoiceAdditionalDetail 
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.MiscUatpInvoiceAdditionalDetail) && invoices.Count != 0)
      {
        MiscUatpInvoiceAdditionalDetailRepository.LoadEntities(objectSet.Context.CreateObjectSet<MiscUatpInvoiceAdditionalDetail>(), loadStrategyResult, null);
      }

      //Load MemberContact 
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.MemberContact) && invoices.Count != 0)
      {
        ContactInformationRepository.LoadEntities(objectSet.Context.CreateObjectSet<ContactInformation>(), loadStrategyResult, null);
      }

      //Load PaymentDetail 
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.PaymentDetail) && invoices.Count != 0)
      {
        PaymentDetailRepository.LoadEntities(objectSet.Context.CreateObjectSet<PaymentDetail>(), loadStrategyResult, paymentDetail => paymentDetail.MiscUatpInvoice = invoices.Find(i => i.Id == paymentDetail.InvoiceId));
      }

      //Load InvoiceOwner
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.InvoiceOwner) && invoices.Count != 0)
      {
        UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>(), loadStrategyResult, null, LoadStrategy.MiscEntities.InvoiceOwner);
      }

      //Load Misc correspondences
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.Correspondence) && invoices.Count != 0)
      {
        MiscCorrespondenceRepository.LoadEntities(objectSet.Context.CreateObjectSet<MiscCorrespondence>(), loadStrategyResult, miscCorrespondence => miscCorrespondence.Invoice = invoices.Find(i => i.Id == miscCorrespondence.InvoiceId));
      }

      //Load OtherOrganizationInformations
      if (loadStrategyResult.IsLoaded(LoadStrategy.MiscEntities.OtherOrganizationInformation) && invoices.Count != 0)
      {
        OtherOrganizationInformationRepository.LoadEntities(objectSet.Context.CreateObjectSet<OtherOrganizationInformation>(), loadStrategyResult, i => i.Invoice = invoices.Find(j => j.Id == i.InvoiceId));
      }
      return invoices;
    }

    /// <summary>
    /// LoadStrategy overload of GetSingleInvoiceTrail
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public MiscUatpInvoice GetSingleInvoiceTrail(Guid invoiceId)
    {
      var entities = new string[] { LoadStrategy.MiscEntities.MiscInvoice, LoadStrategy.MiscEntities.MiscInvoiceAddOnCharge,
        LoadStrategy.Entities.MemberLocation,LoadStrategy.Entities.BillingMember,LoadStrategy.Entities.BilledMember,
        LoadStrategy.Entities.ListingCurrency,LoadStrategy.MiscEntities.ChargeCategory,LoadStrategy.MiscEntities.MiscTaxBreakdown,
        LoadStrategy.MiscEntities.MiscUatpAttachment,LoadStrategy.MiscEntities.MiscUatpInvoiceAdditionalDetail,
        LoadStrategy.MiscEntities.MemberContact,LoadStrategy.MiscEntities.PaymentDetail,LoadStrategy.MiscEntities.InvoiceSummary,
        LoadStrategy.MiscEntities.LineItem,LoadStrategy.MiscEntities.LineItemChargeCode,LoadStrategy.MiscEntities.LineItemUomCode,
        LoadStrategy.Entities.Correspondence,LoadStrategy.Entities.CorrespondencesFromMember,LoadStrategy.Entities.CorrespondencesToMember,
        LoadStrategy.Entities.CorrespondenceAttachment,LoadStrategy.Entities.CorrespondenceCurrency
      };

      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      string invoiceIdStr = null;
      invoiceIdStr = ConvertUtil.ConvertGuidToString(invoiceId);
      var invoices = GetMiscInvoiceLS(loadStrategy, invoiceIdStr);
      if (invoices.Count > 0)
      {
        if (invoices.Count > 1) throw new ApplicationException("Multiple records found");
        return invoices[0];
      }
      return null;
    }

    ////SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
    ///// <summary>
    ///// Updates expiry period of current transaction and transactions prior to it.
    ///// </summary>
    ///// <param name="transactionId"></param>
    ///// <param name="transactionTypeId"></param>
    ///// <param name="expiryPeriod"></param>
    //public void UpdateExpiryDatePeriod(Guid transactionId, int transactionTypeId, DateTime expiryPeriod)
    //{
    //  var parameters = new ObjectParameter[3];

    //  parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.TransactionIdParameterName, typeof(Guid)) { Value = transactionId };
    //  parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.TransactionTypeParameter, typeof(int)) { Value = transactionTypeId };
    //  parameters[2] = new ObjectParameter(MiscInvoiceRepositoryConstants.ExpiryPeriodParameterName, typeof(DateTime)) { Value = expiryPeriod };

    //  ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.UpdateExpiryDatePeriodFunctionName, parameters);
    //}

    /// <summary>
        /// Gets the invoice for listing report.
        /// </summary>
        /// <param name="invoiceId">String representation of the invoice Guid.</param>
        /// <returns>MiscUatpInvoice</returns>
        public MiscUatpInvoice GetInvoiceToGenerateOfflineCollectionWithISWebAttachments(string invoiceId = null, string invoiceNumber = null)
        {
          var entities = new[]
                           {
                             LoadStrategy.MiscEntities.MiscInvoice, 
                             LoadStrategy.Entities.BillingMember, 
                             // LoadStrategy.MiscEntities.MiscUatpInvoiceAdditionalDetail, 
                             LoadStrategy.Entities.BilledMember,
                             // LoadStrategy.Entities.ListingCurrency, 
                             // LoadStrategy.MiscEntities.ChargeCategory, 
                             // LoadStrategy.MiscEntities.LineItem, 
                             // LoadStrategy.MiscEntities.LineItemAddOnCharges,
                             // LoadStrategy.MiscEntities.LineItemChargeCode, 
                             // LoadStrategy.MiscEntities.LineItemUomCode, 
                             // LoadStrategy.MiscEntities.LineItemAdditionalDetails,
                             // LoadStrategy.MiscEntities.LineItemDetails, 
                             LoadStrategy.MiscEntities.MiscUatpAttachmentISWeb, 
                             // LoadStrategy.MiscEntities.OtherOrganizationContactInformations,
                             // LoadStrategy.MiscEntities.OtherOrganizationAdditionalDetails, 
                             // LoadStrategy.MiscEntities.MemberContact, 
                             // LoadStrategy.Entities.MemberLocation,
                             // LoadStrategy.MiscEntities.PaymentDetail, 
                             // LoadStrategy.MiscEntities.LineItemTaxBreakdown, 
                             // LoadStrategy.MiscEntities.LineItemTaxCountry,
                             // LoadStrategy.MiscEntities.LineItemTaxAdditionalDetails, 
                             // LoadStrategy.MiscEntities.LineItemChargeCodeType, 
                             // LoadStrategy.MiscEntities.LineItemDetailTaxBreakdown,
                             // LoadStrategy.MiscEntities.LineItemDetailAdditionalDet, 
                             // LoadStrategy.MiscEntities.LineItemDetailTaxCountry, 
                             // LoadStrategy.MiscEntities.LineItemDetailTaxAdditionalDetails,
                             // LoadStrategy.MiscEntities.LineItemDetailAddOnCharges, 
                             // LoadStrategy.MiscEntities.LineItemDetailsFieldValues, 
                             // LoadStrategy.MiscEntities.LIDetFieldValuesFieldMetaData
                           };

          var loadStrategy = new LoadStrategy(string.Join(",", entities));
          var invoices = GetMiscInvoiceLS(loadStrategy, invoiceId, invoiceNumber);
          if (invoices.Count > 0)
          {
            if (invoices.Count > 1) throw new ApplicationException("Multiple records found");
            return invoices[0];
          }
          return null;
        }

        /// <summary>
    /// Singles the specified where.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    public MiscUatpInvoice GetSingleInvoiceTrail(Expression<Func<MiscUatpInvoice, bool>> where)
    {
      throw new NotImplementedException("Use GetSingleInvoiceTrail instead.");
    }

   public override IQueryable<MiscUatpInvoice> GetAll()
    {
      var invoice = EntityObjectQuery
        .Include("BilledMember")
        .Include("MemberLocationInformation")
        .Include("ChargeCategory")
        .Include("InvoiceSummary")
        .Include("ListingCurrency")
        .Include("IsInputFile")
        .Include("InvoiceOwner");

      return invoice;
    }

    public IQueryable<MiscUatpInvoice> GetAllForPayableSearch()
    {
      var invoice = EntityObjectQuery
        .Include("BillingMember")
        .Include("ChargeCategory")
        .Include("InvoiceSummary")
        .Include("ListingCurrency");

      return invoice;
    }

    /// <summary>
    /// To get matching Misc invoices.
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    public System.Collections.Generic.List<MiscUatpInvoice> GetMiscInvoices(Expression<Func<MiscUatpInvoice, bool>> where)
    {
      throw new NotImplementedException("Use GetMiscUatpInvoices method instead.");
    }

    /// <summary>
    /// This is a loadstrategy method overload of GetMiscInvoices 
    /// </summary>
    /// <param name="billedMemberId">billedMemberId</param>
    /// <param name="billingMemberId"></param>
    /// <param name="billingPeriod">billingPeriod</param>
    /// <param name="billingMonth">billingMonth</param>
    /// <param name="invoiceStatusIds">invoiceStatusId</param>
    /// <param name="billingYear">billingYear</param>
    /// <param name="billingCategoryId">billingCategoryId</param>
    /// <param name="chargeCategoryId"></param>
    /// <param name="chargeCodeId"></param>
    /// <param name="inclusionStatus"></param>
    /// <param name="isWebGenerationDate"></param>
    /// <param name="onBehalfTransmitterCode"></param>
    /// <returns>list of MiscUatpInvoice</returns>
    //CMP#622 : Add output type parameter
    public List<MiscUatpInvoice> GetMiscUatpInvoices(int? billedMemberId = null, int? billingMemberId = null, int? billingPeriod = null, int? billingMonth = null, string invoiceStatusIds = null, int? billingYear = null, int? billingCategoryId = null, int? chargeCategoryId = null, int? chargeCodeId = null, string invoiceId = null, int? inclusionStatus = null, DateTime? isWebGenerationDate = null, int? submissionMethodId = null, string onBehalfTransmitterCode = null, int? dailyDeliveryStatus = null, DateTime? targetDate = null, int? outputType = null, string locationId = null)
    {
      var entities = new string[] {LoadStrategy.MiscEntities.MiscInvoice,LoadStrategy.Entities.MemberLocation,LoadStrategy.Entities.MemberLocationInfoAddDetail,LoadStrategy.Entities.ListingCurrency,LoadStrategy.MiscEntities.InvoiceSummary,LoadStrategy.Entities.BillingMember,
        LoadStrategy.Entities.BilledMember,LoadStrategy.MiscEntities.PaymentDetail,LoadStrategy.MiscEntities.ChargeCategory,LoadStrategy.MiscEntities.ChargeCategoryChargeCode,LoadStrategy.MiscEntities.ChargeCategoryChargeCodeType,
        LoadStrategy.MiscEntities.MiscInvoiceAddOnCharge,LoadStrategy.MiscEntities.MiscTaxBreakdown,LoadStrategy.MiscEntities.MiscInvoiceTaxAdditionalDetail,LoadStrategy.MiscEntities.MemberContact,
        LoadStrategy.MiscEntities.LineItem,LoadStrategy.MiscEntities.LineItemChargeCode,LoadStrategy.MiscEntities.MiscUatpInvoiceAdditionalDetail,LoadStrategy.MiscEntities.OtherOrganizationInformation,
        LoadStrategy.MiscEntities.OtherOrganizationAdditionalDetails,LoadStrategy.MiscEntities.LineItemChargeCodeType,LoadStrategy.MiscEntities.MiscTaxBreakdownCountry,
        LoadStrategy.MiscEntities.OtherOrganizationContactInformations,LoadStrategy.MiscEntities.LineItemUomCode,LoadStrategy.MiscEntities.LineItemAdditionalDetails,LoadStrategy.MiscEntities.LineItemDetails,
        LoadStrategy.MiscEntities.LineItemDetailUomCode,LoadStrategy.MiscEntities.LineItemAddOnCharges,LoadStrategy.MiscEntities.LineItemTaxBreakdown,LoadStrategy.MiscEntities.LineItemTaxAdditionalDetails,
        LoadStrategy.MiscEntities.LineItemTaxCountry,LoadStrategy.MiscEntities.LineItemDetailAdditionalDet,LoadStrategy.MiscEntities.LineItemDetailsFieldValues,LoadStrategy.MiscEntities.LIDetFieldValuesFieldMetaData,
        LoadStrategy.MiscEntities.LineItemDetailTaxBreakdown,LoadStrategy.MiscEntities.LineItemDetailTaxAdditionalDetails,LoadStrategy.MiscEntities.LineItemDetailTaxCountry,LoadStrategy.MiscEntities.LIDFMDataSource,
        LoadStrategy.MiscEntities.LineItemDetailAddOnCharges, LoadStrategy.MiscEntities.LineItemDetailFieldValueAttrValue, LoadStrategy.MiscEntities.LineItemDetailFieldValueParentValue
      };

      var loadStrategy = new LoadStrategy(string.Join(",", entities));

      return (GetMiscInvoiceLS(loadStrategy, billedMemberId: billedMemberId, billingMemberId: billingMemberId, billingPeriod: billingPeriod, billingMonth: billingMonth, invoiceStatusIds: invoiceStatusIds,
      billingYear: billingYear, billingCategoryId: billingCategoryId, chargeCategoryId: chargeCategoryId, chargeCodeId: chargeCodeId, invoiceId: invoiceId, inclusionStatus: inclusionStatus, isWebGenerationDate: isWebGenerationDate, submissionMethodId: submissionMethodId, onBehalfTransmitterCode: onBehalfTransmitterCode,dailyDeliveryStatus: dailyDeliveryStatus, targetDate: targetDate,outputType:outputType, locationId:locationId));
    }

      /// <summary>
      /// This is a loadstrategy method overload of GetMiscInvoices 
      /// </summary>
      /// <param name="billingMemberId"></param>
      /// <param name="invoiceStatusIds">invoiceStatusId</param>
      /// <param name="billingCategoryId">billingCategoryId</param>
      /// <param name="isWebGenerationDate">isWebGenerationDate</param>
      /// <param name="isReprocessing">isReprocessing</param>
      /// <param name="outputType">outputType</param>
      /// <param name="locationId">locationId</param>
      /// <returns>list of Misc Is Web Invoice</returns>
      //CMP#622 : Add output type parameter
      public List<MiscUatpInvoice> GetMiscIsWebInvoices(int? billingMemberId = null, string invoiceStatusIds = null, int? billingCategoryId = null, DateTime? isWebGenerationDate = null, int? isReprocessing = null, int? outputType = null, string locationId = null)
      {
        var entities = new string[] {LoadStrategy.MiscEntities.MiscInvoice,LoadStrategy.Entities.MemberLocation,LoadStrategy.Entities.MemberLocationInfoAddDetail,LoadStrategy.Entities.ListingCurrency,LoadStrategy.MiscEntities.InvoiceSummary,LoadStrategy.Entities.BillingMember,
        LoadStrategy.Entities.BilledMember,LoadStrategy.MiscEntities.PaymentDetail,LoadStrategy.MiscEntities.ChargeCategory,LoadStrategy.MiscEntities.ChargeCategoryChargeCode,LoadStrategy.MiscEntities.ChargeCategoryChargeCodeType,
        LoadStrategy.MiscEntities.MiscInvoiceAddOnCharge,LoadStrategy.MiscEntities.MiscTaxBreakdown,LoadStrategy.MiscEntities.MiscInvoiceTaxAdditionalDetail,LoadStrategy.MiscEntities.MemberContact,
        LoadStrategy.MiscEntities.LineItem,LoadStrategy.MiscEntities.LineItemChargeCode,LoadStrategy.MiscEntities.MiscUatpInvoiceAdditionalDetail,LoadStrategy.MiscEntities.OtherOrganizationInformation,
        LoadStrategy.MiscEntities.OtherOrganizationAdditionalDetails,LoadStrategy.MiscEntities.LineItemChargeCodeType,LoadStrategy.MiscEntities.MiscTaxBreakdownCountry,
        LoadStrategy.MiscEntities.OtherOrganizationContactInformations,LoadStrategy.MiscEntities.LineItemUomCode,LoadStrategy.MiscEntities.LineItemAdditionalDetails,LoadStrategy.MiscEntities.LineItemDetails,
        LoadStrategy.MiscEntities.LineItemDetailUomCode,LoadStrategy.MiscEntities.LineItemAddOnCharges,LoadStrategy.MiscEntities.LineItemTaxBreakdown,LoadStrategy.MiscEntities.LineItemTaxAdditionalDetails,
        LoadStrategy.MiscEntities.LineItemTaxCountry,LoadStrategy.MiscEntities.LineItemDetailAdditionalDet,LoadStrategy.MiscEntities.LineItemDetailsFieldValues,LoadStrategy.MiscEntities.LIDetFieldValuesFieldMetaData,
        LoadStrategy.MiscEntities.LineItemDetailTaxBreakdown,LoadStrategy.MiscEntities.LineItemDetailTaxAdditionalDetails,LoadStrategy.MiscEntities.LineItemDetailTaxCountry,LoadStrategy.MiscEntities.LIDFMDataSource,
        LoadStrategy.MiscEntities.LineItemDetailAddOnCharges, LoadStrategy.MiscEntities.LineItemDetailFieldValueAttrValue, LoadStrategy.MiscEntities.LineItemDetailFieldValueParentValue
      };

        var loadStrategy = new LoadStrategy(string.Join(",", entities));

        return (GetMiscIsWebInvLS(loadStrategy, billingMemberId, invoiceStatusIds,billingCategoryId, isWebGenerationDate, isReprocessing, outputType, locationId));
    }
    
    //public List<MiscUatpInvoice> GetMiscUatpInvoices(string invoiceId)
    //{
    //  var entities = new string[] {LoadStrategy.MiscEntities.MiscInvoice,LoadStrategy.Entities.MemberLocation,LoadStrategy.Entities.ListingCurrency,LoadStrategy.MiscEntities.InvoiceSummary,LoadStrategy.Entities.BillingMember,
    //    LoadStrategy.Entities.BilledMember,LoadStrategy.MiscEntities.PaymentDetail,LoadStrategy.MiscEntities.ChargeCategory,LoadStrategy.MiscEntities.ChargeCategoryChargeCode,LoadStrategy.MiscEntities.ChargeCategoryChargeCodeType,
    //    LoadStrategy.MiscEntities.MiscInvoiceAddOnCharge,LoadStrategy.MiscEntities.MiscTaxBreakdown,LoadStrategy.MiscEntities.MiscInvoiceTaxAdditionalDetail,LoadStrategy.MiscEntities.MemberContact,
    //    LoadStrategy.MiscEntities.LineItem,LoadStrategy.MiscEntities.LineItemChargeCode,LoadStrategy.MiscEntities.MiscUatpInvoiceAdditionalDetail,LoadStrategy.MiscEntities.OtherOrganizationInformation,
    //    LoadStrategy.MiscEntities.OtherOrganizationAdditionalDetails,LoadStrategy.MiscEntities.LineItemChargeCodeType,LoadStrategy.MiscEntities.MiscTaxBreakdownCountry,
    //    LoadStrategy.MiscEntities.OtherOrganizationContactInformations,LoadStrategy.MiscEntities.LineItemUomCode,LoadStrategy.MiscEntities.LineItemAdditionalDetails,LoadStrategy.MiscEntities.LineItemDetails,
    //    LoadStrategy.MiscEntities.LineItemDetailUomCode,LoadStrategy.MiscEntities.LineItemAddOnCharges,LoadStrategy.MiscEntities.LineItemTaxBreakdown,LoadStrategy.MiscEntities.LineItemTaxAdditionalDetails,
    //    LoadStrategy.MiscEntities.LineItemTaxCountry,LoadStrategy.MiscEntities.LineItemDetailAdditionalDet,LoadStrategy.MiscEntities.LineItemDetailsFieldValues,LoadStrategy.MiscEntities.LIDetFieldValuesFieldMetaData,
    //    LoadStrategy.MiscEntities.LineItemDetailTaxBreakdown,LoadStrategy.MiscEntities.LineItemDetailTaxAdditionalDetails,LoadStrategy.MiscEntities.LineItemDetailTaxCountry,
    //    LoadStrategy.MiscEntities.LineItemDetailAddOnCharges, LoadStrategy.MiscEntities.LineItemDetailFieldValueAttrValue, LoadStrategy.MiscEntities.LineItemDetailFieldValueParentValue
    //  };

    //  var loadStrategy = new LoadStrategy(string.Join(",", entities));

    //  return (GetMiscInvoiceLS(loadStrategy,invoiceId:invoiceId));
    //}


    public void UpdateInvoiceTotal(Guid invoiceId, Guid lineItemId, int rollupValue)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid))
      {
        Value = invoiceId
      };
      parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.LineItemIdParameterName, typeof(Guid))
      {
        Value = lineItemId
      };
      parameters[2] = new ObjectParameter(MiscInvoiceRepositoryConstants.RollupValueParameterName, typeof(int))
      {
        Value = rollupValue
      };

      ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.UpdateInvoiceTotalFunctionName, parameters);
    }

    /// <summary>
    /// This function is used to update invoice total based on invoice header data(Tax, Vat, AddOn).
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="totalTaxAmount"></param>
    /// <param name="totalVatAmount"></param>
    /// <param name="totalAddOnAmount"></param>
    //SCP324672: Wrong amount invoice
    public void UpdateMUInvoiceSummary(Guid invoiceId, decimal? totalTaxAmount, decimal? totalVatAmount, decimal? totalAddOnAmount)
    {
      var parameters = new ObjectParameter[4];
      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid))
      {
        Value = invoiceId
      };
      parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.TotalTaxAmountParameterName, typeof(decimal?))
      {
        Value = totalTaxAmount
      };
      parameters[2] = new ObjectParameter(MiscInvoiceRepositoryConstants.TotalVatAmountParameterName, typeof(decimal?))
      {
        Value = totalVatAmount
      };

      parameters[3] = new ObjectParameter(MiscInvoiceRepositoryConstants.TotalAddOnAmountParameterName, typeof(decimal?))
      {
        Value = totalAddOnAmount
      };

      ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.UpdateMUInvoiceSummaryFunctionName, parameters);
    }

    //CMP#502: [3.6] IS-WEB: Save of Invoice Header of Rejection Invoices
    public void UpdateBHInvoice(Guid new_Invoice_Id, Guid old_Invoice_Id, string lineItemIds,string rejReasonCode)
    {
      var parameters = new ObjectParameter[4];
      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.NewInvoiceIdParameterName, typeof(Guid))
      {
        Value = new_Invoice_Id
      };
      parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.OldInvoiceIdParameterName, typeof(Guid))
      {
        Value = old_Invoice_Id
      };
      parameters[2] = new ObjectParameter(MiscInvoiceRepositoryConstants.LineItemNoParameterName, typeof(string))
      {
        Value = lineItemIds
      };
      //CMP#502: [3.6] IS-WEB: Save of Invoice Header of Rejection Invoices
      parameters[3] = new ObjectParameter("REJ_REASON_CODE_I", typeof(string))
      {
          Value = rejReasonCode
      };

      ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.CopyToRejectionInvoice, parameters);
    }

    public void UpdateLineItemNumber(Guid invoiceId, Guid lineItemId, int serialNumber, bool isLineItemNumber)
    {
      var parameters = new ObjectParameter[4];
      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid))
      {
        Value = invoiceId
      };
      parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.LineItemIdParameterName, typeof(Guid))
      {
        Value = lineItemId
      };
      parameters[2] = new ObjectParameter(MiscInvoiceRepositoryConstants.IsLineItemNumberParameterName, typeof(int))
      {
        Value = isLineItemNumber ? 1 : 0
      };
      parameters[3] = new ObjectParameter(MiscInvoiceRepositoryConstants.SerialNumberParameterName, typeof(int))
      {
        Value = serialNumber
      };

      ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.UpdateLineItemNumberFunctionName, parameters);
    }

    public override IQueryable<MiscUatpInvoice> Get(Expression<Func<MiscUatpInvoice, bool>> where)
    {
      return EntityObjectQuery.Include("BillingMember").Where(where);
    }


    public IQueryable<MiscAuditTrail> GetBillingHistoryAuditTrail(string invoiceId)
    {
      var parameters = new ObjectParameter[1];
      Guid invoiceGuid = invoiceId.ToGuid();

      parameters[0] = new ObjectParameter("INVOICE_ID_I", typeof(Guid)) { Value = invoiceGuid };

      var miscAuditTrails = ExecuteStoredFunction<MiscAuditTrail>("GetMiscInvoiceBillingHistory", parameters);

      return miscAuditTrails.ToList().AsQueryable(); ;
    }

    public IList<NavigationDetails> GetLineItemDetailNavigation(Guid currentLineItemNumber, Guid lineItemId, int isOnCreate)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.CurrentLineItemDetailParameterName, typeof(Guid)) { Value = currentLineItemNumber };
      parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.LineItemIdParameterName, typeof(Guid)) { Value = lineItemId };
      parameters[2] = new ObjectParameter(MiscInvoiceRepositoryConstants.IsOnCreateParameterName, typeof(int)) { Value = isOnCreate };

      return ExecuteStoredFunction<NavigationDetails>(MiscInvoiceRepositoryConstants.LineItemDetailNavigationFunctionName, parameters).ToList();

    }

    /// <summary>
    /// Gets the derived vat details for an Invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>List of derived vat details for the Invoice.</returns>
    public IList<MiscDerivedVatDetails> GetDerivedVatDetails(Guid invoiceId)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.DerivedVatInvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };

      return ExecuteStoredFunction<MiscDerivedVatDetails>(MiscInvoiceRepositoryConstants.GetMiscUatpDerivedVatFunctionName, parameters).ToList();
    }

    /// <summary>
    /// Updates the Misc/UATP file invoice status.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="isBadFileExists"></param>
    /// <param name="processId"></param>
    /// <param name="laFlag"></param>
    /// <returns></returns>
    public void UpdateInvoiceAndFileStatus(string fileName, int billingMemberId, bool isBadFileExists, string processId, bool laFlag)
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
    /// Get Invioce Header Information
    /// //SCP363971 - AT - ISXML file for 25th April - Mistake in SIS Validation R1/R2 report received.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="includeBillingBilled"></param>
    /// <returns></returns>
    public MiscUatpInvoice GetInvoiceHeader(Guid invoiceId, bool includeBillingBilled = false)
    {
      var entities = new string[] { LoadStrategy.MiscEntities.MiscInvoice };
      if (includeBillingBilled)
      {
        entities = new string[] { LoadStrategy.MiscEntities.MiscInvoice, LoadStrategy.Entities.BillingMember, LoadStrategy.Entities.BilledMember };
      }

      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      string invoiceIdStr = ConvertUtil.ConvertGuidToString(invoiceId);

      var invoices = GetMiscInvoiceLS(loadStrategy, invoiceIdStr);
      if (invoices.Count > 0)
      {
        if (invoices.Count > 1) throw new ApplicationException("Multiple records found");
        return invoices[0];
      }
      return null;
    }


    /// <summary>
    /// Load strategy overload of GetInvoiceHeaderInformation
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <returns>MiscUatpInvoice</returns>
    public MiscUatpInvoice GetLsInvoiceHeaderInformation(Guid invoiceId)
    {
      var entities = new string[] { LoadStrategy.MiscEntities.MiscInvoice,LoadStrategy.Entities.BilledMember,LoadStrategy.MiscEntities.ChargeCategory,
        LoadStrategy.MiscEntities.InvoiceSummary,LoadStrategy.Entities.ListingCurrency, LoadStrategy.MiscEntities.MiscUatpAttachment
      };

      var loadStrategy = new LoadStrategy(string.Join(",", entities));
      string invoiceIdStr = ConvertUtil.ConvertGuidToString(invoiceId);

      var invoices = GetMiscInvoiceLS(loadStrategy, invoiceIdStr);
      if (invoices.Count > 0)
      {
        if (invoices.Count > 1) throw new ApplicationException("Multiple records found");
        return invoices[0];
      }
      return null;
    }

    public int ValidateMiscUatpInvoiceLocation(Guid invoiceId)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };
      parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.LocationValidationResultParameterName, typeof(int));


      ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.ValidateLocationFunctionName, parameters);

      return int.Parse(parameters[1].Value.ToString());
    }

    public void DeleteLineItem(Guid lineItemId)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.LineItemIdParameterName, typeof(Guid)) { Value = lineItemId };
      ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.DeleteLineItemFunctionName, parameters);
    }

    public void DeleteLineItemDetail(Guid lineItemDetailId)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.LineItemDetailIdParameterName, typeof(Guid)) { Value = lineItemDetailId };
      ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.DeleteLineItemDetailFunctionName, parameters);
    }

    public string GetExceptionCodeList(string filter, int billingCategoryTypeId)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.FilterParameterName, typeof(string)) { Value = filter };
      parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.BillingCategoryIdParameterName, typeof(int)) { Value = billingCategoryTypeId };
      parameters[2] = new ObjectParameter(MiscInvoiceRepositoryConstants.ExceptionStringOutputParameterName, typeof(string));
      ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.GetExceptionCodeFunctionName, parameters);

      return parameters[2].Value.ToString();
    }

     /// <summary>
    /// Updates the multiple invoices inclusion status and Generation date.
    /// </summary>
    /// <param name="invoiceIds">The invoice ids.</param>
    /// <param name="inclusionStatusId"></param>
    /// <param name="isUpdateGenerationDate"></param>
    public void UpdateInclusionStatus(string invoiceIds, int inclusionStatusId, bool isUpdateGenerationDate)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.InvoiceIdsParameterName, typeof(string))
      {
        Value = invoiceIds
      };
      parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.InclusionStatusParameterName, typeof(int))
      {
        Value = (int)inclusionStatusId
      };
      parameters[2] = new ObjectParameter(MiscInvoiceRepositoryConstants.IsUpdateGenerationDateParameterName, typeof(int))
      {
        Value = isUpdateGenerationDate ? 1 : 0
      };
      ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.UpdateInclusionStatusFunctionName, parameters);


            ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.DeleteLineItemFunctionName, parameters);
        }
    
    ///// <summary>
    ///// Updates the file log and invoice status depending on Validation Exception details.
    ///// </summary>
    ///// <param name="invoiceId"></param> 
    //public void UpdateInvoiceAndSetLaParameters(Guid invoiceId)
    //{
    //    var parameters = new ObjectParameter[1];
    //    parameters[0] = new ObjectParameter(InvoiceRepositoryConstants.InvoiceIdParameterName, invoiceId);
    //    ExecuteStoredProcedure(InvoiceRepositoryConstants.UpdateInvoiceSetLaParametersFunctionName, parameters);

    //    ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.UpdateInvoiceSetLaParametersFunctionName, parameters);
          
    //}


    /// <summary>
    /// 
    /// </summary>
    /// <param name="transactionId"></param>
    /// <param name="transactionType"></param>
    /// <param name="totalTaxAmount"></param>
    /// <param name="sumTaxBrDown"></param>
    /// <param name="totalVatAmount"></param>
    /// <param name="sumVatBrDown"></param>
    /// <param name="totalAddOnCharge"></param>
    /// <param name="sumAddonChargeBrDown"></param>
    /// <returns></returns>
    public string ValidateMiscInvoiceBreakDownCaptured(Guid transactionId, int transactionType, decimal totalTaxAmount, decimal sumTaxBrDown, decimal totalVatAmount,
                                                        decimal sumVatBrDown, decimal totalAddOnCharge, decimal sumAddonChargeBrDown)
    {

      var parameters = new ObjectParameter[9];
      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.TransactionId, typeof(Guid)) { Value = transactionId };
      parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.TransactionType, typeof(int)) { Value = transactionType };
      parameters[2] = new ObjectParameter(MiscInvoiceRepositoryConstants.TotalTaxAmount, typeof(double)) { Value = totalTaxAmount };
      parameters[3] = new ObjectParameter(MiscInvoiceRepositoryConstants.SumTaxBreakdown, typeof(double)) { Value = sumTaxBrDown };
      parameters[4] = new ObjectParameter(MiscInvoiceRepositoryConstants.TotalVatAmount, typeof(double)) { Value = totalVatAmount };
      parameters[5] = new ObjectParameter(MiscInvoiceRepositoryConstants.SumVatBreakdown, typeof(double)) { Value = sumVatBrDown };
      parameters[6] = new ObjectParameter(MiscInvoiceRepositoryConstants.TotalAddonAmount, typeof(double)) { Value = totalAddOnCharge };
      parameters[7] = new ObjectParameter(MiscInvoiceRepositoryConstants.SumAddonBreakdown, typeof(double)) { Value = sumAddonChargeBrDown };
      parameters[8] = new ObjectParameter(MiscInvoiceRepositoryConstants.ReturnResult, typeof(string));
      ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.ProcValidateMiscBreakdown, parameters);

      return parameters[8].Value.ToString();

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public string ValidateMiscInvoiceTotalAndBreakdownAmount(Guid invoiceId)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.InvoiceId, typeof(Guid)) { Value = invoiceId };
      parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.ReturnParam, typeof(string));
      ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.ProcValidateMiscTotalAmount, parameters);

      return parameters[1].Value.ToString();
      
    }

      /// <summary>
      /// SCP85039
      /// </summary>
      /// <param name="searchCriteria"></param>
      /// <param name="pageSize"></param>
      /// <param name="pageNo"></param>
      /// <param name="sortColumn"></param>
      /// <param name="sortOrder"></param>
      /// <returns></returns>
    public List<MiscInvoiceSearchDetails> GetMiscManageInvoices(MiscSearchCriteria searchCriteria, int pageSize, int pageNo, string sortColumn, string sortOrder)
    {
        var parameters = new ObjectParameter[19];
        parameters[0] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int)) { Value = searchCriteria.BillingMemberId };
        parameters[1] = new ObjectParameter("BILLING_MONTH_I", typeof(int)) { Value = searchCriteria.BillingMonth };
        parameters[2] = new ObjectParameter("BILLING_YEAR_I", typeof(int)) { Value = searchCriteria.BillingYear };

        parameters[3] = new ObjectParameter("INVOICE_STAUS_ID_I", typeof(int)) { Value = searchCriteria.InvoiceStatusId };
        parameters[4] = new ObjectParameter("BILLING_PERIOD_I", typeof(int)) { Value = searchCriteria.BillingPeriod };

        parameters[5] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int)) { Value = searchCriteria.BilledMemberId };
        parameters[6] = new ObjectParameter("SETTELMENT_METHOD_ID_I", typeof(int)) { Value = searchCriteria.SettlementMethodId };
        parameters[7] = new ObjectParameter("SUBMISSION_METHOD_ID_I", typeof(int)) { Value = searchCriteria.SubmissionMethodId };
        parameters[8] = new ObjectParameter("OWNER_ID_I", typeof(int)) { Value = searchCriteria.OwnerId };
        parameters[9] = new ObjectParameter("PAGE_SIZE_I", typeof(int)) { Value = pageSize };
        parameters[10] = new ObjectParameter("FILENAME_I", typeof(string)) { Value = searchCriteria.FileName };
        parameters[11] = new ObjectParameter("INVOICE_NO_I", typeof(string)) { Value = searchCriteria.InvoiceNumber };

        parameters[12] = new ObjectParameter("PAGE_NO_I", typeof(int)) { Value = pageNo };

        parameters[13] = new ObjectParameter("SORT_COLUMN_I", typeof(string)) { Value = sortColumn };
        parameters[14] = new ObjectParameter("SORT_ORDER_I", typeof(string)) { Value = sortOrder };

        parameters[15] = new ObjectParameter("BILLING_CATEGORY_ID_I", typeof(int)) { Value = searchCriteria.BillingCategoryId };
        parameters[16] = new ObjectParameter("INVOICE_TYPE_ID_I", typeof(int)) { Value = searchCriteria.InvoiceTypeId };
        parameters[17] = new ObjectParameter("CHARGE_CATEGORY_ID_I", typeof(int)) { Value = searchCriteria.ChargeCategoryId };
        parameters[18] = new ObjectParameter("SELECTED_LOCATION_I", typeof(string)) { Value = searchCriteria.BillingMemberLoc }; //CMP #655: IS-WEB Display per Location ID

        return ExecuteStoredFunction<MiscInvoiceSearchDetails>("GetMiscManageInvoices", parameters).ToList();
    }

    /// <summary>
    /// Update invoice status for duplicate BM
    /// </summary>
    /// <param name="isFileLogId"></param>
    public void UpdateInvoiceStatusForDuplicateBM(Guid isFileLogId)
    {
        var parameters = new ObjectParameter[1];

        parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.ISFileLogIdInput, typeof(Guid)) { Value = isFileLogId };

        ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.MUCorrBMDUCheckFunctionName, parameters);
    }

		/// <summary>
		/// Update invoice status for duplicate RM
		/// </summary>
		/// <param name="isFileLogId"></param>
        //SCP251726 - Two reject invoices for same original invoice number
		public void UpdateInvoiceStatusForDuplicateRM(Guid isFileLogId)
		{
			var parameters = new ObjectParameter[1];

			parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.ISFileLogIdInput, typeof (Guid))
			                	{
			                		Value = isFileLogId
			                	};

			ExecuteStoredProcedure(MiscInvoiceRepositoryConstants.MUInvRMDUCheckFunctionName, parameters);
		}

  	/// <summary>
    /// CMP288
    /// get invoice type by invoice descriptions
    /// </summary>
    /// <param name="invoiceDesc">invoice description</param>
    /// <returns>invoice type</returns>
    public string LookupTemplateType(string invoiceDesc)
    {
      var connection = new OracleConnection(Core.Configuration.ConnectionString.Instance.ServiceConnectionString);

      try
      {
        // Build the command object
        using (var dbCommand = new OracleCommand())
        {
          connection.Open();
          dbCommand.Connection = connection;
          dbCommand.CommandType = CommandType.StoredProcedure;
          dbCommand.CommandText = "GETTEMPLATECODE";
          dbCommand.Parameters.Add("Inv_Desc", OracleDbType.NVarChar).Value = invoiceDesc;

          var data = dbCommand.ExecuteReader();

          if (data.HasRows)
          {
            data.Read();
            return data.GetString("TEMPLATETYPE");
          }
          else
          {
            return string.Empty;
          }
        }
      }
      catch (Exception ex)
      {
        return string.Empty;
      }
      finally
      {
        connection.Close();
      }
    }

    /// <summary>
    /// This function is used for fetch data from database based on search criteria.
    /// </summary>
    /// <param name="offlineReportSearchCriteria"></param>
    /// <returns></returns>
    //SCP382334: Daily Bilateral screen is not loading
    public List<MUDailyPayableResultData> SearchDailyPayableInvoices(MiscSearchCriteria searchCriteria)
    {
      try
      {
        var parameters = new ObjectParameter[11];
        parameters[0] = new ObjectParameter("BILLING_CATEGORY_ID_I", typeof(int))
        {
          Value = searchCriteria.BillingCategoryId
        };
        parameters[1] = new ObjectParameter("INVOICE_NUMBER_I", typeof(string))
        {
          Value = searchCriteria.InvoiceNumber
        };
        parameters[2] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int))
        {
          Value = searchCriteria.BillingMemberId
        };
        parameters[3] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int))
        {
          Value = searchCriteria.BilledMemberId
        };
        parameters[4] = new ObjectParameter("SETTLEMENT_METHOD_ID_I", typeof(int))
        {
          Value = searchCriteria.SettlementMethodId
        };
        parameters[5] = new ObjectParameter("CHARGE_CATEGORY_ID_I", typeof(int))
        {
          Value = searchCriteria.ChargeCategoryId
        };
        parameters[6] = new ObjectParameter("LOCATION_CODE_I", typeof(String))
        {
          Value = searchCriteria.LocationCode
        };
        parameters[7] = new ObjectParameter("DELIVERY_DATE_FROM_I", typeof(String))
        {
          Value = searchCriteria.DeliveryDateFrom != null ? searchCriteria.DeliveryDateFrom.Value.ToString("dd-MM-yyyy") : null
        };
        parameters[8] = new ObjectParameter("DELIVERY_DATE_TO_I", typeof(String))
        {
          Value = searchCriteria.DeliveryDateTo != null ? searchCriteria.DeliveryDateTo.Value.ToString("dd-MM-yyyy") : null
        };
        parameters[9] = new ObjectParameter("INVOICE_TYPE_ID_I", typeof(int))
        {
          Value = searchCriteria.InvoiceTypeId
        };

        //CMP #655: IS-WEB Display per Location ID
        parameters[10] = new ObjectParameter("SELECTED_LOCATION_I", typeof(String))
        {
            Value = searchCriteria.BillingMemberLoc
        };

        var returnData = ExecuteStoredFunction<MUDailyPayableResultData>("GetDailyPayableData", parameters);
        return returnData.ToList();
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat(
          "Error occurred while fetching data from database based on search criteria. Exception caught: {0}", ex);
        throw;
      }
    }

    /// <summary>
    /// //SCP425230 - PAYABLES OPTION
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <param name="isPayableScreen"></param>
    /// <returns></returns>
    public List<MiscInvoiceSearch> SearchMiscInvoiceRecords(MiscSearchCriteria searchCriteria, bool isPayableScreen)
    {
        try
        {
            var parameters = new ObjectParameter[17];
            parameters[0] = new ObjectParameter("BILLING_CATEGORY_ID_I", typeof(int))
            {
                Value = searchCriteria.BillingCategoryId
            };
            parameters[1] = new ObjectParameter("INVOICE_NUMBER_I", typeof(string))
            {
                Value = searchCriteria.InvoiceNumber
            };
            parameters[2] = new ObjectParameter("BILLING_YEAR_I", typeof(int))
            {
                Value = searchCriteria.BillingYear
            };
            parameters[3] = new ObjectParameter("BILLING_MONTH_I", typeof(int))
            {
                Value = searchCriteria.BillingMonth
            };
            parameters[4] = new ObjectParameter("BILLING_PERIOD_I", typeof(int))
            {
                Value = searchCriteria.BillingPeriod
            };
            parameters[5] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(int))
            {
                Value = searchCriteria.BillingMemberId
            };
            parameters[6] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(int))
            {
                Value = searchCriteria.BilledMemberId
            };

            parameters[7] = new ObjectParameter("SETTLEMENT_METHOD_ID_I", typeof(int))
            {
                Value = searchCriteria.SettlementMethodId
            };
            parameters[8] = new ObjectParameter("CHARGE_CATEGORY_ID_I", typeof(int))
            {
                Value = searchCriteria.ChargeCategoryId
            };
            parameters[9] = new ObjectParameter("LOCATION_CODE_I", typeof(String))
            {
                Value = searchCriteria.LocationCode
            };
            parameters[10] = new ObjectParameter("INVOICE_STATUS_I", typeof(int))
            {
                Value = searchCriteria.InvoiceStatusId
            };
            parameters[11] = new ObjectParameter("INVOICE_TYPE_ID_I", typeof(int))
            {
                Value = searchCriteria.InvoiceTypeId
            };
            parameters[12] = new ObjectParameter("FILE_NAME_I", typeof(String))
            {
                Value = searchCriteria.FileName
            };
            parameters[13] = new ObjectParameter("OWNER_ID_I", typeof(int))
            {
                Value = searchCriteria.OwnerId
            };
            parameters[14] = new ObjectParameter("SUBMISSION_METHOD_ID", typeof(int))
            {
                Value = searchCriteria.SubmissionMethodId
            };
            parameters[15] = new ObjectParameter("IS_PAYABLE_I", typeof(int))
            {
                Value = isPayableScreen ? 1 : 0
            };
            parameters[16] = new ObjectParameter("SELECTED_LOCATION_I", typeof(String))
            {
                Value = searchCriteria.BillingMemberLoc
            };
            var returnData = ExecuteStoredFunction<MiscInvoiceSearch>("GetMiscInvoiceRecord", parameters);

            return returnData.ToList();
        }
        catch (Exception ex)
        {
            Logger.ErrorFormat(
              "Error occurred while fetching data from database based on search criteria. Exception caught: {0}", ex);
            throw;
        }
    }
	
  }
}

