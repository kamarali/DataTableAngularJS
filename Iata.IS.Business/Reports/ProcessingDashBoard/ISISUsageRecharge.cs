using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.ExternalInterfaces.IATAInterface;

namespace Iata.IS.Business.Reports.ProcessingDashBoard
{
  public interface ISISUsageRecharge
  {
      /* SCP 263438: Usage report producing incomplete results for YH-361
         Description: Usage report fetch logic is changed to consider member id instead of member code and designator.
      */
      List<RechargeData> GetSISUsageReport(DateTime fromDate, DateTime toDate, int memberId, int participantType);

      /// <summary>
      /// This function is used to get sis is-web usage report.
      /// </summary>
      /// <param name="fromBillingPeriod"></param>
      /// <param name="toBillingPeriod"></param>
      /// <param name="memberId"></param>
      /// <returns></returns>
      /// CMP #659: SIS IS-WEB Usage Report.
      List<SisIsWebUsageRptData> GetSisIsWebUsageReport(DateTime fromBillingPeriod, DateTime toBillingPeriod, int memberId);

      //CMP #659: SIS IS-WEB Usage Report.
      void GenerateSisIsWebUsageReport(ReportDownloadRequestMessage requestMessage);
  }
}
