using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  /// <summary>
  /// Lightweight User class created to be used with Invoice modules.
  /// </summary>
  public class User : EntityBase<int>
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    //public new decimal LastUpdatedBy { get; set; }
  }
}