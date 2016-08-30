namespace Iata.IS.Model.Pax.Enums
{
  /// <summary>
  /// Resubmission Flag enum.
  /// </summary>
  public enum ResubmissionStatus
  {
    /// <summary>
    /// None
    /// </summary>
    /// <remarks>
    /// Represents that status is not set.
    /// </remarks>
    NotSet = 0,

    /// <summary>
    /// R - RESUBMITTED, SETTLEMENT PENDING.
    /// </summary>
    R = 1,

    /// <summary>
    /// B - BILATERALLY SETTLED.
    /// </summary>
    B = 2,
    
    /// <summary>
    /// To indicate that the submission to the CH has been completed.
    /// C - RESUBMITTED, SETTLEMENT SUCCESSFUL
    /// </summary>
    C = 3,
    
    /// <summary>
    /// SCP#470044 - KAL: Resubmitted invoice with status Ignored
    /// F - RESUBMITTED, SETTLEMENT FAILED.
    /// </summary>
    F = 4
  }
}
