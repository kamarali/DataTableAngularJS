using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Reports.Miscellaneous;
using Iata.IS.Model.Reports.Miscellaneous;

namespace Iata.IS.Business.Reports.Miscellaneous.Impl
{
    public class Miscellaneous : IMiscellaneous
    {
        public IMiscellaneousData ImiscellaneousdataProp { get; set; }

        public Miscellaneous(IMiscellaneousData Imiscellaneousdata )
        {
            ImiscellaneousdataProp = Imiscellaneousdata;
        }

        /// <summary>
        /// Author: Sanket Shrivastava
        /// Date of Creation: 11-10-2011
        /// Purpose: Pax: Sampling Billing Analysis
        /// Author: Kamarali Dukandar
        /// Date of Creation: 15-07-2016
        /// Purpose: Pax: CMP#663 MISC Invoice Summary Reports - Add 'Transaction Type'
        /// </summary>
        /// <param name="billingType"></param>
        /// <param name="fMonth"></param>
        /// <param name="fYear"></param>
        /// <param name="fPeriod"></param>
        /// <param name="tYear"></param>
        /// <param name="tMonth"></param>
        /// <param name="tPeriod"></param>
        /// <param name="billedEntityId"></param>
        /// <param name="billingEntityId"></param>
        /// <param name="dataSource"></param>
        /// <param name="settlementMethod"></param>
        /// <param name="chargeCategory"></param>
        /// <param name="currencyCode"></param>
        /// <param name="invoiceType"></param>
        /// <returns></returns>
        /// CMP#596: converted int to long to support membernumeric code upto 12 digits
        public List<InvoiceSummaryReportModel> GetInvoiceSummaryReportDetails(int billingType,
                                                                      int fMonth,
                                                                      int fYear,
                                                                      int fPeriod,
                                                                      int tMonth,
                                                                      int tYear,
                                                                      int tPeriod,
                                                                      int? billedEntityId,
                                                                      int? billingEntityId,
                                                                      int? dataSource,
                                                                      int? settlementMethod,
                                                                      int? chargeCategory,
                                                                      int? currencyCode,
                                                                      int? invoiceType)
        {
            return ImiscellaneousdataProp.GetInvoiceSummaryReportDetails(billingType,
                                                                               fMonth,
                                                                               fYear,
                                                                               fPeriod,
                                                                               tMonth,
                                                                               tYear,
                                                                               tPeriod,
                                                                               billedEntityId,
                                                                               billingEntityId,
                                                                               dataSource,
                                                                               settlementMethod,
                                                                               chargeCategory,
                                                                               currencyCode,
                                                                               invoiceType);
        } // End of GetInvoiceSummaryReportDetails

        /// <summary>
        /// Author: Sachin Pharande
        /// Date of Creation: 30-11-2011
        /// Purpose: To Get the Substitution Values Report Result
        /// </summary>
        /// <param name="fromYear"></param>
        /// <param name="fromMonth"></param>
        /// <param name="fromPeriod"></param>
        /// <param name="toYear"></param>
        /// <param name="toMonth"></param>
        /// <param name="toPeriod"></param>
        /// <param name="billingEntityCode"></param>
        /// <param name="billedEntityCode"></param>
        /// <param name="chargeCategory"></param>
        /// <param name="chargeCode"></param>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public List<MiscSubstitutionValuesReportResult> GetMiscSubstitutionValuesReportDetails(int fromYear,
                                                                                               int fromMonth,
                                                                                               int fromPeriod,
                                                                                               int toYear,
                                                                                               int toMonth,
                                                                                               int toPeriod,
                                                                                               int billingEntityCode,
                                                                                               int? billedEntityCode,
                                                                                               int? chargeCategory,
                                                                                               int? chargeCode,
                                                                                               string invoiceNumber)
        {
            return ImiscellaneousdataProp.GetMiscSubstitutionValuesReportDetails(fromYear,
                                                                                 fromMonth,
                                                                                 fromPeriod,
                                                                                 toYear,
                                                                                 toMonth,
                                                                                 toPeriod,
                                                                                 billingEntityCode,
                                                                                 billedEntityCode,
                                                                                 chargeCategory,
                                                                                 chargeCode,
                                                                                 invoiceNumber);
        }
    }
}
