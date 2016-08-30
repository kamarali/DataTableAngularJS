using System;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Business;
//using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Reports;
using Iata.IS.Core.Exceptions;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Reports;
using Iata.IS.Web.Util;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Reports.SuspendedInvoice;
using Iata.IS.Model.Calendar;
using Iata.IS.Core.DI;
using Iata.IS.Business.Common;
using Iata.IS.Web.Util.Filters;



namespace Iata.IS.Web.Areas.Reports.Controllers
{
    public class ManageSuspendedInvoicesController : ISController
    {
        //
        // GET: /Reports/ManageSuspendedInvoices/
        private readonly IManageSuspendedInvoicesManager _manageSuspendedInvoices;
       
        public ManageSuspendedInvoicesController(IManageSuspendedInvoicesManager manageSuspendedInvoices)
        {
            _manageSuspendedInvoices = manageSuspendedInvoices;
        }

        [ISAuthorize(Business.Security.Permissions.General.ManageSuspendedInvoices.Query)]
        public ActionResult Index()
        {
            return View();
        }

        [ISAuthorize(Business.Security.Permissions.General.ManageSuspendedInvoices.Query)]
        public ActionResult ManageSuspendedInvoices()
        {
            const int fromBillingMonth = -1;
            const int toBillingMonth = -1;
            const int fromBillingPeriod = -1;
            const int toBillingPeriod = -1;
            const int smi = -1;
            const int resubmissionStatus = -1;
            const int billedEntityCode = -1;
            const int fromBillingYear = -1;
            const int toBillingYear = -1;

            var suspendedInvoiceSearchGrid = new ManageSuspendedInvoicesSearchResult("ManageSuspendedInvoicesSearchResultGrid", Url.Action("SearchResultGridData", "ManageSuspendedInvoices",
                                                                                  new
                                                                                  {
                                                                                      fromBillingMonth,
                                                                                      toBillingMonth,
                                                                                      fromBillingPeriod,
                                                                                      toBillingPeriod,
                                                                                      smi,
                                                                                      resubmissionStatus,
                                                                                      billedEntityCode,
                                                                                      fromBillingYear,
                                                                                      toBillingYear
                                                                                  }));
            ViewData["ManageSuspendedInvoicesSearchResultGridData"] = suspendedInvoiceSearchGrid.Instance;
            return View();
        }

