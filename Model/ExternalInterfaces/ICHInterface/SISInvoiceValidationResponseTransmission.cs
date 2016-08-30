using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.ExternalInterfaces.ICHInterface
{
  public class SISInvoiceValidationResponseTransmission
  {
    //CMP #624: ICH Rewrite-New SMI X

    public string ValidationResult { get; set; }

    public string CurrencyRateIndicator { get; set; }

    public List<Error> Errors { get; set; }

    public SISInvoiceValidationResponseTransmission()
    {
      Errors = new List<Error>();
    }
  }
}
