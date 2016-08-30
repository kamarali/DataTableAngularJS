using System;

namespace Iata.IS.Model.MiscUatp
{
  public class NavigationDetails
  {
    public Guid FirstId { get; set; }
    public Guid LastId { get; set; }
    public Guid NextId { get; set; }
    public Guid PreviousId { get; set; }
  }
}
