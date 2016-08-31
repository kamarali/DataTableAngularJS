using System.Collections.Generic;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Pax
{
  public interface IValidationSFormFManager
  {
    /// <summary>
    /// Validates the parsed sampling form F.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    bool ValidateParsedSamplingFormF(PaxInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName);
  }
}