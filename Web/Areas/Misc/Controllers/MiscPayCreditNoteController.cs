using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Web.Controllers.Base;
using Iata.IS.Web.Util.Filters;


namespace Iata.IS.Web.Areas.Misc.Controllers
{
    public class MiscPayCreditNoteController : MiscUatpControllerBase
    {
        public MiscPayCreditNoteController(IMiscInvoiceManager miscInvoiceManager, IReferenceManager referenceManager, IMemberManager memberManager)
      : base(miscInvoiceManager, referenceManager, memberManager)
    {
    }

        protected override BillingCategoryType BillingCategory
        {
            get
            {
                return BillingCategoryType.Misc;
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

        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        // This is common method called from Misc Payable Credit Note View and Misc Daily Bilateral Credit Note View
        // [ISAuthorize(Business.Security.Permissions.Misc.Receivables.CreditNote.CreateOrEdit)]
        [HttpPost]
        public override JsonResult RejectInvoice(string invoiceId, string lineItemId, string searchType, string invoiceType)
        {
            return base.RejectInvoice(invoiceId, lineItemId, searchType, "MiscInvoice");
        }

        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        // This is common method called from Misc Payable Credit Note View and Misc Daily Bilateral Credit Note View
        // [ISAuthorize(Business.Security.Permissions.Misc.Payables.CreditNoteBillings.View)]
        [HttpGet]
        public override ActionResult View(string invoiceId)
        {
            return base.View(invoiceId);
        }

        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        // This is common method called from Misc Payable Credit Note View and Misc Daily Bilateral Credit Note View
        // [ISAuthorize(Business.Security.Permissions.Misc.Payables.CreditNoteBillings.View)]
        [HttpGet]
        public override ActionResult ShowDetails(string invoiceId)
        {
            return base.ShowDetails(invoiceId);
        }

        /// <summary>
        /// View for LineItemDetail
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="lineItemId"></param>
        /// <param name="lineItemDetailId"></param>
        /// <returns></returns>
        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        // This is common method called from Misc Payable Credit Note View and Misc Daily Bilateral Credit Note View
        // [ISAuthorize(Business.Security.Permissions.Misc.Payables.CreditNoteBillings.View)]
        [HttpGet]
        public override ActionResult LineItemDetailView(string invoiceId, string lineItemId, string lineItemDetailId)
        {
            return base.LineItemDetailView(invoiceId, lineItemId, lineItemDetailId);
        }

        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        // This is common method called from Misc Payable Credit Note View and Misc Daily Bilateral Credit Note View
        // [ISAuthorize(Business.Security.Permissions.Misc.Payables.CreditNoteBillings.View)]
        [HttpGet]
        public override ActionResult LineItemView(string lineItemId, string invoiceId)
        {
            return base.LineItemView(lineItemId, invoiceId);
        }

        /// <summary>
        /// Download Invoice attachment
        ///  </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="lineItemId">lineItem id</param>
        /// <returns></returns>
        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        // This is common method called from Misc Payable Credit Note View and Misc Daily Bilateral Credit Note View
        // [ISAuthorize(Business.Security.Permissions.Misc.Payables.CreditNoteBillings.Download)]
        [HttpGet]
        public override FileStreamResult InvoiceAttachmentDownload(string invoiceId, string lineItemId)
        {
            return base.InvoiceAttachmentDownload(invoiceId, lineItemId);
        }

        /// <summary>
        /// Used for attachment link on Line Item page.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="lineItemId"></param>
        /// <param name="lineItemDetailId">This is the attachment ID.</param>
        /// <returns></returns>
        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        // This is common method called from Misc Payable Credit Note View and Misc Daily Bilateral Credit Note View
        // [ISAuthorize(Business.Security.Permissions.Misc.Payables.CreditNoteBillings.Download)]
        [HttpGet]
        public override FileStreamResult LineItemAttachmentDownload(string invoiceId, string lineItemId, string lineItemDetailId)
        {
            return base.LineItemAttachmentDownload(invoiceId, lineItemId, lineItemDetailId);
        }



    }
}
