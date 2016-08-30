using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Iata.IS.AdminSystem;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Security;
using Iata.IS.Business.SupportingDocuments;
using Iata.IS.Core;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.MemberProfile;
using iPayables_PDFCreator;
using Iata.IS.LegalXMLFileGenerator;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.LegalXmlGenerator;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.SupportingDocuments.Enums;
using Iata.IS.Web.Areas.Misc.Models;
using Iata.IS.Web.UIModel;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Misc;
using Iata.IS.Web.UIModel.Grid.MU;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using log4net;

namespace Iata.IS.Web.Controllers.Base
{
  public class MiscUatpControllerBase : InvoiceControllerBase<MiscUatpInvoice>
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
   
    private readonly IMiscUatpInvoiceManager _invoiceManager;
    /// <summary>
    /// Gets or sets the reference manager.
    /// </summary>
    /// <value>The reference manager.</value>
    public IReferenceManager ReferenceManager { get; set; }

    public IMiscCorrespondenceManager CorrespondenceManager
    {
      get;
      set;
    }

    private readonly IReferenceManager _referenceManager;

    public ICalendarManager CalendarManager { private get; set; }
      
    private const string DefaultUomCode = "EA";
    private const string OrgNameDelimiter = "!!!";

    public IValidationErrorManager ValidationManager
    {
      get;
      set;
    }

    protected MiscUatpControllerBase(IMiscUatpInvoiceManager miscUatpInvoiceManager, IReferenceManager referenceManager, IMemberManager memberManager)
    {
      _invoiceManager = miscUatpInvoiceManager;
      _referenceManager = referenceManager;
      MemberManager = memberManager;
    }

    protected virtual BillingCategoryType BillingCategory
    {
      get;
      set;
    }

    protected override bool IsValidBillingCode(InvoiceBase invoiceBase)
    {
      var miscInvoice = (MiscUatpInvoice)invoiceBase;
      bool isValidBillingCode = false;

      if (miscInvoice != null)
      {
        if (miscInvoice.BillingCategory == BillingCategory)
        {
          isValidBillingCode = true;
        }
        // check if Invoice is on type credit note, then the controller 
        // should be for Credit Note.
        // for other invoice, Controller should of Invoice.
        isValidBillingCode = miscInvoice.InvoiceType == InvoiceType.CreditNote ? miscInvoice.InvoiceType == InvoiceType.CreditNote : (miscInvoice.InvoiceType == InvoiceType.Invoice || miscInvoice.InvoiceType == InvoiceType.RejectionInvoice || miscInvoice.InvoiceType == InvoiceType.CorrespondenceInvoice);
      }

      return isValidBillingCode;
    }

    protected override MiscUatpInvoice GetInvoiceHeader(string invoiceNumber)
    {
      return _invoiceManager.GetInvoiceDetail(invoiceNumber);
    }

    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public virtual ActionResult CreateBillingMemo(string rejectedInvoiceId, long correspondenceReferenceNumber)
    {
      SessionUtil.InvoiceSearchCriteria = null;
      SetViewDataPageMode(PageMode.Create);
      //ViewData["IsFromBillingHistory"] = true;
      var correspondence = CorrespondenceManager.GetCorrespondenceDetails(correspondenceReferenceNumber);

      var billedMemberId = 0;
      decimal amountToBeSettled = correspondence.AmountToBeSettled;
      var correspondenceStatus = correspondence.CorrespondenceStatusId;
      bool isAuthorityToBill = correspondenceStatus != (int)CorrespondenceStatus.Expired;
      var rejectedInvoiceNumber = string.Empty;
      int chargeCategoryId = 0;
      int? billingCurrencyId = 0;
      int? listingCurrencyId = 0;
      decimal exchangeRate = 0;
      int originalMonth = 0;
      int originalYear = 0;
      int originalPeriod = 0;
      string poNumber = string.Empty;
      var rejectedinvoice = GetInvoiceHeader(rejectedInvoiceId);
      int previousInvoiceSmi = -1;

      if (rejectedinvoice != null)
      {
        previousInvoiceSmi = rejectedinvoice.SettlementMethodId;
        //SCP219674 - Billing Memos with 6A & 6B can be rejected(issue : while creating corr invoice,on invoice header, currency of billing was enabled)
        var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
        var miscUatpInvoice = new MiscUatpInvoice();
        if (calendarManager != null)
        {
          var currentBillingPeriod = calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
          if (currentBillingPeriod != null)
          {
            miscUatpInvoice.BillingYear = currentBillingPeriod.Year;
            miscUatpInvoice.BillingMonth = currentBillingPeriod.Month;
            miscUatpInvoice.BillingPeriod = currentBillingPeriod.Period;
          }
        }
        // Get original billing month to calculate exchange rate.
        var originalInvoice = _invoiceManager.GetOriginalInvoice(miscUatpInvoice, correspondence);
        // _invoiceManager.GetOriginalInvoice(rejectedinvoice.InvoiceNumber, rejectedinvoice.BillingMemberId);

        if (originalInvoice != null)
        {
          originalMonth = originalInvoice.BillingMonth;
          originalYear = originalInvoice.BillingYear;
          originalPeriod = originalInvoice.BillingPeriod;
        }

        rejectedInvoiceNumber = rejectedinvoice.InvoiceNumber;

        // CMP609: MISC Changes Required as per ISW2.
        // When a Correspondence Invoice is initiated for a correspondence eligible for settlement, the Charge Category
        // of the correspondence is automatically inherited and defined as the Charge Category of the Correspondence Invoice
        // Since Charge Category ‘Miscellaneous’ has been discontinued, it cannot be inherited anymore
        // Therefore, a Correspondence Invoice is created for a correspondence having Charge Category as ‘Miscellaneous’,
        // the Charge Category of the Correspondence Invoice should be set as ‘Finance’
        // ChargeCategoryId are: 9 = Miscellaneous, 14 = Finance
        chargeCategoryId = rejectedinvoice.BillingCategory == BillingCategoryType.Misc && rejectedinvoice.ChargeCategoryId == 9 ? 14 : rejectedinvoice.ChargeCategoryId;

        //   billingCurrencyId = rejectedinvoice.BillingCurrencyId;
        listingCurrencyId = rejectedinvoice.ListingCurrencyId;
        // exchangeRate = rejectedinvoice.ExchangeRate;
        poNumber = rejectedinvoice.PONumber;
        //Set billed member Id of corr invoice from Billing MemberId of Rejection invoice
        billedMemberId = rejectedinvoice.BillingMemberId;
      }

      //Create correspondence invoice from rejected invoice and correspondence.
      var invoice = new MiscUatpInvoice
      {
        IsCreatedFromBillingHistory = true,
        RejectedInvoiceNumber = rejectedInvoiceNumber,
        PONumber = poNumber,
        ChargeCategoryId = chargeCategoryId,
        BillingCurrencyId = billingCurrencyId,
        ListingCurrencyId = listingCurrencyId,
        ExchangeRate = exchangeRate,
        InvoiceDate = DateTime.Today,
        BillingMemberId = SessionUtil.MemberId,
        BillingCategoryId = (int)BillingCategoryType.Misc,
        BillingCategory = BillingCategory,
        InvoiceSummary = new InvoiceSummary { TotalAmount = amountToBeSettled },
        PaymentDetail = new PaymentDetail(),
        InvoiceType = InvoiceType.CorrespondenceInvoice,
        BilledMemberId = billedMemberId,
        CorrespondenceRefNo = correspondenceReferenceNumber,
        IsAuthorityToBill = isAuthorityToBill,
        BilledMember = MemberManager.GetMember(billedMemberId),
        DigitalSignatureRequiredId = GetDigitalSignatureRequired(SessionUtil.MemberId), 
        SettlementMonth = originalMonth,
        SettlementYear = originalYear,
        SettlementPeriod = originalPeriod
      };
      if(previousInvoiceSmi == (int)SMI.IchSpecialAgreement)
      {
        invoice.SettlementMethodId = previousInvoiceSmi;
      }
      //var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
     
      //ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      var smIsToBeTreatedBilateralOtherThanX = smIsToBeTreatedBilateral.Where(smi => smi != (int)SMI.IchSpecialAgreement).ToList();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateralOtherThanX);


      MakeInvoiceRenderReady(new Guid(), invoice);

      return View("Create", invoice);
    }

    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public virtual ActionResult CreateRejectionInvoice(string rejectedInvoiceId)
    {

      ViewData["IsFromBillingHistory"] = true;
      var rejectedInvoiceNumber = string.Empty;
      int chargeCategoryId = 0;
      int? billingCurrencyId = 0;
      int? listingCurrencyId = 0;
      decimal exchangeRate = 0;
      int billingMonth = 0;
      int billingYear = 0;
      int period = 0;
      int rejectionStage = 0;
      string poNumber = string.Empty;
      int billedMemberId = 0;
      string billingMemberLocationCode = string.Empty;
      string billedMemberLocationCode = string.Empty;
      string billingMemberContactName = string.Empty;
      string billedMemberContactName = string.Empty;
      string cityAirportCode = string.Empty;
      int previousInvoiceSmi = -1;
      if (TempData != null)
      {
        if (TempData.ContainsKey("rejectedLineItemIds") && TempData["rejectedLineItemIds"] != null)
        {
          string lineItemIds = TempData["rejectedLineItemIds"].ToString();
          TempData.Clear();
          TempData["rejectedLineItemIds"] = lineItemIds;
          TempData["searchtype"] = Request.Form["searchType"];//SIS_SCR_REPORT_23_jun-2016_2:Value_Shadowing
        }
      }

      //var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      //ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      var smIsToBeTreatedBilateralOtherThanX = smIsToBeTreatedBilateral.Where(smi => smi != (int)SMI.IchSpecialAgreement).ToList();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateralOtherThanX);

      SessionUtil.InvoiceSearchCriteria = null;
      SetViewDataPageMode(PageMode.Create);

      var rejectedinvoice = GetInvoiceHeader(rejectedInvoiceId);

      if (rejectedinvoice != null)
      {
        previousInvoiceSmi = rejectedinvoice.SettlementMethodId;
        rejectedInvoiceNumber = rejectedinvoice.InvoiceNumber;

        // CMP609: MISC Changes Required as per ISW2. 
        // When an invoice is rejected from the Payables screen, the Charge Category of the Rejected Invoice/CreditNote
        // is automatically inherited and defined as the Charge Category of the Rejection Invoice
        // Since Charge Category ‘Miscellaneous’ has been discontinued, it cannot be inherited anymore
        // Therefore, if an Invoice/CreditNote billed with discontinued Charge Category ‘Miscellaneous’ is rejected,
        // the Charge Category of the Rejection Invoice should be set as ‘Finance’
        // ChargeCategoryId are: 9 = Miscellaneous, 14 = Finance
        chargeCategoryId = rejectedinvoice.BillingCategory == BillingCategoryType.Misc && rejectedinvoice.ChargeCategoryId == 9 ? 14 : rejectedinvoice.ChargeCategoryId;

        //billingCurrencyId = rejectedinvoice.BillingCurrencyId;
        listingCurrencyId = rejectedinvoice.ListingCurrencyId;
          
        //exchangeRate = rejectedinvoice.ExchangeRate;
        poNumber = rejectedinvoice.PONumber;
        billingMonth = rejectedinvoice.BillingMonth;
        billingYear = rejectedinvoice.BillingYear;
        period = rejectedinvoice.BillingPeriod;
        rejectionStage = rejectedinvoice.RejectionStage + 1;
        billedMemberId = rejectedinvoice.BillingMemberId;
        billingMemberContactName = rejectedinvoice.BilledMemberContactName;
        billedMemberContactName = rejectedinvoice.BillingMemberContactName;
        billingMemberLocationCode = rejectedinvoice.BilledMemberLocationCode;
        billedMemberLocationCode = rejectedinvoice.BillingMemberLocationCode;
        cityAirportCode = rejectedinvoice.LocationCode;

        if (rejectedinvoice.MemberLocationInformation != null && rejectedinvoice.MemberLocationInformation.Count > 0)
        {
            var locInfo = new List<MemberLocationInformation>();

            foreach (var info in rejectedinvoice.MemberLocationInformation)
            {
                var loc = info;

                loc.OrganizationName = info.OrganizationName.Replace(OrgNameDelimiter, string.Empty);
                
                locInfo.Add(loc);
            }

            rejectedinvoice.MemberLocationInformation = locInfo;
        }

      }

      var invoice = new MiscUatpInvoice
                      {
                        IsCreatedFromBillingHistory = true,
                        RejectedInvoiceNumber = rejectedInvoiceNumber,
                        PONumber = poNumber,
                        ChargeCategoryId = chargeCategoryId,
                        BillingCurrencyId = billingCurrencyId,
                        ListingCurrencyId = listingCurrencyId,
                        BillingCategoryId = (int)BillingCategoryType.Misc,
                        BillingCategory = BillingCategory,
                        InvoiceSummary = new InvoiceSummary(),
                        PaymentDetail = new PaymentDetail(),
                        ExchangeRate = exchangeRate,
                        InvoiceDate = DateTime.Today,
                        BillingMemberId = SessionUtil.MemberId,
                        InvoiceType = InvoiceType.RejectionInvoice,
                        BilledMemberId = billedMemberId,
                        IsAuthorityToBill = false,
                        BilledMember = MemberManager.GetMember(billedMemberId),
                        SettlementMonth = billingMonth,
                        SettlementYear = billingYear,
                        SettlementPeriod = period,
                        RejectionStage = rejectionStage,
                        DigitalSignatureRequiredId = GetDigitalSignatureRequired(SessionUtil.MemberId),
                        BillingMemberLocationCode = billingMemberLocationCode,
                        BilledMemberLocationCode = billedMemberLocationCode,
                        BillingMemberContactName = billingMemberContactName,
                        BilledMemberContactName = billedMemberContactName,
                        LocationCode = cityAirportCode
                      };
      if (previousInvoiceSmi == (int)SMI.IchSpecialAgreement)
      {
        invoice.SettlementMethodId = previousInvoiceSmi;
      }
      // Reverse the billing, billed member contact and locations for rejection invoice.
      if (rejectedinvoice != null)
      {
        invoice.MemberContacts.AddRange(rejectedinvoice.MemberContacts);

        foreach (var contact in invoice.MemberContacts)
        {
          if (contact.MemberType == Iata.IS.Model.MiscUatp.Enums.MemberType.Billed)
            contact.MemberTypeId = (int)Iata.IS.Model.MiscUatp.Enums.MemberType.Billing;
          else if (contact.MemberType == Iata.IS.Model.MiscUatp.Enums.MemberType.Billing) contact.MemberTypeId = (int)Iata.IS.Model.MiscUatp.Enums.MemberType.Billed;
        }

        invoice.MemberLocationInformation = new List<MemberLocationInformation>(rejectedinvoice.MemberLocationInformation);
        foreach (var memberLocation in invoice.MemberLocationInformation)
        {
          memberLocation.IsBillingMember = !memberLocation.IsBillingMember;
        }
      }

      MakeInvoiceRenderReady(new Guid(), invoice);
      // Setting page mode to Edit so as to not lose the billing and billed member location codes, reference data set in this function.
      // Otherwise the location codes default to Main location.
      ViewData[ViewDataConstants.PageMode] = PageMode.Edit;

      return View("Create", invoice);
    }

    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public virtual ActionResult Create()
    {
      // Retrieve memberId value from Session and use it across method
      var memberId = SessionUtil.MemberId;

      SessionUtil.InvoiceSearchCriteria = null;
      // Clearing the other two session variables so that 'Back to Billing History' is not seen.
      SessionUtil.MiscCorrSearchCriteria = null;
      SessionUtil.MiscInvoiceSearchCriteria = null;
      SessionUtil.SearchType = null;
      
      SetViewDataPageMode(PageMode.Create);

      var digitalSignatureRequired = GetDigitalSignatureRequired(memberId);
      ViewData[ViewDataConstants.DefaultDigitalSignatureRequiredId] = digitalSignatureRequired;
      
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      var smIsToBeTreatedBilateralOtherThanX = smIsToBeTreatedBilateral.Where(smi => smi != (int)SMI.IchSpecialAgreement).ToList();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateralOtherThanX);

