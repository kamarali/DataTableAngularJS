using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Iata.IS.Web.Util;
using Iata.IS.Core.DI;
using Iata.IS.Web.Reports.Miscellaneous;
using Iata.IS.Business.Reports.Miscellaneous;
using System.Data;
using System.Reflection;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Web;
using log4net;
using InvoiceSummaryReportModel = Iata.IS.Model.Reports.Miscellaneous.InvoiceSummaryReportModel;


namespace Iata.IS.Web
{
    public partial class MiscInvoiceSummary : System.Web.UI.Page
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
  
        protected void Page_Load(object sender, EventArgs e)
        {
            //Log in logger
            Logger.Info("Misc Invoice Summary Report : aspx page Being Load!");

            try
            {
                //Read the values from QueryString into local variables
                int fYear = Convert.ToInt32(Request.QueryString["fYear"]);
                int fMonth = Convert.ToInt32(Request.QueryString["fMonth"]);
                int fPeriod = Convert.ToInt32(Request.QueryString["fPeriod"]);

                int tYear = Convert.ToInt32(Request.QueryString["tYear"]);
                int tMonth = Convert.ToInt32(Request.QueryString["tMonth"]);
                int tPeriod = Convert.ToInt32(Request.QueryString["tPeriod"]);

                int billingType = Convert.ToInt32(Request.QueryString["BillingType"]);
        	
		/// <summary>
	        /// Author: Kamarali Dukandar
        	/// Purpose: CMP#663  MISC Invoice Summary Reports - Add 'Transaction Type'
		/// Gets value of invoiceType  from UI
	        /// </summary>
                int invoiceType = Convert.ToInt32(Request.QueryString["InvoiceType"]);            

                int? billingEntityCode;
                int? billedEntityCode;

                if (billingType == 1)
                {
                    billedEntityCode = SessionUtil.MemberId;
                    if ((Request.QueryString["BilledMemberCode"]) == string.Empty)
                    {
                        billingEntityCode = null;
                    }
                    else
                    {
                        // CMP#596: converted int to long to support membernumeric code upto 12 digits
                        billingEntityCode = Convert.ToInt32(Request.QueryString["BilledMemberCode"]);
                    }
                }
                else
                {
                    billingEntityCode = SessionUtil.MemberId;
                    if ((Request.QueryString["BilledMemberCode"]) == string.Empty)
                    {
                        billedEntityCode = null;
                    }
                    else
                    {
                        // CMP#596: converted int to long to support membernumeric code upto 12 digits
                        billedEntityCode = Convert.ToInt32(Request.QueryString["BilledMemberCode"]);
                    }
                }

                int? dataSource = 0;
                if (billingType == 1)
                {
                    dataSource = 0;
                }
                else if (billingType == 2)
                {
                    if ((Request.QueryString["DataSource"]) == "-1")
                    {
                        dataSource = null;
                    }
                    else
                    {
                        dataSource = Convert.ToInt32((Request.QueryString["DataSource"]));
                    }
                }

                int? settlementMethod;

                if ((Request.QueryString["SettlementId"]) == "-1")
                {
                    settlementMethod = null;
                }
                else
                {
                    settlementMethod = Convert.ToInt32((Request.QueryString["SettlementId"]));
                }

                int? chargeCategory;
                if ((Request.QueryString["ChargeCategory"]) == "0")
                {
                    chargeCategory = null;
                }
                else
                {
                    var chargecat = 0;
                    if (int.TryParse(Request.QueryString["ChargeCategory"], out chargecat))
                    {
                        chargeCategory = chargecat;
                    }
                    else
                    {
                        chargeCategory = null;
                    }
                }

                //int? settlementMethod = Request.QueryString["SettlementId"] == string.Empty ? null : Convert.ToInt32((Request.QueryString["SettlementId"]));
                //int? chargeCategory = Request.QueryString["ChargeCategory"] == string.Empty ? null : Convert.ToInt32((Request.QueryString["ChargeCategory"]));

                int? currencyCode;
                if ((Request.QueryString["CurrencyCode"]) == string.Empty)
                {
                    currencyCode = null;
                }
                else
                {
                    var currencyCd = 0;
                    if (int.TryParse(Request.QueryString["CurrencyCode"], out currencyCd))
                    {
                        currencyCode = currencyCd;
                    }
                    else
                    {
                        currencyCode = null;
                    }
                }

                // call the business method to get all the records from the database

                Logger.Info("Misc Invoice Summary Report: Business Layer being called for Data!");

                    List<InvoiceSummaryReportModel> corrStatusModel = Ioc.Resolve<IMiscellaneous>
                                                                                 (typeof(IMiscellaneous)).GetInvoiceSummaryReportDetails(billingType,
                                                                                                                                         fMonth,
                                                                                                                                         fYear,
                                                                                                                                         fPeriod,
                                                                                                                                         tMonth,
                                                                                                                                         tYear,
                                                                                                                                         tPeriod,
                                                                                                                                         billedEntityCode,
                                                                                                                                         billingEntityCode,
                                                                                                                                         dataSource,
                                                                                                                                         settlementMethod,
                                                                                                                                         chargeCategory,
                                                                                                                                         currencyCode,
                                                                                                                                         invoiceType);


                Logger.Info("Misc Invoice Summary Report : Business Layer retruns data!");
                // Take value from list to data table

                DataTable dt = ListToDataTable(corrStatusModel);

                // Set the string for report type

                string reportPath = "";
                if (billingType == 1)
                {
                    try
                    {
                        reportPath = MapPath("~/Reports/Miscellaneous/ChargeSummaryReport.rpt");
                    }
                    catch (Exception exception)
                    {
                        Logger.Error("Unexpected Error Has Occurred", exception);
                    }
                }
                else if (billingType == 2)
                {
                    try
                    {
                        reportPath = MapPath("~/Reports/Miscellaneous/InvoiceSummaryReport.rpt");
                    }
                    catch (Exception exception)
                    {
                        Logger.Error("Unexpected Error Has Occurred", exception);
                    }
                }

                // Set ReportHeader
                string reportHeaderfield = string.Empty;

                if (billingType == 1)
                {
                    reportHeaderfield = "Miscellaneous Charge Summary Report - Payables";
                }
                else if (billingType == 2)
                {
                    reportHeaderfield = "Miscellaneous Invoice Summary Report - Receivables ";
                }

                // Create an object of report document
                ReportDocument orpt = new ReportDocument();

                Logger.Info("Misc Invoice Summary Report: Report(rpt) being loaded!");
                // Load report to that report document
                orpt.Load(reportPath);

                Logger.Info("Misc Invoice Summary Report: Report(rpt) being loaded with data!");
                // Added data table to report document
                orpt.SetDataSource(dt);

                Logger.Info("Misc Invoice Summary Report: Report(rpt) being loaded with parameter!");
                // Add report columnheader values
                orpt.SetParameterValue("ReportHeading", reportHeaderfield);
                //changes to display search criteria on report
                orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
                orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
                Logger.Info("Misc Invoice Summary Report: Report(rpt) loaded with data and parameter!");
                // Take an format id of pdf and excel report 
                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);
                // Only shows the padf and export to data option in report and supress other options
                CRMiscellaneousReport.AllowedExportFormats = exportFormatFlags;

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
                CRMiscellaneousReport.ReportSource = orpt;
                Logger.Info("Misc Invoice Summary Report: Report generation completed!");

                // for security pourpose need to dispose the object.
                //orpt.Close();
                //orpt.Dispose();
            }
            catch (Exception ex)
            {

                Logger.Error("Error occurred report genration:", ex);
            }
           
        }

        /// <summary>
        /// This method is used to add data to table
        /// </summary>
        /// <param name="list">list of correpondence</param>
        /// <returns></returns> 
        public static DataTable ListToDataTable(List<InvoiceSummaryReportModel> list)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo info in typeof(InvoiceSummaryReportModel).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (InvoiceSummaryReportModel t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(InvoiceSummaryReportModel).GetProperties())
                {
                    //row[info.Name] = info.GetValue(t, null);
                    // TODO
                    if (info.Name == "InvoiceType")
                    {
                        var k = info.GetValue(t, null).ToString();

                        row[info.Name] = EnumMapper.GetInvoiceTypeList().Where(x => x.Value == k).SingleOrDefault().Text;
                    }
                    else
                    {
                        row[info.Name] = info.GetValue(t, null);
                    }
                }

                dt.Rows.Add(row);
            }
            return dt;
        }// End ListToDataTable()

        public static ReportDocument ReportSource { get; set; }
        
    }
}