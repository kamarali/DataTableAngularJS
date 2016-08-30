using Iata.IS.Model.Pax.Base;
using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax
{
  /// <summary>
  /// To match with data model to create Entity Framework Context object 
  /// This InvoiceVat Model is also used in Cargo 
  /// </summary>
  public class InvoiceVat : EntityBase<Guid>
  {
    //Removed inheritance with Vat.cs and added properties to change VatPercentage and  VatCalculatedAmount as nullable properties.
    //These changes are doen to save 0 Vat records with blanks in fields ‘VAT Label’ and ‘VAT %’ and ‘VAT Calculated Amount’. 
    public VatIdentifier VatIdentifier { get; set; }

    public string VatLabel { get; set; }

    public string VatText { get; set; }

    public double VatBaseAmount { get; set; }

    public double? VatPercentage { get; set; }

    public double? VatCalculatedAmount { get; set; }

    public Guid ParentId { get; set; }

    public int VatIdentifierId { get; set; }

    /// <summary>
    /// Added to display data in grid
    /// </summary>
    public string Identifier
    {
      get
      {
        if (VatIdentifier != null)
          return VatIdentifier.Description;
        else
          return string.Empty;
      }
    }
  }
}
