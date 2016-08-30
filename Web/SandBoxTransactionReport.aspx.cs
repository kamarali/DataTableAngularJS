using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Iata.IS.Business.Sandbox;
using Iata.IS.Business.Sandbox.Impl;
using Iata.IS.Core.DI;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.SandBox;
using Iata.IS.Business.Reports.SandBoxTransaction;
using System.Data;
using System.Reflection;
using CrystalDecisions.CrystalReports.Engine;
using Iata.IS.Web.Util;
using log4net;

namespace Iata.IS.Web
{
  public partial class SandBoxTransactionReport : System.Web.UI.Page
  {
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private const string whiteSpace = "_ _ _";
    public readonly ISandBoxTransaction _isandBoxTransaction;



    public SandBoxTransactionReport()
    {
      _isandBoxTransaction = Ioc.Resolve<ISandBoxTransaction>(typeof(ISandBoxTransaction));
    }
    protected void Page_Load(object sender, EventArgs e)
    {
      List<CertificationTransactionDetailsReport> listModel = null;
      //Fetch fromdate, todate from querystring

      try
      {
        _logger.Info("Receivables Report Initiated");


        int memberId = 0;
        int fileFormatId = 0;
        int billingCategoryId = 0;
        int transactionGroupId = 0;
        string requestType = string.Empty;


        //Log in into logger 	
        _logger.Info("Report Initiated");

        // SIS_SCR_REPORT_23_jun-2016_2 :Cross_Site_History_Manipulation
        if (!User.Identity.IsAuthenticated && SessionUtil.UserCategory != UserCategory.SisOps)
        {
            Response.Redirect("/Home/Error");
        }
        
        if (IsPostBack)
        {

        }
        DateTime fromDate = Convert.ToDateTime(Request.QueryString["fdate"]);
        DateTime toDate = Convert.ToDateTime(Request.QueryString["tdate"]);
        toDate = toDate.AddDays(1);
        //if (!String.IsNullOrEmpty(Request.QueryString["fileFormatId"]))                
        //fileFormatId = Convert.ToInt32(Request.QueryString["fileFormatId"]);)




        if (!string.IsNullOrEmpty(Request.QueryString["memberId"]))
          memberId = Convert.ToInt32(Request.QueryString["memberId"]);
        if (!string.IsNullOrEmpty(Request.QueryString["fileFormatId"]))
          fileFormatId = Convert.ToInt32(Request.QueryString["fileFormatId"]);
        if (!string.IsNullOrEmpty(Request.QueryString["billingCategoryId"]))
        {
          billingCategoryId = Convert.ToInt32(Request.QueryString["billingCategoryId"]);

          if (billingCategoryId == -1)
          {
            billingCategoryId = 0;
          }
        }
        if (!String.IsNullOrEmpty(Request.QueryString["requestType"]))
        {
          requestType = Request.QueryString["requestType"];
          if (requestType == "0")
          {
            requestType = string.Empty;
          }

          if (requestType == "1")
          {
            requestType = "ST";
          }
          if (requestType == "2")
          {
            requestType = "CT";
          }

        }

        if (!string.IsNullOrEmpty(Request.QueryString["transactionGroupId"]))
          transactionGroupId = Convert.ToInt32(Request.QueryString["transactionGroupId"]);

        if (!string.IsNullOrEmpty(Request.QueryString["transactionGroupId"]))
          transactionGroupId = Convert.ToInt32(Request.QueryString["transactionGroupId"]);
          string reportPath = string.Empty;
        try
        {
            reportPath = MapPath("~/Reports/SandBoxTransaction/SandBoxTransactionDetail.rpt");
        }
        catch (Exception exception)
        {
            _logger.Error("Unexpected Error Has Occurred", exception);
        }

        //Log in into logger
        _logger.Info("SandBoxTransactionReport : Business Layer being called for Data!");

        listModel = _isandBoxTransaction.GetSandBoxTransationData(memberId, fromDate, toDate, fileFormatId, billingCategoryId, requestType, transactionGroupId);
















        if (listModel != null)
        {
          foreach (var lModel in listModel)
          {
            lModel.MemberName = lModel.MemberCodeAlpha + "-" + lModel.MemberCodeNumeric + "-" +
                               lModel.MemberCommercialName;
          }
        }
        int index = 1;
        for (int i = 0; i < listModel.Count; i++)
        {
          listModel[i].SerialNo = index;

          index++;
        }
        DataTable dt = ListToDataTable(listModel);

        foreach (DataRow row in dt.Rows)
        {
          if (row.IsNull("MemberName")) row["MemberName"] = whiteSpace;

          if (row.IsNull("FileSubmittedDate")) row["FileSubmittedDate"] = whiteSpace;

          if (row.IsNull("FileName")) row["FileName"] = whiteSpace;

          if (row.IsNull("RequestType")) row["RequestType"] = whiteSpace;



          if (row.IsNull("FileStatus")) row["FileStatus"] = whiteSpace;



          if (row.IsNull("TransactionGroupName")) row["TransactionGroupName"] = whiteSpace;

          if (row.IsNull("MinTransactionCount")) row["MinTransactionCount"] = whiteSpace;

          if (row.IsNull("TransactionSubmittedCount")) row["TransactionSubmittedCount"] = whiteSpace;

          if (row.IsNull("TransactionGroupStatus")) row["TransactionGroupStatus"] = whiteSpace;

          if (row.IsNull("TransactionSubType1Label")) row["TransactionSubType1Label"] = whiteSpace;

          if (row.IsNull("TransactionSubType1MinCount")) row["TransactionSubType1MinCount"] = whiteSpace;

          if (row.IsNull("TransactionSubType1RecivedCount")) row["TransactionSubType1RecivedCount"] = whiteSpace;

          if (row.IsNull("TransactionSubType1Status")) row["TransactionSubType1Status"] = whiteSpace;

          if (row.IsNull("TransactionSubType2Label")) row["TransactionSubType2Label"] = whiteSpace;

          if (row.IsNull("TransactionSubType2MinCount")) row["TransactionSubType2MinCount"] = whiteSpace;

          if (row.IsNull("TransactionSubType2ReceivedCount")) row["TransactionSubType2ReceivedCount"] = whiteSpace;

          if (row.IsNull("TransactionSubType2Status")) row["TransactionSubType2Status"] = whiteSpace;

          if (row.IsNull("TransactionSubType3Label")) row["TransactionSubType3Label"] = whiteSpace;

          if (row.IsNull("TransactionSubType3MinCount")) row["TransactionSubType3MinCount"] = whiteSpace;

          if (row.IsNull("TransactionSubType3ReceivedCount")) row["TransactionSubType3ReceivedCount"] = whiteSpace;

          if (row.IsNull("TransactionSubType3Status")) row["TransactionSubType3Status"] = whiteSpace;

          if (row.IsNull("TransactionSubType4Label")) row["TransactionSubType4Label"] = whiteSpace;

          if (row.IsNull("TransactionSubType4MinCount")) row["TransactionSubType4MinCount"] = whiteSpace;

          if (row.IsNull("TransactionSubType4ReceivedCount")) row["TransactionSubType4ReceivedCount"] = whiteSpace;

          if (row.IsNull("TransactionSubType4Status")) row["TransactionSubType4Status"] = whiteSpace;

          if (row.IsNull("TransactionSubType5Label")) row["TransactionSubType5Label"] = whiteSpace;

          if (row.IsNull("TransactionSubType5MinCount")) row["TransactionSubType5MinCount"] = whiteSpace;

          if (row.IsNull("TransactionSubType5ReceivedCount")) row["TransactionSubType5ReceivedCount"] = whiteSpace;

          if (row.IsNull("TransactionSubType5Status")) row["TransactionSubType5Status"] = whiteSpace;

          if (row.IsNull("TransactionSubType6Label")) row["TransactionSubType6Label"] = whiteSpace;

          if (row.IsNull("TransactionSubType6MinCount")) row["TransactionSubType6MinCount"] = whiteSpace;

          if (row.IsNull("TransactionSubType6ReceivedCount")) row["TransactionSubType6ReceivedCount"] = whiteSpace;

          if (row.IsNull("TransactionSubType6Status")) row["TransactionSubType6Status"] = whiteSpace;
        }

        ReportDocument orpt = new ReportDocument();



        //Log in into logger
        _logger.Info("SandBoxTransactionReport : Report(rpt) being loaded with data!");

        orpt.Load(reportPath);
        orpt.SetDataSource(dt);


        // Take an format id of pdf and excel report 
        int exportFormatFlags = (int)(CrystalDecisions.Shared.ViewerExportFormats.PdfFormat | CrystalDecisions.Shared.ViewerExportFormats.ExcelRecordFormat);




        // Only shows the padf and export to data option in report and supress other options
        transactionDetailsCrystal.AllowedExportFormats = exportFormatFlags;
        transactionDetailsCrystal.ReportSource = orpt;

        //Log in into logger
        _logger.Info("SandBoxTransactionReport : Report generation completed!");
      }
      catch (Exception exception)
      {
        _logger.Error("Unexpected Error Has Occurred", exception);
      }


    }
    public static DataTable ListToDataTable(List<CertificationTransactionDetailsReport> list)
    {
      DataTable dt = new DataTable();
      foreach (PropertyInfo info in typeof(CertificationTransactionDetailsReport).GetProperties())
      {
        dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
      }
      foreach (CertificationTransactionDetailsReport t in list)
      {
        DataRow row = dt.NewRow();
        foreach (PropertyInfo info in typeof(CertificationTransactionDetailsReport).GetProperties())
        {
          row[info.Name] = info.GetValue(t, null);
        }

        dt.Rows.Add(row);
      }
      return dt;
    }
  }
}
