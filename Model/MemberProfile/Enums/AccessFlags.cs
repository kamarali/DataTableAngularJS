using System;

namespace Iata.IS.Model.MemberProfile.Enums
{
  [Flags]
  public enum AccessFlags
  {
    None = 0,
    Member = 1,
    SisOps = 2,
    IchOps = 4,
    AchOps = 8,
    All = Member | SisOps | IchOps | AchOps
  }
}