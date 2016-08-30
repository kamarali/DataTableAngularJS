using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Reports
{
  public class ProcessingDashboardInvoiceDetail
  {
    // Property to get and set Billing Year
    public int BillingYear { get; set; }

    // Property to get and set Billing Month
    public int BillingMonth { get; set; }

    // Property to get and set Billing Period No
    public int BillingPeriodNo { get; set; }

    // Property to get and set Billing Period
    public string BillingPeriod
    {
      get
      {
        if (BillingYear != 0 && BillingMonth != 0)
        {
          var dtTemp = new DateTime(BillingYear, BillingMonth, 1);
          var strPeriod = (BillingPeriodNo == 0) ? String.Empty : BillingPeriodNo.ToString().PadLeft(2, '0');
          return string.Format("{0}-{1}-{2}", BillingYear,
                               CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dtTemp.Month).Substring(0, 3),
                               strPeriod);
        }

        return string.Empty;
      }
    }

    // Property to get and set SMI Id
    public int SettleMethodIndicatorId { get; set; }

    // Property to get and set SMI
    public string SettleMethodIndicator { get; set; }

    // Property to get and set Is Invoice Marked For Late Submission
    public bool IsLateSubmitted { get; set; }

    // Property to get and set Billed Member Id
    public int BilledMemberId { get; set; }

    // Property to get and set Billed Member Code
    public string BilledMemberCode { get; set; }

    // Property to get and set Billed Member Name
    public string BilledMemberName { get; set; }

    // Property to get and set billing Member Code
    public string BillingMemberCode { get; set; }

    // Property to get and set billing Member Name
    public string BillingMemberName { get; set; }

    // Property to get and set Invoice number
    public string InvoiceNo { get; set; }

    // Property to get and set Invoice Date
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
                /* string[] test = InvoiceDate.ToString().Split('/');
                var dtTemp = new DateTime(Convert.ToInt32(test[2].Split(' ')[0]),
                    Convert.ToInt32(test[0]),
                    Convert.ToInt32(test[1]),
                    Convert.ToInt32(test[2].Split(' ')[1].Split(':')[0]),
                    Convert.ToInt32(test[2].Split(' ')[1].Split(':')[1]),
                    Convert.ToInt32(test[2].Split(' ')[1].Split(':')[2]));
                formatedDate = string.Format("{0} {1} {2}", dtTemp.Day, dtTemp.ToString("MMM"), dtTemp.Year + " " + dtTemp.ToString("HH:mm")); */
            }
            return formatedDate;
        }
    }

    public Guid InvoiceId { get; set; }

    // Property to get and set Billing Category Id
    public int BillingCategoryId { get; set; }

    // Property to get and set Billing Category
    public string BillingCategory { get; set; }

    // Property to get and set Invoice Currency Id
    public int InvoiceCurrencyId { get; set; }

    // Property to get and set Invoice Currency
    public string InvoiceCurrency { get; set; }

    // Property to get and set Invoice Amount
    public decimal InvoiceAmount { get; set; }

    // Property to get and set Date on which Invoice received in IS 
    public DateTime? ReceivedInIS { get; set; }

    // Property to Format ReceivedInIS date.
    public string FormatedReceivedInISDate
    {
        get
        {
            string formatedDate = string.Empty;
            if (ReceivedInIS != null)
                formatedDate = ReceivedInIS.Value.ToString("dd MMM yyyy HH:mm");
            
            return formatedDate;
        }
    }

    // Property to get and set Invoice source
    public string Source { get; set; }

    // Property to get and set Validation Status Id
    public int ValidationStatusId { get; set; }

    // Property to get and set Validation Status
    public string ValidationStatus { get; set; }

    // Property to get and set Value Confirmation Status Id
    public int ValueConfirmationStatusId { get; set; }

    // Property to get and set Value Confirmation Status
    public string ValueConfirmationStatus { get; set; }

    // Property to get and set Digital Signature Status Id
    public int DigitalSignatureStatusId { get; set; }

    // Property to get and set Digital Signature Status
    public string DigitalSignatureStatus { get; set; }

    // Property to get and set Settlement File Status Id
    public int SettlementFileStatusId { get; set; }

    // Property to get and set Settlement File Status
    public string SettlementFileStatus { get; set; }

    // Property to get and set Presented Status Id
    public int PresentedStatusId { get; set; }

    // Property to get and set Presented Status
    public string PresentedStatus { get; set; }

    public DateTime? ValidationStatusDate { get; set; }

    // Property to Format ValidationStatusDate.
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
  }
}
