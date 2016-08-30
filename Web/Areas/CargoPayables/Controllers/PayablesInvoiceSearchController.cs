using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Model.Cargo;
using Iata.IS.Business.MemberProfile;
using System.Web.Script.Serialization;
using Iata.IS.Web.UIModel.Grid.Cargo;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Business.Common;
using Iata.IS.Business.Cargo;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Business.Cargo;
using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Business;
using Iata.IS.Core.Exceptions;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using System.IO;
using log4net;
using System.Reflection;
//using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Model.Enums;


namespace Iata.IS.Web.Areas.CargoPayables.Controllers
{
    public class PayablesInvoiceSearchController : ISController
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICalendarManager _calendarManager;
        private readonly ISearchInvoiceManager _searchInvoiceManager;
        private const string SearchResultGridAction = "SearchResultGridData";
        private readonly ICargoInvoiceManager _invoiceManager;

       
        public PayablesInvoiceSearchController
            (
                ISearchInvoiceManager searchInvoiceManager,
                ICalendarManager calenderManager,
                ICargoInvoiceManager invoiceManager
            )
        {
            _calendarManager = calenderManager;
            _searchInvoiceManager = searchInvoiceManager;
            _invoiceManager=invoiceManager;
        }
        #region "Old Code"
        //public ActionResult ManageInvoice(PayableSearch payableSearch)
        //{
        //    try
        //    {
        //        PayableSearch obj = new PayableSearch();
        //        ViewData[ViewDataConstants.PageMode] = PageMode.View;
        //        string criteria = payableSearch != null ? new JavaScriptSerializer().Serialize(payableSearch) : string.Empty;
        //        var payableSrGrid = new PayableInvoiceSearchGrid(ControlIdConstants.PayableInvoiceSearchGrid, Url.Action(SearchResultGridAction, new { Area = "Cargo", criteria }));
        //        ViewData[ViewDataConstants.PayableInvoiceSearchGrid] = payableSrGrid.Instance;
        //        ViewData[ViewDataConstants.RejectionOnValidationFlag] = GetRejectionOnValidationFailureFlag();
        //        ViewData[ViewDataConstants.BillingType] = Iata.IS.Web.Util.BillingType.Payables;
                
        //    }
        //    catch (Exception exception)
        //    {
        //        ShowErrorMessage(exception.Message);
        //    }

