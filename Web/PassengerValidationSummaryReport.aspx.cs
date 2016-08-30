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
using Iata.IS.Model.Reports.ValidationSummary;
using Iata.IS.Web.Util;
using iPayables.UserManagement;
using log4net;
using UserCategory = Iata.IS.Model.MemberProfile.Enums.UserCategory;

namespace Iata.IS.Web
{
  public partial class PassengerValidationSummaryReport : System.Web.UI.Page
  {

    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public readonly IMemberManager _imemberManager;
    public PassengerValidationSummaryReport()
    {
      _imemberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      // redirect to Login screen in case unauthorized/anonymous user access 
      // SIS_SCR_REPORT_23_jun-2016_2 :Cross_Site_History_Manipulation
      if (!User.Identity.IsAuthenticated)
      {
          HttpContext.Current.Response.Redirect(AdminSystem.SystemParameters.Instance.General.LogOnURL);
      }


        try
      {
        _logger.Info("Passenger Validation Summary Report Initiated");

        List<PassengerValidationSummary> listModel = null;

        //Log in into logger    


        Int32 BillingPeriod;
        DateTime BillingSubmissiondate;
        string fileName;
        string reportPath = string.Empty;
        string defaultdatetime = "01-01-0001";

        BillingPeriod = Convert.ToInt32(Request.QueryString["fdate"]);

        BillingSubmissiondate = (Request.QueryString["fSubmissionDate"]) == string.Empty ? Convert.ToDateTime(defaultdatetime) : Convert.ToDateTime(Request.QueryString["fSubmissionDate"]);

        string category = Request.QueryString["category"];

        int ClearanceMonth = Convert.ToInt32((Request.QueryString["tbmonth"]) + (Request.QueryString["fbmonth"]));

        if (string.IsNullOrEmpty(Request.QueryString["fname"]))
        {
          fileName = string.Empty;
        }
        else
        {
          fileName = Request.QueryString["fname"];
        }

        // SIS_SCR_REPORT_23_jun-2016_2 :Cross_Site_History_Manipulation
        if (!User.Identity.IsAuthenticated)
        {
          HttpContext.Current.Response.Redirect(AdminSystem.SystemParameters.Instance.General.LogOnURL);
        }

        string memberCode = _imemberManager.GetMemberCode(SessionUtil.MemberId);
        try
        {
            reportPath = MapPath("~/Reports/ValidationSummary/ValidationSummary.rpt");
        }
        catch (Exception exception)
        {
            _logger.Error("Unexpected Error Has Occurred", exception);
        }


        int memberId = SessionUtil.MemberId;


        //Log in into logger
        _logger.Info("PassengerValidationSummaryReport : Business Layer being called for Data!");

        listModel = Ioc.Resolve<IValidationSummary>(typeof(IValidationSummary)).GetPaxValidationSummary(ClearanceMonth, BillingPeriod, fileName, BillingSubmissiondate, memberCode, category);

        int index = 1;
        for (int i = 0; i < listModel.Count; i++)
        {
          listModel[i].SerialNo = index;
          if (listModel[i].ErrorAtInvoiceLevel == 0)
          {
            listModel[i].ErrorLevel = "N";
          }
          else
          {
            listModel[i].ErrorLevel = "Y";
          }

          string ClearenceYear = Convert.ToString(listModel[i].ClearenceMonth).Substring(0, 4);
          string ClearenceMonth = Convert.ToString(listModel[i].ClearenceMonth).Substring(4);

          listModel[i].Clearance = Convert.ToDateTime(ClearenceYear + "-" + ClearenceMonth);


          if (listModel[i].FileSubmissionDate == Convert.ToDateTime("01/01/0001"))
          {
            listModel[i].FileSubmissionDate = Convert.ToDateTime("12/30/1899");
          }
          index++;
        }

        DataTable dt = ListToDataTable(listModel);

        ReportDocument orpt = new ReportDocument();


        //Log in into logger
        _logger.Info("PassengerValidationSummaryReport : Report(rpt) being loaded with data!");

        orpt.Load(reportPath);
        orpt.SetDataSource(dt);

        Iata.IS.Web.Reports.ValidationSummary.ValidationSummary rpt = new Iata.IS.Web.Reports.ValidationSummary.ValidationSummary();

        // Take an format id of pdf and excel report 
        int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

        // Only shows the padf and export to data option in report and supress other options
        CRViewer.AllowedExportFormats = exportFormatFlags;

        CRViewer.ReportSource = orpt;

        //Log in into logger
        _logger.Info("PassengerValidationSummaryReport : Report generation completed!");
      }
      catch (Exception exception)
      {
        _logger.Error("Unexpected Error Has Occurred", exception);
      }

    }
    public static DataTable ListToDataTable(List<PassengerValidationSummary> list)
    {
      DataTable dt = new DataTable();
      foreach (PropertyInfo info in typeof(PassengerValidationSummary).GetProperties())
      {
        dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
      }
      foreach (PassengerValidationSummary t in list)
      {
        DataRow row = dt.NewRow();
        foreach (PropertyInfo info in typeof(PassengerValidationSummary).GetProperties())
        {
          row[info.Name] = info.GetValue(t, null);
        }

        dt.Rows.Add(row);
      }
      return dt;
    }
  }
}