using System;

namespace Iata.IS.Model.Reports.PaxCgoMscTopTenPartner
{
    public class PaxCgoMscTopTenPartnerReportModel
    {
        /// <summary>
        /// Property to get and set Billing Category
        /// </summary>
        public string BillingCategory { get; set; }
        /// <summary>
        /// Property to get and set CUrrency Code
        /// </summary>
        public string CurrencyCode { get; set; }
        /// <summary>
        ///  Property to get and set Billing Member Alpha Code
        /// </summary>
        public string BillingMemberCodeAlpha { get; set; }
        /// <summary>
        ///  Property to get and set Billing Member numeric Code
        /// </summary>
        public string BillingMemberCodeNumeric { get; set; }
        /// <summary>
        ///  Property to get and set Billing Member Display Name
        /// </summary>
        public string BillingMemberName { get; set; }
        /// <summary>
        ///  Property to get and set Billed Member Alpha Code
        /// </summary>
        public string BilledMemberCodeAlpha { get; set; }
        /// <summary>
        ///   Property to get and set Billed Member numeric Code
        /// </summary>
        public string BilledMemberCodeNumeric { get; set; }
        /// <summary>
        ///  Property to get and set Billed Member Display Name
        /// </summary>
        public string BilledMemberName { get; set; }
        /// <summary>
        /// Property to get and set Total amount zero month befor 
        /// </summary>
        public decimal ZeroMonthBefor { get; set; }
        /// <summary>
        /// Property to get and set Total amount one month befor 
        /// </summary>
        public decimal OneMonthBefor { get; set; }
        /// <summary>
        /// Property to get and set Total amount two month befor 
        /// </summary>
        public decimal TwoMonthBefor { get; set; }
        /// <summary>
        /// Property to get and set  Total amount three month befor 
        /// </summary>
        public decimal ThreeMonthBefor { get; set; }
        /// <summary>
        /// Property to get and set  Total amount four month befor 
        /// </summary>
        public decimal FourMonthBefor { get; set; }
        /// <summary>
        /// Property to get and set Total amount five month befor   
        /// </summary>
        public decimal FiveMonthBefor { get; set; }

        /// <summary>
        /// Property to get and set Total amount six month befor 
        /// </summary>
        public decimal SixMonthBefor { get; set; }
        /// <summary>
        /// Property to get and set  Total amount seven month befor 
        /// </summary>
        public decimal SevenMonthBefor { get; set; }
        /// <summary>
        /// Property to get and set Total amount eight month befor 
        /// </summary>
        public decimal EightMonthBefor { get; set; }
        /// <summary>
        /// Property to get and set  Total amount nine month befor 
        /// </summary>
        public decimal NineMonthBefor { get; set; }
        /// <summary>
        /// Property to get and set  Total amount ten month befor 
        /// </summary>
        public decimal TenMonthBefor { get; set; }
        /// <summary>
        /// Property to get and set Total amount eleven month befor   
        /// </summary>
        public decimal ElevenMonthBefor { get; set; }
        /// <summary>
        /// Property to get and set total amount of 12- months  
        /// </summary>
        public decimal Total { get; set; }
    }
}