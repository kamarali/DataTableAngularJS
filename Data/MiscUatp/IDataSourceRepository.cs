using System.Collections.Generic;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Data.MiscUatp
{
  public interface IDataSourceRepository
  {
    /// <summary>
    /// Get list of dictionary based values for field of type dropdown 
    /// </summary>
    /// <param name="dataSourceId"></param>
    /// <returns></returns>
    IList<DropdownDataValue> GetDataSourceValues(int dataSourceId);
  }
}
