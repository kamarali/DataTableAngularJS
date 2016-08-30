using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Web.Areas.Pax.Controllers.Base;
using Iata.IS.Web.Util.Filters;
using TransactionType = Iata.IS.Model.Enums.TransactionType;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
  /// <summary>
  /// This class overrides all the action methods from the base class simply in order to be able to give permissions at a granular level.
  /// </summary>
  public class FormXFController : SamplingRejectionControllerBase
  {
    private readonly ISamplingFormXfManager _samplingFormXfManager;
    public FormXFController(ISamplingFormXfManager samplingFormXFManager, IReferenceManager referenceManager, IMemberManager memberManager)
      : base(samplingFormXFManager, referenceManager, memberManager)
    {
      _samplingFormXfManager = samplingFormXFManager;
    }

    protected override int BillingCodeId
    {
      get { return Convert.ToInt32(BillingCode.SamplingFormXF); }
    }

    protected override int TransactionTypeId
    {
      get { return Convert.ToInt32(TransactionType.SamplingFormXF); }
      set
      {
        base.TransactionTypeId = value;
      }
    }

    protected override string SamplingFormName
    {
      get
      {
        return Messages.FormXF;
      }
    }
    
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [HttpGet]
    public override ActionResult Create()
    {
      return base.Create();
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [HttpPost]
    public override ActionResult Create(PaxInvoice invoice)
    {
      return base.Create(invoice);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public override ActionResult Edit(string invoiceId)
    {
      return base.Edit(invoiceId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.View)]
    [HttpGet]
    public override ActionResult View(string invoiceId)
    {
      return base.View(invoiceId);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "Edit")]
    public override ActionResult Edit(string invoiceId, PaxInvoice invoice)
    {
      return base.Edit(invoiceId, invoice);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.View)]
    [HttpGet]
    [Obsolete]
    public override ActionResult Details(string invoiceId)
    {
      return base.Details(invoiceId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.View)]
    [HttpGet]
    [Obsolete]
    public override ActionResult DetailsView(string invoiceId)
    {
      return base.DetailsView(invoiceId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.View)]
    [HttpGet]
    public override JsonResult GetSummaryList(string invoiceId)
    {
      return base.GetSummaryList(invoiceId);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.Validate)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "Edit")]
    public override ActionResult ValidateInvoice(string invoiceId)
    {
      return base.ValidateInvoice(invoiceId);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.Submit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "Edit")]
    public override ActionResult Submit(string invoiceId)
    {
      return base.Submit(invoiceId);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "transactionId", TableName = TransactionTypeTable.PAX_REJECTION_MEMO)]
    public override JsonResult RMDelete(string transactionId)
    {
      return base.RMDelete(transactionId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.View)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    [Obsolete]
    public override ActionResult RMList(string invoiceId)
    {
      return base.RMList(invoiceId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.View)]
    [HttpGet]
    [Obsolete]
    public override ActionResult RMListView(string invoiceId)
    {
      return base.RMListView(invoiceId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.View)]
    [HttpGet]
    public override JsonResult GetRejectionMemoList(string invoiceId)
    {
      return base.GetRejectionMemoList(invoiceId);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public override ActionResult RMCreate(string invoiceId)
    {
      return base.RMCreate(invoiceId);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public override ActionResult RMCreate(string invoiceId, RejectionMemo rejectionMemoRecord)
    {
      return base.RMCreate(invoiceId, rejectionMemoRecord);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public override ActionResult RMEdit(string invoiceId, string transactionId)
    {
      return base.RMEdit(invoiceId, transactionId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.View)]
    [HttpGet]
    public override ActionResult RMView(string invoiceId, string transactionId)
    {
      return base.RMView(invoiceId, transactionId);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(TransactionParamName = "transactionId", IsJson = false, TableName = TransactionTypeTable.PAX_REJECTION_MEMO)]
    public override ActionResult RMEdit(string transactionId, RejectionMemo rejectionMemoRecord)
    {
      return base.RMEdit(transactionId, rejectionMemoRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public override JsonResult RMFormFXFAttachmentUpload(string invoiceId, string transactionId)
    {
      return base.RMFormFXFAttachmentUpload(invoiceId, transactionId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.Download)]
    [HttpGet]
    public override FileStreamResult RMFormFXFAttachmentDownload(string invoiceId, string transactionId)
    {
      return base.RMFormFXFAttachmentDownload(invoiceId, transactionId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.View)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public override ActionResult RMCouponList(string transactionId)
    {
      return base.RMCouponList(transactionId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.View)]
    [HttpGet]
    public override ActionResult RMCouponListView(string transactionId)
    {
      return base.RMCouponListView(transactionId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.View)]
    [HttpGet]
    public override JsonResult GetRMCouponList(string transactionId)
    {
      return base.GetRMCouponList(transactionId);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public override ActionResult RMCouponCreate(string transactionId)
    {
      return base.RMCouponCreate(transactionId);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(TransactionParamName = "transactionId", IsJson = false, TableName = TransactionTypeTable.PAX_REJECTION_MEMO)]
    public override ActionResult RMCouponCreate(string transactionId, RMCoupon couponBreakdownRecord)
    {
      return base.RMCouponCreate(transactionId, couponBreakdownRecord);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [HttpGet]
    public override ActionResult RMCouponEdit(string transactionId, string couponId)
    {
      return base.RMCouponEdit(transactionId, couponId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.View)]
    [HttpGet]
    public override ActionResult RMCouponView(string invoiceId, string transactionId, string couponId)
    {
      return base.RMCouponView(invoiceId, transactionId, couponId);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(TransactionParamName = "couponId", IsJson = false, TableName = TransactionTypeTable.PAX_RM_COUPON_BREAKDOWN)]
    public override ActionResult RMCouponEdit(string transactionId, string couponId, RMCoupon couponBreakdownRecord)
    {
      return base.RMCouponEdit(transactionId, couponId, couponBreakdownRecord);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "couponId", TableName = TransactionTypeTable.PAX_RM_COUPON_BREAKDOWN)]
    public override JsonResult RMCouponDelete(string couponId)
    {
      return base.RMCouponDelete(couponId);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public override JsonResult RMCouponFormFXFAttachmentUpload(string invoiceId, string transactionId)
    {
      return base.RMCouponFormFXFAttachmentUpload(invoiceId, transactionId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormXF.Download)]
    [HttpGet]
    public override FileStreamResult RMCouponFormFXFAttachmentDownload(string invoiceId, string couponId)
    {
      return base.RMCouponFormFXFAttachmentDownload(invoiceId, couponId);
    }
    
    /// <summary>
    /// Gets the Form F linking details for the given Form XF RM.
    /// </summary>
    /// <param name="reasonCode"></param>
    /// <param name="yourInvoiceNumber"></param>
    /// <param name="yourBillingMonth"></param>
    /// <param name="yourBillingYear"></param>
    /// <param name="yourBillingPeriod"></param>
    /// <param name="yourRejectionMemoNumber"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <param name="rejectingInvoiceId"></param>
    /// <param name="provisionalBillingMonth"></param>
    /// <param name="provisionalBillingYear"></param>
    /// <returns></returns>
    [HttpPost]
    public JsonResult GetLinkedFormFDetails(string reasonCode, string yourInvoiceNumber, int yourBillingMonth, int yourBillingYear, int yourBillingPeriod, string yourRejectionMemoNumber, int billingMemberId, int billedMemberId, Guid rejectingInvoiceId, int provisionalBillingMonth, int provisionalBillingYear)
    {
      var rmLinkingCriteria = new SamplingRMLinkingCriteria { ReasonCode = reasonCode, BilledMemberId = billedMemberId, BillingMemberId = billingMemberId, BillingMonth = yourBillingMonth, BillingPeriod = yourBillingPeriod, BillingYear = yourBillingYear, InvoiceNumber = yourInvoiceNumber, ProvBillingMonth = provisionalBillingMonth, ProvBillingYear = provisionalBillingYear, RejectingInvoiceId = rejectingInvoiceId, RejectionMemoNumber = yourRejectionMemoNumber };
      var linkedMemoDetails = _samplingFormXfManager.GetLinkedFormFDetails(rmLinkingCriteria);

      return Json(linkedMemoDetails);
    }

    /// <summary>
    /// Used to fetch the memo amounts of a single linked Form F RM record.
    /// </summary>
    /// <param name="form">The form.</param>
    /// <returns></returns>
    [HttpPost]
    public JsonResult GetLinkedMemoAmountDetails(FormCollection form)
    {
      var rmLinking = new JavaScriptSerializer().Deserialize(form[0], typeof(RMLinkingCriteria));
      var criteria = rmLinking as RMLinkingCriteria;
      var rejectionMemoResult = _samplingFormXfManager.GetLinkedMemoAmountDetails(criteria);

      return Json(rejectionMemoResult);
    }

    [HttpPost]
    public JsonResult GetRMCouponBreakdownDetails(string issuingAirline, int couponNo, long ticketDocNo, Guid rmId, int billingMemberId, int billedMemberId)
    {
      var rmLinkedCoupons = _samplingFormXfManager.GetSamplingCouponBreakdownRecordDetails(issuingAirline, couponNo, ticketDocNo, rmId, billingMemberId, billedMemberId);

      return Json(rmLinkedCoupons);
    }

    /// <summary>
    /// Get the single record details from the list of RM coupon
    /// </summary>
    [HttpPost]
    public JsonResult GetRMCouponBreakdownSingleRecordDetails(Guid couponId, Guid rejectionMemoId, int billingMemberId, int billedMemberId)
    {
      var rmLinkedSingleCouponsDetails = _samplingFormXfManager.GetRMCouponBreakdownSingleRecordDetails(couponId, rejectionMemoId, billingMemberId, billedMemberId);

      return Json(rmLinkedSingleCouponsDetails);
    }
  }
}
