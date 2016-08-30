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
  public partial class SupportingMismatchDoc : System.Web.UI.Page
  {
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


    protected void Page_Load(object sender, EventArgs e)
    {
      // fetch all values from the query strimg and stored into the variables
      int billingMonth = Convert.ToInt32(Request.QueryString["bmonth"]);
      int billingPeriod = Convert.ToInt32(Request.QueryString["bPeriod"]);
      int billingYear = Convert.ToInt32(Request.QueryString["bYear"]);
      int airlinceCode = Request.QueryString["AirCode"] == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["AirCode"]);
      int settlementMethod = Convert.ToInt32(Request.QueryString["SetelmentMethodId"]) == -1 ? 0 : Convert.ToInt32(Request.QueryString["SetelmentMethodId"]);
      string invoiceNo = Request.QueryString["InNo"];
      int logInMenberid = SessionUtil.MemberId;

      try
      {
        _logger.Info("Supporting Mismatch Doc Report Initiated");

        // Set rootpath of report
          string reportPath = string.Empty;
        try
        {
            reportPath = MapPath("~/Reports/MisMatch/SupportingMismatchDocReport.rpt");
        }
        catch (Exception exception)
        {
            _logger.Error("Unexpected Error Has Occurred", exception);
        }

        // Make an object of SupportingMismatchDoc model class
        List<Iata.IS.Model.Reports.SupportingMismatchDocument.SupportingMismatchDoc> listModel;

        //Log in into logger
        _logger.Info("SupportingMismatchDoc : Business Layer being called for Data!");

        // call the business method to get all the records from the db
        listModel = Ioc.Resolve<IMismatchDoc>(typeof(IMismatchDoc)).GetMismatchDoc(billingMonth, billingPeriod, billingYear, airlinceCode, settlementMethod, 1, invoiceNo, logInMenberid);

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
          }
          else if (listModel[i].Attachment == 2)
          {
            listModel[i].AttachmentIndicator = "P";
          }// End else If

          //if (listModel[i].SettlementMethod.Substring(0, 1) == "I" || listModel[i].SettlementMethod.Substring(0, 1) == "A" || listModel[i].SettlementMethod.Substring(0, 1) == "R" || listModel[i].SettlementMethod.Substring(0, 1) == "B")
          //{
          //  listModel[i].SettlementMethod = listModel[i].SettlementMethod.Substring(3);
          //}

          //if (listModel[i].SettlementMethod == "M")
          //{
          //  listModel[i].SettlementMethod = "M-ACH";
          //}
        }// End for

        // Add the data from list to data table
        DataTable dt = ListToDataTable(listModel);

        // make an object of report document
        ReportDocument orpt = new ReportDocument();

        //Log in into logger
        _logger.Info("SupportingMismatchDoc : Report(rpt) being loaded with data!");


        // add the path of the report
        orpt.Load(reportPath);
        // Add data source
        orpt.SetDataSource(dt);

        //changes to display search criteria on report
        orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
        orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
        // Take an format id of pdf and excel report 
        int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);


        // Only shows the padf and export to data option in report and supress other options
        CRSupportingMisMatch.AllowedExportFormats = exportFormatFlags;
        CRSupportingMisMatch.ReportSource = orpt;
        //Log in into logger
        _logger.Info("SupportingMismatchDocc : Report generation completed!");
      }
      catch (Exception exception)
      {
        _logger.Error("Unexpected Error Has Occurred", exception);
      }



    }// end page_load

    public static DataTable ListToDataTable(List<Iata.IS.Model.Reports.SupportingMismatchDocument.SupportingMismatchDoc> list)
    {
      DataTable dt = new DataTable();
      foreach (PropertyInfo info in typeof(Iata.IS.Model.Reports.SupportingMismatchDocument.SupportingMismatchDoc).GetProperties())
      {
        dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
      }
      foreach (Iata.IS.Model.Reports.SupportingMismatchDocument.SupportingMismatchDoc t in list)
      {
        DataRow row = dt.NewRow();
        foreach (PropertyInfo info in typeof(Iata.IS.Model.Reports.SupportingMismatchDocument.SupportingMismatchDoc).GetProperties())
        {
          row[info.Name] = info.GetValue(t, null);
        }

        dt.Rows.Add(row);
      }
      return dt;
    }
  }// end SupportingMismatchDoc class
}// end namespace

