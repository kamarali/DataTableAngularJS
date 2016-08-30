using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Iata.IS.AdminSystem;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core.DI;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Pax;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Business.Pax;
using Iata.IS.Core.Exceptions;
using Iata.IS.Core;
using Iata.IS.Web.UIModel.Grid.Misc;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using WebUtil = Iata.IS.Web.Util;
using System.Linq;
using Iata.IS.Model.Pax.Enums;
using System;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Model.Pax;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
  public class FormCController : ISController
  {
    private readonly ISamplingFormCManager _samplingFormCManager;
    private readonly IReferenceManager _referenceManager;
    public IInvoiceOfflineCollectionDownloadManager InvoiceOfflineCollectionDownloadManager { get; set; }
    private readonly IMemberManager _memberManager;
    public ISamplingFormCRepository SamplingFormCRepository { get; set; }
    public IInvoiceManager InvoiceManagerBase { get; set; }
   
    public IValidationErrorManager ValidationManager
    {
      get;
      set;
    }

    private SamplingFormC FormCHeader
    {
      get;
      set;
    }

    private const string NilFormCIndicatorYes = "Y";
    private const string GetFormCSummaryAction = "GetFormCSummaryDetails";
    private const string GetFormCCouponsAction = "GetFormCCoupons";
    private const string GetFormCSearchResultAction = "GetFormCSearchResults";
    private const string GetFormCPayablesSearchResultAction = "GetFormCPayablesSearchResults";
    private const string formCInvoice = "FORMC";

    public FormCController(ISamplingFormCManager samplingFormCManager, IReferenceManager referenceManager, IMemberManager memberManager)
    {
      _samplingFormCManager = samplingFormCManager;
      _referenceManager = referenceManager;
      _memberManager = memberManager;
    }

    /// <summary>
    /// Following method will be executed before execution of any Action within this controller.
    /// </summary>
    /// <param name="filterContext">Context of requested action</param>
    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      // Call base class's OnActionExecuting() method
      base.OnActionExecuting(filterContext);

      const string invoiceIdParamKey = "invoiceId";
      const string provisionalBillingMonthParamKey = "provisionalBillingMonth";
      const string provisionalBillingYearParamKey = "provisionalBillingYear";
      const string provisionalBillingMemberIdParamKey = "provisionalBillingMemberId";
      const string fromMemberIdParamKey = "fromMemberId";
      const string listingCurrencyIdParamKey = "listingCurrencyId";
      const string invoiceStatusIdParamKey = "invoiceStatusId";

      SetViewDataBillingType(WebUtil.BillingType.Receivables);

      // Check whether requested action contains parameter named "InvoiceId"
      var invoiceIdParam = filterContext.ActionParameters.ContainsKey(invoiceIdParamKey) ? filterContext.ActionParameters[invoiceIdParamKey] : null;

      // Check whether requested action is "POST".
      var isPost = filterContext.RequestContext.HttpContext.Request.HttpMethod == "POST";

      // If requested action is "GET" action and contains parameter named "InvoiceId" retrieve Invoice header details
      if (!isPost)
      {
        if (invoiceIdParam != null) // Retrieve Invoice header details
        {
          FormCHeader = _samplingFormCManager.GetSamplingFormCDetails(invoiceIdParam.ToString());
          AuthorizeFormCHeader(FormCHeader, filterContext);
        }

        else
        {
          var provisionalBillingMonthParam = filterContext.ActionParameters.ContainsKey(provisionalBillingMonthParamKey) ? filterContext.ActionParameters[provisionalBillingMonthParamKey] : null;
          var provisionalBillingYearParam = filterContext.ActionParameters.ContainsKey(provisionalBillingYearParamKey) ? filterContext.ActionParameters[provisionalBillingYearParamKey] : null;
          var provisionalBillingMemberIdParam = filterContext.ActionParameters.ContainsKey(provisionalBillingMemberIdParamKey)
                                                  ? filterContext.ActionParameters[provisionalBillingMemberIdParamKey]
                                                  : null;
          var fromMemberIdParam = filterContext.ActionParameters.ContainsKey(fromMemberIdParamKey) ? filterContext.ActionParameters[fromMemberIdParamKey] : null;
          var listingCurrencyIdParam = filterContext.ActionParameters.ContainsKey(listingCurrencyIdParamKey) ? filterContext.ActionParameters[listingCurrencyIdParamKey] : null;
          var invoiceStatusIdParam = filterContext.ActionParameters.ContainsKey(invoiceStatusIdParamKey) ? filterContext.ActionParameters[invoiceStatusIdParamKey] : null;
          if (provisionalBillingMonthParam != null && provisionalBillingYearParam != null && provisionalBillingMemberIdParam != null && fromMemberIdParam != null && invoiceStatusIdParam != null)
          {
            FormCHeader = _samplingFormCManager.GetSamplingFormCDetails(Convert.ToInt32(provisionalBillingMonthParam),
                                                                        Convert.ToInt32(provisionalBillingYearParam),
                                                                        Convert.ToInt32(provisionalBillingMemberIdParam),
                                                                        Convert.ToInt32(fromMemberIdParam),
                                                                        Convert.ToInt32(invoiceStatusIdParam),
                                                                        (int?)listingCurrencyIdParam);
            AuthorizeFormCHeader(FormCHeader, filterContext);
          }
        }
      }
    }

    private void AuthorizeFormCHeader(SamplingFormC samplingFormC, ActionExecutingContext filterContext)
    {
      // If invoice was retrieved successfully.
      if (samplingFormC == null)
      {
        return;
      }

      // Check whether member is authorized to access this invoice.
      if (!IsMemberAuthorized(samplingFormC))
      {
        filterContext.Result = Unauthorized(string.Empty);
      }
      else
      {
        // Set the page mode to View or Edit depending on InvoiceStatus.
        SetPageMode(samplingFormC.InvoiceStatus);
        //SCP15780 changes
        // If pageMode == View and selected action method has "RestrictUnauthorizedUpdateAttribute" attribute redirect user to Unauthorized View   
        //if (ViewData[ViewDataConstants.PageMode].Equals(PageMode.View) &&
        //    filterContext.ActionDescriptor.GetCustomAttributes(typeof(RestrictUnauthorizedUpdateAttribute), true).Count() > 0)
        //{
        //  filterContext.Result = Unauthorized(string.Empty);
        //}
      }
    }

    /// <summary>
    /// Following action will check Invoice status and depending on it, will set PageMode i.e. View mode or Edit mode
    /// </summary>
    /// <param name="invoiceStatus">Invoice Status</param>
    public void SetPageMode(InvoiceStatusType invoiceStatus)
    {
      // If InvoiceStatus is equal to any of below set page mode to View, else Edit
      if (invoiceStatus == InvoiceStatusType.ReadyForBilling || invoiceStatus == InvoiceStatusType.ProcessingComplete || invoiceStatus == InvoiceStatusType.Presented ||
          invoiceStatus == InvoiceStatusType.Claimed || invoiceStatus == InvoiceStatusType.ErrorCorrectable || invoiceStatus == InvoiceStatusType.ErrorNonCorrectable ||
          invoiceStatus == InvoiceStatusType.OnHold || invoiceStatus == InvoiceStatusType.FutureSubmitted)
      {
        SetViewDataPageMode(PageMode.View);
      }
      else
      {
        SetViewDataPageMode(PageMode.Edit);
      }
    }

    protected ActionResult Unauthorized(string invoiceNumber)
    {
      TempData[ViewDataConstants.InvoiceNumber] = invoiceNumber;

      return View("~/Views/Shared/UnAuthorized.aspx");
    }

    /// <summary>
    /// Following action is used to check whether the user is either a billing or billed member.
    /// </summary>
    /// <param name="samplingFormC">The sampling form C.</param>
    /// <returns>
    /// 	<c>true</c> if [is member authorized] [the specified sampling form C]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsMemberAuthorized(SamplingFormC samplingFormC)
    {
      var loggedInMemberId = SessionUtil.MemberId;
      bool isAuthorized = false;

      // provisional billing member should be allowed to only view Presented Form C.
      if ((loggedInMemberId == samplingFormC.ProvisionalBillingMemberId) && (samplingFormC.InvoiceStatus == InvoiceStatusType.Presented))
      {
        SetViewDataBillingType(WebUtil.BillingType.Payables);
        isAuthorized = true;
      }
      else if (loggedInMemberId == samplingFormC.FromMemberId)
      {
        SetViewDataBillingType(WebUtil.BillingType.Receivables);
        isAuthorized = true;
      }

      return isAuthorized;
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.View)]
    [HttpGet]
    public ActionResult ViewDetails(int provisionalBillingMonth, int provisionalBillingYear, int provisionalBillingMemberId, int fromMemberId, int? listingCurrencyId, int invoiceStatusId)
    {
      var summaryGrid = new FormCSummaryGrid(ControlIdConstants.FormCSummaryGridId,
                                             Url.Action(GetFormCSummaryAction,
                                                        new { provisionalBillingMonth, provisionalBillingYear, provisionalBillingMemberId, fromMemberId, listingCurrencyId, invoiceStatusId }));
      ViewData[ViewDataConstants.FormCSummaryListGrid] = summaryGrid.Instance;

      var formCCouponListGrid = new FormCCouponGrid(ControlIdConstants.FormCCouponGridId,
                                                    Url.Action(GetFormCCouponsAction,
                                                               new { provisionalBillingMonth, provisionalBillingYear, provisionalBillingMemberId, fromMemberId, invoiceStatusId, listingCurrencyId }));

      ViewData[ViewDataConstants.FormCCouponListGrid] = formCCouponListGrid.Instance;

      return View("Details", FormCHeader);
    }

    public JsonResult GetFormCSummaryDetails(int provisionalBillingMonth, int provisionalBillingYear, int provisionalBillingMemberId, int fromMemberId, int? listingCurrencyId, int invoiceStatusId)
    {
      var summaryGrid = new FormCSummaryGrid(ControlIdConstants.FormCSummaryGridId,
                                             Url.Action(GetFormCSummaryAction,
                                                        new { provisionalBillingMonth, provisionalBillingYear, provisionalBillingMemberId, fromMemberId, listingCurrencyId, invoiceStatusId }));
      var sourceCodeTotal = _samplingFormCManager.GetSamplingFormCSourceCodeTotal(provisionalBillingMonth,
                                                                                  provisionalBillingYear,
                                                                                  provisionalBillingMemberId,
                                                                                  fromMemberId,
                                                                                  invoiceStatusId,
                                                                                  listingCurrencyId);
      return summaryGrid.DataBind(sourceCodeTotal.AsQueryable());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="transactionId">Sampling Form C id.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.Submit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "transactionId", IsJson = false, TableName = TransactionTypeTable.PAX_FORM_C, ActionParamName = "Edit")]
    public ActionResult Submit(string transactionId)
    {
      var submittedFormC = _samplingFormCManager.SubmitSamplingFormC(transactionId);

      if (submittedFormC != null && submittedFormC.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
      {
        //SCP 212028 - Missing data in SFI30 Airline 27 Oct P4 Incoming Form C// Removed below code line and called the UpdateFormCSourceCodeTotal in SubmitSamplingFormC method
        //// Call stored procedure which will update SourceCode total value
        //_samplingFormCManager.UpdateFormCSourceCodeTotal(transactionId.ToGuid());

        ShowSuccessMessage(Messages.FormCSubmissionSuccessful);
        return RedirectToAction("ViewDetails",
                                new
                                  {
                                    provisionalBillingMonth = submittedFormC.ProvisionalBillingMonth,
                                    provisionalBillingYear = submittedFormC.ProvisionalBillingYear,
                                    provisionalBillingMemberId = submittedFormC.ProvisionalBillingMemberId,
                                    fromMemberId = submittedFormC.FromMemberId,
                                    listingCurrencyId = submittedFormC.ListingCurrencyId,
                                    invoiceStatusId = submittedFormC.InvoiceStatusId
                                  });
      }

      ShowErrorMessage(Messages.FormCSubmissionFailed, true);
      return RedirectToAction("Edit", new { invoiceId = transactionId });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="transactionId">Sampling Form C Id.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.Validate)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "transactionId", IsJson = false, TableName = TransactionTypeTable.PAX_FORM_C, ActionParamName = "Edit")]
    public ActionResult Validate(string transactionId)
    {
      SamplingFormC validatedFormC;
      try
      {
        validatedFormC = _samplingFormCManager.ValidateSamplingFormC(transactionId);

        switch (validatedFormC.InvoiceStatus)
        {
          case InvoiceStatusType.ReadyForSubmission:
            ShowSuccessMessage(string.Format(Messages.FormCValidationSuccessful));
            break;

          default:
            ShowErrorMessage(string.Format(Messages.FormCValidationFailed), true);
            break;
        }
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode, true);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      return RedirectToAction("Edit", new { invoiceId = transactionId });
    }

    /// <summary>
    /// Used to validate a Form C record through Manage Form C screen.
    /// </summary>
    /// <param name="provisionalBillingMonth"></param>
    /// <param name="provisionalBillingYear"></param>
    /// <param name="provisionalBillingMemberId"></param>
    /// <param name="fromMemberId"></param>
    /// <param name="listingCurrencyId"></param>
    /// <param name="invoiceStatusId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.Validate)]
    [HttpPost]
    public ActionResult ValidateFormC(int provisionalBillingMonth, int provisionalBillingYear, int provisionalBillingMemberId, int fromMemberId, int? listingCurrencyId, int invoiceStatusId)
    {
      UIExceptionDetail details;
      SamplingFormC validatedFormC;
      var samplingFormCHeader = _samplingFormCManager.GetSamplingFormCDetails(provisionalBillingMonth,
                                                                              provisionalBillingYear,
                                                                              provisionalBillingMemberId,
                                                                              fromMemberId,
                                                                              invoiceStatusId,
                                                                              listingCurrencyId);
      var formCId = samplingFormCHeader.Id.ToString();
      try
      {
        validatedFormC = _samplingFormCManager.ValidateSamplingFormC(formCId);

        details = validatedFormC.InvoiceStatus == InvoiceStatusType.ReadyForSubmission
                    ? new UIExceptionDetail { IsFailed = false, Message = Messages.FormCValidationSuccessful }
                    : new UIExceptionDetail { IsFailed = true, Message = Messages.FormCValidationFailed };
      }
      catch (ISBusinessException exception)
      {
        details = new UIExceptionDetail { IsFailed = true, Message = GetDisplayMessageWithErrorCode(exception.ErrorCode) };
      }

      return Json(details);
    }

    public JsonResult GetFormCCoupons(int provisionalBillingMonth, int provisionalBillingYear, int provisionalBillingMemberId, int fromMemberId, int? listingCurrencyId, int invoiceStatusId)
    {
      var formCCouponListGrid = new FormCCouponGrid(ControlIdConstants.FormCCouponGridId,
                                                    Url.Action(GetFormCCouponsAction,
                                                               new { provisionalBillingMonth, provisionalBillingYear, provisionalBillingMemberId, fromMemberId, invoiceStatusId, listingCurrencyId }));
      var formCCouponList = _samplingFormCManager.GetSamplingFormCRecordList(provisionalBillingMonth,
                                                                             provisionalBillingYear,
                                                                             provisionalBillingMemberId,
                                                                             fromMemberId,
                                                                             invoiceStatusId,
                                                                             listingCurrencyId);

      return formCCouponListGrid.DataBind(formCCouponList.AsQueryable());
    }

    //
    // GET: /Pax/FormC/Create
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.CreateOrEdit)]
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult Create()
    {
      SessionUtil.FormCSearchCriteria = null;
      var samplingFormC = new SamplingFormC { ProvisionalBillingMember = new Model.MemberProfile.Member(), LastUpdatedBy = SessionUtil.UserId };

      return View(samplingFormC);
    }

    //
    // POST: /Pax/FormC/Create
    [ValidateAntiForgeryToken] 
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.CreateOrEdit)]
    [HttpPost]
    public ActionResult Create(SamplingFormC samplingFormCHeader)
    {
      try
      {
        samplingFormCHeader.FromMemberId = SessionUtil.MemberId;
        samplingFormCHeader.SubmissionMethod = SubmissionMethod.IsWeb;
        samplingFormCHeader.LastUpdatedBy = SessionUtil.UserId;
        // CMP #659: SIS IS-WEB Usage Report
        samplingFormCHeader.FormCOwner = SessionUtil.UserId;
        samplingFormCHeader = _samplingFormCManager.CreateSamplingFormC(samplingFormCHeader);

        ShowSuccessMessage(Messages.FormCCreateSuccessful);
        //ShowSuccessMessage(SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod ? Messages.FormCCreateSuccessful + " " + Messages.ValidationIgnoredDueToMigration : Messages.FormCCreateSuccessful);

        return RedirectToAction("Edit", new { invoiceId = samplingFormCHeader.Id.Value() });
      }
      catch (ISBusinessException businessException)
      {
        samplingFormCHeader.ProvisionalBillingMember = _memberManager.GetMember(samplingFormCHeader.ProvisionalBillingMemberId);
        ShowErrorMessage(businessException.ErrorCode);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Create);
      }

      return View(samplingFormCHeader);
    }

    //
    // GET: /Pax/FormC/Edit/5
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    public ActionResult Edit(string invoiceId)
    {
      GetSummaryCouponLists(FormCHeader);

      RenderValidationErrorGrid(invoiceId, FormCHeader);

      return View(FormCHeader);
    }

    private void RenderValidationErrorGrid(string invoiceId, SamplingFormC samplingFormCHeader)
    {
      if (samplingFormCHeader.InvoiceStatus == InvoiceStatusType.ValidationError)
      {
        // Get all submitted errors.
        var submittedErrorsGrid = new SubmittedErrorsGrid(ControlIdConstants.SubmittedErrorsGridId, Url.Action("GetSubmittedErrors", new { invoiceId }));
        ViewData[ViewDataConstants.SubmittedErrorsGrid] = submittedErrorsGrid.Instance;
      }
    }

    private void GetSummaryCouponLists(SamplingFormC samplingFormCHeader)
    {
      var summaryGrid = new FormCSummaryGrid(ControlIdConstants.FormCSummaryGridId,
                                             Url.Action(GetFormCSummaryAction,
                                                        new
                                                          {
                                                            provisionalBillingMonth = samplingFormCHeader.ProvisionalBillingMonth,
                                                            provisionalBillingYear = samplingFormCHeader.ProvisionalBillingYear,
                                                            provisionalBillingMemberId = samplingFormCHeader.ProvisionalBillingMemberId,
                                                            fromMemberId = samplingFormCHeader.FromMemberId,
                                                            listingCurrencyId = samplingFormCHeader.ListingCurrencyId,
                                                            invoiceStatusId = samplingFormCHeader.InvoiceStatusId
                                                          }));
      ViewData[ViewDataConstants.FormCSummaryListGrid] = summaryGrid.Instance;

      var formCCouponListGrid = new FormCCouponGrid(ControlIdConstants.FormCCouponGridId,
                                                    Url.Action(GetFormCCouponsAction,
                                                               new
                                                                 {
                                                                   provisionalBillingMonth = samplingFormCHeader.ProvisionalBillingMonth,
                                                                   provisionalBillingYear = samplingFormCHeader.ProvisionalBillingYear,
                                                                   provisionalBillingMemberId = samplingFormCHeader.ProvisionalBillingMemberId,
                                                                   fromMemberId = samplingFormCHeader.FromMemberId,
                                                                   listingCurrencyId = samplingFormCHeader.ListingCurrencyId,
                                                                   invoiceStatusId = samplingFormCHeader.InvoiceStatusId
                                                                 }));
      ViewData[ViewDataConstants.FormCCouponListGrid] = formCCouponListGrid.Instance;
    }

    //
    // GET: /Pax/FormC/Edit/{}//
    // This method will be used when user navigates to Edit page from Search Form C screen.
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public ActionResult FormCEdit(int provisionalBillingYear, int provisionalBillingMonth, int provisionalBillingMemberId, int fromMemberId, int? listingCurrencyId, int invoiceStatusId)
    {
      var samplingFormCHeader = _samplingFormCManager.GetSamplingFormCDetails(provisionalBillingMonth, provisionalBillingYear, provisionalBillingMemberId, fromMemberId, invoiceStatusId, listingCurrencyId);
      samplingFormCHeader.LastUpdatedBy = SessionUtil.UserId;
      return RedirectToAction("Edit", new { invoiceId = samplingFormCHeader.Id.Value() });
    }

    //
    // POST: /Pax/FormC/Edit/5
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.PAX_FORM_C)]
    public ActionResult Edit(string invoiceId, SamplingFormC samplingFormCHeader)
    {
      try
      {
          if (samplingFormCHeader != null && samplingFormCHeader.ProvisionalBillingMemberId == 0)
          {
              throw new ISBusinessException(ErrorCodes.InvalidMemberType);
          }
        samplingFormCHeader.FromMemberId = SessionUtil.MemberId;
        samplingFormCHeader.Id = invoiceId.ToGuid();
        samplingFormCHeader.LastUpdatedBy = SessionUtil.UserId;
        samplingFormCHeader.SubmissionMethod = SubmissionMethod.IsWeb;
        
        //CMP #659: SIS IS-WEB Usage Report
        samplingFormCHeader.FormCOwner = samplingFormCHeader.FormCOwner;
        samplingFormCHeader = _samplingFormCManager.UpdateSamplingFormC(samplingFormCHeader);
        ShowSuccessMessage(Messages.FormCUpdateSuccessful);
        //ShowSuccessMessage(SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod ? Messages.FormCUpdateSuccessful + " " + Messages.ValidationIgnoredDueToMigration : Messages.FormCUpdateSuccessful);
        return RedirectToAction("Edit", new { invoiceId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
            GetSummaryCouponLists(samplingFormCHeader);
            samplingFormCHeader.ProvisionalBillingMember =
                _memberManager.GetMember(samplingFormCHeader.ProvisionalBillingMemberId);
            return RedirectToAction("Edit", new { invoiceId });
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }
    }

    //
    // POST: /Pax/FormC/Delete/{provisionalBillingYear}/{provisionalBillingMonth}/{provisionalBillingMemberId}/{fromMemberId}/{listingCurrencyId}/{invoiceStatusId}
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.CreateOrEdit)]
    [HttpPost]
    // SCP155930: FORM C APRIL and MAY 2013
    // Changed datatype of fromMemberId from int to string to carry .Net Invoice Id.
    public JsonResult Delete(int provisionalBillingMonth, int provisionalBillingYear, int provisionalBillingMemberId, string fromMemberId, int? listingCurrencyId, int invoiceStatusId)
    {
      UIMessageDetail details;
      SamplingFormC samplingFormCToBeDeleted;

      var dummyMemberId = _memberManager.GetMemberId(String.IsNullOrEmpty(ConfigurationManager.AppSettings["DummyMembercode"].Trim()) ? "000" : ConfigurationManager.AppSettings["DummyMembercode"].Trim());
      var userId = SessionUtil.AdminUserId;

      try
      {
        //var formC = _samplingFormCManager.GetSamplingFormCDetails(provisionalBillingMonth, provisionalBillingYear, provisionalBillingMemberId, fromMemberId, invoiceStatusId, listingCurrencyId);
        //var isDeleted = _samplingFormCManager.DeleteSamplingFormC(provisionalBillingMonth, provisionalBillingYear, provisionalBillingMemberId, fromMemberId, invoiceStatusId, listingCurrencyId);
        //CMP 400: Before delete FormC. Add entry into INVOICE_DELETION_AUDIT table with invoice number 'FROMC' and update invoice provisional billing member and form member id by dummy member.

        // SCP155930: FORM C APRIL and MAY 2013 
        // we have carried the Net Invoice Id in fromMemberId variable for the delete operation and is passed to the delete function by converting to oracle format Invoice Id.
        var invoiceIdStringInOracleFormat = ConvertUtil.ConvertNetGuidToOracleGuid(Convert.ToString((fromMemberId)));

        var isDeleted = InvoiceManagerBase.DeleteInvoice(invoiceIdStringInOracleFormat, dummyMemberId, userId, 1);

        details = GetDeleteMessage(isDeleted);
      }
     
      catch (ISBusinessException ex)
      {
          details = HandleDeleteException(ex.ErrorCode);
      }

      return Json(details);
    }

    //
    // GET: /Pax/FormC/{invoiceId}/CouponCreate
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.CreateOrEdit)]
    [HttpGet]
    [RestrictUnauthorizedUpdate]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult CouponCreate(string invoiceId)
    {
      if (FormCHeader.NilFormCIndicator == NilFormCIndicatorYes)
      {
        return RedirectToAction("Edit", new { invoiceId = FormCHeader.Id });
      }

      var samplingFormCRecord = new SamplingFormCRecord { SamplingFormC = FormCHeader };

      SetViewDataPageMode(PageMode.Create);
      var sourceCodeList = _referenceManager.GetSourceCodeList(Convert.ToInt32(TransactionType.SamplingFormC));

      // For pre-population of the only source code for Credit Memo.
      if (sourceCodeList.Count != 0)
      {
        samplingFormCRecord.SourceCodeId = sourceCodeList[0].SourceCodeIdentifier;
      }

      return View(samplingFormCRecord);
    }

    //
    // POST: /Pax/FormC/{invoiceId}/CouponCreate
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.CreateOrEdit)]
    [HttpPost]

    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.PAX_FORM_C)]
    public ActionResult CouponCreate(string invoiceId, SamplingFormCRecord samplingFormCRecord)
    {
      var couponAttachmentIds = samplingFormCRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 5 */
          _memberManager.ValidateIssuingAirline(samplingFormCRecord.TicketIssuingAirline);
          samplingFormCRecord.SamplingFormCId = invoiceId.ToGuid();
        samplingFormCRecord.LastUpdatedBy = SessionUtil.UserId;
        
        samplingFormCRecord.Attachments.Clear();

        samplingFormCRecord = _samplingFormCManager.AddSamplingFormCRecord(samplingFormCRecord);
        _samplingFormCManager.UpdateSamplingFormCRecordAttachment(couponAttachmentIds, samplingFormCRecord.Id);

        ShowSuccessMessage(Messages.FormCCouponCreateSuccessful);

        return RedirectToAction("CouponCreate", new { invoiceId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        samplingFormCRecord.SamplingFormC = _samplingFormCManager.GetSamplingFormCDetails(invoiceId);
        samplingFormCRecord.Attachments = _samplingFormCManager.GetSamplingFormCRecordAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        samplingFormCRecord.SamplingFormCId = invoiceId.ToGuid();
        return View(samplingFormCRecord);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Create);
      }
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public ActionResult CouponEdit(string transactionId, string invoiceId)
    {
      var samplingFormCRecord = _samplingFormCManager.GetSamplingFormCRecordDetails(transactionId);
      samplingFormCRecord.LastUpdatedBy = SessionUtil.UserId;
      samplingFormCRecord.SamplingFormC = FormCHeader;

      return View(samplingFormCRecord);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.View)]
    [HttpGet]
    public ActionResult CouponView(string transactionId,
                                   int provisionalBillingMonth,
                                   int provisionalBillingYear,
                                   int provisionalBillingMemberId,
                                   int fromMemberId,
                                   int listingCurrencyId,
                                   int invoiceStatusId)
    {
      var samplingFormCRecord = _samplingFormCManager.GetSamplingFormCRecordDetails(transactionId);
      samplingFormCRecord.SamplingFormC = FormCHeader;

      return View("CouponEdit", samplingFormCRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.PAX_FORM_C)]
    public ActionResult CouponEdit(string invoiceId, string transactionId, SamplingFormCRecord samplingFormCRecord)
    {
      var attachmentIds = samplingFormCRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 5 */
          _memberManager.ValidateIssuingAirline(samplingFormCRecord.TicketIssuingAirline);
          samplingFormCRecord.Id = transactionId.ToGuid();
        samplingFormCRecord.SamplingFormCId = invoiceId.ToGuid();
        samplingFormCRecord.LastUpdatedBy = SessionUtil.UserId;
        _samplingFormCManager.UpdateSamplingFormCRecord(samplingFormCRecord);

        ShowSuccessMessage(Messages.FormCCouponUpdateSuccessful);

        return RedirectToAction("CouponEdit", new { transactionId, invoiceId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        samplingFormCRecord.SamplingFormC = _samplingFormCManager.GetSamplingFormCDetails(invoiceId);
        samplingFormCRecord.Attachments = _samplingFormCManager.GetSamplingFormCRecordAttachments(attachmentIds);
        // SCP190774: ERROR SAVING RM
        samplingFormCRecord.Id = transactionId.ToGuid();
        samplingFormCRecord.SamplingFormCId = invoiceId.ToGuid();
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      return View(samplingFormCRecord);
    }

    /// <summary>
    /// Deletes the coupon.
    /// </summary>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "transactionId", TableName = TransactionTypeTable.PAX_FORM_C_DETAIL)]
    public JsonResult CouponDelete(string transactionId)
    {
      UIMessageDetail details;
      SamplingFormC samplingFormC;
      try
      {
        var isDeleted = _samplingFormCManager.DeleteSamplingFormCRecord(transactionId, out samplingFormC);

        details = isDeleted
                    ? new UIMessageDetail { IsFailed = false, Message = Messages.DeleteSuccessful, isRedirect = true, RedirectUrl = Url.Action("Edit", new { invoiceId = samplingFormC.Id }) }
                    : new UIMessageDetail { IsFailed = true, Message = Messages.DeleteFailed };

        if (details.isRedirect)
        {
          TempData[ViewDataConstants.SuccessMessage] = details.Message;
        }

        return Json(details);
      }
      catch (ISBusinessException ex)
      {
        details = new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.DeleteException, GetDisplayMessage(ex.ErrorCode)) };

        return Json(details);
      }
    }

    /// <summary>
    /// Upload Sampling Form C Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.PAX_FORM_C)]
    public JsonResult CouponAttachmentUpload(string invoiceId, string transactionId)
    {
      var files = string.Empty;
      var attachments = new List<SamplingFormCRecordAttachment>();
      // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015 [Pax]
      var isUploadSuccess = false;
      string message;
      HttpPostedFileBase fileToSave;
      FileAttachmentHelper fileUploadHelper = null;
      try
      {
        foreach (string file in Request.Files)
        {
          isUploadSuccess = false;
          fileToSave = Request.Files[file];
          if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
          {
            continue;
          }

          SamplingFormC formC = _samplingFormCManager.GetSamplingFormCDetails(invoiceId);

          fileUploadHelper = new FileAttachmentHelper
                               {
                                 FileToSave = fileToSave,
                                 FileRelativePath = String.Format("{0}_{1}_{2}", formC.ProvisionalBillingMemberId, formC.ProvisionalBillingYear, formC.ProvisionalBillingMonth)
                               };

          if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
          {
              throw new ISBusinessException(Messages.InvalidFileName);
          }

          if (!fileUploadHelper.ValidateFileExtention(formC.ProvisionalBillingMemberId, BillingCategoryType.Pax))
          {
            throw new ISBusinessException(Messages.InvalidFileExtension);
          }

          if (fileUploadHelper.SaveFile())
          {
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            var attachment = new SamplingFormCRecordAttachment
                               {
                                 Id = fileUploadHelper.FileServerName,
                                 OriginalFileName = fileUploadHelper.FileOriginalName,
                                 // Convert file size to KB.
                                 FileSize = fileUploadHelper.FileToSave.ContentLength,
                                 LastUpdatedBy = SessionUtil.UserId,
                                 ServerId = fileUploadHelper.FileServerInfo.ServerId,
                                 FileStatus = FileStatusType.Received,
                                 FilePath = fileUploadHelper.FileRelativePath
                               };

            attachment = _samplingFormCManager.AddSamplingFormCRecordAttachment(attachment);
            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
            if (attachment.UploadedBy == null)
            {
                attachment.UploadedBy = new Iata.IS.Model.Common.User();
            }
            // assign user info from session and file server info.
            attachment.UploadedBy.Id = SessionUtil.UserId;
            attachment.UploadedBy.FirstName = SessionUtil.Username;
            attachment.FileServer = fileUploadHelper.FileServerInfo;
            isUploadSuccess = true;
            attachments.Add(attachment);
          }
        }
        message = string.Format(Messages.FileUploadSuccessful, files.TrimEnd(','));
      }
      catch (ISBusinessException ex)
      {
        message = string.Format(Messages.FileUploadBusinessException, ex.ErrorCode);
        if (fileUploadHelper != null && isUploadSuccess == false)
        {
          fileUploadHelper.DeleteFile();
        }
      }
      catch (Exception)
      {
        message = Messages.FileUploadUnexpectedError;
        if (fileUploadHelper != null && isUploadSuccess == false)
        {
          fileUploadHelper.DeleteFile();
        }
      }

      return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
    }

    /// <summary>
    /// Download Sampling Form C attachment
    ///  </summary>
    /// <param name="invoiceId"></param>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.Download)]
    [HttpGet]
    public FileStreamResult CouponAttachmentDownload(string invoiceId, string transactionId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _samplingFormCManager.GetSamplingFormCRecordAttachmentDetails(transactionId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    private UIMessageDetail HandleDeleteException(string errorCode)
    {
      return new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.DeleteException, GetDisplayMessage(errorCode)) };
    }

    private static UIMessageDetail GetDeleteMessage(bool isDeleted)
    {
      return isDeleted ? new UIMessageDetail { IsFailed = false, Message = Messages.DeleteSuccessful } : new UIMessageDetail { IsFailed = true, Message = Messages.DeleteFailed };
    }

    //SCP419601 PAX permissions issue [Changes permission]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.ManageFormC.Query)]
    [HttpGet]
    public ActionResult Index(SearchCriteria searchCriteria)
    {
      // Note : For first time or default search criteria status is passed as '0'.
      // Stored Procedure is so written that when Status is 0 in Search Criteria, Open and Error status will be returned.
      // when status is -1 in search criteria, Status clause is ignored.
      // any other value is treated as valid Invoice Status Id and comparison is made on that basis.
      searchCriteria.BillingMemberId = SessionUtil.MemberId;

        

      // TODO: Set From/Billed member id
      string criteria = new JavaScriptSerializer().Serialize(searchCriteria);

      var formCSearchGrid = new FormCSearchGrid(ControlIdConstants.FormCSearchGrid, Url.Action(GetFormCSearchResultAction, new { criteria }));
      ViewData[ViewDataConstants.FormCSearchResults] = formCSearchGrid.Instance;

      ViewData[ViewDataConstants.RejectionOnValidationFlag] = GetRejectionOnValidationFailureFlag();

      try
      {
          if (searchCriteria != null && !string.IsNullOrEmpty(searchCriteria.BilledMemberText) && searchCriteria.BilledMemberId == 0)
          {
              throw new ISBusinessException(ErrorCodes.InvalidMemberType);
          }
      }
      catch (ISBusinessException businessException)
      {
          return View("FormCSearch", new SearchCriteria());
      }

      return View("FormCSearch", searchCriteria);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormC.View)]
    [HttpGet]
    public ActionResult PayablesSearch(SearchCriteria searchCriteria)
    {
      searchCriteria.BillingMemberId = SessionUtil.MemberId;

      string criteria = new JavaScriptSerializer().Serialize(searchCriteria);

      var formCSearchGrid = new FormCPayablesSearchGrid(ControlIdConstants.FormCSearchGrid, Url.Action(GetFormCPayablesSearchResultAction, new { criteria }));
      ViewData[ViewDataConstants.FormCSearchResults] = formCSearchGrid.Instance;

      return View("FormCPayablesSearch", searchCriteria);
    }

    //SCP419601 PAX permissions issue [Changes permission]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.ManageFormC.Query)]
    public JsonResult GetFormCSearchResults(string criteria)
    {
      SearchCriteria searchCriteria = GetSearchCriteria(criteria);
      // Create grid instance and retrieve data from database
      var formCSearchGrid = new FormCSearchGrid(ControlIdConstants.FormCSearchGrid, Url.Action(GetFormCSearchResultAction, new { searchCriteria }));
      var formCSearchResults = _samplingFormCManager.GetSamplingFormCList(searchCriteria);

      return formCSearchGrid.DataBind(formCSearchResults.AsQueryable());
    }

    public JsonResult GetFormCPayablesSearchResults(string criteria)
    {
      SearchCriteria searchCriteria = GetSearchCriteria(criteria);
      // Create grid instance and retrieve data from database
      var formCSearchGrid = new FormCPayablesSearchGrid(ControlIdConstants.FormCSearchGrid, Url.Action(GetFormCPayablesSearchResultAction, new { criteria }));
      var formCSearchResults = _samplingFormCManager.GetSamplingFormCPayablesList(searchCriteria);

      return formCSearchGrid.DataBind(formCSearchResults.AsQueryable());
    }

    private SearchCriteria GetSearchCriteria(string criteria)
    {
      var searchCriteria = new SearchCriteria();

      if (Request.UrlReferrer != null)
      {
        SessionUtil.FormCSearchCriteria = Request.UrlReferrer.ToString();
      }

      if (!string.IsNullOrEmpty(criteria))
      {
        searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(SearchCriteria)) as SearchCriteria;
      }

      if (searchCriteria != null)
      {
        searchCriteria.BillingMemberId = SessionUtil.MemberId;
      }
      return searchCriteria;
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.Submit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "transactionId", IsJson = true,InvList = true, TableName = TransactionTypeTable.PAX_FORM_C)]
    public JsonResult SubmitFormC(string transactionId, int alreadySubmittedInvCount = 0, int alreadyDeletedInvCount = 0)
    {
        #region old code

        /* 
      UIMessageDetail details;
      // Message = Messages.FormCNotSelected
      if (string.IsNullOrEmpty(transactionId))
      {
        details = new UIMessageDetail { IsFailed = true, Message = Messages.FormCNotSelected };
        return Json(details);
      }

      var formCRecord = transactionId.Split(new[] { ',' });
      var samplingFormCDetailsList =
        formCRecord.Select(t => t.Split(new[] { '-' })).Select(
          formCFields =>
          new SamplingFormCResultSet
            {
              ProvisionalBillingYear = Convert.ToInt32(formCFields[0]),
              ProvisionalBillingMonth = Convert.ToInt32(formCFields[1]),
              ProvisionalBillingMemberId = Convert.ToInt32(formCFields[2]),
              FromMemberId = Convert.ToInt32(formCFields[3]),
              ListingCurrencyId = string.IsNullOrEmpty(formCFields[4]) ? (int?)null : Convert.ToInt32(formCFields[4]),
              InvoiceStatusId = Convert.ToInt32(formCFields[5])
            }).ToList();

      var submittedSamplingFormCDetails = _samplingFormCManager.SubmitSamplingFormC(samplingFormCDetailsList);

      details = submittedSamplingFormCDetails.Count > 0 ? new UIMessageDetail { IsFailed = false, Message = string.Format(Messages.FormCSubmittedCount, submittedSamplingFormCDetails.Count) }
        : new UIMessageDetail { IsFailed = true, Message = Messages.FormCIneligibleForSubmission };

      return Json(details);
      */

        #endregion
        UIMessageDetail details;
        // If user has not selected formC to submit display message
        // CMP400: handling deleted invoices also by using alreadyDeletedInvCount
        if (!string.IsNullOrEmpty(transactionId))
        {
            var formCRecord = transactionId.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                //foreach (String formCId in formCRecord)
                //{
                //    var samplingFormCRecord = _samplingFormCManager.GetSamplingFormCDetails(formCRecord[0]);
                //}
                //var samplingFormCDetailsList =
                //    formCRecord.Select(t => t.Split(new[] {'-'})).Select(
                //        formCFields =>
                //        new SamplingFormCResultSet
                //            {
                //                ProvisionalBillingYear = Convert.ToInt32(formCFields[0]),
                //                ProvisionalBillingMonth = Convert.ToInt32(formCFields[1]),
                //                ProvisionalBillingMemberId = Convert.ToInt32(formCFields[2]),
                //                FromMemberId = Convert.ToInt32(formCFields[3]),
                //                ListingCurrencyId =
                //                    string.IsNullOrEmpty(formCFields[4]) ? (int?) null : Convert.ToInt32(formCFields[4]),
                //                InvoiceStatusId = Convert.ToInt32(formCFields[5])
                //            }).ToList();

                var submittedSamplingFormCDetails = _samplingFormCManager.SubmitSamplingFormC(formCRecord.ToList());

                if (submittedSamplingFormCDetails.Count > 0)
                {
                    if (alreadySubmittedInvCount > 0 && alreadyDeletedInvCount > 0)
                    {
                        details = new UIMessageDetail
                        {
                            IsFailed = false,
                            Message =
                                string.Format(
                                    "{0} FormC(s) submitted successfully. {1} FormC(s) already submitted. {2} FormC(s) already deleted.",
                                    submittedSamplingFormCDetails.Count, alreadySubmittedInvCount, alreadyDeletedInvCount)
                        };
                    }
                    else if (alreadySubmittedInvCount > 0)
                    {
                        details = new UIMessageDetail
                        {
                            IsFailed = false,
                            Message =
                                string.Format(
                                    "{0} FormC(s) submitted successfully. {1} FormC(s) already submitted..",
                                    submittedSamplingFormCDetails.Count, alreadySubmittedInvCount)
                        };

                    }
                    else if (alreadyDeletedInvCount > 0)
                    {
                        details = new UIMessageDetail
                        {
                            IsFailed = false,
                            Message =
                                string.Format(
                                    "{0} FormC(s) submitted successfully. {1} FormC(s) already deleted..",
                                    submittedSamplingFormCDetails.Count, alreadyDeletedInvCount)
                        };

                    }
                    else
                    {
                        details = new UIMessageDetail
                                      {
                                          IsFailed = false,
                                          Message =
                                              string.Format(Messages.FormCSubmittedCount,
                                                            submittedSamplingFormCDetails.Count)
                                      };
                    }
                }
                else
                {
                    if (alreadySubmittedInvCount > 0 && alreadyDeletedInvCount > 0)
                    {
                        details = new UIMessageDetail
                        {
                            IsFailed = true,
                            Message =
                                string.Format(
                                    Messages.FormCIneligibleForSubmission +
                                    " {0} FormC(s) already submitted. {1} FormC(s) already deleted.", alreadySubmittedInvCount, alreadyDeletedInvCount)
                        };
                    }
                    else if(alreadyDeletedInvCount > 0)
                    {
                        details = new UIMessageDetail
                        {
                            IsFailed = true,
                            Message =
                                string.Format(
                                    Messages.FormCIneligibleForSubmission +
                                    " {0} FormC(s) already deleted.", alreadyDeletedInvCount)
                        };
                    }
                    else if (alreadySubmittedInvCount > 0)
                    {
                        details = new UIMessageDetail
                                      {
                                          IsFailed = true,
                                          Message =
                                              string.Format(
                                                  Messages.FormCIneligibleForSubmission +
                                                  " {0} FormC(s) already submitted.", alreadySubmittedInvCount)
                                      };
                    }
                    else
                    {
                        details = new UIMessageDetail {IsFailed = true, Message = Messages.FormCIneligibleForSubmission};
                    }
                }
                return Json(details);

            }
            catch (ISBusinessException exception)
            {
                details = new UIMessageDetail
                              {IsFailed = true, Message = GetDisplayMessageWithErrorCode(exception.ErrorCode)};

                return Json(details);
            }
        }
        else
        {
            if (alreadySubmittedInvCount > 0)
            {
                details = new UIMessageDetail
                              {IsFailed = true, Message = "FormC(s) already submitted. Please refresh your page."};
            }
            else
            {
                details = new UIMessageDetail {IsFailed = true, Message = Messages.FormCNotSelected};
            }

            return Json(details);

        }
    }

      [HttpPost]
    public JsonResult PresentFormC(string transactionId, SearchCriteria searchCriteria)
    {
      UIMessageDetail details;

      if (string.IsNullOrEmpty(transactionId))
      {
        details = new UIMessageDetail { IsFailed = true, Message = Messages.FormCNotSelected };
        return Json(details);
      }

      var formCRecord = transactionId.Split(new[] { ',' });
      var samplingFormCDetailsList =
        formCRecord.Select(t => t.Split(new[] { '-' })).Select(
          formCFields =>
          new SamplingFormCResultSet
          {
            ProvisionalBillingYear = Convert.ToInt32(formCFields[0]),
            ProvisionalBillingMonth = Convert.ToInt32(formCFields[1]),
            ProvisionalBillingMemberId = Convert.ToInt32(formCFields[2]),
            FromMemberId = Convert.ToInt32(formCFields[3]),
            ListingCurrencyId = string.IsNullOrEmpty(formCFields[4]) ? (int?)null : Convert.ToInt32(formCFields[4]),
            InvoiceStatusId = Convert.ToInt32(formCFields[5])
          }).ToList();

      var submittedSamplingFormCDetails = _samplingFormCManager.PresentSamplingFormC(samplingFormCDetailsList);

      if (submittedSamplingFormCDetails.Count > 0)
      {
        details = new UIMessageDetail { IsFailed = false, Message = string.Format(Messages.FormCPresentedCount, submittedSamplingFormCDetails.Count) };
      }
      else
      {
        details = new UIMessageDetail { IsFailed = true, Message = Messages.FormCIneligibleForPresentation };
      }

      return Json(details);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.View)]
    [HttpPost]
    public JsonResult GetLinkedCouponDetails(int fromMemberId,
                                             int provisionalBillingMemberId,
                                             int provisionalBillingMonth,
                                             int provisionalBillingYear,
                                             string ticketIssuingAirline,
                                             long documentNumber,
                                             int couponNumber,
                                             string listingCurrency)
    {
      var linkedCouponDetails = _samplingFormCManager.GetLinkedCouponDetails(fromMemberId,
                                                                             provisionalBillingMemberId,
                                                                             provisionalBillingMonth,
                                                                             provisionalBillingYear,
                                                                             ticketIssuingAirline,
                                                                             documentNumber,
                                                                             couponNumber,
                                                                             listingCurrency);
      return Json(linkedCouponDetails);
    }

    // Called on Edit Invoice header page 
    public JsonResult GetSubmittedErrors(string invoiceId)
    {
      var submittedErrors = ValidationManager.GetValidationErrors(invoiceId);
      var submittedErrorsGrid = new SubmittedErrorsGrid(ControlIdConstants.SubmittedErrorsGridId, Url.Action("GetSubmittedErrors", new { invoiceId }));
      return submittedErrorsGrid.DataBind(submittedErrors);
    }

    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormC.Download)]
    public JsonResult DownloadZip(string id, string options)  //, string zipFileName)
    {
      UIMessageDetail details;
      try
      {
        //SCP334940: SRM Exception occurred in Iata.IS.Service.Iata.IS.Service.OfflineCollectionDownloadService. - SIS Production
        var memberId = _samplingFormCManager.GetSamplingFormCHeaderDetails(id).FromMemberId;

        if (SessionUtil.UserId > 0 && SessionUtil.MemberId > 0 && memberId == SessionUtil.MemberId)
        {
          var zipFileName = Guid.NewGuid().ToString();
          //var formCParams = id.Split(new[] { '-' });

          var billingType = Request.RequestContext.RouteData.Values["billingType"];
          bool isBillingTypeReceivable = true;
          if (billingType != null && billingType.ToString() == Util.BillingType.Payables) isBillingTypeReceivable = false;

          var downloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
                                                                   "Invoice",
                                  new
                                  {
                                    area = "Pax",
                                    billingType = "Receivables"
                                  }));
          //int? listingCurrencyId = string.IsNullOrEmpty(formCParams[4]) ? (int?)null : Convert.ToInt32(formCParams[4]);
          //var samplingFormC = _samplingFormCManager.GetSamplingFormCDetails(Convert.ToInt32(formCParams[1]),
          //                                                                    Convert.ToInt32(formCParams[0]),
          //                                                                    Convert.ToInt32(formCParams[2]),
          //                                                                    Convert.ToInt32(formCParams[3]),
          //                                                                    Convert.ToInt32(formCParams[5]),
          //                                                                    listingCurrencyId
          //                                                                    );
          var iInvoiceOfflineCollectionManager = Ioc.Resolve<IInvoiceOfflineCollectionManager>(typeof(IInvoiceOfflineCollectionManager));
          IDictionary<string, string> messages = new Dictionary<string, string>
                                                   {
                                                      { "RECORD_ID", ConvertUtil.ConvertGuidToString(id.ToGuid())},
                                                      { "USER_ID", SessionUtil.UserId.ToString() },
                                                      { "OPTIONS", options},
                                                      { "IS_FORM_C", "1" },
                                                      { "IS_RECEIVABLE", isBillingTypeReceivable? "1" :"0" },
                                                      { "OUTPUT_ZIP_FILE_NAME", zipFileName },
                                                      { "DOWNLOAD_URL", downloadUrl },
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

    [HttpPost]
    public JsonResult GetFormABListingCurrency(int provisionalBillingMemberId, int fromMemberId, int provisionalBillingMonth, int provisionalBillingYear)
    {
      var samplingFormC = new SamplingFormC
      {
        FromMemberId = fromMemberId,
        ProvisionalBillingMemberId = provisionalBillingMemberId,
        ProvisionalBillingMonth = provisionalBillingMonth,
        ProvisionalBillingYear = provisionalBillingYear
      };

      int? listingCurrencyId = _samplingFormCManager.GetFormABListingCurrency(samplingFormC);

      return Json(listingCurrencyId);
    }

    private RejectionOnValidationFailure GetRejectionOnValidationFailureFlag()
    {
      var paxConfig = _memberManager.GetPassengerConfiguration(SessionUtil.MemberId);
      return paxConfig != null ? paxConfig.RejectionOnValidationFailure : RejectionOnValidationFailure.RejectInvoiceInError;
    }
  }
}
