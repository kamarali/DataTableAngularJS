using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Iata.IS.Business.Reports.Cargo.RejectionAnalysis;
using Iata.IS.Model.Reports.Cargo.RejectionAnalysis;
using Iata.IS.Core.DI;
using Iata.IS.Web.Util;
using System.Data;
using log4net;


namespace Iata.IS.Web
{
    public partial class CgoRejectionAnalysisRecReport : System.Web.UI.Page
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {               

                // fetch all values from the query strimg and stored into the variables
                //CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
                //input parameter updated (From Year Month and To Year Month)
                int fromMonth = Convert.ToInt32(Request.QueryString["bFromMonth"]);
                int fromYear = Convert.ToInt32(Request.QueryString["bFromYear"]);
                int toMonth = Convert.ToInt32(Request.QueryString["bToMonth"]);
                int toYear = Convert.ToInt32(Request.QueryString["bToYear"]);

                int againstEntityId = Request.QueryString["againstEntityId"] == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["againstEntityId"]);
                int currencyCode = Request.QueryString["currencyCode"] == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["currencyCode"]);
                int isPayableReport = Convert.ToInt32(Request.QueryString["IsPayableReport"]);
                int logInMemberid = SessionUtil.MemberId;

                // Set the string for report type
                string reportPath = string.Empty;
                string reportHeaderName = string.Empty;
                string memberCodeHeader = string.Empty;
                string memberNameHeader = string.Empty;
                bool isCantainsZeroValue = false;

                //Log in into Report initialization
                _logger.Info("Report Initiated");
                try
                {
                    reportPath = MapPath("~/Reports/Cargo/RejectionAnalysis/CgoRejectionAnalysisReport.rpt");
                }
                catch (Exception exception)
                {
                    _logger.Error("Unexpected Error Has Occurred", exception);
                }
                // Set rootpath of report
                if (isPayableReport == 0)
                {
                    reportHeaderName = "Cargo Rejection Analysis - Receivables";
                    memberCodeHeader = "Billed Member Code";
                    memberNameHeader = "Billed Member Name";
                }
                else if (isPayableReport == 1)
                {
                    reportHeaderName = "Cargo Rejection Analysis - Payables";
                    memberCodeHeader = "Billing Member Code";
                    memberNameHeader = "Billing Member Name";
                }
                // Make an object of CgoRejectionAnalysisRecModel model class
                List<CgoRejectionAnalysisRecModel> listModel;

                //Log in into logger
                _logger.Info("CgoRejectionAnalysisRecReport : Business Layer being called for Data!");

                // <!--CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports-->
                // call the business method to get all the records from the db
                listModel = Ioc.Resolve<ICgoRejAnalysisRec>(typeof(ICgoRejAnalysisRec)).GetCgoRejectionAnalysisRec(fromMonth, fromYear,toMonth,toYear, logInMemberid, againstEntityId, currencyCode, isPayableReport);


                //  Iterate through the list 
                //for (int i = 0; i < listModel.Count; i++)
                //{
                //    // field which have 0 value are removed here for testing, in real scenario this must not happen
                //    if (listModel[i].CcAwbAmount == 0 ||
                //        listModel[i].CcAwbCount == 0 ||
                //        listModel[i].PpAwbAmount == 0 ||
                //        listModel[i].PpAwbCount == 0)
                //    {
                //        isCantainsZeroValue = true;
                //        listModel.Remove(listModel[i]);
                //        i--;
                //    }
                     
                //}// End for

                //if (!isCantainsZeroValue)
                //{
                //    ErrorMessage.Visible = false;
                //    CRCgoRejectionAnalysisRec.Visible = true;

                    // Add the data from list to data table
                    DataTable dt = ListToDataTable(listModel);

                    // make an object of report document
                    ReportDocument orpt = new ReportDocument();

                    //Log in into logger
                    _logger.Info("CgoRejectionAnalysisRecReport : Report(rpt) being loaded with data!");

                    // add the path of the report
                    orpt.Load(reportPath);
                    // Add data source
                    orpt.SetDataSource(dt);

                    orpt.SetParameterValue("ReportHeader", reportHeaderName);
                    orpt.SetParameterValue("MemberCodeHeader", memberCodeHeader);
                    orpt.SetParameterValue("MemberNameHeader", memberNameHeader);
                    //changes to display search criteria on report
                    orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
                    orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
                    // Take an format id of pdf and excel report 
                    int exportFormatFlags =
                        (int)
                        (CrystalDecisions.Shared.ViewerExportFormats.PdfFormat |
                         CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

                    // Only shows the padf and export to data option in report and supress other options
                    CRCgoRejectionAnalysisRec.AllowedExportFormats = exportFormatFlags;

                    ExcelDataOnlyFormatOptions optionsformat =
                        CrystalDecisions.Shared.ExportOptions.CreateDataOnlyExcelFormatOptions();
                    optionsformat.MaintainColumnAlignment = true;
                    optionsformat.MaintainRelativeObjectPosition = true;
                    optionsformat.ShowGroupOutlines = true;
                    optionsformat.ExportObjectFormatting = true;
                    optionsformat.ExportPageHeaderAndPageFooter = true;
                    optionsformat.SimplifyPageHeaders = true;
                    optionsformat.ExcelConstantColumnWidth = 1000;

                    orpt.ExportOptions.FormatOptions = optionsformat;

                    CRCgoRejectionAnalysisRec.ReportSource = orpt;
                //}
                //else
                //{
                //    ErrorMessage.Visible = true;
                //    CRCgoRejectionAnalysisRec.Visible = false;
                //    ErrorMessage.Text = "There is some zero values in data which causing issue in calculation in report data!";
                //}
            }
            catch (Exception exception)
            {
                _logger.Error("Unexpected Error Has Occurred", exception);    
            }
            
        }

        public static DataTable ListToDataTable(List<CgoRejectionAnalysisRecModel> list)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo info in typeof(CgoRejectionAnalysisRecModel).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (CgoRejectionAnalysisRecModel t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(CgoRejectionAnalysisRecModel).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }
 
    }
}