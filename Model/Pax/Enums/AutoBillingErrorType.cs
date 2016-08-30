namespace Iata.IS.Model.Pax.Enums
{
  public enum AutoBillingErrorType
  {
    NoSalesRecordFound = 1,
    MissingConcurrence = 2,
    SegmentDataDidNotMatch = 3,
    IncorrectFormat = 4,
    DuplicateSalesRecordFound = 5,
    IncompleteRecord6ReceivedFromARCCompass = 6,
    FilterError = 7,
    NoProrateValuesFound = 8,
    BadIndustryProrateValuesFound = 9
  }
}
