using System;
using System.Collections.Generic;
using Iata.IS.Business.Reports.ReceivablesReport;
using Iata.IS.Core.DI;
using Iata.IS.Web.Util;
using Iata.IS.Model.Reports.ReceivablesReport;
using System.Data;
using System.Reflection;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using log4net;

namespace Iata.IS.Web
{
    public partial class PaxInterlineBillingSummaryReport : System.Web.UI.Page
    {
      private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

      protected void Page_Load(object sender, EventArgs e)
      {
        try
        {
          //Log in into logger 	
          _logger.Info("Report Initiated");

          //Read the values from QuerySting into local variables
          int billingType = Convert.ToInt32(Request.QueryString["qsBillingType"]);
          int month = Convert.ToInt32(Request.QueryString["qsMonth"]);
          int year = Convert.ToInt32(Request.QueryString["qsYear"]);
          int? period = (Request.QueryString["qsPeriod"]) == "-1" ? (int?)null: Convert.ToInt32(Request.QueryString["qsPeriod"]);
          string currencyText = Request.QueryString["qsCurrencyName"];
          string smiText = Request.QueryString["qsSettlementMethod"];
          string billedmember = string.Empty;
          int? billingEntityCode;
          int? billedEntityCode;

          if (billingType == 1)
          {
              billedEntityCode = SessionUtil.MemberId;
              if ((Request.QueryString["qsBilledEntityCode"]) == string.Empty)
              {
                  billingEntityCode = null;
              }
              else
              {
                  billingEntityCode = Convert.ToInt32(Request.QueryString["qsBilledEntityCode"]);
                  billedmember = Request.QueryString["qsBilledMember"];
              }
          }
          else
          {
              billingEntityCode = SessionUtil.MemberId;
              if ((Request.QueryString["qsBilledEntityCode"]) == string.Empty)
              {
                  billedEntityCode = null;
              }
              else
              {
                  billedEntityCode = Convert.ToInt32(Request.QueryString["qsBilledEntityCode"]);
                  billedmember = Request.QueryString["qsBilledMember"];
              }
          }

          int? settlementMethodId = (Request.QueryString["qsSettlementMethodId"]) == "-1" ? (int?) null: Convert.ToInt32(Request.QueryString["qsSettlementMethodId"]);
          int currencyId = Convert.ToInt32(Request.QueryString["qsCurrencyId"]);

          //Log in into logger
          _logger.Info("PaxInterlineBillingSummaryReport : Business Layer being called for Data!");

          List<PaxInterlineBillingSummaryReportResult> corrStatusModel =
                          Ioc.Resolve<IReceivablesReport>(typeof(IReceivablesReport)).GetPaxInterlineBillingSummaryReportDetails(billingType,
                                                                                                                                  month,
                                                                                                                                  year,
                                                                                                                                  period,
                                                                                                                                  billingEntityCode,
                                                                                                                                  billedEntityCode,
                                                                                                                                  settlementMethodId,
                                                                                                                                  currencyId);
          // Take value from list to data table
          DataTable dt = ListToDataTable(corrStatusModel);

          // Set the string for report type
          string reportPath = "";
          try
          {
              reportPath = MapPath("~/Reports/ReceivablesReport/PaxInterlineBillingSummaryReport.rpt");
          }
          catch (Exception exception)
          {
              _logger.Error("Unexpected Error Has Occurred", exception);
          }
          // Set ReportHeader
          string reportHeaderfield = "";
          if (billingType == 1)
          {
              reportHeaderfield = "Payables - Passenger Interline Billing Summary Report";
          }
          else if (billingType == 2)
          {
              reportHeaderfield = "Receivables - Passenger Interline Billing Summary Report";
          }

          string periodParameter = period == null ? "All" : period.ToString();

          // Create an object of report document
            ReportDocument orpt = new ReportDocument();

            //Log in into logger
            _logger.Info("PaxInterlineBillingSummaryReport : Report(rpt) being loaded with data!");

            // Load report to that report document
            orpt.Load(reportPath);

            // Added data table to report document
            orpt.SetDataSource(dt);

            // Add report columnheader values
            orpt.SetParameterValue("BillingTypeField", reportHeaderfield);

            //change to display search criteria on report
            orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
            orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]); 
            // Take an format id of pdf and excel report 
            int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

            // Only shows the padf and export to data option in report and supress other options
            PassengerInterlineBillingSummaryReport.AllowedExportFormats = exportFormatFlags;
            ExcelDataOnlyFormatOptions optionsformat = CrystalDecisions.Shared.ExportOptions.CreateDataOnlyExcelFormatOptions();
            optionsformat.MaintainColumnAlignment = true;
            optionsformat.MaintainRelativeObjectPosition = true;
            optionsformat.ShowGroupOutlines = true;
            optionsformat.ExportObjectFormatting = true;
            optionsformat.ExportPageHeaderAndPageFooter = true;
            optionsformat.SimplifyPageHeaders = true;
            optionsformat.ExcelConstantColumnWidth = 1000;

            orpt.ExportOptions.FormatOptions = optionsformat;

            // set report source of correspondencereport
            PassengerInterlineBillingSummaryReport.ReportSource = orpt;

            //Log in into logger
            _logger.Info("PaxInterlineBillingSummaryReport : Report generation completed!");
        }
        catch (Exception exception)
        {
            _logger.Error("Unexpected Error Has Occurred", exception);
        }
            
      }

      /// <summary>
      /// This method is used to add data to table
      /// </summary>
      /// <param name="list"></param>
      /// <returns></returns>
      public static DataTable ListToDataTable(List<PaxInterlineBillingSummaryReportResult> list)
      {
          DataTable dt = new DataTable();
          foreach (PropertyInfo info in typeof(PaxInterlineBillingSummaryReportResult).GetProperties())
          {
              dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
          }
          foreach (PaxInterlineBillingSummaryReportResult t in list)
          {
              DataRow row = dt.NewRow();
              foreach (PropertyInfo info in typeof(PaxInterlineBillingSummaryReportResult).GetProperties())
              {
                  row[info.Name] = info.GetValue(t, null);
              }

              dt.Rows.Add(row);
          }
          return dt;
      }// End ListToDataTable()
    }
}