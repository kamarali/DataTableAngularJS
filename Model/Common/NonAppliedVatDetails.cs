namespace Iata.IS.Model.Common
{
  /// <summary>
  /// NonApplied Vat Details for an Invoice.
  /// </summary>
  public class NonAppliedVatDetails
  {
    public int RowNumber { get; set; }

    public double NonAppliedAmount { get; set; }

    public string VatIdentifierText { get; set; }
  }
}
