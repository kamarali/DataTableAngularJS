using System;

namespace Iata.IS.Web.Reports.Cargo.RejectionAnalysis
{
    public class CgoRejectionAnalysisRecReportModel
    {
        /// <summary>
        /// Property to get and set Billing Month
        /// </summary>
        public string BillingMonth { get; set; }
        /// <summary>
        /// Property to get and set Billing Year
        /// </summary>
        public int BillingYear { get; set; }
        /// <summary>
        ///  Property to get and set Intity Member Alpha Code
        /// </summary>
        public string EntityCodeAlpha { get; set; }
        /// <summary>
        ///  Property to get and set Intity Member numeric Code
        /// </summary>
        public string EntityCodeNumeric { get; set; }
        /// <summary>
        ///  Property to get and set Intity Member Display Name
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        ///  Property to get and set Count of Prepaid  AWBs
        /// </summary>
        public int PpAwbCount { get; set; }
        /// <summary>
        ///   Property to get and set Value of Prepaid  AWBs
        /// </summary>
        public Decimal PpAwbAmount { get; set; }
        /// <summary>
        ///  Property to get and set No of  Collect AWBs
        /// </summary>
        public int CcAwbCount { get; set; }
        /// <summary>
        /// Property to get and set Value of  Collect  AWBs
        /// </summary>
        public Decimal CcAwbAmount { get; set; }
        /// <summary>
        /// Property to get and set  count of Prepaid  AWBs Rejected 
        /// </summary>
        public int PpAwbRejCount { get; set; }
        /// <summary>
        /// Property to get and set value of Prepaid  AWBs Rejected 
        /// </summary>
        public Decimal PpAwbRejAmount { get; set; }
        /// <summary>
        /// Property to get and set  count of Collect  AWBs Rejected 
        /// </summary>
        public int CcAwbRejCount { get; set; }
        /// <summary>
        /// Property to get and set  value of Collect AWBs Rejected 
        /// </summary>
        public Decimal CcAwbRejAmount { get; set; }
        /// <summary>
        /// Property to get and set CurrencyCode  
        /// </summary>
        public string CurrencyCode { get; set; }
    }
}