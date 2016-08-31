using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data;
using log4net.Repository.Hierarchy;

namespace Iata.IS.Business.ValueConfirmation.Impl
{
  public class AutoGenerateValueConfirmationReport : IAutoGenerateValueConfirmationReport
  {
    //public IRepository<MemberDetails> MemberReository { get; set; }
    public void GenerateAutoValueConfirmationReport()
    {
      
    }

    private void GenerateCSVFile()
    {

      //StringBuilder csvString = new StringBuilder();
      //try
      //{

      //  csvString.Append("Billing Period" + seperator + "Settlement Method Indicator" + seperator + "Billing Member");
      //  csvString.Append(seperator + "Billing Member Name" + seperator + "Billed Member" + seperator +
      //                   "Billed Member Name");
      //  csvString.Append(seperator + "Invoice No.");
      //  csvString.Append(seperator + "Invoice Date");

      //  csvString.Append(seperator + "Billing Category");
      //  csvString.Append(seperator + "Invoice Currency");
      //  csvString.Append(seperator + "Invoice Amount");
      //  csvString.Append(seperator + "Suspended/LateSubmitted");

      //  if (searchCriteria.IncludeProcessingDatesTimestamp)
      //    csvString.Append(seperator + "Recieved in IS");

      //  csvString.Append(seperator + "Validation");
      //  if (searchCriteria.IncludeProcessingDatesTimestamp)
      //    csvString.Append(seperator + "Validation Completed On");

      //  csvString.Append(seperator + "Value Confirmation");
      //  if (searchCriteria.IncludeProcessingDatesTimestamp)
      //    csvString.Append(seperator + "Value Confirmation Completed On");

      //  csvString.Append(seperator + "Digital Signature");
      //  if (searchCriteria.IncludeProcessingDatesTimestamp)
      //    csvString.Append(seperator + "Digital Signature Completed On");


      //  csvString.Append(seperator + "Settlement File Sent");
      //  if (searchCriteria.IncludeProcessingDatesTimestamp)
      //    csvString.Append(seperator + "Settlement File Sent On");

      //  csvString.Append(seperator + "Presented");
      //  if (searchCriteria.IncludeProcessingDatesTimestamp)
      //    csvString.Append(seperator + "Presented On");


      //  csvString.Append(Environment.NewLine);

      //  // Now write all the rows.
      //  foreach (var item in invoiceList)
      //  {
      //    string isSuspendedLateSubmitted = null;
      //    string validationStatus;
      //    string valueConfirmationStatus;
      //    string digitalSignatureStatus;
      //    string settlementFileStatus;
      //    string presentedStatus;

      //    if (item.IsSuspendedLateSubmitted != null)
      //    {
      //      isSuspendedLateSubmitted = GetSuspendedLateSubmittedStatus(item.IsSuspendedLateSubmitted);
      //    }

      //    validationStatus = GetInvoiceStatus(Convert.ToInt32(item.ValidationStatus));
      //    valueConfirmationStatus = GetInvoiceStatus(Convert.ToInt32(item.ValueConfirmationStatus));
      //    digitalSignatureStatus = GetInvoiceStatus(Convert.ToInt32(item.DigitalSignatureStatus));
      //    settlementFileStatus = GetInvoiceStatus(Convert.ToInt32(item.SettlementFileStatus));
      //    presentedStatus = GetInvoiceStatus(Convert.ToInt32(item.PresentedStatus));


      //    csvString.Append(("\"" + item.BillingPeriod + "\"" ?? String.Empty) + seperator +
      //                     ("\"" + item.SettleMethodIndicator + "\"" ?? String.Empty) + seperator +
      //                     ("\"" + item.BillingMemberCode + "\"" ?? String.Empty));
      //    csvString.Append(seperator + item.BillingMemberName + seperator +
      //                     ("\"" + item.BilledMemberCode + "\"" ?? String.Empty));
      //    csvString.Append(seperator + item.BilledMemberName + seperator +
      //                     ("\"" + item.InvoiceNo + "\"" ?? String.Empty));
      //    csvString.Append(seperator + ("\"" + item.InvoiceDate + "\"" ?? String.Empty));

      //    csvString.Append(seperator + ("\"" + item.BillingCategory + "\"" ?? String.Empty));
      //    csvString.Append(seperator + ("\"" + item.InvoiceCurrency + "\"" ?? String.Empty));
      //    csvString.Append(seperator + ("\"" + item.InvoiceAmount + "\"" ?? String.Empty));
      //    csvString.Append(seperator + ("\"" + isSuspendedLateSubmitted + "\"" ?? String.Empty));

      //    if (searchCriteria.IncludeProcessingDatesTimestamp)
      //      csvString.Append(seperator + ("\"" + item.ReceivedInIS + "\"" ?? String.Empty));

      //    csvString.Append(seperator + ("\"" + validationStatus + "\"" ?? String.Empty));
      //    if (searchCriteria.IncludeProcessingDatesTimestamp)
      //      csvString.Append(seperator + ("\"" + item.FormatedValidationStatusDate + "\"" ?? String.Empty));


      //    csvString.Append(seperator + ("\"" + valueConfirmationStatus + "\"" ?? String.Empty));
      //    if (searchCriteria.IncludeProcessingDatesTimestamp)
      //      csvString.Append(seperator + ("\"" + item.FormatedValueConfirmationStatusDate + "\"" ?? String.Empty));

      //    csvString.Append(seperator + ("\"" + digitalSignatureStatus + "\"" ?? String.Empty));
      //    if (searchCriteria.IncludeProcessingDatesTimestamp)
      //      csvString.Append(seperator + ("\"" + item.FormatedDigitalSignatureStatusDate + "\"" ?? String.Empty));


      //    csvString.Append(seperator + ("\"" + settlementFileStatus + "\"" ?? String.Empty));
      //    if (searchCriteria.IncludeProcessingDatesTimestamp)
      //      csvString.Append(seperator + ("\"" + item.FormatedSettlementFileStatusDate + "\"" ?? String.Empty));

      //    csvString.Append(seperator + ("\"" + presentedStatus + "\"" ?? String.Empty));
      //    if (searchCriteria.IncludeProcessingDatesTimestamp)
      //      csvString.Append(seperator + ("\"" + item.FormatedPresentedStatusDate + "\"" ?? String.Empty));

      //    csvString.Append(Environment.NewLine);
      //  }
      //  //flag = true;
      //}
      //catch (Exception exception)
      //{
      //  Logger.Error("Error occurred while generating csv file for Processing dashboard invoice download.", exception);
      //  //flag = false;
      //}


    }
  }
}
