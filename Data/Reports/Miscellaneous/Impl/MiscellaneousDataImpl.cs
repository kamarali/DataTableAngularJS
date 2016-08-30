using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Reports.Miscellaneous;

namespace Iata.IS.Data.Reports.Miscellaneous.Impl
{
    public class MiscellaneousDataImpl : Repository<InvoiceBase>, IMiscellaneousData
    {
        /// <summary>
        /// Author: Kamarali Dukandar
        /// Date of Modification: 15-07-2016
        /// Purpose: Pax:CMP#663 MISC Invoice Summary Reports - Add 'Transaction Type'
        /// Added an extra parameter as invoiceType to call the stored procedure
        /// </summary>
        /// <param name="billingType"></param>
        /// <param name="fMonth"></param>
        /// <param name="fYear"></param>
        /// <param name="fPeriod"></param>
        /// <param name="tMonth"></param>
        /// <param name="tYear"></param>
        /// <param name="tPeriod"></param>
        /// <param name="billedEntityCode"></param>
        /// <param name="billingEntityCode"></param>
        /// <param name="dataSource"></param>
        /// <param name="settlementMethod"></param>
        /// <param name="chargeCategory"></param>
        /// <param name="currencyCode"></param>
        /// <param name="invoiceType"></param>
        /// <returns></returns>
        public List<InvoiceSummaryReportModel> GetInvoiceSummaryReportDetails(int billingType,
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
                                                                      int? invoiceType)
        {
            var parameters = new ObjectParameter[14];

            parameters[0] = new ObjectParameter(MiscellaneousDataImplConstants.BillingType,typeof(int)) { Value = billingType };
            parameters[1] = new ObjectParameter(MiscellaneousDataImplConstants.FMonth, typeof(int)) { Value = fMonth };
            parameters[2] = new ObjectParameter(MiscellaneousDataImplConstants.FYear, typeof(int)) { Value = fYear };
            parameters[3] = new ObjectParameter(MiscellaneousDataImplConstants.FPeriod, typeof(int)) { Value = fPeriod};
            parameters[4] = new ObjectParameter(MiscellaneousDataImplConstants.Tmonth, typeof(int)) { Value = tMonth};
            parameters[5] = new ObjectParameter(MiscellaneousDataImplConstants.Tyear, typeof(int)) { Value = tYear};
            parameters[6] = new ObjectParameter(MiscellaneousDataImplConstants.Tperiod, typeof(int)) { Value = tPeriod};
            parameters[7] = new ObjectParameter(MiscellaneousDataImplConstants.BilledEntityId, typeof(int?)) { Value = billedEntityCode};
            parameters[8] = new ObjectParameter(MiscellaneousDataImplConstants.BillingEntityId, typeof(int?)) { Value = billingEntityCode};
            parameters[9] = new ObjectParameter(MiscellaneousDataImplConstants.SubmissionMethod, typeof(int?)) { Value = dataSource};
            parameters[10] = new ObjectParameter(MiscellaneousDataImplConstants.SettlementMethod, typeof(int?)) { Value = settlementMethod };
            parameters[11] = new ObjectParameter(MiscellaneousDataImplConstants.ChargeCategory, typeof(int?)) { Value = chargeCategory };
            parameters[12] = new ObjectParameter(MiscellaneousDataImplConstants.CurrencyCode, typeof(int?)) { Value = currencyCode };
            parameters[13] = new ObjectParameter(MiscellaneousDataImplConstants.InvoiceType, typeof(int?)) { Value = invoiceType };

            var list = ExecuteStoredFunction<InvoiceSummaryReportModel>(MiscellaneousDataImplConstants.GetInvoiceSummaryFunction, parameters);
            return list.ToList();
        }

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
            var parameters = new ObjectParameter[11];

            parameters[0] = new ObjectParameter(MiscellaneousDataImplConstants.FromYear, typeof(int)) { Value = fromYear };
            parameters[1] = new ObjectParameter(MiscellaneousDataImplConstants.FromMonth, typeof(int)) { Value = fromMonth };
            parameters[2] = new ObjectParameter(MiscellaneousDataImplConstants.FromPeriod, typeof(int)) { Value = fromPeriod };
            parameters[3] = new ObjectParameter(MiscellaneousDataImplConstants.ToYear, typeof(int)) { Value = toYear };
            parameters[4] = new ObjectParameter(MiscellaneousDataImplConstants.ToMonth, typeof(int)) { Value = toMonth };
            parameters[5] = new ObjectParameter(MiscellaneousDataImplConstants.ToPeriod, typeof(int)) { Value = toPeriod };
            parameters[6] = new ObjectParameter(MiscellaneousDataImplConstants.BillingEntityCode, typeof(int)) { Value = billingEntityCode };
            parameters[7] = new ObjectParameter(MiscellaneousDataImplConstants.BilledEntityCode, typeof(int?)) { Value = billedEntityCode };
            parameters[8] = new ObjectParameter(MiscellaneousDataImplConstants.ChargeCategoryMisc, typeof(int?)) { Value = chargeCategory };
            parameters[9] = new ObjectParameter(MiscellaneousDataImplConstants.ChargeCode, typeof(int?)) { Value = chargeCode };
            parameters[10] = new ObjectParameter(MiscellaneousDataImplConstants.InvoiceNumber, typeof(string)) { Value = invoiceNumber };

            var list = ExecuteStoredFunction<MiscSubstitutionValuesReportResult>(MiscellaneousDataImplConstants.GetMiscSubstitutionValuesReportFunction, parameters);
            return list.ToList();
        }
    }
}
