namespace Iata.IS.Web.Reports.ProcessingDashBoard
{
  public class RechargeDataReport
  {
    public string YearAndMonth { get; set; }
    public int PeriodNo { get; set;}
    public string AlphaCode { get; set;}
    public string NumericCode { get; set;}
    public string ISSubmission { get; set;}
    public int PAXInvoices { get; set;}
    public int CGOInvoices { get; set;}
    public int MISCInvoices { get; set;}
    public int UATPInvoices { get; set;}
    public int PAXPrimeCoupons { get; set;}
    public int PAXBillingMemos { get; set;}
    public int PAXCreditMemos { get; set;}
    public int PAXSamplingProvInvoiceCoupons { get; set;}
    public int PAXSamplingDigitEvalCoupons { get; set;}
    public int PAXAutoBillingRequests { get; set;}
    public int CGOOriginalBillingAWBS { get; set;}
    public int CGOBillingMemos { get; set;}
    public int CGOCreditMemos { get; set;}
    public int PAXRejectionMemosInclSampling { get; set;}
    public int PAXSamplingUAFCoupons { get; set;}
    public int PAXCorrespondences { get; set;}
    public int CGORejectionMemos { get; set;}
    public int CGOCorrespondences { get; set;}
    public int MISCCorrespondences { get; set;}
    public int SupportingDocsPAX { get; set;}
    public int SupportingDocsCGO { get; set;}
    public int SupportingDocsMISC { get; set;}
    public int SupportingDocsUATP { get; set;}
    public int DigiSignValidPAXSent { get; set;}
    public int DigiSignValidCGOSent { get; set;}
    public int DigiSignValidMISCSent { get; set;}
    public int DigiSignValidUATPSent { get; set;}
    public int DigiSignValidPAXRcvd { get; set;}
    public int DigiSignValidCGORcvd { get; set;}
    public int DigiSignValidMISCRcvd { get; set;}
    public int DigiSignValidUATPRcvd { get; set;}
    public int EArchivingPAX { get; set;}
    public int EArchivingCGO { get; set;}
    public int EArchivingMISC { get; set;}
    public int EArchivingUATP { get; set; }
    public string MemberCode { get; set; }
    public string PerticipantType { get; set; }

    /// <summary>
    /// property to get and set time on report
    /// </summary>
    public string ReportGeneratedDate { get; set; }
  }
}
