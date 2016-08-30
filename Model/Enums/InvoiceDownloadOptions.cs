using System;

namespace Iata.IS.Model.Enums
{
  [Flags]
  public enum InvoiceDownloadOptions
  {
    EInvoicingFiles = 1,
    ListingReport = 2,
    Memos = 4,
    SupportingDocuments = 8,
    PaxReceivables = EInvoicingFiles | ListingReport | Memos | SupportingDocuments,
    PaxPayables = EInvoicingFiles | ListingReport | Memos | SupportingDocuments,
    MiscReceivables = EInvoicingFiles | ListingReport | SupportingDocuments,
    UatpReceivables = EInvoicingFiles | ListingReport | SupportingDocuments,
    MiscPayables = EInvoicingFiles | ListingReport | SupportingDocuments,
    UatpPayables = EInvoicingFiles | ListingReport | SupportingDocuments,
    FormCReceivables = EInvoicingFiles | ListingReport | Memos | SupportingDocuments,
    FormCPayables = EInvoicingFiles | ListingReport | Memos | SupportingDocuments,
    CargoReceivables = EInvoicingFiles | ListingReport | Memos | SupportingDocuments,
    CargoPayables = EInvoicingFiles | ListingReport | Memos | SupportingDocuments
  }
}
