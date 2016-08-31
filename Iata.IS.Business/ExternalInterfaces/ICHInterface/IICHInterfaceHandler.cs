using System;
using System.Collections.Generic;
using System.IO;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.ExternalInterfaces.ICHInterface;
using Iata.IS.Model.Fdr;
using NVelocity;

namespace Iata.IS.Business.ExternalInterfaces.ICHInterface
{
  public interface IICHInterfaceHandler
  {

    /// <summary>
    /// Gets or sets ICHInterface repository .
    /// </summary>
    List<CrossCheckRequestInvoice> GetICHCrossCheckRequestData(string billingPeriod, DateTime startDateTime, DateTime endDateTime);

    /// <summary>
    ///  Get invoice data for invoice IDs pased in the request
    /// </summary>
    /// <param name="invoiceIdList">List of invoice IDs for which data needs to be fetched</param>
    /// <param name="billingPeriod">BillingPeriod corresponding to invoice ids passed</param>
    /// <returns>XML string corresponding invoice data generated for invoice ids passed</returns>
    string GetICHSettlementDataforResend(List<string> invoiceIdList, string billingPeriod);

    /// <summary>
    /// Gets ICHSettlement Data, create ICHSettlementXML and send it to ICH
    /// </summary>
    void GenerateAndSendICHSettlementXML();

    /// <summary>
    /// This method validates whether there are more than one distinct billing periods present 
    /// in list of invoice ids passed
    /// </summary>
    /// <param name="invoiceData">List of invoices corresponding to invoice ID list passed</param>
    /// <returns>1 when invoice list contains more than one distinct billing periods
    ///          2 when invoice list contains one billing period but the billing period is not current period or not previous period
    ///          3 when invoice list contains only one billing period (either current or previous)
    /// </returns>
    int ValidateBillingPeriodforResendRequest(List<Model.ExternalInterfaces.ICHInterface.ICHSettlementData> invoiceData);

    /// <summary>
    /// Process FiveDayRates file:
    /// 1. Parse and validate the file.
    /// 2. Add the exchange rates obtained from the file in to the database.
    /// Returns list of ISFileParsingException if parsing or validation fails
    /// else returns null.
    /// </summary>
    /// <param name="fiveDayRate">Parsed Five Day Rates File</param>
    /// <param name="fdrFileParsingErrors">Five Day Rates File Parsing Errors</param>
    void ProcessFiveDayRatesFile(FiveDayRate fiveDayRate, List<ISFileParsingException> fdrFileParsingErrors);

    /// <summary>
    /// Send FDR not received notification to IS Admin if FDR file is not received by 4 PM of every month.
    /// </summary>
    void SendFdrNotReceivedNotification();

    /// <summary>
    /// This method checks whether passed billing period is current or previous billing period
    /// </summary>
    /// <param name="billingPeriod">BillingPeriod value to be validated</param>
    /// <returns>true, if billingperiod passed is current or previous billing period, false, otherwise</returns>
    bool ISCurrentorPreviousBillingPeriod(string billingPeriod);

    /// <summary>
    /// This method checks whether passed billing period has valid billing period format
    /// </summary>
    /// <param name="billingPeriod">BillingPeriod value to be validated</param>
    /// <returns>true, if billingperiod format is valid, false, otherwise</returns>
    bool ValidateBillingPeriodFormat(string billingPeriod);

    void SendNotificationToISAdmin(ISSISOpsAlert alert, VelocityContext context, EmailTemplateId templateId, string filePath = null);

    bool ValidateStartEndDateTime(string inputDateTime, out DateTime? outputDateTime);
  }
}
