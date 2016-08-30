using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.MiscUatp
{
  /// <summary>
  /// Stores Correspondence invoice details like rejection invoice number,
  /// is authority to bill.
  /// Used for passing information from Business to Controller.
  /// and in turn from controller to UI. 
  /// </summary>
  /// <remarks>
  /// The property ErrorMessage is used to pass error message across layer.
  /// If this property is set, then correspondence invoice details were failed to get.
  /// </remarks>
  public class CorrespondenceInvoiceDetails
  {
    public bool IsAuthorityToBill { get; set; }
    public string RejectedInvoiceNumber { get; set; }
    public string ErrorMessage { get; set; }
    public string CurrentBilledIn { get; set; }
    public int CurrentBillingCurrencyCode { get; set; }
    public bool DisableBillingCurrency { get; set; }
    public bool EnableExchangeRate { get; set; }
  }
}