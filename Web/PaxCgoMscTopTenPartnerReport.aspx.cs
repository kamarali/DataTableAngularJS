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
using Iata.IS.Business.Reports.PaxCgoMscTopTenPartner;
using Iata.IS.Core.DI;
using Iata.IS.Model.Reports.PaxCgoMscTopTenPartner;
using Iata.IS.Web.Util;
using log4net;

namespace Iata.IS.Web
{
    public partial class PaxCgoMscTopTenPartnerReport : System.Web.UI.Page
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                
                // fetch all values from the query strimg and stored into the variables
                int month = Convert.ToInt32(Request.QueryString["bMonth"]);
                int year = Convert.ToInt32(Request.QueryString["bYear"]);
                int billingCategory = Convert.ToInt32(Request.QueryString["billingCategory"]);
                int currencyCode = Convert.ToInt32(Request.QueryString["currencyCode"]);
                int isPayableReport = Convert.ToInt32(Request.QueryString["IsPayableReport"]);
                int logInMemberid = SessionUtil.MemberId;

                // Set the string for report type
                string reportPath = string.Empty;
                string reportHeaderfield = string.Empty;
                List<string> reportColumnHeader = new List<string>();
                DateTime date = new DateTime();

                //Log in into logger 	
                _logger.Info("Report Initiated");

                // Set rootpath of report
                try
                {
                    reportPath = MapPath("~/Reports/PaxCgoMscTopTenPartner/PaxCgoMscTopTenPartnerReport.rpt");
                }
                catch (Exception exception)
                {
                    _logger.Error("Unexpected Error Has Occurred", exception);
                }

                // Set ReportHeader
                if (isPayableReport == 0)
                {
                    reportHeaderfield = "Top 10 Interline Partner Report- Receivables";
                }
                else if (isPayableReport == 1)
                {
                    reportHeaderfield = "Top 10 Interline Partner Report- Payables";
                }
                // Make an object of SupportingMismatchDoc model class
                List<PaxCgoMscTopTenPartnerReportModel> listModel;

                //Log in into logger
                _logger.Info("PaxCgoMscTopTenPartnerReport : Business Layer being called for Data!");

                // call the business method to get all the records from the db
                listModel =
                    Ioc.Resolve<IPaxCgoMscTopTenInterlinePartner>(typeof(IPaxCgoMscTopTenInterlinePartner)).
                        GetPaxCgoMscTopTenInterlinePartner(month, year, billingCategory, logInMemberid, currencyCode,
                                                           isPayableReport);
                //Iterate through the list 
                //for (int i = 0; i < listModel.Count; i++)
                //{

                //}// End for

                // Add the data from list to data table
                DataTable dt = ListToDataTable(listModel);

                // make an object of report document
                ReportDocument orpt = new ReportDocument();

                //Log in into logger
                _logger.Info("PaxCgoMscTopTenPartnerReport : Report(rpt) being loaded with data!");

                // add the path of the report
                orpt.Load(reportPath);

                // Add data source
                orpt.SetDataSource(dt);

                // get report columnheader values
                date = new DateTime(year, month, 1);
                for (int i = 0; i < 12; i++)
                {
                    reportColumnHeader.Add(String.Format("{0:y}", date.AddMonths(-i)));
                }
                // Add report columnheader values
                orpt.SetParameterValue("ReportHeader", reportHeaderfield);
                orpt.SetParameterValue("zeroMonthBefore", reportColumnHeader[0]);
                orpt.SetParameterValue("oneMonthBefore", reportColumnHeader[1]);
                orpt.SetParameterValue("twoMonthBefore", reportColumnHeader[2]);
                orpt.SetParameterValue("threeMonthBefore", reportColumnHeader[3]);
                orpt.SetParameterValue("fourMonthBefore", reportColumnHeader[4]);
                orpt.SetParameterValue("fiveMonthBefore", reportColumnHeader[5]);
                orpt.SetParameterValue("sixMonthBefore", reportColumnHeader[6]);
                orpt.SetParameterValue("sevenMonthBefore", reportColumnHeader[7]);
                orpt.SetParameterValue("eightMonthBefore", reportColumnHeader[8]);
                orpt.SetParameterValue("nineMonthBefore", reportColumnHeader[9]);
                orpt.SetParameterValue("tenMonthBefore", reportColumnHeader[10]);
                orpt.SetParameterValue("elevenMonthBefore", reportColumnHeader[11]);

                //change to display search criteria on report
                orpt.SetParameterValue("SearchCriteria",Request.QueryString["SearchCriteria"]);
                orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
                // Take an format id of pdf and excel report 
                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

                // Only shows the padf and export to data option in report and supress other options
                CRPaxCgoMscTopTenPartner.AllowedExportFormats = exportFormatFlags;
                ExcelDataOnlyFormatOptions optionsformat = CrystalDecisions.Shared.ExportOptions.CreateDataOnlyExcelFormatOptions();
                optionsformat.MaintainColumnAlignment = true;
                optionsformat.MaintainRelativeObjectPosition = true;
                optionsformat.ShowGroupOutlines = true;
                optionsformat.ExportObjectFormatting = true;
                optionsformat.ExportPageHeaderAndPageFooter = true;
                optionsformat.SimplifyPageHeaders = true;
                optionsformat.ExcelConstantColumnWidth = 1000;

                orpt.ExportOptions.FormatOptions = optionsformat;

                CRPaxCgoMscTopTenPartner.ReportSource = orpt;

                //Log in into logger
                _logger.Info("PaxCgoMscTopTenPartnerReport : Report generation completed!");

            }
            catch (Exception exception)
            {
                _logger.Error("Unexpected Error Has Occurred", exception);
            }
             
        }

        public static DataTable ListToDataTable(List<PaxCgoMscTopTenPartnerReportModel> list)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo info in typeof(PaxCgoMscTopTenPartnerReportModel).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (PaxCgoMscTopTenPartnerReportModel t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(PaxCgoMscTopTenPartnerReportModel).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }
 
    }
}