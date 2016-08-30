using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using Iata.IS.Business.Reports.ReceivablesReport;
using Iata.IS.Core.DI;
using Iata.IS.Model.Reports.Cargo;
using Iata.IS.Web.Reports.CGO.RMBMCMDetails;
using Iata.IS.Web.Util;
using log4net;


namespace Iata.IS.Web
{
    public partial class RMBMCMDetailsReport : System.Web.UI.Page
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Log in into logger 	
                _logger.Info("Report Initiated");

                int ClearanceMonth = Convert.ToInt32(Request.QueryString["ClearanceMonth"]);
                int PeriodNo = Request.QueryString["PeriodNo"] == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["PeriodNo"]);
                int BillingType = Convert.ToInt32(Request.QueryString["BillingType"]);
                int SettlementMethod = Convert.ToInt32(Request.QueryString["SettlementMethod"]) == -1 ? 0 : Convert.ToInt32(Request.QueryString["SettlementMethod"]);
                int MemoType = Convert.ToInt32(Request.QueryString["MemoType"]) == -1 ? 0 : Convert.ToInt32(Request.QueryString["MemoType"]);
                int DataSource = Convert.ToInt32(Request.QueryString["DataSource"]) == -1 ? 0 : Convert.ToInt32(Request.QueryString["DataSource"]);
                int AirlineCode = (Request.QueryString["AirlineCode"]) == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["AirlineCode"]);
                string InvoiceNumber = (Request.QueryString["InvoiceNumber"]);
                int Output = Convert.ToInt32(Request.QueryString["Output"]);
                string RMBMCMNumber = (Request.QueryString["RMBMCMNumber"]);
                int billingYear = Convert.ToInt32(Request.QueryString["billingYear"]);

                //Log in into logger
                _logger.Info("RMBMCMDetailsReport : Business Layer being called for Data!");

                int loginMember = SessionUtil.MemberId;
                List<RMBMCMDetailsUI> rmbmcmdetails =
                    Ioc.Resolve<IReceivablesReport>(typeof(IReceivablesReport)).GetRMBMCMDetailsReport(billingYear,
                                                                                                        ClearanceMonth,
                                                                                                        PeriodNo,
                                                                                                        BillingType,
                                                                                                        SettlementMethod,
                                                                                                        MemoType, DataSource,
                                                                                                        AirlineCode,
                                                                                                        InvoiceNumber,
                                                                                                        RMBMCMNumber,
                                                                                                        loginMember, Output);



                // If billing type is payables then login member should be billed member
                //List<RMBMCMDetailsUI> newrmbmcmDetails = new List<RMBMCMDetailsUI>();
                //List<RMBMCMDetailsUI> rmbmcmdetailssubtotal = new List<RMBMCMDetailsUI>();

                for (int i = 0; i < rmbmcmdetails.Count; i++)
                {
                    rmbmcmdetails[i].BillingType = BillingType == 1 ? "Payables" : "Receivables";

                    if (rmbmcmdetails[i].AttachmentIndicatorOrig == 0)
                    {
                        rmbmcmdetails[i].AttachmentIndicator = "N";
                    }
                    else if (rmbmcmdetails[i].AttachmentIndicatorOrig == 1)
                    {
                        rmbmcmdetails[i].AttachmentIndicator = "Y";
                    }
                    else
                    {
                        rmbmcmdetails[i].AttachmentIndicator = " ";
                    }
                }

                //if (Output == 3)
                //{
                //    newrmbmcmDetails = rmbmcmdetails.Where(l => l.AttachmentIndicatorOrig == 2).ToList();
                //    var invoiceNo = rmbmcmdetails.Select(l => l.InvoiceNumber).ToList().Distinct();

                //    foreach (string invoiceNumber in invoiceNo)
                //    {
                //        var rmbmcminvoicedetails = rmbmcmdetails.Where(l => l.InvoiceNumber == invoiceNumber).ToList();
                //        for(int i= 0; i < rmbmcminvoicedetails.Count ; i++)
                //        {
                //            rmbmcmdetailssubtotal.Add(rmbmcminvoicedetails[i]);
                //        }
                //        var subtotalDetails = newrmbmcmDetails.Where(l => l.InvoiceNumber == invoiceNumber).ToList();
                //        foreach (var rmbmcmDetailsUi in subtotalDetails)
                //        {
                //            rmbmcmdetailssubtotal.Add(rmbmcmDetailsUi);
                //        }

                //    }
                //}

                // Take value from list to data table
                DataTable dt = ListToDataTable(rmbmcmdetails);

                // Set the string for report type
                string reportPath = "";


                // Take a path of report
                try
                {
                    reportPath = MapPath("~/Reports/CGO/RMBMCMDetails/RMBMCMDetailsReport.rpt");

                }
                catch (Exception exception)
                {
                    _logger.Error("Unexpected Error Has Occurred", exception);
                }
                ReportDocument orpt = new ReportDocument();

                //Log in into logger
                _logger.Info("RMBMCMDetailsReport : Report(rpt) being loaded with data!");

                // Load report to that report document
                orpt.Load(reportPath);
                // Added data table to report document
                orpt.SetDataSource(dt);
                //changes to display search criteria on report
                orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
                orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
                // Take an format id of pdf and excel report 
                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

                // Only shows the padf and export to data option in report and supress other options
                RMBMCMReport.AllowedExportFormats = exportFormatFlags;

                // set report source of correspondencereport
                RMBMCMReport.ReportSource = orpt;

                //Log in into logger
                _logger.Info("RMBMCMDetailsReport : Report generation completed!");
            }
            catch (Exception exception)
            {
                _logger.Error("Unexpected Error Has Occurred", exception);
            }
            
        }


        /// <summary>
        /// This method is used to add data to table
        /// </summary>
        /// <param name="list">list of correpondence</param>
        /// <returns></returns>
        public static DataTable ListToDataTable(List<RMBMCMDetailsUI> list)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo info in typeof(RMBMCMDetailsUI).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (RMBMCMDetailsUI t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(RMBMCMDetailsUI).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }// End ListToDataTable()
    }
}