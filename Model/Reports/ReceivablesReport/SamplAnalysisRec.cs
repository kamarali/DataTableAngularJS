using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Reports.ReceivablesReport
{
    public class SamplAnalysisRec
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
        ///  Property to get and set Billed Member Alpha Code
        /// </summary>
        public string BilledMemberCodeAlpha { get; set; }


        /// <summary>
        ///  Property to get and set Billed Member numeric Code
        /// </summary>
        public string BilledMemberCodeNumeric { get; set; }


        /// <summary>
        ///  Property to get and set Billed Member Display Name
        /// </summary>
        public string BilledMemberName { get; set; }

        /// <summary>
        ///  Property to get and set Billed Member Alpha Code
        /// </summary>
        public string BillingMemberCodeAlpha { get; set; }


        /// <summary>
        ///  Property to get and set Billed Member numeric Code
        /// </summary>
        public string BillingMemberCodeNumeric { get; set; }


        /// <summary>
        ///  Property to get and set Billed Member Display Name
        /// </summary>
        public string BillingMemberName { get; set; }
        
        /// <summary>
        /// Property to get and set CurrencyCode  
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Property to get and set Total No Of Prime Coupons  
        /// </summary>
        public int TotPrimeCoupons { get; set; }

        /// <summary>
        /// Property to get and set Total Provisional billing amount  
        /// </summary>
        public int TotProvBAmount { get; set; }

        /// <summary>
        /// Property to get and set No Of UAF COUPONS(FormC) 
        /// </summary>
        public int NoOfUafCoupons { get; set; }

        /// <summary>
        /// Property to get and set No of FormD Coupons in Sample
        /// </summary>
        public int NoOfCouponFormD { get; set; }

        /// <summary>
        /// Property to get and set Form E Eval. Amount 
        /// </summary>
        public int FormEEvalAmt { get; set; }

        /// <summary>
        /// Property to get and set Form E Adjusted Amt.  
        /// </summary>
        public int FormEAdjAmt { get; set; }

        /// <summary>
        /// Property to get and set No. Of F coupons rejected 
        /// </summary>
        public int NoOfFCouponRej { get; set; }

        /// <summary>
        /// Property to get and Total F Rej. amt. 
        /// </summary>
        public int TotFRejAmt { get; set; }

        /// <summary>
        /// Property to get and set No. Of XF coupons rejected 
        /// </summary>
       public int NoOfXfCouponRej { get; set; }

        /// <summary>
       /// Property to get and set Total XF Rej. amt. 
        /// </summary>
       public int TotXfRejAmt { get; set; }


    }
}
