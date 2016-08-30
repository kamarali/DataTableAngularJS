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
using Iata.IS.Business.Reports.MisMatchDoc;
using Iata.IS.Core.DI;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;
using log4net;

namespace Iata.IS.Web
{
    public partial class CargoSupportingMismatchDoc : System.Web.UI.Page
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
  
        protected void Page_Load(object sender, EventArgs e)
        {
            //Log in into logger
            Logger.Info("Cargo Supporting Mismatch Doc : aspx page Being Load!");

            try
            {
                // fetch all values from the query strimg and stored into the variables
                int billingMonth = Convert.ToInt32(Request.QueryString["bmonth"]);
                int billingPeriod = Convert.ToInt32(Request.QueryString["bPeriod"]);
                int billingYear = Convert.ToInt32(Request.QueryString["bYear"]);
                int airlinceCode = Request.QueryString["AirCode"] == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["AirCode"]);
                int settlementMethod = Convert.ToInt32(Request.QueryString["SetelmentMethodId"]) == -1 ? 0 : Convert.ToInt32(Request.QueryString["SetelmentMethodId"]);
                string invoiceNo = Request.QueryString["InNo"];
                int logInMenberid = SessionUtil.MemberId;

                // Set the string for report type
                string reportPath = "";

                // Check for the category ; i.e. passenger or misc
                string category = Request.QueryString["category"];

                // Set rootpath of report
                try
                {
                    reportPath = MapPath("~/Reports/MisMatch/CargoSupportingMismatchDocReport.rpt");
                }
                catch (Exception exception)
                {
                    Logger.Error("Unexpected Error Has Occurred", exception);
                }
                // Make an object of SupportingMismatchDoc model class
                List<Iata.IS.Model.Reports.SupportingMismatchDocument.SupportingMismatchDoc> listModel;

                //Log in into logger
                Logger.Info("Cargo Supporting Mismatch Doc : Business Layer being called for Data!");

                // call the business method to get all the records from the db
                listModel = Ioc.Resolve<IMismatchDoc>(typeof(IMismatchDoc)).GetCgoMismatchDoc(billingMonth, billingPeriod, billingYear, airlinceCode, settlementMethod, 2, invoiceNo, logInMenberid);


                //Log in into logger
                Logger.Info("Cargo Supporting Mismatch Doc : Business Layer returns Data!");

                //Iterate through the list and set attachment indicator
                for (int i = 0; i < listModel.Count; i++)
                {
                    if (listModel[i].Attachment == 0)
                    {
                        listModel[i].AttachmentIndicator = "N";
                    }// End if
                    else if (listModel[i].Attachment == 1)
                    {
                        listModel[i].AttachmentIndicator = "Y";
                    }// End else If

                }// End for

                // Add the data from list to data table
                DataTable dt = ListToDataTable(listModel);

                // make an object of report document
                ReportDocument orpt = new ReportDocument();

                //Log in into logger
                Logger.Info("Cargo Supporting Mismatch Doc : Report(rpt) being loaded!");

                // add the path of the report
                orpt.Load(reportPath);

                //Log in into logger
                Logger.Info("Cargo Supporting Mismatch Doc : Report(rpt) being loaded with data!");
                // Add data source
                orpt.SetDataSource(dt);

                //changes to display search criteria on report
                orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
                orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
                // Take an format id of pdf and excel report 
                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

                // Only shows the padf and export to data option in report and supress other options
                CRSupportingMisMatch.AllowedExportFormats = exportFormatFlags;

                CRSupportingMisMatch.ReportSource = orpt;

                //Log in into logger
                Logger.Info("Cargo Supporting Mismatch Doc : Report generation completed!");
            }
            catch (Exception ex)
            {

                //Log in into logger
                Logger.Error("Cargo Supporting Mismatch Doc :Error--->",ex);
            }
            
        } // end of page load

        public static DataTable ListToDataTable(List<Iata.IS.Model.Reports.SupportingMismatchDocument.SupportingMismatchDoc> list)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo info in typeof(Iata.IS.Model.Reports.SupportingMismatchDocument.SupportingMismatchDoc).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (Iata.IS.Model.Reports.SupportingMismatchDocument.SupportingMismatchDoc t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(Iata.IS.Model.Reports.SupportingMismatchDocument.SupportingMismatchDoc).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }
    }// end SupportingMismatchDoc class
 }