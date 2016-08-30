namespace Iata.IS.Model.Enums
{
  /// <summary>
  /// Stages of rejection.
  /// </summary>
  public enum RejectionStage
  {
    /// <summary>
    /// First stage of rejection.
    /// </summary>
    StageOne = 1,

    /// <summary>
    /// Second stage of rejection.
    /// </summary>
    StageTwo = 2,

    /// <summary>
    /// Third stage rejection.
    /// </summary>
    StageThree = 3
  }

  /// <summary>
  /// CMP#678:Time Limit Validation on Last Stage MISC Rejections
  /// </summary>
  public enum RmValidationType
  {
      //Validate Error Correction screen Rejection Invoice.
      ErrorCorrectionScreen = 1,
      //Validate IsWebStandAlone Rejection
      IsWebStandAlone = 2,
      //Validate Input File Rejection
      InputFile = 3,
      //Validate Billing History
      BillingHistory = 4,
      //IsWebPayableInvoice.
      IsWebPayableInvoice = 5,
      //None
      None = 6
  }
}
