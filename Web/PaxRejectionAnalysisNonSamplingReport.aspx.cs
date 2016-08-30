using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Iata.IS.Model.Reports.ReceivablesReport;
using Iata.IS.Business.Reports.ReceivablesReport;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Reflection;
using Iata.IS.Core.DI;
using Iata.IS.Web.Util;
using CrystalDecisions.Web;
using log4net;

namespace Iata.IS.Web
{
    public partial class PaxRejectionAnalysisNonSamplingReport : System.Web.UI.Page
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Log in into logger 	
                _logger.Info("Report Initiated");

                //CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
                //input parameter updated (From Year Month and To Year Month)

                //Read the values from QuerySting into local variables
                int fromMonth = Convert.ToInt32(Request.QueryString["qsFromMonth"]);

                int fromYear = Convert.ToInt32(Request.QueryString["qsFromYear"]);

                int toMonth = Convert.ToInt32(Request.QueryString["qsToMonth"]);

                int toYear = Convert.ToInt32(Request.QueryString["qsToYear"]);


                int billingEntityCode = SessionUtil.MemberId;

                int billedEntityCode;
                if ((Request.QueryString["qsBilledEntityCode"]) == string.Empty)
                {
                    billedEntityCode = 0;
                }
                else
                {
                    billedEntityCode = Convert.ToInt32(Request.QueryString["qsBilledEntityCode"]);
                }

                int currencyId = Convert.ToInt32(Request.QueryString["qsCurrencyId"]);

                int includeFIMData = Convert.ToInt32(Request.QueryString["qsIncludeFIMData"]);

                int billingType = Convert.ToInt32(Request.QueryString["qsBillingType"]);

                //Log in into logger
                _logger.Info("PaxRejectionAnalysisNonSamplingReport : Business Layer being called for Data!");

               // <!--CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports-->
                List<PaxRejectionAnalysisNonSamplingReportResult> corrStatusModel =
                                 Ioc.Resolve<IReceivablesReport>(typeof(IReceivablesReport)).GetPaxRejectionAnalysisNonSamplingReportDetails(fromMonth,
                                                                                                                                             fromYear,
                                                                                                                                             toMonth,
                                                                                                                                             toYear,
                                                                                                                                             billingEntityCode,
                                                                                                                                             billedEntityCode,
                                                                                                                                             currencyId,
                                                                                                                                             includeFIMData,
                                                                                                                                             billingType
                                                                                                                                             );
                //Iterate through the list 

                /*
                //CMP #570: Enhancements to PAX Non-Sampling Rejection Analysis Report
                for (int i = 0; i < corrStatusModel.Count; i++)
                {
                    // field which have 0 value are removed here for testing, in real scenario this must not happen

                    if (corrStatusModel[i].TotalNoOfPrimeCoupons == 0 && corrStatusModel[i].NoOfRm1CpnsRejected == 0)
                    {
                        corrStatusModel.Remove(corrStatusModel[i]);
                        i--;
                    }
                }// End for
                */

                // Take value from list to data table
                DataTable dt = ListToDataTable(corrStatusModel);

                // Set the string for report type
                string reportPath = "";
                try
                {
                    reportPath = MapPath("~/Reports/ReceivablesReport/PaxRejectionAnalysisNonSamplingReport.rpt");
                }
                catch (Exception exception)
                {
                    _logger.Error("Unexpected Error Has Occurred", exception);
                }
                // Set ReportHeader
                string reportHeaderfield = "";
                string direction = "";
                string memberCode = "";
                string memberName = "";
                string Rej1Caption = "";
                string Rej2Caption = "";
                string Rej3Caption = "";
                string RejRaisedbyCpnCntVsPrimeBillingR1 = "";
                string RejRaisedbyRejValueVsPrimebillingR1 = "";
                string RejReceivedbyCpnVsPrimebillingR2 = "";
                string RejReceivedbyRejValueVsPrimebilling2 = "";
                string RejReceivedbyCpnCntVs1stRejR2 = "";
                string RejReceivedbyRejValueVs1stRejR2 = "";
                string RejRaisedbyCpnCntVsPrimebillingR3 = "";
                string RejRaisedbyRejectedValueVsPrimebillingR3 = "";
                string RejRaisedbyCpnCntVs2ndRejR3 = "";
                string RejRaisedbyRejValueVs2ndRejR3 = "";


