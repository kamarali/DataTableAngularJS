using System;
using Iata.IS.Model.MemberProfile.Enums;

namespace Iata.IS.Model.MemberProfile
{
  public class ProfilePermissionAttribute : Attribute
  {
    public bool IsMandatory { get; set; }

    public bool IsFutureField { get; set; }

    public ControlType ControlType { get; set; }

    public AccessFlags ReadAccessFlags { get; set; }

    public AccessFlags WriteAccessFlags { get; set; }
  }
}