using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Reports.Miscellaneous;


namespace Iata.IS.Business.Reports.Miscellaneous
{
   public interface  IMiscellaneous
   {
       /// <summary>
       /// Author: Sanket Shrivastava
       /// Date of Creation: 17-10-2011
       /// Purpose: Misc: Invoice Summary Report
       /// Author: Kamarali Dukandar
       /// Date of Creation: 15-07-2016
       /// Purpose: Misc: CMP663 MISC Invoice Summary Reports - Add 'Transaction Type'
       /// </summary>
       /// <param name="billingType"></param>
       /// <param name="fMonth"></param>
       /// <param name="fYear"></param>
       /// <param name="fPeriod"></param>
       /// <param name="tYear"></param>
       /// <param name="tMonth"></param>
       /// <param name="tPeriod"></param>
       /// <param name="billedEntityCode"></param>
       /// <param name="billingEntityCode"></param>
       /// <param name="dataSource"></param>
       /// <param name="settlementMethod"></param>
       /// <param name="chargeCategory"></param>
       /// <param name="currencyCode"></param>
       /// <param name="invoiceType"></param>
       /// <returns></returns>
       List<InvoiceSummaryReportModel> GetInvoiceSummaryReportDetails(int billingType,
                                                                      int fMonth,
                                                                      int fYear,
                                                                      int fPeriod,
                                                                      int tMonth,
                                                                      int tYear,
                                                                      int tPeriod, 
                                                                      int? billedEntityCode,
                                                                      int? billingEntityCode,
                                                                      int? dataSource,
                                                                      int? settlementMethod,
                                                                      int? chargeCategory,
                                                                      int? currencyCode,
                                                                      int? invoiceType);

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
       List<MiscSubstitutionValuesReportResult> GetMiscSubstitutionValuesReportDetails(int fromYear,
                                                                                       int fromMonth,
                                                                                       int fromPeriod,
                                                                                       int toYear,
                                                                                       int toMonth,
                                                                                       int toPeriod,
                                                                                       int billingEntityCode,
                                                                                       int? billedEntityCode,
                                                                                       int? chargeCategory,
                                                                                       int? chargeCode,
                                                                                       string invoiceNumber);

   }
}
