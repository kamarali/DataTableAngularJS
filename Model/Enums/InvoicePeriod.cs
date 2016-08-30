namespace Iata.IS.Model.Enums
{
  /// <summary>
  /// The different billing periods in a billing month.
  /// </summary>
  
  public enum InvoicePeriod
  {
    None = 0,

    /// <summary>
    /// Period 1 of a billing month.
    /// </summary>
    Period1 = 1,
    
    /// <summary>
    /// Period 2 of a billing month.
    /// </summary>
    Period2 = 2,

    /// <summary>
    /// Period 3 of a billing month.
    /// </summary>
    Period3 = 3,

    /// <summary>
    /// Period 4 of a billing month.
    /// </summary>
    Period4 = 4
  }
}