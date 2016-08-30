using System;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.Areas.Pax.Controllers.Base;
using Iata.IS.Business.Pax;
using Iata.IS.Web.Util;
using Iata.IS.Model.Pax;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Web.UIModel.Grid.Common;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
  public class FormABPayablesController : PaxInvoiceControllerBase
  {
    private readonly INonSamplingInvoiceManager _nonSamplingInvoiceManager;
    private const string SourceCodeGridAction = "SourceCodeGridData";
    private const string PrimeBillingGridAction = "PrimeBillingGridData";

    public FormABPayablesController(INonSamplingInvoiceManager nonSamplingInvoiceManager)
        : base(nonSamplingInvoiceManager)
    {
      _nonSamplingInvoiceManager = nonSamplingInvoiceManager;
    }

    protected override int BillingCodeId
    {
      get { return Convert.ToInt32(BillingCode.SamplingFormAB); }
    }

    [HttpGet]
    public new ActionResult View(string invoiceId)
    {
      ViewData[ViewDataConstants.TransactionExists] = _nonSamplingInvoiceManager.IsTransactionExists(invoiceId);

      var smIsToBeTreatedBilateral = ReferenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

      // Currently, not all invoices has corresponding InvoiceTotal record entries 
      // in database, hence creating empty object of InvoiceTotal object.
      if (InvoiceHeader.InvoiceTotalRecord == null)
      {
        InvoiceHeader.InvoiceTotalRecord = new InvoiceTotal();
      }

      // Create Source Code grid instance
      var sourceCodeGrid = new SourceCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SourceCodeGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables) ? true : false);
      ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;
      MakeInvoiceRenderReady(InvoiceHeader.Id, InvoiceHeader);

      // If BillingType is Payables instantiate SourceCode Vat Total grid
      if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables)
      {
        var availableVatGrid = new AvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action("AvailableEmptySourceCodeVatTotalGridData"));
        ViewData["VatGrid"] = availableVatGrid.Instance;
      }

      return View("View", InvoiceHeader);
    }

    // SCP#419601: SRM: PAX permissions issue [Bug 9703 - Rel 1.7.13.0 ]
    // [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View)]
    public ActionResult PrimeBillingListView(string invoiceId)
    {
      // Create grid instance for PrimeBilling grid
      var primeBillingGrid = new PrimeBillingGrid(ViewDataConstants.PrimeBillingGrid, Url.Action(PrimeBillingGridAction, new { invoiceId }));
      ViewData[ViewDataConstants.PrimeBillingGrid] = primeBillingGrid.Instance;

      return View("PrimeBillingList", InvoiceHeader);
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

    // SCP#419601: SRM: PAX permissions issue [Bug 9703 - Rel 1.7.13.0 ]
    // [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.View)]
    [HttpGet]
    public ActionResult PrimeBillingView(string invoiceId, string transactionId)
    {
      var couponRecord = GetCouponRecord(transactionId, invoiceId);

      return View("PrimeBillingView", couponRecord);
    }

    private PrimeCoupon GetCouponRecord(string transactionId, string invoiceId)
    {
      var couponRecord = _nonSamplingInvoiceManager.GetCouponRecordDetails((transactionId));
      couponRecord.Invoice = InvoiceHeader;
      couponRecord.LastUpdatedBy = SessionUtil.UserId;
      return couponRecord;
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleInvoice.Download)]
    [HttpGet]
    public FileStreamResult PrimeBillingAttachmentDownload(string invoiceId, string transactionId)
    {
        var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingInvoiceManager.GetCouponLevelAttachmentDetails(transactionId) };

        return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }
  }
}
