using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Business.Output;
using Iata.IS.Core;
using Iata.IS.Data;
using Iata.IS.Data.Pax;
using Iata.IS.Model.Base;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.ParsingModel;
using Iata.IS.Model.Pax.Sampling;
using log4net;
using NVelocity;

namespace Iata.IS.Business.Pax.Impl
{
  public class OutputIdecGeneratorManager : IOutputIdecGeneratorManager
  {
    //private readonly IInvoiceRepository _invoiceRepository;
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public IInvoiceRepository PaxInvoiceRepository { get; set; }

    //private readonly ISamplingFormCRepository _samplingFormCRepository;

    public ISamplingFormCRepository SamplingFormCRepository { get; set; }

    public IBroadcastMessagesManager BroadcastMessagesManager { get; set; }
    /// <summary>
    /// Gets or sets the member repository.
    /// </summary>
    /// <value>The member repository.</value>
    public IRepository<Member> MemberRepository { get; set; }

    /// <summary>
    /// Gets pax invoices matching the specified search criteria
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns></returns>
    public List<PaxInvoice> GetPaxInvoicesForXml(Model.Pax.SearchCriteria searchCriteria)
    {
      return (PaxInvoiceRepository.GetInvoiceHierarchy(billedMemberId: searchCriteria.BilledMemberId, billingPeriod: searchCriteria.BillingPeriod,
                          billingMonth: searchCriteria.BillingMonth, billingYear: searchCriteria.BillingYear, invoiceStatusIds: searchCriteria.InvoiceStatusId.ToString(),
                           billingCode: searchCriteria.BillingCode));
    }

    /// <summary>
    /// Gets pax invoices matching the specified search criteria
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns></returns>
    public List<SamplingFormC> GetSamplingFormCList(Model.Pax.SearchCriteria searchCriteria)
    {
      return SamplingFormCRepository.GetSamplingFormCDataForOutputGeneration(new DateTime(searchCriteria.BillingYear, searchCriteria.BillingMonth, searchCriteria.BillingPeriod), provisionalBillingMemberId: searchCriteria.BilledMemberId).ToList();
    }

