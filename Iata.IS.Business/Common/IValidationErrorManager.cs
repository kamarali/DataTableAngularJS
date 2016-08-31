using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.Common
{
  public interface IValidationErrorManager
  {
    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    IQueryable<WebValidationError> GetValidationErrors(string invoiceId);

    WebValidationError GetWebValidationError(Guid invoiceId, string errorCode, string errorMessage = null);

    /// <summary>
    /// Gets the web validation error.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="args">input parameters for message format.</param>
    /// <returns></returns>
    WebValidationError GetWebValidationError(string errorCode, Guid invoiceId, params string[] args);

    void UpdateValidationErrors(Guid invoiceId, List<WebValidationError> webValidationErrors, IEnumerable<WebValidationError> validationErrorsInDb);   
    
    /// <summary>
    /// To get Validation summary.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="ValidationErrorReport"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <returns></returns>
    IsValidationExceptionSummary GetIsSummary(object invoice, IList<IsValidationExceptionDetail> ValidationErrorReport, string fileName, DateTime fileSubmissionDate);

    /// <summary>
    /// To get the ValidationdetailBase report.
    /// </summary>
    /// <param name="paxExceptionDetails"></param>
    /// <returns></returns>
    List<ValidationExceptionDetailBase> GetBaseReport(IList<IsValidationExceptionDetail> paxExceptionDetails, int errorSerialNumber = 0);

    /// <summary>
    /// To get the list of ValidationException summary for Error Correction. 
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="isValidationExceptionDetails"></param>
    /// <returns></returns>
    List<ValidationExceptionSummary> GetIsSummaryForValidationErrorCorrection(object invoice,
                                                                                     List<IsValidationExceptionDetail>
                                                                                       isValidationExceptionDetails);
  }
}
