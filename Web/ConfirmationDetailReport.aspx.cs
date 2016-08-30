using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.File;
using Iata.IS.Web.Util;
using Iata.IS.Model.Reports.ConfirmationDetails;
using Iata.IS.Business.Reports.ConfirmationDetail;
using System.Data;
using System.Reflection;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using log4net;

namespace Iata.IS.Web
{
  public partial class ConfirmationDetailReport : System.Web.UI.Page
  {
    public readonly IMemberManager _imemberManager;
    public readonly IConfirmationDetail _confirmationDetail;
    public readonly int _pagesize;
    private const string _reportUrl = "~/Reports/ConfirmationDetails/ConfirmationDetail.rpt";
    private string reportPath = string.Empty;

    int clearanceMonth = -1;
    int periodNo = 0;
    int biligAirlineNo = -1;
    int biledAirlineNo = -1;
    string invoiceNo = string.Empty;
    string issuAirline = string.Empty;
    string originalPMI = string.Empty;
    string validatedPMI = string.Empty;
    string agrmntIndiSupplied = string.Empty;
    string agrmntIndiValidated = string.Empty;
    string atpcoReasCd = string.Empty;
    int memberId;
    private int billingYear;

    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public ConfirmationDetailReport()
    {
      _imemberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
      _confirmationDetail = Ioc.Resolve<IConfirmationDetail>(typeof(IConfirmationDetail));
      _pagesize = 37;
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
        _logger.Info("Confirmation Detail Report Initiated");

        //if (!IsPostBack)
        //{

          List<ConfirmationDetailModel> listModel = null;

          GetQueryStringValues();

          reportPath = MapPath(_reportUrl);

          listModel = _confirmationDetail.GetConfirmationDetails(memberId, clearanceMonth, periodNo, biligAirlineNo,
                                                                 biledAirlineNo, invoiceNo, issuAirline, originalPMI,
                                                                 validatedPMI, agrmntIndiSupplied, agrmntIndiValidated,
                                                                 atpcoReasCd, billingYear, 0, _pagesize, 0);

          // Format Confirmation detail model as per report requirement
          listModel = FormatDetailModel(listModel);
          var recordcount = 0;

          if (listModel != null)
          {
            if (listModel.Count > 0)
            {
              recordcount = listModel[0].TotalNumberOfRecords;
              Button1.Visible = true;
            }
            else
            {
              Button1.Visible = false;
            }
          }


          // Code for report export to CSV  format
          var totalPages = Math.Ceiling((decimal)recordcount / _pagesize);

          for (var pageNo = 1; pageNo <= totalPages; pageNo++)
          {
            ddlPaging.Items.Add(new ListItem(pageNo.ToString()));
          }


          ReportDocument orpt = new ReportDocument();
          orpt.Load(reportPath);

          orpt.SetDataSource(listModel);
          //changes to display search criteria on report
          orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
          orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);
          // Take an format id of pdf and excel report 
          int exportFormatFlags =
              (int)
              (CrystalDecisions.Shared.ViewerExportFormats.PdfFormat |
               CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);
          //int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat | CrystalDecisions.Shared.ViewerExportFormats.WordFormat | CrystalDecisions.Shared.ViewerExportFormats.CsvFormat);

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
          // Code for report export to CSV  format
          CRViewer.HasGotoPageButton = false;
          CRViewer.HasPageNavigationButtons = false;
          CRViewer.HasPrintButton = false;

          CRViewer.HasExportButton = false;


        //}
        //else
        //{
        //  if (!string.IsNullOrEmpty(ddlPaging.SelectedValue))
        //  {
        //    if (int.Parse(ddlPaging.SelectedValue) > 0)
        //      PageChange(int.Parse(ddlPaging.SelectedValue));
        //  }

