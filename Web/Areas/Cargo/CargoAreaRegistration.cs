using System.Web.Mvc;

namespace Iata.IS.Web.Areas.Cargo
{
  public class CargoAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Cargo";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
        context.MapRoute("CgoUnlinkedSupportingDocuments", "Cargo/UnlinkedSupportingDocument/{action}/{id}", new { area = "Cargo", controller = "UnlinkedSupportingDocument", id = UrlParameter.Optional });
      // context.MapRoute("CGOCreditNoteEdit", "Cargo/{billingType}/CreditNote/Edit/{invoiceId}", new { area = "Cargo", billingType = "Receivables", action = "Edit" });
       // context.MapRoute("ValidationErrorCorrection", "Cargo/{controller}/Index", new { area = "Cargo", action = "Index" });
      context.MapRoute("CargoCorrespondenceTrail", "Cargo/{billingType}/CorrespondenceTrail/{action}", new { action = "Index", controller = "CorrespondenceTrail", billingType = "Receivables", invoiceId = UrlParameter.Optional, area = "Cargo" });
      context.MapRoute("ValidationError", "Cargo/{billingType}/ValidationErrorCorrection/{action}", new { area = "Cargo", action = "Index", controller = "ValidationErrorCorrection", billingType = "Receivables" });
      //SCP498139: strange type of billing
      context.MapRoute("CargoBillingHistory", "Cargo/{billingType}/BillingHistory/{action}/{invoiceId}", new { action = "Index", controller = "BillingHistory", billingType = "Receivables", invoiceId = UrlParameter.Optional, area = "Cargo" });
      //SCP312528 - IS-Web Performance (Controller: BillingHistory - Log Action: CargoBillingHistoryAuditTrail)
      context.MapRoute("CargoBillingHistoryAuditTrail", "Cargo/BillingHistory/CargoBillingHistoryAuditTrail/{transactionId}/{transactionType}", new { area = "Cargo", action = "CargoBillingHistoryAuditTrail", controller = "BillingHistory", transactionId = UrlParameter.Optional, transactionType = UrlParameter.Optional });
      context.MapRoute("CargoDownLoadPdf", "Cargo/BillingHistory/DownLoadPdf", new { area = "Cargo", action = "DownLoadPdf", controller = "BillingHistory" });
      context.MapRoute("CargoDownLoadFile", "Cargo/{billingType}/Invoice/DownloadFile/{id}", new { area = "Cargo", billingType = "Receivables", action = "DownloadFile", controller = "Invoice" });
      context.MapRoute("GenerateBillingHistoryAuditTrailPdfForCgo", "Cargo/BillingHistory/GenerateBillingHistoryAuditTrailPdfForCgo/{transactionId}/{transactionType}", new { area = "Cargo", action = "GenerateBillingHistoryAuditTrailPdf", controller = "BillingHistory", transactionId = UrlParameter.Optional, transactionType = UrlParameter.Optional });
      context.MapRoute("CargoCorrespondenceAttachmentUpload", "Cargo/Correspondence/{invoiceId}/CorrespondenceAttachmentUpload/{transactionId}", new { action = "CorrespondenceAttachmentUpload", controller = "Correspondence", area = "Cargo" });
      context.MapRoute("CargoInitiateCorrespondence", "Cargo/Correspondence/InitiateCorrespondenceRejectionGridData/{invoiceId}/{rejectedMemoIds}", new { action = "InitiateCorrespondenceRejectionGridData", controller = "Correspondence", area = "Cargo" });
      context.MapRoute("CargoViewLinkedCorrespondence", "Cargo/Correspondence/ViewLinkedCorrespondence", new { action = "ViewLinkedCorrespondence", controller = "Correspondence", area = "Cargo" });
      context.MapRoute("CargoCorrespondence", "Cargo/Correspondence/{action}/{invoiceId}", new { action = "Index", controller = "Correspondence", area = "Cargo", invoiceId = UrlParameter.Optional });
      //context.MapRoute("PaxShowDetails", "Pax/{controller}/ShowDetails/{invoiceId}", new { action = "ShowDetails", controller = "Correspondence", area = "Pax", invoiceId = UrlParameter.Optional });

      //// Unlinked supporting documents
      //context.MapRoute("UnlinkedSupportingDocuments", "Pax/UnlinkedSupportingDocument/{action}/{id}", new { area = "Pax", controller = "UnlinkedSupportingDocument", id = UrlParameter.Optional });
      //context.MapRoute("ManageSupportingDocuments", "Pax/{billingType}/SupportingDoc/{action}", new { area = "Pax", controller = "SupportingDoc", billingType = "Receivables" });

      context.MapRoute("CGObreakdown", "Cargo/{billingType}/{controller}/{invoiceId}/{transaction}/{transactionId}/{action}/{couponId}", new {controller = "Invoice", billingType = "Receivables", area = "Cargo", couponId = UrlParameter.Optional });
      context.MapRoute("CGOAwbProrateLadder", "Cargo/{billingType}/{controller}/{transaction}/{transactionId}/{action}/{couponId}/{prorateLadderId}", new { area = "Cargo", billingType = "Receivables", controller = "Invoice", prorateLadderId = UrlParameter.Optional });
      context.MapRoute("CGOInvoiceEdit", "Cargo/{billingType}/{controller}/Edit/{invoiceId}", new { area = "Cargo", billingType = "Receivables", action = "Edit" });
      context.MapRoute("CGOCreditNoteEdit", "Cargo/{billingType}/CreditNote/Edit/{invoiceId}", new { area = "Cargo", billingType = "Receivables", action = "Edit" });
        //context.MapRoute("InvoiceView", "Cargo/{billingType}/{controller}/View/{invoiceId}", new { area = "Cargo", billingType = "Receivables", action = "View" });
        context.MapRoute("CGOInvoiceCreate", "Cargo/{billingType}/Invoice/Create", new { area = "Cargo", billingType = "Receivables", controller = "Invoice", action="Create"});
        context.MapRoute("CGOCreditNoteCreate", "Cargo/{billingType}/CreditNote/Create", new { area = "Cargo", billingType = "Receivables", controller = "CreditNote", action = "Create" });
        context.MapRoute("InvoiceSearchCGO", "Cargo/{billingType}/CargoManageInvoice/{action}/{id}", new { area = "Cargo", billingType = "Receivables", controller = "CargoManageInvoice", id = UrlParameter.Optional });
        context.MapRoute("CGOtransactions", "Cargo/{billingType}/{controller}/{invoiceId}/{action}/{transactionId}", new { area = "Cargo", billingType = "Receivables", action = "Index", transactionId = UrlParameter.Optional });

        context.MapRoute("CGOInvoiceSearch", "Cargo/{billingType}/PayablesInvoiceSearch/{action}/{id}", new { area = "Cargo", billingType = "Payables", controller = "PayablesInvoiceSearch", id = UrlParameter.Optional });
        context.MapRoute("CGOSupportingDocs", "Cargo/CargoSupportingDoc/{action}/{id}", new { area = "Cargo", billingType = "Receivables", controller = "CgoSupportingDoc", id = UrlParameter.Optional });
        context.MapRoute("Cargo_default", "Cargo/{billingType}/{controller}/{action}/{id}", new { area = "Cargo", billingType = "Receivables", action = "Index", id = UrlParameter.Optional });

        
    }
  }
}
