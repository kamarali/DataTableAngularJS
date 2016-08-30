using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Reports.ValidationSummary;
using Iata.IS.Core.DI;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
//using Iata.IS.Web.Reports.AuditTrail;
using Iata.IS.Model.Reports;
using Iata.IS.Web.Util;
using iPayables.UserManagement;
using Iata.IS.Model.Base;
using Iata.IS.Business.Reports;
using System.Web;
using log4net;

namespace Iata.IS.Web
{
    public partial class SuspendedInvoiceReport : System.Web.UI.Page
    {
        private readonly IManageSuspendedInvoicesManager _manageSuspendedInvoices;

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SuspendedInvoiceReport()
        {
            _manageSuspendedInvoices = Ioc.Resolve<IManageSuspendedInvoicesManager>(typeof(IManageSuspendedInvoicesManager));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // redirect to Login screen in case unauthorized/anonymous user access 
            // SIS_SCR_REPORT_23_jun-2016_2 :Cross_Site_History_Manipulation
            if (!User.Identity.IsAuthenticated)
            {
                HttpContext.Current.Response.Redirect(AdminSystem.SystemParameters.Instance.General.LogOnURL);
            }


            try
            {
                //Log in into logger 	
                _logger.Info("Report Initiated");

                ReportDocument orpt = new ReportDocument();
                List<MemberSuspendedInvoices> listModel = null;
                Int32 fromClearanceYear = -1;
                Int32 toClearanceYear = -1;

                Int32 fromClearanceMonth = -1;
                Int32 toClearanceMonth = -1;

                Int32 fromClearancePeriod = -1;
                Int32 toClearancePeriod = -1;

                Int32 SettlementMethodId = -1;
                Int32 suspendedEntityCode = 0;
                Int32 BillingCategoryId = -1;
                Int32 iataMemberId = 0;
                Int32 achMemberId = 0;

                string reportPath = string.Empty;
                if (!String.IsNullOrEmpty(Request.QueryString["FromBillingYear"]))
                    fromClearanceYear = Convert.ToInt32(Request.QueryString["FromBillingYear"]);
                if (!String.IsNullOrEmpty(Request.QueryString["ToBillingYear"]))
                    toClearanceYear = Convert.ToInt32(Request.QueryString["ToBillingYear"]);
                if (!String.IsNullOrEmpty(Request.QueryString["FromBillingMonth"]))
                    fromClearanceMonth = Convert.ToInt32(Request.QueryString["FromBillingMonth"]);
                if (!String.IsNullOrEmpty(Request.QueryString["ToBillingMonth"]))
                    toClearanceMonth = Convert.ToInt32(Request.QueryString["ToBillingMonth"]);
                if (!String.IsNullOrEmpty(Request.QueryString["FromBillingPeriod"]))
                    fromClearancePeriod = Convert.ToInt32(Request.QueryString["FromBillingPeriod"]);
                if (!String.IsNullOrEmpty(Request.QueryString["ToBillingPeriod"]))
                    toClearancePeriod = Convert.ToInt32(Request.QueryString["ToBillingPeriod"]);
                if (!String.IsNullOrEmpty(Request.QueryString["SettlementMethodId"]))
                    SettlementMethodId = Convert.ToInt32(Request.QueryString["SettlementMethodId"]);
                if (!String.IsNullOrEmpty(Request.QueryString["BillingCategoryId"]))
                    BillingCategoryId = Convert.ToInt32(Request.QueryString["BillingCategoryId"]);
                if (!String.IsNullOrEmpty(Request.QueryString["SuspendedEntityCode"]))
                    suspendedEntityCode = Convert.ToInt32(Request.QueryString["SuspendedEntityCode"]);
                if (!String.IsNullOrEmpty(Request.QueryString["IATAMemberId"]))
                    iataMemberId = Convert.ToInt32(Request.QueryString["IATAMemberId"]);
                if (!String.IsNullOrEmpty(Request.QueryString["ACHMemberId"]))
                    achMemberId = Convert.ToInt32(Request.QueryString["ACHMemberId"]);

                if (fromClearanceMonth == -1)
                    fromClearanceMonth = 0;
                if (fromClearancePeriod == -1)
                    fromClearancePeriod = 0;
                if (toClearanceMonth == -1)
                    toClearanceMonth = 12;
                if (toClearancePeriod == -1)
                    toClearancePeriod = 4;
                try
                {
                    reportPath = MapPath("~/Reports/SuspendedInvoice/SuspendedInvoices.rpt");
                }
                catch (Exception exception)
                {
                    _logger.Error("Unexpected Error Has Occurred", exception);
                }

                int billingMemberId = SessionUtil.MemberId;

                //Log in into logger
                _logger.Info("SuspendedInvoiceReport : Business Layer being called for Data!");

                listModel =
                    Ioc.Resolve<IManageSuspendedInvoicesManager>(typeof(IManageSuspendedInvoicesManager)).
                        GetMemberSuspendedInvoicesList(billingMemberId, fromClearanceYear, fromClearanceMonth,
                                                       fromClearancePeriod, toClearanceYear, toClearanceMonth,
                                                       toClearancePeriod, SettlementMethodId, BillingCategoryId,
                                                       suspendedEntityCode, iataMemberId, achMemberId);

                ////Parameter to display Report Generated date time in UTC format
                //ParameterFields paramFields = new ParameterFields();
                //ParameterField pfReportGeneratedDate = new ParameterField();
                //pfReportGeneratedDate.ParameterFieldName = "ReportGeneratedDate";
                //    //ReportGeneratedDate is Crystal Report Parameter name.
                //ParameterDiscreteValue dcReportGeneratedDate = new ParameterDiscreteValue();
                ////Set Value to parameter
                //dcReportGeneratedDate.Value = string.Format("{0} {1}",
                //                                            DateTime.UtcNow.ToString("dd-MMM-yyyy hh:mm:ss tt"), "UTC");

                //pfReportGeneratedDate.CurrentValues.Add(dcReportGeneratedDate);
                ////Add parameter in ParameterFields
                //paramFields.Add(pfReportGeneratedDate);
                ////Assign ParameterFields to Crystal report viewer
                //CRViewer.ParameterFieldInfo = paramFields;

                //int exportFormatFlags = (int)(ViewerExportFormats.ExcelRecordFormat | ViewerExportFormats.PdfFormat);
                //CRViewer.AllowedExportFormats = exportFormatFlags; 

                //Log in into logger
                _logger.Info("SuspendedInvoiceReport : Report(rpt) being loaded with data!");

                orpt.Load(reportPath);
                orpt.SetDataSource(listModel);
                //change to display search criteria on report
                orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
                orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
                // Take an format id of pdf and excel report 
                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

                // Only shows the padf and export to data option in report and supress other options
                CRViewer.AllowedExportFormats = exportFormatFlags;
                ExcelDataOnlyFormatOptions optionsformat = CrystalDecisions.Shared.ExportOptions.CreateDataOnlyExcelFormatOptions();
                optionsformat.MaintainColumnAlignment = true;
                optionsformat.MaintainRelativeObjectPosition = true;
                optionsformat.ShowGroupOutlines = true;
                optionsformat.ExportObjectFormatting = true;
                optionsformat.ExportPageHeaderAndPageFooter = true;
                optionsformat.SimplifyPageHeaders = true;
                optionsformat.ExcelConstantColumnWidth = 1000;

                orpt.ExportOptions.FormatOptions = optionsformat;
                
                CRViewer.ReportSource = orpt;

                //Log in into logger
                _logger.Info("SuspendedInvoiceReport : Report generation completed!");
            }
            catch (Exception exception)
            {
                _logger.Error("Unexpected Error Has Occurred", exception);
            }
            
        }
    }
}
