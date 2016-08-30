using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.Areas.Pax.Controllers.Base;
using Iata.IS.Web.UIModel.Grid.Common;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
  public class CreditNotePayablesController : PaxInvoiceControllerBase
  {
    private readonly INonSamplingCreditNoteManager _nonSamplingCreditNoteManager;
    private readonly IReferenceManager _referenceManager;
    private const string CMCouponGridAction = "CMCouponGridData";
    private const string CreditMemoGridAction = "CreditMemoGridData";
    private const string SourceCodeGridAction = "SourceCodeGridData";

    public CreditNotePayablesController(INonSamplingCreditNoteManager nonSamplingCreditNoteManager, IReferenceManager referenceManager, IMemberManager memberManager)
      : base(nonSamplingCreditNoteManager)
    {
      _nonSamplingCreditNoteManager = nonSamplingCreditNoteManager;
      _referenceManager = referenceManager;
      MemberManager = memberManager;
    }

    protected override int BillingCodeId
    {
      get { return Convert.ToInt32(BillingCode.NonSampling); }
    }

    protected override InvoiceType InvoiceType
    {
      get { return InvoiceType.CreditNote; }
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleCreditNote.View)]
    public new ActionResult View(string invoiceId)
    {
      ViewData[ViewDataConstants.TransactionExists] = _nonSamplingCreditNoteManager.IsCreditMemoExists(invoiceId);
      MakeInvoiceRenderReady(InvoiceHeader.Id, InvoiceHeader);

      // Creating empty InvoiceTotalRecord object in case if InvoiceTotal Records is
      // not retrieve from database.
      if (InvoiceHeader.InvoiceTotalRecord == null)
      {
        InvoiceHeader.InvoiceTotalRecord = new InvoiceTotal();
      }

      //Initialize source code grid
      var sourceCodeGrid = new SourceCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SourceCodeGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);
      ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;
      //Initialize Credit Memo grid
      var creditMemoGrid = new CreditMemoGrid(ControlIdConstants.CreditMemoGrid, Url.Action(CreditMemoGridAction, new { invoiceId }));
      ViewData[ViewDataConstants.CreditMemoGrid] = creditMemoGrid.Instance;

      // If BillingType is Payables instantiate SourceCode Vat Total grid
      if (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
      {
        var availableVatGrid = new AvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action("AvailableEmptySourceCodeVatTotalGridData"));
        ViewData["VatGrid"] = availableVatGrid.Instance;
      }

      return View("Edit", InvoiceHeader);
    }

    public JsonResult CreditMemoGridData(string invoiceId)
    {
      var creditMemoGrid = new CreditMemoGrid(ControlIdConstants.CreditMemoGrid, Url.Action(CreditMemoGridAction, new { invoiceId }));

      var creditMemoList = _nonSamplingCreditNoteManager.GetCreditMemoList(invoiceId);

      return creditMemoGrid.DataBind(creditMemoList.AsQueryable());
    }

    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleCreditNote.View)]
    public ActionResult CreditMemoView(string invoiceId, string transactionId)
    {
      var creditMemo = GetCreditMemo(transactionId, invoiceId);
      ViewData[ViewDataConstants.BreakdownExists] = _nonSamplingCreditNoteManager.GetCreditMemoCouponBreakdownCount(transactionId) > 0 ? true : false;
      return View("CMEdit", creditMemo);
    }

    /// <summary>
    /// Download  Credit Memo attachment
    /// </summary>
    /// <param name="invoiceId">Invoice id</param>
    /// <param name="transactionId">Transaction id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleCreditNote.Download)]
    [HttpGet]
    public FileStreamResult CreditMemoAttachmentDownload(string invoiceId, string transactionId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingCreditNoteManager.GetCreditMemoAttachmentDetails(transactionId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [RestrictUnauthorizedUpdate]
    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleCreditNote.View)]
    public ActionResult CreditMemoEdit(string invoiceId, string transactionId)
    {
      var creditMemo = GetCreditMemo(transactionId, invoiceId);
      ViewData[ViewDataConstants.BreakdownExists] = _nonSamplingCreditNoteManager.GetCreditMemoCouponBreakdownCount(transactionId) > 0 ? true : false;

      return View("CMEdit", creditMemo);
    }

    private CreditMemo GetCreditMemo(string transactionId, string invoiceId)
    {
      var creditMemo = _nonSamplingCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      creditMemo.LastUpdatedBy = SessionUtil.UserId;
      // Depending on TransactionType and reasonCode retrieve whether Coupon breakdown is mandatory or not
      var isCouponBreakdownMandatory = _referenceManager.GetReasonCode(creditMemo.ReasonCode, (int)TransactionType.CreditMemo).CouponAwbBreakdownMandatory;

      // Set Coupon breakdown value
      creditMemo.CouponAwbBreakdownMandatory = isCouponBreakdownMandatory;

      creditMemo.Invoice = InvoiceHeader;
      var creditMemoCouponGrid = new CreditMemoCouponGrid(ControlIdConstants.CreditMemoCouponGrid, Url.Action(CMCouponGridAction, new { transactionId }));
      ViewData[ViewDataConstants.CMCouponGrid] = creditMemoCouponGrid.Instance;

      return creditMemo;
    }

    public JsonResult CMCouponGridData(string transactionId)
    {
      var creditMemoCouponGrid = new CreditMemoCouponGrid(ControlIdConstants.CreditMemoCouponGrid, Url.Action(CMCouponGridAction, new { transactionId }));
      var creditMemoCouponBreakdownList = _nonSamplingCreditNoteManager.GetCreditMemoCouponBreakdownList(transactionId);

      return creditMemoCouponGrid.DataBind(creditMemoCouponBreakdownList.AsQueryable());
    }

    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleCreditNote.View)]
    public ActionResult CreditMemoCouponView(string invoiceId, string transactionId, string couponId)
    {
      var couponCreditBreakdownRecord = GetCouponCreditBreakdownRecord(couponId, transactionId, invoiceId);
      return View("CMCouponEdit", couponCreditBreakdownRecord);
    }

    private CMCoupon GetCouponCreditBreakdownRecord(string couponId, string transactionId, string invoiceId)
    {
      var couponCreditBreakdownRecord = _nonSamplingCreditNoteManager.GetCreditMemoCouponDetails(couponId);
      couponCreditBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
      couponCreditBreakdownRecord.CreditMemoRecord = _nonSamplingCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      couponCreditBreakdownRecord.CreditMemoRecord.Invoice = InvoiceHeader;

      // added code to remove the extra special char
      if (!string.IsNullOrEmpty(couponCreditBreakdownRecord.ProrateSlipDetails))
        couponCreditBreakdownRecord.ProrateSlipDetails =
          couponCreditBreakdownRecord.ProrateSlipDetails.Replace("\n", string.Empty).Replace("\r", string.Empty);

      return couponCreditBreakdownRecord;
    }

    /// <summary>
    /// Download  Credit Memo Coupon attachment
    ///  </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="couponId">Coupon Id</param>
    /// <returns></returns>
    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.NonSampleCreditNote.Download)]
    public FileStreamResult CreditMemoCouponAttachmentDownload(string invoiceId, string couponId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingCreditNoteManager.GetCreditMemoCouponAttachmentDetails(couponId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }
  }
}