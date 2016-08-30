using System.Web.Mvc;

namespace Iata.IS.Web.Areas.Pax
{
  public class PaxAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Pax";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute("ViewFormCCoupon", "Pax/BillingHistory/ViewFormCCoupon/{id}", new { area = "Pax", action = "ViewFormCCoupon", controller = "BillingHistory", id = UrlParameter.Optional });
      context.MapRoute("PaxCorrespondenceTrail", "Pax/{billingType}/CorrespondenceTrail/{action}", new { action = "Index", controller = "CorrespondenceTrail", billingType = "Receivables", invoiceId = UrlParameter.Optional, area = "Pax" });
      context.MapRoute("PaxBillingHistory", "Pax/BillingHistory/{action}/{invoiceId}", new { action = "Index", controller = "BillingHistory", invoiceId = UrlParameter.Optional, area = "Pax" });
      context.MapRoute("DownLoadPdf", "Pax/BillingHistory/DownLoadPdf", new { area = "Pax", action = "DownLoadPdf", controller = "BillingHistory" });
      context.MapRoute("DownLoadFile", "Pax/{billingType}/Invoice/DownloadFile/{id}", new { area = "Pax", billingType = "Receivables", action = "DownloadFile", controller = "Invoice" });
      context.MapRoute("GenerateBillingHistoryAuditTrailPdfForPax", "Pax/BillingHistory/GenerateBillingHistoryAuditTrailPdfForPax/{transactionId}/{transactionType}", new { area = "Pax", action = "GenerateBillingHistoryAuditTrailPdf", controller = "BillingHistory", transactionId = UrlParameter.Optional, transactionType = UrlParameter.Optional });
      context.MapRoute("PaxBillingHistoryAuditTrail", "Pax/BillingHistory/PaxBillingHistoryAuditTrail/{transactionId}/{transactionType}", new { area = "Pax", action = "PaxBillingHistoryAuditTrail", controller = "BillingHistory", transactionId = UrlParameter.Optional, transactionType = UrlParameter.Optional });
      context.MapRoute("PaxCorrespondenceAttachmentUpload", "Pax/Correspondence/{invoiceId}/CorrespondenceAttachmentUpload/{transactionId}", new { action = "CorrespondenceAttachmentUpload", controller = "Correspondence", area = "Pax" });
      context.MapRoute("PaxInitiateCorrespondence", "Pax/Correspondence/InitiateCorrespondenceRejectionGridData/{invoiceId}/{rejectedMemoIds}", new { action = "InitiateCorrespondenceRejectionGridData", controller = "Correspondence", area = "Pax" });
      context.MapRoute("PaxViewLinkedCorrespondence", "Pax/Correspondence/ViewLinkedCorrespondence", new { action = "ViewLinkedCorrespondence", controller = "Correspondence", area = "Pax" });
      context.MapRoute("PaxCorrespondence", "Pax/Correspondence/{action}/{invoiceId}", new { action = "Index", controller = "Correspondence", area = "Pax", invoiceId = UrlParameter.Optional });
      context.MapRoute("PaxShowDetails", "Pax/{controller}/ShowDetails/{invoiceId}", new { action = "ShowDetails", controller = "Correspondence", area = "Pax", invoiceId = UrlParameter.Optional });

      // Unlinked supporting documents
      context.MapRoute("UnlinkedSupportingDocuments", "Pax/UnlinkedSupportingDocument/{action}/{id}", new { area = "Pax", controller = "UnlinkedSupportingDocument", id = UrlParameter.Optional });
      context.MapRoute("ManageSupportingDocuments", "Pax/{billingType}/SupportingDoc/{action}", new { area = "Pax", controller = "SupportingDoc", billingType = "Receivables" });

      context.MapRoute("FormC", "Pax/{billingType}/{controller}/{action}/{provisionalBillingYear}/{provisionalBillingMonth}/{provisionalBillingMemberId}/{fromMemberId}/{invoiceStatusId}/{listingCurrencyId}", new { area = "Pax", billingType = "Receivables", controller = "FormC", listingCurrencyId = UrlParameter.Optional });
      context.MapRoute("FormCEdit", "Pax/{billingType}/FormC/Edit/{invoiceId}", new { area = "Pax", action = "Edit", billingType = "Receivables", controller = "FormC" });
      context.MapRoute("FormCManage", "Pax/Receivables/FormC/Index/{id}", new { area = "Pax", action = "Index", billingType = "Receivables", controller = "FormC", id = UrlParameter.Optional });
      context.MapRoute("FormCSearch", "Pax/{billingType}/FormC/GetFormCSearchResults/{id}", new { area = "Pax", action = "GetFormCSearchResults", billingType = "Receivables", controller = "FormC", id = UrlParameter.Optional });
      context.MapRoute("GetFormABListingCurrency", "Pax/{billingType}/FormC/GetFormABListingCurrency", new { area = "Pax", action = "GetFormABListingCurrency", billingType = "Receivables", controller = "FormC" });

