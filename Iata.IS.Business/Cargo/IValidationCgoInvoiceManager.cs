using System;
using System.Collections.Generic;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Cargo
{
  public interface IValidationCgoInvoiceManager
  {
    
    /// <summary>
    /// To validate the cargo invoice.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmisionDate"></param>
    /// <param name="issuingAirline"></param>
    /// <returns></returns>
    bool ValidateParsedInvoice(CargoInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList,string fileName, DateTime fileSubmisionDate, IDictionary<string, bool> issuingAirline);

    /// <summary>
    /// Validate Invoice details and batch record sequence number. This change has implemented based on SCP#85837
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="issuingAirline"></param>
    /// <param name="fileRecordSequenceNumber"></param>
    /// <returns></returns>
    bool ValidateParsedInvoice(CargoInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList,
                               string fileName, DateTime fileSubmissionDate, IDictionary<string, bool> issuingAirline,
                               Dictionary<int, Dictionary<Guid, int>> fileRecordSequenceNumber);
  }
}
