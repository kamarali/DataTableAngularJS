namespace Iata.IS.Model.MemberProfile.Enums
{
  /// <summary>
  /// Describes the various options for ACH membership status.
  /// </summary>
  public enum AchMembershipStatus
  {
    /// <summary>
    /// Indicates not an ACH member.
    /// </summary>
    NotAMember = 4,

    /// <summary>
    /// Indicates an operation ACH member.
    /// </summary>
    Live = 1,

    /// <summary>
    /// Represents a suspended ACH member.
    /// </summary>
    Suspended = 2,

    /// <summary>
    /// Represents a terminated ACH member.
    /// </summary>
    Terminated = 3
  }
}