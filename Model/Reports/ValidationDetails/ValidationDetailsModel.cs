using System;

namespace Iata.IS.Model.Reports.ValidationDetails
{
  public class ValidationDetailsModel
  {

    public int SerialNo { get; set;}

    public string BillingEntityCode { get; set; }

    public int ClearanceMonth { get; set;}

    public int BillingPeriod { get; set; }

    public string BillingCategory { get; set; }

    public string BillingFileName { get; set; }

    public DateTime BillingFileSubmissionDate { get; set; }

    public string SubmissionFormat { get; set;}

    public string BilledEntityCode { get; set; }

    public string InvoiceNo { get; set;}

    public string ChargeCategoryAndBillingCode { get; set; }

    public int PaxSourceCode { get; set; }

    public int LineItemAndBatchNo { get; set; }

    public int LineItemAndSeqNo { get; set; }

    public Int64  DocNo { get; set; }

    public Int64 LinkedDocNo { get; set; }

    public string ErrorCode { get; set; }

    public string ErrorLavel { get; set; }

    public string FieldName {get; set;}

    public string FieldValue {get; set;}

    public string ErrorDescription {get; set;}

    public string ErrorStatus { get; set; }

    public DateTime Clearance { get; set; }

    public Int64 Doc { get; set; }

    public Int64 LinkedDoc { get; set; }

    /// <summary>
    /// property to get and set time on report
    /// </summary>
    public string ReportGeneratedDate { get { return string.Format("{0:dd-MMM-yyyy HH:mm:ss tt} {1}", DateTime.UtcNow, "UTC"); } }

  }
}
