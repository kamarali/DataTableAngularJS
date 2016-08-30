using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.ExternalInterfaces.ICHInterface
{
  /*This class corresponds to invoice data which needs to be set as part of response to ICH cross check request */
  public class CrossCheckRequestInvoice
  {
    public Guid UniqueInvoiceNumber { get; set; }
    public string MemberCodeNumeric { get; set; }
    public decimal ClearanceCurrencyAmount { get; set; }
    public string ClearanceCurrencyCode { get; set; }
    public string BillingCategory { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime ICHSubmissionTimestamp { get; set; }
  }
}