        //    return View(payableSearch);
        //}
        #endregion
        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Search.Query)]
        public ActionResult Index(SearchCriteria searchCriteria)
        {
            try
            {
                if (searchCriteria != null)
                    searchCriteria.BillingPeriod = searchCriteria.BillingPeriod == 0 ? _calendarManager.GetLastClosedBillingPeriod().Period : searchCriteria.BillingPeriod;

                string criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;

                var invoiceSearchGrid = new PayableInvoiceSearchGrid(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction, new
                {
                    area = "CargoPayables",
                    criteria
                }));
                ViewData[ViewDataConstants.SearchGrid] = invoiceSearchGrid.Instance;
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
            }

            return View(searchCriteria);
        }

        #region "Old Code"
        //// Display the data in Grid
        //public JsonResult SearchResultGridData_Test123(string criteria)
        //{
        //    var searchCriteria = new PayableSearch();

        //    if (Request.UrlReferrer != null)
        //    {
        //        SessionUtil.InvoiceSearchCriteria = Request.UrlReferrer.ToString();
        //        SessionUtil.CGOCorrSearchCriteria = null;
        //        SessionUtil.CGOInvoiceSearchCriteria = null;
        //    }

        //    if (!string.IsNullOrEmpty(criteria))
        //    {
        //        searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(PayableSearch)) as PayableSearch;
        //    }
        //    searchCriteria.BillingPeriod = searchCriteria.BillingPeriod == 0 ? _calendarManager.GetCurrentBillingPeriod().Period : searchCriteria.BillingPeriod;

        //    // Create grid instance and retrieve data from database
        //    var invoiceSearchGrid = new PayableInvoiceSearchGrid(ControlIdConstants.PayableInvoiceSearchGrid, Url.Action(SearchResultGridAction, new { area = "Cargo", searchCriteria }));

        //    // add billing member id to search criteria.
        //    searchCriteria.BillingMemberId = SessionUtil.MemberId;
        //    // If Owner Id in searchCriteria is not set, then default to current user id.
        //    if (searchCriteria.OwnerId == 0)
        //    {
        //        searchCriteria.OwnerId = SessionUtil.UserId;
        //    }
        //    var invoiceSearchedData = _searchInvoiceManager.GetInvoices(searchCriteria).AsQueryable();
        //    return invoiceSearchGrid.DataBind(invoiceSearchedData);
        //}
          #endregion
        /// <summary>
        /// Fetch invoice searched result and display it in grid
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Search.Query)]
        public JsonResult SearchResultGridData(string criteria)
        {
            var searchCriteria = new SearchCriteria();

            if (Request.UrlReferrer != null)
            {
                SessionUtil.InvoiceSearchCriteria = Request.UrlReferrer.ToString();
                SessionUtil.CGOCorrSearchCriteria = null;
                SessionUtil.CGOInvoiceSearchCriteria = null;
            }

            if (!string.IsNullOrEmpty(criteria))
            {
                searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(SearchCriteria)) as SearchCriteria;
            }

            if (searchCriteria != null)
            {
                searchCriteria.BillingYear = searchCriteria.BillingYear == 0 ? _calendarManager.GetLastClosedBillingPeriod().Year : searchCriteria.BillingYear;
                searchCriteria.BillingMonth = searchCriteria.BillingMonth == 0 ? _calendarManager.GetLastClosedBillingPeriod().Month : searchCriteria.BillingMonth;
                searchCriteria.BillingPeriod = searchCriteria.BillingPeriod == 0 ? _calendarManager.GetLastClosedBillingPeriod().Period : searchCriteria.BillingPeriod;
            }

            // Create grid instance and retrieve data from database
            var invoiceSearchGrid = new PayableInvoiceSearchGrid(ControlIdConstants.SearchGrid,
                                                          Url.Action(SearchResultGridAction,
                                                                     new
                                                                     {
                                                                         area = "CargoPayables",
                                                                         searchCriteria
                                                                     }));

            // add billed member id to search criteria.
            searchCriteria.BilledMemberId = SessionUtil.MemberId;
            var invoiceSearchedData = _searchInvoiceManager.GetAllPayables(searchCriteria).AsQueryable();

            return invoiceSearchGrid.DataBind(invoiceSearchedData);
        }
        #region "Old Code"
        //[HttpPost]
       // public ActionResult SubmitInvoices(string id)
       // {
       //     UIMessageDetail messageDetails = new UIMessageDetail();

       //     //// If user has not selected Invoices to submit display message
       //     //if (id != null)
       //     //{
       //     //    var invoiceIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

       //     //    try
       //     //    {
       //     //        //var submittedInvoices = _invoiceManager.SubmitInvoices(invoiceIdList.ToList());
       //     //        //if (submittedInvoices.Count > 0)
       //     //        //{
       //     //        //    messageDetails = new UIMessageDetail { IsFailed = false, Message = string.Format(Messages.InvoicesSubmittedCount, submittedInvoices.Count) };
       //     //        //}
       //     //        //else
       //     //        //{
       //     //        //    messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.InvoiceIneligibleForSubmission };
       //     //        //}
       //     //        messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.InvoiceIneligibleForSubmission };
       //     //        return Json(messageDetails);
       //     //    }
       //     //    catch (ISBusinessException exception)
       //     //    {
       //     //        messageDetails = new UIMessageDetail { IsFailed = true, Message = GetDisplayMessageWithErrorCode(exception.ErrorCode) };

       //     //        return Json(messageDetails);
       //     //    }
       //     //}
       //     //else
       //     //{
       //     //    messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.SelectInvoiceForSubmission };
       //     //    return Json(messageDetails);
       //     // }
       //     messageDetails = new UIMessageDetail { IsFailed = false, Message = "Saved Successfully" };
       //     return Json(messageDetails);
       // }
       // /// <summary>
       // /// This action method calls Invoice Edit page
       // /// </summary>
       // /// <returns></returns>
       // public ActionResult EditInvoice(string id)
       // {
       //     var invoice = _invoiceManager.GetInvoiceHeaderDetails(id);

       //     string controller = "PayablesInvoiceSearch";

       //     ViewData[ViewDataConstants.PageMode] = PageMode.Edit;
       //     return RedirectToAction("Edit", controller, new { area = "Cargo", invoiceId = id });
       // }
       // [HttpPost]
       // public JsonResult ValidateInvoice(string id)
       // {
       //     // TODO: This method needs refactoring
       //     UIMessageDetail details;
       //     var invoice = _invoiceManager.GetInvoiceHeaderDetails(id);

       //     try
       //     {
       //         //invoice = invoice.InvoiceType == InvoiceType.Invoice ? _nonSamplingInvoiceManager.ValidateInvoice(id) : _nonSamplingCreditNoteManager.ValidateInvoice(id);
       //         invoice = _invoiceManager.ValidateInvoice(id);

       //         details = invoice.InvoiceStatus == InvoiceStatusType.ReadyForSubmission
       //                     ? new UIMessageDetail { IsFailed = false, Message = string.Format(Messages.InvoiceValidateSuccessful, invoice.InvoiceNumber) }
       //                     : new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.InvoiceValidateFailed, invoice.InvoiceNumber) };
       //     }
       //     catch (ISBusinessException)
       //     {
       //         details = new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.InvoiceValidateFailed, invoice.InvoiceNumber) };
       //     }
       //     // details = new UIMessageDetail {};
       //     return Json(details);
       // }
     #endregion

        //public ActionResult ViewInvoice(string id)
        //{
        //    var invoice = _invoiceManager.GetInvoiceHeaderDetails(id);

        //    string controller = "Invoice";

        //    return RedirectToAction("View", controller, new { area = "Cargo", invoiceId = id });
        //}
      #region "Old Code"
        ///// <summary>
        ///// This action method calls invoice delete action method
        ///// </summary>
        //[HttpPost]
        //public JsonResult DeleteInvoice(string id)
        //{
        //    UIExceptionDetail details;

        //    var invoice = _invoiceManager.GetInvoiceHeaderDetails(id);
        //    try
        //    {
        //        //Delete record
        //        bool isDeleted = _invoiceManager.DeleteInvoice(id);

        //        details = isDeleted
        //                    ? new UIExceptionDetail { IsFailed = false, Message = string.Format(Messages.InvoiceDeleteSuccessful, invoice.InvoiceNumber) }
        //                    : new UIExceptionDetail { IsFailed = true, Message = string.Format(Messages.InvoiceDeleteFailed, invoice.InvoiceNumber) };
        //    }
        //    catch (ISBusinessException)
        //    {
        //        details = new UIExceptionDetail { IsFailed = true, Message = string.Format(Messages.InvoiceDeleteFailed, invoice.InvoiceNumber) };
        //    }
        //    //details = new UIExceptionDetail {};
        //    return Json(details);
        //}
   #endregion
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

        [HttpGet]
        public ActionResult ViewInvoice(string id)
        {
            //SCPID : 325374 - File Loading & Web Response Stats -PayablesInvoiceSearch
            var invoice = _invoiceManager.GetInvoiceDetails(id);

            string controller = "InvoicePayables";

            switch (invoice.BillingCode)
            {
              case (int)BillingCode.AWBPrepaid:
                    controller = "InvoicePayables";
                break;
              case (int)BillingCode.AWBChargeCollect:
                controller = invoice.InvoiceType == InvoiceType.Invoice ? "InvoicePayables" : "CreditNote";
                break;
              case (int)BillingCode.BillingMemo:
                controller = "InvoicePayables";
                break;
              case (int)BillingCode.CreditNote:
                controller = "CreditNotePayables";
                break;
              case (int)BillingCode.RejectionMemo:
                controller = "InvoicePayables";
                break;
              default:
                controller = invoice.InvoiceType == InvoiceType.Invoice ? "InvoicePayables" : "CreditNotePayables";
                break;
            }

            return RedirectToAction("View", controller, new { area = "Cargo", invoiceID = id, BillingType = "Payables" });
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
                  int memberId = _invoiceManager.GetInvoiceDetails(id).BilledMemberId;

                  if (SessionUtil.UserId > 0 && SessionUtil.MemberId > 0 && memberId == SessionUtil.MemberId)
                  {
                    var downloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
                                                                           "Invoice",
                                                                           new
                                                                           {
                                                                             area = "Cargo",
                                                                             billingType = "Payables"
                                                                           }));

                    var iInvoiceOfflineCollectionManager = Ioc.Resolve<IInvoiceOfflineCollectionManager>(typeof(IInvoiceOfflineCollectionManager));
                    IDictionary<string, string> messages = new Dictionary<string, string>
                                                   {
                                                      { "RECORD_ID", ConvertUtil.ConvertGuidToString(invoiceId)},
                                                      { "USER_ID", SessionUtil.UserId.ToString() },
                                                      { "OPTIONS", options},
                                                      { "IS_FORM_C", "0" },
                                                      { "IS_RECEIVABLE", "0" },
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
        //private RejectionOnValidationFailure GetRejectionOnValidationFailureFlag()
        //{
        //    var rejectionFlag = RejectionOnValidationFailure.RejectInvoiceInError;
        //    var rejectionFlagId = MemberManager.GetMemberConfigurationValue(SessionUtil.MemberId, MemberConfigParameter.CargoRejectionOnValidationFailure);
        //    if (!string.IsNullOrEmpty(rejectionFlagId))
        //        rejectionFlag = (RejectionOnValidationFailure)Convert.ToInt32(rejectionFlagId);
        //    return rejectionFlag;
        //} 

    }
}