                if (billingType == 1)
                {
                    reportHeaderfield = "Payables - Passenger Rejection Analysis - Non Sampling Report";
                    direction = "Inward Billing Month-Year";
                    memberCode = "Billing Member Code";
                    memberName = "Billing Member Name";
                    Rej1Caption = "1st Rejection - Raised";
                    Rej2Caption = "2nd Rejection - Received";
                    Rej3Caption = "3rd Rejection - Raised";
                    RejRaisedbyCpnCntVsPrimeBillingR1 = "Rejection Raised % by Cpn count Vs Prime billing (R1)";
                    RejRaisedbyRejValueVsPrimebillingR1 = "Rejection  Raised % by Rejected Value Vs Prime billing (R1)";
                    RejReceivedbyCpnVsPrimebillingR2 = "Rejection  Received % by Cpn count Vs Prime billing (R2)";
                    RejReceivedbyRejValueVsPrimebilling2 = "Rejection Received % by Rejected Value Vs Prime billing (R2)";
                    RejReceivedbyCpnCntVs1stRejR2 = "Rejection Received % by Cpn count Vs 1st Rej. (R2)";
                    RejReceivedbyRejValueVs1stRejR2 = "Rejection Received % by Rejected Value Vs 1st Rej. (R2)";
                    RejRaisedbyCpnCntVsPrimebillingR3 = "Rejection Raised % by Cpn count Vs Prime billing (R3)";
                    RejRaisedbyRejectedValueVsPrimebillingR3 = "Rejection Raised % by Rejected Value Vs Prime billing (R3)";
                    RejRaisedbyCpnCntVs2ndRejR3 = "Rejection Raised % by Cpn count Vs 2nd Rej. (R3)";
                    RejRaisedbyRejValueVs2ndRejR3 = "Rejection Raised % by Rejected Value Vs 2nd Rej. (R3)";

                }
                else if (billingType == 2)
                {
                    reportHeaderfield = "Receivables - Passenger Rejection Analysis - Non Sampling Report";
                    direction = "Outward Billing Month-Year";
                    memberCode = "Billed Member Code";
                    memberName = "Billed Member Name";
                    Rej1Caption = "1st Rejection - Received";
                    Rej2Caption = "2nd Rejection - Raised";
                    Rej3Caption = "3rd Rejection - Received";
                    RejRaisedbyCpnCntVsPrimeBillingR1 = "Rejection Received % by Cpn count Vs Prime billing (R1)";
                    RejRaisedbyRejValueVsPrimebillingR1 = "Rejection Received % by Rejected Value Vs Prime billing (R1)";
                    RejReceivedbyCpnVsPrimebillingR2 = "Rejection Raised  % by Cpn count Vs Prime billing (R2)";
                    RejReceivedbyRejValueVsPrimebilling2 = "Rejection  Raised % by Rejected Value Vs Prime billing (R2)";
                    RejReceivedbyCpnCntVs1stRejR2 = "Rejection Raised  % by Cpn count Vs 1st Rej. (R2)";
                    RejReceivedbyRejValueVs1stRejR2 = "Rejection  Raised % by Rejected Value Vs 1st Rej. (R2)";
                    RejRaisedbyCpnCntVsPrimebillingR3 = "Rejection Received % by Cpn count Vs Prime billing (R3)";
                    RejRaisedbyRejectedValueVsPrimebillingR3 = "Rejection  Received % by Rejected Value Vs Prime billing (R3)";
                    RejRaisedbyCpnCntVs2ndRejR3 = "Rejection  Received % by Cpn count Vs 2nd Rej. (R3)";
                    RejRaisedbyRejValueVs2ndRejR3 = "Rejection  Received % by Rejected Value Vs 2nd Rej. (R3)";
                }

                // Create an object of report document
                ReportDocument orpt = new ReportDocument();

                //Log in into logger
                _logger.Info("PaxRejectionAnalysisNonSamplingReport : Report(rpt) being loaded with data!");

                // Load report to that report document
                orpt.Load(reportPath);

                // Added data table to report document
                orpt.SetDataSource(dt);

                // Add report columnheader values
                orpt.SetParameterValue("BillingTypeField", reportHeaderfield);
                orpt.SetParameterValue("Direction", direction);
                orpt.SetParameterValue("MemberCode",memberCode);
                orpt.SetParameterValue("MemberName", memberName);
                orpt.SetParameterValue("Rej1Caption", Rej1Caption);
                orpt.SetParameterValue("Rej2Caption", Rej2Caption);
                orpt.SetParameterValue("Rej3Caption", Rej3Caption);
                orpt.SetParameterValue("RejRaisedbyCpnCntVsPrimeBillingR1", RejRaisedbyCpnCntVsPrimeBillingR1);
                orpt.SetParameterValue("RejRaisedbyRejValueVsPrimebillingR1", RejRaisedbyRejValueVsPrimebillingR1);
                orpt.SetParameterValue("RejReceivedbyCpnVsPrimebillingR2", RejReceivedbyCpnVsPrimebillingR2);
                orpt.SetParameterValue("RejReceivedbyRejValueVsPrimebilling2", RejReceivedbyRejValueVsPrimebilling2);
                orpt.SetParameterValue("RejReceivedbyCpnCntVs1stRejR2", RejReceivedbyCpnCntVs1stRejR2);
                orpt.SetParameterValue("RejReceivedbyRejValueVs1stRejR2", RejReceivedbyRejValueVs1stRejR2);
                orpt.SetParameterValue("RejRaisedbyCpnCntVsPrimebillingR3", RejRaisedbyCpnCntVsPrimebillingR3);
                orpt.SetParameterValue("RejRaisedbyRejectedValueVsPrimebillingR3", RejRaisedbyRejectedValueVsPrimebillingR3);
                orpt.SetParameterValue("RejRaisedbyCpnCntVs2ndRejR3", RejRaisedbyCpnCntVs2ndRejR3);
                orpt.SetParameterValue("RejRaisedbyRejValueVs2ndRejR3", RejRaisedbyRejValueVs2ndRejR3);
               

                //changes to display search criteria on report
                orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
                orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]); 
                // Take an format id of pdf and excel report 
                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

                // Only shows the padf and export to data option in report and supress other options
                PaxRejectionAnalysisNonSamplingReportI.AllowedExportFormats = exportFormatFlags;
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
                PaxRejectionAnalysisNonSamplingReportI.ReportSource = orpt;

                //Log in into logger
                _logger.Info("PaxRejectionAnalysisNonSamplingReport : Report generation completed!");
            }
            catch (Exception exception)
            {
                _logger.Error("Unexpected Error Has Occurred", exception);
            }
            
        }

        /// <summary>
        /// This method is used to add data to table
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable(List<PaxRejectionAnalysisNonSamplingReportResult> list)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo info in typeof(PaxRejectionAnalysisNonSamplingReportResult).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (PaxRejectionAnalysisNonSamplingReportResult t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(PaxRejectionAnalysisNonSamplingReportResult).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }// End ListToDataTable()
    }
}