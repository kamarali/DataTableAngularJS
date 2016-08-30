using System.Collections.Generic;

namespace Iata.IS.Model.Pax.Sampling
{
  /// <summary>
  /// Used while getting linked coupons for Form C, Form D/E.
  /// </summary>
  public class LinkedCouponDetails
  {
    public List<LinkedCoupon> LinkedCoupons { get; set; }
    public string ErrorMessage { get; set; }
  }

  public class LinkedCoupon
  {
    public string ProvisionalInvoiceNumber { get; set; }
    public int BatchNumberOfProvisionalInvoice { get; set; }
    public int RecordSeqNumberOfProvisionalInvoice { get; set; }
    public double GrossAmountAlf { get; set; }
    public bool ElectronicTicketIndicator { get; set; }
    public string AgreementIndicatorSupplied { get; set; }
    public string AgreementIndicatorValidated { get; set; }
    public string OriginalPmi { get; set; }
    public string ValidatedPmi { get; set; }
    public string ListingCurrency { get; set; }
    public string ProrateMethodology { get; set; }
    public string ErrorCode { get; set; }
  }
}
