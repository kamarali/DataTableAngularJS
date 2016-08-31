namespace Iata.IS.Business
{
  public struct ICHInterfaceErrorCodes
  {
    /// <summary>
    /// Used when billing period passed in cross check or resend invoice request is empty or has invalid  request is empty
    /// </summary>
    public const string InvalidBillingPeriodinResendRequest = "ICHERR_10200";

    /// <summary>
    /// Used when billing period passed in cross check or resend invoice request is empty or has invalid  request is empty
    /// </summary>
    public const string InvalidBillingPeriodinCrossCheckRequest = "ICHERR_10100";

    /// <summary>
    /// Used when start date time passed in cross check or resend invoice request is empty or has invalid value.
    /// </summary>
    public const string InvalidStartDateTimeinCrossCheckRequest = "ICHERR_10101";

    /// <summary>
    /// Used when end date time passed in cross check or resend invoice request is empty or has invalid value.
    /// </summary>
    public const string InvalidEndDateTimeinCrossCheckRequest = "ICHERR_10102";

    /// <summary>
    /// Used when start date time is greater than end date time passed in cross check request.
    /// </summary>
    public const string InvalidStartEndDateTimeinCrossCheckRequest = "ICHERR_10103";

    /// <summary>
    /// Used when invoice numbers passed in resend invoice request are invalid
    /// </summary>
    public const string InvalidInvoiceNumbers = "ICHERR_10201";

    /// <summary>
    /// Used when error occurs while processing resend request
    /// </summary>
    public const string ResendRequestProcessingError = "ICHERR_10202";

    /// <summary>
    /// Used when no data is present for resend
    /// </summary>
    public const string NoInvoiceDataPresentforResend = "ICHERR_10203";

    /// <summary>
    /// Used when error occurs while processing cross check request
    /// </summary>
    public const string CrossCheckRequestProcessingError = "ICHERR_10204";

    /// <summary>
    /// Used when no data is present in response to cross check request
    /// </summary>
    public const string NoInvoiceDataPresentForCrossCheckRequest = "ICHERR_10205";

    /// <summary>
    /// Used when invoices corresponding to invoice ids passed in resend request do not belong to current or previous billing period
    /// </summary>
    public const string InvalidBillingPeriodInvoices = "ICHERR_10206";
  }
}



//Errorcode when credentials cannot be validated
//Errorcode when billingperiod is null
//Errorcode when billingperiod is invalid
//Errorcode when processing resend request-10202
//Errorcode when there is no invoice data present - 10203