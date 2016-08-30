using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  [Serializable]
  public class SisMemberStatus : MasterBase<int>
  {
    /// <summary>
    /// Gets or sets the member status.
    /// </summary>
    /// <value>
    /// The member status.
    /// </value>
    public string MemberStatus { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    public string Description { get; set; }
  }
}
