using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Web;
using Iata.IS.Business.Reports.CorrespondenceStatus;
using Iata.IS.Core.DI;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Reports.CorrespondenceStatus;
using Iata.IS.Web.Util;
using log4net;

namespace Iata.IS.Web
{
    /// <summary>
    /// This class is used to generate "Correspondence report". Fetch all the values from querystring and then call the sp which return the list of all correpondence
    /// </summary>
    public partial class CorrespondenceReport : System.Web.UI.Page
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Init(object sender, EventArgs e)
        {

            // redirect to Login screen in case unauthorized/anonymous user access 
            // SIS_SCR_REPORT_23_jun-2016_2 :Cross_Site_History_Manipulation
            if (!User.Identity.IsAuthenticated)
            {
                HttpContext.Current.Response.Redirect(AdminSystem.SystemParameters.Instance.General.LogOnURL);
            }


            try
            {


                _logger.Info("Correspondence Report Initiated");

            //Fetch fromdate, todate from querystring
            DateTime fromDate = Convert.ToDateTime(Request.QueryString["fdate"]);
            DateTime toDate = Convert.ToDateTime(Request.QueryString["tdate"]);

                // Fetch Refrence no and initiating member
                Int64 refrenceNo = (Request.QueryString["Refno"]) == string.Empty ? 0 : Int64.Parse(Request.QueryString["Refno"]);
                int initiatingMember = Convert.ToInt32(Request.QueryString["InitMem"]);

                // Fetch frommember and tomember id
                int frommemberId = (Request.QueryString["fmem"]) == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["fmem"]);
                int tommemberId = frommemberId;

                int loginMemberId = SessionUtil.MemberId;

                //Log in into logger
                _logger.Info("Report Initiated");

                // Fetch Correspondence Status, substatus, stage , expiry in days values and charge category
                int isAuthorize = (Request.QueryString["Autho"]) == "false" ? 0 : 1;
                int corrStatus = (Request.QueryString["corrstatus"]) == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["corrstatus"]);
                int corrSubStatus = (Request.QueryString["corrsub"]) == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["corrsub"]);
                int corrStage = (Request.QueryString["corrstage"]) == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["corrstage"]);
                int expiryDays = (Request.QueryString["Expiry"]) == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["Expiry"]);
                int chargecategory = (Request.QueryString["charge"]) == string.Empty ? 0 : Convert.ToInt32(Request.QueryString["charge"]);

                // Check for the category ; i.e. passenger or misc
                string category = Request.QueryString["category"].Trim();//Trimed because space was get added, which causes incorrectly filtered through if conditions
                //CMP526 - Passenger Correspondence Identifiable by Source Code
                
                int sourceCode = string.IsNullOrEmpty(Request.QueryString["SourceCode"])
                                     ? -1
                                     : Convert.ToInt32(Request.QueryString["SourceCode"]);

                //Log in into logger
                _logger.Info("CorrespondenceReport : Business Layer being called for Data!");
                //CMP526 - Passenger Correspondence Identifiable by Source Code
                
                List<CorrespondenceStatusModel> corrStatusModel = Ioc.Resolve<ICorrespondenceStatus>(typeof(ICorrespondenceStatus)).GetCorrespondenceDetails(fromDate, toDate, refrenceNo, initiatingMember, frommemberId, tommemberId, loginMemberId, isAuthorize, corrStatus, 0, corrStage, expiryDays, chargecategory, category, sourceCode);

                List<CorrespondenceStatusModel> corrNewStatusModel = new List<CorrespondenceStatusModel>();


                // Loop through all the variables in list and set status of authority
                for (int i = 0; i < corrStatusModel.Count; i++)
                {
                    corrStatusModel[i].IsAuthorise = corrStatusModel[i].Authority == "0" ? "N" : "Y";
                    //corrStatusModel[i].RefrenceNo = "0" + corrStatusModel[i].RefrenceNo;
                    if (corrStatusModel[i].ExpirryDate == DateTime.MinValue) // Convert.ToDateTime("01/01/0001"))
                    {
                      corrStatusModel[i].ExpirryDate = null; //Convert.ToDateTime("12/30/1899");
                    }

                    corrStatusModel[i].InitiatingEnity = corrStatusModel[i].InitiatingEntityCodeAlpha + "-" +
                                                         corrStatusModel[i].InitiatingEntityCodeNumeric;
                    //+"-" + corrStatusModel[i].InitiatingEntityName;

                    corrStatusModel[i].FromEntity = corrStatusModel[i].FromEntityCodeAlpha + "-" +
                                                    corrStatusModel[i].FromEntityCodeNumeric + "-" +
                                                    corrStatusModel[i].FromEntityname;

                    corrStatusModel[i].ToEntity = corrStatusModel[i].ToEntityCodeAlpha + "-" +
                                                  corrStatusModel[i].ToEntityCodeNumeric + "-" +
                                                  corrStatusModel[i].ToEntityName;

                    if (corrStatusModel[i].FromEntityname != SessionUtil.MemberName && corrStatusModel[i].SubStatus == CorrespondenceSubStatus.Responded.ToString() && (corrStatusModel[i].Stage % 2) == 0)
                        corrStatusModel[i].SubStatus = CorrespondenceSubStatus.Received.ToString();

                    if (corrStatusModel[i].ToEntityName == SessionUtil.MemberName && corrStatusModel[i].SubStatus == CorrespondenceSubStatus.Responded.ToString() && (corrStatusModel[i].Stage % 2) != 0)
                        corrStatusModel[i].SubStatus = CorrespondenceSubStatus.Received.ToString();

                    if (corrStatusModel[i].FromEntityname != SessionUtil.MemberName && corrStatusModel[i].SubStatus.Replace(" ", "") == CorrespondenceSubStatus.ReadyForSubmit.ToString())
                        corrStatusModel[i].SubStatus = CorrespondenceSubStatus.Responded.ToString();

                    if (corrStatusModel[i].FromEntityname != SessionUtil.MemberName && corrStatusModel[i].SubStatus == CorrespondenceSubStatus.Saved.ToString())
                        corrStatusModel[i].SubStatus = CorrespondenceSubStatus.Responded.ToString();


                }//End if
                DataTable dt;

                if (corrSubStatus > 0)
                {
                    if (corrSubStatus == (int)Enum.Parse(typeof(CorrespondenceSubStatus), "Pending", true))
                    {
                        for (int i = 0; i < corrStatusModel.Count; i++)
                        {
                            var c = Enum.Parse(typeof(CorrespondenceSubStatus),
                                               corrStatusModel[i].SubStatus.Replace(" ", ""), true);
                            if ((int)c == (int)Enum.Parse(typeof(CorrespondenceSubStatus), "Saved", true))
                                corrNewStatusModel.Add(corrStatusModel[i]);
                            else if ((int)c == (int)Enum.Parse(typeof(CorrespondenceSubStatus), "ReadyForSubmit", true))
                                corrNewStatusModel.Add(corrStatusModel[i]);
                            else if ((int)c == (int)Enum.Parse(typeof(CorrespondenceSubStatus), "Received", true))
                                corrNewStatusModel.Add(corrStatusModel[i]);

                        }
                    }
                    else
                    {
                        for (int i = 0; i < corrStatusModel.Count; i++)
                        {

                            var c = Enum.Parse(typeof(CorrespondenceSubStatus),
                                               corrStatusModel[i].SubStatus.Replace(" ", ""), true);
                            if ((int)c == corrSubStatus)
                                corrNewStatusModel.Add(corrStatusModel[i]);


                        }
                    }
                    // Take value from list to data table
                    dt = ListToDataTable(corrNewStatusModel);
                }
                else
                {
                    for (int i = 0; i < corrStatusModel.Count; i++)
                    {
                        if (corrStatusModel[i].SubStatus != string.Empty)
                            corrNewStatusModel.Add(corrStatusModel[i]);
                    }
                    // Take value from list to data table
                    dt = ListToDataTable(corrNewStatusModel);
                }

                // Set the string for report type
                string reportPath = "";

                //Log in into logger
                _logger.Info("CorrespondenceReport : Report(rpt) being Called as per category!");

                // Check for the category if category type is pax then generate report of type pax
                if (category == Convert.ToString((int)BillingCategoryType.Pax))
                {
                    // Take a path of report
                    try
                    {
                        reportPath = MapPath("~/Reports/Correspondence/PaxCorrespondenceReport.rpt");
                    }
                    catch (Exception exception)
                    {
                        _logger.Error("Unexpected Error Has Occurred", exception);
                    }
                }// End if

                else if (category == Convert.ToString((int)BillingCategoryType.Cgo))
                {
                    // Take a path of report
                    try
                    {
                        reportPath = MapPath("~/Reports/Correspondence/CGOCorrespondanceStatusReport.rpt");
                    }
                    catch (Exception exception)
                    {
                        _logger.Error("Unexpected Error Has Occurred", exception);
                    }
                }
                else
                {
                    // Take a path of report
                    try
                    {
                        reportPath = MapPath("~/Reports/Correspondence/MiscCorrespondenceReport.rpt");
                    }
                    catch (Exception exception)
                    {
                        _logger.Error("Unexpected Error Has Occurred", exception);
                    }
                }

                // Create an object of report document
                ReportDocument orpt = new ReportDocument();

                //Log in into logger
                _logger.Info("CorrespondenceReport : Report(rpt) being loaded with data!");

                // Load report to that report document
                orpt.Load(reportPath);
                // Added data table to report document

                //changes to display search criteria on report
                //orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
               //following lengthy parameter passing system used because SetParameterValue is not working, for unknown reason
                ParameterField paramField = new ParameterField();
                ParameterFields paramFields = new ParameterFields();
                ParameterDiscreteValue paramDiscreteValue = new ParameterDiscreteValue();

                paramField.Name = "SearchCriteria";
                paramDiscreteValue.Value = Request.QueryString["SearchCriteria"];
                paramField.CurrentValues.Add(paramDiscreteValue);
                paramFields.Add(paramField);

                paramField = new ParameterField();
                paramField.Name = "BrowserDateTime";
                paramDiscreteValue = new ParameterDiscreteValue();
                paramDiscreteValue.Value = Request.QueryString["BrowserDateTime"];
                paramField.CurrentValues.Add(paramDiscreteValue);

                paramFields.Add(paramField);

                orpt.SetDataSource(dt);
                CRCorrespondenceReport.ParameterFieldInfo = paramFields;
                // Take an format id of pdf and excel report 
                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

                // Only shows the padf and export to data option in report and supress other options
                CRCorrespondenceReport.AllowedExportFormats = exportFormatFlags;


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
            CRCorrespondenceReport.ReportSource = orpt;

            }
            catch (Exception exception)
            {
                _logger.Error("Unexpected Error Has Occurred", exception);
            }
           
        }// End Page_Load


        /// <summary>
        /// This method is used to add data to table
        /// </summary>
        /// <param name="list">list of correpondence</param>
        /// <returns></returns>
        public static DataTable ListToDataTable(List<CorrespondenceStatusModel> list)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo info in typeof(CorrespondenceStatusModel).GetProperties())
            {
              dt.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType ));
            }
            foreach (CorrespondenceStatusModel t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(CorrespondenceStatusModel).GetProperties())
                {
                  row[info.Name] = info.GetValue(t, null) ?? DBNull.Value;

                }

                dt.Rows.Add(row);
            }
            return dt;
        }// End ListToDataTable()
    }// End CorrespondenceReport class
}// End namespace