      context.MapRoute("FormCPayablesManage", "Pax/Payables/FormCPayables/PayablesSearch", new { area = "Pax", action = "PayablesSearch", billingType = "Payables", controller = "FormCPayables" });
      context.MapRoute("FormCPayablesSearch", "Pax/{billingType}/FormCPayables/GetFormCPayablesSearchResults", new { area = "Pax", action = "GetFormCPayablesSearchResults", billingType = "Payables", controller = "FormCPayables" });

      context.MapRoute("FormCCouponAdd", "Pax/{billingType}/FormC/{invoiceId}/CouponCreate", new { area = "Pax", action = "CouponCreate", billingType = "Receivables", controller = "FormC" });
      context.MapRoute("FormCCouponView", "Pax/{billingType}/{controller}/CouponView/{provisionalBillingYear}/{provisionalBillingMonth}/{provisionalBillingMemberId}/{fromMemberId}/{invoiceStatusId}/{listingCurrencyId}/{transactionId}", new { area = "Pax", action = "CouponView", billingType = "Receivables", controller = "FormC" });
      context.MapRoute("FormCCouponSubmit", "Pax/{billingType}/FormC/SubmitFormC/{transactionId}", new { area = "Pax", action = "SubmitFormC", billingType = "Receivables", controller = "FormC", transactionId = UrlParameter.Optional });
      context.MapRoute("FormCCouponPresent", "Pax/{billingType}/FormC/PresentFormC/{transactionId}", new { area = "Pax", action = "PresentFormC", billingType = "Receivables", controller = "FormC" });

      context.MapRoute("FormCPayablesDownload", "Pax/Payables/FormCPayables/DownloadZip/{id}/{options}", new { area = "Pax", action = "DownloadZip", id = "", options = "", controller = "FormCPayables" });
      context.MapRoute("FormCDownload", "Pax/{billingType}/FormC/DownloadZip/{id}/{options}/{zipFileName}", new { area = "Pax", controller = "FormC", action = "DownloadZip", id = "", options = "", zipFileName = "" });
      context.MapRoute("FormCCoupon", "Pax/{billingType}/FormC/{invoiceId}/{action}/{transactionId}", new { area = "Pax", controller = "FormC", transactionId = UrlParameter.Optional });
      context.MapRoute("PaxValidationErrorCorrection", "Pax/Receivables/PaxValidationErrorCorrection/{action}", new { area = "Pax", billingType = "Receivables", action = "Index", controller = "PaxValidationErrorCorrection" });
      context.MapRoute("breakdown", "Pax/{billingType}/{controller}/{invoiceId}/{transaction}/{transactionId}/{action}/{couponId}", new { area = "Pax", billingType = "Receivables", couponId = UrlParameter.Optional });
      context.MapRoute("InvoiceEdit", "Pax/{billingType}/{controller}/Edit/{invoiceId}", new { area = "Pax", billingType = "Receivables", action = "Edit" });
      context.MapRoute("CreditNoteEdit", "Pax/{billingType}/CreditNote/Edit/{invoiceId}", new { area = "Pax", billingType = "Receivables", action = "Edit" });
      context.MapRoute("InvoiceView", "Pax/{billingType}/{controller}/View/{invoiceId}", new { area = "Pax", billingType = "Receivables", action = "View" });
      context.MapRoute("InvoiceCreate", "Pax/{billingType}/{controller}/Create", new { area = "Pax", billingType = "Receivables", action = "Create" });
      context.MapRoute("AutoBillingInvoiceSearch", "Pax/{billingType}/AutoBilling/{action}/{id}", new { area = "Pax", billingType = "Receivables", controller = "AutoBilling", id = UrlParameter.Optional });
      context.MapRoute("InvoiceSearch", "Pax/{billingType}/ManageInvoice/{action}/{id}", new { area = "Pax", billingType = "Receivables", controller = "ManageInvoice", id = UrlParameter.Optional });

      context.MapRoute("transactions", "Pax/{billingType}/{controller}/{invoiceId}/{action}/{transactionId}", new { area = "Pax", billingType = "Receivables", controller = "FormXF", action = "Index", transactionId = UrlParameter.Optional });
      
      context.MapRoute("Pax_default", "Pax/{billingType}/{controller}/{action}/{id}", new { area = "Pax", billingType = "Receivables", action = "Index", id = UrlParameter.Optional });
    }
  }
}
