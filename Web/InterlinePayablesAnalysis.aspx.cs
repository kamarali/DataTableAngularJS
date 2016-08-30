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
using Iata.IS.Business.Reports.InterlinePayablesAnalysis;
using Iata.IS.Core.DI;
using Iata.IS.Model.Reports.InterlinePayablesAnalysis;
using Iata.IS.Web.Util;
using log4net;

namespace Iata.IS.Web
{
    public partial class InterlinePayablesAnalysis : System.Web.UI.Page
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
  
        protected void Page_Load(object sender, EventArgs e)
        {
            //Log in into logger
            Logger.Info("Interline Payables Analysis Report: aspx page Being Load!");
            try
            {
                // fetch all values from the query strimg and stored into the variables
                int month = Convert.ToInt32(Request.QueryString["bMonth"]);
                int year = Convert.ToInt32(Request.QueryString["bYear"]);
                int periodNo = Convert.ToInt32(Request.QueryString["periodNo"]);
                int currencyCode = Convert.ToInt32(Request.QueryString["currencyCode"]);
                int billingEntityId = Request.QueryString["billingEntityId"] == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["billingEntityId"]);
                int settlementMethodId = Convert.ToInt32(Request.QueryString["settlementMethodId"]);
                int isTotalsRequired = Convert.ToInt32(Request.QueryString["isTotalsRequired"]);
                int comparisonPeriod = Convert.ToInt32(Request.QueryString["comparisonPeriod"]);
                int logInMemberid = SessionUtil.MemberId;

                // Set the string for report type
                string reportPath = string.Empty;

                // Set rootpath of report
                try
                {
                    reportPath = MapPath("~/Reports/InterlinePayablesAnalysis/InterlinePayablesAnalysisReport.rpt");
                }
                catch (Exception exception)
                {
                    Logger.Error("Unexpected Error Has Occurred", exception);
                }
                // Make an object of SupportingMismatchDoc model class
                List<InterlinePayablesAnalysisModel> listModel;

                //Log in into logger
                Logger.Info("Interline Payables Analysis Report:Business Layer being called for Data!");

                // call the business method to get all the records from the db
                listModel =
                    Ioc.Resolve<IInterlinePayablesAnalysisReport>(typeof(IInterlinePayablesAnalysisReport)).
                        GetInterlinePayablesAnalysisReport(month, year, periodNo, settlementMethodId, logInMemberid, billingEntityId, currencyCode, comparisonPeriod);


                //Log in into logger
                
                Logger.Info("Interline Payables Analysis Report: Business Layer returns Data!");

                // Add the data from list to data table
                DataTable dt = ListToDataTable(listModel);

                // make an object of report document
                ReportDocument orpt = new ReportDocument();

                //Log in into logger
                Logger.Info("Interline Payables Analysis Report: Report(rpt) being loaded!");

              
                // add the path of the report
                orpt.Load(reportPath);

                //Log in into logger
                Logger.Info("Interline Payables Analysis Report: Report(rpt) being loaded with data!");
               
                // Add data source
                orpt.SetDataSource(dt);

                // Add report columnheader values
                // when comparison period is Period
                if (comparisonPeriod == 0)
                {
                    orpt.SetParameterValue("@BillingValueOneMonth", "Net Billing Values per period (Clr Mth -1)");
                    orpt.SetParameterValue("@BillingValueTwoMonth", "Net Billing Values per period (Clr Mth -2)");
                    orpt.SetParameterValue("@BillingValueThreeMonth", "Net Billing Values per period (Clr Mth -3)");
                    orpt.SetParameterValue("@BillingValueFourMonth", "Net Billing Values per period (Clr Mth -4)");
                    orpt.SetParameterValue("@BillingValueFiveMonth", "Net Billing Values per period (Clr Mth -5)");
                }
                //// when comparison period is Month
                else if (comparisonPeriod == 1)
                {
                    orpt.SetParameterValue("@BillingValueOneMonth", "Average Billing Values per period (Clr Mth -1)");
                    orpt.SetParameterValue("@BillingValueTwoMonth", "Average Billing Values per period (Clr Mth -2)");
                    orpt.SetParameterValue("@BillingValueThreeMonth", "Average Billing Values per period (Clr Mth -3)");
                    orpt.SetParameterValue("@BillingValueFourMonth", "Average Billing Values per period (Clr Mth -4)");
                    orpt.SetParameterValue("@BillingValueFiveMonth", "Average Billing Values per period (Clr Mth -5)");
                }

                if (isTotalsRequired == 0)
                {
                    orpt.SetParameterValue("Total", "");
                }
                else if (isTotalsRequired == 1)
                {
                    orpt.SetParameterValue("Total", "Total");
                }

                //change to display search criteria on report
                orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
                orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
                // Take an format id of pdf and excel report 
                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

                // Only shows the padf and export to data option in report and supress other options
                CRInterlinePayablesAnalysis.AllowedExportFormats = exportFormatFlags;

                ExcelDataOnlyFormatOptions optionsformat = CrystalDecisions.Shared.ExportOptions.CreateDataOnlyExcelFormatOptions();
                optionsformat.MaintainColumnAlignment = true;
                optionsformat.MaintainRelativeObjectPosition = true;
                optionsformat.ShowGroupOutlines = true;
                optionsformat.ExportObjectFormatting = true;
                optionsformat.ExportPageHeaderAndPageFooter = true;
                optionsformat.SimplifyPageHeaders = true;
                optionsformat.ExcelConstantColumnWidth = 1000;

                orpt.ExportOptions.FormatOptions = optionsformat;

                CRInterlinePayablesAnalysis.ReportSource = orpt;
                //Log in into logger

                Logger.Info("Interline Payables Analysis Report : Report generation completed!");
            }
            catch (Exception ex)
            {
                //Log in into logger
                Logger.Error("Interline Payables Analysis Report :Error--->", ex);
            }

           
        }

        public static DataTable ListToDataTable(List<InterlinePayablesAnalysisModel> list)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo info in typeof(InterlinePayablesAnalysisModel).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (InterlinePayablesAnalysisModel t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(InterlinePayablesAnalysisModel).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}