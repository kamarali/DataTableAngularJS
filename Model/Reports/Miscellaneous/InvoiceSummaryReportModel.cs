using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Reports.Miscellaneous
{
   public class InvoiceSummaryReportModel
    {


        /// <summary>
        /// Property to get and set Billing Month
        /// </summary>
       
        public string ClearanceMonth { get; set; }

        /// <summary>
        /// Property to get and set Billing Year
        /// </summary>
        public int BillingYear { get; set; }

        /// <summary>
        /// Property to get and set Billing Month
        /// </summary>

        public string ClearancePeriod { get; set; }

       /// <summary>
       /// Author: Sanket Shrivastava
       /// Purpose: Property to get and set Submission method
       /// </summary>
       /// <remarks>
       /// Wrapper property over SubmissionMethod Id.
       /// </remarks>
       public SubmissionMethod SubmissionMethod
       {
           set
           {
               SubmissionMethodId = Convert.ToInt32(value);
           }
           get
           {
               return (SubmissionMethod) SubmissionMethodId;
           }

       }


       public string SubmissionMethodDisplayText { get; set; }

        /// <summary>
        ///  Property to get and set Member Alpha Code
        /// </summary>

        public string MemberCodeAlpha { get; set; }

        /// <summary>
        ///  Property to get and set Member numeric Code
        /// </summary>
        public string MemberCodeNumeric { get; set; }

        /// <summary>
        ///  Property to get and set Member Display Name
        /// </summary>

        public string MemberName { get; set; }

        /// <summary>
        ///  Property to get and set Invoice Number
        /// </summary>
     
       public string InvoiceNo { get; set; }

        /// <summary>
        ///  Property to get and set Invoice Date
        /// </summary>

        public string InvoiceDate { get; set; }

        /// <summary>
        /// Property to get and set CurrencyCode  
        /// </summary>

        public string CurrencyCode { get; set; }

        /// <summary>
        /// Author: Sanket Shrivastava
        /// Property to get and set Settlement Method Indicator Text  
        /// </summary>
     
        public string SettlementMethodIndicator { get; set; }

        /// <summary>
        /// Get and set Charge Category
        /// </summary>
     
        public string ChargeCategory { get; set; }

         /// <summary>
         /// Get and set Exchange Rate
         /// </summary>

        public Decimal ExchangeRate { get; set; }

        /// <summary>
        /// Property to get and set Billing Currency Amount
        /// </summary>

         public Decimal BillingCurrencyAmount { get; set; }

        /// <summary>
        /// Property to get and set Clearance Currency
        /// </summary>

        public string BillingCurrency { get; set; }

        /// <summary>
        /// Property to get and set Clearance Currency Amount
        /// </summary>
       
        public Decimal ClearanceCurrencyAmount { get; set; }


        /// <summary>
        /// Author: Kamarali Dukandar
        /// Purpose: Property to get and set Invoice Type
        /// </summary>
        public string InvoiceType { get; set; }
        
        public int SubmissionMethodId { set; get; }

        
    }
}
