﻿using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.UIModel.ErrorDetail;
using Enums = Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp.BillingHistory;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.UIModel.BillingHistory.Misc;
using Iata.IS.Web.UIModel.Grid.MU;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Web.Areas.MU.Controllers;
using Iata.IS.Business.Common;

namespace Iata.IS.Web.Areas.Misc.Controllers
{
    public class BillingHistoryController : BillingHistoryControllerBase
    {
        //CMP508: Audit Trail Download with Supporting Documents
        public BillingHistoryController(IMiscInvoiceManager miscInvoiceManager, IUatpInvoiceManager uatpInvoiceManager, IMiscUatpInvoiceManager miscUatpInvoiceManager, IQueryAndDownloadDetailsManager queryAndDownloadDetailsManager, IReferenceManager referenceManager)
            : base(miscInvoiceManager, uatpInvoiceManager, miscUatpInvoiceManager, queryAndDownloadDetailsManager, referenceManager)
        {}
                
        [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewAuditTrail)]
        public override ActionResult Index(InvoiceSearchCriteria searchCriteria, CorrespondenceSearchCriteria correspondenceSearchCriteria, string searchType, int? billingCategoryId)
        {
            SessionUtil.SearchType = "MiscBillingHistory";
            return base.Index(searchCriteria, correspondenceSearchCriteria, searchType, (int)BillingCategoryType.Misc);
        }

        public override JsonResult BillingHistorySearchGridData(string criteria,int billingCategoryId)
        {
            return base.BillingHistorySearchGridData(criteria,(int)BillingCategoryType.Misc);
        }

        [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewAuditTrail)]
        [HttpGet]
        public ViewResult MiscBHAuditTrail(string invoiceId)
        {
            return base.BHAuditTrail(invoiceId);
        }

        public ActionResult GenerateBillingHistoryAuditTrailPdfForMisc(string invoiceId, string areaName)
        {
            return base.GenerateBillingHistoryAuditTrailPdf(invoiceId, areaName);
        }

        [HttpPost]
        public override void GenerateAuditTrailPdfFromHtmlString(string auditTrailHtmlString, string fileLocation)
        {
            base.GenerateAuditTrailPdfFromHtmlString(auditTrailHtmlString, fileLocation);
        }

        [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewAuditTrail)]
        [HttpGet]
        public override FileStreamResult BillingHistoryAttachmentDownload(string invoiceId)
        {
            return base.BillingHistoryAttachmentDownload(invoiceId);
        }

        [HttpPost]
        public JsonResult IsCorrespondenceInvoiceOutSideTimeLimit(string invoiceId, int correspondenceStatusId, bool authorityToBill, DateTime correspondenceDate, string correspondenceRefNumber)
        {
            return base.IsCorrespondenceInvoiceOutSideTimeLimit(invoiceId, correspondenceStatusId, authorityToBill,
                                                                correspondenceDate, correspondenceRefNumber, "MiscInvoice");
        }

        protected override void HandleUnknownAction(string actionName)
        {
            base.HandleUnknownAction(actionName);
        }
    }
}