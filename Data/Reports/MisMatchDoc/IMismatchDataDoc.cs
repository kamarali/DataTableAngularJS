using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Reports.SupportingMismatchDocument;

namespace Iata.IS.Data.Reports.MisMatchDoc
{
   public interface IMismatchDataDoc
    {
        /// <summary>
        /// Returns list of Mismatch record
        /// </summary>
        /// <returns>list of Mismatch records</returns>
       List<SupportingMismatchDoc> GetMismatchDoc(int billingMonth, int billingPeriod, int billingYear , int airlinceCode, int settlementMethod, int invoiceType, string invoiceNo, int loginMemberid);
       List<SupportingMismatchDoc> GetCgoSupportingMismatchDoc(int billingMonth, int billingPeriod, int billingYear, int airlinceCode, int settlementMethod, int invoiceType, string invoiceNo, int loginMemberid);
       
       /// <summary>
       /// CMP# 519 Miscellaneous Supp Doc Mismatch Report
       /// Returns list of Misc Supp Documents Mismatch record
       /// </summary>
       /// <returns>list of Mismatch records</returns>
       List<MiscSupportingMismatchDoc> GetMiscMismatchDoc(int billingMonth, int billingPeriod, int billingYear, int airlineCode, int settlementMethod, int invoiceType, string invoiceNo, int loginMemberid);
    }
}
