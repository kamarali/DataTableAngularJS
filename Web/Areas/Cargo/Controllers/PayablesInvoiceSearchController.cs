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
using Iata.IS.Model.MemberProfile;
using Iata.IS.Business.Common;
using Iata.IS.Business.Cargo;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Business.Pax;
using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Business;
using Iata.IS.Core.Exceptions;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using System.IO;
using log4net;
using System.Reflection;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Enums;


namespace Iata.IS.Web.Areas.Cargo.Controllers
{
    public class PayablesInvoiceSearchController : ISController
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICalendarManager _calendarManager;
        private readonly Iata.IS.Business.Cargo.IPayableInvoiceSearchManager _searchInvoiceManager;
        private const string SearchResultGridAction = "SearchResultGridData";
        private readonly ICargoInvoiceManager _invoiceManager;
         

        public IMemberManager MemberManager
        {
            get;
            set;
        }
        
        //Constructor to initialize the DataMembers
        public PayablesInvoiceSearchController
            (
                Iata.IS.Business.Cargo.IPayableInvoiceSearchManager searchInvoiceManager,
                ICalendarManager calenderManager,
                ICargoInvoiceManager invoiceManager
            )
        {
            _calendarManager = calenderManager;
            _searchInvoiceManager = searchInvoiceManager;
            _invoiceManager=invoiceManager;
        }

        public ActionResult ManageInvoice(PayableSearch payableSearch)
        {
            try
            {
                PayableSearch obj = new PayableSearch();
                ViewData[ViewDataConstants.PageMode] = PageMode.View;
                string criteria = payableSearch != null ? new JavaScriptSerializer().Serialize(payableSearch) : string.Empty;
                var payableSrGrid = new PayableInvoiceSearchGrid(ControlIdConstants.PayableInvoiceSearchGrid, Url.Action(SearchResultGridAction, new { Area = "Cargo", criteria }));
                ViewData[ViewDataConstants.PayableInvoiceSearchGrid] = payableSrGrid.Instance;
                ViewData[ViewDataConstants.RejectionOnValidationFlag] = GetRejectionOnValidationFailureFlag();
                ViewData[ViewDataConstants.BillingType] = Iata.IS.Web.Util.BillingType.Payables;
                
            }
            catch (Exception exception)
            {
                ShowErrorMessage(exception.Message);
            }

            return View(payableSearch);
        }

        // Display the data in Grid
        public JsonResult SearchResultGridData(string criteria)
        {
            var searchCriteria = new PayableSearch();

            if (Request.UrlReferrer != null)
            {
                SessionUtil.InvoiceSearchCriteria = Request.UrlReferrer.ToString();
                SessionUtil.CGOCorrSearchCriteria = null;
                SessionUtil.CGOInvoiceSearchCriteria = null;
            }

            if (!string.IsNullOrEmpty(criteria))
            {
                searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(PayableSearch)) as PayableSearch;
            }

            searchCriteria.BillingPeriod = searchCriteria.BillingPeriod == 0 ? _calendarManager.GetCurrentBillingPeriod().Period : searchCriteria.BillingPeriod;

            // Create grid instance and retrieve data from database
            var invoiceSearchGrid = new PayableInvoiceSearchGrid(ControlIdConstants.PayableInvoiceSearchGrid, Url.Action(SearchResultGridAction, new { area = "Cargo", searchCriteria }));

            // add billing member id to search criteria.
            searchCriteria.BillingMemberId = SessionUtil.MemberId;

            // If Owner Id in searchCriteria is not set, then default to current user id.
            if (searchCriteria.OwnerId == 0)
            {
                searchCriteria.OwnerId = SessionUtil.UserId;
            }

