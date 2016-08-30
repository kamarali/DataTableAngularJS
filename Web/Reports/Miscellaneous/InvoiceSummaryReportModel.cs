using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Web.Reports.Miscellaneous
{
    public class InvoiceSummaryReportModel
    {


        /// <summary>
        /// Property to get and set Billing Month
        /// </summary>
        [DisplayName("Clearance_Month")]
        public string ClearanceMonth { get; set; }

        /// <summary>
        /// Property to get and set Billing Year
        /// </summary>
        public int BillingYear { get; set; }

        /// <summary>
        /// Property to get and set Billing Month
        /// </summary>
        [DisplayName("Clearance_Period")]
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
                return (SubmissionMethod)SubmissionMethodId;
            }

        }

        [DisplayName("Data_Source")]
        public string SubmissionMethodDisplayText { get; set; }

        /// <summary>
        /// Author: Sanket Shrivastava
        /// Property to get and set Settlement Method Indicator Text  
        /// </summary>
        [DisplayName("Settlement_Method_Indicator")]
        public string SettlementMethodIndicator { get; set; }

        /// <summary>
        ///  Property to get and set Member Alpha Code
        /// </summary>
        [DisplayName("Entity_Code")]
        public string MemberCodeAlpha { get; set; }

        /// <summary>
        ///  Property to get and set Member numeric Code
        /// </summary>
        public string MemberCodeNumeric { get; set; }

        /// <summary>
        ///  Property to get and set Member Display Name
        /// </summary>
        [DisplayName("Entity_Name")]
        public string MemberName { get; set; }

        /// <summary>
        ///  Property to get and set Invoice Number
        /// </summary>
        [DisplayName("Invoice_Number")]
        public string InvoiceNo { get; set; }

        /// <summary>
        ///  Property to get and set Invoice Date
        /// </summary>
        [DisplayName("Invoice_Date")]
        public string InvoiceDate { get; set; }

        /// <summary>
        /// Get and set Charge Category
        /// </summary>
        [DisplayName("Charge_Category")]
        public string ChargeCategory { get; set; }

        /// <summary>
        /// Property to get and set CurrencyCode  
        /// </summary>
        [DisplayName("Currency_Code")]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Property to get and set Billing Currency Amount
        /// </summary>
        [DisplayName("Billing_Currency_Amount")]
        public Decimal BillingCurrencyAmount { get; set; }

        /// <summary>
        /// Get and set Exchange Rate
        /// </summary>
        [DisplayName("Exchange_Rate")]
        public Decimal ExchangeRate { get; set; }

        /// <summary>
        /// Property to get and set Clearance Currency
        /// </summary>
        [DisplayName("Clearance_Currency")]
        public string BillingCurrency { get; set; }

        /// <summary>
        /// Property to get and set Clearance Currency Amount
        /// </summary>
        [DisplayName("Clearance_Currency_Amount")]
        public Decimal ClearanceCurrencyAmount { get; set; }


        public int SubmissionMethodId { set; get; }

        /// <summary>
        /// Author: Kamarali Dukandar
        /// Purpose: Property to get and set Invoice Type
        /// </summary>
        [DisplayName("Transaction_Type")]
        public string InvoiceType { get; set; }

    }
}