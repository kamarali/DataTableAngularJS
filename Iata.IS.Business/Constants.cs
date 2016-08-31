namespace Iata.IS.Business
{
  public struct Constants
  {
    public const int SamplingConstantDecimalPlaces = 3;
    public const int PaxDecimalPlaces = 2;
    public const int MiscDecimalPlaces = 3;
    public const int CgoDecimalPlaces = 3;
    public const int ExchangeRateDecimalPlaces = 5;
    public const string SamplingIndicatorYes = "Y";
    public const string SamplingIndicatorNo = "N";
    public const int CurrencyConversionFactorDecimalPlaces = 12;
	public const string ReceivableRMBMCMSummaryReport = "ReceivableRMBMCMSummaryReport";
	public const string PayableRMBMCMSummaryReport = "PayableRMBMCMSummaryReport";
    //SCP#449343: Correction in system response for offline reports
    public const string OfflineReportMessage = "This report will be generated offline and be made available to you in CSV format after it has been created. You will be sent an email containing a link to the zipped CSV file. Alternatively, this report may also be downloaded from screen ‘Download Offline Reports’ after you receive the email notification.";

    /// <summary>
    /// CMP #533: RAM A13 New Validations and New Charge Code
    /// </summary>
    public const string ServiceProvider = "service provider";
    public const string Gds = "gds";
    //SCP207710 - Change Super User(Allow valid special character).
    public const string ValidEmailPattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
  }
}