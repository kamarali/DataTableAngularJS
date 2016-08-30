using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Model.ExternalInterfaces.IATAInterface;
using Iata.IS.Business.Reports.ProcessingDashBoard;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Web.Util;
using Iata.IS.Core.DI;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using System.Reflection;
using CrystalDecisions.Shared;
using log4net;

namespace Iata.IS.Web
{
  public partial class SISUsageReport : System.Web.UI.Page
  {
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    protected void Page_Load(object sender, EventArgs e)
    {
      try
      {
        //Log in into logger 	
        _logger.Info("Report Initiated");
        int membeId;

      
            // redirect to Login screen in case unauthorized/anonymous user access 
           // SIS_SCR_REPORT_23_jun-2016_2 :Cross_Site_History_Manipulation
           if (!User.Identity.IsAuthenticated)
            {
                HttpContext.Current.Response.Redirect(AdminSystem.SystemParameters.Instance.General.LogOnURL);
            }

            if (SessionUtil.UserCategory == UserCategory.SisOps)
            {
              membeId = (Request.QueryString["memberID"]) == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["memberID"]);
            }
            else
            {
              membeId = SessionUtil.MemberId;
            }

        int fromperiod;
        int toPeriod;
              
        fromperiod = Convert.ToInt32(Request.QueryString["fperiod"]);
        toPeriod = Convert.ToInt32(Request.QueryString["tperiod"]);


        DateTime fromdate = new DateTime(Convert.ToInt32(Request.QueryString["fyear"]), Convert.ToInt32(Request.QueryString["fmonth"]), fromperiod);
        DateTime todate = new DateTime(Convert.ToInt32(Request.QueryString["tyear"]), Convert.ToInt32(Request.QueryString["tmonth"]), toPeriod);

        if (SessionUtil.UserCategory == UserCategory.SisOps)
        {
          membeId = (Request.QueryString["memberID"]) == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["memberID"]);
        }
        else
        {
          membeId = SessionUtil.MemberId;
        }

        int ParticipantType = (Request.QueryString["ParticipantType"]) == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["ParticipantType"]);


        List<RechargeData> rechargeData = new List<RechargeData>();


        //Log in into logger
        _logger.Info("SISUsageReport : Business Layer being called for Data!");


        /* SCP 263438: Usage report producing incomplete results for YH-361
           Description: Usage report fetch logic is changed to consider member id instead of member code and designator. */

        rechargeData = Ioc.Resolve<ISISUsageRecharge>(typeof(ISISUsageRecharge)).GetSISUsageReport(fromdate, todate, membeId, ParticipantType);


        string reportPath = string.Empty;
        try
        {
            if (SessionUtil.UserCategory == UserCategory.SisOps)
            {
                reportPath = MapPath("~/Reports/ProcessingDashBoard/SISUsageReportOPS.rpt");
            }
            else
            {
                reportPath = MapPath("~/Reports/ProcessingDashBoard/SISUsageReport.rpt");
            }
        }
        catch (Exception exception)
        {
            _logger.Error("Unexpected Error Has Occurred", exception);
        }

        for (int i = 0; i < rechargeData.Count; i++)
        {
          rechargeData[i].MemberCode = rechargeData[i].AlphaCode + "-" + rechargeData[i].NumericCode;
        }
        DataTable dt = ListToDataTable(rechargeData);

        ReportDocument orpt = new ReportDocument();


        //Log in into logger
        _logger.Info("SISUsageReport : Report(rpt) being loaded with data!");

        orpt.Load(reportPath);
        orpt.SetDataSource(dt);








        // Take an format id of pdf and excel report 
        int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);




        // Only shows the padf and export to data option in report and supress other options
        CRSISUsageReport.AllowedExportFormats = exportFormatFlags;


        CRSISUsageReport.ReportSource = orpt;


        //Log in into logger
        _logger.Info("SISUsageReport : Report generation completed!");
      }
      catch (Exception exception)
      {
        _logger.Error("Unexpected Error Has Occurred", exception);
      }


    }

    public static DataTable ListToDataTable(List<RechargeData> list)
    {
      DataTable dt = new DataTable();
      foreach (PropertyInfo info in typeof(RechargeData).GetProperties())
      {
        dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
      }
      foreach (RechargeData t in list)
      {
        DataRow row = dt.NewRow();
        foreach (PropertyInfo info in typeof(RechargeData).GetProperties())
        {
          row[info.Name] = info.GetValue(t, null);
        }

        dt.Rows.Add(row);
      }
      return dt;
    }
  }
}
