namespace Iata.IS.Model.Common
{

  public class DerivedVatDetails
  {
    public int RowNumber { get; set; }

    public int VatIdentifierId { get; set; }

    public string Identifier { get; set; }

    public string VatLabel { get; set; }

    public string VatText { get; set; }

    public double VatPercentage { get; set; }

    public double VatCalculatedAmount { get; set; }

    public double VatBaseAmount { get; set; }

  }
}