        //}

      }
      catch (Exception exception)
      {
        _logger.Error("Unexpected Error Has Occurred", exception);
      }

    }


    private void GetQueryStringValues()
    {

      billingYear = Convert.ToInt32(Request.QueryString["bYear"]);

      if (!String.IsNullOrEmpty(Request.QueryString["cleMonth"]))
        clearanceMonth = Convert.ToInt32(Request.QueryString["cleMonth"]);

      if (string.IsNullOrEmpty(Request.QueryString["perNo"]))
      {
        periodNo = 0;
      }
      else
      {
        periodNo = Convert.ToInt32(Request.QueryString["perNo"]);
      }

      if (!String.IsNullOrEmpty(Request.QueryString["billiAirNumber"]))
        biligAirlineNo = Convert.ToInt32(Request.QueryString["billiAirNumber"]);
      if (!String.IsNullOrEmpty(Request.QueryString["billdAirNumber"]))
        biledAirlineNo = Convert.ToInt32(Request.QueryString["billdAirNumber"]);
      if (!String.IsNullOrEmpty(Request.QueryString["invNumber"]))
        invoiceNo = Request.QueryString["invNumber"];
      if (!String.IsNullOrEmpty(Request.QueryString["issAirNumber"]))
        issuAirline = Request.QueryString["issAirNumber"];
      if (!String.IsNullOrEmpty(Request.QueryString["origPMI"]))
        originalPMI = Request.QueryString["origPMI"];
      if (!String.IsNullOrEmpty(Request.QueryString["validPMI"]))
        validatedPMI = Request.QueryString["validPMI"];
      if (!String.IsNullOrEmpty(Request.QueryString["agreIndSupplied"]))
        agrmntIndiSupplied = Request.QueryString["agreIndSupplied"];
      if (!String.IsNullOrEmpty(Request.QueryString["agreIndValidated"]))
        agrmntIndiValidated = Request.QueryString["agreIndValidated"];
      if (!String.IsNullOrEmpty(Request.QueryString["atpcoReasCode"]))
        atpcoReasCd = Request.QueryString["atpcoReasCode"];

      memberId = Convert.ToInt32(Request.QueryString["memId"]);

    }

    private static List<ConfirmationDetailModel> FormatDetailModel(IEnumerable<ConfirmationDetailModel> confirmationDetailModel)
    {


      foreach (ConfirmationDetailModel t in confirmationDetailModel)
      {
        t.BilledAmountUSD =
            Convert.ToDouble(Math.Abs(t.BilledAmountUSD).ToString("N2").Replace(
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

        t.ProrateAmountasperATPCO =
            Convert.ToDouble(Math.Abs(t.ProrateAmountasperATPCO).ToString("N2").Replace(
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

        t.BilledTotalTaxAmountUSD =
            Convert.ToDouble(Math.Abs(t.BilledTotalTaxAmountUSD).ToString("N2").Replace(
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

        t.TotalTaxAmountasperATPCO =
            Convert.ToDouble(Math.Abs(t.TotalTaxAmountasperATPCO).ToString("N2").Replace(
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

        t.BilledISCPer =
            Convert.ToDouble(Math.Abs(t.BilledISCPer).ToString("N3").Replace(
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

        t.ISCFeePer =
            Convert.ToDouble(Math.Abs(t.ISCFeePer).ToString("N3").Replace(
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

        t.BilledHandlingFeeAmountUSD =
            Convert.ToDouble(Math.Abs(t.BilledHandlingFeeAmountUSD).ToString("N2").Replace(
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

        t.HandlingFeeAmount =
            Convert.ToDouble(Math.Abs(t.HandlingFeeAmount).ToString("N2").Replace(
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

        t.BilledUATPPercentage =
            Convert.ToDouble(Math.Abs(ConvertUtil.Round(t.BilledUATPPercentage, 2)).ToString("N2").Replace(
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

        t.UATPPercentage =
            Convert.ToDouble(Math.Abs(ConvertUtil.Round(t.UATPPercentage, 2)).ToString("N2").Replace(
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty));

        t.MonthOfSales = t.MonthofSale == 0 ? string.Empty : Convert.ToString(t.MonthofSale);
        t.YearofSales = t.YearofSale == 0 ? string.Empty : Convert.ToString(t.YearofSale);
      }


      return confirmationDetailModel.ToList();

    }


    // Code for report export to CSV  format

    protected void ddlPaging_SelectedIndexChanged(object sender, EventArgs e)
    {
      var pager = (DropDownList)sender;

      //  PageChange(Convert.ToInt32(pager.SelectedValue));
    }

    private void PageChange(int currentPageNo)
    {
      var listModel = (List<ConfirmationDetailModel>)Cache[SessionUtil.SessionId];
      List<ConfirmationDetailModel> filteredList;
      new List<ConfirmationDetailModel>();
      GetQueryStringValues();

      if (currentPageNo == 1)
      {
        filteredList = _confirmationDetail.GetConfirmationDetails(memberId, clearanceMonth, periodNo, biligAirlineNo,
                                                                    biledAirlineNo, invoiceNo, issuAirline, originalPMI,
                                                                    validatedPMI, agrmntIndiSupplied, agrmntIndiValidated,
                                                                    atpcoReasCd, billingYear, 0, _pagesize, 0);
     
      }
      else
      {
        var pageStartEndIndex = 0;
        var pageEndIndex = 0;

        pageStartEndIndex = ((currentPageNo - 1) * _pagesize) + 1;

        pageEndIndex = pageStartEndIndex + _pagesize;


        filteredList = _confirmationDetail.GetConfirmationDetails(memberId, clearanceMonth, periodNo, biligAirlineNo,
                                                                    biledAirlineNo, invoiceNo, issuAirline, originalPMI,
                                                                    validatedPMI, agrmntIndiSupplied, agrmntIndiValidated,
                                                                    atpcoReasCd, billingYear, pageStartEndIndex, pageEndIndex, 1);
      }


      // Format Confirmation detail model as per report requirement
      filteredList = FormatDetailModel(filteredList);



      reportPath = MapPath(_reportUrl);
      var orpt = new ReportDocument();
      orpt.Load(reportPath);
      //changes to display search criteria on report
      orpt.SetParameterValue("SearchCriteria", Request.QueryString["SearchCriteria"]);
      orpt.SetParameterValue("BrowserDateTime", Request.QueryString["BrowserDateTime"]);

      orpt.SetDataSource(filteredList);
      CRViewer.ReportSource = orpt;

      CRViewer.HasGotoPageButton = false;
      CRViewer.HasPageNavigationButtons = false;
      CRViewer.HasPrintButton = false;


      CRViewer.HasExportButton = false;



    }

    protected void CRViewer_Navigate(object source, CrystalDecisions.Web.NavigateEventArgs e)
    {
      var currentPageNo = e.CurrentPageNumber;
      PageChange(currentPageNo);
    }


    // Code for report export to CSV  format
    protected void Button1_Click(object sender, EventArgs e)
    {
      var tempFolderPath = @"C:\TempDownloadFolder";
      // Create Temp Folder in C :\ to copy network files 
      if (!Directory.Exists(tempFolderPath))
      {
        Directory.CreateDirectory(tempFolderPath);
      }

      var reportFileName = tempFolderPath + @"\PAX-BVCReport" + DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss") + ".csv";

      if (File.Exists(reportFileName))
      {
        File.Delete(reportFileName);
      }


      var filteredList = _confirmationDetail.GetConfirmationDetails(memberId, clearanceMonth, periodNo, biligAirlineNo,
                                                                       biledAirlineNo, invoiceNo, issuAirline, originalPMI,
                                                                       validatedPMI, agrmntIndiSupplied, agrmntIndiValidated,
                                                                       atpcoReasCd, billingYear, -1, -1, 1);


      var csvListModel = new List<CsvConfirmationDetailModel>();

      if (filteredList != null)
      {

        foreach (var confirmationDetailModel in filteredList)
        {
          var csvcomfimationobject = new CsvConfirmationDetailModel
          {
            BillingAirline = confirmationDetailModel.BillingAirline,
            BillingAirlineNumber = confirmationDetailModel.BillingAirlineNumber,
            BillingPeriod = confirmationDetailModel.BillingPeriod,
            ClearanceMonth = confirmationDetailModel.ClearanceMonth,
            ClearanceYear = confirmationDetailModel.ClearanceYear,
            InvoiceNumber = confirmationDetailModel.InvoiceNumber,
            BilledAirline = confirmationDetailModel.BilledAirline,
            BilledAirlineNumber = confirmationDetailModel.BilledAirlineNumber,
            MonthOfSales = confirmationDetailModel.MonthOfSales,
            YearofSales = confirmationDetailModel.YearofSales,
            IssuingAirline = confirmationDetailModel.IssuingAirline,
            DocumentNumber = confirmationDetailModel.DocumentNumber,
            CouponNumber = confirmationDetailModel.CouponNumber,
            OriginalPMI = confirmationDetailModel.OriginalPMI,
            ValidatedPMI = confirmationDetailModel.ValidatedPMI,
            AgreementIndicatorSupplied = confirmationDetailModel.AgreementIndicatorSupplied,
            AgreementIndicatorValidated = confirmationDetailModel.AgreementIndicatorValidated,
            ProrateMethodologySupplied = confirmationDetailModel.ProrateMethodologySupplied,
            ProrateMethodologyValidated = confirmationDetailModel.ProrateMethodologyValidated,
            NFPReasonCodeSupplied = confirmationDetailModel.NFPReasonCodeSupplied,
            NFPReasonCodeValidated = confirmationDetailModel.NFPReasonCodeValidated,
            BilledAmountUSD = confirmationDetailModel.BilledAmountUSD,
            ProrateAmountasperATPCO = confirmationDetailModel.ProrateAmountasperATPCO,
            ProrateAmountBaseSupplied = confirmationDetailModel.ProrateAmountBaseSupplied,
            ProrateAmountBaseATPCO = confirmationDetailModel.ProrateAmountBaseATPCO,
            BilledTotalTaxAmountUSD = confirmationDetailModel.BilledTotalTaxAmountUSD,
            TotalTaxAmountasperATPCO = confirmationDetailModel.TotalTaxAmountasperATPCO,
            PublishedTaxAmountCurrency1 = confirmationDetailModel.PublishedTaxAmountCurrency1,
            PublishedTaxAmountCurrency2 = confirmationDetailModel.PublishedTaxAmountCurrency2,
            PublishedTaxAmountCurrency3 = confirmationDetailModel.PublishedTaxAmountCurrency3,
            PublishedTaxAmountCurrency4 = confirmationDetailModel.PublishedTaxAmountCurrency4,
            BilledISCPer = confirmationDetailModel.BilledISCPer,
            ISCFeePer = confirmationDetailModel.ISCFeePer,
            BilledHandlingFeeAmountUSD = confirmationDetailModel.BilledHandlingFeeAmountUSD,
            BilledUATPPercentage = confirmationDetailModel.BilledUATPPercentage,
            UATPPercentage = confirmationDetailModel.UATPPercentage,
            ATPCOReasonCode = confirmationDetailModel.ATPCOReasonCode

          };

          csvListModel.Add(csvcomfimationobject);
        }





      }
      var couponSpecialRecords = new List<SpecialRecord>();
      CsvProcessor.GenerateCsvReport(csvListModel,
                                     reportFileName,
                                     couponSpecialRecords);


      if (File.Exists(reportFileName))
      {
        Context.Response.Buffer = true;
        Context.Response.Clear();
        Context.Response.AddHeader("content-disposition", "attachment; filename=" + Path.GetFileName(reportFileName));
        Context.Response.ContentType = "octet/stream";
        Context.Response.WriteFile(reportFileName);
        Context.Response.End();
      }
      if (!string.IsNullOrEmpty(ddlPaging.SelectedValue))
      {
        if (int.Parse(ddlPaging.SelectedValue) > 0)
          PageChange(int.Parse(ddlPaging.SelectedValue));
      }
    }


  }
}




