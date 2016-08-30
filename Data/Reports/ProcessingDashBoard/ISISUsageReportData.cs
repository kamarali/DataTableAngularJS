using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.ExternalInterfaces.IATAInterface;

namespace Iata.IS.Data.Reports.ProcessingDashBoard
{
  public  interface ISISUsageReportData : IRepository<InvoiceBase>
    {
        /* SCP 263438: Usage report producing incomplete results for YH-361
           Description: Usage report fetch logic is changed to consider member id instead of member code and designator.
        */
      List<RechargeData> GetSISUsageReport(DateTime fromDate, DateTime toDate, int memberId, int ParticipantType);


      /// <summary>
      /// This function is used to get sis is-web usage report.
      /// </summary>
      /// <param name="fromDate"></param>
      /// <param name="toDate"></param>
      /// <param name="memberId"></param>
      /// <returns></returns>
      /// CMP #659: SIS IS-WEB Usage Report.
      List<SisIsWebUsageRptData> GetSisIsWebUsageReport(DateTime fromDate, DateTime toDate, int memberId);
    }
}
