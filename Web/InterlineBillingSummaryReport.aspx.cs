using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Iata.IS.Business.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Reports.InterlinePayablesAnalysis;
using Iata.IS.Business.Reports.InterlinePayablesAnalysis;
using Iata.IS.Web.Util;
using Iata.IS.Core.DI;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Reflection;
using CrystalDecisions.Web;
using log4net;

namespace Iata.IS.Web
{
    public partial class InterlineBillingSummaryReport1 : System.Web.UI.Page
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
  
        protected void Page_Load(object sender, EventArgs e)
        {
            //Log in into logger
            Logger.Info("Interline Billing Summary Report: aspx page Being Load!");

            try
            {
                //Read the values from QuerySting into local variables
                int year = Convert.ToInt32(Request.QueryString["qsYear"]);

                int month = Convert.ToInt32(Request.QueryString["qsMonth"]);

                
                
                  int  period = Convert.ToInt32(Request.QueryString["qsPeriodNo"]);
               

                int? settlementMethodId;
                if ((Request.QueryString["qsSettlementMethodStatusId"]) == "-1")
                {
                    settlementMethodId = null;
                }
                else
                {
                    settlementMethodId = Convert.ToInt32(Request.QueryString["qsSettlementMethodStatusId"]);
                }

                int currencyId = Convert.ToInt32(Request.QueryString["qsCurrencyId"]);

                int billingEntityCode = SessionUtil.MemberId;

                int? billedEntityCode;
                if ((Request.QueryString["qsBilledEntityCode"]) == string.Empty)
                {
                    billedEntityCode = null;
                }
                else
                {
                    billedEntityCode = Convert.ToInt32(Request.QueryString["qsBilledEntityCode"]);
                }

                int isTotalsRequired = Convert.ToInt32(Request.QueryString["qsIsTotalsRequired"]);
                int isCurrentPeriod = 0;
                try
                {


                    var calender =
                        Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentBillingPeriod(ClearingHouse.Ich);
                    DateTime calDateTime = new DateTime(calender.Year, calender.Month, calender.Period);
                    DateTime UiDateTime = new DateTime(year, month, period);

                    if (calDateTime.Date == UiDateTime.Date)
                    {
                        isCurrentPeriod = 1;
                    }

                }
                catch (Exception)
                {

                    isCurrentPeriod = 0;
                }
               
                

               
                
                string rptTotals = "";
                //string endLine = "";
                if (isTotalsRequired == 0)
                {
                    rptTotals = "";
                }
                else if (isTotalsRequired == 1)
                {
                    rptTotals = "Totals";
                 }

                //Log in into logger
                Logger.Info("Interline Billing Summary Report:Business Layer being called for Data!");

                List<InterlineBillingSummaryReportResultModel> corrStatusModel =
                    Ioc.Resolve<IInterlinePayablesAnalysisReport>(typeof(IInterlinePayablesAnalysisReport)).GetInterlineBillingSummaryReportDetails(year,
                                                                                                                                                    month,
                                                                                                                                                    period,
                                                                                                                                                    settlementMethodId,
                                                                                                                                                    currencyId,
                                                                                                                                                    billedEntityCode,
                                                                                                                                                    billingEntityCode,
                                                                                                                                                    isCurrentPeriod);
                 //  Iterate through the list 
                for (int i = 0; i < corrStatusModel.Count; i++)
                {

                    if (corrStatusModel[i].CargoInAccept == 0 && corrStatusModel[i].CargoOutAccept == 0 && corrStatusModel[i].CargoOutNotAccept == 0
                        && corrStatusModel[i].MiscInAccept == 0 && corrStatusModel[i].MiscOutAccept == 0 && corrStatusModel[i].MiscOutNotAccept == 0
                        && corrStatusModel[i].PaxInAccept == 0 && corrStatusModel[i].PaxOutAccept == 0 && corrStatusModel[i].PaxOutNotAccept == 0
                        && corrStatusModel[i].UatpInAccept == 0 && corrStatusModel[i].UatpOutAccept == 0 && corrStatusModel[i].UatpOutNotAccept == 0)
                    {
                        corrStatusModel.Remove(corrStatusModel[i]);
                        i--;
                    }
                   
                }


                //Log in into logger
                Logger.Info("Interline Billing Summary Report: Business Layer returns Data!");

                // Take value from list to data table
                DataTable dt = ListToDataTable(corrStatusModel);

                // Set the string for report type
                string reportPath = "";
                try
                {
                    reportPath = MapPath("~/Reports/InterlinePayablesAnalysis/InterlineBillingSummaryReport.rpt");
                }
                catch (Exception exception)
                {
                    Logger.Error("Unexpected Error Has Occurred", exception);
                }
                // Create an object of report document
                ReportDocument orpt = new ReportDocument();

                //Log in into logger
                Logger.Info("Interline Billing Summary Report: Report(rpt) being loaded!");

                // Load report to that report document
                orpt.Load(reportPath);

                //Log in into logger
                Logger.Info("Interline Billing Summary Report: Report(rpt) being loaded with data!");
               
                // Added data table to report document
                orpt.SetDataSource(dt);

                // Add report Totalsfield values
                orpt.SetParameterValue("Totals", rptTotals);
                //orpt.SetParameterValue("EndLine",endLine);

                //change to display search criteria on report
                orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
                orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);  
                // Take an format id of pdf and excel report 
                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

                // Only shows the padf and export to data option in report and supress other options
                CrvInterlineBillingSummaryReportId.AllowedExportFormats = exportFormatFlags;
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
                CrvInterlineBillingSummaryReportId.ReportSource = orpt;
                //Log in into logger
                Logger.Info("Interline Billing Summary Report : Report generation completed!");
            }
            catch (Exception ex)
            {
                //Log in into logger
                Logger.Error("Interline Billing Summary Report :Error--->", ex);
            }
           
        }

        /// <summary>
        /// This method is used to add data to table
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable(List<InterlineBillingSummaryReportResultModel> list)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo info in typeof(InterlineBillingSummaryReportResultModel).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (InterlineBillingSummaryReportResultModel t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(InterlineBillingSummaryReportResultModel).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }// End ListToDataTable()
    }
}