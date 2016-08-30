using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Iata.IS.Model.Reports.Cargo;
using Iata.IS.Business.Reports.Cargo.InterlineBillingSummary;
using Iata.IS.Core.DI;
using System.Data;
using Iata.IS.Web.Util;
using CrystalDecisions.CrystalReports.Engine;
using System.Reflection;
using log4net;

namespace Iata.IS.Web
{
    public partial class InterlineBillingSummaryReport : System.Web.UI.Page
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                int fBillingMonth = Convert.ToInt32(Request.QueryString["fMonth"]);
                int tBillingMonth = Convert.ToInt32(Request.QueryString["tMonth"]);
                int fBillingYear = Convert.ToInt32(Request.QueryString["fYear"]);
                int tBillingYear = Convert.ToInt32(Request.QueryString["tYear"]);
                int? fBillingPeriod;
                if ((Request.QueryString["fPeriodNo"]) == string.Empty)
                {
                    fBillingPeriod = null;
                }
                else
                {
                    fBillingPeriod = Convert.ToInt32(Request.QueryString["fPeriodNo"]);
                }

                int? tBillingPeriod;
                if ((Request.QueryString["tPeriodNo"]) == string.Empty)
                {
                    tBillingPeriod = null;
                }
                else
                {
                    tBillingPeriod = Convert.ToInt32(Request.QueryString["tPeriodNo"]);
                }

                int? settlementMethod;
                if ((Request.QueryString["settlementMethod"]) == "-1")
                {
                    settlementMethod = null;
                }
                else
                {
                    settlementMethod = Convert.ToInt32(Request.QueryString["settlementMethod"]);
                }

                int billingType = Convert.ToInt32(Request.QueryString["bType"]);
                int curencyCode = Convert.ToInt32(Request.QueryString["currencyId"]) == 0 ? 0 : Convert.ToInt32(Request.QueryString["currencyId"]);

                int? airlineCode;
                int? memberId;
                string reportHeaderfield = "";
                string reportPath = "";
                List<InterlineBillingSummary> listModel = null;

                _logger.Info("Report Initiated");

                if (billingType == 1)
                {
                    // Set ReportHeader
                    reportHeaderfield = "Cargo Payables - Interline Billing Summary";
                    memberId = SessionUtil.MemberId;
                    if ((Request.QueryString["airlineCode"]) == string.Empty)
                    {
                        airlineCode = null;
                    }
                    else
                    {
                        airlineCode = Convert.ToInt32(Request.QueryString["airlineCode"]);
                    }
                    listModel = Ioc.Resolve<IInterlineBillingSummaryReport>(typeof(IInterlineBillingSummaryReport)).GetInterlineBillingSummaryReport(billingType, fBillingMonth,
                                                tBillingMonth, fBillingYear, tBillingYear, fBillingPeriod, tBillingPeriod, airlineCode, memberId, settlementMethod, curencyCode);
                }
                else if (billingType == 2)
                {
                    // Set ReportHeader
                    reportHeaderfield = "Cargo Receivables - Interline Billing Summary";
                    airlineCode = SessionUtil.MemberId;
                    if ((Request.QueryString["airlineCode"]) == string.Empty)
                    {
                        memberId = null;
                    }
                    else
                    {
                        memberId = Convert.ToInt32(Request.QueryString["airlineCode"]);
                    }
                    listModel = Ioc.Resolve<IInterlineBillingSummaryReport>(typeof(IInterlineBillingSummaryReport)).GetInterlineBillingSummaryReport(billingType, fBillingMonth,
                                                tBillingMonth, fBillingYear, tBillingYear, fBillingPeriod, tBillingPeriod, airlineCode, memberId, settlementMethod, curencyCode);
                   
                    _logger.Info("Manager method sucessfully called");

                }
                try
                {
                    reportPath = MapPath("~/Reports/Cargo/InterlineBillingSummary/CargoInterlineBillingSummaryReport.rpt");
                }
                catch (Exception exception)
                {
                    _logger.Error("Unexpected Error Has Occurred", exception);
                }
                DataTable dt = ListToDataTable(listModel);

                ReportDocument orpt = new ReportDocument();

                // Load report to that report document
                orpt.Load(reportPath);

                // Added data table to report document
                orpt.SetDataSource(dt);

                // Add report columnheader values
                orpt.SetParameterValue("ReportHeader", reportHeaderfield);
                //changes to display search criteria on report
                orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
                orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
                // Take an format id of pdf and excel report 
                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

                // Only shows the pdf and export to data option in report and supress other options
                InterlineBillingSummaryRpt.AllowedExportFormats = exportFormatFlags;

                // set report source of correspondencereport
                InterlineBillingSummaryRpt.ReportSource = orpt;
            }
            catch (Exception exception)
            {
                _logger.Error("Unexpected Error Has Occurred", exception);
            }
        }
        public static DataTable ListToDataTable(List<InterlineBillingSummary> list)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo info in typeof(InterlineBillingSummary).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (InterlineBillingSummary t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(InterlineBillingSummary).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }// End ListToDataTable()
    }
}