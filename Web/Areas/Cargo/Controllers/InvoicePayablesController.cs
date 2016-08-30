using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Cargo;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Security;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Areas.Cargo.Controllers.Base;
using Iata.IS.Web.UIModel.BillingHistory.Cargo;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Cargo;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using log4net;

using BillingCode = Iata.IS.Model.Cargo.Enums.BillingCode;
using RejectionMemo = Iata.IS.Model.Cargo.CargoRejectionMemo;
using SubmissionMethod = Iata.IS.Model.Cargo.Enums.SubmissionMethod;


namespace Iata.IS.Web.Areas.Cargo.Controllers
{
    public class InvoicePayablesController : CargoInvoiceControllerBase
    {

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //private readonly IReferenceManager _referenceManager;
        // public ICargoInvoiceManager CargoInvoiceManager { get; set; }
        private readonly ICargoInvoiceManager _cargoInvoiceManager;
        private readonly IReferenceManager _referenceManager;
        private const string PrepaidBillingGridAction = "PrepaidBillingGridData";
        private const string SubTotalGridAction = "SubTotalGridData";
        private const string BillingMemoGridAction = "BillingMemoGridData";
        private const string InvoiceVatGridAction = "InvoiceVatGrid";
        private const string AvailableVatGridAction = "AvailableVatGridData";
        private const string UnappliedAmountVatGridAction = "UnappliedAmountVatGridData";
        public InvoicePayablesController(ICargoInvoiceManager cargoInvoiceManager, IReferenceManager referenceManager, IMemberManager memberManager)
            : base(cargoInvoiceManager)
        {

            _referenceManager = referenceManager;
            MemberManager = memberManager;
            _cargoInvoiceManager = cargoInvoiceManager;
        }

        /// <summary>
        /// Allows to edit an invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="billingType">Type of the billing.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.View)]
        public new ActionResult View(string invoiceId, string billingType)
        {
            var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
            ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

            ViewData[ViewDataConstants.TransactionExists] = _cargoInvoiceManager.IsTransactionExists(invoiceId);

            // Currently, not all invoices has corresponding InvoiceTotal record entries 
            // in database, hence creating empty object of InvoiceTotal object.
            if (InvoiceHeader.CGOInvoiceTotal == null)
            {
                InvoiceHeader.CGOInvoiceTotal = new CargoInvoiceTotal();
            }

            // Create Source Code grid instance
            var sourceCodeGrid = new AwbCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SubTotalGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);
            ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;
            MakeInvoiceRenderReady(InvoiceHeader.Id, InvoiceHeader);

            if (InvoiceHeader != null && InvoiceHeader.MemberLocationInformation.Count > 0)
            {
                foreach (var memLocation in InvoiceHeader.MemberLocationInformation)
                {
                    memLocation.LegalText = (!string.IsNullOrEmpty(InvoiceHeader.LegalText) ? InvoiceHeader.LegalText.Trim() : string.Empty);
                }
            }

            // If BillingType is Payables instantiate SourceCode Vat Total grid
            if (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
            {
                var availableVatGrid = new CargoAvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action("AvailableEmptySourceCodeVatTotalGridData"));
                ViewData["VatGrid"] = availableVatGrid.Instance;
            }

            return View("Edit", InvoiceHeader);
        }

        ///// <summary>
        ///// Used to instantiate SourceCode vat Total grid
        ///// </summary>
        ///// <returns>null</returns>
        public JsonResult AvailableEmptySourceCodeVatTotalGridData()
        {
            return null;
        }

