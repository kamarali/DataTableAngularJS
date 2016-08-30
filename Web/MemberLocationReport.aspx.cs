using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core.DI;
using Iata.IS.Web.Util;
using Iata.IS.Model.Reports.MemberLocations;
using Iata.IS.Business.Reports.MemberLocations;
using System.Data;
using System.Reflection;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using log4net;

namespace Iata.IS.Web
{

  public partial class MemberLocationReport : System.Web.UI.Page
  {
    public readonly IMemberLocation _memberLocation;
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public MemberLocationReport()
    {
      _memberLocation = Ioc.Resolve<IMemberLocation>(typeof(IMemberLocation));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      List<MemberLocationModel> listModel = null;
      int memberID;
      string locationID;
      string reportPath = string.Empty;
      try
      {

        _logger.Info("Member Location Report Initiated");


        if (string.IsNullOrEmpty(Request.QueryString["memId"]))
        {
          memberID = 0;
        }
        else
        {
          memberID = Convert.ToInt32(Request.QueryString["memId"]);
        }
        if (string.IsNullOrEmpty(Request.QueryString["locId"]))
        {
          locationID = string.Empty;
        }
        else
        {
          locationID = Request.QueryString["locId"];
        }

     //Log in into logger
        _logger.Info("MemberLocationReport : Business Layer being called for Data!");

        // This returns list of future updates Member Locations for passed search criteria
        listModel = _memberLocation.GetMemberLocationData(memberID, locationID);
        int index = 1;
        for (int i = 0; i < listModel.Count; i++)
        {
          listModel[i].SerialNo = index;

          index++;
        }

        try
        {
            reportPath = MapPath("~/Reports/MemberLocations/MemberLocation.rpt");
        }
        catch (Exception exception)
        {
            _logger.Error("Unexpected Error Has Occurred", exception);
        }
        DataTable dt = ListToDataTable(listModel);
        ReportDocument orpt = new ReportDocument();

 
        //Log in into logger
        _logger.Info("MemberLocationReport : Report(rpt) being loaded with data!");

        orpt.Load(reportPath);

        orpt.SetDataSource(dt);
        //changes to display search criteria on report
        orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
        orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
        // SCP#398460: IsWeb Report “Invoice Reference Data Update”, XLS export inconsistent
        // Take an format id of PDF and excel report 
        const int exportFormatFlags = (int)(ViewerExportFormats.PdfFormat | ViewerExportFormats.ExcelRecordFormat);

        // Only shows the PDF and export to data option in report and suppress other options
        CrystalReportViewer.AllowedExportFormats = exportFormatFlags;
        var optionsformat = ExportOptions.CreateDataOnlyExcelFormatOptions();
        optionsformat.MaintainColumnAlignment = true;
        optionsformat.MaintainRelativeObjectPosition = true;
        optionsformat.ShowGroupOutlines = true;
        optionsformat.ExportObjectFormatting = true;
        optionsformat.ExportPageHeaderAndPageFooter = true;
        optionsformat.SimplifyPageHeaders = true;
        optionsformat.ExcelConstantColumnWidth = 1000;

        orpt.ExportOptions.FormatOptions = optionsformat;
        CrystalReportViewer.ReportSource = orpt;

        //Log in into logger
        _logger.Info("MemberLocationReport : Report generation completed!");
      }
      catch (Exception exception)
      {
        _logger.Error("Unexpected Error Has Occurred", exception);
      }

    }
    public static DataTable ListToDataTable(List<MemberLocationModel> list)
    {
      DataTable dt = new DataTable();
      foreach (PropertyInfo info in typeof(MemberLocationModel).GetProperties())
      {
        dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
      }
      foreach (MemberLocationModel t in list)
      {
        DataRow row = dt.NewRow();
        foreach (PropertyInfo info in typeof(MemberLocationModel).GetProperties())
        {
          row[info.Name] = info.GetValue(t, null);
        }

        dt.Rows.Add(row);
      }
      return dt;
    }
  }
}
