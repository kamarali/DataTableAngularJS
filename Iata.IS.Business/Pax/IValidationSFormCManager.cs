using System;
using System.Collections.Generic;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Pax
{
  public interface IValidationSFormCManager
  {
    /// <summary>
    /// Validates the sampling invoice db.
    /// </summary>
    /// <param name="samplingFormC"></param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    bool ValidateParsedSamplingFormC(SamplingFormC samplingFormC, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName,DateTime fileSubmissionDate);
  }
}