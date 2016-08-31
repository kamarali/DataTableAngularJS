using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Reports.MisMatchDoc;
using Iata.IS.Model.Reports.SupportingMismatchDocument;

namespace Iata.IS.Business.Reports.MisMatchDoc.Impl
{
    public class Mismatchdoc : IMismatchDoc
    {
        public IMismatchDataDoc MismatchDataDoc;

        public Mismatchdoc(IMismatchDataDoc _mismatchDoc)
        {
            MismatchDataDoc = _mismatchDoc;
        }
        /// <summary>
        /// This method is used to get Supporting Mismatch document list
        /// </summary>
        /// <param name="billingMonth">Billing month</param>
        /// <param name="billingPeriod">Billing Period</param>
        /// <param name="airlinceCode">Airline Code</param>
        /// <param name="airlienceName">Airline Name</param>
        /// <param name="settlementMethod">Settlement method</param>
        /// <param name="invoiceType">Invoive type</param>
        /// <param name="invoiceNo">Invoice No</param>
        /// <returns>Get the list of mismatch doc</returns>
        public List<SupportingMismatchDoc> GetMismatchDoc(int billingMonth, int billingPeriod,int billingYear , int airlinceCode, int settlementMethod, int invoiceType, string invoiceNo, int loginMemberid)
        {
            return MismatchDataDoc.GetMismatchDoc(billingMonth, billingPeriod, billingYear , airlinceCode,
                                                  settlementMethod, invoiceType, invoiceNo,loginMemberid);
        }// end GetMismatchDoc

        public List<SupportingMismatchDoc> GetCgoMismatchDoc(int billingMonth, int billingPeriod, int billingYear, int airlinceCode, int settlementMethod, int invoiceType, string invoiceNo, int loginMemberid)
        {
            return MismatchDataDoc.GetCgoSupportingMismatchDoc(billingMonth, billingPeriod, billingYear, airlinceCode,
                                                  settlementMethod, invoiceType, invoiceNo, loginMemberid);
        }// end GetMismatchDoc

        /// <summary>
        /// CMP# 519 Miscellaneous Supp Doc Mismatch Report
        /// This method is used to get Supporting Mismatch document list
        /// </summary>
        /// <param name="billingMonth">Billing month</param>
        /// <param name="billingPeriod">Billing Period</param>
        /// <param name="billingYear">Billing Year</param>
        /// <param name="airlineCode">Airline Code</param>
        /// <param name="settlementMethod">Settlement method</param>
        /// <param name="invoiceType">Invoive type</param>
        /// <param name="invoiceNo">Invoice No</param>
        /// <returns>Get the list of mismatch doc</returns>
        public List<MiscSupportingMismatchDoc> GetMiscMismatchDoc(int billingMonth, int billingPeriod, int billingYear, int airlineCode, int settlementMethod, int invoiceType, string invoiceNo, int loginMemberid)
        {
          return MismatchDataDoc.GetMiscMismatchDoc(billingMonth, billingPeriod, billingYear, airlineCode,settlementMethod, invoiceType, invoiceNo, loginMemberid);
        }// end GetMismatchDoc
    }// end Mismatchdoc class
}// end namespace
