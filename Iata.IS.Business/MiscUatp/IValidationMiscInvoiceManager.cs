using System;
using System.Collections.Generic;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.MiscUatp
{
  public interface IValidationMiscInvoiceManager
  {
    /// <summary>
    /// Validates the parsed invoice.
    /// </summary>
    /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    bool ValidateParsedInvoice(MiscUatpInvoice miscUatpInvoice, IList<IsValidationExceptionDetail> exceptionDetailsList,
                                        string fileName, DateTime fileSubmissionDate, string issuingOrganizationId);
      
  }
}