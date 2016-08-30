namespace Iata.IS.Model.Enums
{
  public enum DigitalSignatureStatus
  {
        /// <summary>
    /// Required but Not Requested
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Processing is pending.
    /// </summary>
    Requested = 1,

    /// <summary>
    /// Processing.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Processing successful.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Not Required.
    /// </summary>
    NotRequired = 4,


  }
}