      var invoice = new MiscUatpInvoice { InvoiceDate = DateTime.Today, BillingMemberId = memberId, BillingCategory = BillingCategory, InvoiceSummary = new InvoiceSummary(), PaymentDetail = new PaymentDetail(), InvoiceType = InvoiceType, BilledMemberId = 0, IsAuthorityToBill = true, DigitalSignatureRequiredId = digitalSignatureRequired};
      MakeInvoiceRenderReady(new Guid(), invoice);

      // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
      // To display TaxSubType drop down when Billing category is Misc set ViewData["isTaxNameDropdown"] to True.
      if (invoice.BillingCategoryId != null && invoice.BillingCategoryId == (int)BillingCategoryType.Misc)
      {
        ViewData["isTaxNameDropdown"] = "True";
      }
      else
      {
        ViewData["isTaxNameDropdown"] = "False";
      }
      // CMP #534: Tax Issues in MISC and UATP Invoices. [End]

      return View(invoice);
    }

    #region SCP197299: Incorrect ISIDEC format - PIDECT-21720130904.ZIP

    /// <summary>
    /// Method is used to check the valid settlement Billing Year/Month/Period
    /// </summary>
    /// <param name="settlementYear"></param>
    /// <param name="settlementMonth"></param>
    /// <param name="settlementPeriod"></param>
    /// <returns>true or false</returns>
    private bool IsYourBillingDateValid(int settlementYear, int settlementMonth, int settlementPeriod)
    {
      DateTime yourInvoiceBillingDate;
      var yourInvoiceDateString = string.Format("{2}{1}{0}", Convert.ToString(settlementPeriod).PadLeft(2, '0'),
                                                             Convert.ToString(settlementMonth).PadLeft(2, '0'),
                                                             Convert.ToString(settlementYear).PadLeft(4, '0'));

      if (DateTime.TryParseExact(yourInvoiceDateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out yourInvoiceBillingDate))
      {
        if (yourInvoiceBillingDate.Day < 1 || yourInvoiceBillingDate.Day > 4)
        {
          return false;
        }
      }
      else
      {
        return false;
      }
      return true;
    }

    #endregion

    [HttpPost]
    public virtual ActionResult Create(MiscUatpInvoice invoiceHeader)
    {
      // Retrieve userId value from Session and use it across method
      var userId = SessionUtil.UserId;

      string lineItemIds = null;

      // SCP#417067: Validations for Notes and Legal text
      var msgErrorCode = string.Empty;

      try
      {
          //SCP141404: IS-IDEC File error - Cargo
        if (invoiceHeader.RejectionStage <= 0 && invoiceHeader.InvoiceTypeId == 3)
        {
          throw new ISBusinessException(MiscUatpErrorCodes.InvalidRejectionDetailStage);
        }

        //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
        //SCP226313: ERROR MESSAGES
        if (invoiceHeader.InvoiceType == InvoiceType.CorrespondenceInvoice)
          CorrespondenceManager.ValidateCorrespondenceReference(invoiceHeader.CorrespondenceRefNo, BillingCategory);

        #region SCP197299: Incorrect ISIDEC format - PIDECT-21720130904.ZIP
        if (invoiceHeader.InvoiceType == InvoiceType.RejectionInvoice)
        {
          if (!IsYourBillingDateValid(invoiceHeader.SettlementYear, invoiceHeader.SettlementMonth, invoiceHeader.SettlementPeriod))
          {
            throw new ISBusinessException("Invalid Your Invoice Billing Date.", "");
          }
        }
        #endregion

        invoiceHeader.BillingCategory = BillingCategory;
        invoiceHeader.SubmissionMethod = SubmissionMethod.IsWeb;
        invoiceHeader.LastUpdatedBy = SessionUtil.UserId;
        int invoiceOwnerId = 0;
        if (userId > 0)
        {
          invoiceOwnerId = userId;
        }
        else
        {
            /* SCP# 303708 - Invoice Search - Note Owner
             * Desc: Server side validation is added to prevent value 0 as Invoice/Credit Note owner Id. */
            throw new ISBusinessException(ErrorCodes.OwnerMissing); 
        }

        invoiceHeader.InvoiceOwnerId = invoiceOwnerId;

        //CMP #655: IS-WEB Display per Location ID
        ValidateLocationAssociation(invoiceHeader);
       

        /* SCP# 414515 - Inquiry about Exchange Rate
         * Desc: Commented below code since settlement month and settlement year was useful to get original invoice exchange rate 
         * for validation purposes. But Same code is required before saving the invoice in DB. */
        //// Explicitly set the settlement month to 0 in case of Correspondence Invoice as set and used in CreateBillingMemo.
        //if(invoiceHeader.InvoiceType == InvoiceType.CorrespondenceInvoice)
        //{
        //  invoiceHeader.SettlementYear = 0;
        //  invoiceHeader.SettlementMonth = 0;
        //  invoiceHeader.SettlementPeriod = 0;
        //}

        //CMP #648: Clearance Information in MISC Invoice PDFs
        if (invoiceHeader.BillingCategory == BillingCategoryType.Misc && (ReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId,false)))
        {
          msgErrorCode = string.Empty;
          invoiceHeader = _invoiceManager.ValidateIswebMiscInvExchangeRate(invoiceHeader, out msgErrorCode);
          if (!string.IsNullOrEmpty(msgErrorCode))
          {
            throw new ISBusinessException(msgErrorCode);
          }
        }

        //CMP #648: Clearance Information in MISC Invoice PDFs
        if (invoiceHeader.ExchangeRate.HasValue)
        {
          if (invoiceHeader.ExchangeRate.Value > 0)
          {
            // Update TotalAmountInClearanceCurrency Amount
            invoiceHeader.InvoiceSummary.TotalAmountInClearanceCurrency = invoiceHeader.BillingAmount/
                                            Convert.ToDecimal(invoiceHeader.ExchangeRate.Value);
          }
          else
          {
            invoiceHeader.InvoiceSummary.TotalAmountInClearanceCurrency = 0.0M;
          }
        }
        else
        {
          invoiceHeader.InvoiceSummary.TotalAmountInClearanceCurrency = (decimal?)null;
        }

        // SCP#417067: Validations for Notes and Legal text
        if (invoiceHeader.AdditionalDetails.Count > 0)
        {
            invoiceHeader = _invoiceManager.ValidateIswebMiscInvHeaderNoteDescription(invoiceHeader, out msgErrorCode);
            if (!string.IsNullOrEmpty(msgErrorCode))
            {
                throw new ISBusinessException(msgErrorCode);
            }
        }

        if (invoiceHeader.IsCreatedFromBillingHistory && (TempData.ContainsKey("rejectedLineItemIds") && TempData["rejectedLineItemIds"] != null) && invoiceHeader.InvoiceType == InvoiceType.RejectionInvoice)
        {
          lineItemIds = TempData["rejectedLineItemIds"].ToString();
          invoiceHeader = _invoiceManager.CreateBHRejectionInvoice(invoiceHeader, lineItemIds);
        }
        else
        {
          invoiceHeader = _invoiceManager.CreateInvoice(invoiceHeader);
        }

        // Display different success message depending on Invoice Type.
        ShowSuccessMessage(invoiceHeader.InvoiceType == InvoiceType.CreditNote ? Messages.CreditNoteCreateSuccessful :
                                                                          //SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod ?
                                                                          //Messages.InvoiceCreateSuccessful + " " + Messages.ValidationIgnoredDueToMigration :
                                                                          Messages.InvoiceCreateSuccessful);

        return invoiceHeader.IsCreatedFromBillingHistory && invoiceHeader.InvoiceType == InvoiceType.RejectionInvoice
                 ? RedirectToAction("Edit",
                                    new
                                      {
                                        invoiceId = invoiceHeader.Id
                                      })
                 : RedirectToAction("LineItemCreate",
                                    new
                                      {
                                        invoiceId = invoiceHeader.Id
                                      });
      }
      catch (ISBusinessException businessException)
      {
          /* CMP #624: ICH Rewrite-New SMI X 
              * Description: As per ICH Web Service Response Message specifications 
              * Refer FRS Section 3.3 Table 9. 
              * TFS Issue #9205: CMP 624: Incorrect error if SMI is changed to X on ISWEB. */

          var validationResultFail = "F"; // F when Invoice/Credit Note fails validation in ICH
          var validationResultError = "E"; // E when ICH receives a Bad Request from SIS
          invoiceHeader.BilledMember = MemberManager.GetMember(invoiceHeader.BilledMemberId);

          if (!string.IsNullOrWhiteSpace(invoiceHeader.ChValidationResult) && invoiceHeader.ChValidationResult.Equals(validationResultFail, StringComparison.CurrentCultureIgnoreCase))
          {
              ShowSmiXWebServiceErrorMessage(businessException.Message);
          }
          else //if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) && invoice.ChValidationResult.Equals(validationResultError, StringComparison.CurrentCultureIgnoreCase))
          {
              ShowErrorMessage(businessException.ErrorCode);
          }
        MakeInvoiceRenderReady(new Guid(), invoiceHeader);
        // To display BilledMemberText
        
        ViewData[ViewDataConstants.DefaultDigitalSignatureRequiredId] = GetDigitalSignatureRequired(invoiceHeader.BillingMemberId);

        if (invoiceHeader.IsCreatedFromBillingHistory && invoiceHeader.InvoiceType == InvoiceType.RejectionInvoice && !string.IsNullOrEmpty(lineItemIds))
        {
          TempData["rejectedLineItemIds"] = lineItemIds;
        }

        //var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
        //ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);
        var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
        var smIsToBeTreatedBilateralOtherThanX = smIsToBeTreatedBilateral.Where(smi => smi != (int)SMI.IchSpecialAgreement).ToList();
        ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateralOtherThanX);

        if (invoiceHeader.MemberLocationInformation != null)
        {
          var billingMemberLocationInfo = invoiceHeader.MemberLocationInformation.Find(locInfo => locInfo.IsBillingMember);
          if (billingMemberLocationInfo != null && !string.IsNullOrEmpty(billingMemberLocationInfo.LegalText))
          {
            ViewData["IsLegalTextSet"] = true;
          }
        }
      }
      finally
      {
        SetViewDataPageMode(PageMode.Create);
      }

      return View(invoiceHeader);
    }

    [HttpGet]
    public virtual ActionResult Edit(string invoiceId)
    {
      //var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      //ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      var smIsToBeTreatedBilateralOtherThanX = smIsToBeTreatedBilateral.Where(smi => smi != (int)SMI.IchSpecialAgreement).ToList();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateralOtherThanX);
      // Below ViewData is set in order to hide the Reject button in case of Receivables.
      ViewData["Reject"] = false;
      ViewData[ViewDataConstants.PageMode] = PageMode.Edit;
      MakeInvoiceRenderReady(invoiceId.ToGuid(), InvoiceHeader);

      ViewData[ViewDataConstants.TransactionExists] = _invoiceManager.IsLineItemExists(invoiceId);
      ViewData[ViewDataConstants.IsLocationCodePresent] = _invoiceManager.IsLocationCodePresent(invoiceId);

      if (InvoiceHeader.InvoiceType == InvoiceType.RejectionInvoice)
      {
        var rejctionInvoiceDetail = _invoiceManager.GetRejectedInvoiceDetails(InvoiceHeader.RejectedInvoiceNumber, InvoiceHeader.SettlementMethodId, InvoiceHeader.BilledMemberId, InvoiceHeader.BillingMemberId, InvoiceHeader.SettlementMonth,InvoiceHeader.SettlementYear ,InvoiceHeader.SettlementPeriod);

        if (rejctionInvoiceDetail.BillingPeriod.Year != 0 || rejctionInvoiceDetail.BillingPeriod.Month != 0)
        {
          ViewData[ViewDataConstants.RejectedInvoiceNumberExist] = true;
        }
      }

      if (InvoiceHeader.InvoiceStatus == InvoiceStatusType.ValidationError)
      {
        // Get all submitted errors.
        var submittedErrorsGrid = new SubmittedErrorsGrid(ControlIdConstants.SubmittedErrorsGridId, Url.Action("GetSubmittedErrors", new { invoiceId }));
        ViewData[ViewDataConstants.SubmittedErrorsGrid] = submittedErrorsGrid.Instance;
      }

      if (InvoiceHeader.MemberLocationInformation != null)
      {
        var billingMemberLocationInfo = InvoiceHeader.MemberLocationInformation.Find(locInfo => locInfo.IsBillingMember);
        if (billingMemberLocationInfo != null && !string.IsNullOrEmpty(billingMemberLocationInfo.LegalText))
        {
          ViewData["IsLegalTextSet"] = true;
        }
      }

      // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
      // To display TaxSubType drop down when Billing category is Misc set ViewData["isTaxNameDropdown"] to True.
      if (InvoiceHeader.BillingCategoryId != null && InvoiceHeader.BillingCategoryId == (int)BillingCategoryType.Misc)
      {
        ViewData["isTaxNameDropdown"] = "True";
      }
      else
      {
        ViewData["isTaxNameDropdown"] = "False";
      }
      // CMP #534: Tax Issues in MISC and UATP Invoices. [End]

      return View(InvoiceHeader);
    }

    [HttpGet]
    public new virtual ActionResult View(string invoiceId)
    {
      //var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      //ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      var smIsToBeTreatedBilateralOtherThanX = smIsToBeTreatedBilateral.Where(smi => smi != (int)SMI.IchSpecialAgreement).ToList();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateralOtherThanX);

      MakeInvoiceRenderReady(invoiceId.ToGuid(), InvoiceHeader);


      if (InvoiceHeader != null && InvoiceHeader.MemberLocationInformation.Count > 0)
      {
        foreach (var memLocation in InvoiceHeader.MemberLocationInformation)
        {
          memLocation.LegalText = InvoiceHeader.LegalText;
        }
      }

      ViewData[ViewDataConstants.TransactionExists] = _invoiceManager.IsLineItemExists(invoiceId);

      // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
      var authorizationManager = Ioc.Resolve<IAuthorizationManager>();

      if (InvoiceHeader != null)
      {
          if (InvoiceHeader.BillingCategory == BillingCategoryType.Misc)
          {
              TempData["canCreate"] = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Misc.Receivables.Invoice.CreateOrEdit);
              TempData["canView"] = InvoiceHeader.InvoiceType == InvoiceType.CreditNote
                                    ? authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Misc.Payables.CreditNoteBillings.View)
                                    : authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Misc.Payables.Invoice.View);
          }

          if (InvoiceHeader.BillingCategory == BillingCategoryType.Uatp)
          {
              TempData["canCreate"] = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.Receivables.Invoice.CreateOrEdit);
              TempData["canView"] = InvoiceHeader.InvoiceType == InvoiceType.CreditNote
                                    ? authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.Payables.CreditNoteBillings.View)
                                    : authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.Payables.Invoice.View);
          }
      }

        return View("Edit", InvoiceHeader);
    }

    [HttpPost]
    // This is not an Ajax call, as after Validate, page should refresh and show Submit button.
    public virtual ActionResult ValidateInvoice(string invoiceId)
    {
      try
      {
        var miscUatpInvoice = _invoiceManager.ValidateInvoice(invoiceId);

        switch (miscUatpInvoice.InvoiceStatus)
        {
          case InvoiceStatusType.ReadyForSubmission:
            ShowSuccessMessage(string.Format(Messages.InvoiceValidateSuccessful, miscUatpInvoice.InvoiceNumber));
            break;

          default:
            ShowErrorMessage(string.Format(Messages.InvoiceValidateFailed, miscUatpInvoice.InvoiceNumber), true);
            break;
        }
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode, true);
      }

      return RedirectToAction("Edit", new { invoiceId });
    }

    [HttpPost]
    public virtual ActionResult Submit(string invoiceId)
    {
      MiscUatpInvoice submittedInvoice = null;
      var controllerName = string.Empty;

      try
      {
        submittedInvoice = _invoiceManager.SubmitInvoice(invoiceId);
        controllerName = submittedInvoice.InvoiceType == InvoiceType.CreditNote ? BillingCategory + "CreditNote" : BillingCategory + "Invoice";

        switch (submittedInvoice.InvoiceStatus)
        {
          case InvoiceStatusType.ReadyForBilling:
          case InvoiceStatusType.FutureSubmitted:
            TempData[ViewDataConstants.SuccessMessage] = string.Format(Messages.InvoiceSubmissionSuccessful, submittedInvoice.InvoiceNumber);
            break;
          case InvoiceStatusType.ValidationError:
            TempData[ViewDataConstants.ErrorMessage] = string.Format(Messages.InvoiceSubmissionFailed, submittedInvoice.InvoiceNumber, EnumMapper.GetInvoiceStatusDisplayValue((int)submittedInvoice.InvoiceStatus));
            break;
        }
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode, true);
      }

      return RedirectToAction((submittedInvoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling || submittedInvoice.InvoiceStatus == InvoiceStatusType.FutureSubmitted) ? "View" : "Edit", controllerName, new { invoiceId });
    }

    [HttpPost]
    public virtual JsonResult RejectInvoice(string invoiceId, string lineItemId, string searchType,string invoiceType)
    {
      if (!string.IsNullOrEmpty(lineItemId))
      {
        TempData["rejectedLineItemIds"] = lineItemId;
      }

      #region CMP #678: Time Limit Validation on Last Stage MISC Rejections

      if (invoiceType == "MiscInvoice")
      {
          /*Billing History Screen*/
          var invoiceHeader = _invoiceManager.GetInvoiceHeader(invoiceId);

          var rmInTimeLimtMsg = _invoiceManager.ValidateMiscLastStageRmForTimeLimit(invoiceHeader,
                                                                                    null,
                                                                                    RmValidationType.BillingHistory);
          if (!string.IsNullOrWhiteSpace(rmInTimeLimtMsg))
          {
              return Json(new UIMessageDetail() {isRedirect = false, Message = rmInTimeLimtMsg});
          }
      }

        #endregion

      var details = new UIMessageDetail() { isRedirect = true, RedirectUrl = Url.Action("CreateRejectionInvoice", invoiceType, new { rejectedInvoiceId = invoiceId, searchType }) };

      return Json(details);
    }

    /// <summary>
    /// Rejection through Billing History.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [HttpGet]
    public virtual ActionResult ShowDetails(string invoiceId)
    {
      //// Retrieve memberId value from Session and use it across method
      var memberId = SessionUtil.MemberId;
      bool flag = false;

      if (!_invoiceManager.IsRejectionInvoiceExistsWithAnyStatus(invoiceId))
      {
        if (InvoiceHeader.RejectionStage == 1)
        {
            // Get Final Parent Details for SMI,Clearing House validations
            var billingFinalParent = MemberManager.GetFinalParentDetails(InvoiceHeader.BillingMemberId);
            var billedFinalParent = MemberManager.GetFinalParentDetails(InvoiceHeader.BilledMemberId);
            InvoiceHeader.InvoiceSmi = _referenceManager.GetDefaultSettlementMethodForMembers(billingFinalParent, billedFinalParent, InvoiceHeader.BillingCategoryId);

          flag = (InvoiceHeader.InvoiceSmi == SMI.Ach || InvoiceHeader.InvoiceSmi == SMI.AchUsingIataRules) && InvoiceHeader.BillingMemberId != memberId && InvoiceHeader.InvoiceType != InvoiceType.CorrespondenceInvoice;
        }
        else if (InvoiceHeader.RejectionStage == 2)
        {
          flag = false;
        }
        else
        {
          if (InvoiceHeader.BillingMemberId != memberId && InvoiceHeader.InvoiceType != InvoiceType.CorrespondenceInvoice)
          {
            flag = true;
          }
        }
      }
      else
      {
        flag = false;
      }
      //Check Whether invoice is already rejected or not
      if (!flag)
      {
        ViewData["Reject"] = false;
      }
      else
      {
        TempData["Reject"] = true;

        // SCP#477993 : SRM - initiate Rejection MISC invoice from Billing history screen
        var authorizationManager = Ioc.Resolve<IAuthorizationManager>();

        if (InvoiceHeader != null)
        {
            if (InvoiceHeader.BillingCategory == BillingCategoryType.Misc)
            {
                TempData["canCreate"] = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Misc.Receivables.Invoice.CreateOrEdit);
                TempData["canView"] = InvoiceHeader.InvoiceType == InvoiceType.CreditNote
                                      ? authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Misc.Payables.CreditNoteBillings.View)
                                      : authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Misc.Payables.Invoice.View);
            }

            if (InvoiceHeader.BillingCategory == BillingCategoryType.Uatp)
            {
                TempData["canCreate"] = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.Receivables.Invoice.CreateOrEdit);
                TempData["canView"] = InvoiceHeader.InvoiceType == InvoiceType.CreditNote
                                      ? authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.Payables.CreditNoteBillings.View)
                                      : authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.Payables.Invoice.View);
            }
        }
      }

      MakeInvoiceRenderReady(invoiceId.ToGuid(), InvoiceHeader);

      return View("Edit", InvoiceHeader);
    }

    private void MakeInvoiceRenderReady(Guid invoiceId, MiscUatpInvoice invoice)
    {
      // Initialize member location information for the invoice.
      InitMemberLocationInfo(invoice);

      if (invoice.InvoiceSummary == null)
      {
        invoice.InvoiceSummary = new InvoiceSummary();
      }

      var lineItemGrid = new LineItemGrid(ControlIdConstants.LineItemGridId, Url.Action("GetLineItems", new { invoiceId }), BillingCategory == BillingCategoryType.Uatp, isDisplayRejectionReasonCode: (invoice.InvoiceTypeId == (int)InvoiceType.RejectionInvoice));
      ViewData[ViewDataConstants.LineItemGrid] = lineItemGrid.Instance;


      string routeName = "Derived_Vat";
      if(BillingCategory == BillingCategoryType.Uatp)
      {
        routeName = "Uatp_Derived_Vat";
      }
      //Get derived vat
      var derivedVatGrid = new DerivedVatGrid(ControlIdConstants.DerivedVatGridId, Url.RouteUrl(routeName, new { action = "GetDerivedVat", parentId = invoiceId.ToString() }));
      ViewData[ViewDataConstants.DerivedVatGrid] = derivedVatGrid.Instance;

      // workaround for exception that occurs when invoice(not having payment details) uploaded through IS-XML is viewed on IS-Web.
      if (invoice.PaymentDetail == null)
      {
        invoice.PaymentDetail = new PaymentDetail();
      }
    }

    [HttpPost]
    public virtual ActionResult Edit(string invoiceId, MiscUatpInvoice invoiceHeader)
    {
      var lineItems = _invoiceManager.GetLineItemList(invoiceId);
      string ExchangeRateValidationFlag = "EX";
      var billingCurrencyId = invoiceHeader.BillingCurrencyId;

      // SCP#417067: Validations for Notes and Legal text
      var msgErrorCode = string.Empty;
      
        // SCP#417067: Validations for Notes and Legal text [TFSBug#9700 - Rel 1.7.13.0]
        List<MiscUatpInvoiceAdditionalDetail> tempMiscUatpInvoiceAdditionalDetail = null;

      try
      {
          //CMP #655: IS-WEB Display per Location ID
          ValidateLocationAssociation(invoiceHeader);

        //SCP141404: IS-IDEC File error - Cargo
        if (invoiceHeader.RejectionStage <= 0 && invoiceHeader.InvoiceTypeId == 3)
        {
          throw new ISBusinessException(MiscUatpErrorCodes.InvalidRejectionDetailStage);
        }

        #region SCP197299: Incorrect ISIDEC format - PIDECT-21720130904.ZIP
        if (invoiceHeader.InvoiceType == InvoiceType.RejectionInvoice)
        {
          if (!IsYourBillingDateValid(invoiceHeader.SettlementYear, invoiceHeader.SettlementMonth, invoiceHeader.SettlementPeriod))
          {
            throw new ISBusinessException("Invalid Your Invoice Billing Date.", "");
          }
        }
        #endregion
        #region 279473 - Misc credit note- inconsistency between ISWEB and File behavior
        if (invoiceHeader.InvoiceType == InvoiceType.CreditNote)
        {
          if (!ReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId, true) && (invoiceHeader.ListingCurrencyId != invoiceHeader.BillingCurrencyId))
          {
            invoiceHeader.IsValidationFlag = ExchangeRateValidationFlag;
          }
          else
          {
            invoiceHeader.IsValidationFlag = null;
          }
        }
        #endregion
        _invoiceManager.ValidateLocationCode(lineItems.FirstOrDefault(), invoiceHeader.LocationCode);

        //SCP327666 - AIATSL - Query about SIS charge category/code for invoice MA68123027
        _invoiceManager.ValidateChargeCategory(lineItems.FirstOrDefault(), invoiceId, invoiceHeader.ChargeCategoryId);

        MakeInvoiceUpdateReady(invoiceId, invoiceHeader);

        invoiceHeader.LineItems.AddRange(lineItems);
        invoiceHeader.LastUpdatedBy = SessionUtil.UserId;

        //CMP #648: Clearance Information in MISC Invoice PDFs
          if (invoiceHeader.BillingCategory == BillingCategoryType.Misc && ((ReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId, false))))
        {
          msgErrorCode = string.Empty;
          invoiceHeader = _invoiceManager.ValidateIswebMiscInvExchangeRate(invoiceHeader, out msgErrorCode);
          if (!string.IsNullOrEmpty(msgErrorCode))
          {
            throw new ISBusinessException(msgErrorCode);
          }
        }

        //CMP #648: Clearance Information in MISC Invoice PDFs
          if (invoiceHeader.ExchangeRate.HasValue)
        {
          if (invoiceHeader.ExchangeRate.Value > 0)
          {
            // Update TotalAmountInClearanceCurrency Amount
            invoiceHeader.InvoiceSummary.TotalAmountInClearanceCurrency = invoiceHeader.BillingAmount / Convert.ToDecimal(invoiceHeader.ExchangeRate.Value);
          }
          else
          {
            invoiceHeader.InvoiceSummary.TotalAmountInClearanceCurrency = 0.0M;
          }
        }
        else
        {
          invoiceHeader.InvoiceSummary.TotalAmountInClearanceCurrency = null;
        }

          // SCP#417067: Validations for Notes and Legal text
          if (invoiceHeader.AdditionalDetails.Count > 0)
          {
              invoiceHeader = _invoiceManager.ValidateIswebMiscInvHeaderNoteDescription(invoiceHeader, out msgErrorCode);
              if (!string.IsNullOrEmpty(msgErrorCode))
              {
                  tempMiscUatpInvoiceAdditionalDetail = invoiceHeader.AdditionalDetails;
                  throw new ISBusinessException(msgErrorCode);
              }
          }

        invoiceHeader = _invoiceManager.UpdateInvoice(invoiceHeader);

        // Display different success message depending on Invoice Type.
        //ShowSuccessMessage(invoiceHeader.InvoiceType == InvoiceType.CreditNote ? Messages.CreditNoteUpdateSuccessful : Messages.InvoiceUpdateSuccessful);
        ShowSuccessMessage(invoiceHeader.InvoiceType == InvoiceType.CreditNote
                             ? Messages.CreditNoteUpdateSuccessful
                             : //SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod ?
                           //Messages.InvoiceUpdateSuccessful + " " + Messages.ValidationIgnoredDueToMigration :
                           Messages.InvoiceUpdateSuccessful);

        return RedirectToAction("Edit", new {invoiceId});
      }
      catch (ISBusinessException businessException)
      {
        //SCP327666 - AIATSL - Query about SIS charge category/code for invoice MA68123027
        invoiceHeader = _invoiceManager.GetInvoiceHeader(invoiceId);

        invoiceHeader.BillingCurrencyId = billingCurrencyId;

        // SCP#417067: Validations for Notes and Legal text [TFSBug#9700 - Rel 1.7.13.0]
        if (tempMiscUatpInvoiceAdditionalDetail != null)
        {
            invoiceHeader.AdditionalDetails = tempMiscUatpInvoiceAdditionalDetail;
        }

          SetViewDataPageMode(PageMode.Edit);

        /* CMP #624: ICH Rewrite-New SMI X 
            * Description: As per ICH Web Service Response Message specifications 
            * Refer FRS Section 3.3 Table 9. 
            * TFS Issue #9205: CMP 624: Incorrect error if SMI is changed to X on ISWEB. */

        var validationResultFail = "F"; // F when Invoice/Credit Note fails validation in ICH
        var validationResultError = "E"; // E when ICH receives a Bad Request from SIS
        invoiceHeader.BilledMember = MemberManager.GetMember(invoiceHeader.BilledMemberId);

        if (!string.IsNullOrWhiteSpace(invoiceHeader.ChValidationResult) && invoiceHeader.ChValidationResult.Equals(validationResultFail, StringComparison.CurrentCultureIgnoreCase))
        {
          ShowSmiXWebServiceErrorMessage(businessException.Message);
        }
        else //if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) && invoice.ChValidationResult.Equals(validationResultError, StringComparison.CurrentCultureIgnoreCase))
        {
          ShowErrorMessage(businessException.ErrorCode);
        }


        MakeInvoiceRenderReady(invoiceId.ToGuid(), invoiceHeader);
        var attachments = _invoiceManager.GetAttachments(invoiceId);
        invoiceHeader.Attachments.AddRange(attachments);
        ViewData[ViewDataConstants.TransactionExists] = _invoiceManager.IsLineItemExists(invoiceId);
        //CMP #624: TFS:9339 System is behaving unexpectedly fro MISC invoices through IS WEB
        ViewData[ViewDataConstants.IsLocationCodePresent] = _invoiceManager.IsLocationCodePresent(invoiceId);

        //var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
        //ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);
        var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
        var smIsToBeTreatedBilateralOtherThanX = smIsToBeTreatedBilateral.Where(smi => smi != (int)SMI.IchSpecialAgreement).ToList();
        ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateralOtherThanX);

        if (invoiceHeader.MemberLocationInformation != null)
        {
          var billingMemberLocationInfo = invoiceHeader.MemberLocationInformation.Find(locInfo => locInfo.IsBillingMember);
          if (billingMemberLocationInfo != null && !string.IsNullOrEmpty(billingMemberLocationInfo.LegalText))
          {
            ViewData["IsLegalTextSet"] = true;
          }
        }

        return View(invoiceHeader);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }
    }

    private void MakeInvoiceUpdateReady(string invoiceId, MiscUatpInvoice invoiceHeader)
    {
      var invoiceGuid = invoiceId.ToGuid();
      invoiceHeader.Id = invoiceGuid;
      invoiceHeader.SubmissionMethod = SubmissionMethod.IsWeb;
      invoiceHeader.BillingCategory = BillingCategory;
      if (invoiceHeader.InvoiceSummary != null)
      {
        invoiceHeader.InvoiceSummary.InvoiceId = invoiceGuid;
      }
      if (invoiceHeader.PaymentDetail != null)
      {
        invoiceHeader.PaymentDetail.InvoiceId = invoiceGuid;
      }

      //if (invoiceHeader.TaxBreakdown != null && invoiceHeader.TaxBreakdown.Count > 0)
      //{
      //  foreach (var taxBrakdown in invoiceHeader.TaxBreakdown)
      //  {
      //    taxBrakdown.ParentId = invoiceGuid;
      //  }
      //}

      //if (invoiceHeader.AddOnCharges != null && invoiceHeader.AddOnCharges.Count > 0)
      //{
      //  foreach (var addOnCharge in invoiceHeader.AddOnCharges)
      //  {
      //    addOnCharge.ParentId = invoiceGuid;
      //  }
      //}

      //if (invoiceHeader.AdditionalDetails != null && invoiceHeader.AdditionalDetails.Count > 0)
      //{
      //  foreach (var additionalDetail in invoiceHeader.AdditionalDetails)
      //  {
      //    additionalDetail.InvoiceId = invoiceGuid;
      //  }
      //}

      if (invoiceHeader.MemberContacts != null && invoiceHeader.MemberContacts.Count > 0)
      {
        foreach (var memberContact in invoiceHeader.MemberContacts)
        {
          memberContact.InvoiceId = invoiceGuid;
        }
      }
    }

      /// <summary>
      /// CMP #655: IS-WEB Display per Location ID
      /// </summary>
      /// <param name="invoiceHeader"></param>
      private void ValidateLocationAssociation(MiscUatpInvoice invoiceHeader)
      {
          var memberLocation = Ioc.Resolve<IManageContactsManager>(typeof(IManageContactsManager)); // IOC resolve for interface
          var associatedLocations = memberLocation.GetMemberAssoLocForInvCapture(SessionUtil.UserId, SessionUtil.MemberId);
          if (associatedLocations.Count > 0)
          {
              var contains = associatedLocations.SingleOrDefault(l => l.LocationCode == invoiceHeader.BillingMemberLocationCode);
              if (contains == null)
              {
                  throw new ISBusinessException(MiscErrorCodes.NoLongerAssociatedWithLocation);
              }
          }
          else
          {
              throw new ISBusinessException(MiscErrorCodes.NoLongerAssociatedWithLocation);
          }
      }

      [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public virtual ActionResult LineItemCreate(string invoiceId)
    {
      SetViewDataPageMode(PageMode.Create);
      var lineItem = new LineItem();
      lineItem.Invoice = InvoiceHeader;

      // Select ATCAN by default in Charge Code dropdown.
      var chargeCodeATCAN = _invoiceManager.GetChargeCode("ATCAN");
      if(chargeCodeATCAN != null)
        lineItem.ChargeCodeId = chargeCodeATCAN.Id;

      MakeLineItemRenderReady(lineItem);

      // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
      // To display TaxSubType drop down when Billing category is Misc set ViewData["isTaxNameDropdown"] to True.
      if (lineItem.Invoice.BillingCategoryId != null && lineItem.Invoice.BillingCategoryId == (int)BillingCategoryType.Misc)
      {
        ViewData["isTaxNameDropdown"] = "True";
      }
      else
      {
        ViewData["isTaxNameDropdown"] = "False";
      }
      // CMP #534: Tax Issues in MISC and UATP Invoices. [End]

      return View(lineItem);
    }

    private void MakeLineItemRenderReady(LineItem lineItem)
    {
      var invoice = lineItem.Invoice;
      // Line Item number will be set to Max line item number + 1 if line item count is > 0.
      //lineItem.LineItemNumber = invoice.LineItems.Count > 0 ? invoice.LineItems.Max(lItem => lItem.LineItemNumber) + 1 : 1;
      lineItem.LineItemNumber = _invoiceManager.GetMaxLineItemNumber(invoice.Id) + 1;
      lineItem.UomCodeId = DefaultUomCode;
      //prepopulate service start date and end date as per current month
      lineItem.StartDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
      lineItem.EndDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month));
      lineItem.Quantity = 1;
    }

    [HttpPost]
    public virtual ActionResult LineItemCreate(LineItem lineItem)
    {
      try
      {

        _invoiceManager.ValidateLocationCode(lineItem, null, lineItem.InvoiceId.ToString("N"));

        //SCP327666 - AIATSL - Query about SIS charge category/code for invoice MA68123027
        _invoiceManager.ValidateChargeCategory(lineItem,lineItem.InvoiceId.ToString("N"),null);

        lineItem.LastUpdatedBy = SessionUtil.UserId;
        var lineItemObject = _invoiceManager.AddLineItem(lineItem);

        ShowSuccessMessage(Messages.LineItemCreateSuccessful);

        if (lineItemObject.Invoice.InvoiceType == InvoiceType.Invoice && _invoiceManager.IsFieldMetaDataExists(lineItemObject.ChargeCodeId, lineItemObject.ChargeCodeTypeId, (int)BillingCategory))
        {
          // If line item detail is expected redirect to Line Item Detail Create screen.
          return RedirectToAction("LineItemDetailCreate", new { lineItemId = lineItemObject.Id, invoiceId = lineItemObject.InvoiceId });
        }
        else
        {
          // Else line item Edit screen.
          return RedirectToAction("LineItemEdit", new { lineItemId = lineItemObject.Id, invoiceId = lineItemObject.InvoiceId });
        }
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);

        // Get invoice header from database and set in new Line Item object.
        lineItem.Invoice = _invoiceManager.GetInvoiceDetail(lineItem.InvoiceId.ToString());
        ViewData[ViewDataConstants.IsPostback] = true;
      }
      //finally
      //{
      //  SetViewDataPageMode(PageMode.Create);
      //}

      return View(lineItem);
    }

    // Called on Edit Invoice header page 
    public JsonResult GetLineItems(string invoiceId)
    {
        var invoice = _invoiceManager.GetInvoiceDetail(invoiceId);
      var lineItems = from lineItem in _invoiceManager.GetLineItemList(invoiceId)
                      select new
                               {
                                 lineItem.Id,
                                 lineItem.LineItemNumber,
                                 lineItem.DisplayChargeCode,
                                 ChargeCodeType = lineItem.ChargeCodeType != null ? lineItem.ChargeCodeType.Name : string.Empty,
                                 lineItem.StartDate,
                                 lineItem.EndDate,
                                 //CMP#502 : [3.7] Rejection Reason for MISC Invoices
                                 lineItem.RejectionReasonCode,
                                 lineItem.Description,
                                 lineItem.Quantity,
                                 lineItem.UomCodeId,
                                 lineItem.UnitPrice,
                                 lineItem.ScalingFactor,
                                 lineItem.Total,
                                 lineItem.TotalTaxAmount,
                                 lineItem.TotalVatAmount,
                                 lineItem.TotalAddOnChargeAmount,
                                 lineItem.TotalNetAmount
                               };

      var lineItemGrid = new LineItemGrid(ControlIdConstants.LineItemGridId, Url.Action("GetLineItems", new { invoiceId }), BillingCategory == BillingCategoryType.Uatp, isDisplayRejectionReasonCode: (invoice.InvoiceTypeId == (int)InvoiceType.RejectionInvoice));
      return lineItemGrid.DataBind(lineItems.AsQueryable());
    }

    // Called on Edit Invoice header page 
    public JsonResult GetSubmittedErrors(string invoiceId)
    {
      var submittedErrors = ValidationManager.GetValidationErrors(invoiceId);
      var submittedErrorsGrid = new SubmittedErrorsGrid(ControlIdConstants.SubmittedErrorsGridId, Url.Action("GetSubmittedErrors", new { invoiceId }));
      return submittedErrorsGrid.DataBind(submittedErrors);
    }

    /// <summary>
    /// Get derived vat records
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public JsonResult GetDerivedVat(string parentId)
    {
      var derivedVatGrid = new DerivedVatGrid(ControlIdConstants.DerivedVatGridId, Url.RouteUrl("Derived_Vat", new { action = "GetDerivedVat", parentId = parentId }));
      //Url.Action("GetDerivedVat", new { parentId })
      var derivedVat = _invoiceManager.GetDerivedVatDetails(parentId);
      return derivedVatGrid.DataBind(derivedVat.AsQueryable());
    }

    /// <summary>
    /// Called on Edit LineItem page.
    /// </summary>
    /// <param name="lineItemId">The line item id.</param>
    /// <returns></returns>
    public JsonResult GetLineItemDetails(string lineItemId)
    {
      var lineItemDetails = _invoiceManager.GetLineItemDetailList(lineItemId);
      var lineItemDetailGrid = new LineItemDetailGrid(ControlIdConstants.LineItemDetailGridId, Url.Action("GetLineItemDetails", new { lineItemId }));
      return lineItemDetailGrid.DataBind(lineItemDetails.AsQueryable());
    }

    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public virtual ActionResult LineItemDetailCreate(string invoiceId, string lineItemId)
    {
      SetViewDataPageMode(PageMode.Create);

      // TODO: Remove hardcoded values after line item implementation is done
      var invoice = _invoiceManager.GetInvoiceHeader(invoiceId);
      var lineItem = _invoiceManager.GetLineItemHeaderInformation(lineItemId);
      lineItem.Invoice = invoice;

      var considerLineItemDate = false;
      // check if temp data has value for retaining line item detail start date.
      // pass this to the view, so that start date can be retained.
      // this value is set in Line Item Details Create Post method.
      if (TempData[ViewDataConstants.RetainLineItemDetailStartDate] != null)
      {
        if ((int)TempData[ViewDataConstants.RetainLineItemDetailStartDate] != -1)
        {
          ViewData[ViewDataConstants.RetainLineItemDetailStartDate] = TempData[ViewDataConstants.RetainLineItemDetailStartDate];
        }
      }
      else
      {
        considerLineItemDate = true;
      }

      var detail = PrepopulateLineItemDetail(lineItem, considerLineItemDate);

      

      int? chargeCodeTypeId = null;
      if (lineItem.ChargeCodeTypeId != 0)
      {
        chargeCodeTypeId = lineItem.ChargeCodeTypeId;
      }
      //CMP #636: Standard Update Mobilization.
      //Added billing category id for getting field meta data.
      var requiredFields = _invoiceManager.GetFieldMetadata(lineItem.ChargeCodeId, chargeCodeTypeId, null, (int)BillingCategory);
      var lineItemDetailModel = new LineItemDetailViewModel { LineItemDetail = detail, RequiredFields = requiredFields };

      // CMP # 533: RAM A13 New Validations and New Charge Code [Start]
      // To display Product Id field as dropdown or as textbox
      if (invoice.BillingCategoryId == (int)BillingCategoryType.Misc && 
          invoice.ChargeCategoryDisplayName.ToLower().Equals(ControlIdConstants.ServiceProvider) && lineItem.DisplayChargeCode.ToLower().Equals(ControlIdConstants.Gds))
      { 
        // For the Billing Category = Misc and ChargeCategory = Service Provider and Charge Code = GDS set ViewData as true.
        ViewData[ViewDataConstants.IsProductIdDropDown] = "True";
      }
      else
      {
        // else set ViewData as false.
        ViewData[ViewDataConstants.IsProductIdDropDown] = "False";
      }
      // CMP # 533: RAM A13 New Validations and New Charge Code [End]

      // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
      // To display TaxSubType drop down when Billing category is Misc set ViewData["isTaxNameDropdown"] to True.
      if (invoice.BillingCategoryId == (int)BillingCategoryType.Misc)
      {
        ViewData["isTaxNameDropdown"] = "True";
      }
      else
      {
        ViewData["isTaxNameDropdown"] = "False";
      }
      // CMP #534: Tax Issues in MISC and UATP Invoices. [End]

      return View(lineItemDetailModel);
    }

    /// <summary>
    /// Assign default values to LineItemDetail
    /// </summary>
    /// <param name="lineItem">The line item.</param>
    /// <param name="considerLineItemDate">if set to true </param>
    /// <returns></returns>
    private LineItemDetail PrepopulateLineItemDetail(LineItem lineItem, bool considerLineItemDate)
    {
      var detail = new LineItemDetail { ProductId = lineItem.ProductId, LineItem = lineItem, };

      var ucomCode = _invoiceManager.GetUomCode("EA");
      detail.UomCodeId = ucomCode.Id;
      detail.ScalingFactor = 1;
      // Line Item Detail number will be set to Max line item detail number + 1 if line item details count is > 0.
      var maxDetailNumber = _invoiceManager.GetMaxLineItemDetailNumber(lineItem.Id);
      detail.DetailNumber = maxDetailNumber + 1; // increment for new line item details.

      if (maxDetailNumber == 0 || considerLineItemDate)
      {
        detail.StartDate = lineItem.StartDate;
        detail.EndDate = lineItem.EndDate;
      }
      else
      {
        var lastLineItemDetail = _invoiceManager.GetLineItemDetailHeaderInformation(lineItem.Id, maxDetailNumber);
        detail.StartDate = lastLineItemDetail.StartDate;
        detail.EndDate = lastLineItemDetail.EndDate;

        // If Line Item added form Line item detail and start date should be populated from the last record, hence we need to set view data with same value.
        if (detail.StartDate.HasValue)
        {
          ViewData[ViewDataConstants.RetainLineItemDetailStartDate] = detail.StartDate.Value.Day;
        }
        else
        {
          ViewData[ViewDataConstants.RetainLineItemDetailStartDate] = -1;
        }
      }

      return detail;
    }

    /// <summary>
    /// Get Invoice, LineItem details and Dynamic fields on error exception and return ViewModel for LineItemDetail page
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetail"></param>
    /// <param name="requiredFields"></param>
    /// <returns></returns>
    public LineItemDetailViewModel GetLIneItemDetailModelOnException(string invoiceId, string lineItemId, LineItemDetail lineItemDetail)
    {
      var invoice = _invoiceManager.GetInvoiceDetail(invoiceId);
      var lineItem = _invoiceManager.GetLineItemInformation(lineItemId);
      lineItem.Invoice = invoice;
      lineItemDetail.LineItem = lineItem;
      int? chargeCodeTypeId = null;
      if (lineItem.ChargeCodeTypeId != 0)
      {
        chargeCodeTypeId = lineItem.ChargeCodeTypeId;
      }
      //CMP #636: Standard Update Mobilization.
      //Added billing category id for getting field meta data.
      var requiredFields = _invoiceManager.GetFieldMetadata(lineItem.ChargeCodeId, chargeCodeTypeId, lineItemDetail.Id,(int)BillingCategory);

      //// Get optional group metadata.
      //foreach (var fieldValue in lineItemDetail.FieldValues)
      //{
      //  var fieldMetadata = _invoiceManager.GetFieldMetadataForGroup(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, fieldValue.FieldMetaDataId, true);
      //  if (fieldMetadata != null && fieldMetadata.RequiredType == RequiredType.Optional && requiredFields.Contains(fieldMetadata) == false)
      //    requiredFields.Add(fieldMetadata);
      //}

      var lineItemDetailModel = new LineItemDetailViewModel { LineItemDetail = lineItemDetail, RequiredFields = requiredFields };
      return lineItemDetailModel;
    }

    public LineItemDetailViewModel GetLIneItemDetailModelOnCreateException(string invoiceId, string lineItemId, LineItemDetail lineItemDetail, IList<FieldMetaData> requiredFields)
    {
      var invoice = _invoiceManager.GetInvoiceDetail(invoiceId);
      var lineItem = _invoiceManager.GetLineItemInformation(lineItemId);
      lineItem.Invoice = invoice;
      lineItemDetail.LineItem = lineItem;
      int? chargeCodeTypeId = null;
      if (lineItem.ChargeCodeTypeId != 0)
      {
        chargeCodeTypeId = lineItem.ChargeCodeTypeId;
      }
      //var requiredFields = _invoiceManager.GetFieldMetadata(lineItem.ChargeCodeId, chargeCodeTypeId, lineItemDetail.Id);
      var lineItemDetailModel = new LineItemDetailViewModel { LineItemDetail = lineItemDetail, RequiredFields = requiredFields };
      return lineItemDetailModel;
    }

    /// <summary>
    /// Create LineItemDetail
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetail"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual ActionResult LineItemDetailCreate(string invoiceId, string lineItemId, LineItemDetail lineItemDetail)
    {
      SetViewDataPageMode(PageMode.Create);
      //TODO: Add code to save lineItemDetail and redirect to edit mode
      var lineItem = _invoiceManager.GetLineItemInformation(lineItemId);
      IList<FieldMetaData> requiredFields = new List<FieldMetaData>();
      try
      {
        lineItemDetail.LineItemId = lineItemId.ToGuid();
        //SCP239761 - Issue with ISXML - JAN P2 (20140102)
        IsLineItemDetailPopulated(lineItemDetail);

        // Save FieldValues as it is received from model binder.
        //var dynamicFieldValues = _invoiceManager.SetFieldValueForLineItemDetail(lineItemDetail.FieldValues, lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, new Guid());
        //lineItemDetail.FieldValues.Clear();
        //lineItemDetail.FieldValues.AddRange(dynamicFieldValues);

        lineItemDetail.LineItemId = lineItemId.ToGuid();
        lineItemDetail.LastUpdatedBy = SessionUtil.UserId;

        // Get required group metadata.
        //CMP #636: Standard Update Mobilization.
        //Added billing category id for getting field meta data.
        requiredFields = _invoiceManager.GetFieldMetadata(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, lineItemDetail.Id, (int)BillingCategory);

        // Get optional group metadata.
        foreach (var fieldValue in lineItemDetail.FieldValues)
        {
          var fieldMetadata = _invoiceManager.GetFieldMetadataForGroup(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, fieldValue.FieldMetaDataId, true);
          if (fieldMetadata != null && fieldMetadata.RequiredType == RequiredType.Optional)
            requiredFields.Add(fieldMetadata);
        }

        _invoiceManager.AddLineItemDetail(lineItemDetail, requiredFields);
        //TODO: Change error message
        ShowSuccessMessage(Messages.LineItemDetailCreateSuccessful);

        // If line item detail is not expected, show a warning message that Quantity, Unit Price, Scaling Factor and UOM Code at the Line Item Level will be overwritten.
        if(!_invoiceManager.IsFieldMetaDataExists(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, (int)BillingCategory))
        {
          ShowErrorMessage(Messages.LineItemDetailsOverwrittenWarning, true);
        }
        // if start date has some value, then it should be retained in next 
        // line item detail. as per 'Save and Add' requirement.
        TempData[ViewDataConstants.RetainLineItemDetailStartDate] = lineItemDetail.StartDate.HasValue ? lineItemDetail.StartDate.Value.Day : -1;

        return RedirectToAction("LineItemDetailCreate", new { lineItemId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessageForServerValidation(businessException.ErrorCode);
        ViewData[ViewDataConstants.IsExceptionOccurred] = true;
      }
      finally
      {
        SetViewDataPageMode(PageMode.Create);
      }

      var lineItemDetailModel = GetLIneItemDetailModelOnCreateException(invoiceId, lineItemId, lineItemDetail, requiredFields);
      return View(lineItemDetailModel);
    }

    /// <summary>
    /// Add LineItemDetail and duplicate record
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetail"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual ActionResult LineItemDetailDuplicate(string invoiceId, string lineItemId, LineItemDetail lineItemDetail)
    {
      SetViewDataPageMode(PageMode.Clone);
      //TODO: Add code to save lineItemDetail and redirect to edit mode
      IList<FieldMetaData> requiredFields = new List<FieldMetaData>();
      try
      {
        //SCP239761 - Issue with ISXML - JAN P2 (20140102)
        IsLineItemDetailPopulated(lineItemDetail);

        var lineItem = _invoiceManager.GetLineItemInformation(lineItemId);
        // Save FieldValues as it is received from model binder.
        //var dynamicFieldValues = _invoiceManager.SetFieldValueForLineItemDetail(lineItemDetail.FieldValues, lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, new Guid());
        //lineItemDetail.FieldValues.Clear();
        //lineItemDetail.FieldValues.AddRange(dynamicFieldValues);

        lineItemDetail.LineItemId = lineItemId.ToGuid();
        lineItemDetail.LastUpdatedBy = SessionUtil.UserId;
        
        //CMP #636: Standard Update Mobilization.
        //Added billing category id for getting field meta data.
        // Get required group metadata.
        requiredFields = _invoiceManager.GetFieldMetadata(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, lineItemDetail.Id, (int)BillingCategory);

        // Get optional group metadata.
        foreach (var fieldValue in lineItemDetail.FieldValues)
        {
          var fieldMetadata = _invoiceManager.GetFieldMetadataForGroup(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, fieldValue.FieldMetaDataId, true);
          if (fieldMetadata != null && fieldMetadata.RequiredType == RequiredType.Optional)
            requiredFields.Add(fieldMetadata);
        }

        //// Get optional group metadata.
        //var optionalGroups = _invoiceManager.GetOptionalGroupDetails(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId);

        //foreach (var dynamicGroupDetail in optionalGroups)
        //{
        //  requiredFields.Add(_invoiceManager.GetFieldMetadataForGroup(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, dynamicGroupDetail.FieldMetadataId, true));
        //}

        _invoiceManager.AddLineItemDetail(lineItemDetail, requiredFields);

        //TODO: Change error message
        ShowSuccessMessage(Messages.LineItemDetailCreateSuccessful);
        TempData[ViewDataConstants.RetainLineItemDetailStartDate] = lineItemDetail.StartDate.HasValue ? lineItemDetail.StartDate.Value.Day : -1;

        // If line item detail is not expected, show a warning message that Quantity, Unit Price, Scaling Factor and UOM Code at the Line Item Level will be overwritten.
        if (!_invoiceManager.IsFieldMetaDataExists(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, (int)BillingCategory))
        {
          ShowErrorMessage(Messages.LineItemDetailsOverwrittenWarning);
        }

        // CMP # 533: RAM A13 New Validations and New Charge Code [Start]
        // To display Product Id field as dropdown or as textbox
        if (lineItem.Invoice.BillingCategoryId == (int)BillingCategoryType.Misc &&
            lineItem.Invoice.ChargeCategoryDisplayName.ToLower().Equals(ControlIdConstants.ServiceProvider) && lineItem.DisplayChargeCode.ToLower().Equals(ControlIdConstants.Gds))
        {
          // For the Billing Category = Misc and ChargeCategory = Service Provider and Charge Code = GDS set ViewData as true.
          ViewData[ViewDataConstants.IsProductIdDropDown] = "True";
        }
        else
        {
          // else set ViewData as false.
          ViewData[ViewDataConstants.IsProductIdDropDown] = "False";
        }
        // CMP # 533: RAM A13 New Validations and New Charge Code [End]
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessageForServerValidation(businessException.ErrorCode);
        var lineItemDetailModelExc = GetLIneItemDetailModelOnCreateException(invoiceId, lineItemId, lineItemDetail, requiredFields);
        ViewData[ViewDataConstants.IsExceptionOccurred] = true;
        return View("LineItemDetailCreate", lineItemDetailModelExc);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Clone);
      }
      ModelState.Clear();
      var lineItemDetailModel = GetLIneItemDetailModelOnException(invoiceId, lineItemId, lineItemDetail);
      lineItemDetailModel.LineItemDetail = CloneLineItemDetail(lineItemDetailModel.LineItemDetail);
      return View("LineItemDetailCreate", lineItemDetailModel);
    }

    /// <summary>
    /// Added function to clone object
    /// </summary>
    /// <param name="detail"></param>
    /// <returns></returns>
    private static LineItemDetail CloneLineItemDetail(LineItemDetail detail)
    {
      //TODO: move this method in class
      var clonedDetail = new LineItemDetail
      {
        Description = detail.Description,
        DetailNumber = detail.LineItem.LineItemDetails.Count > 0 ? detail.LineItem.LineItemDetails.Max(lineItemDetail => lineItemDetail.DetailNumber) + 1 : 1,
        StartDate = detail.StartDate,
        EndDate = detail.EndDate,
        LineItemId = detail.LineItemId,
        LineItem = detail.LineItem,
        MinimumQuantityFlag = detail.MinimumQuantityFlag,
        ProductId = detail.ProductId,
        Quantity = detail.Quantity,
        ScalingFactor = detail.ScalingFactor,
        TotalAddOnChargeAmount = detail.TotalAddOnChargeAmount,
        TotalNetAmount = detail.TotalNetAmount,
        TotalTaxAmount = detail.TotalTaxAmount,
        TotalVatAmount = detail.TotalVatAmount,
        ChargeAmount = detail.ChargeAmount,
        UnitPrice = detail.UnitPrice,
        UomCode = detail.UomCode,
        UomCodeId = detail.UomCodeId
      };

      foreach (var tax in detail.TaxBreakdown)
      {
        clonedDetail.TaxBreakdown.Add(new LineItemDetailTax()
                                        {
                                          Amount = tax.Amount,
                                          Percentage = tax.Percentage,
                                          CalculatedAmount = tax.CalculatedAmount,
                                          CategoryCode = tax.CategoryCode,
                                          Description = tax.Description,
                                          Type = tax.Type,
                                          SubType = tax.SubType
                                        });
      }

      foreach (var addCharge in detail.AddOnCharges)
      {
        clonedDetail.AddOnCharges.Add(new LineItemDetailAddOnCharge() { Name = addCharge.Name, ChargeableAmount = addCharge.ChargeableAmount, Percentage = addCharge.Percentage, Amount = addCharge.Amount });
      }

      return clonedDetail;
    }

    /// <summary>
    /// Method to save LineItemDetail and return to Edit LineItem page
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetail"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual ActionResult LineItemDetailReturn(string invoiceId, string lineItemId, LineItemDetail lineItemDetail)
    {
      SetViewDataPageMode(PageMode.Create);
      //TODO: Add code to save lineItemDetail and redirect to edit mode
      IList<FieldMetaData> requiredFields = new List<FieldMetaData>();
      try
      {
        //SCP239761 - Issue with ISXML - JAN P2 (20140102)
        IsLineItemDetailPopulated(lineItemDetail);

        lineItemDetail.LineItemId = lineItemId.ToGuid();
        lineItemDetail.LastUpdatedBy = SessionUtil.UserId;
        var lineItem = _invoiceManager.GetLineItemInformation(lineItemId);
        //var dynamicFieldValues = _invoiceManager.SetFieldValueForLineItemDetail(lineItemDetail.FieldValues, lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, new Guid());
        //lineItemDetail.FieldValues.Clear();
        //lineItemDetail.FieldValues.AddRange(dynamicFieldValues);
        
        //CMP #636: Standard Update Mobilization.
        //Added billing category id for getting field meta data.
        // Get required group metadata.
        requiredFields = _invoiceManager.GetFieldMetadata(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, lineItemDetail.Id, (int)BillingCategory);
        
        // Get optional group metadata.
        foreach (var fieldValue in lineItemDetail.FieldValues)
        {
          var fieldMetadata = _invoiceManager.GetFieldMetadataForGroup(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, fieldValue.FieldMetaDataId, true);
          if(fieldMetadata != null && fieldMetadata.RequiredType == RequiredType.Optional)
            requiredFields.Add(fieldMetadata);
        }

        //// Get optional group metadata.
        //var optionalGroups = _invoiceManager.GetOptionalGroupDetails(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId);

        //foreach (var dynamicGroupDetail in optionalGroups)
        //{
        //  requiredFields.Add(_invoiceManager.GetFieldMetadataForGroup(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, dynamicGroupDetail.FieldMetadataId, true));
        //}

        _invoiceManager.AddLineItemDetail(lineItemDetail, requiredFields);
        //TODO: Change error message
        ShowSuccessMessage(Messages.LineItemDetailCreateSuccessful);

        // If line item detail is not expected, show a warning message that Quantity, Unit Price, Scaling Factor and UOM Code at the Line Item Level will be overwritten.
        if (!_invoiceManager.IsFieldMetaDataExists(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, (int)BillingCategory))
        {
          ShowErrorMessage(Messages.LineItemDetailsOverwrittenWarning, true);
        }

        return RedirectToAction("LineItemEdit", new { lineItemId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessageForServerValidation(businessException.ErrorCode);
        ViewData[ViewDataConstants.IsExceptionOccurred] = true;
        //lineItemDetail.LineItem = _invoiceManager.GetLineItemHeaderInformation(lineItemId);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Create);
      }
      var lineItemDetailModel = GetLIneItemDetailModelOnCreateException(invoiceId, lineItemId, lineItemDetail, requiredFields);
      return View("LineItemDetailCreate", lineItemDetailModel);
    }

    /// <summary>
    /// View for LineItemDetail
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetailId"></param>
    /// <returns></returns>
    [HttpGet]
    public virtual ActionResult LineItemDetailView(string invoiceId, string lineItemId, string lineItemDetailId)
    {
      var lineItemDetail = _invoiceManager.GetLineItemDetailInformation(lineItemDetailId);
      var lineItemDetailModel = GetLIneItemDetailModelOnException(invoiceId, lineItemId, lineItemDetail);
      
      // CMP # 533: RAM A13 New Validations and New Charge Code [Start]
      // To display Product Id field as dropdown or as textbox
      var lineItem = _invoiceManager.GetLineItemInformation(lineItemId);
      if (lineItem.Invoice.BillingCategoryId == (int)BillingCategoryType.Misc && 
        lineItem.Invoice.ChargeCategoryDisplayName.ToLower().Equals(ControlIdConstants.ServiceProvider) && lineItem.DisplayChargeCode.ToLower().Equals(ControlIdConstants.Gds))
      {
        // For the Billing Category = Misc and ChargeCategory = Service Provider and Charge Code = GDS set ViewData as true.
        ViewData[ViewDataConstants.IsProductIdDropDown] = "True";
      }
      else
      {
        // else set ViewData as false.
        ViewData[ViewDataConstants.IsProductIdDropDown] = "False";
      }
      // CMP # 533: RAM A13 New Validations and New Charge Code [End]

      return View("LineItemDetailEdit", lineItemDetailModel);
    }

    /// <summary>
    /// Edit LineItemDetail Get method
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetailId"></param>
    /// <returns></returns>
    [HttpGet]
    public virtual ActionResult LineItemDetailEdit(string invoiceId, string lineItemId, string lineItemDetailId)
    {
      var lineItemDetail = _invoiceManager.GetLineItemDetailInformation(lineItemDetailId);
      var lineItem = _invoiceManager.GetLineItemInformation(lineItemId);
      lineItem.Invoice = InvoiceHeader;
      
      lineItemDetail.LineItem = lineItem;

      
      int? chargeCodeTypeId = null;
      if (lineItem.ChargeCodeTypeId != 0)
      {
        chargeCodeTypeId = lineItem.ChargeCodeTypeId;
      }

      //CMP #636: Standard Update Mobilization.
      //Added billing category id for getting field meta data.
      var requiredFields = _invoiceManager.GetFieldMetadata(lineItem.ChargeCodeId, chargeCodeTypeId, lineItemDetailId.ToGuid(), (int)BillingCategory);

      var lineItemDetailModel = new LineItemDetailViewModel { LineItemDetail = lineItemDetail, RequiredFields = requiredFields };

      // CMP # 533: RAM A13 New Validations and New Charge Code [Start]
      // To display Product Id field as dropdown or as textbox
      if (lineItem.Invoice.BillingCategoryId == (int)BillingCategoryType.Misc && 
        lineItem.Invoice.ChargeCategoryDisplayName.ToLower().Equals(ControlIdConstants.ServiceProvider) && lineItem.DisplayChargeCode.ToLower().Equals(ControlIdConstants.Gds))
      {
        // For the Billing Category = Misc and ChargeCategory = Service Provider and Charge Code = GDS set ViewData as true.
        ViewData[ViewDataConstants.IsProductIdDropDown] = "True";
      }
      else
      {
        // else set ViewData as false.
        ViewData[ViewDataConstants.IsProductIdDropDown] = "False";
      }
      // CMP # 533: RAM A13 New Validations and New Charge Code [End]

      // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
      // To display TaxSubType drop down when Billing category is Misc set ViewData["isTaxNameDropdown"] to True.
      if (lineItem.Invoice.BillingCategoryId != null && lineItem.Invoice.BillingCategoryId == (int)BillingCategoryType.Misc)
      {
        ViewData["isTaxNameDropdown"] = "True";
      }
      else
      {
        ViewData["isTaxNameDropdown"] = "False";
      }
      // CMP #534: Tax Issues in MISC and UATP Invoices. [End]

      return View(lineItemDetailModel);
    }

    /// <summary>
    /// Edit LineItemDetail
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetailId"></param>
    /// <param name="lineItemDetail"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual ActionResult LineItemDetailEdit(string invoiceId, string lineItemId, string lineItemDetailId, LineItemDetail lineItemDetail)
    {
      IList<FieldMetaData> requiredFields = new List<FieldMetaData>();
      try
      {
        lineItemDetail.Id = lineItemDetailId.ToGuid();
        //SCP239761 - Issue with ISXML - JAN P2 (20140102)
        IsLineItemDetailPopulated(lineItemDetail);

        var lineItem = _invoiceManager.GetLineItemInformation(lineItemId);
        // Save FieldValues as it is received from model binder.
        //var dynamicFieldValues = _invoiceManager.SetFieldValueForLineItemDetail(lineItemDetail.FieldValues, lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, lineItemDetailId.ToGuid());
        //lineItemDetail.FieldValues.Clear();
        //lineItemDetail.FieldValues.AddRange(dynamicFieldValues);
        
        lineItemDetail.LastUpdatedBy = SessionUtil.UserId;
        // Set Line Item Detail Id for all the field values in LineItemDetail.
        UpdateLineItemDetailIdInFieldValues(lineItemDetail);

        // Get required group metadata.
        //requiredFields = _invoiceManager.GetFieldMetadata(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, lineItemDetail.Id);

        //CMP #636: Standard Update Mobilization.
        //Added billing category id for getting field meta data.
        requiredFields = _invoiceManager.GetFieldMetadata(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, lineItemDetail.Id, (int)BillingCategory);

        // Get optional group metadata.
        foreach (var fieldValue in lineItemDetail.FieldValues)
        {
          var fieldMetadata = _invoiceManager.GetFieldMetadataForGroup(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, fieldValue.FieldMetaDataId, true);
          if (fieldMetadata != null && fieldMetadata.RequiredType == RequiredType.Optional && requiredFields.Contains(fieldMetadata) == false)
            requiredFields.Add(fieldMetadata);
        }

        _invoiceManager.UpdateLineItemDetail(lineItemDetail, requiredFields);
        ShowSuccessMessage(Messages.LineItemDetailUpdateSuccessful);

        TempData[ViewDataConstants.RetainLineItemDetailStartDate] = lineItemDetail.StartDate.HasValue ? lineItemDetail.StartDate.Value.Day : -1;

        // If line item detail is not expected, show a warning message that Quantity, Unit Price, Scaling Factor and UOM Code at the Line Item Level will be overwritten.
        if (!_invoiceManager.IsFieldMetaDataExists(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, (int)BillingCategory))
        {
          ShowErrorMessage(Messages.LineItemDetailsOverwrittenWarning, true);
        }

        return RedirectToAction("LineItemDetailCreate", new { lineItemId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessageForServerValidation(businessException.ErrorCode);
        ViewData[ViewDataConstants.IsExceptionOccurred] = true;
        lineItemDetail.NavigationDetails = _invoiceManager.GetNavigationDetails(lineItemDetailId, lineItemId);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      var lineItemDetailModel = GetLIneItemDetailModelOnCreateException(invoiceId, lineItemId, lineItemDetail, requiredFields);
      return View(lineItemDetailModel);
    }

    private static void UpdateLineItemDetailIdInFieldValues(LineItemDetail lineItemDetail)
    {
      foreach (FieldValue fieldValue in lineItemDetail.FieldValues)
      {
        fieldValue.LineItemDetailId = lineItemDetail.Id;
        foreach (FieldValue attributeValue in fieldValue.AttributeValues)
        {
          attributeValue.LineItemDetailId = lineItemDetail.Id;

          // Set line item detail ids upto 3 level.
          attributeValue.AttributeValues.ForEach(attrValue => { attrValue.LineItemDetailId = lineItemDetail.Id; });
        }
      }
    }

    /// <summary>
    /// Update LineItemDetail and clone
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetailId"></param>
    /// <param name="lineItemDetail"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual ActionResult LineItemDetailClone(string invoiceId, string lineItemId, string lineItemDetailId, LineItemDetail lineItemDetail)
    {
      IList<FieldMetaData> requiredFields = new List<FieldMetaData>();
      try
      {
        lineItemDetail.Id = lineItemDetailId.ToGuid();
        //SCP239761 - Issue with ISXML - JAN P2 (20140102)
        IsLineItemDetailPopulated(lineItemDetail);

        var lineItem = _invoiceManager.GetLineItemInformation(lineItemId);

        // Save FieldValues as it is received from model binder.
        //var dynamicFieldValues = _invoiceManager.SetFieldValueForLineItemDetail(lineItemDetail.FieldValues, lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, lineItemDetailId.ToGuid());
        //lineItemDetail.FieldValues.Clear();
        //lineItemDetail.FieldValues.AddRange(dynamicFieldValues);

        // Set Line Item Detail Id for all the field values in LineItemDetail.
        UpdateLineItemDetailIdInFieldValues(lineItemDetail);

        foreach (var tax in lineItemDetail.TaxBreakdown)
        {
          tax.ParentId = lineItemDetail.Id;
        }
        foreach (var addCharge in lineItemDetail.AddOnCharges)
        {
          addCharge.ParentId = lineItemDetail.Id;
        }
        //CMP #636: Standard Update Mobilization.
        //Added billing category id for getting field meta data.
        requiredFields = _invoiceManager.GetFieldMetadata(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, lineItemDetail.Id, (int)BillingCategory);

        // Get optional group metadata.
        foreach (var fieldValue in lineItemDetail.FieldValues)
        {
          var fieldMetadata = _invoiceManager.GetFieldMetadataForGroup(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, fieldValue.FieldMetaDataId, true);
          if (fieldMetadata != null && fieldMetadata.RequiredType == RequiredType.Optional && requiredFields.Contains(fieldMetadata) == false)
            requiredFields.Add(fieldMetadata);
        }

        _invoiceManager.UpdateLineItemDetail(lineItemDetail, requiredFields);
        ShowSuccessMessage(Messages.LineItemDetailUpdateSuccessful);

        // If line item detail is not expected, show a warning message that Quantity, Unit Price, Scaling Factor and UOM Code at the Line Item Level will be overwritten.
        if (!_invoiceManager.IsFieldMetaDataExists(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, (int)BillingCategory))
        {
          ShowErrorMessage(Messages.LineItemDetailsOverwrittenWarning);
        }

        TempData[ViewDataConstants.RetainLineItemDetailStartDate] = lineItemDetail.StartDate.HasValue ? lineItemDetail.StartDate.Value.Day : -1;

        // CMP # 533: RAM A13 New Validations and New Charge Code [Start]
        // To display Product Id field as dropdown or as textbox
        if (lineItem.Invoice.BillingCategoryId == (int)BillingCategoryType.Misc &&
            lineItem.Invoice.ChargeCategoryDisplayName.ToLower().Equals(ControlIdConstants.ServiceProvider) && lineItem.DisplayChargeCode.ToLower().Equals(ControlIdConstants.Gds))
        {
          // For the Billing Category = Misc and ChargeCategory = Service Provider and Charge Code = GDS set ViewData as true.
          ViewData[ViewDataConstants.IsProductIdDropDown] = "True";
        }
        else
        {
          // else set ViewData as false.
          ViewData[ViewDataConstants.IsProductIdDropDown] = "False";
        }
        // CMP # 533: RAM A13 New Validations and New Charge Code [End]

        // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
        // To display TaxSubType drop down when Billing category is Misc set ViewData["isTaxNameDropdown"] to True.
        if (lineItem.Invoice.BillingCategoryId != null && lineItem.Invoice.BillingCategoryId == (int)BillingCategoryType.Misc)
        {
          ViewData["isTaxNameDropdown"] = "True";
        }
        else
        {
          ViewData["isTaxNameDropdown"] = "False";
        }
        // CMP #534: Tax Issues in MISC and UATP Invoices. [End]
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessageForServerValidation(businessException.ErrorCode);
        ViewData[ViewDataConstants.IsExceptionOccurred] = true;
        var lineItemDetailModelExc = GetLIneItemDetailModelOnCreateException(invoiceId, lineItemId, lineItemDetail, requiredFields);
        lineItemDetailModelExc.LineItemDetail.NavigationDetails = _invoiceManager.GetNavigationDetails(lineItemDetailId, lineItemId);
        return View("LineItemDetailEdit", lineItemDetailModelExc);
      }
      finally
      {
        //SetViewDataPageMode(PageMode.Edit);
        SetViewDataPageMode(PageMode.Clone);
      }

      ModelState.Clear();
      var lineItemDetailModel = GetLIneItemDetailModelOnException(invoiceId, lineItemId, lineItemDetail);
      lineItemDetailModel.LineItemDetail = CloneLineItemDetail(lineItemDetailModel.LineItemDetail);
      return View("LineItemDetailCreate", lineItemDetailModel);
    }

    /// <summary>
    /// Update lineiTemDetail and redirect to Edit LineItem page
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetailId"></param>
    /// <param name="lineItemDetail"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual ActionResult LineItemDetailEditAndReturn(string invoiceId, string lineItemId, string lineItemDetailId, LineItemDetail lineItemDetail)
    {
      IList<FieldMetaData> requiredFields = new List<FieldMetaData>();
      try
      {
        lineItemDetail.Id = lineItemDetailId.ToGuid();
        //SCP239761 - Issue with ISXML - JAN P2 (20140102)
        IsLineItemDetailPopulated(lineItemDetail);

        lineItemDetail.LastUpdatedBy = SessionUtil.UserId;
        //Set field value for dynamic fields
        var lineItem = _invoiceManager.GetLineItemInformation(lineItemId);

        // Save FieldValues as it is received from model binder.
        //var dynamicFieldValues = _invoiceManager.SetFieldValueForLineItemDetail(lineItemDetail.FieldValues, lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, lineItemDetailId.ToGuid());
        //lineItemDetail.FieldValues.Clear();
        //lineItemDetail.FieldValues.AddRange(dynamicFieldValues);

        // Set Line Item Detail Id for all the field values in LineItemDetail.
        UpdateLineItemDetailIdInFieldValues(lineItemDetail);

        //CMP #636: Standard Update Mobilization.
        //Added billing category id for getting field meta data.
        requiredFields = _invoiceManager.GetFieldMetadata(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, lineItemDetail.Id, (int)BillingCategory);

        // Get optional group metadata.
        foreach (var fieldValue in lineItemDetail.FieldValues)
        {
          var fieldMetadata = _invoiceManager.GetFieldMetadataForGroup(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, fieldValue.FieldMetaDataId, true);
          if (fieldMetadata != null && fieldMetadata.RequiredType == RequiredType.Optional && requiredFields.Contains(fieldMetadata) == false)
            requiredFields.Add(fieldMetadata);
        }

        _invoiceManager.UpdateLineItemDetail(lineItemDetail, requiredFields);
        ShowSuccessMessage(Messages.LineItemDetailUpdateSuccessful);

        // If line item detail is not expected, show a warning message that Quantity, Unit Price, Scaling Factor and UOM Code at the Line Item Level will be overwritten.
        if (!_invoiceManager.IsFieldMetaDataExists(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, (int)BillingCategory))
        {
          ShowErrorMessage(Messages.LineItemDetailsOverwrittenWarning, true);
        }

        return RedirectToAction("LineItemEdit", new { lineItemId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessageForServerValidation(businessException.ErrorCode);
        ViewData[ViewDataConstants.IsExceptionOccurred] = true;
        lineItemDetail.NavigationDetails = _invoiceManager.GetNavigationDetails(lineItemDetailId, lineItemId);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      var lineItemDetailModel = GetLIneItemDetailModelOnCreateException(invoiceId, lineItemId, lineItemDetail, requiredFields);
      return View("LineItemDetailEdit", lineItemDetailModel);
    }

    //
    // GET: /Misc/LineItem/Create
    [HttpGet]
    public virtual ActionResult LineItemEdit(string lineItemId, string invoiceId)
    {
      var lineItem = GetLineItem(lineItemId, invoiceId);

      //CMP #636: Standard Update Mobilization
      if (lineItem.ChargeCode != null)
        ViewData[ViewDataConstants.IsChargeCodeTypeExists] = lineItem.ChargeCode.IsChargeCodeTypeRequired;

      ViewData[ViewDataConstants.TransactionExists] = _invoiceManager.IsLineItemDetailExists(lineItemId).ToString().ToLower();
      ViewData[ViewDataConstants.FieldMetaDataExists] = GetFieldMetaDataExistsFlag(lineItem).ToString().ToLower();

      // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
      // To display TaxSubType drop down when Billing category is Misc set ViewData["isTaxNameDropdown"] to True.
      if (lineItem.Invoice.BillingCategoryId != null && lineItem.Invoice.BillingCategoryId == (int)BillingCategoryType.Misc)
      {
        ViewData["isTaxNameDropdown"] = "True";
      }
      else
      {
        ViewData["isTaxNameDropdown"] = "False";
      }
      // CMP #534: Tax Issues in MISC and UATP Invoices. [End]

      return View(lineItem);
    }

    [HttpGet]
    public virtual ActionResult LineItemView(string lineItemId, string invoiceId)
    {
      var lineItem = GetLineItem(lineItemId, invoiceId);
      TempData["Reject"] = TempData["Reject"];

      //CMP #636: Standard Update Mobilization
      if (lineItem.ChargeCode != null)
        ViewData[ViewDataConstants.IsChargeCodeTypeExists] = lineItem.ChargeCode.IsChargeCodeTypeRequired;

      ViewData[ViewDataConstants.TransactionExists] = _invoiceManager.IsLineItemDetailExists(lineItemId).ToString().ToLower();
      ViewData[ViewDataConstants.FieldMetaDataExists] = GetFieldMetaDataExistsFlag(lineItem).ToString().ToLower();
      return View("LineItemEdit", lineItem);
    }

    private LineItem GetLineItem(string lineItemId, string invoiceId)
    {
      var lineItem = _invoiceManager.GetLineItemInformation(lineItemId);
      lineItem.Invoice = _invoiceManager.GetInvoiceHeader(invoiceId);
      lineItem.LastUpdatedBy = SessionUtil.UserId;
      GetLineItemDetailGrid(lineItemId);

      return lineItem;
    }

    private void GetLineItemDetailGrid(string lineItemId)
    {
      var lineItemDetailGrid = new LineItemDetailGrid(ControlIdConstants.LineItemDetailGridId, Url.Action("GetLineItemDetails", new { lineItemId }));
      ViewData[ViewDataConstants.LineItemDetailGrid] = lineItemDetailGrid.Instance;
    }

    [HttpPost]
    public virtual ActionResult LineItemEdit(string invoiceId, string lineItemId, LineItem lineItem)
    {
      lineItem.Invoice = _invoiceManager.GetInvoiceDetail(invoiceId);

      try
      {
        lineItem.Id = lineItemId.ToGuid();
        lineItem.LastUpdatedBy = SessionUtil.UserId;
        _invoiceManager.UpdateLineItem(lineItem);
        ShowSuccessMessage(Messages.LineItemUpdateSuccessful);

        // User should not be redirected to Create Line Item detail page even if line item details are expected.
        // Redirection is required only in case of Create Line Item. 
        //if ((lineItem.Invoice.InvoiceType != InvoiceType.RejectionInvoice
        //  || lineItem.Invoice.InvoiceType != InvoiceType.CreditNote || lineItem.Invoice.InvoiceType != InvoiceType.CorrespondenceInvoice)
        //  && _invoiceManager.IsFieldMetaDataExists(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId))
        //{
        //  // If line item detail is expected redirect to Line Item Detail Create screen.
        //  return RedirectToAction("LineItemDetailCreate",
        //                          new
        //                          {
        //                            lineItemId = lineItemObject.Id,
        //                            invoiceId = lineItemObject.InvoiceId
        //                          });
        //}

        return RedirectToAction("LineItemEdit", new { lineItemId, invoiceId = lineItem.InvoiceId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        ViewData[ViewDataConstants.TransactionExists] = _invoiceManager.IsLineItemDetailExists(lineItemId).ToString().ToLower();
        ViewData[ViewDataConstants.FieldMetaDataExists] = GetFieldMetaDataExistsFlag(lineItem).ToString().ToLower();

        //CMP #636: Standard Update Mobilization
        if (lineItem.ChargeCode != null)
          ViewData[ViewDataConstants.IsChargeCodeTypeExists] = lineItem.ChargeCode.IsChargeCodeTypeRequired;

        GetLineItemDetailGrid(lineItem.Id.ToString());
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      return View(lineItem);
    }

    /// <summary>
    /// Upload Rejection Memo Coupon Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public virtual JsonResult InvoiceAttachmentUpload(string invoiceId)
    {
      return UploadInvoiceAttachment(invoiceId, true);
    }

    /// <summary>
    /// Created function to update Attachment indicator original flag only in case of Attachment upload. 
    /// In case of Manage supporting doc, this flag is not be changed
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="isUpdateAttachmentIndOrig"></param>
    /// <returns></returns>
    protected JsonResult UploadInvoiceAttachment(string invoiceId, bool isUpdateAttachmentIndOrig)
    {
      var files = string.Empty;
      var attachments = new List<MiscUatpAttachment>();
      // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015
      var isUploadSuccess = false;
      string message;
      HttpPostedFileBase fileToSave;
      FileAttachmentHelper fileUploadHelper = null;
      try
      {
          Logger.Info("Started Execution For method UploadInvoiceAttachment for invoice ID" + invoiceId);
        var invoice = _invoiceManager.GetInvoiceDetail(invoiceId);
        Logger.Info("Fetched all invoice details successfully.");

        DateTime eventTime = CalendarManager.GetCalendarEventTime(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn,
                                          invoice.BillingYear, invoice.BillingMonth,
                                          invoice.BillingPeriod);

        
        if (DateTime.UtcNow > eventTime)
        {
            throw new ISBusinessException(Messages.SupportingDocDeadline);
        }

        foreach (string file in Request.Files)
        {
          isUploadSuccess = false;
          fileToSave = Request.Files[file];
          Logger.Info("Started saving the file" + fileToSave);
          if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
          {
             Logger.Info(fileToSave+"is null or empty file");
            continue;
          }

          fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };

          Logger.Info("checking the file is valid or not");
          if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
          {
              throw new ISBusinessException(Messages.InvalidFileName);
          }
          
          if (!fileUploadHelper.ValidateFileExtention(invoice.BilledMemberId, invoice.BillingCategory))
          {
            throw new ISBusinessException(Messages.InvalidFileExtension);
          }

          //Check if duplicate file exists
          var supportingDocumentManager = Ioc.Resolve<ISupportingDocumentManager>(typeof(ISupportingDocumentManager));
          Logger.Info("Checking for duplicate supporting document attachement");
          if (supportingDocumentManager.IsDuplicateFileName(fileUploadHelper.FileOriginalName, invoiceId, (int)SupportingDocRecordType.Misc))
          {
            throw new ISBusinessException(Messages.FileDuplicateError);
          }

          if (fileUploadHelper.SaveFile())
          {
            Logger.Info("File " + fileUploadHelper.FileOriginalName + " is saved successfully at " + fileUploadHelper.FileRelativePath + " folder");
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            var attachment = new MiscUatpAttachment
            {
              Id = fileUploadHelper.FileServerName,
              OriginalFileName = fileUploadHelper.FileOriginalName,
              // Convert file size to KB.
              FileSize = fileUploadHelper.FileToSave.ContentLength,
              LastUpdatedBy = SessionUtil.UserId,
              ServerId = fileUploadHelper.FileServerInfo.ServerId,
              //FileServer = fileUploadHelper.FileServerInfo,
              FileStatus = FileStatusType.Received,
              FilePath = fileUploadHelper.FileRelativePath,
              ParentId = invoiceId.ToGuid()
            };

            //SCP218213 - SRM: Admin Alert - Offline collection generation failure notification - SIS Production
            //In case the supp doc attachment  failed to copy its desired location then the attachement entry should not be  update in database.
            if (System.IO.File.Exists(fileUploadHelper.FileUploadPath))
            {
              attachment = _invoiceManager.AddInvoiceAttachment(attachment, invoice, isUpdateAttachmentIndOrig);
              isUploadSuccess = true;
              attachments.Add(attachment);
              Logger.Info("Attachment Entry is inserted successfully in database");
            }
            else
            {
              Logger.Info("Please attention : SaveFile() method not executed as expected and return True... Attachment file not found at source location");
              isUploadSuccess = false;
              throw new Exception("Exception occured while uploading the file. Please try again.");
            }
          }
        }
        message = string.Format(Messages.FileUploadSuccessful, files.TrimEnd(','));
        
      }
      catch (ISBusinessException ex)
      {
        message = string.Format(Messages.FileUploadBusinessException, ex.ErrorCode);
        Logger.Info("Business Exeption occured as :-" +ex);
        if (fileUploadHelper != null && isUploadSuccess == false)
        {
          fileUploadHelper.DeleteFile();
        }
      }
      catch (Exception e)
      {
        Logger.Info("Exeption :-" + e.Message);
        message = Messages.FileUploadUnexpectedError;
        if (fileUploadHelper != null && isUploadSuccess == false)
        {
          fileUploadHelper.DeleteFile();
        }
      }

      return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
    }

    /// <summary>
    /// Download Invoice attachment
    ///  </summary>
    /// <param name="invoiceId">Invoice id</param>
    /// <param name="lineItemId">lineItem Id</param>
    /// <returns></returns>
    [HttpGet]
    public virtual FileStreamResult InvoiceAttachmentDownload(string invoiceId, string lineItemId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _invoiceManager.GetInvoiceAttachmentDetail(lineItemId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [HttpPost]
    public virtual JsonResult DeleteAttachment(string id, bool isSupportingDoc = false)
    {
        //UIMessageDetail details;
        bool isDeleted;
        try
        {
            isDeleted = _invoiceManager.DeleteAttachment(id, isSupportingDoc);
            return  Json(isDeleted); 
        //    details = isDeleted
        //                   ? new UIMessageDetail { IsFailed = false, Message = Messages.DeleteSuccessful }
        //                   : new UIMessageDetail { IsFailed = true, Message = Messages.DeleteFailed };
        }
        catch (ISBusinessException ex)
        {
            isDeleted = false;
            //details = new UIMessageDetail
            //{
            //    IsFailed = true,
            //    Message = string.Format(Messages.DeleteException, GetDisplayMessage(ex.ErrorCode))
            //};
        }
        return Json(isDeleted);
    }

    /// <summary>
    /// Used for attachment link on Line Item page.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetailId">This is the attachment ID.</param>
    /// <returns></returns>
    [HttpGet]
    public virtual FileStreamResult LineItemAttachmentDownload(string invoiceId, string lineItemId, string lineItemDetailId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _invoiceManager.GetInvoiceAttachmentDetail(lineItemDetailId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [HttpPost]
    public virtual JsonResult GetRejectionInvoiceDetails(string billingMemberId, string billedMemberId, string rejectionInvoiceNumber, string settlementMethod, int? settlementMonth, int? settlementYear, int? settlementPeriod)
    {
      int settlementMethodId = Convert.ToInt32(settlementMethod);
      int billingMember = Convert.ToInt32(billingMemberId);
      int billedMember = Convert.ToInt32(billedMemberId);

      var rejectedInvoiceDetails = _invoiceManager.GetRejectedInvoiceDetails(rejectionInvoiceNumber, settlementMethodId, billedMember, billingMember, settlementMonth, settlementYear, settlementPeriod);

      // TODO: UI Model RejectionInvoiceDetails can be removed as business has same model.
      var rejectionInvoiceDetails = new RejectionInvoiceDetails
                                      {
                                        BillingMonth = rejectedInvoiceDetails.BillingPeriod.Month,
                                        BillingYear = rejectedInvoiceDetails.BillingPeriod.Year,
                                        BillingPeriod = rejectedInvoiceDetails.BillingPeriod.Period,
                                        RejectionStage = rejectedInvoiceDetails.RejectionStage,
                                        BilledInMonths = _referenceManager.GetRejectionTimeLimit(settlementMethodId, BillingCategory),
                                        DisableBillingCurrency = rejectedInvoiceDetails.DisableBillingCurrency,
                                        CurrentBilledIn = rejectedInvoiceDetails.CurrentBilledIn,
                                        CurrentBillingCurrencyId = Convert.ToInt32(rejectedInvoiceDetails.CurrentBillingCurrencyCode),
                                        ErrorMessage = rejectedInvoiceDetails.ErrorMessage,
                                        AlertMessage = rejectedInvoiceDetails.AlertMessage
                                      };
      //if(rejectionStage > 0 && settlementYear > 0 && settlementMonth > 0 && billingYear > 0 && billingMonth > 0)
      //{
      //  rejectionInvoiceDetails.IsRejectionOutSideTimeLimit = IsTransactionOutSideTimeLimit(rejectionStage, settlementYear, settlementMonth, settlementMethodId, billingYear, billingMonth);
      //}          

      return Json(rejectionInvoiceDetails);


      //int billedInMonths = _referenceManager.GetRejectionTimeLimit(settlementMethodId, BillingCategory);

      //return Json(billedInMonths);
    }

    [HttpPost]
    public virtual JsonResult IsRejectionOutsideTimeLimit(int rejectionStage, int settlementYear, int settlementMonth, int settlementMethodId, int billingYear, int billingMonth, int settlementPeriod)
    {
      return Json(IsTransactionOutSideTimeLimit(rejectionStage, settlementYear, settlementMonth, settlementMethodId, billingYear, billingMonth, settlementPeriod));
    }

    private bool IsTransactionOutSideTimeLimit(int rejectionStage, int settlementYear, int settlementMonth, int settlementMethodId, int billingYear, int billingMonth, int settlementPeriod)
    {
      var miscInvoice = new MiscUatpInvoice
                          {
                            SettlementYear = settlementYear,
                            SettlementMonth = settlementMonth,
                            SettlementPeriod = settlementPeriod,
                            BillingYear = billingYear,
                            BillingMonth = billingMonth,
                            RejectionStage = rejectionStage,
                            SettlementMethodId = settlementMethodId,
                            ValidationDate = new DateTime(settlementYear, settlementMonth, settlementPeriod),
                            BillingCategory = BillingCategory
                          };

      //Validate Time Limit
      if (_invoiceManager.IsTransactionOutSideTimeLimit(miscInvoice))
      {
        return true;
      }

      return false;
    }


    [HttpPost]
    public virtual JsonResult GetCorrespondenceInvoiceDetails(long correspondenceRefNumber, int billedMemberId, string invoiceId, bool isUpdateOperation)
    {
      int billingMemberId = SessionUtil.MemberId;

      var invoiceGuid = string.IsNullOrEmpty(invoiceId) ? new Guid() : invoiceId.ToGuid();
      var correspondenceInvoiceDetails = _invoiceManager.GetRejectionCorrespondenceDetail(correspondenceRefNumber, billingMemberId, billedMemberId, invoiceGuid, isUpdateOperation);

      return Json(correspondenceInvoiceDetails);
    }

    [HttpPost]
    public virtual JsonResult LineItemDelete(string lineItemId,string invoiceId)
    {
      UIMessageDetail details;
      try
      {
        //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
          //Commented below code...as it is used to get invoiceId only. Now invoice id is passed as a parameter
        /*var lineItem = _invoiceManager.GetLineItemInformation(lineItemId);
        var invoiceId = lineItem.InvoiceId.ToString();*/
        //Delete record
        var isDeleted = _invoiceManager.DeleteLineItem(lineItemId);
        //SCP294253 - Issue with miscellaneous ISWEB invoices
        //Desc: Error occurred while edit the line item once deleted one.
        String controllerName = String.Format("Manage{0}Invoice", BillingCategory);

        details = isDeleted
                    ? new UIMessageDetail { IsFailed = false, Message = Messages.DeleteSuccessful, RedirectUrl = Url.Action("EditInvoice", controllerName, new {id=  invoiceId }), isRedirect = true }
                    : new UIMessageDetail { IsFailed = true, Message = Messages.DeleteFailed };
        return Json(details);
      }
      catch (ISBusinessException ex)
      {
        details = new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.DeleteException, GetDisplayMessage(ex.ErrorCode)) };
        return Json(details);
      }
    }

    [HttpPost]
    public virtual JsonResult LineItemDetailDelete(string lineItemDetailId, string invoiceId, string lineItemId)
    {
      UIMessageDetail details;
      try
      {
        //Delete record
        var isDeleted = _invoiceManager.DeleteLineItemDetail(lineItemDetailId);
        var lineItemDetailExpected = false;
        if (isDeleted)
        {
          var lineItem = _invoiceManager.GetLineItemInformation(lineItemId);
          if (_invoiceManager.IsFieldMetaDataExists(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, (int)BillingCategory))
          {
            lineItemDetailExpected = _invoiceManager.GetLineItemDetailsCount(lineItemId) == 0;
          }
        }
        details = isDeleted
                    ? new UIMessageDetail
                        {
                          IsFailed = false,
                          Message = Messages.DeleteSuccessful,
                          RedirectUrl = Url.Action("LineItemEdit", new { lineItemId, invoiceId }),
                          isRedirect = true,
                          LineItemDetailExpected = bool.Parse(lineItemDetailExpected.ToString().ToLower())
                        }
                    : new UIMessageDetail { IsFailed = true, Message = Messages.DeleteFailed };
        return Json(details);
      }
      catch (ISBusinessException ex)
      {
        details = new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.DeleteException, GetDisplayMessage(ex.ErrorCode)) };
        return Json(details);
      }
    }

    private bool GetFieldMetaDataExistsFlag(LineItem lineItem)
    {
      var invoiceType = lineItem.Invoice != null ? lineItem.Invoice.InvoiceType : InvoiceType.Invoice;

      return invoiceType == InvoiceType.Invoice ? _invoiceManager.IsFieldMetaDataExists(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, (int)BillingCategory) : false;
    }

    //CMP288:
    /// <summary>
    /// This method use to get process and generate legal invoice PDF.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [HttpGet]
    public virtual ActionResult PreviewInvoiceOnScreen(string invoiceId)
    {
      FileStream file = null;
      var pdfFileName = string.Format("{0}_Preview.pdf", Guid.NewGuid());
      const string digitalSignatureStatus = "N";

      var legalXml = new GenerateLegalXML();
      var foPocessor = new FOProcessor();
     
      //Get Invoice Details using Invoice Id.
      var invoice = _invoiceManager.GetInvoiceDetail(invoiceId);
      
      try
      {
        //Show preview only if it's status is Ready for Billing
         if (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForSubmission)
        {
          //create object of invoice for xml generation.
          var invoiceDetails = new InvoicesForXmlGeneration()
          {
            //Logic: For Misc invoices, Set BillingCodeId by InvoiceType Id.(This is existing logic.) 
            BillingCodeId = invoice.InvoiceTypeId,
            BillingCtaegoryId = invoice.BillingCategoryId,
            InvTemplateLanguage = invoice.InvTemplateLanguage
          };

          if (string.IsNullOrEmpty(invoiceDetails.InvTemplateLanguage))
          {
            invoiceDetails.InvTemplateLanguage = "en";
          }

          var invPreviewFolderLocation = FileIo.GetForlderPath(SFRFolderPath.InvoicePreviewWorkFolder);
          
          if (!Directory.Exists(invPreviewFolderLocation))
          {
            Directory.CreateDirectory(invPreviewFolderLocation);
          }

          var workingFolder = Path.Combine(invPreviewFolderLocation, ConvertUtil.ConvertGuidToString(invoice.Id),Guid.NewGuid().ToString());
          var fONETResourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
      
          Directory.CreateDirectory(workingFolder);

          var pdfPath = string.Format("{0}\\{1}", workingFolder, pdfFileName);

          //Get legal xml for Invoice.
          var invoiceXmlDoc = legalXml.ProcessLegalXml(invoiceDetails,
                                                       ConvertUtil.ConvertGuidToString(new Guid(invoiceId)),
                                                       workingFolder, pdfPath, string.Empty, digitalSignatureStatus,
                                                       string.Empty, true);
          // Get Invoice type using invoice header discription.
          var invoiceType =
            _invoiceManager.LookupTemplateType(invoiceXmlDoc.SelectSingleNode("//TemplateInfo/Type").InnerText);

           //render pdf using invoice legal xml on fly.
          foPocessor.RenderPDF(pdfPath, invoiceType, invoiceXmlDoc, workingFolder,
                               invoiceDetails.InvTemplateLanguage, fONETResourcePath, invoice.BillingMember.MemberCodeAlpha,
                               invoice.BillingMember.MemberCodeNumeric, true);

          file = System.IO.File.Open(pdfPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

          return File(pdfPath, "application/pdf");
        }
        else
        {

          ShowErrorMessage("Unable to generate invoice preview as its status has been changed by another session.", true);
          //if invoice status is under 1,2,7,12 then open in editable mode.
          if (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForSubmission || invoice.InvoiceStatusId == (int)InvoiceStatusType.Open || invoice.InvoiceStatusId == (int)InvoiceStatusType.ValidationError || invoice.InvoiceStatusId == (int)InvoiceStatusType.FutureSubmitted)
          {
            return RedirectToAction("Edit", new
            {
              area = "Misc",
              invoiceId = invoice.Id
            });
          }
          else
          {
            return RedirectToAction("View", new
            {
              area = "Misc",
              invoiceId = invoice.Id
            });

          }
        }
      }
      catch (Exception exception)
      {
        ShowErrorMessage(string.Format("Unable to generate invoice preview."), true);
        
        Logger.InfoFormat("Error Message: {0} Stack Trace: {1}", exception.Message, exception.StackTrace);

        //if invoice status is under 1,2,7,12 then open in editable mode.
        if (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForSubmission || invoice.InvoiceStatusId == (int)InvoiceStatusType.Open || invoice.InvoiceStatusId == (int)InvoiceStatusType.ValidationError || invoice.InvoiceStatusId == (int)InvoiceStatusType.FutureSubmitted)
        {
          return RedirectToAction("Edit", new
          {
            area = "Misc",
            invoiceId = invoice.Id
          });
        }
        else
        {
          return RedirectToAction("View", new
          {
            area = "Misc",
            invoiceId = invoice.Id
          });

        }
      }
      finally
      {
        foPocessor = null;
        legalXml = null;
        if (file != null)
        {
          file.Close();
        }
      }
    }
    /// <summary>
    /// //SCP239761 - Issue with ISXML - JAN P2 (20140102)
    /// Check for line item detail is well populated or not
    /// </summary>
    /// <param name="detail">lineitem details object</param>
    private void IsLineItemDetailPopulated(LineItemDetail detail)
    {
      if (string.IsNullOrEmpty(detail.Description) || detail.Quantity <= 0 || detail.EndDate == DateTime.MinValue || string.IsNullOrEmpty(detail.UomCodeId))
      {
        throw new ISBusinessException("BMISC_10783", Messages.BMISC_10783);
      }
    }

  }
}
