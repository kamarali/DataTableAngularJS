using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using System.Linq;
namespace Iata.IS.Data
{
  public interface IMiscCodeRepository : IRepository<MiscCode>
  {
    /// <summary>
    /// Gets the misc codes.
    /// </summary>
    /// <param name="miscGroup"></param>
    /// <returns></returns>
    IList<MiscCode> GetMiscCodes(MiscGroups miscGroup);

    /// <summary>
    /// Gets all misc codes.
    /// </summary>
    /// <returns></returns>
    IQueryable<MiscCode> GetAllMiscCodes();

    /// <summary>
    /// Gets the misc code.
    /// </summary>
    /// <param name="miscGroup"></param>
    /// <param name="miscCodeName"></param>
    /// <returns></returns>
    MiscCode GetMiscCode(MiscGroups miscGroup, string miscCodeName);

    /// <summary>
    /// Get UOM Code type list
    /// </summary>
    /// <returns></returns>
      IList<MiscCode> GetUomCodeTypeList();
  }
}