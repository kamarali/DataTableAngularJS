using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Web.Util.Filters;
using System.Web.Mvc;
using Iata.IS.Web.Util;

namespace Iata.IS.Web.Areas.Misc.Controllers
{
    public class MiscDailyPayInvoiceController : MiscPayInvoiceController
    {
        public MiscDailyPayInvoiceController(IMiscUatpInvoiceManager miscUatpInvoiceManager, IReferenceManager referenceManager, IMemberManager memberManager)
            : base(miscUatpInvoiceManager, referenceManager, memberManager)
        {
        }

        [ISAuthorize(Business.Security.Permissions.Misc.Payables.DailyBilateralDelivery.Download)]
        [HttpGet]
        public override FileStreamResult InvoiceAttachmentDownload(string invoiceId, string lineItemId)
        {
            return base.InvoiceAttachmentDownload(invoiceId, lineItemId);
        }

        [ISAuthorize(Business.Security.Permissions.Misc.Payables.DailyBilateralDelivery.Download)]
        [HttpGet]
        public override FileStreamResult LineItemAttachmentDownload(string invoiceId, string lineItemId, string lineItemDetailId)
        {
            return base.LineItemAttachmentDownload(invoiceId, lineItemId, lineItemDetailId);
        }
    }
}
