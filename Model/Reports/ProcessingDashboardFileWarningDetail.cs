using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Reports
{
  public class ProcessingDashboardFileWarningDetail
  {
   
    // Property to get and set Billed Member Code
    public string BilledMemberCode { get; set; }

    // Property to get and set Billed Member Name
    public string BilledMemberName { get; set; }

    // Property to get and set Invoice number
    public string InvoiceNo { get; set; }

    // Property to get and set Invoice Date
    public DateTime? InvoiceDate { get; set; }

    public string FormatedInvoiceDate
    {
      get
      {
        string formatedDate = string.Empty;
        if (InvoiceDate != null)
        {
          formatedDate = InvoiceDate.Value.ToString("dd MMM yyyy HH:mm");
        }
        return formatedDate;
      }
    }

    // Property to get and set Invoice Currency
    public string InvoiceCurrency { get; set; }

    // Property to get and set Invoice Amount
    public decimal InvoiceAmount { get; set; }

    // Property to get and set Validation Status
    public string ValidationStatus { get; set; }

  }
}
