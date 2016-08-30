namespace Iata.IS.Model.MemberProfile.Enums
{
  public enum IchMemberShipStatus
  {
    /// <summary>
    /// Indicates an operation ICH member.
    /// </summary>
    Live = 1,

    /// <summary>
    /// Represents a suspended ICH member.
    /// </summary>
    Suspended = 2,

    /// <summary>
    /// Represents a terminated ICH member.
    /// </summary>
    Terminated = 3,

    /// <summary>
    /// Indicates not an ICH member.
    /// </summary>
    NotAMember = 4

  };
}
