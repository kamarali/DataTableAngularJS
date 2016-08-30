using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.Pax;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Web.Controllers.Base;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Uatp.Controllers
{
  public class UatpCreditNoteController : MiscUatpControllerBase
  {
    public UatpCreditNoteController(IUatpInvoiceManager uatpInvoiceManager, IReferenceManager referenceManager, IMemberManager memberManager)
      : base(uatpInvoiceManager, referenceManager, memberManager)
    {

    }

    protected override BillingCategoryType BillingCategory
    {
      get
      {
        return BillingCategoryType.Uatp;
      }
      set
      {
        base.BillingCategory = value;
      }
    }

    protected override InvoiceType InvoiceType
    {
      get
      {
        return InvoiceType.CreditNote;
      }
    }
    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpGet]
    public override ActionResult Create()
    {
        return base.Create();
    }

    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public override ActionResult Create(MiscUatpInvoice invoiceHeader)
    {
        return base.Create(invoiceHeader);
    }

    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpGet]
    [RestrictUnauthorizedUpdate]
    public override ActionResult Edit(string invoiceId)
    {
        return base.Edit(invoiceId);
    }

    [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.View)]
    [HttpGet]
    public override ActionResult View(string invoiceId)
    {
        return base.View(invoiceId);
    }

    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.Validate)]
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "Edit")]
    public override ActionResult ValidateInvoice(string invoiceId)
    {
        return base.ValidateInvoice(invoiceId);
    }

    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.Submit)]
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "Edit")]
    public override ActionResult Submit(string invoiceId)
    {
        return base.Submit(invoiceId);
    }

    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpPost]
    public override JsonResult RejectInvoice(string invoiceId, string lineItemId, string searchType,string invoiceType)
    {
        return base.RejectInvoice(invoiceId, lineItemId, searchType,"UatpInvoice");
    }

    [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.View)]
    [HttpGet]
    public override ActionResult ShowDetails(string invoiceId)
    {
        return base.ShowDetails(invoiceId);
    }

    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public override ActionResult Edit(string invoiceId, MiscUatpInvoice invoiceHeader)
    {
        return base.Edit(invoiceId, invoiceHeader);
    }

    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpGet]
    [RestrictUnauthorizedUpdate]
    public override ActionResult LineItemCreate(string invoiceId)
    {
        return base.LineItemCreate(invoiceId);
    }

    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "lineItem.InvoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public override ActionResult LineItemCreate(LineItem lineItem)
    {
        return base.LineItemCreate(lineItem);
    }

    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpGet]
    [RestrictUnauthorizedUpdate]
    public override ActionResult LineItemDetailCreate(string invoiceId, string lineItemId)
    {
        return base.LineItemDetailCreate(invoiceId, lineItemId);
    }

    /// <summary>
    /// Create LineItemDetail
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetail"></param>
    /// <returns></returns>
    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public override ActionResult LineItemDetailCreate(string invoiceId, string lineItemId, LineItemDetail lineItemDetail)
    {
        return base.LineItemDetailCreate(invoiceId, lineItemId, lineItemDetail);
    }

    /// <summary>
    /// Add LineItemDetail and duplicate record
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetail"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "LineItemDetailCreate")]
    public override ActionResult LineItemDetailDuplicate(string invoiceId, string lineItemId, LineItemDetail lineItemDetail)
    {
        return base.LineItemDetailDuplicate(invoiceId, lineItemId, lineItemDetail);
    }

    /// <summary>
    /// Method to save LineItemDetail and return to Edit LineItem page
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetail"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "LineItemEdit")]
    public override ActionResult LineItemDetailReturn(string invoiceId, string lineItemId, LineItemDetail lineItemDetail)
    {
        return base.LineItemDetailReturn(invoiceId, lineItemId, lineItemDetail);
    }

    /// <summary>
    /// View for LineItemDetail
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetailId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.View)]
    [HttpGet]
    public override ActionResult LineItemDetailView(string invoiceId, string lineItemId, string lineItemDetailId)
    {
        return base.LineItemDetailView(invoiceId, lineItemId, lineItemDetailId);
    }

    /// <summary>
    /// Edit LineItemDetail Get method
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetailId"></param>
    /// <returns></returns>
    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpGet]
    [RestrictUnauthorizedUpdate]
    public override ActionResult LineItemDetailEdit(string invoiceId, string lineItemId, string lineItemDetailId)
    {
        return base.LineItemDetailEdit(invoiceId, lineItemId, lineItemDetailId);
    }

    /// <summary>
    /// Edit LineItemDetail
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetailId"></param>
    /// <param name="lineItemDetail"></param>
    /// <returns></returns>
    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public override ActionResult LineItemDetailEdit(string invoiceId, string lineItemId, string lineItemDetailId, LineItemDetail lineItemDetail)
    {
        return base.LineItemDetailEdit(invoiceId, lineItemId, lineItemDetailId, lineItemDetail);
    }

    /// <summary>
    /// Update LineItemDetail and clone
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetailId"></param>
    /// <param name="lineItemDetail"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "LineItemDetailCreate")]
    public override ActionResult LineItemDetailClone(string invoiceId, string lineItemId, string lineItemDetailId, LineItemDetail lineItemDetail)
    {
        return base.LineItemDetailClone(invoiceId, lineItemId, lineItemDetailId, lineItemDetail);
    }

    /// <summary>
    /// Update lineiTemDetail and redirect to Edit LineItem page
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetailId"></param>
    /// <param name="lineItemDetail"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "LineItemEdit")]
    public override ActionResult LineItemDetailEditAndReturn(string invoiceId, string lineItemId, string lineItemDetailId, LineItemDetail lineItemDetail)
    {
        return base.LineItemDetailEditAndReturn(invoiceId, lineItemId, lineItemDetailId, lineItemDetail);
    }

    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpGet]
    [RestrictUnauthorizedUpdate]
    public override ActionResult LineItemEdit(string lineItemId, string invoiceId)
    {
        return base.LineItemEdit(lineItemId, invoiceId);
    }

    [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.View)]
    [HttpGet]
    public override ActionResult LineItemView(string lineItemId, string invoiceId)
    {
        return base.LineItemView(lineItemId, invoiceId);
    }

    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public override ActionResult LineItemEdit(string invoiceId, string lineItemId, LineItem lineItem)
    {
        return base.LineItemEdit(invoiceId, lineItemId, lineItem);
    }

    /// <summary>
    /// Upload Rejection Memo Coupon Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <returns></returns>
    //[ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public override JsonResult InvoiceAttachmentUpload(string invoiceId)
    {
        return base.InvoiceAttachmentUpload(invoiceId);
    }

    /// <summary>
    /// Download Invoice attachment
    ///  </summary>
    /// <param name="invoiceId">Invoice id</param>
    /// <param name="lineItemId">lineItem id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.Download)]
    [HttpGet]
    public override FileStreamResult InvoiceAttachmentDownload(string invoiceId, string lineItemId)
    {
        return base.InvoiceAttachmentDownload(invoiceId, lineItemId);
    }

    [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpPost]
    public override JsonResult DeleteAttachment(string id, bool isSupportingDoc = false)
    {
        return base.DeleteAttachment(id, isSupportingDoc);
    }

    /// <summary>
    /// Used for attachment link on Line Item page.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="lineItemId"></param>
    /// <param name="lineItemDetailId">This is the attachment ID.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.Download)]
    [HttpGet]
    public override FileStreamResult LineItemAttachmentDownload(string invoiceId, string lineItemId, string lineItemDetailId)
    {
        return base.LineItemAttachmentDownload(invoiceId, lineItemId, lineItemDetailId);
    }

    [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(TransactionParamName = "lineItemId", IsJson = true, TableName = TransactionTypeTable.MU_LINE_ITEM)]
    public override JsonResult LineItemDelete(string lineItemId,string invoiceId)
    {
        //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
        //invoiceId as a new parametere is passed
        return base.LineItemDelete(lineItemId, invoiceId);
    }

    [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, TableName = TransactionTypeTable.INVOICE)]
    public override JsonResult LineItemDetailDelete(string lineItemDetailId, string invoiceId, string lineItemId)
    {
        return base.LineItemDetailDelete(lineItemDetailId, invoiceId, lineItemId);
    }
  }
}

