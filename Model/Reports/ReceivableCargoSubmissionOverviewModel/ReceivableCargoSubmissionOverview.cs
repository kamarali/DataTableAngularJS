using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.ReceivableCargoSubmissionOverviewModel
{
    public class ReceivableCargoSubmissionOverview
    {
        /// <summary>
        /// Property to get and set BillingYearFrom
        /// </summary>
        public int BillingYearFrom { get; set; }

        /// <summary>
        /// Property to get and set BillingYearTo
        /// </summary>
        public int BillingYearTo { get; set; }

        /// <summary>
        /// Property to get and set BillingMonth
        /// </summary>
        public string BillingMonth { get; set; }

        /// <summary>
        /// Property to get and set BillingMonthFrom
        /// </summary>
        public int BillingMonthFrom { get; set; }

        /// <summary>
        /// Property to get and set BillingMonthTo
        /// </summary>
        public int BillingMonthTo { get; set; }

        /// <summary>
        /// Property to get and set PeriodNo
        /// </summary>
        public int PeriodNo { get; set; }

        /// <summary>
        /// Property to get and set PeriodNoFrom
        /// </summary>
        public int PeriodNoFrom { get; set; }

        /// <summary>
        /// Property to get and set PeriodNoTo
        /// </summary>
        public int PeriodNoTo { get; set; }

        /// <summary>
        /// Property to get and set BilledEntity
        /// </summary>
        public string BilledEntity { get; set; }

        /// <summary>
        /// Property to get and set BillingEntity
        /// </summary>
        public string BillingEntity { get; set; }

        /// <summary>
        /// Property to get and set BillingMonthFrom
        /// </summary>
        public string SettlementMethod { get; set; }

        /// <summary>
        /// Property to get and set Output
        /// </summary>
        public int Output { get; set; }

        /// <summary>
        /// Property to get and set InvoiceNo
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// Property to get and set BillingRecord
        /// </summary>
        public string BillingRecord { get; set; }

        /// <summary>
        /// Property to get and set TotalNoOfBillingRecord
        /// </summary>
        public int TotalNoOfBillingRecord { get; set; }

        /// <summary>
        /// Property to get and set ListingCurrency
        /// </summary>
        public string ListingCurrency { get; set; }

        /// <summary>
        /// Property to get and set TotalWeightCharges
        /// </summary>
        public Double TotalWeightCharges { get; set; }

        /// <summary>
        /// Property to get and set TotalValuationCharges
        /// </summary>
        public Double TotalValuationCharges { get; set; }

        /// <summary>
        /// Property to get and set TotalOtherCharges
        /// </summary>
        public Double TotalOtherCharges { get; set; }

        /// <summary>
        /// Property to get and set TotalIscAmount
        /// </summary>
        public Double TotalIscAmount { get; set; }

        /// <summary>
        /// Property to get and set TotalVatAmount
        /// </summary>
        public Double TotalVatAmount { get; set; }

        /// <summary>
        /// Property to get and set BillingCodeSubTotal
        /// </summary>
        public Double BillingCodeSubTotal { get; set; }

        /// <summary>
        /// Property to get and set ListingToBillingRate
        /// </summary>
        public Double ListingToBillingRate { get; set; }

        /// <summary>
        /// Property to get and set BillingCurrency
        /// </summary>
        public string BillingCurrency { get; set; }

        /// <summary>
        /// Property to get and set InvoiceBillingAmount
        /// </summary>
        public Double InvoiceBillingAmount { get; set; }
    }

}
