using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.AutoBilling;
using System;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Pax
{
  public interface IValidationNSInvoiceManager
  {
    /// <summary>
    /// Validates the non sampling invoice db.
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="fileSubmisionDate"></param>
    /// <param name="issuingAirline"></param>
    /// <returns></returns>
    bool ValidateParsedInvoice(PaxInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList,string fileName,DateTime fileSubmisionDate, IDictionary<string, bool> issuingAirline);

    /// <summary>
    /// Validate Invoice details and batch record sequence number. This change has implemented based on SCP#85837
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmisionDate"></param>
    /// <param name="issuingAirline"></param>
    /// <param name="fileRecordSequenceNumber"></param>
    /// <returns></returns>
    bool ValidateParsedInvoice(PaxInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList,
                             string fileName, DateTime fileSubmisionDate, IDictionary<string, bool> issuingAirline,
                             Dictionary<int, Dictionary<Guid, int>> fileRecordSequenceNumber);

    /// <summary>
    /// To validate Record50.
    /// </summary>
    /// <param name="record50LiftRequest"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmisionDate"></param>
    /// <returns></returns>
    bool ValidateParsedRecord50(Record50LiftRequest record50LiftRequest, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmisionDate, int billedMemberId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isrAutoBillingModel"></param>
    /// <param name="couponRecord"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    PaxInvoice GetAutoBillingOpenInvoice(IsrAutoBillingModel isrAutoBillingModel,PrimeCoupon couponRecord, DateTime fileSubmissionDate, int billingMemberId, int billedMemberId);

    /// <summary>
    /// To Validate ISR File.
    /// </summary>
    /// <param name="isrAutoBillingModel"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmisionDate"></param>
    /// <param name="autoBillingAirlineId"></param>
    /// <returns></returns>
    bool ValidateParsedIsrModel(IsrAutoBillingModel isrAutoBillingModel, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmisionDate,int autoBillingAirlineId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isrAutoBillingModel"></param>
    /// <param name="invoice"></param>
    /// <param name="isrCoupon"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmisionDate"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    PaxInvoice CreateAutoBillingInvoice(IsrAutoBillingModel isrAutoBillingModel, PaxInvoice invoice, PrimeCoupon isrCoupon, string fileName, DateTime fileSubmisionDate, int billingMemberId, int billedMemberId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="isrCoupon"></param>
    /// <returns></returns>
    PaxInvoice UpdateSourceCodeTotalRecord(PaxInvoice invoice, PrimeCoupon isrCoupon);
  }
}
