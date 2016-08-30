using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.SupportingMismatchDocument
{
    /// <summary>
    /// This class hold the property for Supporting Mismatch document
    /// </summary>
   public class SupportingMismatchDocument
    {
       /// <summary>
       /// Property to get and set Billing month
       /// </summary>
       public int BillingMonth { get; set; }

       /// <summary>
       /// Property to get and set Billing Period
       /// </summary>
       public int BillingPeriod { get; set; }

       /// <summary>
       /// Property to get and set Billing Year
       /// </summary>
       public int BillingYear { get; set; }

       /// <summary>
       /// Property to get and set Settelment method
       /// </summary>
       public int SettlementMethodId { get; set; }

       /// <summary>
       /// Property to get and set Invoice type 
       /// </summary>
       public int InvoiceTypeId { get; set; }

       /// <summary>
       /// Property to get and set Invoice No 
       /// </summary>
       public string InvoiceNo { get; set; }

       /// <summary>
       /// Property to get and set member code
       /// </summary>
       public string MemberCode { get; set; }


       /// <summary>
       /// Property to get and set member id
       /// </summary>
       public int MemberId { get; set; }

       public bool RadioMemberId { get; set; }

       public string Airlincename { get; set; }

       public string AirlineCode { get; set; }

       /// <summary>
       /// Property to get and set Coupon Breakdown Serial Number
       /// </summary>
       public int CouponBreakdownSerialNumber { get; set; }

       

    }// End SupportingMismatchDocument class
}// End namespace