    /// <summary>
    /// To populate InvoiceModelList for OLD IDEC ouput file generation
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns>InvoiceModelList</returns>
    public InvoiceModelList GetInvoicesForOldIdec(SearchCriteria searchCriteria)
    {
      var invoiceModelList = new InvoiceModelList();
      try
      {
        var filteredList = PaxInvoiceRepository.GetOldIdecInvoiceHierarchy(billedMemberId: searchCriteria.BilledMemberId, billingPeriod: searchCriteria.BillingPeriod,
                            billingMonth: searchCriteria.BillingMonth, billingYear: searchCriteria.BillingYear, invoiceStatusId: searchCriteria.InvoiceStatusId);

        var invoiceModelCollection = new List<InvoiceModel>();
        foreach (PaxInvoice oInvoice in filteredList)
        {
          invoiceModelCollection.Add(new InvoiceModel() { Invoice = oInvoice });
        }

        //int billedMemberCodeNumeric = 0;
        //billedMemberCodeNumeric = Convert.ToInt32(GetBilledMemberCodeNumeric(searchCriteria.BilledMemberId));

        if (invoiceModelCollection.Count > 0)
        {
          invoiceModelList.FileTotal = GetOldPaxFileTotalModel(filteredList);
        }
        invoiceModelList.InvoiceModelCollection = invoiceModelCollection;
        return invoiceModelList;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// This will return data for Consolidated Provisional Invoice file
    /// </summary>
    /// <param name="billingPeriod"></param>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    public InvoiceModelList GetConsolidatedProvisionalInvoice(BillingPeriod billingPeriod, int billedMemberId)
    {
      //Note : for Consolidated prov invoices search invoices having status both ProcessingComplete and Presented
      var invoiceStatusIds = ((int)InvoiceStatusType.ProcessingComplete) + "," + ((int)InvoiceStatusType.Presented);
      var oSearchCriteria = new SearchCriteria
      {
        BillingMonth = billingPeriod.Month,
        BillingYear = billingPeriod.Year,
        BilledMemberId = billedMemberId,
        BillingCode = (int?)BillingCode.SamplingFormAB,
        InvoiceStatusIds = invoiceStatusIds
      };

      return GetConsolidatedProvisionalInvoice(oSearchCriteria);
    }
    public InvoiceModelList SystemMonitorGetConsolidatedProvisionalInvoice(BillingPeriod billingPeriod, int billedMemberId, string invoiceStatusIds)
    {
      //Note : for Consolidated prov invoices search invoices having status both ProcessingComplete and Presented
      //var invoiceStatusIds = ((int)InvoiceStatusType.ProcessingComplete) + "," + ((int)InvoiceStatusType.Presented);
      var oSearchCriteria = new SearchCriteria
      {
        BillingMonth = billingPeriod.Month,
        BillingYear = billingPeriod.Year,
        BilledMemberId = billedMemberId,
        BillingCode = (int?)BillingCode.SamplingFormAB,
        InvoiceStatusIds = invoiceStatusIds
      };

      return GetConsolidatedProvisionalInvoice(oSearchCriteria);
    }
    /// <summary>
    /// This will return data for Consolidated Provisional Invoice file
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns></returns>
    public InvoiceModelList GetConsolidatedProvisionalInvoice(SearchCriteria searchCriteria)
    {
      var invoiceModelList = new InvoiceModelList();
      var filteredList = PaxInvoiceRepository.GetInvoiceHierarchy(billedMemberId: searchCriteria.BilledMemberId, billingMonth: searchCriteria.BillingMonth,
                          billingYear: searchCriteria.BillingYear, invoiceStatusIds: searchCriteria.InvoiceStatusIds, billingCode: searchCriteria.BillingCode);
      var invoiceModelCollection = new List<InvoiceModel>();
      foreach (PaxInvoice oInvoice in filteredList)
      {
        invoiceModelCollection.Add(new InvoiceModel() { Invoice = oInvoice });
      }
      var billedMemberCodeNumeric = GetNumericMemberCode(GetBilledMemberCodeNumeric(searchCriteria.BilledMemberId));
      if (invoiceModelCollection.Count > 0)
      {
        invoiceModelList.FileTotal = GetPaxFileTotalModel(filteredList, billedMemberCodeNumeric);
      }
      invoiceModelList.FileHeader = GetPaxFileHeader(billedMemberCodeNumeric);
      invoiceModelList.InvoiceModelCollection = invoiceModelCollection;
      return invoiceModelList;
    }

    /// <summary>
    /// This will form and return the appropriate error message
    /// </summary>
    /// <param name="paxinvoiceBases">paxinvoiceBases</param>
    /// <returns>error message</returns>
    public string GetErrorMessage(IEnumerable<InvoiceBase> paxinvoiceBases)
    {
      var errorMessageStringBuilder = new StringBuilder();
      var readyForBillingOrClaimedInvoices = paxinvoiceBases.Where(invoice => invoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling || invoice.InvoiceStatus == InvoiceStatusType.Claimed);
      foreach (var paxInvoice in readyForBillingOrClaimedInvoices)
      {
        var error = new List<string>();
        var count = 0;
        if (paxInvoice.ValueConfirmationStatus == ValueConfirmationStatus.RequiredButNotRequested || paxInvoice.ValueConfirmationStatus == ValueConfirmationStatus.Requested || paxInvoice.ValueConfirmationStatus == ValueConfirmationStatus.None)
        {
          error.Add("Value Confirmation Status");
          count++;
        }
        if (paxInvoice.DigitalSignatureStatus == DigitalSignatureStatus.Pending || paxInvoice.DigitalSignatureStatus == DigitalSignatureStatus.Requested || paxInvoice.DigitalSignatureStatus == DigitalSignatureStatus.Failed)
        {
          error.Add("Digital Signature Status");
          count++;
        }
        if (paxInvoice.SupportingAttachmentStatus == SupportingAttachmentStatus.RequiredButNotRequested || paxInvoice.SupportingAttachmentStatus == SupportingAttachmentStatus.InProgress)
        {
          error.Add("Supporting Attachment Status");
          count++;
        }
        if (paxInvoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling || paxInvoice.InvoiceStatus == InvoiceStatusType.Claimed)
        {
          error.Add("Invoice Status");
          count++;
        }

        var invalidStatus = String.Join(",", error);
        if (count > 0)
        {
          var errorString = String.Format("{0} of Pax Invoice with Invoice Number : {1} is Invalid.<BR/>", invalidStatus, paxInvoice.InvoiceNumber);
          Logger.Info(errorString);
          //3661 - SIS Admin Alert messages need to be properly formatted
          errorMessageStringBuilder.AppendLine();
          errorMessageStringBuilder.AppendLine(errorString);
        }
      }
      return errorMessageStringBuilder.ToString();
    }

    /// <summary>
    /// This function will select and return invoices to write in output file 
    /// UC 3950 : point 0.7
    /// </summary>
    /// <param name="invoiceModelList"></param>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    public InvoiceModelList GetFilteredInvoices(InvoiceModelList invoiceModelList, int billedMemberId)
    {
      var newInvoiceModelList = new InvoiceModelList();
      var invoiceModels = invoiceModelList.InvoiceModelCollection;
      var invoiceModelCollection = new List<InvoiceModel>();
      var paxInvoices = new List<PaxInvoice>();
      foreach (var invoiceModel in invoiceModels)
      {
        if (invoiceModel.Invoice != null)
        {
          // No need to apply filter as only ProcessingComplete invoices are fetched.
          invoiceModelCollection.Add(new InvoiceModel()
                                       {
                                         Invoice = invoiceModel.Invoice
                                       });
          paxInvoices.Add(invoiceModel.Invoice);
        }
        else
        {
          invoiceModelCollection.Add(new InvoiceModel()
          {
            SamplingFormC = invoiceModel.SamplingFormC
          });
        }
      }

      var billedMemberCodeNumeric = GetNumericMemberCode(GetBilledMemberCodeNumeric(billedMemberId));

      newInvoiceModelList.FileHeader = invoiceModelList.FileHeader;
      if (invoiceModelCollection.Count > 0)
      {
        newInvoiceModelList.FileTotal = GetPaxFileTotalModel(paxInvoices, billedMemberCodeNumeric);
      }

      newInvoiceModelList.InvoiceModelCollection = invoiceModelCollection;
      return newInvoiceModelList;
    }

    /// <summary>
    /// Gets Pax invoices matching the specified search criteria
    /// When ever this method is get called with isOutput param value true 
    /// then DetachPaxInvoices method shoud get call after that and before the any unitofcommit.
    /// </summary>
    /// <param name="searchCriteria">The search criteria.</param>
    /// <param name="isOutput">if set to <c>true</c> [is output].</param>
    /// <returns></returns>
    public InvoiceModelList GetAllInvoicesForIdecAndSamlingFormC(Model.Pax.SearchCriteria searchCriteria, bool isOutput = false)
    {
      var invoiceModelList = new InvoiceModelList();
      // SCP249528: Changes done to improve performance of output
      var filteredList = PaxInvoiceRepository.GetInvoiceHierarchy(billedMemberId: searchCriteria.BilledMemberId > 0 ? (int?)searchCriteria.BilledMemberId : null, billingPeriod: searchCriteria.BillingPeriod,
                          billingMonth: searchCriteria.BillingMonth, billingYear: searchCriteria.BillingYear, invoiceStatusIds: searchCriteria.InvoiceStatusIds,
                          billingCode: searchCriteria.BillingCode, billingMemberId: searchCriteria.BillingMemberId > 0 ? (int?)searchCriteria.BillingMemberId : null, isOutput: isOutput);

      List<InvoiceModel> invoiceModelCollection = new List<InvoiceModel>();
     
      List<PrimeCoupon> primeCouponLists = new List<PrimeCoupon>();
      if (isOutput)
      {
          Stopwatch watch = new Stopwatch();
          watch.Start();
          var paxOutputMgr = new PaxOutputManager();
          //Get prime coupons from outside of Entity framework context and add into invoice's coupon collection
        primeCouponLists = paxOutputMgr.GetPrimeCouponList(filteredList,
                                                           billedMemberId: searchCriteria.BilledMemberId > 0 ? (int?) searchCriteria.BilledMemberId : null,
                                                           billingPeriod: searchCriteria.BillingPeriod,
                                                           billingMonth: searchCriteria.BillingMonth,
                                                           billingYear: searchCriteria.BillingYear,
                                                           invoiceStatusIds: searchCriteria.InvoiceStatusIds);
          //Detach invoice object so it goes outside of Entity framework
          watch.Stop();
          Logger.Info(" Coupons Count :" + primeCouponLists.Count + " Time :" + watch.Elapsed);
      }
      foreach (PaxInvoice oInvoice in filteredList)
      {
        // SCP249528: Changes done to improve performance of output
        if (isOutput)
        {
          if (oInvoice != null && primeCouponLists.Count > 0)
          {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //add coupons in invoice coupon collection
            var coupons = primeCouponLists.Where(c => c.InvoiceId == oInvoice.Id).ToList();
            if (coupons.Count > 0)
              oInvoice.CouponDataRecord.AddRange(coupons);
            watch.Stop();
            Logger.Info(oInvoice.InvoiceNumber + " - Coupons Count :" + oInvoice.CouponDataRecord.Count + " Time :" + watch.Elapsed);
          }
        }

        invoiceModelCollection.Add(new InvoiceModel() { Invoice = oInvoice });
      }
      //Changes done to handle type B member
      //var billedMemberCodeNumeric = GetNumericMemberCode(GetBilledMemberCodeNumeric(searchCriteria.BilledMemberId));



      //invoiceModelList.FileHeader = GetPaxFileHeader(billedMemberCodeNumeric);

      // For Form C, we are applying the ProcessingComplete and Presented, in regular as well as reprocessing.
      var invoiceStatusIdList = string.Format("{0},{1}", (int) InvoiceStatusType.ProcessingComplete, (int) InvoiceStatusType.Presented);
      var filteredFormCList = SamplingFormCRepository.GetSamplingFormCDataForOutputGeneration(new DateTime(searchCriteria.BillingYear, searchCriteria.BillingMonth, searchCriteria.BillingPeriod), searchCriteria.BilledMemberId, invoiceStatusId: invoiceStatusIdList);

      foreach (var oFormC in filteredFormCList)
      {
        invoiceModelCollection.Add(new InvoiceModel() { SamplingFormC = oFormC });
      }

      if (invoiceModelCollection.Count > 0)
      {
        //Changes done to handle type B member
        var billedMemberCodeNumeric = GetNumericMemberCode(GetBilledMemberCodeNumeric(searchCriteria.BilledMemberId));
        invoiceModelList.FileHeader = GetPaxFileHeader(billedMemberCodeNumeric);
        invoiceModelList.FileTotal = GetPaxFileTotalModel(filteredList, billedMemberCodeNumeric);
      }

      invoiceModelList.InvoiceModelCollection = invoiceModelCollection;
      return invoiceModelList;
    }

    public void DetachPaxInvoices(List<InvoiceBase> paxinvoiceBases)
    {
      foreach (var oInvoice in paxinvoiceBases)
      {
        //Detach invoice object so it goes outside of Entity framework
        PaxInvoiceRepository.Detach((PaxInvoice)oInvoice);
      }
    }

    /// <summary>
    /// Gets AutoBilling invoices matching the specified search criteria
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <param name="couponSearchCriteriaString">SCP215457: Daily RRF Query. To include only those coupons in DRR report that haveing 'INCLUDE_IN_DAILY_REV_RECOGN' flag is set to zero.</param>
    /// <returns></returns>
    public InvoiceModelList GetAutoBillingInvoices(Model.Pax.SearchCriteria searchCriteria, string couponSearchCriteriaString = null)
    {
      var invoiceModelList = new InvoiceModelList();

      // SCP215457: Daily RRF Query. To include only those coupons in DRR report that haveing 'INCLUDE_IN_DAILY_REV_RECOGN' flag is set to zero.
      var filteredList = PaxInvoiceRepository.GetInvoiceHierarchy(billingPeriod: searchCriteria.BillingPeriod,
                          billingMonth: searchCriteria.BillingMonth, billingYear: searchCriteria.BillingYear, invoiceStatusIds: searchCriteria.InvoiceStatusIds,
                          billingMemberId: searchCriteria.BillingMemberId > 0 ? (int?)searchCriteria.BillingMemberId : null, submissionMethodId: searchCriteria.SubmissionMethodId, couponSearchCriteriaString: couponSearchCriteriaString);

      List<InvoiceModel> invoiceModelCollection = new List<InvoiceModel>();
      foreach (PaxInvoice oInvoice in filteredList)
      {
        invoiceModelCollection.Add(new InvoiceModel() { Invoice = oInvoice });
      }

      var billingMemberCodeNumeric = GetNumericMemberCode(GetBilledMemberCodeNumeric(searchCriteria.BillingMemberId));
      invoiceModelList.FileHeader = GetPaxFileHeader(billingMemberCodeNumeric);

      if (invoiceModelCollection.Count > 0)
      {
        invoiceModelList.FileTotal = GetPaxFileTotalModel(filteredList, billingMemberCodeNumeric);
      }
      invoiceModelList.InvoiceModelCollection = invoiceModelCollection;

      return invoiceModelList;
    }

    public InvoiceModelList SystemMonitorGetAllInvoicesForIdecAndSamlingFormC(Model.Pax.SearchCriteria searchCriteria)
    {
      var invoiceModelList = new InvoiceModelList();

      var filteredList = PaxInvoiceRepository.GetInvoiceHierarchy(billedMemberId: searchCriteria.BilledMemberId > 0 ? (int?)searchCriteria.BilledMemberId : null, billingPeriod: searchCriteria.BillingPeriod,
                          billingMonth: searchCriteria.BillingMonth, billingYear: searchCriteria.BillingYear, invoiceStatusIds: searchCriteria.InvoiceStatusIds,
                          billingCode: searchCriteria.BillingCode, billingMemberId: searchCriteria.BillingMemberId > 0 ? (int?)searchCriteria.BillingMemberId : null);

      List<InvoiceModel> invoiceModelCollection = new List<InvoiceModel>();
      foreach (PaxInvoice oInvoice in filteredList)
      {
        invoiceModelCollection.Add(new InvoiceModel() { Invoice = oInvoice });
      }

      var billedMemberCodeNumeric = GetNumericMemberCode(GetBilledMemberCodeNumeric(searchCriteria.BilledMemberId));



      invoiceModelList.FileHeader = GetPaxFileHeader(billedMemberCodeNumeric);

      var filteredFormCList = SamplingFormCRepository.GetSamplingFormCDataForOutputGeneration(new DateTime(searchCriteria.BillingYear, searchCriteria.BillingMonth, searchCriteria.BillingPeriod), searchCriteria.BilledMemberId, invoiceStatusId: searchCriteria.InvoiceStatusIds);

      foreach (var oFormC in filteredFormCList)
      {
        invoiceModelCollection.Add(new InvoiceModel() { SamplingFormC = oFormC });
      }

      if (invoiceModelCollection.Count > 0)
      {
        invoiceModelList.FileTotal = GetPaxFileTotalModel(filteredList, billedMemberCodeNumeric);
      }

      invoiceModelList.InvoiceModelCollection = invoiceModelCollection;
      return invoiceModelList;
    }

    /// <summary>
    /// Gets the numeric member code.
    /// </summary>
    /// <param name="memberCode">The member code.</param>
    /// <returns></returns>
    public int GetNumericMemberCode(string memberCode)
    {
      var index = 0;
      int value;

      if (Validators.IsWholeNumber(memberCode))
      {
        return Convert.ToInt32(memberCode);
      }

      var memberCodeAsciiChars = new byte[memberCode.Length];
      Encoding.ASCII.GetBytes(memberCode.ToUpper(), 0, memberCode.Length, memberCodeAsciiChars, 0);
      foreach (var memberCodeAsciiValue in memberCodeAsciiChars)
      {
        if (memberCodeAsciiValue <= 90 && memberCodeAsciiValue >= 65)
        {
          //To get A = 10, B=11
          value = memberCodeAsciiValue - 55;
          string toReplace = memberCode.Substring(index, 1);
          memberCode = memberCode.Replace(toReplace, value.ToString());
        }
        index++;
      }

      int numericMemberCode;
      int returnValue;
      if (Int32.TryParse(memberCode, out numericMemberCode))
      {
        returnValue = numericMemberCode > 9999 ? 0 : numericMemberCode;
      }
      else
      {
        returnValue = 0;
      }

      return returnValue;
    }


    /// <summary>
    /// Returns MemberCodeNumeric of respective billed member
    /// </summary>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    private string GetBilledMemberCodeNumeric(int billedMemberId)
    {
      var member = MemberRepository.First(i => i.Id == billedMemberId);
      if (member != null) return member.MemberCodeNumeric;
      return "0";
    }

    /// <summary>
    /// Generates and returns FileHeader model
    /// </summary>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    private FileHeader GetPaxFileHeader(int billedMemberId)
    {
      FileHeader fileHeader = new FileHeader();
      fileHeader.AirlineCode = billedMemberId;
      return fileHeader;
    }

    /// <summary>
    /// Generates and returns the FileTotal model 
    /// </summary>
    /// <param name="invoiceCollection"></param>
    /// <returns></returns>
    private FileTotal GetOldPaxFileTotalModel(List<PaxInvoice> invoiceCollection)
    {
      var fileTotal = new FileTotal();
      decimal totalGrossValue = 0, totalIscAmount = 0, totalTaxAmount = 0, netTotalAmount = 0, netBillingAmount = 0;
      int noOfBillingRecords = 0;
      double totalHandlingFee = 0;

      foreach (PaxInvoice invoice in invoiceCollection)
      {
        var invoiceTotal = invoice.InvoiceTotalRecord;
        if (invoiceTotal != null)
        {
          totalGrossValue += invoiceTotal.TotalGrossValue > 0 ? invoiceTotal.TotalGrossValue : -(invoiceTotal.TotalGrossValue);

          decimal totalInterlineServiceCharge = 0;
          if (invoice.BillingCode == 0)
          {
            totalInterlineServiceCharge = invoiceTotal.TotalIscAmount + invoiceTotal.TotalOtherCommission + invoiceTotal.TotalUatpAmount;
          }
          else if (invoice.BillingCode == 3)
          {
            totalInterlineServiceCharge = invoiceTotal.TotalProvisionalAdjustmentAmount;
          }
          totalIscAmount += totalInterlineServiceCharge > 0 ? totalInterlineServiceCharge : -(totalInterlineServiceCharge);

          var totalTax = invoiceTotal.TotalTaxAmount + invoiceTotal.TotalVatAmount;
          totalTaxAmount += totalTax > 0 ? totalTax : -(totalTax);

          var netTotal = totalGrossValue + totalInterlineServiceCharge + totalTax;
          netTotalAmount += netTotal > 0 ? netTotal : -(netTotal);
          
          //CMP#648: Convert Exchange rate into nullable field.
          var netBillingAmt = (netTotal * (1 / (invoice.ExchangeRate.HasValue? invoice.ExchangeRate.Value:0.0M)));

          netBillingAmount += netBillingAmt > 0 ? netBillingAmt : -(netBillingAmt);

          noOfBillingRecords += invoiceTotal.NoOfBillingRecords;

          totalHandlingFee += invoiceTotal.TotalHandlingFee > 0 ? invoiceTotal.TotalHandlingFee : -(invoiceTotal.TotalHandlingFee);
        }
      }
      fileTotal.TotalGrossValue = totalGrossValue;
      fileTotal.TotalInterlineServiceChargeAmount = totalIscAmount;
      fileTotal.TotalTaxAmount = totalTaxAmount;
      fileTotal.NetTotal = netTotalAmount;
      fileTotal.NetBillingAmount = netBillingAmount;
      fileTotal.NoOfBillingRecords = noOfBillingRecords;
      fileTotal.HandlingFeeAmount = totalHandlingFee;
      return fileTotal;
    }

    /// <summary>
    /// Generates and returns the FileTotal model 
    /// </summary>
    /// <param name="invoiceCollection"></param>
    /// <returns></returns>
    private FileTotal GetPaxFileTotalModel(List<Model.Pax.PaxInvoice> invoiceCollection, int billedMemberId)
    {
      FileTotal fileTotal = new FileTotal();
      decimal totalGrossValue = 0, totalIscAmount = 0, totalTaxAmount = 0, netTotal = 0, netBillingAmount = 0, totalOtherCommission = 0, totalUatpAmount = 0;
      decimal totalVatAmount = 0;
      int noOfBillingRecords = 0, totalNoOfRecords = 0;
      double totalHandlingFee = 0;
      foreach (PaxInvoice invoice in invoiceCollection)
      {
        
        if (invoice.InvoiceTotalRecord != null)
        {
          totalGrossValue += invoice.InvoiceTotalRecord.TotalGrossValue < 0 ? -(ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.TotalGrossValue)) : ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.TotalGrossValue);
          totalIscAmount += invoice.InvoiceTotalRecord.TotalIscAmount < 0 ? -(ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.TotalIscAmount)) : ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.TotalIscAmount);
          totalTaxAmount += invoice.InvoiceTotalRecord.TotalTaxAmount < 0 ? -(ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.TotalTaxAmount)) : ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.TotalTaxAmount);
          netTotal += invoice.InvoiceTotalRecord.NetTotal < 0 ? -(ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.NetTotal)) : ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.NetTotal);
          netBillingAmount += invoice.InvoiceTotalRecord.NetBillingAmount < 0 ? -(ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.NetBillingAmount)) : ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.NetBillingAmount);
          noOfBillingRecords += invoice.InvoiceTotalRecord.NoOfBillingRecords < 0 ? -(invoice.InvoiceTotalRecord.NoOfBillingRecords) : invoice.InvoiceTotalRecord.NoOfBillingRecords;
          totalHandlingFee += invoice.InvoiceTotalRecord.TotalHandlingFee < 0 ? -(Convert.ToDouble(ConvertUtil.TruncateToTwoDecimal((decimal)invoice.InvoiceTotalRecord.TotalHandlingFee))) : (Convert.ToDouble(ConvertUtil.TruncateToTwoDecimal((decimal)invoice.InvoiceTotalRecord.TotalHandlingFee)));
          totalOtherCommission += invoice.InvoiceTotalRecord.TotalOtherCommission < 0 ? -(ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.TotalOtherCommission)) : ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.TotalOtherCommission);
          totalUatpAmount += invoice.InvoiceTotalRecord.TotalUatpAmount < 0 ? -(ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.TotalUatpAmount)) : ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.TotalUatpAmount);
          totalVatAmount += invoice.InvoiceTotalRecord.TotalVatAmount < 0 ? -(ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.TotalVatAmount)) : ConvertUtil.TruncateToTwoDecimal(invoice.InvoiceTotalRecord.TotalVatAmount);
          totalNoOfRecords += invoice.InvoiceTotalRecord.TotalNoOfRecords < 0 ? -(invoice.InvoiceTotalRecord.TotalNoOfRecords) : invoice.InvoiceTotalRecord.TotalNoOfRecords;
        }

        if (invoice.SamplingFormEDetails != null)
        {
          netBillingAmount += invoice.SamplingFormEDetails.NetBilledCreditedAmount < 0 ? -(ConvertUtil.TruncateToTwoDecimal(invoice.SamplingFormEDetails.NetBilledCreditedAmount)) : ConvertUtil.TruncateToTwoDecimal(invoice.SamplingFormEDetails.NetBilledCreditedAmount);
          //netBillingAmount += invoice.SamplingFormEDetails.NetBilledCreditedAmount;
          noOfBillingRecords += invoice.SamplingFormEDetails.NumberOfBillingRecords;
        }
      }

      fileTotal.TotalGrossValue = totalGrossValue;
      fileTotal.TotalInterlineServiceChargeAmount = totalIscAmount;
      fileTotal.TotalTaxAmount = totalTaxAmount;
      fileTotal.NetTotal = netTotal;
      fileTotal.NetBillingAmount = netBillingAmount;
      fileTotal.NoOfBillingRecords = noOfBillingRecords;
      fileTotal.HandlingFeeAmount = totalHandlingFee;
      fileTotal.TotalOtherCommissionAmount = totalOtherCommission;
      fileTotal.TotalUatpAmount = totalUatpAmount;
      fileTotal.TotalVatAmount = totalVatAmount;
      fileTotal.TotalNumberOfRecords = totalNoOfRecords;
      fileTotal.BilledAirline = billedMemberId;
      return fileTotal;
    }
  }
}
