namespace Iata.IS.Model.Enums
{
  /// <summary>
  /// Invoice status values.
  /// </summary>
  public enum InvoiceStatusType
  {
    Open = 1,
    ReadyForSubmission = 2,
    ReadyForBilling = 3,
    Claimed = 4,
    ProcessingComplete = 5,
    Presented = 6,
    ValidationError = 7,
    ErrorCorrectable = 8,
    ErrorNonCorrectable = 9,
    OnHold = 10,
    ValidationCompleted = 11,
    FutureSubmitted = 12
  }
}
