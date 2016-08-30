using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Iata.IS.Web.Reports.ReceivablesReport;
using Iata.IS.Web.Util;
using Iata.IS.Core.DI;
using Iata.IS.Business.Reports.ReceivablesReport;
using System.Data;
using System.Reflection;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Web;
using log4net;
using SamplAnalysisRec = Iata.IS.Model.Reports.ReceivablesReport.SamplAnalysisRec;


namespace Iata.IS.Web
{
    public partial class OwSampling : System.Web.UI.Page
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Read the values from QueryString into local variables

                int sYear = Convert.ToInt32(Request.QueryString["qsYear"]);

                int sMonth = Convert.ToInt32(Request.QueryString["qsMonth"]);
                int sBillingType = Convert.ToInt32(Request.QueryString["qsBillingType"]);

                //int sBillingEntityCode = Request.QueryString["qsBillingEntityCode"] == string.Empty ? 0 : Convert.ToInt32((Request.QueryString["qsBillingEntityCode"]));
                //int sBilledEntityCode = Request.QueryString["qsBilledEntityCode"] == string.Empty ? 0 : Convert.ToInt32((Request.QueryString["qsBilledEntityCode"]));

                int? sBillingEntityCode;
                int? sBilledEntityCode;

                if (sBillingType == 1)
                {
                    sBilledEntityCode = SessionUtil.MemberId;
                    if ((Request.QueryString["qsBilledEntityCode"]) == string.Empty)
                    {
                        sBillingEntityCode = null;
                    }
                    else
                    {
                        sBillingEntityCode = Convert.ToInt32(Request.QueryString["qsBilledEntityCode"]);
                    }
                }
                else
                {
                    sBillingEntityCode = SessionUtil.MemberId;
                    if ((Request.QueryString["qsBilledEntityCode"]) == string.Empty)
                    {
                        sBilledEntityCode = null;
                    }
                    else
                    {
                        sBilledEntityCode = Convert.ToInt32(Request.QueryString["qsBilledEntityCode"]);
                    }
                }

                int sCurrencyCode = Convert.ToInt32(Request.QueryString["qsCurrencyId"]);


                //Log in into logger
                _logger.Info("OwSampling : Business Layer being called for Data!");

                // call the business method to get all the records from the database

                List<SamplAnalysisRec> corrStatusModel = Ioc.Resolve<IReceivablesReport>
                                                                (typeof(IReceivablesReport)).GetSamplAnalysisRecReportDetails(sBillingType,
                                                                                                                              sMonth,
                                                                                                                              sYear,
                                                                                                                              sBilledEntityCode,
                                                                                                                              sBillingEntityCode,
                                                                                                                              sCurrencyCode);

                ////Iterate through the list 

                //for (int i = 0; i < corrStatusModel.Count; i++)
                //{
                //    // field which have 0 value are removed here for testing, in real scenario this must not happen

                //    if (corrStatusModel[i].NoOfCouponFormD == 0 ||
                //        //corrStatusModel[i].NoOfFCouponRej == 0 ||
                //        //corrStatusModel[i].NoOfXfCouponRej == 0 ||
                //        corrStatusModel[i].FormEAdjAmt == 0 ||
                //        corrStatusModel[i].FormEEvalAmt == 0 ||
                //        //corrStatusModel[i].TotFRejAmt== 0 ||
                //        //corrStatusModel[i].TotXfRejAmt == 0 ||
                //        corrStatusModel[i].TotProvBAmount == 0)
                //    //corrStatusModel[i].TotPrimeCoupons == 0 ||
                //    //corrStatusModel[i].NoOfUafCoupons == 0
                //    {
                //        corrStatusModel.Remove(corrStatusModel[i]);
                //        i--;
                //    }

                //}// End for

                // Take value from list to data table
                DataTable dt = ListToDataTable(corrStatusModel);

                // Set the string for report type
                string reportPath = "";
                try
                {
                    reportPath = MapPath("~/Reports/ReceivablesReport/OwSamplingRec.rpt");
                }
                catch (Exception exception)
                {
                    _logger.Error("Unexpected Error Has Occurred", exception);
                }

                // Set ReportHeader
                string reportHeaderfield = string.Empty;
                string billingMonthHeading = string.Empty;
                string entityCodeHeading = string.Empty;
                string entityNameHeading = string.Empty;
                if (sBillingType == 1)
                {
                    reportHeaderfield = "Passenger Sampling Billing Analysis - Payables";
                    billingMonthHeading = "Inward Provisional Billing Month";
                    entityCodeHeading = "Billing Member Code";
                    entityNameHeading = "Billing Member Name";
                }
                else if (sBillingType == 2)
                {
                    reportHeaderfield = "Passenger Sampling Billing Analysis - Receivables ";
                    billingMonthHeading = "Outward Provisional Billing Month";
                    entityCodeHeading = "Billed Member Code";
                    entityNameHeading = "Billed Member Name";
                }

                // Create an object of report document
                ReportDocument orpt = new ReportDocument();

                //Log in into logger
                _logger.Info("OwSampling : Report(rpt) being loaded with data!");

                // Load report to that report document
                orpt.Load(reportPath);

                // Added data table to report document
                orpt.SetDataSource(dt);

                // Add report columnheader values
                orpt.SetParameterValue("ReportHeading", reportHeaderfield);
                orpt.SetParameterValue("BillingMonthHeading", billingMonthHeading);
                orpt.SetParameterValue("EntityCodeHeading", entityCodeHeading);
                orpt.SetParameterValue("EntityNameHeading", entityNameHeading);

                //changes to display search criteria on report
                orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
                orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]); 
                // Take an format id of pdf and excel report 
                int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);
                // Only shows the padf and export to data option in report and supress other options
                PassengerSamplingBillingAnalysis.AllowedExportFormats = exportFormatFlags;

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
                PassengerSamplingBillingAnalysis.ReportSource = orpt;

                //Log in into logger
                _logger.Info("OwSampling : Report generation completed!");
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
        public static DataTable ListToDataTable(List<SamplAnalysisRec> list)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo info in typeof(SamplAnalysisRec).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (SamplAnalysisRec t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(SamplAnalysisRec).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }// End ListToDataTable()

        public static ReportDocument ReportSource { get; set; }
    }
}