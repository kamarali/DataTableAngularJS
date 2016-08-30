using System;

namespace Iata.IS.Model.MemberProfile.Enums
{
  [Flags]
  public enum BilledOtherOutputFormats
  {
    InvoicePdf = 1,
    DetailListingXls = 2,
    DigitalSignatureFiles = 4,
    LegalInvoiceXml = 8,
    MemoDetailsHtml = 16,
    SupportingDocuments = 32
  }
}
