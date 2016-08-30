using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Iata.IS.Business.Reports.MisMatchDoc;
using Iata.IS.Core.DI;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;
using log4net;

namespace Iata.IS.Web
{
  public partial class MiscSupportingMismatchDoc : System.Web.UI.Page
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    protected void Page_Load(object sender, EventArgs e)
    {
      //Log it into logger
      Logger.Info("Misc Supporting Document Mismatch : aspx page Being Load!");

      try
      {
        // fetch all values from the query strimg and stored into the variables
        int billingMonth = Convert.ToInt32(Request.QueryString["bmonth"]);
        int billingPeriod = Convert.ToInt32(Request.QueryString["bPeriod"]);
        int billingYear = Convert.ToInt32(Request.QueryString["bYear"]);
        int airlineCode = Request.QueryString["AirCode"] == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["AirCode"]);
        int settlementMethod = Convert.ToInt32(Request.QueryString["SetelmentMethodId"]) == -1 ? 0 : Convert.ToInt32(Request.QueryString["SetelmentMethodId"]);
        string invoiceNo = Request.QueryString["InNo"];
        int logInMemberid = SessionUtil.MemberId;

        // Set the string for report type
        string reportPath = "";

        // Check for the category ; i.e. misc, passenger or cargo
        string category = Request.QueryString["category"];

        // Set rootpath of report
        try
        {
          reportPath = MapPath("~/Reports/MisMatch/MiscSupportingMismatchDocReport.rpt");
        }
        catch (Exception exception)
        {
          Logger.Error("Unexpected Error Has Occurred", exception);
        }
        // Make an object of MiscSupportingMismatchDoc model class
        List<Model.Reports.SupportingMismatchDocument.MiscSupportingMismatchDoc> listModel;

        //Log it into logger
        Logger.Info("Misc Supporting Document Mismatch : Business Layer being called for Data!");

        // Call the business method to get all the records from the db
        listModel = Ioc.Resolve<IMismatchDoc>(typeof(IMismatchDoc)).GetMiscMismatchDoc(billingMonth, billingPeriod, billingYear, airlineCode, settlementMethod, 3, invoiceNo, logInMemberid);


        //Log it into logger
        Logger.Info("Misc Supporting Document Mismatch : Business Layer returns Data!");

        //Iterate through the list and set attachment indicator
        for (int i = 0; i < listModel.Count; i++)
        {
          if (listModel[i].Attachment == 0)
          {
            listModel[i].AttachmentIndicator = "N";
          }// End if
          else if (listModel[i].Attachment == 1)
          {
            listModel[i].AttachmentIndicator = "Y";
          }// End else If

        }// End for

        // Add the data from list to data table
        DataTable dt = ListToDataTable(listModel);

        // Make an object of report document
        ReportDocument orpt = new ReportDocument();

        //Log it into logger
        Logger.Info("Misc Supporting Document Mismatch : Report(rpt) being loaded!");

        // Add the path of the report
        orpt.Load(reportPath);

        //Log it into logger
        Logger.Info("Misc Supporting Document Mismatch : Report(rpt) being loaded with data!");
        // Add data source
        orpt.SetDataSource(dt);

        //Changes to display search criteria on report
        orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
        orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
        // Take a format id of pdf and excel report 
        int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

        // Only shows the pdf and export to data option in report and suppress other options
        CRSupportingMisMatch.AllowedExportFormats = exportFormatFlags;

        CRSupportingMisMatch.ReportSource = orpt;

        //Log it into logger
        Logger.Info("Misc Supporting Document Mismatch : Report generation completed!");
      }
      catch (Exception ex)
      {

        //Log it into logger
        Logger.Error("Misc Supporting Document Mismatch :Error--->", ex);
      }

    } // end of page load

    public static DataTable ListToDataTable(List<Iata.IS.Model.Reports.SupportingMismatchDocument.MiscSupportingMismatchDoc> list)
    {
      DataTable dt = new DataTable();
      foreach (PropertyInfo info in typeof(Iata.IS.Model.Reports.SupportingMismatchDocument.MiscSupportingMismatchDoc).GetProperties())
      {
        dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
      }
      foreach (Iata.IS.Model.Reports.SupportingMismatchDocument.MiscSupportingMismatchDoc t in list)
      {
        DataRow row = dt.NewRow();
        foreach (PropertyInfo info in typeof(Iata.IS.Model.Reports.SupportingMismatchDocument.MiscSupportingMismatchDoc).GetProperties())
        {
          row[info.Name] = info.GetValue(t, null);
        }

        dt.Rows.Add(row);
      }
      return dt;
    }
  }// end MiscSupportingMismatchDoc class
}