            var invoiceSearchedData = _searchInvoiceManager.GetInvoices(searchCriteria).AsQueryable();
            
            
            return invoiceSearchGrid.DataBind(invoiceSearchedData);

        }

        //================================================================

       [HttpPost]
        public ActionResult SubmitInvoices(string id)
        {
            UIMessageDetail messageDetails = new UIMessageDetail();

            //// If user has not selected Invoices to submit display message
            //if (id != null)
            //{
            //    var invoiceIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //    try
            //    {
            //        //var submittedInvoices = _invoiceManager.SubmitInvoices(invoiceIdList.ToList());
            //        //if (submittedInvoices.Count > 0)
            //        //{
            //        //    messageDetails = new UIMessageDetail { IsFailed = false, Message = string.Format(Messages.InvoicesSubmittedCount, submittedInvoices.Count) };
            //        //}
            //        //else
            //        //{
            //        //    messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.InvoiceIneligibleForSubmission };
            //        //}
            //        messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.InvoiceIneligibleForSubmission };
            //        return Json(messageDetails);
            //    }
            //    catch (ISBusinessException exception)
            //    {
            //        messageDetails = new UIMessageDetail { IsFailed = true, Message = GetDisplayMessageWithErrorCode(exception.ErrorCode) };

            //        return Json(messageDetails);
            //    }
            //}
            //else
            //{
            //    messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.SelectInvoiceForSubmission };
            //    return Json(messageDetails);
            // }
            messageDetails = new UIMessageDetail { IsFailed = false, Message = "Saved Successfully" };
            return Json(messageDetails);
        }
        /// <summary>
        /// This action method calls Invoice Edit page
        /// </summary>
        /// <returns></returns>
        public ActionResult EditInvoice(string id)
        {
            var invoice = _invoiceManager.GetInvoiceHeaderDetails(id);

            string controller = "PayablesInvoiceSearch";

            ViewData[ViewDataConstants.PageMode] = PageMode.Edit;
            return RedirectToAction("Edit", controller, new { area = "Cargo", invoiceId = id });
        }
        [HttpPost]
        public JsonResult ValidateInvoice(string id)
        {
            // TODO: This method needs refactoring
            UIMessageDetail details;
            var invoice = _invoiceManager.GetInvoiceHeaderDetails(id);

            try
            {
                //invoice = invoice.InvoiceType == InvoiceType.Invoice ? _nonSamplingInvoiceManager.ValidateInvoice(id) : _nonSamplingCreditNoteManager.ValidateInvoice(id);
                invoice = _invoiceManager.ValidateInvoice(id);

                details = invoice.InvoiceStatus == InvoiceStatusType.ReadyForSubmission
                            ? new UIMessageDetail { IsFailed = false, Message = string.Format(Messages.InvoiceValidateSuccessful, invoice.InvoiceNumber) }
                            : new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.InvoiceValidateFailed, invoice.InvoiceNumber) };
            }
            catch (ISBusinessException)
            {
                details = new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.InvoiceValidateFailed, invoice.InvoiceNumber) };
            }
            // details = new UIMessageDetail {};
            return Json(details);
        }

        public ActionResult ViewInvoice(string id)
        {
            var invoice = _invoiceManager.GetInvoiceHeaderDetails(id);

            string controller = "Invoice";

            return RedirectToAction("View", controller, new { area = "Cargo", invoiceId = id });
        }

        /// <summary>
        /// This action method calls invoice delete action method
        /// </summary>
        [HttpPost]
        public JsonResult DeleteInvoice(string id)
        {
            UIExceptionDetail details;

            var invoice = _invoiceManager.GetInvoiceHeaderDetails(id);
            try
            {
                //Delete record
                bool isDeleted = _invoiceManager.DeleteInvoice(id);

                details = isDeleted
                            ? new UIExceptionDetail { IsFailed = false, Message = string.Format(Messages.InvoiceDeleteSuccessful, invoice.InvoiceNumber) }
                            : new UIExceptionDetail { IsFailed = true, Message = string.Format(Messages.InvoiceDeleteFailed, invoice.InvoiceNumber) };
            }
            catch (ISBusinessException)
            {
                details = new UIExceptionDetail { IsFailed = true, Message = string.Format(Messages.InvoiceDeleteFailed, invoice.InvoiceNumber) };
            }
            //details = new UIExceptionDetail {};
            return Json(details);
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
                    var downloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
                                                                           "Invoice",
                                                                           new
                                                                           {
                                                                               area = "Cargo",
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
                    iInvoiceOfflineCollectionManager.EnqueueDownloadRequest(messages);

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
        private RejectionOnValidationFailure GetRejectionOnValidationFailureFlag()
        {
            var rejectionFlag = RejectionOnValidationFailure.RejectInvoiceInError;
            var rejectionFlagId = MemberManager.GetMemberConfigurationValue(SessionUtil.MemberId, MemberConfigParameter.CargoRejectionOnValidationFailure);
            if (!string.IsNullOrEmpty(rejectionFlagId))
                rejectionFlag = (RejectionOnValidationFailure)Convert.ToInt32(rejectionFlagId);
            return rejectionFlag;
        } 

    }
}
