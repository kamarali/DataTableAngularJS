using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Cargo
{
  public class CargoInvoiceVat : EntityBase<Guid>
  {
    //Removed inheritance with Vat.cs and added properties to change VatPercentage and  VatCalculatedAmount as nullable properties.
    //These changes are doen to save 0 Vat records with blanks in fields ‘VAT Label’ and ‘VAT %’ and ‘VAT Calculated Amount’. 
    public CgoVatIdentifier VatIdentifier { get; set; }

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
