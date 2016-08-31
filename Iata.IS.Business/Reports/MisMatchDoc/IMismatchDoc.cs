using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Reports.SupportingMismatchDocument;
using Iata.IS.Model.Reports.ValidationDetails;

namespace Iata.IS.Business.Reports.MisMatchDoc
{
    public interface IMismatchDoc
    {
        /// <summary>
        /// Returns list of Mismatch record
        /// </summary>
        /// <returns>list of Mismatch records</returns>
        /// CMP#596: converted int to long to support membernumeric code upto 12 digits
        List<SupportingMismatchDoc>GetMismatchDoc(int billingMonth,int billingPeriod, int billingYear , int airlinceCode,int settlementMethod,int invoiceType,string invoiceNo,int loginMemberid);
        List<SupportingMismatchDoc> GetCgoMismatchDoc(int billingMonth, int billingPeriod, int billingYear, int airlinceCode, int settlementMethod, int invoiceType, string invoiceNo, int loginMemberid);
        
        /// <summary>
        /// CMP# 519 Miscellaneous Supp Doc Mismatch Report
        /// This method is used to get Miscellaneous Supporting Documents Mismatch record
        /// </summary>
        /// <returns>list of Mismatch records</returns>
        /// CMP#596: converted int to long to support membernumeric code upto 12 digits
        List<MiscSupportingMismatchDoc> GetMiscMismatchDoc(int billingMonth, int billingPeriod, int billingYear,int airlinceCode, int settlementMethod, int invoiceType,string invoiceNo, int loginMemberid);
    }
}