        public JsonResult SearchResultGridData(int fromBillingMonth, int toBillingMonth, int fromBillingPeriod, int toBillingPeriod, int smi, int resubmissionStatus, int billedEntityCode, int fromBillingYear, int toBillingYear)
        {
          var suspendedInvoiceSearchGrid = new ManageSuspendedInvoicesSearchResult("ManageSuspendedInvoicesSearchResultGrid", Url.Action("SearchResultGridData", new { fromBillingMonth, toBillingMonth, fromBillingPeriod, toBillingPeriod, smi, resubmissionStatus, billedEntityCode, fromBillingYear, toBillingYear }));
          var suspendeInvoices = _manageSuspendedInvoices.GetSuspendedInvoiceList(SessionUtil.MemberId, fromBillingMonth, toBillingMonth, fromBillingPeriod, toBillingPeriod, smi, resubmissionStatus, billedEntityCode, fromBillingYear, toBillingYear);
            try
            {
                return suspendedInvoiceSearchGrid.DataBind(suspendeInvoices.AsQueryable());

            }
            catch (ISBusinessException be)
            {
                ViewData["errorMessage"] = be.ErrorCode;
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateInvoiceRemark(Guid invoiceId, string remark)
        {
            var result = _manageSuspendedInvoices.UpdateInvoiceRemark(invoiceId, remark);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public string GetInvoiceRemark(Guid invoiceId)
        {
            var invoiceRecord = _manageSuspendedInvoices.GetInvoice(invoiceId);
            return invoiceRecord.ResubmissionRemarks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedInvoiceIds"></param>
        /// <param name="isIchLateSubmit"></param>
        /// <param name="isAchLateSubmit"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MarkInvoiceAsResubmitted(string selectedInvoiceIds, string isIchLateSubmit, string isAchLateSubmit)
        {
                UIMessageDetail details;
            
                var invoiceIdArray = selectedInvoiceIds.Split(',').ToList();
                if (invoiceIdArray[0].Equals(String.Empty))
                    invoiceIdArray.RemoveAt(0);
                try
                {
                    var result = _manageSuspendedInvoices.MarkInvoicesAsResubmitted(invoiceIdArray,
                                                                                    bool.Parse(isIchLateSubmit),
                                                                                    bool.Parse(isAchLateSubmit));
                    details = new UIMessageDetail
                                  {
                                      IsFailed = false,
                                      Message = "Invoices resubmitted successfully.",
                                      isRedirect = false

                                  };
                }
                catch (ISBusinessException exception)
                {
                    if (exception.ErrorCode == ErrorCodes.CheckforParentMember)
                        details = new UIMessageDetail
                        {
                            IsFailed = true,
                            Message =
                                exception.ErrorCode == ErrorCodes.CheckforParentMember
                                    ? GetDisplayMessageWithErrorCode(exception.ErrorCode) + " Unable to resubmit Invoice(s): " + exception.Message
                                    : "Error",
                            ErrorCode =
                                exception.ErrorCode == ErrorCodes.CheckforParentMember
                                    ? ErrorCodes.CheckforParentMember
                                    : string.Empty
                        };
                    else if (exception.ErrorCode == ErrorCodes.InvoiceIsBilaterallySettled)
                        details = new UIMessageDetail
                                      {
                                          IsFailed = true,
                                          Message =
                                              exception.ErrorCode == ErrorCodes.InvoiceIsBilaterallySettled
                                                  ? GetDisplayMessageWithErrorCode(exception.ErrorCode) + " Invoice Number: " + exception.Message
                                                  : "Error",
                                          ErrorCode =
                                              exception.ErrorCode == ErrorCodes.InvoiceIsBilaterallySettled
                                                  ? ErrorCodes.InvoiceIsBilaterallySettled
                                                  : string.Empty
                                      };
                    else if (exception.ErrorCode == ErrorCodes.BillingMemberSuspended)
                        details = new UIMessageDetail
                                      {
                                          IsFailed = true,
                                          Message =
                                              exception.ErrorCode == ErrorCodes.BillingMemberSuspended
                                                  ? GetDisplayMessageWithErrorCode(exception.ErrorCode)
                                                  : "Error",
                                          ErrorCode =
                                              exception.ErrorCode == ErrorCodes.BillingMemberSuspended
                                                  ? ErrorCodes.BillingMemberSuspended
                                                  : string.Empty
                                      };
                    else if (exception.ErrorCode == ErrorCodes.AlreadyResubmitedInvoice)
                        details = new UIMessageDetail
                                      {
                                          IsFailed = true,
                                          Message =
                                              exception.ErrorCode == ErrorCodes.AlreadyResubmitedInvoice
                                                  ? GetDisplayMessageWithErrorCode(exception.ErrorCode) +" Invoice Number: " + exception.Message
                                                  : "Error",
                                          ErrorCode =
                                              exception.ErrorCode == ErrorCodes.AlreadyResubmitedInvoice
                                                  ? ErrorCodes.AlreadyResubmitedInvoice
                                                  : string.Empty
                                      };
                    else if (exception.ErrorCode == ErrorCodes.UnableToResubmit)
                        details = new UIMessageDetail
                                      {
                                          IsFailed = true,
                                          Message =
                                              exception.ErrorCode == ErrorCodes.UnableToResubmit
                                                  ? GetDisplayMessageWithErrorCode(exception.ErrorCode) +" Invoice Number: " + exception.Message
                                                  : "Error",
                                          ErrorCode =
                                              exception.ErrorCode == ErrorCodes.UnableToResubmit
                                                  ? ErrorCodes.UnableToResubmit
                                                  : string.Empty
                                      };
                    else
                    {
                        details = new UIMessageDetail
                                      {
                                          IsFailed = true,
                                          Message =
                                              exception.ErrorCode == ErrorCodes.BilledMemberSuspended
                                                  ? GetDisplayMessageWithErrorCode(exception.ErrorCode)
                                                  : "Error",
                                          ErrorCode =
                                              exception.ErrorCode == ErrorCodes.BilledMemberSuspended
                                                  ? ErrorCodes.BilledMemberSuspended
                                                  : string.Empty
                                      };
                    }
                }
            
            return Json(details);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedInvoiceIds"></param>
        /// <returns></returns>
        public JsonResult MarkInvoiceAsBilaterallySettled(string selectedInvoiceIds)
        {
            UIMessageDetail details;
            var invoiceIdArray = selectedInvoiceIds.Split(',').ToList();
            if (invoiceIdArray[0].Equals(String.Empty))
                invoiceIdArray.RemoveAt(0);
            try
            {
                var result = _manageSuspendedInvoices.MarkInvoicesAsBilaterallySettled(invoiceIdArray);
                details = new UIMessageDetail
                {
                    IsFailed = false,
                    Message = "Invoices marked as bilaterally settled successfully.",
                    isRedirect = false

                };
            }

            catch (ISBusinessException exception)
            {
              if (exception.ErrorCode == ErrorCodes.AlreadyBilaterallySettledInvoice)
                details = new UIMessageDetail
                {
                  IsFailed = true,
                  Message = exception.ErrorCode == ErrorCodes.AlreadyBilaterallySettledInvoice ? GetDisplayMessageWithErrorCode(exception.ErrorCode) : "Error",
                  ErrorCode = exception.ErrorCode == ErrorCodes.AlreadyBilaterallySettledInvoice ? ErrorCodes.AlreadyBilaterallySettledInvoice : string.Empty
                };
              else if (exception.ErrorCode == ErrorCodes.AlreadyResubmitedInvoice)
                details = new UIMessageDetail
                {
                  IsFailed = true,
                  Message = exception.ErrorCode == ErrorCodes.AlreadyResubmitedInvoice ? GetDisplayMessageWithErrorCode(exception.ErrorCode) : "Error",
                  ErrorCode = exception.ErrorCode == ErrorCodes.AlreadyResubmitedInvoice ? ErrorCodes.AlreadyResubmitedInvoice : string.Empty
                };
              else
                details = new UIMessageDetail
                {
                  IsFailed = true,
                  Message = "Error occured...",
                  isRedirect = false
                };
              
            }
           
           
            return Json(details);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedInvoiceIds"></param>
        /// <returns></returns>
        public JsonResult UndoBilateral(string selectedInvoiceIds)
        {
            UIMessageDetail details;
            var invoiceIdArray = selectedInvoiceIds.Split(',').ToList();
            if (invoiceIdArray[0].Equals(String.Empty))
                invoiceIdArray.RemoveAt(0);
            try
            {
                var result = _manageSuspendedInvoices.UndoBilateral(invoiceIdArray);
                details = new UIMessageDetail
                {
                    IsFailed = false,
                    Message = "Bilaterally settled invoices reverted successfully.",
                    isRedirect = false

                };
            }
            catch (Exception exception)
            {

                details = new UIMessageDetail
                {
                    IsFailed = true,
                    Message = "Error occured...",
                    isRedirect = false
                };
            }
            return Json(details);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckIfLateSubmissionWindowOpen(string selectedInvoiceIds)
        {
          var invoiceIdArray = selectedInvoiceIds.Split(',').ToList();
          if (invoiceIdArray[0].Equals(String.Empty))
            invoiceIdArray.RemoveAt(0);
          var isLateSubmission = _manageSuspendedInvoices.CheckIfLateSubmissionWindowOpen(invoiceIdArray);
          return Json(isLateSubmission[0] + "," + isLateSubmission[1]);

        }

        /// <summary>
        /// Members the suspended invoices.
        /// </summary>
        /// <returns></returns>
         [ISAuthorize(Business.Security.Permissions.Reports.FinancialController.SuspendedBillingsAccess)]
        public ActionResult MemberSuspendedInvoices()
        {
            MemberSuspendedInvoice invoice = new MemberSuspendedInvoice();
            BillingPeriod currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
            invoice.ToBillingYear = currentBillingPeriod.Year;
            invoice.ToBillingMonth = currentBillingPeriod.Month;
            invoice.ToBillingPeriod = currentBillingPeriod.Period;
            invoice.FromBillingYear = currentBillingPeriod.Year;
            invoice.FromBillingMonth = currentBillingPeriod.Month;
            invoice.FromBillingPeriod = currentBillingPeriod.Period;
            //Set the IATA Member Id from SystemParameter class
            invoice.IataMemberId = (int) FileSenderRecieverType.IATA;
            //Set the ACH Member Id from SystemParameter class
            invoice.AchMemberId = (int) FileSenderRecieverType.ACH;
            return View(invoice);
        }

        /// <summary>
        /// Members the suspended invoices.
        /// </summary>
        /// <param name="invoiceBase">The invoice base.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult MemberSuspendedInvoices(MemberSuspendedInvoice invoiceBase)
        {
            return View(invoiceBase);
        }
      
        public JsonResult GetInvoiceResubmissionStatus(string invoiceId)
        {
          try
          {
            var resubmissionStatusId = _manageSuspendedInvoices.GetInvoiceResubmissionStatus(Guid.Parse(invoiceId));
            return Json(resubmissionStatusId);

          }
          catch (ISBusinessException be)
          {
            ViewData["errorMessage"] = be.ErrorCode;
            return Json(0);
          }
        }
    }
}
