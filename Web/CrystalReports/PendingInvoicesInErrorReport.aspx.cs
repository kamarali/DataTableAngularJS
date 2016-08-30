using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Iata.IS.Business.Reports.PendingInvoicesInErrorReport;
using Iata.IS.Core.DI;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using Iata.IS.Web.Util;
using System.Reflection;

namespace Iata.IS.Web.CrystalReports
{
    public partial class PendingInvoicesInErrorReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int billingMonth = Convert.ToInt32(Request.QueryString["month"]);
            int billingYear = Convert.ToInt32(Request.QueryString["year"]);
            int billingPeriod = (Request.QueryString["periodNo"]) == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["periodNo"]);
            int billingCategory = (Request.QueryString["billingCategory"]) == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["billingCategory"]);
            int settlementMethod = Convert.ToInt32(Request.QueryString["settlementMethod"]) == -1 ? 0 : Convert.ToInt32(Request.QueryString["settlementMethod"]);
            int errorType = Convert.ToInt32(Request.QueryString["errorType"]) == -1 ? 0 : Convert.ToInt32(Request.QueryString["errorType"]);
            int isTotalRequired = Convert.ToInt32(Request.QueryString["isTotalRequired"]);
            int memberId = SessionUtil.MemberId;
            string rptTotals = "";
            string reportPath = "";

            if (isTotalRequired == 0)
            {
                rptTotals = "";
            }
            else if (isTotalRequired == 1)
            {
                rptTotals = "Totals";
            }

            List<Iata.IS.Model.Reports.PendingInvoicesInErrorReport> listModel = null;

            listModel = Ioc.Resolve<IPendingInvoicesInErrorReport>(typeof(IPendingInvoicesInErrorReport)).GetPendingInvoicesInErrorReport(memberId,billingMonth, billingYear,
                                                                                                                                    billingPeriod, billingCategory,
                                                                                                                settlementMethod, errorType, isTotalRequired);

            reportPath = MapPath("~/Reports/PendingInvoicesInErrorReport/PendingInvoicesInErrorReport.rpt");


            DataTable dt = ListToDataTable(listModel);

            ReportDocument orpt = new ReportDocument();

            // Load report to that report document
            orpt.Load(reportPath);
            // Added data table to report document
            orpt.SetDataSource(dt);

            // Add report Totalsfield values
            orpt.SetParameterValue("Totals", rptTotals);

            //change to display search criteria on report
            orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
            orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
            // Take an format id of pdf and excel report 
            int exportFormatFlags =
                (int)
                (CrystalDecisions.Shared.ViewerExportFormats.PdfFormat |
                 CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

            // Only shows the pdf and export to data option in report and supress other options
            PendingInvoicesInErrorReportRpt.AllowedExportFormats = exportFormatFlags;

            // set report source of correspondencereport
            PendingInvoicesInErrorReportRpt.ReportSource = orpt;
        }

        public static DataTable ListToDataTable(List<Iata.IS.Model.Reports.PendingInvoicesInErrorReport> list)
        {

            DataTable dt = new DataTable();
            foreach (PropertyInfo info in typeof(Iata.IS.Model.Reports.PendingInvoicesInErrorReport).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (Iata.IS.Model.Reports.PendingInvoicesInErrorReport t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(Iata.IS.Model.Reports.PendingInvoicesInErrorReport).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }// End ListToDataTable()
    }
}