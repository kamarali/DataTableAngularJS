using System.Web;
using System.Web.Mvc;
using Iata.IS.AdminSystem;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core.DI;
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
  public class FormCPayablesController : ISController
  {
    private readonly ISamplingFormCManager _samplingFormCManager;
    private readonly IReferenceManager _referenceManager;
    private readonly IMemberManager _memberManager;
    public IInvoiceOfflineCollectionDownloadManager InvoiceOfflineCollectionDownloadManager { get; set; }

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

    public FormCPayablesController(ISamplingFormCManager samplingFormCManager, IReferenceManager referenceManager, IMemberManager memberManager)
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
          invoiceStatus == InvoiceStatusType.OnHold)
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

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormC.View)]
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

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormC.View)]
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
    /// Download Sampling Form C attachment
    ///  </summary>
    /// <param name="invoiceId"></param>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormC.Download)]
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

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SearchSampleFormC.Query)]
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

      return View("FormCSearch", searchCriteria);
    }

    //SCP419601: PAX permissions issue
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SearchSampleFormC.Query)]
    [HttpGet]
    public ActionResult PayablesSearch(SearchCriteria searchCriteria)
    {
      searchCriteria.BillingMemberId = SessionUtil.MemberId;

      string criteria = new JavaScriptSerializer().Serialize(searchCriteria);

      var formCSearchGrid = new FormCPayablesSearchGrid(ControlIdConstants.FormCSearchGrid, Url.Action(GetFormCPayablesSearchResultAction, new { criteria }));
      ViewData[ViewDataConstants.FormCSearchResults] = formCSearchGrid.Instance;

      return View("FormCPayablesSearch", searchCriteria);
    }

    //SCP419601: PAX permissions issue
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SearchSampleFormC.Query)]
    public JsonResult GetFormCSearchResults(string criteria)
    {
      SearchCriteria searchCriteria = GetSearchCriteria(criteria);
      // Create grid instance and retrieve data from database
      var formCSearchGrid = new FormCSearchGrid(ControlIdConstants.FormCSearchGrid, Url.Action(GetFormCSearchResultAction, new { searchCriteria }));
      var formCSearchResults = _samplingFormCManager.GetSamplingFormCList(searchCriteria);

      return formCSearchGrid.DataBind(formCSearchResults.AsQueryable());
    }

    //SCP419601: PAX permissions issue
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SearchSampleFormC.Query)]
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

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormC.View)]
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
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormC.Download)]
    public JsonResult DownloadZip(string id, string options)  //, string zipFileName)
    {
      UIMessageDetail details;
      try
      {
          var zipFileName = Guid.NewGuid().ToString();
          var formCParams = id.Split(new[] { '-' });

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
          int? listingCurrencyId = string.IsNullOrEmpty(formCParams[4]) ? (int?)null : Convert.ToInt32(formCParams[4]);
          var samplingFormC = _samplingFormCManager.GetSamplingFormCDetails(Convert.ToInt32(formCParams[1]),
                                                                              Convert.ToInt32(formCParams[0]),
                                                                              Convert.ToInt32(formCParams[2]),
                                                                              Convert.ToInt32(formCParams[3]),
                                                                              Convert.ToInt32(formCParams[5]),
                                                                              listingCurrencyId
                                                                              );
          //SCP334940: SRM Exception occurred in Iata.IS.Service.Iata.IS.Service.OfflineCollectionDownloadService. - SIS Production
          if (SessionUtil.UserId > 0 && SessionUtil.MemberId > 0 && samplingFormC.ProvisionalBillingMemberId == SessionUtil.MemberId)
          {
            var iInvoiceOfflineCollectionManager = Ioc.Resolve<IInvoiceOfflineCollectionManager>(typeof(IInvoiceOfflineCollectionManager));
            IDictionary<string, string> messages = new Dictionary<string, string>
                                                   {
                                                      { "RECORD_ID", ConvertUtil.ConvertGuidToString(samplingFormC.Id)},
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
