using System.Web.Mvc;

namespace Iata.IS.Web.Areas.Misc
{
  public class MiscAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Misc";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute("MiscValidationErrorCorrection", "Misc/{billingType}/{controller}/Index", new { area = "Misc", billingType = "Receivables", action = "Index" });
      context.MapRoute("MiscCorrespondenceTrail", "Misc/{billingType}/CorrespondenceTrail/{action}", new { action = "Index", controller = "CorrespondenceTrail", billingType = "Receivables", invoiceId = UrlParameter.Optional, area = "Misc" });
      context.MapRoute("MiscDownLoadFile", "Misc/MiscInvoice/DownloadFile/{id}", new { area = "Misc", action = "DownloadFile", controller = "MiscInvoice" });

      context.MapRoute("CreateBillingMemo", "Misc/{controller}/CreateBillingMemo/{rejectedInvoiceId}/{correspondenceReferenceNumber}", new { action = "CreateBillingMemo", controller = "MiscInvoice", rejectedInvoiceId = UrlParameter.Optional, correspondenceReferenceNumber = UrlParameter.Optional, area = "Misc" });

      context.MapRoute("CorrespondenceAttachmentDownload", "Misc/Correspondence/CorrespondenceAttachmentDownload/{attachmentId}", new { action = "CorrespondenceAttachmentDownload", controller = "Correspondence", transactionId = UrlParameter.Optional, area = "Misc" });

      context.MapRoute("OpenCorrespondenceForEdit", "Misc/Correspondence/OpenCorrespondenceForEdit/{invoiceId}/{transactionId}", new { action = "OpenCorrespondenceForEdit", controller = "Correspondence", area = "Misc" });

      context.MapRoute("ViewCorrespondenceDetails", "Misc/Correspondence/ViewCorrespondenceDetails/{invoiceId}/{transactionId}", new { action = "ViewCorrespondenceDetails", controller = "Correspondence", area = "Misc" });
      
      context.MapRoute("ViewLinkedCorrespondence", "Misc/Correspondence/ViewLinkedCorrespondence", new { action = "ViewLinkedCorrespondence", controller = "Correspondence", area = "Misc" });

      // Unlinked supporting documents
      context.MapRoute("MiscUnlinkedSupportingDocuments", "Misc/UnlinkedSupportingDocument/{action}/{id}", new { area = "Misc", controller = "UnlinkedSupportingDocument", id = UrlParameter.Optional });
      context.MapRoute("MiscManageSupportingDocuments", "Misc/{billingType}/MiscSupportingDoc/{action}", new { area = "Misc", controller = "MiscSupportingDoc", billingType = "Receivables" });

      context.MapRoute("CorrespondenceTransaction", "Misc/Correspondence/{invoiceId}/{action}/{transactionId}", new { action = "Index", controller = "Correspondence", area = "Misc" });

      context.MapRoute("Correspondence", "Misc/Correspondence/{action}/{invoiceId}", new { action = "Index", controller = "Correspondence", area = "Misc", invoiceId = UrlParameter.Optional });

      context.MapRoute("RejectInvoice", "Misc/{controller}/{rejectedInvoiceId}/CreateRejectionInvoice", new { action = "CreateRejectionInvoice", area = "Misc" });

      context.MapRoute("ShowDetails", "Misc/{controller}/ShowDetails/{invoiceId}", new { action = "ShowDetails", area = "Misc", invoiceId = UrlParameter.Optional });

      context.MapRoute("Upload", "Misc/Correspondence/{invoiceId}/{action}/{transactionId}", new { action = "Index", controller = "Correspondence", invoiceId = UrlParameter.Optional, transactionId = UrlParameter.Optional, area = "Misc" });

      context.MapRoute("MiscInvoice", "Misc/{controller}/GetRejectionInvoiceDetails", new { action = "GetRejectionInvoiceDetails", area = "Misc", controller = "MiscInvoice" });

      context.MapRoute("CorrespondenceInvoice", "Misc/{controller}/GetCorrespondenceInvoiceDetails", new { action = "GetCorrespondenceInvoiceDetails", area = "Misc", controller = "MiscInvoice" });

      context.MapRoute("LineItemDetailEdit", "Misc/{billingType}/{controller}/{invoiceId}/LineItem/{lineItemId}/{action}/{lineItemDetailId}", new { lineItemDetailId = UrlParameter.Optional, area = "Misc", billingType = "Receivables" });

      context.MapRoute("Edit", "Misc/{billingType}/{controller}/Edit/{invoiceId}", new { action = "Edit", billingType = "Receivables", area = "Misc" });

      context.MapRoute("Derived_Vat", "Misc/{billingType}/{controller}/GetDerivedVat/{parentId}", new { area = "Misc", billingType = "Receivables", action = "GetDerivedVat", parentId = UrlParameter.Optional});

      context.MapRoute("MiscInvoiceCreate", "Misc/{billingType}/{controller}/Create", new { action = "Create", billingType = "Receivables", area = "Misc" });

      context.MapRoute("MiscInvoiceEdit", "Misc/{billingType}/{controller}/Edit/{invoiceId}", new { area = "Misc", billingType = "Receivables", action = "Edit" });

      context.MapRoute("MiscInvoiceView", "Misc/{billingType}/{controller}/View/{invoiceId}", new { area = "Misc", billingType = "Receivables", action = "View" });

      context.MapRoute("MiscInvoiceSearch", "Misc/ManageMiscInvoice/{action}/{id}", new { controller = "ManageMiscInvoice", id = UrlParameter.Optional, area = "Misc" });

      context.MapRoute("BillingHistory", "Misc/BillingHistory/{action}/{invoiceId}", new { action = "Index", controller = "BillingHistory", invoiceId = UrlParameter.Optional, area = "Misc" });

      context.MapRoute("LineItem", "Misc/{billingType}/{controller}/{invoiceId}/{action}/{lineItemId}", new { action = "Index", billingType = "Receivables", lineItemId = UrlParameter.Optional, area = "Misc" });

      context.MapRoute("MiscTransactions", "Misc/{billingType}/{controller}/{invoiceId}/{action}/{transactionId}", new { area = "Misc", action = "LineItemView", billingType = "Receivables", transactionId = UrlParameter.Optional });

      context.MapRoute("BillingHistoryPdf", "Misc/{controller}/{action}/{id}/{areaName}", new { area = "Misc", action = "Index", billingType = "Receivables", id = UrlParameter.Optional, areaName = UrlParameter.Optional });

      context.MapRoute("Misc_default", "Misc/{controller}/{action}/{id}", new { area = "Misc", action = "Index", billingType = "Receivables", id = UrlParameter.Optional });
      
      

    }
  }
}
