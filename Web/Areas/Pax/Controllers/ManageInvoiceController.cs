using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Security.Permissions.Pax.Receivables;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.LateSubmission.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using System.IO;
using Iata.IS.Core;
using log4net;
using BillingType = Iata.IS.Web.Util.BillingType;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
  public class ManageInvoiceController : ISController
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private const string SearchResultGridAction = "SearchResultGridData";
    private readonly ICalendarManager _calendarManager;
    private readonly IInvoiceManager _invoiceManager;
    private readonly INonSamplingCreditNoteManager _nonSamplingCreditNoteManager;
    private readonly INonSamplingInvoiceManager _nonSamplingInvoiceManager;
    private readonly ISamplingFormDEManager _samplingFormDEManager;
    private readonly ISamplingFormFManager _samplingFormFManager;
    private readonly ISamplingFormXfManager _samplingFormXFManager;
    private readonly ISearchInvoiceManager _searchInvoiceManager;
    public IInvoiceManager InvoiceManagerBase { get; set; }
    public IRepository<InvoiceBase> InvoiceReository { get; set; }

    // SCP#401400: SRM: Passenger - Validate Invoices is slow
    private readonly IReferenceManager _referenceManager;

    public IMemberManager MemberManager
    {
      get;
      set;
    }

    public ManageInvoiceController(ISearchInvoiceManager searchInvoiceManager,
                                   IInvoiceManager invoiceManager,
                                   INonSamplingInvoiceManager nonSamplingInvoiceManager,
                                   INonSamplingCreditNoteManager nonSamplingCreditNoteManager,
                                   ISamplingFormDEManager samplingFormDEManager,
                                   ISamplingFormFManager samplingFormFManager,
                                   ISamplingFormXfManager samplingFormXfManager,
                                   ICalendarManager calendarManager,
                                   IReferenceManager referenceManager
      )
    {
      _searchInvoiceManager = searchInvoiceManager;
      _invoiceManager = invoiceManager;
      _nonSamplingInvoiceManager = nonSamplingInvoiceManager;
      _nonSamplingCreditNoteManager = nonSamplingCreditNoteManager;
      _samplingFormDEManager = samplingFormDEManager;
      _samplingFormFManager = samplingFormFManager;
      _samplingFormXFManager = samplingFormXfManager;
      _calendarManager = calendarManager;
      _referenceManager = referenceManager;
    }

    /// <summary>
    /// Invoice search - When page get posted this method get invoked
    /// </summary>
    [ISAuthorize(Manage.Query)]
    public ActionResult Index(SearchCriteria searchCriteria)
    {
      try
      {
        if (searchCriteria != null)
        {
          var currentBillingPeriodDetails = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
          searchCriteria.BillingPeriod = searchCriteria.BillingPeriod == 0 ? currentBillingPeriodDetails.Period : searchCriteria.BillingPeriod;
          searchCriteria.BillingMonth = searchCriteria.BillingMonth == 0 ? currentBillingPeriodDetails.Month : searchCriteria.BillingMonth;
          searchCriteria.BillingYear = searchCriteria.BillingYear == 0 ? currentBillingPeriodDetails.Year : searchCriteria.BillingYear;
        }

        string criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;

        var invoiceSearchGrid = new InvoiceSearchGrid(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction, new { area = "Pax", criteria }));
        ViewData[ViewDataConstants.SearchGrid] = invoiceSearchGrid.Instance;
        ViewData[ViewDataConstants.RejectionOnValidationFlag] = GetRejectionOnValidationFailureFlag();
        ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
      }

      return View(searchCriteria);
    }

    /// <summary>
    /// Fetch invoice searched result and display it in grid
    /// </summary>
    /// <returns></returns>
    public JsonResult SearchResultGridData(string criteria, string sidx, string sord, int page, int rows)
    {
      //int page1 = 1;
      var searchCriteria = new SearchCriteria();

      if (Request.UrlReferrer != null)
      {
        SessionUtil.InvoiceSearchCriteria = Request.UrlReferrer.ToString();
        SessionUtil.PaxCorrSearchCriteria = null;
        SessionUtil.PaxInvoiceSearchCriteria = null;
      }

      if (!string.IsNullOrEmpty(criteria))
      {
        searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(SearchCriteria)) as SearchCriteria;
      }

      searchCriteria.BillingPeriod = searchCriteria.BillingPeriod == 0 ? _calendarManager.GetCurrentBillingPeriod().Period : searchCriteria.BillingPeriod;

      // Create grid instance and retrieve data from database
      var invoiceSearchGrid = new InvoiceSearchGrid(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction, new { area = "Pax", searchCriteria }));

      // add billing member id to search criteria.
      searchCriteria.BillingMemberId = SessionUtil.MemberId;

      // If Owner Id in searchCriteria is not set, then default to current user id.
      if (searchCriteria.OwnerId == 0)
      {
        searchCriteria.OwnerId = SessionUtil.UserId;
      }

      //SCP - 85037 Performnace Issue: Querying SP to get data 
      var invoiceSearchedData = _searchInvoiceManager.GetInvoices(searchCriteria, page, rows, sidx, sord).AsQueryable();
      #region SCP - 85037:

      //Sorting list in order as per user desire
      invoiceSearchedData = SortInvoiceList(invoiceSearchedData, sidx, sord);

      //Total records related to search excluding page filter
      int totalRecords = invoiceSearchedData.Select(x => x.TotalRows).FirstOrDefault();

      //Calculating total pages grid will show
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

      //Creating json result to bind to database.
      var jsonData = new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = invoiceSearchedData.ToArray().ToSanitizeStringField()
      };

      return Json(jsonData, JsonRequestBehavior.AllowGet); //invoiceSearchGrid.DataBind(invoiceSearchedData); 
      #endregion
    }

    /// <summary>
    /// SCP - 85037: Sort list as per supplied params
    /// </summary>
    /// <param name="invoiceSearchedData"></param>
    /// <param name="sidx">sort on column name</param>
    /// <param name="sord">sort order</param>
    /// <returns></returns>
    private IQueryable<PaxInvoiceSearchDetails> SortInvoiceList(IQueryable<PaxInvoiceSearchDetails> invoiceSearchedData, string sidx, string sord)
    {
      switch (sidx)
      {
        case "DisplayBillingMonthYear":
          if (sord.ToUpper().Equals("ASC"))
          {
            invoiceSearchedData = invoiceSearchedData.OrderBy(x => x.DisplayBillingMonthYear);
          }
          else
          {
            invoiceSearchedData = invoiceSearchedData.OrderByDescending(x => x.DisplayBillingMonthYear);
          }
          break;
        case "BilledMemberText":
          if (sord.ToUpper().Equals("ASC"))
          {
            invoiceSearchedData = invoiceSearchedData.OrderBy(x => x.BilledMemberText);
          }
          else
          {
            invoiceSearchedData = invoiceSearchedData.OrderByDescending(x => x.BilledMemberText);
          }
          break;
        case "InvoiceOwnerDisplayText":
          if (sord.ToUpper().Equals("ASC"))
          {
            invoiceSearchedData = invoiceSearchedData.OrderBy(x => x.InvoiceOwnerDisplayText);
          }
          else
          {
            invoiceSearchedData = invoiceSearchedData.OrderByDescending(x => x.InvoiceOwnerDisplayText);
          }
          break;
        case "DisplayBillingCode":
          if (sord.ToUpper().Equals("ASC"))
          {
            invoiceSearchedData = invoiceSearchedData.OrderBy(x => x.DisplayBillingCode);
          }
          else
          {
            invoiceSearchedData = invoiceSearchedData.OrderByDescending(x => x.DisplayBillingCode);
          }
          break;
        case "InvoiceNumber":
          if (sord.ToUpper().Equals("ASC"))
          {
            invoiceSearchedData = invoiceSearchedData.OrderBy(x => x.InvoiceNumber);
          }
          else
          {
            invoiceSearchedData = invoiceSearchedData.OrderByDescending(x => x.InvoiceNumber);
          }
          break;
        case "InvoiceStatusDisplayText":
          if (sord.ToUpper().Equals("ASC"))
          {
            invoiceSearchedData = invoiceSearchedData.OrderBy(x => x.InvoiceStatusDisplayText);
          }
          else
          {
            invoiceSearchedData = invoiceSearchedData.OrderByDescending(x => x.InvoiceStatusDisplayText);
          }
          break;
        case "ListingCurrencyDisplayText":
          if (sord.ToUpper().Equals("ASC"))
          {
            invoiceSearchedData = invoiceSearchedData.OrderBy(x => x.ListingCurrencyDisplayText);
          }
          else
          {
            invoiceSearchedData = invoiceSearchedData.OrderByDescending(x => x.ListingCurrencyDisplayText);
          }
          break;
        case "SettlementMethodDisplayText":
          if (sord.ToUpper().Equals("ASC"))
          {
            invoiceSearchedData = invoiceSearchedData.OrderBy(x => x.SettlementMethodDisplayText);
          }
          else
          {
            invoiceSearchedData = invoiceSearchedData.OrderByDescending(x => x.SettlementMethodDisplayText);
          }
          break;
        case "ListingAmount":
          if (sord.ToUpper().Equals("ASC"))
          {
            invoiceSearchedData = invoiceSearchedData.OrderBy(x => x.ListingAmount);
          }
          else
          {
            invoiceSearchedData = invoiceSearchedData.OrderByDescending(x => x.ListingAmount);
          }
          break;
        case "ExchangeRate":
          if (sord.ToUpper().Equals("ASC"))
          {
            invoiceSearchedData = invoiceSearchedData.OrderBy(x => x.ExchangeRate);
          }
          else
          {
            invoiceSearchedData = invoiceSearchedData.OrderByDescending(x => x.ExchangeRate);
          }
          break;
        case "BillingCurrencyDisplayText":
          if (sord.ToUpper().Equals("ASC"))
          {
            invoiceSearchedData = invoiceSearchedData.OrderBy(x => x.BillingCurrencyDisplayText);
          }
          else
          {
            invoiceSearchedData = invoiceSearchedData.OrderByDescending(x => x.BillingCurrencyDisplayText);
          }
          break;
        case "BillingAmount":
          if (sord.ToUpper().Equals("ASC"))
          {
            invoiceSearchedData = invoiceSearchedData.OrderBy(x => x.BillingAmount);
          }
          else
          {
            invoiceSearchedData = invoiceSearchedData.OrderByDescending(x => x.BillingAmount);
          }
          break;
        case "SubmissionMethodDisplayText":
          if (sord.ToUpper().Equals("ASC"))
          {
            invoiceSearchedData = invoiceSearchedData.OrderBy(x => x.SubmissionMethodDisplayText);
          }
          else
          {
            invoiceSearchedData = invoiceSearchedData.OrderByDescending(x => x.SubmissionMethodDisplayText);
          }
          break;
        case "InputFileNameDisplayText":
          if (sord.ToUpper().Equals("ASC"))
          {
            invoiceSearchedData = invoiceSearchedData.OrderBy(x => x.InputFileNameDisplayText);
          }
          else
          {
            invoiceSearchedData = invoiceSearchedData.OrderByDescending(x => x.InputFileNameDisplayText);
          }
          break;
      }

      return invoiceSearchedData;
    }

    /// <summary>
    /// This action method calls Invoice Edit page
    /// </summary>
    /// <returns></returns>
    public ActionResult EditInvoice(string id)
    {
      //SCP325376 - File Loading & Web Response Stats EditInvoice CargoManageInvoice
      var invoice = _invoiceManager.GetInvoiceDetails(id);

      string controller;

      switch (invoice.BillingCode)
      {
        case (int)BillingCode.SamplingFormDE:
          controller = "FormDE";
          break;
        case (int)BillingCode.NonSampling:
          controller = invoice.InvoiceType == InvoiceType.Invoice ? "Invoice" : "CreditNote";
          break;
        case (int)BillingCode.SamplingFormF:
          controller = "FormF";
          break;
        case (int)BillingCode.SamplingFormXF:
          controller = "FormXF";
          break;
        default:
          controller = "Invoice";
          break;
      }

      return RedirectToAction("Edit", controller, new { area = "Pax", invoiceId = id });
    }

    public ActionResult ViewInvoice(string id)
    {
      //SCP325377 - File Loading & Web Response Stats ViewInvoice CargoManageInvoice
      var invoice = _invoiceManager.GetInvoiceDetails(id);

      string controller;

      switch (invoice.BillingCode)
      {
        case (int)BillingCode.SamplingFormDE:
          controller = "FormDE";
          break;
        case (int)BillingCode.NonSampling:
          controller = invoice.InvoiceType == InvoiceType.Invoice ? "Invoice" : "CreditNote";
          break;
        case (int)BillingCode.SamplingFormF:
          controller = "FormF";
          break;
        case (int)BillingCode.SamplingFormXF:
          controller = "FormXF";
          break;
        case (int)BillingCode.SamplingFormAB:
          controller = "FormAB";
          break;
        default:
          controller = invoice.InvoiceType == InvoiceType.Invoice ? "Invoice" : "CreditNote";
          break;
      }

      return RedirectToAction("View", controller, new { area = "Pax", invoiceId = id });
    }

    /// <summary>
    /// This action method calls invoice delete action method
    /// </summary>
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "id", IsJson = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult DeleteInvoice(string id)
    {
      UIExceptionDetail details;

      //SCP325378 - File Loading & Web Response Stats DeleteInvoice CargoManageInvoice
      var invoice = _invoiceManager.GetInvoiceDetails(id);
      var dummyMemberId = MemberManager.GetMemberId(String.IsNullOrEmpty(ConfigurationManager.AppSettings["DummyMembercode"].Trim()) ? "000" : ConfigurationManager.AppSettings["DummyMembercode"].Trim());
      var userId = SessionUtil.AdminUserId;

      try
      {
        //Delete record
        //bool isDeleted = _invoiceManager.DeleteInvoice(id);
        //CMP 400: Before delete invoice. Add entry into INVOICE_DELETION_AUDIT table and update invoice billing and billed member by dummy member.

        var invoiceIdStringInOracleFormat = ConvertUtil.ConvertNetGuidToOracleGuid(id);

        var isDeleted = _invoiceManager.DeleteInvoice(invoiceIdStringInOracleFormat, dummyMemberId, userId, 1);

        details = isDeleted
                    ? new UIExceptionDetail { IsFailed = false, Message = string.Format(Messages.InvoiceDeleteSuccessful, invoice.InvoiceNumber) }
                    : new UIExceptionDetail { IsFailed = true, Message = string.Format(Messages.InvoiceDeleteFailed, invoice.InvoiceNumber) };
      }
      catch (ISBusinessException)
      {
        details = new UIExceptionDetail { IsFailed = true, Message = string.Format(Messages.InvoiceDeleteFailed, invoice.InvoiceNumber) };
      }

      return Json(details);
    }

    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "id", IsJson = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult ValidateInvoice(string id)
    {
        // SCP#401400: SRM: Passenger - Validate Invoices is slow
        var logRefId = Guid.NewGuid();
        var sessionUserId = SessionUtil.UserId;
        var log = _referenceManager.GetDebugLog(DateTime.Now,
                                                "ManageInvoiceController.ValidateInvoice",
                                                this.ToString(),
                                                BillingCategorys.Passenger.ToString(),
                                                "Step 1 of 12: Id: " + id + " ManageInvoiceController.ValidateInvoice Start",
                                                sessionUserId,
                                                logRefId.ToString());
        _referenceManager.LogDebugData(log);

      // TODO: This method needs refactoring
      UIMessageDetail details;
      //SCP325375:File Loading & Web Response Stats ManageInvoice
      var invoice = _invoiceManager.GetInvoiceHeader(id);
      
      // SCP#401400: SRM: Passenger - Validate Invoices is slow
      log = _referenceManager.GetDebugLog(DateTime.Now,
                                          "ManageInvoiceController.ValidateInvoice",
                                          this.ToString(),
                                          BillingCategorys.Passenger.ToString(),
                                          "Step 2 of 12: Id: " + id + " After _invoiceManager.GetInvoiceHeader(id)",
                                          sessionUserId,
                                          logRefId.ToString());
      _referenceManager.LogDebugData(log);

      bool isFutureSubmission;
      try
      {
        if (invoice.BillingCode == Convert.ToInt32(BillingCode.SamplingFormDE))
        {
            invoice = _samplingFormDEManager.ValidateInvoice(id, out isFutureSubmission, sessionUserId, logRefId.ToString());
        }
        else if (invoice.BillingCode == Convert.ToInt32(BillingCode.SamplingFormF))
        {
            invoice = _samplingFormFManager.ValidateInvoice(id, out isFutureSubmission, sessionUserId, logRefId.ToString());
        }
        else if (invoice.BillingCode == Convert.ToInt32(BillingCode.SamplingFormXF))
        {
            invoice = _samplingFormXFManager.ValidateInvoice(id, out isFutureSubmission, sessionUserId, logRefId.ToString());
        }
        else
        {
            invoice = invoice.InvoiceType == InvoiceType.Invoice ? _nonSamplingInvoiceManager.ValidateInvoice(id, out isFutureSubmission, sessionUserId, logRefId.ToString())
                                                                 : _nonSamplingCreditNoteManager.ValidateInvoice(id, out isFutureSubmission, sessionUserId, logRefId.ToString());
        }



        details = invoice.InvoiceStatus == InvoiceStatusType.ReadyForSubmission || invoice.InvoiceStatus == InvoiceStatusType.FutureSubmitted
                    ? new UIMessageDetail { IsFailed = false, Message = string.Format(Messages.InvoiceValidateSuccessful, invoice.InvoiceNumber) }
                    : new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.InvoiceValidateFailed, invoice.InvoiceNumber) };

        //SCP85837: PAX CGO Sequence Number
        if (invoice.IsRecordSequenceArranged == RecordSequence.IsArranged)
        {
          details.IsAlert = true;
        }

        //SCP149711: Incorrect Form E UA to 3M
        if (invoice.IsRecalculatedFormE == RecalculateFormE.Yes)
        {
          details.IsRecalAlert = true;
        }
      }
      catch (ISBusinessException)
      {
        details = new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.InvoiceValidateFailed, invoice.InvoiceNumber) };
      }

      // SCP#401400: SRM: Passenger - Validate Invoices is slow
      log = _referenceManager.GetDebugLog(DateTime.Now,
                                          "ManageInvoiceController.ValidateInvoice",
                                          this.ToString(),
                                          BillingCategorys.Passenger.ToString(),
                                          "Step 12 of 12: Id: "+ id + " ManageInvoiceController.ValidateInvoice End",
                                          sessionUserId,
                                          logRefId.ToString());
      _referenceManager.LogDebugData(log);

      return Json(details);
    }

    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "id", InvList = true, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult SubmitInvoices(string id, int alreadySubmittedInvCount = 0, int alreadyDeletedInvCount = 0)
    {
      UIMessageDetail messageDetails;

      // If user has not selected Invoices to submit display message
      // CMP400: handling deleted invoices also by using alreadyDeletedInvCount
      if (!string.IsNullOrEmpty(id))
      {
        var invoiceIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        try
        {
          // ID : 296572 - Submission and Assign permission to user doesn't match !
          //Get invoice count if manage invoice screen display mix invoices.
          //if count > 0 then show generic error in alert message property.
          var invoicesTobeSubmit = _invoiceManager.ChkInvSubmitPermission(invoiceIdList, SessionUtil.UserId);
          var permissionReq = (invoiceIdList.Count - invoicesTobeSubmit.Count) > 0 ? "One or more invoice(s)/credit note(s) could not submitted due to missing permissions." : string.Empty;
          var submittedInvoices = _invoiceManager.SubmitInvoices(invoicesTobeSubmit);

          if (submittedInvoices.Count > 0)
          {
            if (alreadySubmittedInvCount > 0 && alreadyDeletedInvCount > 0)
            {
              messageDetails = new UIMessageDetail
                                   {
                                     IsFailed = false,
                                     Message =
                                         string.Format(
                                             "{0} invoice(s) submitted Successfully. {1} invoice(s) already submitted. {2} invoice(s) already submitted.",
                                             submittedInvoices.Count, alreadySubmittedInvCount,
                                             alreadyDeletedInvCount),
                                     AlertMessage = permissionReq
                                   };

            }
            else if (alreadyDeletedInvCount > 0)
            {
              messageDetails = new UIMessageDetail
                                   {
                                     IsFailed = false,
                                     Message =
                                         string.Format(
                                             "{0} invoice(s) submitted Successfully. {1} invoice(s) already deleted.",
                                             submittedInvoices.Count, alreadyDeletedInvCount),
                                     AlertMessage = permissionReq
                                   };

            }
            else if (alreadySubmittedInvCount > 0)
            {
              messageDetails = new UIMessageDetail
                                   {
                                     IsFailed = false,
                                     Message =
                                         string.Format(
                                             "{0} invoice(s) submitted Successfully. {1} invoice(s) already submitted..",
                                             submittedInvoices.Count, alreadySubmittedInvCount),
                                     AlertMessage = permissionReq
                                   };

            }
            else
            {
              messageDetails = new UIMessageDetail
                                   {
                                     IsFailed = false,
                                     Message =
                                         string.Format(Messages.InvoicesSubmittedCount, submittedInvoices.Count),
                                     AlertMessage = permissionReq
                                   };
            }
          }
          else
          {
            if (alreadySubmittedInvCount > 0 && alreadyDeletedInvCount > 0)
            {
              messageDetails = new UIMessageDetail
                                   {
                                     IsFailed = false,
                                     Message =
                                         string.Format(
                                             Messages.InvoiceIneligibleForSubmission +
                                             "{0} invoice(s) already submitted. {1} invoice(s) already deleted.",
                                             alreadySubmittedInvCount, alreadyDeletedInvCount),
                                     AlertMessage = permissionReq
                                   };

            }
            else if (alreadyDeletedInvCount > 0)
            {
              messageDetails = new UIMessageDetail
                                   {
                                     IsFailed = false,
                                     Message =
                                         string.Format(Messages.InvoiceIneligibleForSubmission +
                                                       " {0} invoice(s) already deleted.",
                                                       alreadyDeletedInvCount),
                                     AlertMessage = permissionReq
                                   };
            }
            else if (alreadySubmittedInvCount > 0)
            {
              messageDetails = new UIMessageDetail
                                   {
                                     IsFailed = true,
                                     Message =
                                         string.Format(
                                             Messages.InvoiceIneligibleForSubmission +
                                             " {0} invoice(s) already submitted..", alreadySubmittedInvCount) + "/" + permissionReq
                                   };
            }
            else
            {
              messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.InvoiceIneligibleForSubmission + "/" + permissionReq };
            }
          }
          return Json(messageDetails);

        }
        catch (ISBusinessException exception)
        {
          messageDetails = new UIMessageDetail { IsFailed = true, Message = GetDisplayMessageWithErrorCode(exception.ErrorCode) };

          return Json(messageDetails);
        }
      }
      else
      {
        if (alreadySubmittedInvCount > 0)
        {
          messageDetails = new UIMessageDetail { IsFailed = true, Message = "Invoice(s) already submitted. Please refresh your page." };
        }
        else
        {
          messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.SelectInvoiceForSubmission };
        }

        return Json(messageDetails);

      }
    }

    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "id", InvList = true, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult PresentInvoices(string id)
    {
      UIMessageDetail messageDetails;

      var invoiceIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

      try
      {
        var presentInvoices = _invoiceManager.PresentInvoices(invoiceIdList.ToList());
        if (presentInvoices.Count > 0)
        {
          messageDetails = new UIMessageDetail { IsFailed = false, Message = string.Format(Messages.InvoicesSubmittedCount, presentInvoices.Count) };
        }
        else
        {
          messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.InvoiceIneligibleForSubmission };
        }

        return Json(messageDetails);
      }
      catch (ISBusinessException exception)
      {
        messageDetails = new UIMessageDetail { IsFailed = true, Message = GetDisplayMessageWithErrorCode(exception.ErrorCode) };

        return Json(messageDetails);
      }
    }

    [HttpPost]
    public ActionResult MarkInvoicesToProcessingComplete(string id)
    {
      UIMessageDetail messageDetails;

      var invoiceIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

      try
      {
        var processingCompleteInvoices = _invoiceManager.ProcessingCompleteInvoices(invoiceIdList.ToList());
        if (processingCompleteInvoices.Count > 0)
        {
          messageDetails = new UIMessageDetail { IsFailed = false, Message = string.Format(Messages.InvoicesProcessingCompleteCount, processingCompleteInvoices.Count) };
        }
        else
        {
          messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.InvoiceIneligibleForProcessingComplete };
        }

        return Json(messageDetails);
      }
      catch (ISBusinessException exception)
      {
        messageDetails = new UIMessageDetail { IsFailed = true, Message = GetDisplayMessageWithErrorCode(exception.ErrorCode) };

        return Json(messageDetails);
      }
    }

    [HttpGet]
    public FileStreamResult DownloadPdf(string id)
    {
      var invoicePdfFilePath = _invoiceManager.GetInvoiceLegalPfdPath(id.ToGuid());
      var fileName = Path.GetFileName(invoicePdfFilePath);
      try
      {
        var fs = System.IO.File.Open(invoicePdfFilePath, FileMode.Open, FileAccess.Read);
        return File(fs, "application/octet", fileName);
      }
      catch (Exception exception)
      {
        Logger.Error(string.Format("Exception:{0} StackTrace:{1}", exception.Message, exception.StackTrace), exception);

        var memoryStream = ConvertUtil.GetMemoryStreamForMessage("Error occurred while downloading pdf file.");

        return File(memoryStream, "text/plain", "Download-Pdf-Error.txt");
      }
    }

    [HttpPost]
    public JsonResult DownloadZip(string id, string options)
    {
      UIMessageDetail details;
      try
      {
        Guid invoiceId;
        var zipFileName = Guid.NewGuid().ToString();
        if (Guid.TryParse(id, out invoiceId))
        {
          //SCP334940: SRM Exception occurred in Iata.IS.Service.Iata.IS.Service.OfflineCollectionDownloadService. - SIS Production
          int memberId = InvoiceManagerBase.GetInvoice(invoiceId).BillingMemberId;
          
          if (SessionUtil.UserId > 0 && SessionUtil.MemberId > 0 && memberId == SessionUtil.MemberId)
          {
            var downloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
                                                                   "Invoice",
                                                                   new
                                                                     {
                                                                       area = "Pax",
                                                                       billingType = "Receivables"
                                                                     }));

            var iInvoiceOfflineCollectionManager = Ioc.Resolve<IInvoiceOfflineCollectionManager>(typeof(IInvoiceOfflineCollectionManager));
            IDictionary<string, string> messages = new Dictionary<string, string>
                                                   {
                                                      { "RECORD_ID", ConvertUtil.ConvertGuidToString(invoiceId)},
                                                      { "USER_ID", SessionUtil.UserId.ToString() },
                                                      { "OPTIONS", options},
                                                      { "IS_FORM_C", "0" },
                                                      { "IS_RECEIVABLE", "1" },
                                                      { "OUTPUT_ZIP_FILE_NAME", zipFileName },
                                                      {"DOWNLOAD_URL", downloadUrl },
                                                      {"IS_WEB_DOWNLOAD","1"}
                                                   };
            // SCP227747: Cargo Invoice Data Download
            // Message will display on screen depending on Success or Failure of Enqueing message to queue.
            var isEnqueSuccess = false;
            isEnqueSuccess = iInvoiceOfflineCollectionManager.EnqueueDownloadRequest(messages);

            if (isEnqueSuccess)
            {
              details = new UIMessageDetail
              {
                IsFailed = false,
                Message =
                  string.Format(
                    "Generation of selected information in progress. You will be notified as per your profile settings, once the required information is ready and available for retrieval [File: {0}]",
                    string.Format("{0}.zip", zipFileName))
              };
            }
            else
            {
              details = new UIMessageDetail
              {
                IsFailed = true,
                Message = "Failed to download the invoice, please try again!"
              };
            }
          }
          else
          {
            details = new UIMessageDetail
            {
              IsFailed = true,
              RedirectUrl = Url.Action("LogOn", "Account", new { area = "" })
            };
          }
        }
        else
        {
          details = new UIMessageDetail
          {
            IsFailed = true,
            Message = "Given invoice id is not valid."
          };
        }
      }
      catch (Exception)
      {
        details = new UIMessageDetail
        {
          IsFailed = true,
          Message = "Failed to download the invoice, please try again!"
        };
      }

      return Json(details);
    }

    protected override void HandleUnknownAction(string actionName)
    {
      EditInvoice(Request.QueryString["transactionId"]);

      base.HandleUnknownAction(actionName);
    }


    private RejectionOnValidationFailure GetRejectionOnValidationFailureFlag()
    {
      var rejectionFlag = RejectionOnValidationFailure.RejectInvoiceInError;
      var rejectionFlagId = MemberManager.GetMemberConfigurationValue(SessionUtil.MemberId, MemberConfigParameter.PaxRejectionOnValidationFailure);
      if (!string.IsNullOrEmpty(rejectionFlagId))
        rejectionFlag = (RejectionOnValidationFailure)Convert.ToInt32(rejectionFlagId);
      return rejectionFlag;
    }
  }
}
