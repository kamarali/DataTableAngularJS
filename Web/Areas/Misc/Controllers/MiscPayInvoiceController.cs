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
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Misc.Controllers
{
    public class MiscPayInvoiceController : MiscUatpControllerBase
    {

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
                return Iata.IS.Model.Enums.InvoiceType.Invoice;
            }
        }

        public MiscPayInvoiceController(IMiscUatpInvoiceManager miscUatpInvoiceManager, IReferenceManager referenceManager, IMemberManager memberManager) 
            : base(miscUatpInvoiceManager, referenceManager, memberManager)
        {
        }

        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        // This is common method called from Misc Payable Invoice View and Misc Daily Bilateral Invoice View
        // [ISAuthorize(Business.Security.Permissions.Misc.Receivables.Invoice.CreateOrEdit)]
        [HttpPost]
        public override JsonResult RejectInvoice(string invoiceId, string lineItemId, string searchType, string invoiceType)
        {
            return base.RejectInvoice(invoiceId, lineItemId, searchType, "MiscInvoice");
        }


        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        // This is common method called from Misc Payable Invoice View and Misc Daily Bilateral Invoice View
        // [ISAuthorize(Business.Security.Permissions.Misc.Payables.Invoice.View)]
        [HttpGet]
        public override ActionResult View(string invoiceId)
        {
            ViewData[ViewDataConstants.BillingType] = Iata.IS.Web.Util.BillingType.Payables;
            return base.View(invoiceId);
        }


        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        // This is common method called from Misc Payable Invoice View and Misc Daily Bilateral Invoice View
        // [ISAuthorize(Business.Security.Permissions.Misc.Payables.Invoice.View)]
        [HttpGet]
        public override ActionResult ShowDetails(string invoiceId)
        {
            ViewData[ViewDataConstants.BillingType] = Iata.IS.Web.Util.BillingType.Payables;
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
        // This is common method called from Misc Payable Invoice View and Misc Daily Bilateral Invoice View
        // [ISAuthorize(Business.Security.Permissions.Misc.Payables.Invoice.View)]
        [HttpGet]
        public override ActionResult LineItemDetailView(string invoiceId, string lineItemId, string lineItemDetailId)
        {
            ViewData[ViewDataConstants.BillingType] = Iata.IS.Web.Util.BillingType.Payables;
            return base.LineItemDetailView(invoiceId, lineItemId, lineItemDetailId);
        }


        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        // This is common method called from Misc Payable Invoice View and Misc Daily Bilateral Invoice View
        // [ISAuthorize(Business.Security.Permissions.Misc.Payables.Invoice.View)]
        [HttpGet]
        public override ActionResult LineItemView(string lineItemId, string invoiceId)
        {
            ViewData[ViewDataConstants.BillingType] = Iata.IS.Web.Util.BillingType.Payables;
            return base.LineItemView(lineItemId, invoiceId);
        }


        /// <summary>
        /// Download Invoice attachment
        ///  </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="lineItemId">lineItem id</param>
        /// <returns></returns>
        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        // This is common method called from Misc Payable Invoice View and Misc Daily Bilateral View
        // [ISAuthorize(Business.Security.Permissions.Misc.Payables.Invoice.Download)]
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
        // This is common method called from Misc Payable Invoice View and Misc Daily Bilateral View
        // [ISAuthorize(Business.Security.Permissions.Misc.Payables.Invoice.Download)]
        [HttpGet]
        public override FileStreamResult LineItemAttachmentDownload(string invoiceId, string lineItemId, string lineItemDetailId)
        {
            return base.LineItemAttachmentDownload(invoiceId, lineItemId, lineItemDetailId);
        }

        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        // This is common method called from Misc Payable Invoice View and Misc Daily Bilateral Invoice View
        // [ISAuthorize(Business.Security.Permissions.Misc.Payables.Invoice.View)]
        [HttpPost]
        public override JsonResult GetRejectionInvoiceDetails(string billingMemberId, string billedMemberId, string rejectionInvoiceNumber,  string settlementMethod, int? settlementMonth, int? settlementYear, int? settlementPeriod)
        {
            return base.GetRejectionInvoiceDetails(billingMemberId, billedMemberId, rejectionInvoiceNumber, settlementMethod, settlementMonth, settlementYear, settlementPeriod);
        }

        // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
        // This is common method called from Misc Payable Invoice View and Misc Daily Bilateral Invoice View
        // [ISAuthorize(Business.Security.Permissions.Misc.Payables.Invoice.View)]
        [HttpPost]
        [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, TableName = TransactionTypeTable.INVOICE)]
        public override JsonResult GetCorrespondenceInvoiceDetails(long correspondenceRefNumber, int billedMemberId, string invoiceId, bool isUpdateOperation)
        {
            return base.GetCorrespondenceInvoiceDetails(correspondenceRefNumber, billedMemberId, invoiceId, isUpdateOperation);
        }

    }
}
