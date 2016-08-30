namespace Iata.IS.Model.Enums
{
  public enum InvoiceValidationStatus
  {
    /// <summary>
    /// Processing is pending.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Validation Processing Failed.
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Validation Process is in Progress.
    /// </summary>
    InProgress = 3,

    /// <summary>
    /// Validation Processing has Error in Period.
    /// </summary>
    ErrorPeriod = 4,

    /// <summary>
    /// Processing Completed
    /// </summary>
    Completed = 5,


     /// <summary>
    /// Future submission
    /// </summary>
    FutureSubmission = 6
  }
}
