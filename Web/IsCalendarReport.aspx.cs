using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Reports.ValidationSummary;
using Iata.IS.Core.DI;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
//using Iata.IS.Web.Reports.AuditTrail;
using Iata.IS.Model.Reports;
using Iata.IS.Web.Util;
using iPayables.UserManagement;
using Iata.IS.Model.Base;
using Iata.IS.Business.Reports;
using System.Linq;
using Iata.IS.Web.Reports.ISCalender;
using Iata.IS.Data.Reports;
using log4net;

namespace Iata.IS.Web
{
  public partial class IsCalendarReport : System.Web.UI.Page
  {
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private readonly IManageIsCalendarRepository _manageCalendar;

    public IsCalendarReport()
    {
      _manageCalendar = Ioc.Resolve<IManageIsCalendarRepository>(typeof(IManageIsCalendarRepository));
    }
    protected void Page_Load(object sender, EventArgs e)
    {
      try
      {

        _logger.Info("IS Calendar Report Initiated");

        //Crystal report physical file path on server
          string reportPath = string.Empty;
        try
        {
            reportPath = MapPath("~/Reports/ISCalender/IsCalendar.rpt");
        }
        catch (Exception exception)
        {
            _logger.Error("Unexpected Error Has Occurred", exception);
        }
        Int32 calendarYear = -1;
        Int32 calendarType = 0;
        string timeZone = string.Empty;
        if (!String.IsNullOrEmpty(Request.QueryString["ClearanceYear"]))
          calendarYear = Convert.ToInt32(Request.QueryString["ClearanceYear"]);
        if (!String.IsNullOrEmpty(Request.QueryString["EventCategory"]))
          calendarType = Convert.ToInt32(Request.QueryString["EventCategory"]);
        if (!String.IsNullOrEmpty(Request.QueryString["TimeZone"]))
          timeZone = Convert.ToString(Request.QueryString["TimeZone"]);

        //Get Report data from Repository
        var calendarventDateList = Ioc.Resolve<IManageIsCalendarRepository>(typeof(IManageIsCalendarRepository)).GetIsClearingHouseCalendar(calendarYear, calendarType, timeZone);
        if (calendarventDateList.Count > 0)
        {
           
      
            // redirect to Login screen in case unauthorized/anonymous user access 
            // SIS_SCR_REPORT_23_jun-2016_2 :Cross_Site_History_Manipulation
            if (!User.Identity.IsAuthenticated)
            {
                HttpContext.Current.Response.Redirect(AdminSystem.SystemParameters.Instance.General.LogOnURL);
            }
          //Log in into logger
          _logger.Info("IsCalendarReport : Business Layer being called for Data!");

          //if (calendarType == 6)
          //{
          //  try
          //  {
          //      reportPath = MapPath("~/Reports/ISCalender/IsCalendarIchAch.rpt");
          //  }
          //  catch (Exception exception)
          //  {
          //      _logger.Error("Unexpected Error Has Occurred", exception);
          //  }
          //}
          ReportDocument orpt = new ReportDocument();
          //CRViewer.RefreshReport(); -- this line is commented because, it does not let SetParameterValue statement to set value and cr report ask again value
          //Parameter to display Report Generated date time in UTC format
          ParameterFields paramFields = new ParameterFields();
          ParameterField pfReportGeneratedDate = new ParameterField();
          pfReportGeneratedDate.ParameterFieldName = "ReportGeneratedDate"; //ReportGeneratedDate is Crystal Report Parameter name.
          ParameterDiscreteValue dcReportGeneratedDate = new ParameterDiscreteValue();
          //Set Value to parameter
          dcReportGeneratedDate.Value = string.Format("{0} {1}", DateTime.UtcNow.ToString("dd-MMM-yyyy hh:mm:ss tt"), "UTC");
          pfReportGeneratedDate.CurrentValues.Add(dcReportGeneratedDate);
          //Add parameter in ParameterFields
          paramFields.Add(pfReportGeneratedDate);
          //Assign ParameterFields to Crystal report viewer
          CRViewer.ParameterFieldInfo = paramFields;
          //Log in into logger
          _logger.Info("Cargo Supporting Mismatch Doc : Report(rpt) being loaded with data!");

          orpt.Load(reportPath);

          
          orpt.SetDataSource(calendarventDateList);

          // Take an format id of pdf and excel report 
          int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);
          // Only shows the padf and export to data option in report and supress other options
          CRViewer.AllowedExportFormats = exportFormatFlags;
          //changes to display search criteria on report
          orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
          orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
          CRViewer.ReportSource = orpt;
        }
        else
        {
          //if no record then show error message
          errorDiv.Visible = true;
        }
      }
      catch (Exception exception)
      {
        _logger.Error("Unexpected Error Has Occurred", exception);

      }

    }
  }
}
