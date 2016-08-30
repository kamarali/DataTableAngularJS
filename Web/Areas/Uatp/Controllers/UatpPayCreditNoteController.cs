using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.Pax;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Web.Controllers.Base;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Uatp.Controllers
{
    public class UatpPayCreditNoteController : MiscUatpControllerBase
    {
        public UatpPayCreditNoteController(IUatpInvoiceManager uatpInvoiceManager, IReferenceManager referenceManager, IMemberManager memberManager)
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

        [ISAuthorize(Business.Security.Permissions.UATP.Receivables.CreditNote.CreateOrEdit)]
        [HttpPost]
        public override JsonResult RejectInvoice(string invoiceId, string lineItemId, string searchType, string invoiceType)
        {
            return base.RejectInvoice(invoiceId, lineItemId, searchType, "UatpInvoice");
        }

        [ISAuthorize(Business.Security.Permissions.UATP.Payables.CreditNoteBillings.View)]
        [HttpGet]
        public override ActionResult View(string invoiceId)
        {
            return base.View(invoiceId);
        }

        [ISAuthorize(Business.Security.Permissions.UATP.Payables.CreditNoteBillings.View)]
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
        [ISAuthorize(Business.Security.Permissions.UATP.Payables.CreditNoteBillings.View)]
        [HttpGet]
        public override ActionResult LineItemDetailView(string invoiceId, string lineItemId, string lineItemDetailId)
        {
            return base.LineItemDetailView(invoiceId, lineItemId, lineItemDetailId);

        }

        [ISAuthorize(Business.Security.Permissions.UATP.Payables.CreditNoteBillings.View)]
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
        [ISAuthorize(Business.Security.Permissions.UATP.Payables.CreditNoteBillings.Download)]
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
        [ISAuthorize(Business.Security.Permissions.UATP.Payables.CreditNoteBillings.Download)]
        [HttpGet]
        public override FileStreamResult LineItemAttachmentDownload(string invoiceId, string lineItemId, string lineItemDetailId)
        {
            return base.LineItemAttachmentDownload(invoiceId, lineItemId, lineItemDetailId);
        }

    }
}