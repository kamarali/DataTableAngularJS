using System;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Core;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.Areas.Pax.Controllers.Base;
using Iata.IS.Web.Util.Filters;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
  /// <summary>
  /// This class overrides all the action methods from the base class simply in order to be able to give permissions at a granular level.
  /// </summary>
  public class FormFPayablesController : SamplingRejectionControllerBase
  {
    private readonly ISamplingFormFManager _samplingFormFManager;
    public FormFPayablesController(ISamplingFormFManager samplingFormFManager, IReferenceManager referenceManager, IMemberManager memberManager)
        : base(samplingFormFManager, referenceManager, memberManager)
    {
      _samplingFormFManager = samplingFormFManager;
    }

    protected override int BillingCodeId
    {
      get { return Convert.ToInt32(BillingCode.SamplingFormF); }
    }

    protected override int TransactionTypeId
    {
      get { return Convert.ToInt32(TransactionType.SamplingFormF); }
      set
      {
        base.TransactionTypeId = value;
      }
    }

    protected override string SamplingFormName
    {
      get
      {
        return Messages.FormF;
      }
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormF.View)]
    [HttpGet]
    public override ActionResult View(string invoiceId)
    {
      return base.View(invoiceId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormF.View)]
    [HttpGet]
    [Obsolete]
    public override ActionResult Details(string invoiceId)
    {
      return base.Details(invoiceId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormF.View)]
    [HttpGet]
    [Obsolete]
    public override ActionResult DetailsView(string invoiceId)
    {
      return base.DetailsView(invoiceId);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormF.View)]
    [HttpGet]
    public override JsonResult GetSummaryList(string invoiceId)
    {
      return base.GetSummaryList(invoiceId);
    }


    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormF.View)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    [Obsolete]
    public override ActionResult RMList(string invoiceId)
    {
      return base.RMList(invoiceId);
    }

    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormF.View)]
    [HttpGet]
    [Obsolete]
    public override ActionResult RMListView(string invoiceId)
    {
      return base.RMListView(invoiceId);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormF.View)]
    [HttpGet]
    public override JsonResult GetRejectionMemoList(string invoiceId)
    {
      return base.GetRejectionMemoList(invoiceId);
    }

   
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormF.View)]
    [HttpGet]
    public override ActionResult RMView(string invoiceId, string transactionId)
    {
      return base.RMView(invoiceId, transactionId);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormF.Download)]
    [HttpGet]
    public override FileStreamResult RMFormFXFAttachmentDownload(string invoiceId, string transactionId)
    {
      return base.RMFormFXFAttachmentDownload(invoiceId, transactionId);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormF.View)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public override ActionResult RMCouponList(string transactionId)
    {
      return base.RMCouponList(transactionId);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormF.View)]
    [HttpGet]
    public override ActionResult RMCouponListView(string transactionId)
    {
      return base.RMCouponListView(transactionId);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormF.View)]
    [HttpGet]
    public override JsonResult GetRMCouponList(string transactionId)
    {
      return base.GetRMCouponList(transactionId);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormF.View)]
    [HttpGet]
    public override ActionResult RMCouponView(string invoiceId, string transactionId, string couponId)
    {
      return base.RMCouponView(invoiceId, transactionId, couponId);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.SampleFormF.Download)]
    [HttpGet]
    public override FileStreamResult RMCouponFormFXFAttachmentDownload(string invoiceId, string couponId)
    {
      return base.RMCouponFormFXFAttachmentDownload(invoiceId, couponId);
    }

    /// <summary>
    /// call from jquery using Ajax and json, for getting the FORM DE coupon break down details
    /// </summary>
    /// <param name="issuingAirline"> Issuing Airline.</param>
    /// <param name="couponNo">Coupon Number.</param>
    /// <param name="ticketDocNo">Ticket document Number.</param>
    /// <param name="rmId"></param>
    /// <param name="billingMemberId">Billing member ID.</param>
    /// <param name="billedMemberId">Billed member ID.</param>
    /// <returns></returns>
    [HttpPost]
    public virtual JsonResult GetSamplingCouponBreakDownDetails(string issuingAirline, int couponNo, long ticketDocNo, Guid rmId, int billingMemberId, int billedMemberId)
    {
      var deLinkedCoupons = _samplingFormFManager.GetSamplingCouponBreakdownRecordDetails(issuingAirline, couponNo, ticketDocNo, rmId, billingMemberId, billedMemberId);  
      return Json(deLinkedCoupons);
    }

    /// <summary>
    /// Get the single record details from the list of FORM DE coupon.
    /// </summary>
    /// <param name="couponId">Coupon ID.</param>
    /// <param name="rejectionMemoId">Rejection Memo ID.</param>
    /// <param name="billingMemberId">Billing member ID.</param>
    /// <param name="billedMemberId">Billed Member ID.</param>
    /// <returns></returns>
    [HttpPost]
    public JsonResult GetSamplingCouponBreakdownSingleRecordDetails(string couponId, string rejectionMemoId, int billingMemberId, int billedMemberId)
    {
      var deLinkedSingleCouponsDetails = _samplingFormFManager.GetSamplingCouponBreakdownSingleRecordDetails(couponId.ToGuid(), rejectionMemoId.ToGuid(), billingMemberId, billedMemberId);
      return Json(deLinkedSingleCouponsDetails);
    }

    /// <summary>
    /// Used to validate if linking is successful at RM level.
    /// </summary>
    /// <param name="reasonCode"></param>
    /// <param name="yourInvoiceNumber"></param>
    /// <param name="yourBillingMonth"></param>
    /// <param name="yourBillingYear"></param>
    /// <param name="yourBillingPeriod"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <param name="rejectingInvoiceId"></param>
    /// <param name="provisionalBillingMonth"></param>
    /// <param name="provisionalBillingYear"></param>
    /// <returns></returns>
    [HttpPost]
    public JsonResult GetLinkedFormDEDetails(string reasonCode, string yourInvoiceNumber, int yourBillingMonth, int yourBillingYear, int yourBillingPeriod, int billingMemberId, int billedMemberId, Guid rejectingInvoiceId, int provisionalBillingMonth, int provisionalBillingYear)
    {
      var rmLinkingCriteria = new SamplingRMLinkingCriteria{ ReasonCode = reasonCode, BilledMemberId = billedMemberId, BillingMemberId = billingMemberId, BillingMonth = yourBillingMonth, BillingPeriod = yourBillingPeriod, BillingYear = yourBillingYear, InvoiceNumber = yourInvoiceNumber, ProvBillingMonth = provisionalBillingMonth, ProvBillingYear = provisionalBillingYear, RejectingInvoiceId = rejectingInvoiceId };
      var linkedCouponDetails = _samplingFormFManager.GetLinkedFormDEDetails(rmLinkingCriteria);

      return Json(linkedCouponDetails);
    }
  }
}