        public ActionResult AwbPrepaidBillingListView(string invoiceId)
        {
            // Create grid instance for PrimeBilling grid
            bool isRejectionAllowed = false;
            if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
            {
                isRejectionAllowed = true;
            }
            var PrepaidBillingGrid = new AwbPrepaidBillingGrid(ViewDataConstants.PrimeBillingGrid, Url.Action("PrepaidBillingGridData", new { invoiceId }), isRejectionAllowed);
            ViewData[ViewDataConstants.PrimeBillingGrid] = PrepaidBillingGrid.Instance;

            // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
            var authorizationManager = Ioc.Resolve<IAuthorizationManager>();

            if (InvoiceHeader != null)
            {
                TempData["canCreate"] = authorizationManager.IsAuthorized(SessionUtil.UserId, Iata.IS.Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit);
                TempData["canView"] = InvoiceHeader.InvoiceType == InvoiceType.CreditNote
                                          ? authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Cargo.Payables.CreditNote.View)
                                          : authorizationManager.IsAuthorized(SessionUtil.UserId, Iata.IS.Business.Security.Permissions.Cargo.Payables.Invoice.View);
            }

            return View("AwbPrepaidBillingList", InvoiceHeader);
        }

        /// <summary>
        /// Fetch data for Prepaid Billing for Invoice and display it in grid
        /// </summary>
        public JsonResult PrepaidBillingGridData(string invoiceId)
        {
            // Create grid instance and retrieve data from database
            var preapidBillingGrid = new AwbPrepaidBillingGrid(ControlIdConstants.CouponGridId, Url.Action(PrepaidBillingGridAction, new { invoiceId }));
            var prepaidBillingCoupons = _cargoInvoiceManager.GetAwbPrepaidBillingRecordList(invoiceId).AsQueryable();
            //List<AwbRecord> invObj = new List<AwbRecord>();
            return preapidBillingGrid.DataBind(prepaidBillingCoupons.AsQueryable());
        }

        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.View)]
        [HttpGet]
        public ActionResult AwbPrepaidBillingView(string invoiceId, string transactionId)
        {
            var awbRecord = GetAwbRecord(transactionId, invoiceId);

            return View("AwbPrepaidRecordEdit", awbRecord);
        }

        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.Download)]
        [HttpGet]
        public FileStreamResult AwbAttachmentDownload(string invoiceId, string transactionId)
        {
            var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoInvoiceManager.GetAwbAttachmentDetails(transactionId) };

            return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
        }
        /// <summary>
        /// Display Rejection Memo in Invoice 
        /// </summary>
        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.View)]
        public ActionResult RMListView(string invoiceId)
        {
            // Create grid instance for Rejection Memo grid
            var rejectionMemoGrid = new RejectionMemoGrid(ViewDataConstants.RejectionMemoListGrid, Url.Action("RMGridData", "InvoicePayables", new { invoiceId }));
            ViewData[ViewDataConstants.RejectionMemoListGrid] = rejectionMemoGrid.Instance;

            return View("RMList", InvoiceHeader);
        }

        /// <summary>
        /// Fetch data for Rejection Memo for Invoice and display it in grid
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public JsonResult RMGridData(string invoiceId)
        {
            //Create grid instance and retrieve data from database
            var rejectionMemoGrid = new RejectionMemoGrid(ViewDataConstants.RejectionMemoListGrid, Url.Action("RMGridData", new { invoiceId }));

            var rejectionMemoCouponsList = _cargoInvoiceManager.GetRejectionMemoList(invoiceId);

            return rejectionMemoGrid.DataBind(rejectionMemoCouponsList.AsQueryable());
        }

        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.View)]
        [HttpGet]
        public ActionResult AwbChargeCollectBillingView(string invoiceId, string transactionId)
        {
            var awbRecord = GetAwbRecord(transactionId, invoiceId);

            return View("AwbChargeCollectRecordEdit", awbRecord);
        }

        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.Download)]
        [HttpGet]
        public FileStreamResult RejectionMemoAttachmentDownload(string invoiceId, string transactionId)
        {

            var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoInvoiceManager.GetRejectionMemoAttachmentDetails(transactionId) };

            return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
        }

        /// <summary>
        /// Download Rejection Memo AWB attachment
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="couponId">Coupon id</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.Download)]
        [HttpGet]
        public FileStreamResult RMAwbAttachmentDownload(string invoiceId, string couponId)
        {
            var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoInvoiceManager.GetRejectionMemoAwbAttachmentDetails(couponId) };

            return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
        }

        /// <summary>
        /// Billings the memo attachment download.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="transactionId">The transaction id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.Download)]
        [HttpGet]
        public FileStreamResult BillingMemoAttachmentDownload(string invoiceId, string transactionId)
        {
            var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoInvoiceManager.GetBillingMemoAttachmentDetails(transactionId) };

            return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
        }

        /// <summary>
        /// Billings the memo attachment download.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="transactionId">The transaction id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.Download)]
        [HttpGet]
        public FileStreamResult BMAwbAttachmentDownload(string invoiceId, string couponId)
        {
            var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoInvoiceManager.GetBMAwbAttachmentDetails(couponId) };

            return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
        }

        [HttpGet]
        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.View)]
        public ActionResult BMView(string invoiceId, string transactionId)
        {

            var billingMemoRecord = GetBillingMemoRecord(invoiceId, transactionId);
            ViewData[ViewDataConstants.BreakdownExists] = _cargoInvoiceManager.GetBillingMemoAwbCount(transactionId) > 0 ? true : false;
            // Initialize BMCoupon grid
            var billingMemoAwbGrid = new CargoBMAwbGrid(ViewDataConstants.BillingMemoAwbGrid, Url.Action("BillingMemoAwbGridData", "InvoicePayables", new { transactionId }));
            ViewData[ViewDataConstants.BillingMemoAwbGrid] = billingMemoAwbGrid.Instance;

            return View("BMEdit", billingMemoRecord);
        }

        /// <summary>
        /// BMs the awb prepaid data.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <returns></returns>
        public JsonResult BillingMemoAwbGridData(string transactionId)
        {
            // Create grid instance and retrieve data from database
            var billingMemoAwbGrid = new CargoBMAwbGrid(ControlIdConstants.BillingMemoAwbGrid, Url.Action("BillingMemoAwbGridData", "InvoicePayables", new { transactionId }));
            ViewData[ViewDataConstants.BillingMemoAwbGrid] = billingMemoAwbGrid.Instance;
            var billingMemoAwbs = _cargoInvoiceManager.GetBMAwbList(transactionId).AsQueryable();
            try
            {
                // return contactsGrid.DataBind(contacts.AsQueryable());
                // return null;
                return billingMemoAwbGrid.DataBind(billingMemoAwbs);
            }
            catch (ISBusinessException be)
            {

                ViewData["errorMessage"] = be.ErrorCode;
                return null;
            }

        }

        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.View)]
        [HttpGet]
        public ActionResult BMAwbView(string invoiceId, string transactionId, string couponId)
        {
            SetViewDataPageMode(PageMode.View);
            var awbRecord = GetBMAwbRecord(invoiceId, transactionId, couponId);
            if (awbRecord.AwbBillingCode == Convert.ToInt32(BillingCode.AWBPrepaid))
            {
                Session["helplinkurl"] = "Bm_Awb_Prepaid_view";
                return View("BMAwbPrepaidEdit", awbRecord);
            }
            Session["helplinkurl"] = "Bm_Awb_ChargeCollect_view";
            return View("BMAwbChargeCollectEdit", awbRecord);
        }

        [HttpGet]
        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.View)]
        public ActionResult BMAwbPrepaidView(string invoiceId, string transactionId, string couponId)
        {
            var bmAwbRecord = GetBMAwbRecord(invoiceId, transactionId, couponId);



            return View("BMAwbPrepaidEdit", bmAwbRecord);
        }

        /// <summary>
        /// Gets the BM awb record.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="transactionId">The transaction id.</param>
        /// <param name="couponId">The bm awb id.</param>
        /// <returns></returns>
        private CargoBillingMemoAwb GetBMAwbRecord(string invoiceId, string transactionId, string couponId)
        {
            var bmAwb = _cargoInvoiceManager.GetBMemoAwbRecordDetails(couponId);
            //var billingMemo = new CargoBillingMemo();
            bmAwb.BillingMemoRecord.Invoice = InvoiceHeader;
            bmAwb.LastUpdatedBy = SessionUtil.UserId;

            //var transactionTypeId = billingMemo.ReasonCode == "6A"
            //                              ? (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill
            //                              : billingMemo.ReasonCode == "6B" ? (int)TransactionType.PasNsBillingMemoDueToExpiry : (int)TransactionType.BillingMemo;
            // Depending on TransactionType and reasonCode retrieve whether Coupon breakdown is mandatory or not
            //var isCouponBreakdownMandatory = _referenceManager.GetReasonCode(billingMemo.ReasonCode, transactionTypeId).CouponAwbBreakdownMandatory;

            // Set Coupon breakdown value
            //billingMemo.CouponAwbBreakdownMandatory = isCouponBreakdownMandatory;

            return bmAwb;
        }

        /// <summary>
        /// BMs the list view.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.View)]
        public ActionResult BMListView(string invoiceId)
        {
            // Create grid instance for Billing Memo grid
            var billingMemoGrid = new CargoBMListGrid(ControlIdConstants.TransactionGridId, Url.Action(BillingMemoGridAction, new { invoiceId }));
            ViewData[ViewDataConstants.BillingMemoGrid] = billingMemoGrid.Instance;

            return View("BMList", InvoiceHeader);
        }

        public JsonResult InitiateDuplicateRejection(string rejectedRecordIds, string invoiceId)
        {
            return GetInvoicesForBillingHistory(rejectedRecordIds, invoiceId);
        }

        private JsonResult GetInvoicesForBillingHistory(string rejectedRecordIds, string invoiceId)
        {
            var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

            var billingCode = invoice.BillingCode;
            if (billingCode > (int)Iata.IS.Model.Pax.Enums.BillingCode.NonSampling) ++billingCode;
            var billedMemberId = invoice.BillingMemberId;
            var billingMemberId = invoice.BilledMemberId;

            var invoices = _cargoInvoiceManager.GetInvoicesForBillingHistory(billingCode, billedMemberId, SessionUtil.MemberId, invoice.SettlementMethodId);

            TempData[TempDataConstants.RejectedRecordIds] = string.Format("{0}@{1}", rejectedRecordIds, invoiceId);
            TempData[TempDataConstants.CorrespondenceNumber] = null;
            var result = new JsonResult();

            // If single open invoice found, redirect to rejection memo create page of that invoice.
            if (invoices.Count() == 1)
            {
                string redirectUrl = Url.Action("RMCreate", "Invoice", new { invoiceId = invoices.First().Id.Value(), area = "Cargo" });

                IList<CargoAuditTrailInvoice> invoiceList = new List<CargoAuditTrailInvoice> {
                                                   new CargoAuditTrailInvoice {
                                                       RedirectUrl = redirectUrl
                                                     }
                                                 };

                result.Data = invoiceList.ToList();

                return result;
            }

            if (invoices.Count() > 1)
            {
                IList<CargoAuditTrailInvoice> invoiceList = invoices.Select(paxInvoice => new CargoAuditTrailInvoice
                {
                    Id = paxInvoice.Id,
                    InvoiceNumber = paxInvoice.InvoiceNumber
                }).ToList();
                result.Data = invoiceList.ToList();

                return result;
            }

            var url = Url.Action("Create", "Invoice" , new {area = "Cargo"}) + "?FromBillingHistory=true";

            result.Data = new UIMessageDetail
            {
                Message = "Success",
                isRedirect = true,
                RedirectUrl = url
            };
            return result;
        }

        /// <summary>
        /// Fetch data for Rejection Memo Coupon Breakdown for Rejection Memo and display it in grid
        /// </summary>
        /// <param name="transactionId"></param>
        public JsonResult RejectionMemoAwbGridData(string transactionId)
        {
            // Create grid instance and retrieve data from database
            var rmAwbGrid = new RejectionMemoAwbGrid(ViewDataConstants.RejectionMemoAwbGrid, Url.Action("RejectionMemoAwbGridData", "InvoicePayables", new { transactionId }));
            var rejectionMemoAwbList = _cargoInvoiceManager.GetRejectionMemoAwbList(transactionId);

            return rmAwbGrid.DataBind(rejectionMemoAwbList.AsQueryable());
        }

        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.View)]
        [HttpGet]
        public ActionResult RMView(string invoiceId, string transactionId)
        {
            var rejectionMemoRecord = GetRejectionMemoRecord(transactionId, invoiceId);

            if (_cargoInvoiceManager.GetRejectionMemoAwbCount(transactionId) != 0)
            {
                ViewData[ViewDataConstants.BreakdownExists] = true;
            }
            else
            {
                ViewData[ViewDataConstants.BreakdownExists] = false;
            }

            var rejectionMemoAwbGrid = new RejectionMemoAwbGrid(ViewDataConstants.RejectionMemoAwbGrid, Url.Action("RejectionMemoAwbGridData", "InvoicePayables", new { transactionId }));
            ViewData[ViewDataConstants.RejectionMemoAwbGrid] = rejectionMemoAwbGrid.Instance;

            // This is done so as to not show the Reject icon when user navigates to RM listing page from correspondence.
            if (Request.QueryString["fc"] != null && Convert.ToBoolean(Request.QueryString["fc"]))
            {
                TempData[TempDataConstants.FromCorrespondence] = true;
            }

            return View("RMEdit", rejectionMemoRecord);
        }

        /// <summary>
        /// Display Rejection Memo in Invoice 
        /// </summary>
        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.View)]
        public ActionResult RMList(string invoiceId)
        {
            if (InvoiceHeader.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
            {
                return RedirectToAction("RMListView", new { invoiceId });
            }

            // Clear rejection memo data stored while navigating from Billing history.
            if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
            {
                TempData.Remove(TempDataConstants.RejectedRecordIds);
            }

            // Create grid instance for Rejection Memo grid
            var rejectionMemoGrid = new RejectionMemoGrid(ViewDataConstants.RejectionMemoListGrid, Url.Action("RMGridData", "Invoice", new { invoiceId }));
            ViewData[ViewDataConstants.RejectionMemoListGrid] = rejectionMemoGrid.Instance;

            return View(InvoiceHeader);
        }


        /// <summary>
        /// Following action is used to retrieve Source Code Vat total and display it on SourceCodeVatTotal grid
        /// </summary>
        /// <param name="sourceCodeId">SourceCodeVat Total Id</param>
        /// <returns>Json result for SourceCode vat total</returns>
        public JsonResult GetBillingCodeVatTotal(string sourceCodeId)
        {
            // Call GetSourceCodeVatTotal() method which returns SourceCode vat total details
            var sourceCodeVatTotalList = _cargoInvoiceManager.GetBillingCodeVatTotal(sourceCodeId);
            //var sourceCodeVatTotalList = _cargoInvoiceManager.FetchBillingCodeVatTotal(sourceCodeId); 

            // Return Json result
            return Json(sourceCodeVatTotalList);
        }
        
        /// <summary>
        /// Fetch billing memo for Invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public JsonResult BillingMemoGridData(string invoiceId)
        {
            // Create grid instance and retrieve data from database
            var billingMemoGrid = new CargoBMListGrid(ControlIdConstants.TransactionGridId, Url.Action(BillingMemoGridAction, new { invoiceId }));
            var billingMemoCoupons = _cargoInvoiceManager.GetBillingMemoList(invoiceId).AsQueryable();

            return billingMemoGrid.DataBind(billingMemoCoupons);
        }
        [HttpGet]
        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.View)]
        public ActionResult BMAwbChargeCollectView(string invoiceId, string transactionId, string couponId)
        {
            var bmAwbRecord = GetBMAwbRecord(invoiceId, transactionId, couponId);
            return View("BMAwbChargeCollectEdit", bmAwbRecord);
        }

        private CargoBillingMemo GetBillingMemoRecord(string invoiceId, string transactionId)
        {
            var billingMemo = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);
            //var billingMemo = new CargoBillingMemo();
            billingMemo.Invoice = InvoiceHeader;
            billingMemo.LastUpdatedBy = SessionUtil.UserId;

            //var transactionTypeId = billingMemo.ReasonCode == "6A"
            //                              ? (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill
            //                              : billingMemo.ReasonCode == "6B" ? (int)TransactionType.PasNsBillingMemoDueToExpiry : (int)TransactionType.BillingMemo;
            // Depending on TransactionType and reasonCode retrieve whether Coupon breakdown is mandatory or not
            //var isCouponBreakdownMandatory = _referenceManager.GetReasonCode(billingMemo.ReasonCode, transactionTypeId).CouponAwbBreakdownMandatory;

            // Set Coupon breakdown value
            //billingMemo.CouponAwbBreakdownMandatory = isCouponBreakdownMandatory;

            return billingMemo;
        }

        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.View)]
        [HttpGet]
        public ActionResult RMAwbView(string invoiceId, string transactionId, string couponId)
        {
            var awbRecord = GetRMAwbRecord(couponId, transactionId);
            SetViewDataPageMode(PageMode.View);
            if (awbRecord.AwbBillingCode == Convert.ToInt32(BillingCode.AWBPrepaid))
                return RedirectToAction("RMPrepaidAwbView", new { invoiceId, transactionId, couponId });

            return RedirectToAction("RMChargeCollectAwbView", new { invoiceId, transactionId, couponId });
        }

        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.View)]
        [HttpGet]
        public ActionResult RMPrepaidAwbView(string invoiceId, string transactionId, string couponId)
        {
            var awbRecord = GetRMAwbRecord(couponId, transactionId);

            return View("RMPrepaidAWBEdit", awbRecord);
        }

        [ISAuthorize(Business.Security.Permissions.Cargo.Payables.Invoice.View)]
        [HttpGet]
        public ActionResult RMChargeCollectAwbView(string invoiceId, string transactionId, string couponId)
        {
            var awbRecord = GetRMAwbRecord(couponId, transactionId);

            return View("RMChargeCollectAWBEdit", awbRecord);
        }

        private RMAwb GetRMAwbRecord(string couponId, string transactionId)
        {
            var rejectionMemoAwbDetails = _cargoInvoiceManager.GetRejectionMemoAwbDetails(couponId);

            rejectionMemoAwbDetails.LastUpdatedBy = SessionUtil.UserId;
            rejectionMemoAwbDetails.RejectionMemoRecord.Invoice = InvoiceHeader;
            return rejectionMemoAwbDetails;
        }

        //<summary>
        //Gets the rejection memo record.
        //</summary>
        //<param name="transactionId">The transaction id.</param>
        //<param name="invoiceId">The invoice id.</param>
        //<returns></returns>
        private RejectionMemo GetRejectionMemoRecord(string transactionId, string invoiceId)
        {
            var rejectionMemoRecord = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
            var transactionType = 0;

            // Depending on RejectionStage retrieve transaction type
            switch (rejectionMemoRecord.RejectionStage)
            {
                case 1:
                    transactionType = (int)TransactionType.CargoRejectionMemoStage1;
                    break;
                case 2:
                    transactionType = (int)TransactionType.CargoRejectionMemoStage2;
                    break;
                case 3:
                    transactionType = (int)TransactionType.CargoRejectionMemoStage3;
                    break;
            }

            // Depending on TransactionType and reasonCode retrieve whether Coupon breakdown is mandatory or not
            var reasonCode = _referenceManager.GetReasonCode(rejectionMemoRecord.ReasonCode, transactionType);
            bool isCouponBreakdownMandatory = reasonCode == null ? false : reasonCode.CouponAwbBreakdownMandatory;
            rejectionMemoRecord.Invoice = InvoiceHeader;

            // Set Coupon breakdown value
            rejectionMemoRecord.CouponAwbBreakdownMandatory = isCouponBreakdownMandatory;

            rejectionMemoRecord.LastUpdatedBy = SessionUtil.UserId;

            return rejectionMemoRecord;
        }


        private AwbRecord GetAwbRecord(string transactionId, string invoiceId)
        {
            var awbRecord = _cargoInvoiceManager.GetAwbRecordDetails((transactionId));
            awbRecord.Invoice = InvoiceHeader;
            awbRecord.LastUpdatedBy = SessionUtil.UserId;
            return awbRecord;
        }

        //ChargeCollectBillingListView
        public ActionResult AwbChargeCollectBillingList(string invoiceId)
        {
            //Create grid instance for PrimeBilling grid
            bool isRejectionAllowed = false;
            if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
            {
                isRejectionAllowed = true;
            }
            var PrepaidBillingGrid = new AwbChargeCollectGrid(ViewDataConstants.PrimeBillingGrid, Url.Action("ChargeCollectBillingGridData", new { invoiceId }), isRejectionAllowed);
            ViewData[ViewDataConstants.PrimeBillingGrid] = PrepaidBillingGrid.Instance;
            //===========NK==========
            // CargoInvoice InvoiceHeader = new CargoInvoice();
            //=========================

            // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
            var authorizationManager = Ioc.Resolve<IAuthorizationManager>();

            if (InvoiceHeader != null)
            {
                TempData["canCreate"] = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit);
                TempData["canView"] = InvoiceHeader.InvoiceType == InvoiceType.CreditNote
                                      ? authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Cargo.Payables.CreditNote.View)
                                      : authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Cargo.Payables.Invoice.View);
            }

            return View(InvoiceHeader);
        }

        /// <summary>
        /// Fetch data for Prepaid Billing for Invoice and display it in grid
        /// </summary>
        public JsonResult ChargeCollectBillingGridData(string invoiceId)
        {
            // Create grid instance and retrieve data from database
            var chargeCollectBillingGrid = new AwbChargeCollectGrid(ControlIdConstants.CouponGridId, Url.Action("ChargeCollectBillingGridData", new { invoiceId }));
            var chargeCollectBillingCoupons = _cargoInvoiceManager.GetAwbChargeCollectBillingRecordList(invoiceId).AsQueryable();
            //List<AwbRecord> invObj = new List<AwbRecord>();
            return chargeCollectBillingGrid.DataBind(chargeCollectBillingCoupons.AsQueryable());
        }


        public ActionResult AwbChargeCollectBillingListView(string invoiceId)
        {
            // Create grid instance for PrimeBilling grid
            bool isRejectionAllowed = false;
            if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
            {
                isRejectionAllowed = true;
            }
            var PrepaidBillingGrid = new AwbChargeCollectGrid(ViewDataConstants.PrimeBillingGrid, Url.Action("ChargeCollectBillingGridData", new { invoiceId }), isRejectionAllowed);
            ViewData[ViewDataConstants.PrimeBillingGrid] = PrepaidBillingGrid.Instance;

            // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions [Misc Payables]
            var authorizationManager = Ioc.Resolve<IAuthorizationManager>();

            if (InvoiceHeader != null)
            {
                TempData["canCreate"] = authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit);
                TempData["canView"] = InvoiceHeader.InvoiceType == InvoiceType.CreditNote
                                      ? authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Cargo.Payables.CreditNote.View)
                                      : authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Cargo.Payables.Invoice.View);
            }

            return View("AwbChargeCollectBillingList", InvoiceHeader);
        }

    }
}
