using System;

namespace Iata.IS.Model.MemberProfile.Enums
{
  [Flags]
  public enum BillingOtherOutputFormats
  {
    InvoicePdf = 1,
    DetailListing = 2,
    DigitalSignature = 4,
    OtherLegalFiles = 8,
    MemoDetails = 16
  }
}
