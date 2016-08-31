using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core.DI;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;
using Iata.IS.Model.Reports.ValidationDetails;
using Iata.IS.Business.Reports.ValidationDetails;
using System.Data;
using System.Reflection;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using log4net;

namespace Iata.IS.Web
{
    public partial class ValidationDetailReport : System.Web.UI.Page
    {
        public readonly IMemberManager _imemberManager;
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public readonly IValidationDetails _validationDetails;
        public ValidationDetailReport()
        {
            _imemberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
            _validationDetails = Ioc.Resolve<IValidationDetails>(typeof(IValidationDetails));
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            List<ValidationDetailsModel> listModel = null;

            // redirect to Login screen in case unauthorized/anonymous user access 
            if (SessionUtil.UserId == 0)
            {
                HttpContext.Current.Response.Redirect(AdminSystem.SystemParameters.Instance.General.LogOnURL);
            }


            try
            {
                _logger.Info("Validation Detail Report Initiated");

                int fromDateOrPeriod;
                DateTime fromFileSubdate;
                string fileName;
                string reportPath = string.Empty;
                string defaultdatetime = "01-01-0001";
                fromDateOrPeriod = Convert.ToInt32(Request.QueryString["fdate"]);

                fromFileSubdate = (Request.QueryString["fSubmissionDate"]) == string.Empty ? Convert.ToDateTime(defaultdatetime) : Convert.ToDateTime(Request.QueryString["fSubmissionDate"]);


                string category = Request.QueryString["category"];

                string BilledOrganisation;
                string InvoiceNo;

                int clearanceMonth = Convert.ToInt32((Request.QueryString["tbmonth"]) + (Request.QueryString["fbmonth"]));

                if (string.IsNullOrEmpty(Request.QueryString["fname"]))
                {
                    fileName = string.Empty;
                }
                else
                {
                    fileName = Request.QueryString["fname"];
                }

                if (string.IsNullOrEmpty(Request.QueryString["BilledOrg"]))
                {
                    BilledOrganisation = string.Empty;
                }
                else
                {
                    BilledOrganisation = Request.QueryString["BilledOrg"];
                }

                if (string.IsNullOrEmpty(Request.QueryString["InvoiceNo"]))
                {
                    InvoiceNo = string.Empty;
                }
                else
                {
                    InvoiceNo = Request.QueryString["InvoiceNo"];
                }

                string memberCode = _imemberManager.GetMemberCode(SessionUtil.MemberId);

                reportPath = MapPath("~/Reports/ValidationDetails/ValidationDetails.rpt");

                int memberId = SessionUtil.MemberId;

                //listModel = Ioc.Resolve<IValidationDetails>(typeof(IValidationDetails)).GetValidationDetails(fromBillingMonth, toBillingMonth, fromDateOrPeriod, toDateOrPeriod, fileName, fromFileSubdate, toFileSubdate, BilledOrganisation, InvoiceNo, category, memberCode);
                listModel = _validationDetails.GetValidationDetails(clearanceMonth, fromDateOrPeriod, fileName, fromFileSubdate, BilledOrganisation, InvoiceNo, category, memberCode);

                int index = 1;

                for (int i = 0; i < listModel.Count; i++)
                {
                    listModel[i].SerialNo = index;
                    string ClearenceYear = Convert.ToString(listModel[i].ClearanceMonth).Substring(0, 4);
                    string ClearenceMonth = Convert.ToString(listModel[i].ClearanceMonth).Substring(4);

                    listModel[i].Clearance = Convert.ToDateTime(ClearenceYear + "-" + ClearenceMonth);
                    listModel[i].Doc = listModel[i].DocNo;
                    listModel[i].LinkedDoc = listModel[i].LinkedDoc;

                    if (listModel[i].BillingFileSubmissionDate == Convert.ToDateTime("01/01/0001"))
                    {
                        listModel[i].BillingFileSubmissionDate = Convert.ToDateTime("12/30/1899");
                    }
                    index++;
                }

                DataTable dt = ListToDataTable(listModel);

                ReportDocument orpt = new ReportDocument();
                orpt.Load(reportPath);
                orpt.SetDataSource(dt);

                orpt.SummaryInfo.ReportTitle = "IS Validation Details";


                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);

                CRValidationDetail.AllowedExportFormats = exportFormatFlags;

                Iata.IS.Web.Reports.ValidationSummary.ValidationSummary rpt = new Iata.IS.Web.Reports.ValidationSummary.ValidationSummary();

                if (category == Enum.GetName(typeof(BillingCategoryType), (int)BillingCategoryType.Pax))
                {
                    ((TextObject)orpt.ReportDefinition.Sections["Section2"].ReportObjects["Text12"]).Text = "PAX Billing Code";
                    ((TextObject)orpt.ReportDefinition.Sections["Section2"].ReportObjects["Text14"]).Text = "Pax Batch Number";
                    (((TextObject)orpt.ReportDefinition.Sections["Section2"].ReportObjects["Text15"]).Text) = "Pax Seq Number";
                }
                else
                {
                    ((TextObject)orpt.ReportDefinition.Sections["Section2"].ReportObjects["Text12"]).Text = "MISC Charge Category";
                    ((TextObject)orpt.ReportDefinition.Sections["Section2"].ReportObjects["Text14"]).Text = "Misc Line Item No";
                    ((TextObject)orpt.ReportDefinition.Sections["Section2"].ReportObjects["Text15"]).Text = "Misc Line Item Detail No";
                }
                CRValidationDetail.ReportSource = orpt;
            }
            catch (Exception exception)
            {

                _logger.Error("Unexpected Error Has Occurred", exception);
            }

        }

        public static DataTable ListToDataTable(List<ValidationDetailsModel> list)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo info in typeof(ValidationDetailsModel).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (ValidationDetailsModel t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(ValidationDetailsModel).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}