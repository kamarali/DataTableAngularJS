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
using Iata.IS.Business.Reports;
using Iata.IS.Business.Reports.CargoSubmissionOverview;
using Iata.IS.Core.DI;
using Iata.IS.Web.Util;
using log4net;

namespace Iata.IS.Web
{
    public partial class CargoSubmissionOverview : System.Web.UI.Page
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
  
        protected void Page_Load(object sender, EventArgs e)
        {
            //Log in into logger
            Logger.Info("Cargo Submission Overview : aspx page Being Load!");


            try
            {
                // fetch all values from the query strimg and stored into the variables

                int billingMonthFrom = Convert.ToInt32(Request.QueryString["bMonthFrom"]);
                int billingMonthTo = Convert.ToInt32(Request.QueryString["bMonthTo"]);
                int billingPeriodFrom = (Request.QueryString["PeriodFrom"] == string.Empty) ? 0 : Convert.ToInt32(Request.QueryString["PeriodFrom"]);
                int billingPeriodTo = (Request.QueryString["PeriodTo"] == string.Empty) ? 0 : Convert.ToInt32(Request.QueryString["PeriodTo"]);
                int billingYearFrom = Convert.ToInt32(Request.QueryString["bYearFrom"]);
                int billingYearTo = Convert.ToInt32(Request.QueryString["bYearTo"]);
                int settlementMethod = Convert.ToInt32(Request.QueryString["SetelmentMethodId"]);
                string BillingType = Request.QueryString["BillingType"] == string.Empty ? null : Request.QueryString["BillingType"];
                int logInMemberid = SessionUtil.MemberId;
                int BilledEntity = (Request.QueryString["BilledEntity"] == string.Empty) ? 0 : Convert.ToInt32(Request.QueryString["BilledEntity"]);
                int BillingEntity = (Request.QueryString["BillingEntity"] == string.Empty) ? 0 : Convert.ToInt32(Request.QueryString["BillingEntity"]);
                int Output = Convert.ToInt32(Request.QueryString["Output"]);
                string EntityLabel = "";
                string ReportHeader = "";
                // Set the string for report type
                string reportPath = "";

                // Make an object of SupportingMismatchDoc model class
                List<Iata.IS.Model.Reports.ReceivableCargoSubmissionOverviewModel.ReceivableCargoSubmissionOverview> listModel;


                //Log in into logger
                Logger.Info("Cargo Submission Overview :Business Layer being called for Data!");

                if (BillingType == "Payables")
                {
                    ReportHeader = "Cargo Submission Overview - Report for Payables";
                    EntityLabel = "Billing Member Code";
                    BilledEntity = logInMemberid;
                    // call the business method to get all the records from the db
                    listModel = Ioc.Resolve<ICargoSubmissionOverview>(typeof(ICargoSubmissionOverview)).GetSubmissionOverview(BillingType, billingYearFrom, billingYearTo, billingMonthFrom, billingMonthTo, billingPeriodFrom, billingPeriodTo, BillingEntity, BilledEntity, settlementMethod, Output);
                    // Set rootpath of report
                    
                    try
                    {
                        reportPath = MapPath("~/Reports/CargoSubmissionOverview/CargoSubmissionOverviewReport.rpt");
                    }
                    catch (Exception exception)
                    {
                        Logger.Error("Unexpected Error Has Occurred", exception);
                    }
                }
                else
                {
                    ReportHeader = "Cargo Submission Overview - Report for Receivables";
                    EntityLabel = "Billed Member Code";
                    BillingEntity = logInMemberid;
                    // call the business method to get all the records from the db
                    listModel = Ioc.Resolve<ICargoSubmissionOverview>(typeof(ICargoSubmissionOverview)).GetSubmissionOverview(BillingType, billingYearFrom, billingYearTo, billingMonthFrom, billingMonthTo, billingPeriodFrom, billingPeriodTo, BillingEntity, BilledEntity, settlementMethod, Output);
                    // Set rootpath of report
                    try
                    {
                        reportPath = MapPath("~/Reports/CargoSubmissionOverview/CargoSubmissionOverviewReport.rpt");
                    }
                    catch (Exception exception)
                    {
                        Logger.Error("Unexpected Error Has Occurred", exception);
                    }
                }

                //Log in into logger
                Logger.Info("Cargo Submission Overview: Business Layer returns Data!");

                //Add the data from list to data table
                DataTable dt = ListToDataTable(listModel);
                // make an object of report document
                ReportDocument orpt = new ReportDocument();

                //Log in into logger
                Logger.Info("Cargo Submission Overview : Report(rpt) being loaded!");

                // add the path of the report
                orpt.Load(reportPath);

                //Log in into logger
                Logger.Info("Cargo Submission Overview : Report(rpt) being loaded with data!");
               
                // Add data source
                orpt.SetDataSource(dt);

                // setting value for Billed/Billing entity Caption in .rpt
                orpt.SetParameterValue("EntityLabel", EntityLabel);

                // Setting value for Payable/Receivable header in .rpt
                orpt.SetParameterValue("ReportHeader", ReportHeader);

                // Take an format id of pdf and excel report 
                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

                // Only shows the padf and export to data option in report and supress other options
                CRSubmissionOverview.AllowedExportFormats = exportFormatFlags;

                ExcelDataOnlyFormatOptions optionsformat = CrystalDecisions.Shared.ExportOptions.CreateDataOnlyExcelFormatOptions();
                optionsformat.MaintainColumnAlignment = true;
                optionsformat.MaintainRelativeObjectPosition = true;
                optionsformat.ShowGroupOutlines = true;
                optionsformat.ExportObjectFormatting = true;
                optionsformat.ExportPageHeaderAndPageFooter = true;
                optionsformat.SimplifyPageHeaders = true;
                optionsformat.ExcelConstantColumnWidth = 1000;

                orpt.ExportOptions.FormatOptions = optionsformat;

                CRSubmissionOverview.ReportSource = orpt;
                //changes to display search criteria on report
                orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
                orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
                //Log in into logger
                Logger.Info("Cargo Submission Overview : Report generation completed!");
            }
            catch (Exception ex)
            {

                //Log in into logger
                Logger.Error("Cargo Submission Overview  :Error--->", ex);
            }
          

        }// end page_load

        public static DataTable ListToDataTable(List<Iata.IS.Model.Reports.ReceivableCargoSubmissionOverviewModel.ReceivableCargoSubmissionOverview> list)
        {
            DataTable dt = new DataTable();
            foreach (
                PropertyInfo info in
                    typeof(Iata.IS.Model.Reports.ReceivableCargoSubmissionOverviewModel.ReceivableCargoSubmissionOverview).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (Iata.IS.Model.Reports.ReceivableCargoSubmissionOverviewModel.ReceivableCargoSubmissionOverview t in list)
            {
                DataRow row = dt.NewRow();
                foreach (
                    PropertyInfo info in
                        typeof(Iata.IS.Model.Reports.ReceivableCargoSubmissionOverviewModel.ReceivableCargoSubmissionOverview).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}