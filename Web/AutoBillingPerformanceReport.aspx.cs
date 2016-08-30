using System;
using System.Collections.Generic;
using System.Reflection;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Pax.Impl;
using Iata.IS.Core.DI;
using Iata.IS.Web.Util;
using log4net;

namespace Iata.IS.Web
{
  public partial class AutoBillingPerformanceReport : System.Web.UI.Page
  {
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private readonly IInvoiceManager _invoiceManager = Ioc.Resolve<IInvoiceManager>(typeof(IInvoiceManager));


    protected void Page_Load(object sender, EventArgs e)
    {
      try
      {
        int clearanceYear = Convert.ToInt32(Request.QueryString["clearanceYear"]);
        int clearanceMonth = Convert.ToInt32(Request.QueryString["clearanceMonth"]);
        string entityIdString = Convert.ToString(Request.QueryString["entityId"]);
        int currencyCode = Convert.ToInt32(Request.QueryString["currencyCode"]);
        int logInMemberid = SessionUtil.MemberId;


        List<string> reportColumnHeader = new List<string>();
        DateTime date = new DateTime();

        int entityId;
        if (string.IsNullOrEmpty(entityIdString))
        {
          entityId = -1;
        }
        else
        {
          entityId = Convert.ToInt32(entityIdString);
        }

          string reportPath = string.Empty;
        try
        {
            reportPath = MapPath("Reports/AutoBilling/PerformanceReport/PerformanceReport.rpt");
        }
        catch (Exception exception)
        {
            _logger.Error("Unexpected Error Has Occurred", exception);
        }

        var reportDocument = new ReportDocument();
        var performanceReportData = _invoiceManager.GetAutoBillingPerformanceReportSearchResult(logInMemberid,entityId, currencyCode,
                                                                                       clearanceMonth, clearanceYear);

        reportDocument.Load(reportPath);



        reportDocument.SetDataSource(performanceReportData);

        // get report columnheader values
        date = new DateTime(clearanceYear, clearanceMonth, 1);
        for (int i = 0; i < 12; i++)
        {
            reportColumnHeader.Add(String.Format("{0:y}", date.AddMonths(-i)));
        }
        // Add report columnheader values
        reportDocument.SetParameterValue("@clrMonth1", reportColumnHeader[0]);
        reportDocument.SetParameterValue("@clrMonth2", reportColumnHeader[1]);
        reportDocument.SetParameterValue("@clrMonth3", reportColumnHeader[2]);
        reportDocument.SetParameterValue("@clrMonth4", reportColumnHeader[3]);
        reportDocument.SetParameterValue("@clrMonth5", reportColumnHeader[4]);
        reportDocument.SetParameterValue("@clrMonth6", reportColumnHeader[5]);

        //changes to display search criteria on report
        reportDocument.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
        reportDocument.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]); 
        const int exportFormatFlags = (int)(ViewerExportFormats.PdfFormat | ViewerExportFormats.ExcelRecordFormat);

        AutoBillingPerformanceReportViewer.AllowedExportFormats = exportFormatFlags;
        ExcelDataOnlyFormatOptions optionsformat = CrystalDecisions.Shared.ExportOptions.CreateDataOnlyExcelFormatOptions();
        optionsformat.MaintainColumnAlignment = true;
        optionsformat.MaintainRelativeObjectPosition = true;
        optionsformat.ShowGroupOutlines = true;
        optionsformat.ExportObjectFormatting = true;
        optionsformat.ExportPageHeaderAndPageFooter = true;
        optionsformat.SimplifyPageHeaders = true;
        optionsformat.ExcelConstantColumnWidth = 1000;

        reportDocument.ExportOptions.FormatOptions = optionsformat;
        AutoBillingPerformanceReportViewer.ReportSource = reportDocument;

      }
      catch (Exception)
      {

      }
    }
  }
}