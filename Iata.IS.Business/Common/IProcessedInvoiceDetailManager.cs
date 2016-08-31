using Iata.IS.Data.Pax;
using Iata.IS.Model.Enums;

namespace Iata.IS.Business.Common
{
  public interface IProcessedInvoiceDetailManager
  {
    IInvoiceRepository ProcessedInvoiceDetailRepository { get; set; }

    /// <summary>
    /// Creates and compresses the processed invoice data CSV.
    /// </summary>
    /// <param name="triggerGroupName">Name of the trigger group.</param>
    /// <returns>
    /// flag indicates that output zip file created or not.
    /// </returns>
    bool CreateAndTransmitProcessedInvoiceDataCsv(string triggerGroupName);

    /// <summary>
    /// Creates processed invoice for a specific billing member.
    /// </summary>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="reGenerateFlag">The re generate flag.</param>
    /// <returns></returns>
    bool CreateAndTransmitProcessedInvoiceDataCsv(int billingMemberId, int billingYear, int billingMonth, int billingPeriod, int reGenerateFlag);

    void AddISSISOpsAlert(string billingEntityCode, string message, string title, EmailTemplateId emailTemplateId, string billingPeriodText, string invoiceNo);
  }
}