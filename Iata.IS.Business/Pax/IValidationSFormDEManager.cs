using System;
using System.Collections.Generic;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Pax
{
  public interface IValidationSFormDEManager
  {
    /// <summary>
    /// Validates the sampling invoice db.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    bool ValidateParsedSamplingFormD(PaxInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate, IDictionary<string, bool> issuingAirline);

    /// <summary>
    /// Validates the sampling invoice db.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    bool ValidateParsedSamplingFormE(PaxInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName,DateTime fileSubmissionDate);
  }
}