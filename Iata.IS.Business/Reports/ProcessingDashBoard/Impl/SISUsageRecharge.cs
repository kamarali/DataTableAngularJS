using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.Reports.OfflineReportManger;
using Iata.IS.Business.Reports.OfflineReports.Imp;
using Iata.IS.Core;
using Iata.IS.Core.File;
using Iata.IS.Data.Reports.ProcessingDashBoard;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.ExternalInterfaces.IATAInterface;
using Iata.IS.Model.Reports.ProcessingDashBoard;
using log4net;

namespace Iata.IS.Business.Reports.ProcessingDashBoard.Impl
{
  public class SISUsageRecharge : OfflineReportBase, ISISUsageRecharge
  {
    private ISISUsageReportData SISUsageReportParam { get; set; }
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private IOfflineReportLogManager _offlineReportLogManager;

    public SISUsageRecharge(ISISUsageReportData validationDetailsData, IOfflineReportLogManager offlineReportLogManager)
    {
      SISUsageReportParam = validationDetailsData;
      _offlineReportLogManager = offlineReportLogManager;
    }

    /* SCP 263438: Usage report producing incomplete results for YH-361
       Description: Usage report fetch logic is changed to consider member id instead of member code and designator.
    */
    public List<RechargeData> GetSISUsageReport(DateTime fromDate, DateTime toDate, int memberId, int participantType)
    {
        return SISUsageReportParam.GetSISUsageReport(fromDate, toDate, memberId, participantType);
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
        return SISUsageReportParam.GetSisIsWebUsageReport(fromBillingPeriod, toBillingPeriod, memberId);
    }

    /// <summary>
    /// This function is used to generate Sis Is-Web Usage Report.
    /// </summary>
    /// <param name="requestMessage"></param>
    /// CMP #659: SIS IS-WEB Usage Report.
    public void GenerateSisIsWebUsageReport(ReportDownloadRequestMessage requestMessage)
    {
        //Deserialize input object and cast based on model.
        var sisIsWebUsageModel =
            (SisUsageReportModel)ConvertUtil.DeSerializeXml(requestMessage.InputData, typeof(SisUsageReportModel));

        Logger.InfoFormat("Generating SIS IS-WEB Usage report for Transaction Id : {0}", requestMessage.RecordId);

        //Get FTP path.
        string memberFtpPath = FileIo.GetForlderPath(SFRFolderPath.SisIsWebUsageReport);

        //Create ftp member path if not exist.
        if (!Directory.Exists(memberFtpPath))
            Directory.CreateDirectory(memberFtpPath);

        //Report File name with path
        var reportFileName = Path.Combine(memberFtpPath, requestMessage.RecordId.ToString());

        Logger.InfoFormat(" SIS IS-WEB Usage report file path: '{0}'", reportFileName);
        var fromBillingPeriod =
            DateTime.ParseExact(sisIsWebUsageModel.FromBillingYear + "-" + sisIsWebUsageModel.FromBillingMonth + "-" +
                                sisIsWebUsageModel.FromPeriod, "yyyy-M-d", CultureInfo.CurrentCulture);


        var toBillingPeriod =
            DateTime.ParseExact(sisIsWebUsageModel.ToBillingYear + "-" + sisIsWebUsageModel.ToBillingMonth + "-" +
                                sisIsWebUsageModel.ToPeriod, "yyyy-M-d", CultureInfo.CurrentCulture);

        Logger.InfoFormat(
            "Fetching data from database for SIS IS-WEB Usage report with search criteria, fromBillingPeriod: {0}, toBillingPeriod: {1}, Member Id: {2} ",
            fromBillingPeriod, toBillingPeriod, requestMessage.RequestingMemberId);

        //Get Data from Database.
        var sisIsWebUsageRptData = GetSisIsWebUsageReport(fromBillingPeriod, toBillingPeriod, requestMessage.RequestingMemberId);

        Logger.InfoFormat("Fetched data from database for SIS IS-WEB Usage report, Count: {0}",
                          sisIsWebUsageRptData.Count);

        // Generate CSV Report for given data.
        CsvProcessor.GenerateCsvReport(sisIsWebUsageRptData, reportFileName + ".csv", new List<SpecialRecord>(),
                                       includesearchCriteria: false, isOfflineReport: true);

        //Zip CSV file 
        if (File.Exists(reportFileName + ".csv"))
            FileIo.ZipOutputFile(reportFileName + ".csv");

        Logger.InfoFormat("Sending email to user");

        //Send mail to user.
        SendEmail(sisIsWebUsageModel.OfflineReportLogId, requestMessage.UserId, reportFileName + ".ZIP", requestMessage.DownloadUrl,
                  reportFileName,
                  _offlineReportLogManager.GetOfflineReportName((int)OfflineReportType.SisIsWebUsageReports),
                  sisIsWebUsageModel.SearchCriteria);

        Logger.InfoFormat("Finished execution of SIS IS-WEB Usage report");
    }
  }
}
