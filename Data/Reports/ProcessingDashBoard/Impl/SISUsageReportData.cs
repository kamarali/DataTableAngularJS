using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.ExternalInterfaces.IATAInterface;
using Iata.IS.Data.Impl;
using System.Data.Objects;

namespace Iata.IS.Data.Reports.ProcessingDashBoard.Impl
{
    public class SISUsageReportData : Repository<InvoiceBase> , ISISUsageReportData
    {
        public List<RechargeData> GetSISUsageReport(DateTime fromDate, DateTime toDate, int memberId, int ParticipantType)
        {
            var parameters = new ObjectParameter[5];

            parameters[0] = new ObjectParameter(SISUsageReportDataConstants.FromDate, fromDate);
            parameters[1] = new ObjectParameter(SISUsageReportDataConstants.ToDate, toDate);
            parameters[2] = new ObjectParameter(SISUsageReportDataConstants.MemberId, memberId);
            parameters[3] = new ObjectParameter(SISUsageReportDataConstants.PartcipantId, ParticipantType);
            parameters[4] = new ObjectParameter(SISUsageReportDataConstants.IsReportData , 1);

            var listItem = ExecuteStoredFunction<RechargeData>(SISUsageReportDataConstants.GetSisUsagereport, parameters);

            return listItem.ToList();
        }

        /// <summary>
        /// This function is used to get sis is-web usage report.
        /// </summary>
        /// <param name="fromBillingPeriod"></param>
        /// <param name="toBillingPeriod"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        /// CMP #659: SIS IS-WEB Usage Report.
        public List<SisIsWebUsageRptData> GetSisIsWebUsageReport(DateTime fromBillingPeriod, DateTime toBillingPeriod, int memberId)
        {
            var parameters = new ObjectParameter[3];

            parameters[0] = new ObjectParameter(SISUsageReportDataConstants.FromBillingPeriodInputParam, fromBillingPeriod);
            parameters[1] = new ObjectParameter(SISUsageReportDataConstants.ToBillingPeriodInputParam, toBillingPeriod);
            parameters[2] = new ObjectParameter(SISUsageReportDataConstants.MemberIdInputParam, memberId);

            var listItem = ExecuteStoredFunction<SisIsWebUsageRptData>(SISUsageReportDataConstants.GetSisIsWebUsagereport, parameters);

            return listItem.ToList();
        }
    }
}
