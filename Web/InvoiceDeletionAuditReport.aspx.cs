using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using Iata.IS.Business.Pax;
using Iata.IS.Core.DI;
using Iata.IS.Model.Pax;
using Iata.IS.Web.Util;
using log4net;
namespace Iata.IS.Web
{
    public partial class InvoiceDeletionAuditReport : System.Web.UI.Page
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public readonly IInvoiceManager InvoiceManagerBase;

        public InvoiceDeletionAuditReport()
        {
            InvoiceManagerBase = Ioc.Resolve<IInvoiceManager>(typeof(IInvoiceManager));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string reportPath = string.Empty;

                var auditDeletedInvoice = new AuditDeletedInvoice();

                if (!string.IsNullOrEmpty(Request.QueryString["BillingCat"]))
                {
                    auditDeletedInvoice.BillingCategoryId = Convert.ToInt32(Request.QueryString["BillingCat"]);
                }
                if (!string.IsNullOrEmpty(Request.QueryString["Year"]))
                {
                    auditDeletedInvoice.BillingYear = Convert.ToInt32(Request.QueryString["Year"]);
                }
                if (!string.IsNullOrEmpty(Request.QueryString["Month"]))
                {
                    auditDeletedInvoice.BillingMonth = Convert.ToInt32(Request.QueryString["Month"]);
                }
                if (!string.IsNullOrEmpty(Request.QueryString["Period"]))
                {
                    auditDeletedInvoice.BillingPeriod = Convert.ToInt32(Request.QueryString["Period"]);
                }
                if (!string.IsNullOrEmpty(Request.QueryString["MemId"]))
                {
                    auditDeletedInvoice.BilledMemberId = Convert.ToInt32(Request.QueryString["MemId"]);
                }
                if (!string.IsNullOrEmpty(Request.QueryString["Invoice"]))
                {
                    auditDeletedInvoice.InvoiceNo = Request.QueryString["Invoice"];
                }
                if (!string.IsNullOrEmpty(Request.QueryString["DateFrom"]))
                {
                    DateTime oDate = DateTime.Parse(Request.QueryString["DateFrom"].ToString());
                    auditDeletedInvoice.DeletionDateFrom = oDate.ToString("MM/dd/yyyy");
                }
                if (!string.IsNullOrEmpty(Request.QueryString["DateTo"]))
                {
                    DateTime oDate = DateTime.Parse(Request.QueryString["DateTo"].ToString());

                    auditDeletedInvoice.DeletionDateTo = oDate.ToString("MM/dd/yyyy");
                }
                if (!string.IsNullOrEmpty(Request.QueryString["DeletedBy"]))
                {
                    
                    auditDeletedInvoice.DeletedBy = Request.QueryString["DeletedBy"];
                }

                auditDeletedInvoice.BillingMemberId = SessionUtil.MemberId;

                //Log in into logger
                _logger.Info("MemberLocationReport : Business Layer being called for Data!");

                
                try
                {
                    reportPath = MapPath("~/Reports/DeletedInvoicesAudit/DeletedInvoicesAudit.rpt");
                }
                catch (Exception exception)
                {
                    _logger.Error("Unexpected Error Has Occurred", exception);
                }
                DataTable dt = ListToDataTable(InvoiceManagerBase.GetInvoiceDeletionAuditDetails(auditDeletedInvoice));
                var orpt = new ReportDocument();


                //Log in into logger
                _logger.Info("MemberLocationReport : Report(rpt) being loaded with data!");

                orpt.Load(reportPath);

                orpt.SetDataSource(dt);
                //changes to display search criteria on report
                orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
                orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
                // Take an format id of pdf and excel report 
                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

                // Only shows the padf and export to data option in report and supress other options
                CrystalReportViewer.AllowedExportFormats = exportFormatFlags;
                CrystalReportViewer.ReportSource = orpt;

                //Log in into logger
                _logger.Info("MemberLocationReport : Report generation completed!");

            }
            catch (Exception exception)
            {
                _logger.Error("Unexpected Error Has Occurred", exception);
            }

        }

        private static DataTable ListToDataTable(List<Model.Pax.InvoiceDeletionAuditReport> list)
        {
            var dt = new DataTable();
            foreach (PropertyInfo info in typeof(Model.Pax.InvoiceDeletionAuditReport).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (Model.Pax.InvoiceDeletionAuditReport t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(Model.Pax.InvoiceDeletionAuditReport).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }

    }
}