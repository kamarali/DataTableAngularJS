using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.SupportingMismatchDocument;

namespace Iata.IS.Data.Reports.MisMatchDoc.Impl
{
    public class MismatchDataDoc : Repository<InvoiceBase>, IMismatchDataDoc
    {
        public List<SupportingMismatchDoc> GetMismatchDoc(int billingMonth, int billingPeriod, int billingYear ,  int airlinceCode,  int settlementMethod, int invoiceType, string invoiceNo, int loginMemberid)
        {
            var parameters = new ObjectParameter[8];
        
            parameters[0] = new ObjectParameter(MisMatchDocConstant.BillingMonth, billingMonth);
            parameters[1] = new ObjectParameter(MisMatchDocConstant.Billingperiod, billingPeriod);
            parameters[2] = new ObjectParameter(MisMatchDocConstant.BillingYear, billingYear);
            parameters[3] = new ObjectParameter(MisMatchDocConstant.AirLinecode, airlinceCode);
            parameters[4] = new ObjectParameter(MisMatchDocConstant.SettlementMethod, settlementMethod);
            parameters[5] = new ObjectParameter(MisMatchDocConstant.Invoicetype, invoiceType);
            parameters[6] = new ObjectParameter(MisMatchDocConstant.InvoiceNo, invoiceNo);
            parameters[7] = new ObjectParameter(MisMatchDocConstant.LoginMemberId, loginMemberid);

            var list = ExecuteStoredFunction<SupportingMismatchDoc>(MisMatchDocConstant.GetSupportingMismatchDoc, parameters);


            return list.ToList();
        }// end GetMismatchDoc

        public List<SupportingMismatchDoc> GetCgoSupportingMismatchDoc(int billingMonth, int billingPeriod, int billingYear, int airlinceCode, int settlementMethod, int invoiceType, string invoiceNo, int loginMemberid)
        {
            var parameters = new ObjectParameter[8];

            parameters[0] = new ObjectParameter(MisMatchDocConstant.BillingMonth, billingMonth);
            parameters[1] = new ObjectParameter(MisMatchDocConstant.Billingperiod, billingPeriod);
            parameters[2] = new ObjectParameter(MisMatchDocConstant.BillingYear, billingYear);
            parameters[3] = new ObjectParameter(MisMatchDocConstant.AirLinecode, airlinceCode);
            parameters[4] = new ObjectParameter(MisMatchDocConstant.SettlementMethod, settlementMethod);
            parameters[5] = new ObjectParameter(MisMatchDocConstant.Invoicetype, invoiceType);
            parameters[6] = new ObjectParameter(MisMatchDocConstant.InvoiceNo, invoiceNo);
            parameters[7] = new ObjectParameter(MisMatchDocConstant.LoginMemberId, loginMemberid);

            var list = ExecuteStoredFunction<SupportingMismatchDoc>(MisMatchDocConstant.GetCgoSupportingMismatchDoc, parameters);


            return list.ToList();
        }// end GetMismatchDoc


        /// <summary>
        /// CMP# 519 Miscellaneous Supp Doc Mismatch Report
        /// Returns list of Misc Supp Documents Mismatch record
        /// </summary>
        /// <returns>list of Mismatch records</returns>
        public List<MiscSupportingMismatchDoc> GetMiscMismatchDoc(int billingMonth, int billingPeriod, int billingYear, int airlineCode, int settlementMethod, int invoiceType, string invoiceNo, int loginMemberid)
        {
          var parameters = new ObjectParameter[8];

          parameters[0] = new ObjectParameter(MisMatchDocConstant.BillingMonth, billingMonth);
          parameters[1] = new ObjectParameter(MisMatchDocConstant.Billingperiod, billingPeriod);
          parameters[2] = new ObjectParameter(MisMatchDocConstant.BillingYear, billingYear);
          parameters[3] = new ObjectParameter(MisMatchDocConstant.AirLinecode, airlineCode);
          parameters[4] = new ObjectParameter(MisMatchDocConstant.SettlementMethod, settlementMethod);
          parameters[5] = new ObjectParameter(MisMatchDocConstant.Invoicetype, invoiceType);
          parameters[6] = new ObjectParameter(MisMatchDocConstant.InvoiceNo, invoiceNo);
          parameters[7] = new ObjectParameter(MisMatchDocConstant.LoginMemberId, loginMemberid);

          var list = ExecuteStoredFunction<MiscSupportingMismatchDoc>(MisMatchDocConstant.GetMiscSupportingMismatchDoc, parameters);


          return list.ToList();
        }// end GetMismatchDoc for Misc

    }// end MismatchDataDoc class
}// end namespace
