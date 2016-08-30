using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class ElementGroup: MasterBase<int>
  {
    public string Group { get; set; }

    public string TableName { get; set; }
  }
}
