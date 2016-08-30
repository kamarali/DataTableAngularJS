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
using Iata.IS.Business.Reports.Miscellaneous;
using Iata.IS.Core.DI;
using Iata.IS.Model.Reports.Miscellaneous;
using Iata.IS.Web.Util;
using log4net;

namespace Iata.IS.Web
{
    public partial class MiscSubstitutionValuesReport : System.Web.UI.Page
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Read the values from QueryString into local variables
                int fromYear = Convert.ToInt32(Request.QueryString["fYear"]);
                int fromMonth = Convert.ToInt32(Request.QueryString["fMonth"]);
                int fromPeriod = Convert.ToInt32(Request.QueryString["fPeriod"]);

                int toYear = Convert.ToInt32(Request.QueryString["tYear"]);
                int toMonth = Convert.ToInt32(Request.QueryString["tMonth"]);
                int toPeriod = Convert.ToInt32(Request.QueryString["tPeriod"]);

                int billingEntityCode = Convert.ToInt32(Request.QueryString["BillingMemberCode"]);
                int? billedEntityCode = null;
                if ((Request.QueryString["BilledMemberCode"]) == string.Empty)
                {
                    billedEntityCode = null;
                }
                else
                {
                    billedEntityCode = Convert.ToInt32(Request.QueryString["BilledMemberCode"]);
                }

                int? chargeCategory = null;
                if ((Request.QueryString["ChargeCategory"]) == "0")
                {
                    chargeCategory = null;
                }
                else
                {
                    chargeCategory = Convert.ToInt32(Request.QueryString["ChargeCategory"]);
                }

                int? chargeCode = null;
                if ((Request.QueryString["ChargeCode"]) == string.Empty)
                {
                    chargeCode = null;
                }
                else
                {
                    chargeCode = Convert.ToInt32(Request.QueryString["ChargeCode"]);
                }

                string invoiceNumber = null;
                if ((Request.QueryString["InvoiceNumber"]) == string.Empty)
                {
                    invoiceNumber = null;
                }
                else
                {
                    invoiceNumber = Convert.ToString(Request.QueryString["InvoiceNumber"]);
                }

                // call the business method to get all the records from the database
                List<MiscSubstitutionValuesReportResult> corrStatusModel =
                    Ioc.Resolve<IMiscellaneous>(typeof(IMiscellaneous)).GetMiscSubstitutionValuesReportDetails(fromYear,
                                                                                                               fromMonth,
                                                                                                               fromPeriod,
                                                                                                               toYear,
                                                                                                               toMonth,
                                                                                                               toPeriod,
                                                                                                               billingEntityCode,
                                                                                                               billedEntityCode,
                                                                                                               chargeCategory,
                                                                                                               chargeCode,
                                                                                                               invoiceNumber);

                // Take value from list to data table
                DataTable dt = ListToDataTable(corrStatusModel);

                // Set the string for report type
                string reportPath = string.Empty;
                try
                {
                    reportPath = MapPath("~/Reports/Miscellaneous/MiscSubstitutionValuesReport.rpt");
                }
                catch (Exception exception)
                {
                    Logger.Error("Unexpected Error Has Occurred", exception);
                }
                // Create an object of report document
                ReportDocument orpt = new ReportDocument();

                // Load report to that report document
                orpt.Load(reportPath);

                // Added data table to report document
                orpt.SetDataSource(dt);

                // Take an format id of pdf and excel report 
                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);
                // Only shows the padf and export to data option in report and supress other options
                MiscSubstitutionValuesReportViewer.AllowedExportFormats = exportFormatFlags;

                ExcelDataOnlyFormatOptions optionsformat = CrystalDecisions.Shared.ExportOptions.CreateDataOnlyExcelFormatOptions();
                optionsformat.MaintainColumnAlignment = true;
                optionsformat.MaintainRelativeObjectPosition = true;
                optionsformat.ShowGroupOutlines = true;
                optionsformat.ExportObjectFormatting = true;
                optionsformat.ExportPageHeaderAndPageFooter = true;
                optionsformat.SimplifyPageHeaders = true;
                optionsformat.ExcelConstantColumnWidth = 1000;

                orpt.ExportOptions.FormatOptions = optionsformat;

                //changes to display search criteria on report
                orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
                orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]); 
                // set report source of correspondencereport
                MiscSubstitutionValuesReportViewer.ReportSource = orpt;
            }
            catch (Exception exception)
            {
                Logger.Error("Unexpected Error Has Occurred", exception);
            }
        }

        public static DataTable ListToDataTable(List<MiscSubstitutionValuesReportResult> list)
        {
            try
            {
                DataTable dt = new DataTable();
                foreach (PropertyInfo info in typeof(MiscSubstitutionValuesReportResult).GetProperties())
                {
                    dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
                }
                foreach (MiscSubstitutionValuesReportResult t in list)
                {
                    DataRow row = dt.NewRow();
                    foreach (PropertyInfo info in typeof(MiscSubstitutionValuesReportResult).GetProperties())
                    {
                        row[info.Name] = info.GetValue(t, null);
                    }

                    dt.Rows.Add(row);
                }
                return dt;
            }
            catch (Exception exception)
            {
                Logger.Error("Unexpected Error Has Occurred", exception);
                return null;
            }
        }// End ListToDataTable()

        public static ReportDocument ReportSource { get; set; }
    }
}