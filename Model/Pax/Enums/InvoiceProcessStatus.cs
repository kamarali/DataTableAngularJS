namespace Iata.IS.Model.Pax.Enums
{
  /// <summary>
  /// The status of various processing that is done on a invoice. E.g. Digital Signature, Value Confirmation, etc.
  /// </summary>
  public enum InvoiceProcessStatus
  {
      
      /// <summary>
      /// None
      /// </summary>
      /// <remarks>
      /// Represents that status is not set.
      /// </remarks>
      NotSet = 0,

    /// <summary>
    /// Processing is pending.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Processing failed.
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Processing successful.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Processing.
    /// </summary>
    InProgress = 4,

    

    /// <summary>
    /// Not Required.
    /// </summary>
    NotRequired = 5

    
  };
}
