using System.Web.Mvc;

namespace Iata.IS.Web.Areas.Uatp
{
  public class UatpAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Uatp";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
        context.MapRoute("UatpValidationErrorCorrection", "Uatp/{billingType}/{controller}/Index", new { area = "Uatp", action = "Index", billingType = "Receivables" });

        context.MapRoute("UatpDownLoadFile", "Uatp/UatpInvoice/DownloadFile/{id}", new { area = "Uatp", action = "DownloadFile", controller = "UatpInvoice" });

        context.MapRoute("UatpCreateBillingMemo", "Uatp/{controller}/CreateBillingMemo/{rejectedInvoiceId}/{correspondenceReferenceNumber}", new { action = "CreateBillingMemo", controller = "UatpInvoice", rejectedInvoiceId = UrlParameter.Optional, correspondenceReferenceNumber = UrlParameter.Optional, area = "Uatp" });

        context.MapRoute("UatpCorrespondenceAttachmentDownload", "Uatp/Correspondence/CorrespondenceAttachmentDownload/{attachmentId}", new { action = "CorrespondenceAttachmentDownload", controller = "Correspondence", transactionId = UrlParameter.Optional, area = "Uatp" });

        context.MapRoute("UatpOpenCorrespondenceForEdit", "Uatp/Correspondence/OpenCorrespondenceForEdit/{invoiceId}/{transactionId}", new { action = "OpenCorrespondenceForEdit", controller = "Correspondence", area = "Uatp" });

        context.MapRoute("UatpViewCorrespondenceDetails", "Uatp/Correspondence/ViewCorrespondenceDetails/{invoiceId}/{transactionId}", new { action = "ViewCorrespondenceDetails", controller = "Correspondence", area = "Uatp" });

        context.MapRoute("UatpViewLinkedCorrespondence", "Uatp/Correspondence/ViewLinkedCorrespondence", new { action = "ViewLinkedCorrespondence", controller = "Correspondence", area = "Uatp" });

        // Unlinked supporting documents
        context.MapRoute("UatpUnlinkedSupportingDocuments", "Uatp/UnlinkedSupportingDocument/{action}/{id}", new { area = "Uatp", controller = "UnlinkedSupportingDocument", id = UrlParameter.Optional });
        context.MapRoute("UatpManageSupportingDocuments", "Uatp/{billingType}/SupportingDoc/{action}", new { area = "Uatp", controller = "SupportingDoc", billingType = "Receivables" });

        context.MapRoute("UatpCorrespondenceTransaction", "Uatp/Correspondence/{invoiceId}/{action}/{transactionId}", new { action = "Index", controller = "Correspondence", area = "Uatp" });

        context.MapRoute("UatpCorrespondence", "Uatp/Correspondence/{action}/{invoiceId}", new { action = "Index", controller = "Correspondence", area = "Uatp", invoiceId = UrlParameter.Optional });

        context.MapRoute("UatpRejectInvoice", "Uatp/{controller}/{rejectedInvoiceId}/CreateRejectionInvoice", new { action = "CreateRejectionInvoice", area = "Uatp" });

        context.MapRoute("UatpShowDetails", "Uatp/{controller}/ShowDetails/{invoiceId}", new { action = "ShowDetails", area = "Uatp", invoiceId = UrlParameter.Optional });

        context.MapRoute("UatpUpload", "Uatp/Correspondence/{invoiceId}/{action}/{transactionId}", new { action = "Index", controller = "Correspondence", invoiceId = UrlParameter.Optional, transactionId = UrlParameter.Optional, area = "Uatp" });

        context.MapRoute("UatpInvoice", "Uatp/{controller}/GetRejectionInvoiceDetails", new { action = "GetRejectionInvoiceDetails", area = "Uatp", controller = "UatpInvoice" });

        context.MapRoute("UatpCorrespondenceInvoice", "Uatp/{controller}/GetCorrespondenceInvoiceDetails", new { action = "GetCorrespondenceInvoiceDetails", area = "Uatp", controller = "UatpInvoice" });

        context.MapRoute("UatpLineItemDetailEdit", "Uatp/{billingType}/{controller}/{invoiceId}/LineItem/{lineItemId}/{action}/{lineItemDetailId}", new { lineItemDetailId = UrlParameter.Optional, area = "Uatp", billingType = "Receivables" });

        context.MapRoute("UatpEdit", "Uatp/{billingType}/{controller}/Edit/{invoiceId}", new { action = "Edit", billingType = "Receivables", area = "Uatp" });

        context.MapRoute("Uatp_Derived_Vat", "Uatp/{billingType}/{controller}/GetDerivedVat/{parentId}", new { area = "Uatp", billingType = "Receivables", action = "GetDerivedVat", parentId = UrlParameter.Optional });

        context.MapRoute("UatpInvoiceCreate", "Uatp/{billingType}/{controller}/Create", new { action = "Create", billingType = "Receivables", area = "Uatp" });

        context.MapRoute("UatpInvoiceEdit", "Uatp/{billingType}/{controller}/Edit/{invoiceId}", new { area = "Uatp", billingType = "Receivables", action = "Edit" });

        context.MapRoute("UatpInvoiceView", "Uatp/{billingType}/{controller}/View/{invoiceId}", new { area = "Uatp", billingType = "Receivables", action = "View" });

        context.MapRoute("UatpInvoiceSearch", "Uatp/ManageUatpInvoice/{action}/{id}", new { controller = "ManageUatpInvoice", id = UrlParameter.Optional, area = "Uatp" });

        context.MapRoute("UatpBillingHistory", "Uatp/BillingHistory/{action}/{invoiceId}", new { action = "Index", controller = "BillingHistory", invoiceId = UrlParameter.Optional, area = "Uatp" });

        context.MapRoute("UatpLineItem", "Uatp/{billingType}/{controller}/{invoiceId}/{action}/{lineItemId}", new { action = "Index", billingType = "Receivables", lineItemId = UrlParameter.Optional, area = "Uatp" });

        //context.MapRoute("UatpTransactions", "Uatp/{billingType}/{controller}/{invoiceId}/{action}/{transactionId}", new { action = "Index", billingType = "Receivables", transactionId = UrlParameter.Optional, area = "Uatp" });

        context.MapRoute("UatpTransactions", "Uatp/{billingType}/{controller}/{invoiceId}/{action}/{transactionId}", new { area = "Uatp", action = "LineItemView", billingType = "Receivables", transactionId = UrlParameter.Optional });

        context.MapRoute("UatpBillingHistoryPdf", "Uatp/{controller}/{action}/{id}/{areaName}", new { area = "Uatp", action = "Index", billingType = "Receivables", id = UrlParameter.Optional, areaName = UrlParameter.Optional });

        context.MapRoute("Uatp_default", "Uatp/{controller}/{action}/{id}", new { area = "Uatp", action = "Index", billingType = "Receivables", id = UrlParameter.Optional });

    }
  }
}
