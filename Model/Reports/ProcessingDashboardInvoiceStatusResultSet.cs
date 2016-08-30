using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Reports
{
  public class ProcessingDashboardInvoiceStatusResultSet 
    {

      // Property to get and set Billing Year
      public int BillingYear { get; set; }

      // Property to get and set Billing Month
      public int BillingMonth { get; set; }

      // Property to get and set Billing Period No
      public int BillingPeriodNo { get; set; }

      // SCP255637: Sorting Billing Period in the Processing Dashboard does not work properly
      public string BillingPeriodNumber
      {
        get { return string.Format("{0}{1}{2}", BillingYear, BillingMonth.ToString().PadLeft(2, '0'), BillingPeriodNo.ToString().PadLeft(2, '0')); }
      }

      // Property to get and set Billing Period
    public string BillingPeriod
    {
      get
      {
        var dtTemp  = new DateTime(BillingYear,BillingMonth,1);
        var strPeriod = (BillingPeriodNo == 0) ? String.Empty : "-" + BillingPeriodNo.ToString().PadLeft(2,'0');
        return string.Format("{0}-{1}{2}", BillingYear, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dtTemp.Month).Substring(0,3),strPeriod);
      }
    }

      // Property to get and set SMI Id
      public int SettleMethodIndicatorId { get; set; }

      // Property to get and set SMI
      public string SettleMethodIndicator { get; set; }

      // Property to get and set Billing Member Id
      public int BillingMemberId { get; set; }

      // Property to get and set Billing Member Code
      public string BillingMemberCode { get; set; }

      // Property to get and set Billing Member Name
      public string BillingMemberName { get; set; }

      // Property to get and set Billed Member Id
      public int BilledMemberId { get; set; }

      // Property to get and set Billed Member Code
      public string BilledMemberCode { get; set; }

      // Property to get and set Billed Member Name
      public string BilledMemberName { get; set; }

      // Property to get and set Invoice/Form C Id
      public Guid InvoiceId { get; set; }

      // Property to get and set Unique Invoice/Form C No.
      public string UInvoiceId { get; set; }

      // Property to get and set Invoice/Form C No
      public string InvoiceNo { get; set; }

      // Property to get and set Invoice/Form C Date
      public DateTime? InvoiceDate { get; set; }

      // Property to Format Invoice date.
      public string FormatedInvoiceDate
      {
          get
          {
              string formatedDate = string.Empty;
              if (InvoiceDate != null)
              {
                  formatedDate = InvoiceDate.Value.ToString("dd MMM yyyy");
              }
              return formatedDate;
          }
      }

      // Property to get and set Invoice/Form C Status Id
      public int InvoiceStatusId { get; set; }

      // Property to get and set Billing Category Id
      public int BillingCategoryId { get; set; }

      // Property to get and set Billing Category
      public string BillingCategory { get; set; }

      // Property to get and set Invoice Currency Id
      public int InvoiceCurrencyId { get; set; }

      // Property to get and set Invoice Currency
      public string InvoiceCurrency { get; set; }

      //CMP#415- Clearance Currency and Amount field in Dashboard
      // Property to get and set Currency of Billing 
      public string CurrancyOfBilling { get; set; }

      // Property to get and set Invoice Amount
      public decimal InvoiceAmount { get; set; }

      //CMP#415- Clearance Currency and Amount field in Dashboard
      // Property to get and set Currency Invoice Amount
      public decimal CurrencyAmount { get; set; }

      // Property to get and set Validation Status Id
      public int ValidationStatusId { get; set; }

      // Property to get and set Validation Status
      public byte ValidationStatus { get; set; }

      // Property to get and set Value Confirmation Status Id
      public int ValueConfirmationStatusId { get; set; }

      // Property to get and set Value Confirmation Status
      public byte ValueConfirmationStatus { get; set; }

      // Property to get and set Digital Signature Status Id
      public int DigitalSignatureStatusId { get; set; }

      // Property to get and set Digital Signature Status
      public byte DigitalSignatureStatus { get; set; }

      // Property to get and set Settlement File Status Id
      public int SettlementFileStatusId { get; set; }

      // Property to get and set Settlement File Status
      public byte SettlementFileStatus { get; set; }

      // Property to get and set Presented Status Id
      public int PresentedStatusId { get; set; }

      // Property to get and set Presented Status
      public byte PresentedStatus { get; set; }

      // Property to get and set Is Member Suspended
      public bool IsSuspended { get; set; }

      // Property to get and set Is Invoice Marked For Late Submission
      public bool IsLateSubmitted { get; set; }

      //This is a cumulative property generated depending on values of IsSuspended and IsLateSubmitted. 
      //This is added for use by presentation Layer
      public string IsSuspendedLateSubmitted
      {
        get
        {
          if (IsSuspended && IsLateSubmitted) return "11";
          else if (IsSuspended == false && IsLateSubmitted == false) return "00";
          else if (IsSuspended && IsLateSubmitted == false) return "10";
          else if (IsSuspended == false && IsLateSubmitted) return "01";

          return "00";
        }
      }

      public DateTime? ReceivedInIS { get; set; }

      public DateTime? ValidationStatusDate { get; set; }

      // Property to Format ValidationStatusDate
      public string FormatedValidationStatusDate
      {
        get
        {
          string formatedDate = string.Empty;
          if (ValidationStatusDate != null)
            formatedDate = ValidationStatusDate.Value.ToString("dd MMM yyyy HH:mm");

          return formatedDate;
        }
      }

      public DateTime? ValueConfirmationStatusDate { get; set; }

      // Property to Format ValueConfirmationStatusDate.
      public string FormatedValueConfirmationStatusDate
      {
        get
        {
          string formatedDate = string.Empty;
          if (ValueConfirmationStatusDate != null)
            formatedDate = ValueConfirmationStatusDate.Value.ToString("dd MMM yyyy HH:mm");

          return formatedDate;
        }
      }

      public DateTime? DigitalSignatureStatusDate { get; set; }

      // Property to Format DigitalSignatureStatusDate.
      public string FormatedDigitalSignatureStatusDate
      {
        get
        {
          string formatedDate = string.Empty;
          if (DigitalSignatureStatusDate != null)
            formatedDate = DigitalSignatureStatusDate.Value.ToString("dd MMM yyyy HH:mm");

          return formatedDate;
        }
      }


      public DateTime? SettlementFileStatusDate { get; set; }

      // Property to Format SettlementFileStatusDate.
      public string FormatedSettlementFileStatusDate
      {
        get
        {
          string formatedDate = string.Empty;
          if (SettlementFileStatusDate != null)
            formatedDate = SettlementFileStatusDate.Value.ToString("dd MMM yyyy HH:mm");

          return formatedDate;
        }
      }

      public DateTime? PresentedStatusDate { get; set; }

      // Property to Format PresentedStatusDate.
      public string FormatedPresentedStatusDate
      {
        get
        {
          string formatedDate = string.Empty;
          if (PresentedStatusDate != null)
            formatedDate = PresentedStatusDate.Value.ToString("dd MMM yyyy HH:mm");

          return formatedDate;
        }
      }

      public string InvoiceStatusDescription { get; set; }

      //CMP559 : Add Submission Method Column to Processing Dashboard
      public int SubmissionMethodId { get; set; }

      public string FileName { get; set; }

      public Guid FileLogId { get; set; }

      public int IsPurged { get; set;}

      //CMP529 : Daily Output Generation for MISC Bilateral Invoices
      public int? DailyDeliveryStatusId { get; set; }

      //CMP529 : Delivered On for MISC Bilateral Invoices
      public DateTime? DeliveredOn  { get; set; }

    }// end ProcessingDashboardInvoiceStatus class
}// end namespace
