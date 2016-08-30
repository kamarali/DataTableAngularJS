using System;
using System.Collections.Generic;
using System.Data;
using Iata.IS.Business.Reports.UserPermission;
using Iata.IS.Core.DI;
using CrystalDecisions.CrystalReports.Engine;
using System.Reflection;
using CrystalDecisions.Shared;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Web.Util;
using log4net;

namespace Iata.IS.Web
{
    public partial class UserPermissionReport : System.Web.UI.Page
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string UsercategoryId = "UserCatId";
        private const string MemberId = "MemId";
        private const string EmailId = "Email";

        private const string UserName = "User Name";
        private const string IsActive = "Is Active";
        private const string IsSuperUser = "Is Super User";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Set the string for report type
            var reportPath = string.Empty;
            // List of Header Values
            var headerValue = new List<string>();

            //dt original Column Count
            var dtCount = 0;

            try
            {
              var categoryId = 0;
              var memberId = 0;
              string userName = null;

              switch (SessionUtil.UserCategory)
              {
                case UserCategory.SisOps:
                  categoryId = Request.QueryString[UsercategoryId] == string.Empty ? 0 : Convert.ToInt32(Request.QueryString[UsercategoryId]);
                  memberId = Request.QueryString[MemberId] == string.Empty? 0: Convert.ToInt32(Request.QueryString[MemberId]);;
                  userName = Request.QueryString[EmailId];
                  break;
                case UserCategory.IchOps:
                  categoryId = 2;
                  memberId = 0;
                  userName = Request.QueryString[EmailId];
                  break;
                case UserCategory.AchOps:
                  categoryId = 3;
                  memberId = 0;
                  userName = Request.QueryString[EmailId];
                  break;
                case UserCategory.Member:
                  categoryId = 4;
                  memberId = SessionUtil.MemberId;
                  userName = Request.QueryString[EmailId];
                  break;
              }
              // Set rootpath of report 
              switch (categoryId)
              {
                  case 1:
                      reportPath = MapPath("~/Reports/UserPermissionReport/SISOpsPermissionReport.rpt");
                      break;
                  case 2:
                      reportPath = MapPath("~/Reports/UserPermissionReport/ICHOpsPermissionReport.rpt");
                      break;
                  case 3:
                      reportPath = MapPath("~/Reports/UserPermissionReport/ACHOpsPermissionReport.rpt");
                      break;
                  case 4:
                      reportPath = MapPath("~/Reports/UserPermissionReport/MemberPermissionReport.rpt");
                      break;
              }

                // Call Bussiness Layer to Get Data
                var dt = Ioc.Resolve<IUserPermissionReport>(typeof(IUserPermissionReport)).GetUserPermissionReport(categoryId, memberId, userName);
              
                //set dt original Column count here 
                dtCount = dt.Columns.Count;

                //Get Header Values here
                headerValue.Add(UserName);
                headerValue.Add(IsActive);
                headerValue.Add(IsSuperUser);

                //Get Column Header Name from First row data of datatable
                foreach (DataColumn column in dt.Columns)
                {
                    var cName = dt.Rows[0][column.ColumnName].ToString();
                    if (!dt.Columns.Contains(cName) && !string.IsNullOrWhiteSpace(cName))
                    {
                        headerValue.Add(cName);
                    }
                }
              
                // Delete First Row from DataTable
                dt.Rows[0].Delete();
                dt.AcceptChanges();

                //Log in into logger
                _logger.Info("User Permission  Report : Report(rpt) being loaded with data!");

                // Make an object of report document
                var orpt = new ReportDocument();

                // add the path of the report
                orpt.Load(reportPath);
                var paramField = orpt.DataDefinition.ParameterFields.Count - 2;
                //Bind Data Value here
                for (var i = 1; i <= paramField; i++)
                {
                  if (i > dtCount)
                  {
                    dt.Columns.Add("COL" + i);
                    dt.AcceptChanges();
                  }
                }
                
                // Add data source
                orpt.SetDataSource(dt);

                //Bind Header Values here
                for (var i = 1; i <= paramField; i++)
                { // Update report columnheader values
                  orpt.SetParameterValue("p" + i, i <= dtCount ? headerValue[i - 1] : string.Format("{0} ", " "));
                }
               
               const int exportFormatFlags =(int)(ViewerExportFormats.PdfFormat | ViewerExportFormats.ExcelRecordFormat);

                // Only shows the padf and export to data option in report and supress other options
                CRUserPermissionReport.AllowedExportFormats = exportFormatFlags;
                ExcelDataOnlyFormatOptions optionsformat = ExportOptions.CreateDataOnlyExcelFormatOptions();
                optionsformat.MaintainColumnAlignment = true;
                optionsformat.MaintainRelativeObjectPosition = true;
                optionsformat.ShowGroupOutlines = true;
                optionsformat.ExportObjectFormatting = true;
                optionsformat.ExportPageHeaderAndPageFooter = true;
                optionsformat.SimplifyPageHeaders = true;
                optionsformat.ExcelConstantColumnWidth = 1000;

                orpt.ExportOptions.FormatOptions = optionsformat;
                //changes to display search criteria on report
                orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
                orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
                // set report source of User Permission Report
                CRUserPermissionReport.ReportSource = orpt;
                //Log in into logger
                _logger.Info("User Permission Report : Report generation completed!");
            }

            catch (Exception exception)
            {
                _logger.Error("Unexpected Error Has Occurred", exception);
            }

        }

    }
}