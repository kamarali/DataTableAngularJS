using System;
using System.Xml.Serialization;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
  /// <summary>
  /// Used to store the user category of the logged in user (only used for authorization).
  /// </summary>
  [Serializable]
  public class ProfileEntity : EntityBase<int> 
  {
    /// <summary>
    /// This field has been added so that the user category does not have to be fetched from the session each time it is required.
    /// </summary>
    [XmlIgnore]
    public Enums.UserCategory UserCategory { get; set; }
  }
}
