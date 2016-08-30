using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Core;
using Iata.IS.Data.Cargo;
using Iata.IS.Data.Cargo.Impl;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.Areas.Pax.Controllers.Base;
using Iata.IS.Web.UIModel.Grid.Common;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using log4net;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
    public class InvoicePayablesController : PaxInvoiceControllerBase
    {

        private readonly INonSamplingInvoiceManager _nonSamplingInvoiceManager;
        private readonly IReferenceManager _referenceManager;
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string PrimeBillingGridAction = "PrimeBillingGridData";
        private const string SourceCodeGridAction = "SourceCodeGridData";
        private const string BillingMemoGridAction = "BillingMemoGridData";

        /// <summary>
        /// Gets or sets SourceCodevatTotal repository
        /// </summary>
        public ICargoBillingCodeSubTotalVatRepository CargoBillingCodeSubTotalVatRepository { get; set; }


        public InvoicePayablesController(INonSamplingInvoiceManager nonSamplingInvoiceManager, IReferenceManager referenceManager, IMemberManager memberManager)
            : base(nonSamplingInvoiceManager)
        {
            _nonSamplingInvoiceManager = nonSamplingInvoiceManager;
            _referenceManager = referenceManager;
            MemberManager = memberManager;
        }

        protected override int BillingCodeId
        {
            get { return Convert.ToInt32(BillingCode.NonSampling); }
        }

        protected override InvoiceType InvoiceType
        {
            get { return InvoiceType.Invoice; }
        }

        [HttpGet]
        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View)]
        public ActionResult BMCouponView(string invoiceId, string transactionId, string couponId)
        {
            var breakdownrecord = GetBreakdownrecord(couponId, transactionId, invoiceId);

            return View("BMCouponEdit", breakdownrecord);
        }

        /// <summary>
        /// Gets billing memo list and displays in grid for given invoice.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View)]
        public ActionResult BMList(string invoiceId)
        {
            if (InvoiceHeader.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
            {
                return RedirectToAction("BMListView", new { invoiceId });
            }

            // Create grid instance for Billing Memo grid
            var billingMemoGrid = new BillingMemoGrid(ControlIdConstants.TransactionGridId, Url.Action(BillingMemoGridAction, new { invoiceId }));
            ViewData[ViewDataConstants.BillingMemoGrid] = billingMemoGrid.Instance;

            return View(InvoiceHeader);
        }

        private BMCoupon GetBreakdownrecord(string couponId, string transactionId, string invoiceId)
        {
            var breakdownrecord = _nonSamplingInvoiceManager.GetBillingMemoCouponDetails(couponId);
            breakdownrecord.LastUpdatedBy = SessionUtil.UserId;
            breakdownrecord.BillingMemo = _nonSamplingInvoiceManager.GetBillingMemoRecordDetails(transactionId);
            breakdownrecord.BillingMemo.Invoice = InvoiceHeader;
            // added code to remove the extra special char
            if (!string.IsNullOrEmpty(breakdownrecord.ProrateSlipDetails))
                breakdownrecord.ProrateSlipDetails =
                  breakdownrecord.ProrateSlipDetails.Replace("\n", string.Empty).Replace("\r", string.Empty);

            return breakdownrecord;
        }

        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View)]
        public ActionResult BMListView(string invoiceId)
        {
            // Create grid instance for Billing Memo grid
            var billingMemoGrid = new BillingMemoGrid(ControlIdConstants.TransactionGridId, Url.Action(BillingMemoGridAction, new { invoiceId }));
            ViewData[ViewDataConstants.BillingMemoGrid] = billingMemoGrid.Instance;

            return View("BMList", InvoiceHeader);
        }

        /// <summary>
        /// Fetch billing memo for Invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public JsonResult BillingMemoGridData(string invoiceId)
        {
            // Create grid instance and retrieve data from database
            var billingMemoGrid = new BillingMemoGrid(ControlIdConstants.TransactionGridId, Url.Action(BillingMemoGridAction, new { invoiceId }));
            var billingMemoCoupons = _nonSamplingInvoiceManager.GetBillingMemoList(invoiceId).AsQueryable();

            return billingMemoGrid.DataBind(billingMemoCoupons);
        }


        /// <summary>
        /// Download Billing Memo attachment 
        ///  </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="transactionId">Transaction id</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.Download)]
        [HttpGet]
        public FileStreamResult BillingMemoAttachmentDownload(string invoiceId, string transactionId)
        {
            var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingInvoiceManager.GetBillingMemoAttachmentDetails(transactionId) };

            return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
        }

        /// <summary>
        /// Download Rejection Memo attachment
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="transactionId">Transaction id</param>
        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.Download)]
        [HttpGet]
        public FileStreamResult RejectionMemoAttachmentDownload(string invoiceId, string transactionId)
        {
            var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingInvoiceManager.GetRejectionMemoAttachmentDetails(transactionId) };

            return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
        }


        /// <summary>
        /// Download Billing Memo Coupon attachment
        ///  </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="couponId">Coupon id</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.Download)]
        [HttpGet]
        public FileStreamResult BMCouponAttachmentDownload(string invoiceId, string couponId)
        {
            var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingInvoiceManager.GetBillingMemoCouponAttachmentDetails(couponId) };

            return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
        }

        /// <summary>
        /// Following action is used to retrieve Source Code Vat total and display it on SourceCodeVatTotal grid
        /// </summary>
        /// <param name="sourceCodeId">SourceCodeVat Total Id</param>
        /// <returns>Json result for SourceCode vat total</returns>
        public JsonResult GetBillingCodeVatTotal(string sourceCodeId)
        {
            // Call GetSourceCodeVatTotal() method which returns SourceCode vat total details
            var sourceCodeVatTotalList = _nonSamplingInvoiceManager.GetBillingCodeVatTotal(sourceCodeId);
            //var sourceCodeVatTotalList = _cargoInvoiceManager.FetchBillingCodeVatTotal(sourceCodeId); 

            // Return Json result
            return Json(sourceCodeVatTotalList);
        }

        [HttpGet]
        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View)]
        public ActionResult BMView(string invoiceId, string transactionId)
        {
            var billingMemoRecord = GetBillingMemoRecord(invoiceId, transactionId);

            // Initialize BMCoupon grid
            var billingMemoCouponGrid = new BillingMemoCouponGrid(ViewDataConstants.BillingMemoCouponGrid, Url.Action("BillingMemoCouponGridData", "InvoicePayables", new { transactionId }));
            ViewData[ViewDataConstants.BillingMemoCouponGrid] = billingMemoCouponGrid.Instance;

            return View("BMEdit", billingMemoRecord);
        }

        private BillingMemo GetBillingMemoRecord(string invoiceId, string transactionId)
        {
            var billingMemo = _nonSamplingInvoiceManager.GetBillingMemoRecordDetails(transactionId);
            billingMemo.Invoice = InvoiceHeader;
            billingMemo.LastUpdatedBy = SessionUtil.UserId;

            var transactionTypeId = billingMemo.ReasonCode == "6A"
                                          ? (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill
                                          : billingMemo.ReasonCode == "6B" ? (int)TransactionType.PasNsBillingMemoDueToExpiry : (int)TransactionType.BillingMemo;
            // Depending on TransactionType and reasonCode retrieve whether Coupon breakdown is mandatory or not
            var isCouponBreakdownMandatory = _referenceManager.GetReasonCode(billingMemo.ReasonCode, transactionTypeId).CouponAwbBreakdownMandatory;

            // Set Coupon breakdown value
            billingMemo.CouponAwbBreakdownMandatory = isCouponBreakdownMandatory;

            return billingMemo;
        }

        /// <summary>
        /// Display Prime Billing coupons in Invoice 
        /// </summary>
        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View)]
        public ActionResult PrimeBillingList(string invoiceId)
        {
            //Create grid instance for PrimeBilling grid
            bool isRejectionAllowed = false;
            if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
            {
                isRejectionAllowed = true;
            }
            var primeBillingGrid = new PrimeBillingGrid(ViewDataConstants.PrimeBillingGrid, Url.Action(PrimeBillingGridAction, new { invoiceId }), isRejectionAllowed);
            ViewData[ViewDataConstants.PrimeBillingGrid] = primeBillingGrid.Instance;

            return View(InvoiceHeader);
        }

        /// <summary>
        /// Fetch data for Prime Billing Coupons for Invoice and display it in grid
        /// </summary>
        public JsonResult PrimeBillingGridData(string invoiceId)
        {
            // Create grid instance and retrieve data from database
            var primeBillingGrid = new PrimeBillingGrid(ControlIdConstants.CouponGridId, Url.Action(PrimeBillingGridAction, new { invoiceId }));
            var primeBillingCoupons = _nonSamplingInvoiceManager.GetPrimeBillingCouponList(invoiceId).AsQueryable();

            return primeBillingGrid.DataBind(primeBillingCoupons);
        }

        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View)]
        public ActionResult PrimeBillingListView(string invoiceId)
        {
            // Create grid instance for PrimeBilling grid
            bool isRejectionAllowed = false;
            if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
            {
                isRejectionAllowed = true;
            }
            var primeBillingGrid = new PrimeBillingGrid(ViewDataConstants.PrimeBillingGrid, Url.Action(PrimeBillingGridAction, new { invoiceId }), isRejectionAllowed);
            ViewData[ViewDataConstants.PrimeBillingGrid] = primeBillingGrid.Instance;

            return View("PrimeBillingList", InvoiceHeader);
        }

        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View)]
        [HttpGet]
        public ActionResult PrimeBillingView(string invoiceId, string transactionId)
        {
            var couponRecord = GetCouponRecord(transactionId, invoiceId);

            return View("PrimeBillingEdit", couponRecord);
        }

        private PrimeCoupon GetCouponRecord(string transactionId, string invoiceId)
        {
            var couponRecord = _nonSamplingInvoiceManager.GetCouponRecordDetails((transactionId));
            couponRecord.Invoice = InvoiceHeader;
            couponRecord.LastUpdatedBy = SessionUtil.UserId;
            return couponRecord;
        }

        /// <summary>
        /// Download prime billing attachment
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="transactionId">Transaction Id</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.Download)]
        [HttpGet]
        public FileStreamResult PrimeBillingAttachmentDownload(string invoiceId, string transactionId)
        {
            var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingInvoiceManager.GetCouponLevelAttachmentDetails(transactionId) };

            return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
        }

        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View)]
        [HttpGet]
        public ActionResult RMCouponView(string invoiceId, string transactionId, string couponId)
        {
            var couponRejectionBreakdownRecord = GetCouponRejectionBreakdownRecord(couponId, transactionId, invoiceId);

            return View("RMCouponBreakdownEdit", couponRejectionBreakdownRecord);
        }

        private RMCoupon GetCouponRejectionBreakdownRecord(string couponId, string transactionId, string invoiceId)
        {
            var couponRejectionBreakdownRecord = _nonSamplingInvoiceManager.GetRejectionMemoCouponDetails(couponId);
            couponRejectionBreakdownRecord.RejectionMemoRecord = _nonSamplingInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
            couponRejectionBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
            couponRejectionBreakdownRecord.RejectionMemoRecord.Invoice = InvoiceHeader;

            // added code to remove the extra special char
            if (!string.IsNullOrEmpty(couponRejectionBreakdownRecord.ProrateSlipDetails))
                couponRejectionBreakdownRecord.ProrateSlipDetails =
                  couponRejectionBreakdownRecord.ProrateSlipDetails.Replace("\n", string.Empty).Replace("\r", string.Empty);

            return couponRejectionBreakdownRecord;
        }

        /// <summary>
        /// Display Rejection Memo in Invoice 
        /// </summary>
        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View)]
        public ActionResult RMList(string invoiceId)
        {
            if (InvoiceHeader.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
            {
                return RedirectToAction("RMListView", new { invoiceId });
            }

            // Clear rejection memo data from Tempdata.
            TempData.Clear();

            // Create grid instance for Rejection Memo grid
            var rejectionMemoGrid = new RejectionMemoGrid(ViewDataConstants.RejectionMemoListGrid, Url.Action("RMGridData", "InvoicePayables", new { invoiceId }));
            ViewData[ViewDataConstants.RejectionMemoListGrid] = rejectionMemoGrid.Instance;

            return View(InvoiceHeader);
        }

        /// <summary>
        /// Fetch data for Rejection Memo for Invoice and display it in grid
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public JsonResult RMGridData(string invoiceId)
        {
            //Create grid instance and retrieve data from database
            var rejectionMemoGrid = new RejectionMemoGrid(ControlIdConstants.TransactionGridId, Url.Action("RMGridData", new { invoiceId }));

            var rejectionMemoCouponsList = _nonSamplingInvoiceManager.GetRejectionMemoList(invoiceId);

            return rejectionMemoGrid.DataBind(rejectionMemoCouponsList.AsQueryable());
        }

        /// <summary>
        /// Display Rejection Memo in Invoice 
        /// </summary>
        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View)]
        public ActionResult RMListView(string invoiceId)
        {
            // Create grid instance for Rejection Memo grid
            var rejectionMemoGrid = new RejectionMemoGrid(ViewDataConstants.RejectionMemoListGrid, Url.Action("RMGridData", "InvoicePayables", new { invoiceId }));
            ViewData[ViewDataConstants.RejectionMemoListGrid] = rejectionMemoGrid.Instance;

            return View("RMList", InvoiceHeader);
        }

        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View)]
        [HttpGet]
        public ActionResult RMView(string invoiceId, string transactionId)
        {
            var rejectionMemoRecord = GetRejectionMemoRecord(transactionId, invoiceId);

            if (_nonSamplingInvoiceManager.GetRejectionMemoCouponBreakdownCount(transactionId) != 0)
            {
                ViewData[ViewDataConstants.BreakdownExists] = true;
            }
            else
            {
                ViewData[ViewDataConstants.BreakdownExists] = false;
            }

            var rejectionMemoCouponBreakdownGrid = new RejectionMemoCouponBreakdownGrid(ViewDataConstants.RMCouponListGrid,
                                                                                        Url.Action("RejectionMemoCouponBreakdownGridData", "InvoicePayables", new { transactionId }));
            ViewData[ViewDataConstants.RMCouponListGrid] = rejectionMemoCouponBreakdownGrid.Instance;

            // This is done so as to not show the Reject icon when user navigates to RM listing page from correspondence.
            if (Request.QueryString["fc"] != null && Convert.ToBoolean(Request.QueryString["fc"]))
            {
                TempData[TempDataConstants.FromCorrespondence] = true;
            }

            return View("RMEdit", rejectionMemoRecord);
        }

        /// <summary>
        /// Fetch data for coupon list in Billing memo
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public JsonResult BillingMemoCouponGridData(string transactionId)
        {
            // Create grid instance and retrieve data from database
            var billingMemoCouponGrid = new BillingMemoCouponGrid(ControlIdConstants.CouponGridId, Url.Action("BillingMemoCouponGridData", "InvoicePayables", new { transactionId }));

            var billingMemoCoupons = _nonSamplingInvoiceManager.GetBillingMemoCouponList(transactionId).AsQueryable();
            return billingMemoCouponGrid.DataBind(billingMemoCoupons);
        }

        /// <summary>
        /// Fetch data for Rejection Memo Coupon Breakdown for Rejection Memo and display it in grid
        /// </summary>
        /// <param name="transactionId"></param>
        public JsonResult RejectionMemoCouponBreakdownGridData(string transactionId)
        {
            // Create grid instance and retrieve data from database
            var rmCbGrid = new RejectionMemoCouponBreakdownGrid(ControlIdConstants.CouponGridId, Url.Action("RejectionMemoCouponBreakdownGridData", "InvoicePayables", new { transactionId }));
            var rejectionMemoCouponBreakdownList = _nonSamplingInvoiceManager.GetRejectionMemoCouponBreakdownList(transactionId);

            return rmCbGrid.DataBind(rejectionMemoCouponBreakdownList.AsQueryable());
        }

        /// <summary>
        /// Download Rejection Memo Coupon attachment
        /// </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="couponId">Coupon id</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.Download)]
        [HttpGet]
        public FileStreamResult RMCouponAttachmentDownload(string invoiceId, string couponId)
        {
            var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingInvoiceManager.GetRejectionMemoCouponAttachmentDetails(couponId) };

            return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
        }

        /// <summary>
        /// Gets the rejection memo record.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns></returns>
        private RejectionMemo GetRejectionMemoRecord(string transactionId, string invoiceId)
        {
            var rejectionMemoRecord = _nonSamplingInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
            var transactionType = 0;

            // Depending on RejectionStage retrieve transaction type
            switch (rejectionMemoRecord.RejectionStage)
            {
                case 1:
                    transactionType = (int)TransactionType.RejectionMemo1;
                    break;
                case 2:
                    transactionType = (int)TransactionType.RejectionMemo2;
                    break;
                case 3:
                    transactionType = (int)TransactionType.RejectionMemo3;
                    break;
            }

            // Depending on TransactionType and reasonCode retrieve whether Coupon breakdown is mandatory or not
            bool isCouponBreakdownMandatory = _referenceManager.GetReasonCode(rejectionMemoRecord.ReasonCode, transactionType).CouponAwbBreakdownMandatory;
            rejectionMemoRecord.Invoice = InvoiceHeader;

            // Set Coupon breakdown value
            rejectionMemoRecord.CouponAwbBreakdownMandatory = isCouponBreakdownMandatory;

            rejectionMemoRecord.LastUpdatedBy = SessionUtil.UserId;

            return rejectionMemoRecord;
        }

        /// <summary>
        /// Allows to edit an invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="billingType">Type of the billing.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View)]
        public new ActionResult View(string invoiceId, string billingType)
        {
            var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
            ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);
            ViewData[ViewDataConstants.TransactionExists] = _nonSamplingInvoiceManager.IsTransactionExists(invoiceId);

            // Currently, not all invoices has corresponding InvoiceTotal record entries 
            // in database, hence creating empty object of InvoiceTotal object.
            if (InvoiceHeader.InvoiceTotalRecord == null)
            {
                InvoiceHeader.InvoiceTotalRecord = new InvoiceTotal();
            }

            // Create Source Code grid instance
            var sourceCodeGrid = new SourceCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SourceCodeGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);
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
                var availableVatGrid = new AvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action("AvailableEmptySourceCodeVatTotalGridData"));
                ViewData["VatGrid"] = availableVatGrid.Instance;
            }

            return View("Edit", InvoiceHeader);
        }


    }
}