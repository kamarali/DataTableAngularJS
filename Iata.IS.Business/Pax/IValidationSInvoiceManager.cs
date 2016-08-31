using System.Collections.Generic;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.Pax
{
  public interface IValidationSInvoiceManager
  {

    /// <summary>
    /// Validates the sampling invoice db.
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    bool ValidateParsedSamplingInvoice(Invoice invoice, IList<ValidationExceptionDetail> exceptionDetailsList, string fileName);
  }
}