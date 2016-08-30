using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Reports.Enums;

namespace Iata.IS.Model.Reports
{
  public class SuspendedInvoiceDetails : EntityBase<Guid>
  {
    public string InvoiceNumber { get; set; }

    public DateTime? InvoiceDate { get; set; }

    // Property to get and set Billing Category Id
    public int BillingCategoryId { get; set; }

    // Property to get and set Billing Category
    public string BillingCategory { get; set; }

    public int BillingMonth { get; set; }

    public string OriginalBillingMonth { get; set; }

    public int OriginalPeriod { get; set; }

    // Property to get and set Invoice Currency Id
    public int BillingCurrencyId { get; set; }

    // Property to get and set Invoice Currency
    public string BillingCurrency { get; set; }

    // Property to get and set Billed Member Id
    public int BilledMemberId { get; set; }

    // Property to get and set Billed Member Code
    public string BilledMemberCode { get; set; }

    // Property to get and set Billed Member Name
    public string BilledMemberName { get; set; }

    public int SettlementMethodId { get; set; }

    public SMI InvoiceSmi
    {
      get
      {
        return (SMI)SettlementMethodId;
      }
      set
      {
        SettlementMethodId = Convert.ToInt32(value);
      }
    }


    //public string DisplayOriginalBillingMonth
    //{
    //  get
    //  {
    //    if(BillingMonth.ToString().Length<2)
    //      return OriginalBillingMonth + "0" + BillingMonth;
        
    //      return OriginalBillingMonth.ToString() + BillingMonth;
    //  }
    //}

    //public string DisplayOriginalPeriod
    //{
    //  get
    //  {
    //    return EnumList.GetPeriodDisplayValue((InvoicePeriod)OriginalPeriod);
    //  }
    //}

    public string SettlementMethodDisplayText { get; set; }

    public string ResubmissionStatusDisplayText { get; set; }

    //public string DisplaySuspensionMonth
    //{
    //  get
    //  {
    //    if (SuspensionMonth == 0)
    //      return String.Empty;
    //    else
    //      return SuspensionMonth.ToString();
    //  }
    //}

    //public string DisplaySuspensionMonth { get; set; }
    //public string DisplaySuspensionPeriod
    //{
    //  get
    //  {
    //    return EnumList.GetPeriodDisplayValue((InvoicePeriod)SuspensionPeriod);
    //  }
    //}
   //public string DisplayReinstatementMonth
   // {
   //   get
   //   {
   //     if (ReinstatementMonth == 0)
   //       return String.Empty;
   //     else
   //     {
   //      return ReinstatementMonth.ToString();
   //     }
   //   }
   // }

   //public string DisplayReinstatementPeriod
   //{
   //  get
   //  {
   //    return EnumList.GetPeriodDisplayValue((InvoicePeriod)ReinstatementPeriod);
   //  }
   //}

    //public string DisplayResubmissionBillingMonth
    //{
    //  get
    //  {
    //    return EnumList.GetMonthDisplayValue((Month)ResubmissionBillingMonth);
    //  }
    //}

    //public string DisplayResubmissionPeriod
    //{
    //  get
    //  {
    //    return EnumList.GetPeriodDisplayValue((InvoicePeriod)ResubmissionPeriod);
    //  }
    //}

    public string SuspensionMonth { get; set; }

    public int SuspensionPeriod { get; set; }

    public string ReinstatementMonth { get; set; }

    public int ReinstatementPeriod { get; set; }

    public string ResubmissionBillingMonth { get; set; }

    public int ResubmissionPeriod { get; set; }

    public string ResubmissionRemarks { get; set; }

    // Property to get and set Invoice Amount
    public decimal InvoiceAmount { get; set; }

    public string BillingCurrencyDisplayText { get; set; }

    // Resubmission Status
    public int? ResubmissionStatusId { get; set; }

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

    public string DisplayBilledMemberCode
    {
      get { return BilledMemberCode + "-" + BilledMemberName; }
    }
  }
}
